using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacosApp.Web.Data;
using MacosApp.Web.Data.Entities;
using MacosApp.Web.Helpers;
using MacosApp.Web.Models;

namespace MacosApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public HomeController(
            DataContext dataContext,
            ICombosHelper combosHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Authorize(Roles = "Customer")]
        public IActionResult MyLabours()
        {
            return View(_dataContext.Labours
                .Include(p => p.LabourType)
                .Include(p => p.Reports)
                .Where(p => p.Employee.User.Email.ToLower().Equals(User.Identity.Name.ToLower())));
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours
                .Include(p => p.Employee)
                .Include(p => p.LabourType)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (labour == null)
            {
                return NotFound();
            }

            var model = new LabourViewModel
            {
                Start = labour.Start,
                Id = labour.Id,
                ImageUrl = labour.ImageUrl,
                Name = labour.Name,
                EmployeeId = labour.Employee.Id,
                LabourTypeId = labour.LabourType.Id,
                LabourTypes = _combosHelper.GetComboLabourTypes(),
                Activity = labour.Activity,
                Remarks = labour.Remarks
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LabourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Labours",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Labours/{file}";
                }

                var labour = new Labour
                {
                    Start = model.Start,
                    Id = model.Id,
                    ImageUrl = path,
                    Name = model.Name,
                    Employee = await _dataContext.Employees.FindAsync(model.EmployeeId),
                    LabourType = await _dataContext.LabourTypes.FindAsync(model.LabourTypeId),
                    Activity = model.Activity,
                    Remarks = model.Remarks
                };

                _dataContext.Labours.Update(labour);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(MyLabours));
            }

            model.LabourTypes = _combosHelper.GetComboLabourTypes();
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours
                .Include(p => p.Employee)
                .ThenInclude(o => o.User)
                .Include(p => p.Reports)
                .ThenInclude(h => h.ServiceType)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (labour == null)
            {
                return NotFound();
            }

            return View(labour);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours
                .Include(p => p.Reports)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labour == null)
            {
                return NotFound();
            }

            if (labour.Reports.Count > 0)
            {
                return RedirectToAction(nameof(MyLabours));
            }

            _dataContext.Labours.Remove(labour);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(MyLabours));
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var employee = await _dataContext.Employees
                .FirstOrDefaultAsync(o => o.User.Email.ToLower().Equals(User.Identity.Name.ToLower()));
            if (employee == null)
            {
                return NotFound();
            }

            var model = new LabourViewModel
            {
                Start = DateTime.Now,
                LabourTypes = _combosHelper.GetComboLabourTypes(),
                EmployeeId = employee.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LabourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Labours",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Labours/{file}";
                }

                var labour = new Labour
                {
                    Start = model.Start,
                    ImageUrl = path,
                    Name = model.Name,
                    Employee = await _dataContext.Employees.FindAsync(model.EmployeeId),
                    LabourType = await _dataContext.LabourTypes.FindAsync(model.LabourTypeId),
                    Activity = model.Activity,
                    Remarks = model.Remarks
                };

                _dataContext.Labours.Add(labour);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(MyLabours)}");
            }

            model.LabourTypes = _combosHelper.GetComboLabourTypes();
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyAgenda()
        {
            var agendas = await _dataContext.Agendas
                .Include(a => a.Employee)
                .ThenInclude(o => o.User)
                .Include(a => a.Labour)
                .Where(a => a.Date >= DateTime.Today.ToUniversalTime()).ToListAsync();

            var list = new List<AgendaViewModel>(agendas.Select(a => new AgendaViewModel
            {
                Date = a.Date,
                Id = a.Id,
                IsAvailable = a.IsAvailable,
                Employee = a.Employee,
                Labour = a.Labour,
                Remarks = a.Remarks
            }).ToList());

            list.Where(a => a.Employee != null && a.Employee.User.UserName.ToLower().Equals(User.Identity.Name.ToLower()))
                .All(a => { a.IsMine = true; return true; });

            return View(list);
        }

        [Authorize(Roles = "Customer")]
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

            var employee = await _dataContext.Employees.FirstOrDefaultAsync(o => o.User.UserName.ToLower().Equals(User.Identity.Name.ToLower()));
            if (employee == null)
            {
                return NotFound();
            }

            var model = new AgendaViewModel
            {
                Id = agenda.Id,
                EmployeeId = employee.Id,
                Labours = _combosHelper.GetComboLabours(employee.Id)
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
                    agenda.Employee = await _dataContext.Employees.FindAsync(model.EmployeeId);
                    agenda.Labour = await _dataContext.Labours.FindAsync(model.LabourId);
                    agenda.Remarks = model.Remarks;
                    _dataContext.Agendas.Update(agenda);
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction(nameof(MyAgenda));
                }
            }

            model.Labours = _combosHelper.GetComboLabours(model.Id);
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Unassign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agenda = await _dataContext.Agendas
                .Include(a => a.Employee)
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
            return RedirectToAction(nameof(MyAgenda));
        }
    }
}