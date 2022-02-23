using application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected void AddHeaderPagination<T>(PaginatedList<T> paginatedList)
        {
            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(GetMeta(paginatedList)));
        }

        protected static object GetMeta<T>(PaginatedList<T> paginatedList)
        {
            return new
            {
                paginatedList.TotalCount,
                paginatedList.PageSize,
                paginatedList.CurrentPage,
                paginatedList.TotalPages,
                paginatedList.HasNext,
                paginatedList.HasPrevious
            };
        }
    }
}
