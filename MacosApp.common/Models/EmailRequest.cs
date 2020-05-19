using System.ComponentModel.DataAnnotations;

namespace MacosApp.Common.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
