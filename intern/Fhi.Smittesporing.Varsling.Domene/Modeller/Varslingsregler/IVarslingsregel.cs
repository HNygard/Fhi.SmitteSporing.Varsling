namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler
{
    public interface IVarslingsregel
    {
        bool KanVarsles(Smittekontakt kontakt);
        string Navn { get; }
        string Beskrivelse { get; }
    }
}