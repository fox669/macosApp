using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacosApp.Common.Models;
using MacosApp.Web.Data;

namespace MacosApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public EmployeesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("GetEmployeeByEmail")]
        public async Task<IActionResult> GetEmployee(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var employee = await _dataContext.Employees
                .Include(o => o.User)
                .Include(o => o.Labours)
                .ThenInclude(p => p.LabourType)
                .Include(o => o.Labours)
                .ThenInclude(p => p.Reports)
                .ThenInclude(h => h.ServiceType)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == emailRequest.Email.ToLower());

            var response = new EmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.User.FirstName,
                LastName = employee.User.LastName,
                Address = employee.User.Address,
                Document = employee.User.Document,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber,
                Labours = employee.Labours.Select(p => new LabourResponse
                {
                    Start = p.Start,
                    Id = p.Id,
                    ImageUrl = p.ImageFullPath,
                    Name = p.Name,
                    Activity = p.Activity,
                    Remarks = p.Remarks,
                    LabourType = p.LabourType.Name,
                    Reports = p.Reports.Select(h => new ReportResponse
                    {
                        Date = h.Date,
                        Description = h.Description,
                        Id = h.Id,
                        Remarks = h.Remarks,
                        ServiceType = h.ServiceType.Name
                    }).ToList()
                }).ToList()
            };

            return Ok(response);
        }
    }
}
