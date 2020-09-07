using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using SpellyPerfApp;

namespace SpellyPerfBenchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Make sure you run this with `dotnet run -c Release` so you don't accidentally run the debug build
            BenchmarkRunner.Run(typeof(Program).Assembly, DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
        }

        [ShortRunJob]
        public class SpellingSpeed
        {
            [ParamsSource(nameof(ValuesForTextToSpellCheck))]
            public string TextToSpellCheck { get; set; }

            public static IEnumerable<string> ValuesForTextToSpellCheck => new[] 
            { 
                "Hello",
                "Hello there",
                "Hellooo",
                "Hellooo to you",
            };

            private SpellingService service = new SpellingService();

            public SpellingSpeed()
            {
                
            }

            [Benchmark]
            public string[] Benchmark() => service.GetMisspelledWords(TextToSpellCheck).ToArray();
        }
    }
}
