using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;

namespace ATABBI.TexE.AddIns
{
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {

        private TextEditor mEditor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="highlightBackgroundColorBrush"></param>
        public HighlightCurrentLineBackgroundRenderer(TextEditor editor,SolidColorBrush highlightBackgroundColorBrush = null)
        {
            this.mEditor = editor;
            // Light Blue 0x100000FF
            this.BackgroundColorBrush = new SolidColorBrush((highlightBackgroundColorBrush == null ? Color.FromArgb(30, 106, 90, 205) : highlightBackgroundColorBrush.Color));
        }

        /// <summary>
        /// Get the <seealso cref="KnownLayer"/> of the <seealso cref="TextEditor"/> control.
        /// </summary>
        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        /// <summary>
        /// Get/Set color brush to show for highlighting current line
        /// </summary>
        public SolidColorBrush BackgroundColorBrush { get; set; }

        /// <summary>
        /// Draw the background line highlighting of the current line.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (this.mEditor.Document == null)
                return;
            textView.EnsureVisualLines();
            var currentLine = mEditor.Document.GetLineByOffset(mEditor.CaretOffset);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(new SolidColorBrush(this.BackgroundColorBrush.Color), null,new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
            }


        }
    
    }

    //public class SpellCurrentLineBackgroundRenderer : IBackgroundRenderer
    //{
    //    private TextEditor mEditor;
    //    //NetSpell.SpellChecker.Spelling SpellChecker = new NetSpell.SpellChecker.Spelling();
         
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="editor"></param>
    //    /// <param name="highlightBackgroundColorBrush"></param>
    //    public SpellCurrentLineBackgroundRenderer(TextEditor editor, SolidColorBrush highlightBackgroundColorBrush = null)
    //    {
    //        this.mEditor = editor;
    //        // Light Blue 0x100000FF
    //        this.BackgroundColorBrush = new SolidColorBrush((highlightBackgroundColorBrush == null ? Color.FromArgb(0x20, 0x80, 0x80, 0x80) : highlightBackgroundColorBrush.Color));
    //        NetSpell.SpellChecker.Dictionary.WordDictionary Dic = new NetSpell.SpellChecker.Dictionary.WordDictionary();
    //        Dic.DictionaryFile = "en-GB.dic";
    //        this.SpellChecker.Dictionary = Dic;
    //    }

    //    /// <summary>
    //    /// Get the <seealso cref="KnownLayer"/> of the <seealso cref="TextEditor"/> control.
    //    /// </summary>
    //    public KnownLayer Layer
    //    {
    //        get { return KnownLayer.Background; }
    //    }

    //    /// <summary>
    //    /// Get/Set color brush to show for highlighting current line
    //    /// </summary>
    //    public SolidColorBrush BackgroundColorBrush { get; set; }

    //    /// <summary>
    //    /// Draw the background line highlighting of the current line.
    //    /// </summary>
    //    /// <param name="textView"></param>
    //    /// <param name="drawingContext"></param>
    //    public void Draw(TextView textView, DrawingContext drawingContext)
    //    {
    //        if (this.mEditor.Document == null)
    //            return;

    //        textView.EnsureVisualLines();
    //        DocumentLine line = mEditor.Document.GetLineByOffset(mEditor.TextArea.Caret.Offset);
             
    //        string text = this.mEditor.TextArea.Document.GetText(line.Offset, line.TotalLength);
    //        int start = 0;
    //        int index;
    //        Regex re = new Regex(@"\w+");

    //        foreach (Match m in re.Matches(text))
    //        {
    //            string w = m.Groups[0].Value;
    //            //if ((index = text.IndexOf(w, start)) >= 0)
    //            //{
    //                this.SpellChecker.Suggest(w);
    //                if (this.SpellChecker.Suggestions.Count > 0)
    //                { 
                                                       

    //                }

                    
    //                //base.ChangeLinePart(
                        
    //                 //   lineStartOffset + index, // startOffset
    //                 //   lineStartOffset + index + w.Length, // endOffset

    //                   // (VisualLineElement element) =>
    //                    //{

    //                        //element.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0x90, 0xff, 0xff, 0x00));
    //                        // This lambda gets called once for every VisualLineElement
    //                        // between the specified offsets.

    //                        TextDecorationCollection collection = new TextDecorationCollection();
    //                        var dec = new TextDecoration();
    //                        dec.Pen = new Pen { Thickness = 1, DashStyle = DashStyles.DashDot, Brush = new SolidColorBrush(Colors.Red) };
    //                        dec.PenThicknessUnit = TextDecorationUnit.FontRecommended;
    //                        collection.Add(dec);

    //                      //  element.TextRunProperties.SetTextDecorations(collection);

    //                        //    Typeface tf = element.TextRunProperties.Typeface;

    //                        //    //// Replace the typeface with a modified version of
    //                        //    //// the same typeface
    //                        //    element.TextRunProperties.SetTypeface(new Typeface(
    //                        //        tf.FontFamily,
    //                        //        FontStyles.Oblique,
    //                        //        FontWeights.Bold,
    //                        //        tf.Stretch
    //                        //    ));
    //                    //});

    //              //  start = index + 1; // search for next occurrence
    //            //}
    //        }
            
    //    }

    //    void ApplyChanges(VisualLineElement element)
    //    {
    //        // apply changes here
    //        element.TextRunProperties.SetForegroundBrush(Brushes.Red);
    //        TextDecorationCollection collection = new TextDecorationCollection();
    //        var dec = new TextDecoration();
    //        dec.Pen = new Pen { Thickness = 1, DashStyle = DashStyles.DashDot, Brush = new SolidColorBrush(Colors.Red) };
    //        dec.PenThicknessUnit = TextDecorationUnit.FontRecommended;
    //        collection.Add(dec);
    //    }
         
            

    //}
}
