using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MacosApp.web.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [MaxLength(30, ErrorMessage ="The {0} field can't have more than {1} characters.")]
        [Required(ErrorMessage ="The field {0} is mandatory.")] 
        public string Document { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can't have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can't have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]        
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can't have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]          
        public string Address { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(100, ErrorMessage = "The {0} field can't have more than {1} characters.")]        
        [Display(Name = "Cell Phone")]
        public string CellPhone { get; set; }

       
        public string FullName => $"{FirstName} {LastName}";
        public string FullNameWithDocument => $"{FirstName} {LastName} {Document }";
        public ICollection<Labour> labours { get; set; }
        public ICollection<Agenda> Agendas { get; set; }


    }
}
