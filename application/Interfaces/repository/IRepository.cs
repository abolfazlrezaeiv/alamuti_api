using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.Interfaces.repository
{
    public interface IRepository<IEntity> 
    {
        Task<IEnumerable<IEntity>> GetAll();
        Task<IEntity> Get(int id);
        Task Add(IEntity entity);
        Task Update(IEntity entity);
        Task Delete(IEntity entity);

    }
}
