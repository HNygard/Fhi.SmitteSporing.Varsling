using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Preg;
using Fhi.Smittesporing.Varsling.Preg;
using Optional;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public class PregFacade : IPregFacade
    {
        private static readonly int[] HemmeligAdressekoder = { 6, 7 };
        private readonly IPregClient _pregClient;

        public PregFacade(IPregClient pregClient)
        {
            _pregClient = pregClient;
        }

        public async Task<Option<PregPerson>> FinnPerson(string identifikator)
        {
            var pregResultat = await _pregClient.GetPerson(identifikator);

            return pregResultat
                .SomeNotNull()
                .Map(r => new PregPerson
                {
                    Identifikator = r.NIN,
                    Fodselsdato = r.DateOfBirth.ToOption(),
                    HarHemmeligAdresse = r.Addresses.Any(a => HemmeligAdressekoder.Contains(a.PostalType ?? -1))
                });
        }
    }
}