using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JWTExample.Models;
using SortingFilteringPaging0.Models;

namespace SortingFilteringPaging0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
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

    }
}
