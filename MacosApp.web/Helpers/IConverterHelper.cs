using System.Threading.Tasks;
using MacosApp.Common.Models;
using MacosApp.Web.Data.Entities;
using MacosApp.Web.Models;

namespace MacosApp.Web.Helpers
{
    public interface IConverterHelper
    {
        Task<Labour> ToLabourAsync(LabourViewModel model, string path, bool isNew);

        LabourViewModel ToLabourViewModel(Labour labour);

        Task<Report> ToReportAsync(ReportViewModel model, bool isNew);

        ReportViewModel ToReportViewModel(Report report);

        LabourResponse ToLabourResponse(Labour labour);

        EmployeeResponse ToEmployeeResposne(Employee employee);
    }
}