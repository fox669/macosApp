using MacosApp.Web.Data.Entities;
using MacosApp.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MacosApp.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRoles();
            var manager = await CheckUserAsync("1010", "Juan", "Zuluaga", "jzuluaga55@gmail.com", "350 634 2747", "Calle Luna Calle Sol", "Admin");
            var customer = await CheckUserAsync("2020", "Juan", "Zuluaga", "jzuluaga55@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", "Customer");
            await CheckLabourTypesAsync();
            await CheckServiceTypesAsync();
            await CheckEmployeeAsync(customer);
            await CheckManagerAsync(manager);
            await CheckLaboursAsync();
            //await CheckAgendasAsync();
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");
        }

        private async Task<User> CheckUserAsync(
            string document, 
            string firstName, 
            string lastName, 
            string email, 
            string phone, 
            string address, 
            string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role);

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckLaboursAsync()
        {
            if (!_dataContext.Labours.Any())
            {
                var employee = _dataContext.Employees.FirstOrDefault();
                var labourType = _dataContext.LabourTypes.FirstOrDefault();
                AddLabour("Otto", employee, labourType, "Shih tzu");
                AddLabour("Killer", employee, labourType, "Dobermann");
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckServiceTypesAsync()
        {
            if (!_dataContext.ServiceTypes.Any())
            {
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Consulta" });
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Urgencia" });
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Vacunación" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckLabourTypesAsync()
        {
            if (!_dataContext.LabourTypes.Any())
            {
                _dataContext.LabourTypes.Add(new LabourType { Name = "Perro" });
                _dataContext.LabourTypes.Add(new LabourType { Name = "Gato" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckEmployeeAsync(User user)
        {
            if (!_dataContext.Employees.Any())
            {
                _dataContext.Employees.Add(new Employee { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckManagerAsync(User user)
        {
            if (!_dataContext.Managers.Any())
            {
                _dataContext.Managers.Add(new Manager { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private void AddLabour(string name, Employee employee, LabourType labourType, string race)
        {
            _dataContext.Labours.Add(new Labour
            {
                Start = DateTime.Now.AddYears(-2),
                Name = name,
                Employee = employee,
                LabourType = labourType,
                Activity = race
            });
        }

        private async Task CheckAgendasAsync()
        {
            if (!_dataContext.Agendas.Any())
            {
                var initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                var finalDate = initialDate.AddYears(1);
                while (initialDate < finalDate)
                {
                    if (initialDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        var finalDate2 = initialDate.AddHours(10);
                        while (initialDate < finalDate2)
                        {
                            _dataContext.Agendas.Add(new Agenda
                            {
                                Date = initialDate,
                                IsAvailable = true
                            });

                            initialDate = initialDate.AddMinutes(30);
                        }

                        initialDate = initialDate.AddHours(14);
                    }
                    else
                    {
                        initialDate = initialDate.AddDays(1);
                    }
                }
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}
