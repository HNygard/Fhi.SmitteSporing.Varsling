using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Preg;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IPregFacade
    {
        Task<Option<PregPerson>> FinnPerson(string identifikator);
    }
}