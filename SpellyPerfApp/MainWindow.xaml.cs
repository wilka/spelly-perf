using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
                this.Bind(ViewModel, vm => vm.InputText, view => view.InputBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel, vm => vm.OutputText, view => view.OutputBox.Text)
                    .DisposeWith(disposableRegistration);

                this.WhenAnyValue(x => x.OutputBox.Text)
                    .Subscribe(x =>
                    {
                        var color = new XshdColor(){Underline = true, Foreground = new SimpleHighlightingBrush(Colors.Red)};
                        var words = new XshdSyntaxDefinition() 
                        {
                            Elements = 
                            {
                                new XshdRuleSet
                                {
                                    Elements = 
                                    {
                                        new XshdKeywords 
                                        {
                                            Words = {"cheese"}, ColorReference = new XshdReference<XshdColor>(color),
                                        }
                                    },
                                }
                            },
                        };

                        
                        OutputBox.SyntaxHighlighting = HighlightingLoader.Load(words, HighlightingManager.Instance);
                    })
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
