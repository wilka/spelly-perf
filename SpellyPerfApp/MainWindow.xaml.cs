using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ReactiveUI;
using Splat;
using Path = System.IO.Path;


namespace SpellyPerfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(Locator.Current.GetService<ISpellingService>());

            this.WhenActivated(disposableRegistration =>
            {
                Observable.FromEventPattern(h => TextEdit.TextChanged += h, h => TextEdit.TextChanged -= h)
                    .Select(x => ((TextEditor) x.Sender).Text)
                    .BindTo(ViewModel, vm => vm.InputText)
                    .DisposeWith(disposableRegistration);

                this.ViewModel.WhenAnyValue(vm => vm.SpellingMistakes)
                    .Subscribe(spellingErrors =>
                    {
                        var color = new XshdColor(){Underline = true, Foreground = new SimpleHighlightingBrush(Colors.Red)};

                        var keywords = new XshdKeywords
                        {
                            ColorReference = new XshdReference<XshdColor>(color),
                        };
                        foreach (var word in spellingErrors)
                        {
                            keywords.Words.Add(word);
                        }

                        var words = new XshdSyntaxDefinition() 
                        {
                            Elements = 
                            {
                                new XshdRuleSet
                                {
                                    Elements = 
                                    {
                                        keywords
                                    },
                                }
                            },
                        };


                        TextEdit.SyntaxHighlighting = HighlightingLoader.Load(words, HighlightingManager.Instance);
                    })
                    .DisposeWith(disposableRegistration);
            });
        }
        
    }
}
