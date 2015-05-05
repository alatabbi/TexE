using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Folding;
using AvalonEdit.Sample;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Edi.Intellisense;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.AddIn;
using ATABBI.TexE.AddIns;
using ATABBI.TexE.Forms;
using ICSharpCode.AvalonEdit.Search;
using Classes;
using ICSharpCode.AvalonEdit.Rendering;
using NHunspellComponent.Spelling;
namespace ATABBI.TexE
{
    public partial class TextDocument  : DockContent
    {
        #region MyRegion
        // workaround of RichTextbox control's bug:
        // If load file before the control showed, all the text format will be lost
        // re-load the file after it get showed.
        //private bool m_resetText = true;
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);

        //    if (m_resetText)
        //    {
        //        m_resetText = false;
        //        FileName = FileName;
        //    }
        //} 
        #endregion
        
        public TextDocument()
        {
            InitializeComponent();
            nextRefreshTime = DateTime.Now;
            refreshInterval = TimeSpan.FromSeconds(2);
            //DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            //foldingUpdateTimer.Interval = TimeSpan.FromSeconds(3);
            //foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            //foldingUpdateTimer.Start();   
            //txtEditor.TextArea.TextView.LineTransformers.Add(new SpellingErrorColorizer());
        }
        
        private void TextDoc_Load(object sender, EventArgs e)
        {

            this.HighlightText();
            this.Fold(); 
            //tb.TextChanged += new EventHandler(tb_TextChanged);
            txtEditor.TextArea.KeyDown += this.TextArea_KeyDown;
            txtEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            txtEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            txtEditor.TextArea.SelectionChanged += textEditor_TextArea_SelectionChanged;
            this.txtEditor.TextArea.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
            txtEditor.TextChanged += new EventHandler(txtEditor_TextChanged);
            this.txtEditor.Focus();

            //string userPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NetSpell");
            //string filePath = Path.Combine(userPath, "user.dic");
            
            this.txtEditor.MouseRightButtonDown += this.TextAreaMouseRightButtonDown;
            
            // this.txtEditor.ContextMenu= this.ContextMenuStrip;
            //this.txtEditor.TextAreaContextMenuStrip = this.contextMenuStrip1;
            //this.txtEditor.ContextMenu.Items.Add(item); 

            //this.txtEditor.MouseHover += new System.Windows.Input.MouseEventHandler(this.TextEditorMouseHover);
            txtEditor.TextArea.TextView.LineTransformers.Add(new LineSpellColorizer(this.txtEditor));
            this.SpellCheckerTextBox.TextChanged += new EventHandler(customTextBox1_TextChanged);
            
        }
        
        System.Windows.Controls.ToolTip tt = new System.Windows.Controls.ToolTip();
        
        void TextEditorMouseHover(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pos = txtEditor.GetPositionFromPoint(e.GetPosition(txtEditor));
            if (pos != null)
            {
                tt.PlacementTarget = this.txtEditor; // required for property inheritance
                tt.Content = pos.ToString();
                tt.IsOpen = true;
                e.Handled = true;
            }
        }

        void TextAreaMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowSpellSuggestion();
            //var position = txtEditor.GetPositionFromPoint(e.GetPosition(txtEditor));
            //if (position.HasValue)
            //{
            //    txtEditor.TextArea.Caret.Position = position.Value;
            //    contextMenuStrip1.Visible = true;
            //    //Point p = new Point((int)e.GetPosition(txtEditor).X, (int)e.GetPosition(txtEditor).Y);
            //    System.Windows.Point ppp = this.txtEditor.PointToScreen(e.GetPosition(txtEditor));
            //    contextMenuStrip1.Show((int)ppp.X, (int)ppp.Y);

            //    this.SpellChecker.Suggest(this.txtEditor.SelectedText);
            //    ((ToolStripMenuItem)this.contextMenuStrip1.Items[0]).DropDownItems.Clear();
            //    foreach (string s in this.SpellChecker.Suggestions)
            //    {
            //        ToolStripMenuItem sug = new ToolStripMenuItem(s, null, SuggestionItem_Click);

            //        ((ToolStripMenuItem)this.contextMenuStrip1.Items[0]).DropDownItems.Add(sug);
            //    }                
            //}
        }

        #region Fields
        private string m_fileName = string.Empty;
        public string FileName
        {
            get    { return m_fileName;    }
            set
            {
                if (value != string.Empty)
                {
                    m_fileName = value;
                    //Stream s = new FileStream(value, FileMode.Open);
                    //FileInfo efInfo = new FileInfo(value);
                    //string fext = efInfo.Extension.ToUpper();
                    //this.ToolTipText = value;
                    //this.txtEditor.Load(m_fileName);
                    //s.Close();
                }
                //m_fileName = value;
                //this.ToolTipText = value;
            }
        }
        public FileInfo CurrentFileInfo
        {
            get
            {
                return new FileInfo(this.m_fileName);
            }
        }
        private CompletionWindow _completionWindow;
        public ICSharpCode.AvalonEdit.TextEditor txtEditor
        {
            get
            {
                return this.cUserControl1.TextEditor;
            }
        }
        #endregion

        #region Methods
        protected override string GetPersistString()
        {
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
            return GetType().ToString() + "," + FileName + "," + Text;
        }
        public void SaveFile()
        {
            try
            {
                this.txtEditor.Save(this.m_fileName);
                this.ToolTipText = m_fileName;
                this.Text  = Path.GetFileName(m_fileName);
            }
            catch (SystemException ee)
            {
                string ss = ee.Message;
            }
        }
        public void LoadFile()
        {
            if (m_fileName != string.Empty)
            {
                this.ToolTipText = m_fileName;
                this.txtEditor.Load(m_fileName);
            }
        }
        private void track()
        {
            int newCaret = this.txtEditor.TextArea.Caret.Offset;
            int line = txtEditor.TextArea.Caret.Line;
            int column = txtEditor.TextArea.Caret.Column;
            this.toolStripLabelTracker.Text = string.Format("Ln {0}, Col {1}", line, column);
        }
        #endregion

        #region Handlers
        void txtEditor_TextChanged(object sender, EventArgs e)
        {
            if (!this.Text.EndsWith("*"))
                this.Text  += "*";
            this.RefreshFolding();
        }
        private void menuItemCheckTest_Click(object sender, System.EventArgs e)
        {
            //this.SpellChecker.Suggest(this.txtEditor.SelectedText);
            //ICSharpCode.SharpDevelop.Editor.DocumentUtilitites.GetWordBeforeCaret
            //ColorizSpellEdit spell = new ColorizSpellEdit(this.txtEditor);
            //spell.run( txtEditor.Document.GetLineByOffset(  txtEditor.TextArea.Caret.Offset));
            //txtEditor.TextArea.TextView.LineTransformers.Add(new LineSpellColorizer(txtEditor.Document.GetLineByOffset(txtEditor.TextArea.Caret.Offset).LineNumber));
            //txtEditor.TextArea.TextView.LineTransformers.Add( new ColorizSpellEdit(this.txtEditor));
            ////new ColorizKeywordEdit(txtEditor));
            ////this.txtEditor.TextArea.TextView.Redraw();
            //return;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged (e);
            if (FileName == string.Empty)
                this.txtEditor.Text = Text;
        }       
        
        void textEditor_TextArea_SelectionChanged(object sender, EventArgs e)
        {
            tt.IsOpen = false;

            if (this.txtEditor.SelectedText.Length > 0)
            {
                tt.PlacementTarget = this.txtEditor; // required for property inheritance

                tt.IsOpen = true;

                List<string> sug = NHunspellWrapper.Instance.Hunspeller.Suggest(this.txtEditor.SelectedText);
                if (!NHunspellWrapper.Instance.Hunspeller.Spell(this.txtEditor.SelectedText))
                {
                    if (sug.Count>0)
                        tt.Content = string.Format("Do you mean '{0}'? \r\nPress F7 for more suggestions", NHunspellWrapper.Instance.Hunspeller.Suggest(this.txtEditor.SelectedText)[0]);
                }
                else
                {
                    tt.Content = this.txtEditor.SelectedText;
                }
                // e.Handled = true;
            }
            //this.ShowSpellSuggestion();
        }
        
        void Caret_PositionChanged(object sender, EventArgs e)
        {
            this.txtEditor.TextArea.TextView.BackgroundRenderers.Clear();
            txtEditor.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this.txtEditor));

           

            this.HighlightBrackets();
        }
              
        void TextArea_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F11)// && (Keyboard.IsKeyDown( Key.LeftCtrl)||Keyboard.IsKeyDown( Key.RightCtrl)))
            {
                this.Compelet();
            }
            if (e.Key == Key.F12)// && (Keyboard.IsKeyDown( Key.LeftCtrl)||Keyboard.IsKeyDown( Key.RightCtrl)))
            {
                this.InsertSnippet();
            }

        }

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            ICompletionWindowResolver resolver = new CompletionWindowResolver(this.txtEditor, e.Text);
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
                completionWindow = new CompletionWindow(this.txtEditor.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                string[] sss = new string[] { "b", "a" };
                foreach (string s in sss)// TexHelper.TexEntries()["Greek letters"])
                {
                    MyCompletionData d = new MyCompletionData(s);

                    data.Add(new MyCompletionData(s.Substring(1, s.Length - 1)));
                }
                completionWindow.CloseAutomatically = true;
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
        #endregion

        #region AutoCompletion
        CompletionWindow completionWindow;

        private void Compelet()
        {
            string path = "";
            Regex reBiblio = new Regex(@"(?<!%)(?<=\\bibliography\{)([^\{]+)(?=\})", RegexOptions.IgnoreCase);
            Match m = reBiblio.Match(txtEditor.Text);
            if (m.Success)
                path = this.CurrentFileInfo.Directory + "\\" + m.Groups[1].Captures[0].Value.ToString() + ".bib";
            if (!File.Exists(path))
                return;

            //if (e.Text == @"\ref{")
            //{
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(txtEditor.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                StringBuilder entry = new StringBuilder();

                foreach (BibTex b in BibTex.Parse(path))
                {
                    MyCompletionData d = new MyCompletionData(b.UID);
                    d.Data = b.Text;
                    //d.Description = "des";
                    //d.Content = "content";
                    data.Add(d);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            //}
        }

        #endregion

        #region Snippets

        private void InsertSnippet()
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
            snippet.Insert(this.txtEditor.TextArea);

        }
        #endregion

        #region Highlighting 
        void HighlightText()
        {
            IHighlightingDefinition latexHighlighting;

            using (Stream s = typeof(TextDocument).Assembly.GetManifestResourceStream("ATABBI.TexE.Resources.tex.xshd"))
            {
                if (s == null)
                {
                    return;
                    throw new InvalidOperationException("Could not find embedded resource");
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    latexHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            HighlightingManager.Instance.RegisterHighlighting("tex", new string[] { ".cool" }, latexHighlighting);


            this.txtEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("tex");
            txtEditor.SyntaxHighlighting = latexHighlighting;

            
        
        }
       
        private void HighlightBrackets()
        {
            CSharpBracketSearcher BracketSearcher = new CSharpBracketSearcher();
            var bracketSearchResult = BracketSearcher.SearchBracket(this.txtEditor.Document, this.txtEditor.TextArea.Caret.Offset);
            if (bracketSearchResult!= null)
            {
                BracketHighlightRenderer br = new BracketHighlightRenderer(txtEditor.TextArea.TextView);
                br.SetHighlight(bracketSearchResult);
                BracketHighlightRenderer.ApplyCustomizationsToRendering(br, CustomizedHighlightingColor.FetchCustomizations(this.txtEditor.SyntaxHighlighting.Name));
            }
        }
        #endregion

        #region Folding
        private TimeSpan refreshInterval;
        private DateTime nextRefreshTime;
        private void RefreshFolding()
        {
            if (this.InvokeRequired)
            {
                MethodInvoker mi = new MethodInvoker(RefreshFolding);
                this.BeginInvoke(mi);
                //this.pbar2.Value = 0;
            }
            else
            {
                //lock (this)
                {
                    try
                    {
                        if (DateTime.Now > nextRefreshTime)
                        {
                            if (foldingStrategy != null)
                            {
                                foldingStrategy.UpdateFoldings(foldingManager, txtEditor.Document);
                            }
                            nextRefreshTime = DateTime.Now + refreshInterval;

                        }
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }

        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;

        private void Fold()
        {
            if (this.txtEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {

                foldingStrategy = new BeginEndFoldingStrategy();
                txtEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

                if (false)
                {
                    switch (txtEditor.SyntaxHighlighting.Name)
                    {
                        case "XML":
                            foldingStrategy = new XmlFoldingStrategy();
                            txtEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                            break;
                        case "C#":
                        case "C++":
                        case "PHP":
                        case "Java":
                            txtEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(txtEditor.Options);
                            foldingStrategy = new BraceFoldingStrategy();
                            break;
                        default:
                            txtEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                            foldingStrategy = null;
                            break;
                    }
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(txtEditor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, txtEditor.Document);
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
        #endregion

        #region SpellChecking hack

        private void SuggestionItem_Click(object sender, EventArgs e)
        {
            this.txtEditor.SelectedText = this.txtEditor.SelectedText.Replace(this.txtEditor.SelectedText, ((ToolStripMenuItem)sender).Text);
        }
        
        public void goToNextLine()
        {
            this.txtEditor.TextArea.Caret.Line++;
            this.SpellCheckerTextBox.Text = this.txtEditor.TextArea.Document.GetText(this.txtEditor.Document.Lines[this.txtEditor.TextArea.Caret.Line - 1]);
            this.SpellCheckerTextBox.Focus();
        }
        
        private void ShowSpellSuggestion()
        {
            try
            {
                var position = txtEditor.GetPositionFromPoint(this.txtEditor.TextArea.TextView.GetVisualPosition(txtEditor.TextArea.Caret.Position, VisualYPosition.LineBottom) - txtEditor.TextArea.TextView.ScrollOffset);
                if (position.HasValue)
                {
                    //SpellingEngine.Hunspeller.Suggest(this.txtEditor.SelectedText);
                    System.Windows.Point ppp = this.txtEditor.PointToScreen(
                    this.txtEditor.TextArea.TextView.GetVisualPosition(txtEditor.TextArea.Caret.Position, VisualYPosition.LineBottom) - txtEditor.TextArea.TextView.ScrollOffset);
                    contextMenuStrip1.Show((int)ppp.X, (int)ppp.Y);

                    //((ToolStripMenuItem)this.contextMenuStrip1.Items[0]).Text = SpellingEngine.Hunspeller.Spell(this.txtEditor.SelectedText)? "No suggestions" : "Suggestions";

                    //((ToolStripMenuItem)this.contextMenuStrip1.Items[0]).Text = "Suggestions";                
                    ((ToolStripMenuItem)this.contextMenuStrip1.Items[0]).DropDownItems.Clear();
                    foreach (string s in NHunspellWrapper.Instance.Hunspeller.Suggest(this.txtEditor.SelectedText))
                    {
                        ToolStripMenuItem sug = new ToolStripMenuItem(s, null, SuggestionItem_Click);

                        ((ToolStripMenuItem)this.contextMenuStrip1.Items[0]).DropDownItems.Add(sug);
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        public void SpellCheck()
        {
                this.SpellCheckerTextBox.Text = this.txtEditor.TextArea.Document.GetText(this.txtEditor.Document.Lines[this.txtEditor.TextArea.Caret.Line - 1]);
                this.SpellCheckerTextBox.Focus();
                NHunspellWrapper.Instance.ShowCheckAllWindow();
        }

        private void customTextBox1_TextChanged(object sender, EventArgs e)
        {
            //if (!handle)
            //    return;
            //handle = true;
            //string ttx = this.txtEditor.TextArea.Document.GetText(this.txtEditor.Document.Lines[this.txtEditor.TextArea.Caret.Line-1]);
            int lineNo = this.txtEditor.TextArea.Caret.Line-1;
            int offset =this.txtEditor.Document.Lines[lineNo].Offset;
            int length=this.txtEditor.Document.Lines[lineNo].TotalLength;
            txtEditor.Document.Replace(offset, length-2, this.SpellCheckerTextBox.Text);
            this.txtEditor.TextArea.Caret.BringCaretToView();
            this.txtEditor.ScrollTo(txtEditor.TextArea.Caret.Position.Line, txtEditor.TextArea.Caret.Position.Column);
        }
        
        private void spellCheckToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtEditor.SelectedText))
            {
                this.SpellCheck();
            }
            else
            {
                ShowSpellSuggestion();
            }  
        }
        
        private void checkWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtEditor.SelectedText))
            {
                this.SpellCheck();
            }

            else
            {
                ShowSpellSuggestion();
            }
        }

        #endregion
        
   
    }
}