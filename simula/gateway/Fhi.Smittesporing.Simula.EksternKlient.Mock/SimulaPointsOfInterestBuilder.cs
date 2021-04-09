using System;
using System.Collections.Generic;
using System.Linq;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaPointsOfInterestBuilder
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        public static string[] PoiOptions =
        {
            "outside",
            "residential"
        };

        public Dictionary<string, double> Build()
        {
            return PoiOptions
                .Where(x => Rand.NextDouble() > 0.75)
                .ToDictionary(x => x, x => Rand.NextDouble() * 1000);
        }
    }
}