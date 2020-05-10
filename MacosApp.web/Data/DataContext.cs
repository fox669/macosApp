﻿using MacosApp.web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacosApp.web.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Labour> Labours { get; set; }
        public DbSet<LabourType> labourTypes { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }

    }
}
