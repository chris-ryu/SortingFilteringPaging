using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JWTExample.Models;
using SortingFilteringPaging.Models;

namespace SortingFilteringPaging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ListBaseController
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context, IHttpContextAccessor httpContextAccessor):base(httpContextAccessor)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet("search")]
        public async Task<IActionResult> GetEmployees([FromQuery] string searchKey, [FromQuery] int page)
        {
            int pageSize = 10;
             var result = await _context.Employees.Where(x => x.FirstName.Contains(searchKey)|| x.LastName.Contains(searchKey)).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
             //var result = await _context.Employees.ToListAsync();
            int total = await _context.Employees.CountAsync();
            return Ok(new {Total = total, Page=page, Employees = result});
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetEmployeeList(
            [FromQuery] string firstName_like, 
            [FromQuery] string lastName_like, 
            [FromQuery] bool lastName_sort,
            [FromQuery] int page)
        {
            int limit = 10;
            var baseResultSet = _context.Employees.AsQueryable();

            var filters = this.GetFilter();

            var deleg = ExpressionBuilder.GetExpression<Employee>(filters).Compile();
            var filteredResultSet = baseResultSet.Where(deleg).AsQueryable();
            var orderedResultSet = lastName_sort ? filteredResultSet.OrderByProperty("LastName") : filteredResultSet.OrderByPropertyDescending("LastName");
            var resultSet = orderedResultSet.Skip((page - 1) * limit).Take(limit).ToList();
            var total = filteredResultSet.Count();
            return Ok(new { List = resultSet, Page = page, Total = total });
        }


    }
}
