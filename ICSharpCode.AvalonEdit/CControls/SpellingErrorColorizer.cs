using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Timers;

namespace SpellCheckAvalonEdit
{
    public class SpellingErrorColorizer : DocumentColorizingTransformer
    {
        private static readonly TextBox staticTextBox = new TextBox();
        private readonly TextDecorationCollection collection;

        bool el = true;
        private static string t = "";
        public SpellingErrorColorizer()
        {
            // Create the Text decoration collection for the visual effect - you can get creative here
            collection = new TextDecorationCollection();
            var dec = new TextDecoration();
            dec.Pen = new Pen {Thickness = 1, DashStyle = DashStyles.DashDot, Brush = new SolidColorBrush(Colors.Red)};
            dec.PenThicknessUnit = TextDecorationUnit.FontRecommended;
            collection.Add(dec);

            // Set the static text box properties
            staticTextBox.AcceptsReturn = true;
            staticTextBox.AcceptsTab = true;
            staticTextBox.SpellCheck.IsEnabled = true;
            this.tmr.Interval = 60000;
            tmr.Elapsed += new ElapsedEventHandler(tmr_Elapsed);
            this.tmr.Start();
            
        }

        void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            el = true;
        }

        Timer tmr = new Timer();
                 
        protected override void ColorizeLine(DocumentLine line)
        {

                    if (el)
                    {
                     
                        //if (line.LineNumber == CurrentContext.Document.LineCount - 1)
                        //{
                            //el = false;
                        //}

                        lock (staticTextBox)
                        {
                            try
                            {
                                //if (string.IsNullOrEmpty(t))
                                //    t = "";
                                staticTextBox.Text = CurrentContext.Document.GetText(line.Offset, line.Length);//CurrentContext.Document.Text;
                                int start = 0;// line.Offset;
                                int end = line.Length;//.EndOffset;
                                start = staticTextBox.GetNextSpellingErrorCharacterIndex(start, LogicalDirection.Forward);
                                while (start < end)
                                {
                                    if (start == -1)
                                        break;
                                    int wordEnd = start + line.Offset + staticTextBox.GetSpellingErrorLength(start);

                                    SpellingError error = staticTextBox.GetSpellingError(start);
                                    if (error != null)
                                    {
                                        base.ChangeLinePart(start + line.Offset, wordEnd, (VisualLineElement element) => element.TextRunProperties.SetTextDecorations(collection));
                                    }

                                    start = staticTextBox.GetNextSpellingErrorCharacterIndex(wordEnd - line.Offset, LogicalDirection.Forward);
                                    el = false;
                                }
                            }
                            catch (System.Exception ex)
                            {
                            }
                        }

                    }
                    else
                    {
                        //el = true; 
                        //tmr.Stop();
                    }


        }
    }
}