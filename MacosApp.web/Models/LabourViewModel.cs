using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MacosApp.Web.Data.Entities;

namespace MacosApp.Web.Models
{
    public class LabourViewModel : Labour
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Labour Type")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a labour type.")]
        public int LabourTypeId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public IEnumerable<SelectListItem> LabourTypes { get; set; }
    }
}
