using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Utils;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Test.TestUtils
{
    public class SmitteVarslingContextBuilder
    {
        private static readonly Random Random = new Random();

        private IArbeidskontekst _arbeidskontekst;

        // NB! Må være unikt per test for å unngå å dele dataset med tester som kjøres i parallell
        private string _dbName;

        private Option<Func<SmitteVarslingContext, Task>> _dataSeeding;

        public SmitteVarslingContextBuilder([CallerMemberName] string dbName = null)
        {
            _dbName = dbName ?? nameof(SmitteVarslingContext) + Random.Next();
        }

        public async Task<SmitteVarslingContext> BuildAsync()
        {
            var options = new DbContextOptionsBuilder<SmitteVarslingContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options;

            await _dataSeeding.MatchSomeAsync(async seeding =>
            {
                await using var dbContext = new SmitteVarslingContext(options, _arbeidskontekst ?? new Arbeidskontekst());
                await seeding(dbContext);
            });

            return new SmitteVarslingContext(options, _arbeidskontekst ?? new Arbeidskontekst());
        }

        public SmitteVarslingContextBuilder MedDataSeeding(Func<SmitteVarslingContext, Task> dataSeeding)
        {
            _dataSeeding = dataSeeding.Some();
            return this;
        }

        public SmitteVarslingContextBuilder MedArbeidskontekts(IArbeidskontekst arbeidskontekst)
        {
            _arbeidskontekst = arbeidskontekst;
            return this;
        }

        public SmitteVarslingContextBuilder MedNavneSuffix(string suffix)
        {
            _dbName += "_" + suffix;
            return this;
        }
    }
}