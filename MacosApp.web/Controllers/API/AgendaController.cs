using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacosApp.Common.Models;
using MacosApp.Web.Data;
using MacosApp.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacosApp.Web.Controllers.API
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AgendaController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;

        public AgendaController(
            DataContext dataContext,
            IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        [Route("GetAgendaForEmployee")]
        public async Task<IActionResult> GetAgendaForEmployee(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agendas = await _dataContext.Agendas
                .Include(a => a.Employee)
                .ThenInclude(o => o.User)
                .Include(a => a.Labour)
                .ThenInclude(p => p.LabourType)
                .Where(a => a.Date >= DateTime.Today.ToUniversalTime())
                .OrderBy(a => a.Date)
                .ToListAsync();

            var response = new List<AgendaResponse>();
            foreach (var agenda in agendas)
            {
                var agendaRespose = new AgendaResponse
                {
                    Date = agenda.Date,
                    Id = agenda.Id,
                    IsAvailable = agenda.IsAvailable
                };

                if (agenda.Employee != null)
                {
                    if (agenda.Employee.User.Email.ToLower().Equals(emailRequest.Email.ToLower()))
                    {
                        agendaRespose.Employee = _converterHelper.ToEmployeeResposne(agenda.Employee);
                        agendaRespose.Labour = _converterHelper.ToLabourResponse(agenda.Labour);
                        agendaRespose.Remarks = agenda.Remarks;
                    }
                    else
                    {
                        agendaRespose.Employee = new EmployeeResponse { FirstName = "Reserved" };
                    }
                }

                response.Add(agendaRespose);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("AssignAgenda")]
        public async Task<IActionResult> AssignAgenda(AssignRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agenda = await _dataContext.Agendas.FindAsync(request.AgendaId);
            if (agenda == null)
            {
                return BadRequest("Agenda doesn't exists.");
            }

            if (!agenda.IsAvailable)
            {
                return BadRequest("Agenda is not available.");
            }

            var employee = await _dataContext.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Employee doesn't exists.");
            }

            var labour = await _dataContext.Labours.FindAsync(request.LabourId);
            if (labour == null)
            {
                return BadRequest("Labour doesn't exists.");
            }

            agenda.IsAvailable = false;
            agenda.Remarks = request.Remarks;
            agenda.Employee = employee;
            agenda.Labour = labour;

            _dataContext.Agendas.Update(agenda);
            await _dataContext.SaveChangesAsync();
            return Ok(true);
        }

        [HttpPost]
        [Route("UnAssignAgenda")]
        public async Task<IActionResult> UnAssignAgenda(UnAssignRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agenda = await _dataContext.Agendas
                .Include(a => a.Employee)
                .Include(a => a.Labour)
                .FirstOrDefaultAsync(a => a.Id == request.AgendaId);
            if (agenda == null)
            {
                return BadRequest("Agenda doesn't exists.");
            }

            if (agenda.IsAvailable)
            {
                return BadRequest("Agenda is available.");
            }

            agenda.IsAvailable = true;
            agenda.Remarks = null;
            agenda.Employee = null;
            agenda.Labour = null;

            _dataContext.Agendas.Update(agenda);
            await _dataContext.SaveChangesAsync();
            return Ok(true);
        }
    }
}
