﻿// <auto-generated />
using System;
using Fhi.Smittesporing.Varsling.Datalag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    [DbContext(typeof(SmitteVarslingContext))]
    [Migration("20200329150853_SmitteTilfellePasient")]
    partial class SmitteTilfellePasient
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Pasient", b =>
                {
                    b.Property<int>("PasientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fodselsnummer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TelefonId")
                        .HasColumnType("int");

                    b.HasKey("PasientId");

                    b.ToTable("Pasient");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", b =>
                {
                    b.Property<int>("SmittekontaktId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<int>("SmittetilfelleId")
                        .HasColumnType("int");

                    b.Property<int?>("StedsinfoId")
                        .HasColumnType("int");

                    b.Property<int>("TelefonId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("VarsletTidspunkt")
                        .HasColumnType("datetime2");

                    b.HasKey("SmittekontaktId");

                    b.HasIndex("StedsinfoId");

                    b.ToTable("Smittekontakt","Simula");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittetilfelle", b =>
                {
                    b.Property<int>("SmittetilfelleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fodselsnummer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PasientId")
                        .HasColumnType("int");

                    b.HasKey("SmittetilfelleId");

                    b.HasIndex("PasientId");

                    b.ToTable("Smittetilfelle","MSIS");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Stedsinfo", b =>
                {
                    b.Property<int>("StedsinfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Kommune")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StedsinfoId");

                    b.ToTable("Stedsinfo");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Telefon", b =>
                {
                    b.Property<int>("TelefonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Telefonnummer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TelefonId");

                    b.ToTable("Telefon","KRR");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Stedsinfo", "Stedsinfo")
                        .WithMany()
                        .HasForeignKey("StedsinfoId");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittetilfelle", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Pasient", "Pasient")
                        .WithMany()
                        .HasForeignKey("PasientId");
                });
#pragma warning restore 612, 618
        }
    }
}