using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Domene.SmsVarsler;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.SmsVarsler
{
    public class OppdaterSmsStatuserTester
    {
        [Fact]
        public async Task Handle_GittIngenHendelserBehandlet_HenterHendelserFraStart()
        {
            var automocker = new AutoMocker();

            var target = automocker.CreateInstance<OppdaterSmsStatuser.Handler>();

            await target.Handle(new OppdaterSmsStatuser.Command(), new CancellationToken());

            automocker.Verify<ISmsTjenesteFacade>(t => t.HentStatusoppdateringerEtterLopenummer(It.Is<int>(i => i <= 0), It.IsAny<int>()));
        }

        [Fact]
        public async Task Handle_GittTidligereHendelserBehandlet_HenterFraForrigeLopenummer()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IApplikasjonsinnstillingRepository, Task<Option<int>>>(x => x.HentInnstilling<int>("SisteSmsOppdateringLopenummer"))
                .ReturnsAsync(42.Some());

            var target = automocker.CreateInstance<OppdaterSmsStatuser.Handler>();

            await target.Handle(new OppdaterSmsStatuser.Command(), new CancellationToken());

            automocker.Verify<ISmsTjenesteFacade>(t => t.HentStatusoppdateringerEtterLopenummer(42, It.IsAny<int>()));
        }

        [Fact]
        public async Task Handle_GittNyeHendelser_OppdatereAppInnstilling()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<ISmsTjenesteFacade, Task<IEnumerable<SmsStatusoppdatering>>>(x => x.HentStatusoppdateringerEtterLopenummer(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new []
                {
                    new SmsStatusoppdatering
                    {
                        Loepenummer = 42,
                        SmsUtsendingReferanse = Guid.NewGuid().Some(),
                        GjeldeneStatus = SmsStatus.Levert,
                        Beskrivelse = "OK",
                        AntallSegmenter = 2.Some(),
                        Tidspunkt = DateTime.Now.AddMinutes(-2)
                    }
                });

            var target = automocker.CreateInstance<OppdaterSmsStatuser.Handler>();

            await target.Handle(new OppdaterSmsStatuser.Command(), new CancellationToken());

            automocker.Verify<IApplikasjonsinnstillingRepository>(x => x.SettInnstilling("SisteSmsOppdateringLopenummer", 42));
        }

        [Theory]
        [InlineData(SmsStatus.Sendt)]
        [InlineData(SmsStatus.DelvisLevert)]
        [InlineData(SmsStatus.Levert)]
        [InlineData(SmsStatus.Feilet)]
        public async Task Handle_GittNyeHendelser_OppdatererTilhørendeSmsVarsel(SmsStatus status)
        {
            var automocker = new AutoMocker();

            var varselRef = Guid.NewGuid();

            var varsel = new SmsVarsel
            {
                Status = SmsStatus.Opprettet,
                Referanse = varselRef
            };

            automocker
                .Setup<ISmsTjenesteFacade, Task<IEnumerable<SmsStatusoppdatering>>>(x => x.HentStatusoppdateringerEtterLopenummer(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new[]
                {
                    new SmsStatusoppdatering
                    {
                        Loepenummer = 42,
                        SmsUtsendingReferanse = varselRef.Some(),
                        GjeldeneStatus = status,
                        Beskrivelse = status.ToString(),
                        AntallSegmenter = 2.Some(),
                        Tidspunkt = DateTime.Now.AddMinutes(-2)
                    }
                });

            automocker
                .Setup<ISmsVarselRepository, Task<Option<SmsVarsel>>>(x => x.FinnForReferanse(varselRef))
                .ReturnsAsync(varsel.Some());

            var target = automocker.CreateInstance<OppdaterSmsStatuser.Handler>();

            await target.Handle(new OppdaterSmsStatuser.Command(), new CancellationToken());

            varsel.Status.Should().Be(status);
            automocker.Verify<ISmsVarselRepository>(x => x.Lagre());
        }
    }
}
