using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SpellyPerfApp
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            this.WhenAnyValue(me => me.InputText)
                .ToPropertyEx(this, me => me.OutputText);
        }

        [Reactive]
        public string InputText { get; set; }

        [ObservableAsProperty]
        public string OutputText { get; }
    }
}