using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class FjerntUtgatteDataJobb : IPeriodiskJobb
    {
        private readonly ILogger<FjerntUtgatteDataJobb> _logger;
        private readonly Konfig _konfig;
        private readonly IMediator _mediator;

        public FjerntUtgatteDataJobb(IOptions<Konfig> konfig, ILogger<FjerntUtgatteDataJobb> logger, IMediator mediator)
        {
            _konfig = konfig.Value;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<bool> UtforJobb(CancellationToken stoppingToken)
        {
            //step 0; beregn utgått tidspunkt
            var utgattTidspunkt = DateTime.Now - _konfig.SlettDataEldreEnn;

            //step 1: fjern alle data eldre enn 'utgattTidspunkt
            int antallfjernet = await _mediator.Send(new FjernUtgatteIndekspasienter.Command
            {
                UtgattTidspunkt = utgattTidspunkt
            }, stoppingToken);

            if (antallfjernet > 0)
            {
                _logger.LogInformation($" Antall elementer fjernet: {antallfjernet}");
                return true;
            }
            else
            {
                _logger.LogDebug("Ungen utgåtte elementer ble funnet for sletting.");
                return false;
            }
        }

        public class Konfig : IPeriodiskJobbKonfig
        {
            public JobbIntervallKonfig JobbIntervaller { get; set; }
            public TimeSpan SlettDataEldreEnn { get; set; } = TimeSpan.FromDays(30);
        }
    }
}