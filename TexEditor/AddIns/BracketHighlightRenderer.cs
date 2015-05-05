using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Diagnostics;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using System.ComponentModel;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
//using ICSharpCode.SharpDevelop.Editor;

    namespace ICSharpCode.AvalonEdit.AddIn
    {
        public interface IBracketSearcher
        {
            /// <summary>
            /// Searches for a matching bracket from the given offset to the start of the document.
            /// </summary>
            /// <returns>A BracketSearchResult that contains the positions and lengths of the brackets. Return null if there is nothing to highlight.</returns>
            BracketSearchResult SearchBracket(TextDocument document, int offset);
        }

        public class DefaultBracketSearcher : IBracketSearcher
        {
            public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();

            public BracketSearchResult SearchBracket(TextDocument document, int offset)
            {
                return null;
            }
        }

        public class CSharpBracketSearcher : IBracketSearcher
        {
            string openingBrackets = "([{";
            string closingBrackets = ")]}";

            public BracketSearchResult SearchBracket(TextDocument document, int offset)
            {
                if (offset > 0)
                {
                    char c = document.GetCharAt(offset - 1);
                    int index = openingBrackets.IndexOf(c);
                    int otherOffset = -1;
                    if (index > -1)
                        otherOffset = SearchBracketForward(document, offset, openingBrackets[index], closingBrackets[index]);

                    index = closingBrackets.IndexOf(c);
                    if (index > -1)
                        otherOffset = SearchBracketBackward(document, offset - 2, openingBrackets[index], closingBrackets[index]);

                    if (otherOffset > -1)
                        return new BracketSearchResult(Math.Min(offset - 1, otherOffset), 1, Math.Max(offset - 1, otherOffset), 1);
                }

                return null;
            }

            #region SearchBracket helper functions
            static int ScanLineStart(TextDocument document, int offset)
            {
                for (int i = offset - 1; i > 0; --i)
                {
                    if (document.GetCharAt(i) == '\n')
                        return i + 1;
                }
                return 0;
            }

            /// <summary>
            /// Gets the type of code at offset.<br/>
            /// 0 = Code,<br/>
            /// 1 = Comment,<br/>
            /// 2 = String<br/>
            /// Block comments and multiline strings are not supported.
            /// </summary>
            static int GetStartType(TextDocument document, int linestart, int offset)
            {
                bool inString = false;
                bool inChar = false;
                bool verbatim = false;
                int result = 0;
                for (int i = linestart; i < offset; i++)
                {
                    switch (document.GetCharAt(i))
                    {
                        case '/':
                            if (!inString && !inChar && i + 1 < document.TextLength)
                            {
                                if (document.GetCharAt(i + 1) == '/')
                                {
                                    result = 1;
                                }
                            }
                            break;
                        case '"':
                            if (!inChar)
                            {
                                if (inString && verbatim)
                                {
                                    if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
                                    {
                                        ++i; // skip escaped quote
                                        inString = false; // let the string go on
                                    }
                                    else
                                    {
                                        verbatim = false;
                                    }
                                }
                                else if (!inString && i > 0 && document.GetCharAt(i - 1) == '@')
                                {
                                    verbatim = true;
                                }
                                inString = !inString;
                            }
                            break;
                        case '\'':
                            if (!inString) inChar = !inChar;
                            break;
                        case '\\':
                            if ((inString && !verbatim) || inChar)
                                ++i; // skip next character
                            break;
                    }
                }

                return (inString || inChar) ? 2 : result;
            }
            #endregion

            #region SearchBracketBackward
            int SearchBracketBackward(TextDocument document, int offset, char openBracket, char closingBracket)
            {
                if (offset + 1 >= document.TextLength) return -1;
                // this method parses a c# document backwards to find the matching bracket

                // first try "quick find" - find the matching bracket if there is no string/comment in the way
                int quickResult = QuickSearchBracketBackward(document, offset, openBracket, closingBracket);
                if (quickResult >= 0) return quickResult;

                // we need to parse the line from the beginning, so get the line start position
                int linestart = ScanLineStart(document, offset + 1);

                // we need to know where offset is - in a string/comment or in normal code?
                // ignore cases where offset is in a block comment
                int starttype = GetStartType(document, linestart, offset + 1);
                if (starttype == 1)
                {
                    return -1; // start position is in a comment
                }

                // I don't see any possibility to parse a C# document backwards...
                // We have to do it forwards and push all bracket positions on a stack.
                Stack<int> bracketStack = new Stack<int>();
                bool blockComment = false;
                bool lineComment = false;
                bool inChar = false;
                bool inString = false;
                bool verbatim = false;

                for (int i = 0; i <= offset; ++i)
                {
                    char ch = document.GetCharAt(i);
                    switch (ch)
                    {
                        case '\r':
                        case '\n':
                            lineComment = false;
                            inChar = false;
                            if (!verbatim) inString = false;
                            break;
                        case '/':
                            if (blockComment)
                            {
                                Debug.Assert(i > 0);
                                if (document.GetCharAt(i - 1) == '*')
                                {
                                    blockComment = false;
                                }
                            }
                            if (!inString && !inChar && i + 1 < document.TextLength)
                            {
                                if (!blockComment && document.GetCharAt(i + 1) == '/')
                                {
                                    lineComment = true;
                                }
                                if (!lineComment && document.GetCharAt(i + 1) == '*')
                                {
                                    blockComment = true;
                                }
                            }
                            break;
                        case '"':
                            if (!(inChar || lineComment || blockComment))
                            {
                                if (inString && verbatim)
                                {
                                    if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
                                    {
                                        ++i; // skip escaped quote
                                        inString = false; // let the string go
                                    }
                                    else
                                    {
                                        verbatim = false;
                                    }
                                }
                                else if (!inString && offset > 0 && document.GetCharAt(i - 1) == '@')
                                {
                                    verbatim = true;
                                }
                                inString = !inString;
                            }
                            break;
                        case '\'':
                            if (!(inString || lineComment || blockComment))
                            {
                                inChar = !inChar;
                            }
                            break;
                        case '\\':
                            if ((inString && !verbatim) || inChar)
                                ++i; // skip next character
                            break;
                        default:
                            if (ch == openBracket)
                            {
                                if (!(inString || inChar || lineComment || blockComment))
                                {
                                    bracketStack.Push(i);
                                }
                            }
                            else if (ch == closingBracket)
                            {
                                if (!(inString || inChar || lineComment || blockComment))
                                {
                                    if (bracketStack.Count > 0)
                                        bracketStack.Pop();
                                }
                            }
                            break;
                    }
                }
                if (bracketStack.Count > 0) return (int)bracketStack.Pop();
                return -1;
            }
            #endregion

            #region SearchBracketForward
            int SearchBracketForward(TextDocument document, int offset, char openBracket, char closingBracket)
            {
                bool inString = false;
                bool inChar = false;
                bool verbatim = false;

                bool lineComment = false;
                bool blockComment = false;

                if (offset < 0) return -1;

                // first try "quick find" - find the matching bracket if there is no string/comment in the way
                int quickResult = QuickSearchBracketForward(document, offset, openBracket, closingBracket);
                if (quickResult >= 0) return quickResult;

                // we need to parse the line from the beginning, so get the line start position
                int linestart = ScanLineStart(document, offset);

                // we need to know where offset is - in a string/comment or in normal code?
                // ignore cases where offset is in a block comment
                int starttype = GetStartType(document, linestart, offset);
                if (starttype != 0) return -1; // start position is in a comment/string

                int brackets = 1;

                while (offset < document.TextLength)
                {
                    char ch = document.GetCharAt(offset);
                    switch (ch)
                    {
                        case '\r':
                        case '\n':
                            lineComment = false;
                            inChar = false;
                            if (!verbatim) inString = false;
                            break;
                        case '/':
                            if (blockComment)
                            {
                                Debug.Assert(offset > 0);
                                if (document.GetCharAt(offset - 1) == '*')
                                {
                                    blockComment = false;
                                }
                            }
                            if (!inString && !inChar && offset + 1 < document.TextLength)
                            {
                                if (!blockComment && document.GetCharAt(offset + 1) == '/')
                                {
                                    lineComment = true;
                                }
                                if (!lineComment && document.GetCharAt(offset + 1) == '*')
                                {
                                    blockComment = true;
                                }
                            }
                            break;
                        case '"':
                            if (!(inChar || lineComment || blockComment))
                            {
                                if (inString && verbatim)
                                {
                                    if (offset + 1 < document.TextLength && document.GetCharAt(offset + 1) == '"')
                                    {
                                        ++offset; // skip escaped quote
                                        inString = false; // let the string go
                                    }
                                    else
                                    {
                                        verbatim = false;
                                    }
                                }
                                else if (!inString && offset > 0 && document.GetCharAt(offset - 1) == '@')
                                {
                                    verbatim = true;
                                }
                                inString = !inString;
                            }
                            break;
                        case '\'':
                            if (!(inString || lineComment || blockComment))
                            {
                                inChar = !inChar;
                            }
                            break;
                        case '\\':
                            if ((inString && !verbatim) || inChar)
                                ++offset; // skip next character
                            break;
                        default:
                            if (ch == openBracket)
                            {
                                if (!(inString || inChar || lineComment || blockComment))
                                {
                                    ++brackets;
                                }
                            }
                            else if (ch == closingBracket)
                            {
                                if (!(inString || inChar || lineComment || blockComment))
                                {
                                    --brackets;
                                    if (brackets == 0)
                                    {
                                        return offset;
                                    }
                                }
                            }
                            break;
                    }
                    ++offset;
                }
                return -1;
            }
            #endregion

            int QuickSearchBracketBackward(TextDocument document, int offset, char openBracket, char closingBracket)
            {
                int brackets = -1;
                // first try "quick find" - find the matching bracket if there is no string/comment in the way
                for (int i = offset; i >= 0; --i)
                {
                    char ch = document.GetCharAt(i);
                    if (ch == openBracket)
                    {
                        ++brackets;
                        if (brackets == 0) return i;
                    }
                    else if (ch == closingBracket)
                    {
                        --brackets;
                    }
                    else if (ch == '"')
                    {
                        break;
                    }
                    else if (ch == '\'')
                    {
                        break;
                    }
                    else if (ch == '/' && i > 0)
                    {
                        if (document.GetCharAt(i - 1) == '/') break;
                        if (document.GetCharAt(i - 1) == '*') break;
                    }
                }
                return -1;
            }

            int QuickSearchBracketForward(TextDocument document, int offset, char openBracket, char closingBracket)
            {
                int brackets = 1;
                // try "quick find" - find the matching bracket if there is no string/comment in the way
                for (int i = offset; i < document.TextLength; ++i)
                {
                    char ch = document.GetCharAt(i);
                    if (ch == openBracket)
                    {
                        ++brackets;
                    }
                    else if (ch == closingBracket)
                    {
                        --brackets;
                        if (brackets == 0) return i;
                    }
                    else if (ch == '"')
                    {
                        break;
                    }
                    else if (ch == '\'')
                    {
                        break;
                    }
                    else if (ch == '/' && i > 0)
                    {
                        if (document.GetCharAt(i - 1) == '/') break;
                    }
                    else if (ch == '*' && i > 0)
                    {
                        if (document.GetCharAt(i - 1) == '/') break;
                    }
                }
                return -1;
            }
        }
        /// <summary>
        /// Holds a customized highlighting color.
        /// </summary>
        public class CustomizedHighlightingColor
        {
            /// <summary>
            /// The language to which this customization applies. null==all languages.
            /// </summary>
            public string Language;

            /// <summary>
            /// The name of the highlighting color being modified.
            /// </summary>
            public string Name;

            public bool Bold, Italic;
            public Color? Foreground, Background;

            public static List<CustomizedHighlightingColor> LoadColors()
            {
                List<CustomizedHighlightingColor> list = new  List<CustomizedHighlightingColor>();// PropertyService.Get("CustomizedHighlightingRules", new List<CustomizedHighlightingColor>());
                CustomizedHighlightingColor cc = new CustomizedHighlightingColor();
                cc.Background = Colors.Red;
                list.Add(cc);

                // Always make a copy of the list so that the original list cannot be modified without using SaveColors().
                return new List<CustomizedHighlightingColor>(list);
            }

            /// <summary>
            /// Saves the set of colors.
            /// </summary>
            public static void SaveColors(IEnumerable<CustomizedHighlightingColor> colors)
            {
                lock (staticLockObj)
                {
                    activeColors = null;
                    //PropertyService.Set("CustomizedHighlightingRules", colors.ToList());
                }
                EventHandler e = ActiveColorsChanged;
                if (e != null)
                    e(null, EventArgs.Empty);
            }

            static ReadOnlyCollection<CustomizedHighlightingColor> activeColors;
            static readonly object staticLockObj = new object();

            public static ReadOnlyCollection<CustomizedHighlightingColor> ActiveColors
            {
                get
                {
                    lock (staticLockObj)
                    {
                        if (activeColors == null)
                            activeColors = LoadColors().AsReadOnly();
                        return activeColors;
                    }
                }
            }

            public static IEnumerable<CustomizedHighlightingColor> FetchCustomizations(string languageName)
            {
               
                // Access CustomizedHighlightingColor.ActiveColors within enumerator so that always the latest version is used.
                // Using CustomizedHighlightingColor.ActiveColors.Where(...) would not work correctly!
                foreach (CustomizedHighlightingColor color in CustomizedHighlightingColor.ActiveColors)
                {
                    if (color.Language == null || color.Language == languageName)
                        yield return color;
                }
            }

            /// <summary>
            /// Occurs when the set of customized highlighting colors was changed.
            /// </summary>
            public static EventHandler ActiveColorsChanged;
        }

        public class BracketSearchResult
        {
            public int OpeningBracketOffset { get; private set; }

            public int OpeningBracketLength { get; private set; }

            public int ClosingBracketOffset { get; private set; }

            public int ClosingBracketLength { get; private set; }

            public BracketSearchResult(int openingBracketOffset, int openingBracketLength,int closingBracketOffset, int closingBracketLength)
            {
                this.OpeningBracketOffset = openingBracketOffset;
                this.OpeningBracketLength = openingBracketLength;
                this.ClosingBracketOffset = closingBracketOffset;
                this.ClosingBracketLength = closingBracketLength;
            }
        }

        public class BracketHighlightRenderer : IBackgroundRenderer
        {
            BracketSearchResult result;
            Pen borderPen;
            Brush backgroundBrush;
            TextView textView;

            public static readonly Color DefaultBackground = Color.FromArgb(22, 0, 0, 255);
            public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 0, 255);

            public const string BracketHighlight = "Bracket highlight";

            public void SetHighlight(BracketSearchResult result)
            {
                if (this.result != result)
                {
                    this.result = result;
                    textView.InvalidateLayer(this.Layer);
                }
            }

            public BracketHighlightRenderer(TextView textView)
            {
                if (textView == null)
                    throw new ArgumentNullException("textView");

                this.textView = textView;

                this.textView.BackgroundRenderers.Add(this);
            }

            void UpdateColors(Color background, Color foreground)
            {
                this.borderPen = new Pen(new SolidColorBrush(foreground), 1);
                this.borderPen.Freeze();

                this.backgroundBrush = new SolidColorBrush(background);
                this.backgroundBrush.Freeze();
            }

            public KnownLayer Layer
            {
                get
                {
                    return KnownLayer.Selection;
                }
            }

            public void Draw(TextView textView, DrawingContext drawingContext)
            {
                if (this.result == null)
                    return;

                BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();

                builder.CornerRadius = 1;
                builder.AlignToMiddleOfPixels = true;

                builder.AddSegment(textView, new TextSegment() { StartOffset = result.OpeningBracketOffset, Length = result.OpeningBracketLength });
                builder.CloseFigure(); // prevent connecting the two segments
                builder.AddSegment(textView, new TextSegment() { StartOffset = result.ClosingBracketOffset, Length = result.ClosingBracketLength });

                Geometry geometry = builder.CreateGeometry();
                if (geometry != null)
                {
                    drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
                }
            }

            public static void ApplyCustomizationsToRendering(BracketHighlightRenderer renderer, IEnumerable<CustomizedHighlightingColor> customizations)
            {
                renderer.UpdateColors(DefaultBackground, DefaultBorder);
                foreach (CustomizedHighlightingColor color in customizations)
                {
                    if (color.Name == BracketHighlight)
                    {
                        renderer.UpdateColors(color.Background ?? Colors.Blue, color.Foreground ?? Colors.Blue);
                        // 'break;' is necessary because more specific customizations come first in the list
                        // (language-specific customizations are first, followed by 'all languages' customizations)
                        break;
                    }
                }
            }
        }
    }
 
