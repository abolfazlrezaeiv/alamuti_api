using Alamuti.Application.Interfaces.repository;
using Alamuti.Application.Interfaces.UnitOfWork;
using Alamuti.Infrastructure.Repository;
using application.Interfaces.repository;
using Infrastructure.Data;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamuti.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AlamutDbContext _context;

        public IAdvertisementRepository Advertisement { get; private set; }
        public IAdminRepository Admin { get; private set; }
        public IChatRepository Chat { get; private set; }
        public IAuthRepository Auth { get; private set; }

        public UnitOfWork(AlamutDbContext context)
        {
            _context = context;
            Advertisement = new AdvertisementRepository(context);
            Admin = new AdminRepository(context);
            Chat = new ChatRepository(context);
            Auth = new AuthRepository(context);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
