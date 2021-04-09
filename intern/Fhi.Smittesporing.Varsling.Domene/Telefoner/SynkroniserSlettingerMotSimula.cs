using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fhi.Smittesporing.Varsling.Domene.Telefoner
{
    public class SynkroniserSlettingerMotSimula
    {
        public class Command : IRequest<int>
        {

        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly ISimulaFacade _simulaFacade;
            private readonly ITelefonRespository _telefonRespository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;
            private readonly ILogger<SynkroniserSlettingerMotSimula> _logger;

            public Handler(ISimulaFacade simulaFacade, ITelefonRespository telefonRespository, ICryptoManagerFacade cryptoManagerFacade, ILogger<SynkroniserSlettingerMotSimula> logger)
            {
                _simulaFacade = simulaFacade;
                _telefonRespository = telefonRespository;
                _cryptoManagerFacade = cryptoManagerFacade;
                _logger = logger;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var alleTelefoner = await _telefonRespository.HentAlleTelefoner();

                if (!alleTelefoner.Any())
                {
                    return 0;
                }

                _logger.LogInformation($"Sjekker {alleTelefoner.Count} telefoner for sletting hos Simula.");

                var dekryptTlfMap = alleTelefoner.ToDictionary(
                    x => _cryptoManagerFacade.DekrypterUtenBrukerinnsyn(x.Telefonnummer),
                    x => x
                );

                var slettedeTlfer = await _simulaFacade.SjekkSlettinger(dekryptTlfMap.Keys);
                var antallSlettet = 0;

                foreach (var slettetTlf in slettedeTlfer)
                {
                    await _telefonRespository.SlettTelefonMedTilknyttetInnhold(dekryptTlfMap[slettetTlf]);
                    antallSlettet++;
                }

                _logger.LogInformation($"Slettet data for {antallSlettet} telefoner.");

                return antallSlettet;
            }
        }
    }
}