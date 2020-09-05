using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            if (input.Contains("cheese"))
            {
                return new[] { "cheese" };
            }
            else
            {
                return new string[0];
            }
        }
    }
}
