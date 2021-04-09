using System;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    /// <summary>
    /// Modeller som implementerer dette grensesnittet får verdi for OpprettetAv satt ved
    /// lagring via logikk i DbContext
    /// </summary>
    public interface IOpprettetAv
    {
        DateTime Created { get; set; }
        string OpprettetAv { get; set; }
    }

    /// <summary>
    /// Modeller som implementerer dette grensesnittet får verdi for OppdatertAv satt ved
    /// lagring via logikk i DbContext
    /// </summary>
    public interface ISistOppdatertAv
    {
        string SistOppdatertAv { get; set; }
        DateTime? SistOppdatert { get; set; }
    }
}