using System;
using System.Threading.Tasks;
using PersonService;

namespace Fhi.Smittesporing.Varsling.Preg
{
    public class PregClientMock : IPregClient
    {
        public static Random Random = new Random();
        public Task<Person> GetPerson(string fodselsnummer)
        {
            return Task.FromResult(Random.NextDouble() > 0.05 ? new Person
            {
                DateOfBirth = DateTime.Now.AddYears(-Random.Next(12, 99)),
                Addresses = new[]
                {
                    new Address
                    {
                        PostalType = Random.Next(0, 8)
                    }
                }
            } : null);
        }
    }
}