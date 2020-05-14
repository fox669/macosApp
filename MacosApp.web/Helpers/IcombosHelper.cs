using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MacosApp.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboLabourTypes();

        IEnumerable<SelectListItem> GetComboServiceTypes();

        IEnumerable<SelectListItem> GetComboEmployees();

        IEnumerable<SelectListItem> GetComboLabours(int employeeId);
    }
}