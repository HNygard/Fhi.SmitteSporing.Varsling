using Fhi.Smittesporing.Varsling.Datalag;
using FluentAssertions;
using Optional;
using Optional.Unsafe;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.DataLag
{
    public class TelefonNormalFacadeTester 
    {
        [Theory]
        [Trait("TelefonNormalCrypto", "Test")]
        [InlineData("12345678" ,"+4712345678")]
        [InlineData("12345600", "+4712345600")]
        [InlineData("123 456 78", "+4712345678")]
        [InlineData(" 123 456 78 ", "+4712345678")]
        [InlineData("123-456-78", "+4712345678")]
        [InlineData(" 123-456-78", "+4712345678")]
        public void UtenPrefixTest(string orgTelefonnummer, string forventetNormalisertTlf)
        {
            var normalisert = new TelefonNormalFacade().NormaliserStrict(orgTelefonnummer);

            //sjekk at som forventet
            Assert.Equal(forventetNormalisertTlf, normalisert.ValueOrFailure());
        }

        [Theory]
        [Trait("TelefonNormalCrypto", "Test")]
        [InlineData("+4712345678", "+4712345678")]
        [InlineData("+4812345678", "+4812345678")]
        [InlineData("004812345678", "+4812345678")]
        [InlineData("+10112345678", "+10112345678")]
        [InlineData("0064 21 345 687", "+6421345687")]
        [InlineData("+64 21 345 687", "+6421345687")]
        [InlineData("+676 12 123", "+67612123")]
        public void MedPrefixTest(string orgTelefonnummer, string forventetNormalisertTlf)
        {
            var normalisert = new TelefonNormalFacade().NormaliserStrict(orgTelefonnummer);

            //sjekk at som forventet
            Assert.Equal(forventetNormalisertTlf, normalisert.ValueOrFailure());
        }

        [Theory]
        [Trait("TelefonNormalCrypto", "Test")]
        [InlineData("")]
        [InlineData(" ")]
        public void TomtTelefonnummerForbliTomt(string orgTelefonnummer)
        {
            var normalisert = new TelefonNormalFacade().NormaliserStrict(orgTelefonnummer);
            normalisert.ValueOrException().Should().Be("Tomt telefonnummer");
        }

        [Theory]
        [Trait("TelefonNormalCrypto", "Test")]
        [InlineData("1234567")]
        public void UGyldigTelefonnummerKasterException(string telefonnummer)
        {
            //step 1 Normaliser og krypter
            var normalisert = new TelefonNormalFacade().NormaliserStrict(telefonnummer);
            normalisert.ValueOrException().Should().Be("Ugyldig telefonnummer");
        }
    }
}