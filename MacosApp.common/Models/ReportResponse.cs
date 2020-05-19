using System;

namespace MacosApp.Common.Models
{
    public class ReportResponse
    {
        public int Id { get; set; }

        public string ServiceType { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Remarks { get; set; }

        public DateTime DateLocal => Date.ToLocalTime();
    }
}
