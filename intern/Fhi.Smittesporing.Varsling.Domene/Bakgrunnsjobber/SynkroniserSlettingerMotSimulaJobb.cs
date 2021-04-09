using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Telefoner;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class SynkroniserSlettingerMotSimulaJobb : IPeriodiskJobb
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SynkroniserSlettingerMotSimulaJobb> _logger;

        public SynkroniserSlettingerMotSimulaJobb(IMediator mediator, ILogger<SynkroniserSlettingerMotSimulaJobb> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<bool> UtforJobb(CancellationToken stoppingToken)
        {
            var antallSlettet = await _mediator.Send(new SynkroniserSlettingerMotSimula.Command(), stoppingToken);

            if (antallSlettet > 0)
            {
                _logger.LogInformation($"Slettet data for {antallSlettet} telefoner etter synkronisering mot Simula.");
            }

            return antallSlettet > 0;
        }

        public class Konfig : IPeriodiskJobbKonfig
        {
            public JobbIntervallKonfig JobbIntervaller { get; set; }
        }
    }
}