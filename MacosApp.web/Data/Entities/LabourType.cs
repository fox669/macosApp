using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacosApp.Web.Data.Entities
{
    public class LabourType
    {
        public int Id { get; set; }

        [Display(Name = "Labour Type")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Name { get; set; }

        public ICollection<Labour> Labours { get; set; }
    }
}
