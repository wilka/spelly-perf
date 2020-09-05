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
            ViewModel = new MainWindowViewModel();
            
            this.WhenActivated(disposableRegistration =>
            {
                Observable.FromEventPattern(h => TextEdit.TextChanged += h, h => TextEdit.TextChanged -= h)
                    .Select(x => ((TextEditor)x.Sender).Text)
                    .Subscribe(currentText =>
                    {
                        var color = new XshdColor(){Underline = true, Foreground = new SimpleHighlightingBrush(Colors.Red)};

                        var keywords = new XshdKeywords
                        {
                            ColorReference = new XshdReference<XshdColor>(color),
                        };
                        foreach (var word in BadSpelling(currentText))
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
        private string[] BadSpelling(string input)
        {
            var wrongWords = new List<string>();

            foreach (var inputWord in input.Split())
            {
                if (!CorrectlySpelledWords().ToList().Contains(inputWord))
                {
                    if (!string.IsNullOrWhiteSpace(inputWord))
                    {
                        wrongWords.Add(inputWord);
                    }
                }
            }

            return wrongWords.ToArray();
        }

        private IEnumerable<string> CorrectlySpelledWords()
        {
            return TitleCaseWordsInDictionary()
                .Concat(UppercaseWordsInDictionary())
                .Concat(LowercaseWordsInDictionary());
        }

        private IEnumerable<string> UppercaseWordsInDictionary()
        {
            foreach(var word in LowercaseWordsInDictionary())
            {
                yield return word.ToUpperInvariant();
            }
        }

        private IEnumerable<string> TitleCaseWordsInDictionary()
        {
            foreach (var word in LowercaseWordsInDictionary())
            {
                var  textInfo = new CultureInfo("en-GB", false).TextInfo;
                yield return textInfo.ToTitleCase(word);
            }
        }

        private IEnumerable<string> LowercaseWordsInDictionary([CallerFilePath] string thisFilePath = "")
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
