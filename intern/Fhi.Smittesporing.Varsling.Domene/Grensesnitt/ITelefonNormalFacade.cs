using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ITelefonNormalFacade
    {
        Option<string, string> NormaliserStrict(string telefonnummer);
        string Normaliser(string telefonnummer);
    }
}
