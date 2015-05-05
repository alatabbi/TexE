using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.Folding;
using AvalonEdit.Sample;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
using Classes;
using SpellCheckAvalonEdit;
using ICSharpCode.AvalonEdit.Edi.Intellisense;
namespace CControl
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class CUserControl : UserControl
    {
        public ICSharpCode.AvalonEdit.TextEditor TextEditor
        {
            get
            {
                return this.textEditor;
            }
        }
        
        public CUserControl()
        {
            InitializeComponent();
            //this.textEditor.SyntaxHighlighting
            IHighlightingDefinition latexHighlighting;

            using (Stream s = typeof(CUserControl).Assembly.GetManifestResourceStream("ATABBI.TexE.Resources.tex.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    latexHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            HighlightingManager.Instance.RegisterHighlighting("tex", new string[] { ".cool" }, latexHighlighting);
            InitializeComponent();

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("tex");
            textEditor.SyntaxHighlighting = latexHighlighting;

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            textEditor.TextArea.KeyDown += this.TextArea_KeyDown;
         
            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            this.Fold();
            this.textEditor.Focus();

            this.textEditor.TextArea.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
          
           // textEditor.TextArea.TextView.LineTransformers.Add(new SpellingErrorColorizer());
 
        }

        private CompletionWindow _completionWindow;
        private void textEditor_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            //var aa= new IBackgroundRenderer
            //this.textEditor.TextArea.TextView.InvalidateVisual();
            textEditor.TextArea.TextView.BackgroundRenderers.Clear();
            textEditor.TextArea.TextView.BackgroundRenderers.Add(new XBackgroundRenderer(this.textEditor));
            
        }       

        void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            
            if ( e.Key == Key.F11)// && (Keyboard.IsKeyDown( Key.LeftCtrl)||Keyboard.IsKeyDown( Key.RightCtrl)))
            {
                this.Compelet();
            }
            if (e.Key == Key.F12)// && (Keyboard.IsKeyDown( Key.LeftCtrl)||Keyboard.IsKeyDown( Key.RightCtrl)))
            {
                this.InsertSnippet(); 
            }

        }       
         
        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            ICompletionWindowResolver resolver = new CompletionWindowResolver(this.textEditor.Text, this.textEditor.TextArea.Caret.Offset, e.Text, this.textEditor);
            _completionWindow = resolver.Resolve();
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        void textEditor_TextArea_TextEntered1(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == @"\")
            {
                    // open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    // provide AvalonEdit with the data:
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (string s in TexHelper.TexEntries()["Greek letters"])
                    {
                        MyCompletionData d = new MyCompletionData(s);
                        
                        data.Add(new MyCompletionData(s.Substring(1,s.Length-1)));  
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate
                    {
                        completionWindow = null;
                    };
            }
        }
        void textEditor_TextArea_TextEntering1(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        private void Compelet()
        {
            Regex re = new Regex(@"\\bibliography\{(\w+)\}");

            Match m = re.Match(this.textEditor.Document.Text);
            string path = @"Z:\30\1\ref.bib";// m.Groups[1].Captures[0].ToString() + " .bib";

            //if (e.Text == @"\ref{")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                StringBuilder entry = new StringBuilder();

                foreach (BibTex b in BibTex.Parse(path))
                {
                    MyCompletionData d = new MyCompletionData(b.UID);
                    //d.Name = "name";
                    //d.Description = "des";
                    //d.Content = "content";
                    data.Add(d);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        private void  InsertSnippet()
        {
            var caption = new SnippetReplaceableTextElement { Text = "caption" };
            var label = new SnippetReplaceableTextElement { Text = "label" };
            Snippet ss = new Snippet();
            Snippet snippet = new Snippet
            {

                //\begin{figure}[htbp]\centering $0\caption{${1:caption}}\label{${2:$(unless yas/modified-p (reftex-label nil 'dont-insert))}}\end{figure}
                Elements = {
                            new SnippetTextElement { Text =  @"\begin{figure}[htbp]"+ Environment.NewLine+ @"\centering"+ Environment.NewLine+ @"\caption{" },
                            new SnippetReplaceableTextElement { Text = "caption" },
                            //caption,
                            //new SnippetReplaceableTextElement { Text = "caption" },
                            //caption,
                            new SnippetTextElement { Text = "}"+ Environment.NewLine+ @"\label{"},
                            //new SnippetBoundElement { TargetElement = label },
                             //label,
                            new SnippetReplaceableTextElement { Text = "label" },
                            new SnippetTextElement { Text =    @"}"+ Environment.NewLine + @"\end{figure}" }
                            }
            };
            snippet.Insert(textEditor.TextArea);

        }

        #region Folding
        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;

        void Fold()
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {

                foldingStrategy = new BeginEndFoldingStrategy();
                textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

                if (false)
                {
                    switch (textEditor.SyntaxHighlighting.Name)
                    {
                        case "XML":
                            foldingStrategy = new XmlFoldingStrategy();
                            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                            break;
                        case "C#":
                        case "C++":
                        case "PHP":
                        case "Java":
                            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                            foldingStrategy = new BraceFoldingStrategy();
                            break;
                        default:
                            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                            foldingStrategy = null;
                            break;
                    }
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }

        }
        #endregion
    }
}
