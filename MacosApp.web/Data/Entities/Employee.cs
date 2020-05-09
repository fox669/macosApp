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
        [Required]
        [MaxLength(30)]

        public string Document { get; set; }
        [Required]
        [MaxLength(50)]

        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name ="First Name")]

        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name="Last Name")]

        public string Address { get; set; }
        [Required]
        [MaxLength(100)]

        public string CellPhone { get; set; }
        [Required]
        [MaxLength(20)]
        [Display(Name ="Cell Phone")]

        public string FullName => $"{FirstName} {LastName}";
        public string FullNameWithDocument => $"{FirstName} {LastName} {Document }";
    }
}
