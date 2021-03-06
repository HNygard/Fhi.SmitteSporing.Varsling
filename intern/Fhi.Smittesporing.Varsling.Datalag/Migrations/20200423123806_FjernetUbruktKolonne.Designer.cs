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
    [Migration("20200423123806_FjernetUbruktKolonne")]
    partial class FjernetUbruktKolonne
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

                    b.ToTable("Applikasjonsinnstilling","App");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Indekspasient", b =>
                {
                    b.Property<int>("IndekspasientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fodselsnummer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("KommuneId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Opprettettidspunkt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Provedato")
                        .HasColumnType("datetime2");

                    b.Property<string>("Sha256")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("TelefonId")
                        .HasColumnType("int");

                    b.Property<int>("Varslingsstatus")
                        .HasColumnType("int");

                    b.HasKey("IndekspasientId");

                    b.HasIndex("KommuneId");

                    b.HasIndex("TelefonId");

                    b.ToTable("Indekspasient","Msis");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg.Innsynlogg", b =>
                {
                    b.Property<int>("InnsynloggId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Felt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hva")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hvem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hvorfor")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InnsynloggId");

                    b.ToTable("Innsyn","Innsyn");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Kommune", b =>
                {
                    b.Property<int>("KommuneId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("KommuneNr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Navn")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("KommuneId");

                    b.ToTable("Kommune","App");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", b =>
                {
                    b.Property<int>("SmittekontaktId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AntallKontakter")
                        .HasColumnType("int");

                    b.Property<double>("BluetoothAkkumulertRisikoscore")
                        .HasColumnType("float");

                    b.Property<double>("BluetoothAkkumulertVarighet")
                        .HasColumnType("float");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<double>("GpsAkkumulertRisikoscore")
                        .HasColumnType("float");

                    b.Property<double>("GpsAkkumulertVarighet")
                        .HasColumnType("float");

                    b.Property<int>("IndekspasientId")
                        .HasColumnType("int");

                    b.Property<string>("InteressepunkterJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Risikokategori")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TelefonId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("VarsletTidspunkt")
                        .HasColumnType("datetime2");

                    b.HasKey("SmittekontaktId");

                    b.HasIndex("IndekspasientId");

                    b.HasIndex("TelefonId");

                    b.ToTable("Smittekontakt","Simula");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDetaljer", b =>
                {
                    b.Property<int>("SmittekontaktDetaljerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("BluetoothAkkumulertRisiko")
                        .HasColumnType("float");

                    b.Property<double>("BluetoothAkkumulertVarighet")
                        .HasColumnType("float");

                    b.Property<string>("BluetoothInteressepunkterJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("BluetoothMedianavstand")
                        .HasColumnType("float");

                    b.Property<DateTime>("Dato")
                        .HasColumnType("datetime2");

                    b.Property<double>("GpsAkkumulertRisiko")
                        .HasColumnType("float");

                    b.Property<double>("GpsAkkumulertVarighet")
                        .HasColumnType("float");

                    b.Property<string>("GpsInteressepunkterJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("GpsMedianavstand")
                        .HasColumnType("float");

                    b.Property<int>("OppsummertPlotHtmlId")
                        .HasColumnType("int");

                    b.Property<int>("SmittekontaktId")
                        .HasColumnType("int");

                    b.HasKey("SmittekontaktDetaljerId");

                    b.HasIndex("SmittekontaktId");

                    b.ToTable("SmittekontaktDetaljer","Simula");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDetaljerHtmlKart", b =>
                {
                    b.Property<int>("SmittekontaktDetaljerHtmlKartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Innhold")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SmittekontaktDetaljerId")
                        .HasColumnType("int");

                    b.HasKey("SmittekontaktDetaljerHtmlKartId");

                    b.HasIndex("SmittekontaktDetaljerId")
                        .IsUnique();

                    b.ToTable("SmittekontaktDetaljerHtmlKart","Simula");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDiagram", b =>
                {
                    b.Property<int>("SmittekontaktDiagramId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Data")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("SmittekontaktId")
                        .HasColumnType("int");

                    b.HasKey("SmittekontaktDiagramId");

                    b.HasIndex("SmittekontaktId")
                        .IsUnique();

                    b.ToTable("SmittekontaktDiagram","Simula");
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

                    b.ToTable("SmsVarsel","Sms");
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
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TelefonId");

                    b.HasIndex("Telefonnummer")
                        .IsUnique()
                        .HasFilter("[Telefonnummer] IS NOT NULL");

                    b.ToTable("Telefon","Krr");
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Indekspasient", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Kommune", "Kommune")
                        .WithMany()
                        .HasForeignKey("KommuneId");

                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Telefon", "Telefon")
                        .WithMany("IndekspasienterForTelefon")
                        .HasForeignKey("TelefonId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Indekspasient", "Indekspasient")
                        .WithMany("Smittekontakter")
                        .HasForeignKey("IndekspasientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Telefon", "Telefon")
                        .WithMany("SmittekontaktForTelefon")
                        .HasForeignKey("TelefonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDetaljer", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", "Smittekontakt")
                        .WithMany("Detaljer")
                        .HasForeignKey("SmittekontaktId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDetaljerHtmlKart", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDetaljer", "SmittekontaktDetaljer")
                        .WithOne("OppsummertPlotDetaljerHtml")
                        .HasForeignKey("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDetaljerHtmlKart", "SmittekontaktDetaljerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDiagram", b =>
                {
                    b.HasOne("Fhi.Smittesporing.Varsling.Domene.Modeller.Smittekontakt", "Smittekontakt")
                        .WithOne("SoyleDiagram")
                        .HasForeignKey("Fhi.Smittesporing.Varsling.Domene.Modeller.SmittekontaktDiagram", "SmittekontaktId")
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
