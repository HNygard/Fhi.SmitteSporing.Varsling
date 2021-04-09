using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class LastInnSmittekontakterJobb : IPeriodiskJobb
    {
        private readonly ILogger<LastInnSmittekontakterJobb> _logger;
        private readonly IMediator _mediator;
        private readonly Konfig _konfig;

        public LastInnSmittekontakterJobb(ILogger<LastInnSmittekontakterJobb> logger, IMediator mediator, IOptions<Konfig> konfig)
        {
            _logger = logger;
            _mediator = mediator;
            _konfig = konfig.Value;
        }

        public async Task<bool> UtforJobb(CancellationToken stoppingToken)
        {
            //step 0: hent nye  smittekontakter fra Simula
            var antallIndekspasienterKontaktsjekket = await _mediator.Send(new LastInnSmittekontakter.Command
            {
                MaksAntallDagerBakover = _konfig.MaksAntallDagerBakover,
                AntallDagerForProvedato = _konfig.AntallDagerForProvedato
            }, stoppingToken);

            if (antallIndekspasienterKontaktsjekket > 0)
                _logger.LogInformation($"Antall indekspasienter sjekket for kontakt: {antallIndekspasienterKontaktsjekket}");

            return antallIndekspasienterKontaktsjekket > 0;
        }

        public class Konfig : IPeriodiskJobbKonfig
        {
            public JobbIntervallKonfig JobbIntervaller { get; set; }
            public int MaksAntallDagerBakover { get; set; } = 14;
            public int AntallDagerForProvedato { get; set; } = 7;
        }
    }
}