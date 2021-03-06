// <auto-generated />
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
    [Migration("20200331183053_OppdateringSmsStatus")]
    partial class OppdateringSmsStatus
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Applikasjonsinnstilling", b =>
                {
                    b.Property<int>("ApplikasjonsinnstillingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nokkel")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Verdi")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApplikasjonsinnstillingId");

                    b.HasIndex("Nokkel")
                        .IsUnique()
                        .HasFilter("[Nokkel] IS NOT NULL");

                    b.ToTable("Applikasjonsinnstilling");
                });

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

                    b.Property<bool>("IkkeRegistrertKRK")
                        .HasColumnType("bit");

                    b.Property<int?>("TelefonId")
                        .HasColumnType("int");

                    b.HasKey("PasientId");

                    b.HasIndex("TelefonId");

                    b.ToTable("Pasient");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", b =>
                {
                    b.Property<int>("SmittekontaktId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AccumulatedDistance")
                        .HasColumnType("float");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExtraInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RiskScore")
                        .HasColumnType("float");

                    b.Property<int>("SmittetilfelleId")
                        .HasColumnType("int");

                    b.Property<int?>("StedsinfoId")
                        .HasColumnType("int");

                    b.Property<int>("TelefonId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("VarsletTidspunkt")
                        .HasColumnType("datetime2");

                    b.HasKey("SmittekontaktId");

                    b.HasIndex("SmittetilfelleId");

                    b.HasIndex("StedsinfoId");

                    b.HasIndex("TelefonId");

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

                    b.Property<DateTime>("Opprettettidspunkt")
                        .HasColumnType("datetime2");

                    b.Property<int>("PasientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Provedato")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Testing")
                        .HasColumnType("bit");

                    b.HasKey("SmittetilfelleId");

                    b.HasIndex("PasientId");

                    b.ToTable("Smittetilfelle","MSIS");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmsVarsel", b =>
                {
                    b.Property<int>("SmsVarselId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("OppdatertTidspunkt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("Referanse")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SmittekontaktId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("SmsVarselId");

                    b.HasIndex("Referanse")
                        .IsUnique()
                        .HasFilter("[Referanse] IS NOT NULL");

                    b.HasIndex("SmittekontaktId");

                    b.ToTable("SmsVarsel");
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

                    b.Property<DateTime?>("HentetFraKontaktInfo")
                        .HasColumnType("datetime2");

                    b.Property<string>("Telefonnummer")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TelefonId");

                    b.HasIndex("Telefonnummer")
                        .IsUnique()
                        .HasFilter("[Telefonnummer] IS NOT NULL");

                    b.ToTable("Telefon","KRR");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Pasient", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Telefon", "Telefon")
                        .WithMany("PasienterForTelefon")
                        .HasForeignKey("TelefonId");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittetilfelle", "Smittetilfelle")
                        .WithMany()
                        .HasForeignKey("SmittetilfelleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Stedsinfo", "Stedsinfo")
                        .WithMany()
                        .HasForeignKey("StedsinfoId");

                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Telefon", "Telefon")
                        .WithMany("SmittekontaktForTelefon")
                        .HasForeignKey("TelefonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittetilfelle", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Pasient", "Pasient")
                        .WithMany()
                        .HasForeignKey("PasientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmsVarsel", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", "Smittekontakt")
                        .WithMany("SmsVarsler")
                        .HasForeignKey("SmittekontaktId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
