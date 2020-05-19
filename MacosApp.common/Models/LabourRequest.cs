using System;
using System.ComponentModel.DataAnnotations;

namespace MacosApp.Common.Models
{
    public class LabourRequest
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Activity { get; set; }

        public int EmployeeId { get; set; }

        public int LabourTypeId { get; set; }

        [Required]
        public DateTime Start { get; set; }

        public string Remarks { get; set; }

        public byte[] ImageArray { get; set; }
    }
}
