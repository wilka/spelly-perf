using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpellyPerfApp
{
    public class SpellingService : ISpellingService
    {
        public IEnumerable<string> GetMisspelledWords(string input)
        {
            var wordLookups = new List<Task<string>>();

            foreach (var inputWord in (input ?? "").Split())
            {
                wordLookups.Add(Task.Run(() =>
                {
                    if (!CorrectlySpelledWords().ToList().Contains(inputWord))
                    {
                        return inputWord;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }));
            }

            var completedTasks = Task.WhenAll(wordLookups);
            foreach (var word in completedTasks.Result)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    yield return word;
                }
            }
        }

        private static IEnumerable<string> CorrectlySpelledWords()
        {
            return TitleCaseWordsInDictionary()
                .Concat(UppercaseWordsInDictionary())
                .Concat(LowercaseWordsInDictionary());
        }

        private static IEnumerable<string> UppercaseWordsInDictionary()
        {
            foreach (var word in LowercaseWordsInDictionary())
            {
                yield return word.ToUpperInvariant();
            }
        }

        private static IEnumerable<string> TitleCaseWordsInDictionary()
        {
            foreach (var word in LowercaseWordsInDictionary())
            {
                var textInfo = new CultureInfo("en-GB", false).TextInfo;
                yield return textInfo.ToTitleCase(word);
            }
        }

        private static IEnumerable<string> LowercaseWordsInDictionary([CallerFilePath] string thisFilePath = "")
        {
            var wordsFile = Path.Combine(Path.GetDirectoryName(thisFilePath), "words.txt");

            using (var stream = File.OpenText(wordsFile))
            {
                string line;
                while (((line = stream.ReadLine()) != null))
                {
                    yield return line;
                }
            }
        }
    }
}