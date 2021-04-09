using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PersonService;

namespace Fhi.Smittesporing.Varsling.Preg
{
    public class PregClientOptions
    {
        public string ApiUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public interface IPregClient
    {
        Task<Person> GetPerson(string fodselsnummer);
    }

    public class PregClient : IPregClient
    {
        private readonly PregClientOptions _options;

        public PregClient(IOptions<PregClientOptions> options)
        {
            _options = options.Value;
        }

        public async Task<Person> GetPerson(string fodselsnummer)
        {
            var client = new PersonServiceClient(
                _options.ApiUrl,
                TimeSpan.FromSeconds(30),
                _options.Username,
                _options.Password);

            var param = new LookupParameters {NIN = fodselsnummer};
            var person = await client.GetPersonAsync(param);

            return person;
        }

    }
}
