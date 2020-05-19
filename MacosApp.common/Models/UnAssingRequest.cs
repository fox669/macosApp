using System.ComponentModel.DataAnnotations;

namespace MacosApp.Common.Models
{
    public class UnAssignRequest
    {
        [Required]
        public int AgendaId { get; set; }
    }
}

