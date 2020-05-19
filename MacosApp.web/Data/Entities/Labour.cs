using MacosApp.web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacosApp.Web.Data.Entities
{
    public class Labour
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Activity { get; set; }

        [Display(Name = "Start")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        public string Remarks { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(ImageUrl)
            ? null
            : $"https://myveterinary.azurewebsites.net{ImageUrl.Substring(1)}";

        [Display(Name = "Start")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime StartLocal => Start.ToLocalTime();

        public LabourType LabourType { get; set; }

        public Employee Employee { get; set; }

        public ICollection<Report> Reports { get; set; }

        public ICollection<Agenda> Agendas { get; set; }
    }
}
