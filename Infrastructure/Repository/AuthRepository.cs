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
    public class AuthRepository 
    {
        private readonly AlamutDbContext _context;

        public AuthRepository(AlamutDbContext context)
        {
            _context = context;
        }

        public async Task Add(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> Get(TokenRequest tokenRequest)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

        }



        public async Task Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

      

       

      
    }
}
