using application.DTOs.Requests;
using application.Interfaces.repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamuti.Application.Interfaces.repository
{
    public interface IAuthRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> Get(TokenRequest tokenRequest);
    }
}
