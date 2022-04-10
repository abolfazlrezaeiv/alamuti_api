using application.DTOs;
using application.DTOs.Advertisement;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace application.Interfaces.repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> All();
        Task<T> GetById(int id);
        Task<bool> Add(T entity);
        Task<bool> Delete(int id);
        Task<bool> Update(T entity);
    }
}
