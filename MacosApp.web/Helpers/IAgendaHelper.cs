using System.Threading.Tasks;

namespace MacosApp.Web.Helpers
{
    public interface IAgendaHelper
    {
        Task AddDaysAsync(int days);
    }
}
