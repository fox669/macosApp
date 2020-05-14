using System.Collections.Generic;

namespace MacosApp.web.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public User User { get; set; }



        public ICollection<Labour> Labours { get; set; }

        public ICollection<Agenda> Agendas { get; set; }


    }
}
