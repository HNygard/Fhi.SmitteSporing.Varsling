using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class SendSmsVarslerJobb : IPeriodiskJobb
    {
        private readonly Konfig _konfig;
        private readonly ILogger<SendSmsVarslerJobb> _logger;
        private readonly IMediator _mediator;

        public SendSmsVarslerJobb(IOptions<Konfig> konfig, ILogger<SendSmsVarslerJobb> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _konfig = konfig.Value;
        }

        public async Task<bool> UtforJobb(CancellationToken stoppingToken)
        {
            // Denne implementasjonen vil kunne forskyve gyldig tidspunkt dager det byttes mellom sommer-/vintertid
            // men vi anser dette som OK
            var gjeldendeTid = DateTime.Now;
            if (gjeldendeTid - gjeldendeTid.Date <= _konfig.SendEtterKlokken)
            {
                _logger.LogDebug("For tidlig på dagen for utsending av varsler");
                return false;
            }
            if (gjeldendeTid - gjeldendeTid.Date > _konfig.IkkeSendEtterKlokken)
            {
                _logger.LogDebug("For seint på dagen for utsending av varsler");
                return false;
            }

            var behandletTilfelle = await _mediator
                .Send(new SendVarslerForEldsteGodkjente.Command(), stoppingToken);

            return behandletTilfelle;
        }

        public class Konfig : IPeriodiskJobbKonfig
        {
            public JobbIntervallKonfig JobbIntervaller { get; set; }
            public TimeSpan SendEtterKlokken { get; set; } = TimeSpan.FromHours(8);
            public TimeSpan IkkeSendEtterKlokken { get; set; } = TimeSpan.FromHours(22);
        }
    }
}