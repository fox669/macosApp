using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MacosApp.Web.Data.Entities;
using MacosApp.web.Data.Entities;

namespace MacosApp.Web.Models
{
    public class AgendaViewModel : Agenda
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Employee")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an .")]
        public int EmployeeId { get; set; }

        public IEnumerable<SelectListItem> Employees { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Labour")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a labour.")]
        public int LabourId { get; set; }

        public IEnumerable<SelectListItem> Labours { get; set; }

        public bool IsMine { get; set; }

        public string Reserved => "Reserved";
    }
}