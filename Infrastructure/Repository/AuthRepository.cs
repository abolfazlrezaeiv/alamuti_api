using Alamuti.Application.Interfaces.repository;
using Alamuti.Infrastructure.Repository;
using application.DTOs.Requests;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AuthRepository : GenericRepository<RefreshToken> , IAuthRepository
    {
        private readonly AlamutDbContext _context;
        public AuthRepository(AlamutDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken> Get(TokenRequest tokenRequest) 
            => await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
    }
}
