using System;

namespace MacosApp.Common.Models
{
    public class AgendaResponse
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public EmployeeResponse Employee { get; set; }

        public LabourResponse Labour { get; set; }

        public string Remarks { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime DateLocal => Date.ToLocalTime();
    }
}
