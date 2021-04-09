using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Domene.InnsynsLogg
{
	public interface IEksporterInnsynService
	{
		Task<byte[]> CreateFile(InnsynLoggHelsenorgeAm[] innsyn, InnsynHelsenorgeAm data, InnsynSimulaDatabrukAm[] simuladatabruk, InnsynSimulaGpsDataAm[] simulagpsdata);
	}
	public class EksporterInnsynService : IEksporterInnsynService
	{
		public async Task<byte[]> CreateFile(InnsynLoggHelsenorgeAm[] innsyn, InnsynHelsenorgeAm data, InnsynSimulaDatabrukAm[] simuladatabruk, InnsynSimulaGpsDataAm[] simulagpsdata)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using (var stream = new MemoryStream())
			{
				using (var package = new ExcelPackage(stream))
				{
					var sheet = package.Workbook.Worksheets.Add("Prøvedatoer");
					sheet.Cells["A1"].Value = "Prøvedato";
					int teller = 2;
					foreach (var prøvedato in data.Prøvedatoer)
					{
						sheet.Cells["A" + teller].Value = prøvedato.ToString("yyyy-MM-dd");
						teller++;
					}

					sheet = package.Workbook.Worksheets.Add("Smittekontakter");
					sheet.Cells.LoadFromCollection(data.Smittekontakter.Select(s => new
					{
						Dato = s.Dato.ToString("yyyy-MM-dd"),
						Varslet = s.Varslet.HasValue ? s.Varslet.Value.ToString("s") : string.Empty,
						s.Risikokategori,
						s.Verifiseringskode
					}), true);

					sheet = package.Workbook.Worksheets.Add("Sms-varsel");
					sheet.Cells.LoadFromCollection(
							data.SmsVarsel.Select(s => new
							{
								Tidspunkt = s.Tidspunkt.ToString("s"),
								s.Status,
								s.Kode
							}), true
						);

					sheet = package.Workbook.Worksheets.Add("Innsyn");
					sheet.Cells.LoadFromCollection(innsyn.
						Select(i => new
						{
							Tidspunkt = i.Dato.ToString("s"),
							i.Navn,
							i.Organisasjon,
							i.Formal
						}),
						 true);

					sheet = package.Workbook.Worksheets.Add("GpsData");
					sheet.Cells.LoadFromCollection(simulagpsdata.Select(s => new
					{
						Fra = s.FraTidspunkt.ToString("s"),
						Til = s.TilTidspunkt.ToString("s"),
						s.Breddegrad,
						s.Lengdegrad,
						s.Hastighet,
						s.Hoyde
					}), true);

					sheet = package.Workbook.Worksheets.Add("Databruk");
					sheet.Cells.LoadFromCollection(simuladatabruk.Select(s => new
					{
						Tidspunkt = s.Tidspunkt.ToString("s"),
						s.TilknyttetTelefonnummer,
						s.PersonNavn,
						s.PersonOrganisasjon,
						s.PersonIdentifikator,
						s.TekniskOrganisasjon,
						s.RettsligFormal
					}), true);

					// Save to file
					await package.SaveAsync();
				}

				return stream.ToArray();
			}
		}
	}
}
