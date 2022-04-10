using Alamuti.Application.Interfaces.repository;
using application.Interfaces.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamuti.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IAdvertisementRepository Advertisement { get; }
        IAdminRepository Admin { get; }
        IChatRepository Chat { get; }
        IAuthRepository Auth { get; }
        Task CompleteAsync();
        void Dispose();
    }
}
