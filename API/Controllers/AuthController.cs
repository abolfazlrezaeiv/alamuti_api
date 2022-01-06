using application;
using application.DTOs.Requests;
using application.DTOs.Responses;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly AuthRepository _authRepository;
        private readonly JwtConfig _JwtConfig;
        private readonly AlamutDbContext _context;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
                TokenValidationParameters tokenValidationParameters,
                AuthRepository authRepository,
                AlamutDbContext context)
        { 
            _context = context;
       

        _userManager = userManager;
            _tokenValidationParameters = tokenValidationParameters;
            _authRepository = authRepository;
            _JwtConfig = optionsMonitor.CurrentValue;
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if (user == null) return BadRequest();

            var generatedNumber = GenerateRandomNo();

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.PhoneNumber);

                var sms = new Ghasedak.Core.Api("47541ce5eed0c53032beda134b59199b7d7da859ef6c9c25de5b06f7a9de0798");

                var userphoneNumber = user.PhoneNumber;

                //await sms.SendSMSAsync($"کد فعالسازی {generatedNumber} می باشد\n الموتی ", userphoneNumber, "10008566");
                var client = new RestClient("https://api.ghasedak.me/v2/verification/send/simple");
                var request = new RestRequest(Method.POST);
                request.AddHeader("apikey", "47541ce5eed0c53032beda134b59199b7d7da859ef6c9c25de5b06f7a9de0798");
                request.AddParameter("receptor", $"{userphoneNumber}");
                request.AddParameter("type", 1);
                request.AddParameter("template", "alamut2");
                request.AddParameter("param1", $"{generatedNumber}"); 
             
               
                IRestResponse response = client.Execute(request);

                if (existingUser != null)
                {
                    existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser,generatedNumber.ToString());
                    await _userManager.UpdateAsync(existingUser);
                    return Ok(new { id = existingUser.Id, phonenumber = existingUser.UserName });
                }

                var newUser = new IdentityUser() {  UserName = user.PhoneNumber  };

                var isCreated = await _userManager.CreateAsync(newUser, generatedNumber.ToString());

                if (!isCreated.Succeeded)
                {
                    return BadRequest(new RegistrationResponseDto()
                    {
                        Errors = isCreated.Errors.Select(X => X.Description).ToList(),
                        Success = false,
                    });
                }
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


       
        [HttpPost("/login")]
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


        

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_JwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
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


            await _authRepository.Add(refreshToken);

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

       

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }




        [HttpPost]
        [Route("/RefreshToken")]
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

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
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
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

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
                var storedToken = await _authRepository.Get(tokenRequest);

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
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

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
               await _authRepository.Update(storedToken);

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

        private int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

    }
}
