using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;
using System.Windows;
using ICSharpCode.AvalonEdit;
using System.Text.RegularExpressions;
using NHunspell;
using NHunspellComponent.Spelling;

namespace ATABBI.TexE.Forms
{
    public class ColorizKeywordEdit : DocumentColorizingTransformer
    {
        public ColorizKeywordEdit(TextEditor editor, string keyword="")
        {
            this.keyword = keyword;
            mEditor = editor;
        }
        public string keyword = "";
        TextEditor mEditor;
        protected override void ColorizeLine(DocumentLine line)
        {
            //this.CurrentContext.TextView.LineTransformers.Clear();
            if (this.keyword=="")
                this.keyword = this.mEditor.SelectedText;
            Remove();
            if (keyword.Length==0)
                return;
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            while ((index = text.IndexOf(keyword, start)) >= 0)
            {
                base.ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index +keyword.Length, // endOffset
                    (VisualLineElement element) =>
                    {
                        element.BackgroundBrush = new SolidColorBrush( Color.FromArgb(0x90, 0xff, 0xff, 0x00)); 
                        // This lambda gets called once for every VisualLineElement
                        // between the specified offsets.
                        //Typeface tf = element.TextRunProperties.Typeface;
                        //// Replace the typeface with a modified version of
                        //// the same typeface
                        //element.TextRunProperties.SetTypeface(new Typeface(
                        //    tf.FontFamily,
                        //    FontStyles.Italic,
                        //    FontWeights.Bold,
                            
                        //    tf.Stretch
                        //));
                    });
               
                start = index + 1; // search for next occurrence
            }
        }

        private void Remove()
        {
            int ind = -1;
            bool found = false;
            foreach (object tr in this.CurrentContext.TextView.LineTransformers)
            {
                ind++;
                if (tr.GetType() == typeof(ColorizKeywordEdit))
                {
                    found = true;
                    break;
                }
            }
            if (found)
                this.CurrentContext.TextView.LineTransformers.RemoveAt(ind);
        }
    }

   


    public class LineSpellColorizer : DocumentColorizingTransformer
    {

        TextDecorationCollection collection = new TextDecorationCollection();
        TextEditor mEditor;
        //public string keyword = "";
        Regex wordEx = new Regex(@"\b[A-Za-z']+\b", RegexOptions.Compiled);
        public LineSpellColorizer(TextEditor editor)
        {
            mEditor = editor;
            var dec = new TextDecoration();
            dec.Pen = new Pen { Thickness = 1, DashStyle = DashStyles.DashDot, Brush = new SolidColorBrush(Colors.Red) };
            dec.PenThicknessUnit = TextDecorationUnit.FontRecommended;
            collection.Add(dec);
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (mEditor.TextArea.Caret.Line != line.LineNumber)
                return;
            string text = CurrentContext.Document.GetText(line);
            foreach (Match m in wordEx.Matches(text))
            {
                string w = m.Groups[0].Value;

                bool correct = NHunspellWrapper.Instance.Hunspeller.Spell(w);
                if (correct) 
                    continue;
                List<string> suggestions = NHunspellWrapper.Instance.Hunspeller.Suggest(w);
 
                base.ChangeLinePart(
                line.Offset + m.Index, // startOffset
                line.Offset + m.Index + m.Length, // endOffset
                (VisualLineElement element) =>
                {
                    element.TextRunProperties.SetTextDecorations(collection);
                });
            }
        }
    }
}
