using System.ComponentModel.DataAnnotations;

namespace MacosApp.Common.Models
{
    public class AssignRequest
    {
        [Required]
        public int AgendaId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LabourId { get; set; }

        public string Remarks { get; set; }
    }
}
