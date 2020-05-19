using System;
using System.Collections.Generic;
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
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public EmployeesController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Employees
                .Include(o => o.User)
                .Include(o => o.Labours));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataContext.Employees
                .Include(o => o.User)
                .Include(o => o.Labours)
                .ThenInclude(p => p.LabourType)
                .Include(o => o.Labours)
                .ThenInclude(p => p.Reports)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Address = model.Address,
                    Document = model.Document,
                    Email = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Username
                };

                var response = await _userHelper.AddUserAsync(user, model.Password);
                if (response.Succeeded)
                {
                    var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);
                    await _userHelper.AddUserToRoleAsync(userInDB, "Customer");

                    var employee = new Employee
                    {
                        Agendas = new List<Agenda>(),
                        Labours = new List<Labour>(),
                        User = userInDB
                    };

                    _dataContext.Employees.Add(employee);

                    try
                    {
                        await _dataContext.SaveChangesAsync();

                        var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        var tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userid = user.Id,
                            token = myToken
                        }, protocol: HttpContext.Request.Scheme);

                        _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                            $"To allow the user, " +
                            $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.ToString());
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataContext.Employees
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = employee.User.Address,
                Document = employee.User.Document,
                FirstName = employee.User.FirstName,
                Id = employee.Id,
                LastName = employee.User.LastName,
                PhoneNumber = employee.User.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = await _dataContext.Employees
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                employee.User.Document = model.Document;
                employee.User.FirstName = model.FirstName;
                employee.User.LastName = model.LastName;
                employee.User.Address = model.Address;
                employee.User.PhoneNumber = model.PhoneNumber;

                await _userHelper.UpdateUserAsync(employee.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataContext.Employees
                .Include(o => o.User)
                .Include(o => o.Labours)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            if (employee.Labours.Count > 0)
            {
                //TODO: Message
                return RedirectToAction(nameof(Index));
            }

            await _userHelper.DeleteUserAsync(employee.User.Email);
            _dataContext.Employees.Remove(employee);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _dataContext.Employees.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddLabour(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataContext.Employees.FindAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new LabourViewModel
            {
                Start = DateTime.Today,
                EmployeeId = employee.Id,
                LabourTypes = _combosHelper.GetComboLabourTypes()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLabour(LabourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var labour = await _converterHelper.ToLabourAsync(model, path, true);
                _dataContext.Labours.Add(labour);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"Details/{model.EmployeeId}");
            }

            model.LabourTypes = _combosHelper.GetComboLabourTypes();
            return View(model);
        }

        public async Task<IActionResult> EditLabour(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours
                .Include(p => p.Employee)
                .Include(p => p.LabourType)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (labour == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToLabourViewModel(labour));
        }

        [HttpPost]
        public async Task<IActionResult> EditLabour(LabourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var labour = await _converterHelper.ToLabourAsync(model, path, false);
                _dataContext.Labours.Update(labour);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"Details/{model.EmployeeId}");
            }

            model.LabourTypes = _combosHelper.GetComboLabourTypes();
            return View(model);
        }

        public async Task<IActionResult> DetailsLabour(int? id)
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

            var model = new ReportViewModel
            {
                Date = DateTime.Now,
                LabourId = labour.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReport(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = await _converterHelper.ToReportAsync(model, true);
                _dataContext.Reports.Add(report);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsLabour)}/{model.LabourId}");
            }

            model.ServiceTypes = _combosHelper.GetComboServiceTypes();
            return View(model);
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

            return View(_converterHelper.ToReportViewModel(report));
        }

        [HttpPost]
        public async Task<IActionResult> EditReport(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = await _converterHelper.ToReportAsync(model, false);
                _dataContext.Reports.Update(report);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsLabour)}/{model.LabourId}");
            }

            model.ServiceTypes = _combosHelper.GetComboServiceTypes();
            return View(model);
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
            return RedirectToAction($"{nameof(DetailsLabour)}/{report.Labour.Id}");
        }

        public async Task<IActionResult> DeleteLabour(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labour = await _dataContext.Labours
                .Include(p => p.Employee)
                .Include(p => p.Reports)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (labour == null)
            {
                return NotFound();
            }

            if(labour.Reports.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The labour can't be deleted because it has related records.");
                return RedirectToAction($"{nameof(Details)}/{labour.Employee.Id}");
            }

            _dataContext.Labours.Remove(labour);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{labour.Employee.Id}");
        }
    }
}
