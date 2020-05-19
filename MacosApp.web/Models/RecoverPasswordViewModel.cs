using System.ComponentModel.DataAnnotations;

namespace MacosApp.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
