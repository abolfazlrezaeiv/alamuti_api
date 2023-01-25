using Alamuti.Application.Interfaces.UnitOfWork;
using Alamuti.Domain.Entities;
using application;
using application.DTOs.Requests;
using application.DTOs.Responses;
using application.Interfaces;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IOTPSevice _otpService;
        private readonly JwtConfig _JwtConfig;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            TokenValidationParameters tokenValidationParameters,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork, IOTPSevice otpService)
        { 
            _userManager = userManager;
            _tokenValidationParameters = tokenValidationParameters;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _JwtConfig = optionsMonitor.CurrentValue;
        }



        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserRegistrationDto user)
        {
            if (user == null) return BadRequest();
            var generatedNumber = GenerateRandomNo();
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.PhoneNumber);
                var userphoneNumber = user.PhoneNumber;
                 _otpService.SendMessage(userphoneNumber, generatedNumber);
                if (existingUser != null)
                {
                    existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser,generatedNumber.ToString());
                    await _userManager.UpdateAsync(existingUser);
                    return Ok(new { id = existingUser.Id, phonenumber = existingUser.UserName });
                }
            IdentityUser newUser = new() {UserName = user.PhoneNumber};
                var isCreated = await _userManager.CreateAsync(newUser, generatedNumber.ToString());
                if (!isCreated.Succeeded)
                {
                    return BadRequest(new RegistrationResponseDto()
                    {
                        Errors = isCreated.Errors.Select(X => X.Description).ToList(),
                        Success = false,
                    });
                }
                await AddRoleToUser(newUser, "user");
                var createdUser = await _userManager.FindByNameAsync(user.PhoneNumber);
                return Ok(new { id = createdUser.Id , phonenumber = createdUser.UserName,});
            }
            else
            {
                return BadRequest(new RegistrationResponseDto()
                {
                    Errors = new List<string>() { "Invalid payload"},
                    Success=false
                });
            }
        }


        [HttpPost("authenticate-admin")]
        public async Task<IActionResult> AdminAuthenticate([FromBody] UserRegistrationDto admin)
        {
            var generatedNumber = GenerateRandomNo();
            if (admin == null) return BadRequest();
            var existingUser = await _userManager.FindByNameAsync(admin.PhoneNumber);
            var userphoneNumber = admin.PhoneNumber;
            _otpService.SendMessage(userphoneNumber, generatedNumber);

            if (existingUser != null)
            {
                existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, generatedNumber.ToString());
                await _userManager.UpdateAsync(existingUser);
                await AddRoleToUser(existingUser, "admin");
                return Ok(new { id = existingUser.Id, phonenumber = existingUser.UserName });
            }


        IdentityUser newUser = new()
            {
                UserName = admin.PhoneNumber
            };

            var isCreated = await _userManager.CreateAsync(newUser, generatedNumber.ToString());
            if (!isCreated.Succeeded)
                return BadRequest(new RegistrationResponseDto()
                {
                    Errors = isCreated.Errors.Select(X => X.Description).ToList(),
                    Success = false,
                });
            await AddRoleToUser(newUser,"admin");
            return Ok(new{ Status = "Success", Message = "User created successfully!" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.PhoneNumber);
                if (existingUser == null)
                {
                    return BadRequest(new RegistrationResponseDto()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid Login Request"
                        },
                        Success = false
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.VerificationCode);
                if (!isCorrect)
                {
                    return BadRequest(new RegistrationResponseDto()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid Login Request"
                        },
                        Success = false,
                    });
                }
                var result = await GenerateJwtToken(existingUser);
                return Ok(new {token = result.Token , refreshToken = result.RefreshToken,success = result.Success});
            }
                return BadRequest(new RegistrationResponseDto()
                {
                    Errors = new List<string>() { "Invalid payload" },
                    Success = false
                });
            }


        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(tokenRequest);
                if (result == null)
                {
                    return BadRequest(new RegistrationResponseDto()
                    {
                        Errors = new List<string>() {
                            "Invalid tokens"
                        },
                        Success = false
                    });
                }
                return Ok(result);
            }
            return BadRequest(new RegistrationResponseDto()
            {
                Errors = new List<string>() {
                    "Invalid payload"
                },
                Success = false
            });
        }

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_JwtConfig.Secret);
            var claims = new List<Claim>
                  {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                  };

            var userRoles = await _userManager.GetRolesAsync(user);
            AddRolesToClaims(claims, userRoles);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid(),
            };
            await _unitOfWork.Auth.Add(refreshToken);
            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }


        private async Task<AuthResult?> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                // Validation 1 - Validation JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
                _tokenValidationParameters.ValidateLifetime = true;
                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                // Validation 3 - validate expiry date
                long.TryParse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value, out var utcExpiryDate);
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }
                // validation 4 - validate existence of the token
                var storedToken = await _unitOfWork.Auth.Get(tokenRequest);
                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }
                // Validation 5 - validate if used
                if (storedToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevorked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }
                // Validation 7 - validate the id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }
                // update current token 
                storedToken.IsUsed = true;
                await _unitOfWork.Auth.Update(storedToken);
                await _unitOfWork.CompleteAsync();
                // Generate a new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };
                }
                else
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }
        }

        private async Task AddRoleToUser(IdentityUser user, string role)
        {
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            await _userManager.AddToRoleAsync(user, "User");
            if (role == "user")
                return;
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        private static void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }

        private static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }
