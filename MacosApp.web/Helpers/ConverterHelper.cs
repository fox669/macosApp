using System.Threading.Tasks;
using MacosApp.Common.Models;
using MacosApp.Web.Data;
using MacosApp.Web.Data.Entities;
using MacosApp.Web.Models;

namespace MacosApp.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(
            DataContext dataContext,
            ICombosHelper combosHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }

        public async Task<Labour> ToLabourAsync(LabourViewModel model, string path, bool isNew)
        {
            var labour = new Labour
            {
                Agendas = model.Agendas,
                Start = model.Start,
                Reports = model.Reports,
                Id = isNew ? 0 : model.Id,
                ImageUrl = path,
                Name = model.Name,
                Employee = await _dataContext.Employees.FindAsync(model.EmployeeId),
                LabourType = await _dataContext.LabourTypes.FindAsync(model.LabourTypeId),
                Activity = model.Activity,
                Remarks = model.Remarks
            };

            return labour;
        }

        public LabourViewModel ToLabourViewModel(Labour labour)
        {
            return new LabourViewModel
            {
                Agendas = labour.Agendas,
                Start = labour.Start,
                Reports = labour.Reports,
                ImageUrl = labour.ImageUrl,
                Name = labour.Name,
                Employee = labour.Employee,
                LabourType = labour.LabourType,
                Activity = labour.Activity,
                Remarks = labour.Remarks,
                Id = labour.Id,
                EmployeeId = labour.Employee.Id,
                LabourTypeId = labour.LabourType.Id,
                LabourTypes = _combosHelper.GetComboLabourTypes()
            };
        }

        public async Task<Report> ToReportAsync(ReportViewModel model, bool isNew)
        {
            return new Report
            {
                Date = model.Date.ToUniversalTime(),
                Description = model.Description,
                Id = isNew ? 0 : model.Id,
                Labour = await _dataContext.Labours.FindAsync(model.LabourId),
                Remarks = model.Remarks,
                ServiceType = await _dataContext.ServiceTypes.FindAsync(model.ServiceTypeId)
            };
        }

        public ReportViewModel ToReportViewModel(Report report)
        {
            return new ReportViewModel
            {
                Date = report.Date,
                Description = report.Description,
                Id = report.Id,
                LabourId = report.Labour.Id,
                Remarks = report.Remarks,
                ServiceTypeId = report.ServiceType.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes()
            };
        }

        public LabourResponse ToLabourResponse(Labour labour)
        {
            if (labour == null)
            {
                return null;
            }

            return new LabourResponse
            {
                Start = labour.Start,
                Id = labour.Id,
                ImageUrl = labour.ImageFullPath,
                Name = labour.Name,
                LabourType = labour.LabourType.Name,
                Activity = labour.Activity,
                Remarks = labour.Remarks
            };
        }

        public EmployeeResponse ToEmployeeResposne(Employee employee)
        {
            if (employee == null)
            {
                return null;
            }

            return new EmployeeResponse
            {
                Address = employee.User.Address,
                Document = employee.User.Document,
                Email = employee.User.Email,
                FirstName = employee.User.FirstName,
                LastName = employee.User.LastName,
                PhoneNumber = employee.User.PhoneNumber
            };
        }
    }
}
