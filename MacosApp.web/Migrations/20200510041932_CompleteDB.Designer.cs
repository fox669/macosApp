﻿// <auto-generated />
using System;
using MacosApp.web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MacosApp.web.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200510041932_CompleteDB")]
    partial class CompleteDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MacosApp.web.Data.Entities.Agenda", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int?>("EmployeeId");

                    b.Property<bool>("IsAvailable");

                    b.Property<int?>("LabourId");

                    b.Property<string>("Remarks");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("LabourId");

                    b.ToTable("Agendas");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CellPhone")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.Labour", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EmployeeId");

                    b.Property<string>("ImageUrl");

                    b.Property<int?>("LabourTypeId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Remarks");

                    b.Property<DateTime>("Start");

                    b.Property<string>("WorkCrew")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("LabourTypeId");

                    b.ToTable("Labours");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.LabourType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("labourTypes");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.Report", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("LabourId");

                    b.Property<string>("Remarks");

                    b.Property<int?>("ServiceTypeId");

                    b.HasKey("id");

                    b.HasIndex("LabourId");

                    b.HasIndex("ServiceTypeId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.ServiceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("ServiceTypes");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.Agenda", b =>
                {
                    b.HasOne("MacosApp.web.Data.Entities.Employee", "Employee")
                        .WithMany("Agendas")
                        .HasForeignKey("EmployeeId");

                    b.HasOne("MacosApp.web.Data.Entities.Labour", "Labour")
                        .WithMany("Agendas")
                        .HasForeignKey("LabourId");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.Labour", b =>
                {
                    b.HasOne("MacosApp.web.Data.Entities.Employee", "Employee")
                        .WithMany("labours")
                        .HasForeignKey("EmployeeId");

                    b.HasOne("MacosApp.web.Data.Entities.LabourType", "LabourType")
                        .WithMany("labours")
                        .HasForeignKey("LabourTypeId");
                });

            modelBuilder.Entity("MacosApp.web.Data.Entities.Report", b =>
                {
                    b.HasOne("MacosApp.web.Data.Entities.Labour", "Labour")
                        .WithMany("Reports")
                        .HasForeignKey("LabourId");

                    b.HasOne("MacosApp.web.Data.Entities.ServiceType", "ServiceType")
                        .WithMany("Reports")
                        .HasForeignKey("ServiceTypeId");
                });
#pragma warning restore 612, 618
        }
    }
}
