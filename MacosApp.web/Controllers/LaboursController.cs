using System;
using System.IO;
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
    [Authorize(Roles = "Admin")]
    public class LaboursController : Controller
    {
        private readonly ICombosHelper _combosHelper;
        private readonly DataContext _dataContext;

        public LaboursController(
            ICombosHelper combosHelper,
            DataContext dataContext)
        {
            _combosHelper = combosHelper;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Labours
                .Include(p => p.Employee)
                .ThenInclude(o => o.User)
                .Include(p => p.LabourType)
                .Include(p => p.Reports));
        }

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

            var view = new LabourViewModel
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

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LabourViewModel view)
        {
            if (ModelState.IsValid)
            {
                var path = view.ImageUrl;

                if (view.ImageFile != null && view.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Labours",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await view.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Labours/{file}";
                }

                var labour = new Labour
                {
                    Start = view.Start,
                    Id = view.Id,
                    ImageUrl = path,
                    Name = view.Name,
                    Employee = await _dataContext.Employees.FindAsync(view.EmployeeId),
                    LabourType = await _dataContext.LabourTypes.FindAsync(view.LabourTypeId),
                    Activity = view.Activity,
                    Remarks = view.Remarks
                };

                _dataContext.Labours.Update(labour);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labour == null)
            {
                return NotFound();
            }

            _dataContext.Labours.Remove(labour);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _dataContext.Reports
                .Include(h => h.Labour)
                .FirstOrDefaultAsync(h => h.Id == id.Value);
            if (report == null)
            {
                return NotFound();
            }

            _dataContext.Reports.Remove(report);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{report.Labour.Id}");
        }

        public async Task<IActionResult> EditReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _dataContext.Reports
                .Include(h => h.Labour)
                .Include(h => h.ServiceType)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (report == null)
            {
                return NotFound();
            }

            var view = new ReportViewModel
            {
                Date = report.Date,
                Description = report.Description,
                Id = report.Id,
                LabourId = report.Labour.Id,
                Remarks = report.Remarks,
                ServiceTypeId = report.ServiceType.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes()
            };

            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> EditReport(ReportViewModel view)
        {
            if (ModelState.IsValid)
            {
                var report = new Report
                {
                    Date = view.Date,
                    Description = view.Description,
                    Id = view.Id,
                    Labour = await _dataContext.Labours.FindAsync(view.LabourId),
                    Remarks = view.Remarks,
                    ServiceType = await _dataContext.ServiceTypes.FindAsync(view.ServiceTypeId)
                };

                _dataContext.Reports.Update(report);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{view.LabourId}");
            }

            return View(view);
        }

        public async Task<IActionResult> AddReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours.FindAsync(id.Value);
            if (labour == null)
            {
                return NotFound();
            }

            var view = new ReportViewModel
            {
                Date = DateTime.Now,
                LabourId = labour.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes(),
            };

            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> AddReport(ReportViewModel view)
        {
            if (ModelState.IsValid)
            {
                var report = new Report
                {
                    Date = view.Date,
                    Description = view.Description,
                    Labour = await _dataContext.Labours.FindAsync(view.LabourId),
                    Remarks = view.Remarks,
                    ServiceType = await _dataContext.ServiceTypes.FindAsync(view.ServiceTypeId)
                };

                _dataContext.Reports.Add(report);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{view.LabourId}");
            }

            return View(view);
        }
    }
}
