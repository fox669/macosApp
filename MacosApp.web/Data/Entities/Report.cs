using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MacosApp.web.Data.Entities
{
    public class Report
    {
        public int id { get; set; }

        [Display(Name ="Description")]
        [MaxLength(100,ErrorMessage ="The {0} field can not have more than {1} charcaters")]
        [Required(ErrorMessage ="The field {0} is mandatory")]
        public string Description { get; set; }

        [Display(Name = "Date*")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string Remarks { get; set; }

        [Display(Name = "Date*")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Datelocal => Date.ToLocalTime();

        public ServiceType ServiceType { get; set; }

        public Labour Labour { get; set; }




    }
}
