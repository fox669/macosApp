using System;
using System.Collections.Generic;

namespace MacosApp.Common.Models
{
    public class LabourResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Activity { get; set; }

        public DateTime Start { get; set; }

        public string Remarks { get; set; }

        public string LabourType { get; set; }

        public ICollection<ReportResponse> Reports { get; set; }
    }
}
