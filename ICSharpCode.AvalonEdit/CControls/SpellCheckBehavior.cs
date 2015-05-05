using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Timers;

namespace SpellCheckAvalonEdit
{
    internal class SpellCheckBehavior : Behavior<TextEditor>
    {
        private TextBox textBox;
        private TextEditor textEditor;

        protected override void OnAttached()
        {
            textEditor = AssociatedObject;
            if (textEditor != null)
            {
                textBox = new TextBox();
                textEditor.ContextMenuOpening += textEditor_ContextMenuOpening;
            }
            base.OnAttached();
        }

        private void textEditor_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TextViewPosition? pos = textEditor.TextArea.TextView.GetPosition(new Point(e.CursorLeft, e.CursorTop));

            if (pos != null)
            {
                //Reset the context menu
                textEditor.ContextMenu = null;

                //Get the new caret position
                int newCaret = textEditor.Document.GetOffset(pos.Value.Line, pos.Value.Column);

                //Text box properties
                textBox.AcceptsReturn = true;
                textBox.AcceptsTab = true;
                textBox.SpellCheck.IsEnabled = true;
                textBox.Text = textEditor.Text;

                //Check for spelling errors
                SpellingError error = textBox.GetSpellingError(newCaret);

                //If there is a spelling mistake
                if (error != null)
                {
                    textEditor.ContextMenu = new ContextMenu();
                    int wordStartOffset = textBox.GetSpellingErrorStart(newCaret);
                    int wordLength = textBox.GetSpellingErrorLength(wordStartOffset);
                    foreach (string err in error.Suggestions)
                    {
                        var item = new MenuItem { Header = err, FontWeight = FontWeights.Bold };
                        var t = new Tuple<int, int>(wordStartOffset, wordLength);
                        item.Tag = t;
                        item.Click += item_Click;
                        textEditor.ContextMenu.Items.Add(item);
                    }
                }
            }
        }

        //Click event of the context menu
        private void item_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item != null)
            {
                var seg = item.Tag as Tuple<int, int>;
                textEditor.Document.Replace(seg.Item1, seg.Item2, item.Header.ToString());
            }
        }
    }

    public class SpellingErrorColorizer : DocumentColorizingTransformer
    {
        private static readonly TextBox staticTextBox = new TextBox();
        private readonly TextDecorationCollection collection;

        public SpellingErrorColorizer()
        {
            // Create the Text decoration collection for the visual effect - you can get creative here
            collection = new TextDecorationCollection();
            var dec = new TextDecoration();
            dec.Pen = new Pen { Thickness = 1, DashStyle = DashStyles.DashDot, Brush = new SolidColorBrush(Colors.Red) };
            dec.PenThicknessUnit = TextDecorationUnit.FontRecommended;
            collection.Add(dec);

            // Set the static text box properties
            staticTextBox.AcceptsReturn = true;
            staticTextBox.AcceptsTab = true;
            staticTextBox.SpellCheck.IsEnabled = true;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            lock (staticTextBox)
            {
                staticTextBox.Text = CurrentContext.Document.Text;
                int start = line.Offset;
                int end = line.EndOffset;
                start = staticTextBox.GetNextSpellingErrorCharacterIndex(start, LogicalDirection.Forward);
                while (start < end)
                {
                    if (start == -1)
                        break;

                    int wordEnd = start + staticTextBox.GetSpellingErrorLength(start);

                    SpellingError error = staticTextBox.GetSpellingError(start);
                    if (error != null)
                    {
                        base.ChangeLinePart(start, wordEnd, (VisualLineElement element) => element.TextRunProperties.SetTextDecorations(collection));
                    }

                    start = staticTextBox.GetNextSpellingErrorCharacterIndex(wordEnd, LogicalDirection.Forward);
                }
                //staticTextBox.SpellCheck.IsEnabled = false;
            }
        }
    }

}