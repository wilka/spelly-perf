using System.Collections.Generic;

namespace SpellyPerfApp
{
    public interface ISpellingService
    {
        IEnumerable<string> GetMisspelledWords(string input);
    }
}