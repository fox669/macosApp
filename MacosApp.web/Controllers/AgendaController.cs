using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MacosApp.web.Data;
using MacosApp.web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using MacosApp.Web.Helpers;
using MacosApp.Web.Models;

namespace MacosApp.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AgendaController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IAgendaHelper _agendaHelper;
        private readonly ICombosHelper _combosHelper;

        public AgendaController(
            DataContext dataContext,
            IAgendaHelper agendaHelper,
            ICombosHelper combosHelper)
        {
            _dataContext = dataContext;
            _agendaHelper = agendaHelper;
            _combosHelper = combosHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Agendas
                .Include(a => a.Employee)
                .ThenInclude(o => o.User)
                .Include(a => a.Labour)
                .Where(a => a.Date >= DateTime.Today.ToUniversalTime()));
        }

        public async Task<IActionResult> AddDays()
        {
            await _agendaHelper.AddDaysAsync(7);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Assing(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agenda = await _dataContext.Agendas
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (agenda == null)
            {
                return NotFound();
            }

            var model = new AgendaViewModel
            {
                Id = agenda.Id,
                Employees = _combosHelper.GetComboEmployees(),
                Labours = _combosHelper.GetComboLabours(0)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assing(AgendaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var agenda = await _dataContext.Agendas.FindAsync(model.Id);
                if (agenda != null)
                {
                    agenda.IsAvailable = false;
                    agenda.Labour = await _dataContext.Labours.FindAsync(model.LabourId);
                    agenda.Employee = await _dataContext.Employees.FindAsync(model.EmployeeId);
                    agenda.Remarks = model.Remarks;
                    _dataContext.Agendas.Update(agenda);
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            model.Employees = _combosHelper.GetComboEmployees();
            model.Labours = _combosHelper.GetComboLabours(model.LabourId);

            return View(model);
        }

        public async Task<JsonResult> GetLaboursAsync(int employeeId)
        {
            var labours = await _dataContext.Labours
                .Where(p => p.Employee.Id == employeeId)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Json(labours);
        }

        public async Task<IActionResult> Unassign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agenda = await _dataContext.Agendas
                .Include(a => a.Labour)
                .Include(a => a.Labour)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (agenda == null)
            {
                return NotFound();
            }

            agenda.IsAvailable = true;
            agenda.Labour = null;
            agenda.Employee = null;
            agenda.Remarks = null;

            _dataContext.Agendas.Update(agenda);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
