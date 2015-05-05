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
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using AvalonEdit.Sample;
using ICSharpCode.AvalonEdit.Edi.Intellisense;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using System.Windows.Threading;
using System.Xml;
using System.ComponentModel;



namespace ICSharpCode.AvalonEdit.CControls
{
    /// <summary>
    /// Interaction logic for CUserControl.xaml
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
            //this.textEditor.TextArea.Caret.PositionChanged += this.Caret_PositionChanged;
            //textEditor.TextArea.TextView.LineTransformers.Add(new SpellingErrorColorizer());
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            //var aa= new IBackgroundRenderer
            //this.textEditor.TextArea.TextView.InvalidateVisual();
            //textEditor.TextArea.TextView.BackgroundRenderers.Clear();
            //textEditor.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this.textEditor));

        }


         
    }
}
