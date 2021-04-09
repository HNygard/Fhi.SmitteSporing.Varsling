using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class RegistrerTelefonnummer
    {
        public class Command : IRequest
        {
            public int IndekspasientId { get; set; }
            public string Telefonnummer { get; set; }

            public bool IkkeManueltFunnetKontaktInfo { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly ITelefonRespository _telefonRespository;
            private readonly ITelefonNormalFacade _telefonNormalFacade;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(IIndekspasientRepository indekspasientRepository, ITelefonNormalFacade telefonNormalFacade, ITelefonRespository telefonRespository, ICryptoManagerFacade cryptoManagerFacade)
            {
                _indekspasientRepository = indekspasientRepository;
                _telefonNormalFacade = telefonNormalFacade;
                _telefonRespository = telefonRespository;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var indekspasient = (await _indekspasientRepository.HentForIdInkluderTelefon(request.IndekspasientId))
                    .ValueOr(() => throw new Exception("Indekspasient for oppgitt ID finnes ikke"));

                if (indekspasient.Status != IndekspasientStatus.KontaktInfoMangler)
                {
                    throw new Exception("Kan bare registrere telefonnummer på pasienter som mangler kontaktinfo");
                }

                if (request.IkkeManueltFunnetKontaktInfo)
                {
                    indekspasient.SlettData();
                }
                else if (!string.IsNullOrEmpty(request.Telefonnummer))
                {
                    var normTlf = _telefonNormalFacade.NormaliserStrict(request.Telefonnummer)
                        .ValueOr(errorMessage => throw new Exception(errorMessage));
                    var kryptTlf = _cryptoManagerFacade.KrypterUtenBrukerinnsyn(normTlf);

                    indekspasient.Status = IndekspasientStatus.Registrert;
                    indekspasient.Telefon = (await _telefonRespository.FinnForTelefonnummer(kryptTlf))
                        .ValueOr(new Telefon
                        {
                            Telefonnummer = kryptTlf
                        });
                }
                else
                {
                    throw new Exception("Hverken telefonnummer eller IkkeManueltFunnetKontaktInfo er angitt.");
                }

                await _indekspasientRepository.Lagre();

                return Unit.Value;
            }
        }
    }
}