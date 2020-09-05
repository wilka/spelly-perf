using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SpellyPerfApp
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(ISpellingService spellingService)
        {
            this.WhenAnyValue(me => me.InputText)
                .Select(text => spellingService.GetMisspelledWords(text).ToArray())
                .ToPropertyEx(this, me => me.SpellingMistakes);
        }


        [Reactive]
        public string InputText { get; set; }

        [ObservableAsProperty]
        public string[] SpellingMistakes { get; }

        
    }
}