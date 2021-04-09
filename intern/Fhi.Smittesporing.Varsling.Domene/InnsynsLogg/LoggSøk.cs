using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Domene.InnsynsLogg
{
    public class LoggSøk
    {
        public class Command : IRequest
        {
            public string Fodselsnummer { get; set; }
            public string Navn { get; set; }
            public string Formal { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IInnsynloggRespository _repository;
            private readonly ICryptoManagerFacade _cryptoManager;

            public Handler(IInnsynloggRespository repository, ICryptoManagerFacade cryptoManager)
            {
                _repository = repository;
                _cryptoManager = cryptoManager;
            }

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                await _repository.OpprettOgLagre(new Modeller.Innsyn.Innsynlogg.Innsynlogg
                {
                    Felt = "Fodselsnummer",
                    Hva = _cryptoManager.KrypterUtenBrukerinnsyn(command.Fodselsnummer),
                    Hvem = command.Navn,
                    Hvorfor = command.Formal
                });

                return Unit.Value;
            }
        }
    }
}
