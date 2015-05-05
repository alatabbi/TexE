using System;
using System.Text.RegularExpressions;

namespace NHunspellComponent.Spelling
{
    internal struct Word
    {
        private const string PATTERN_IS_DEVANAGARI = @"(\p{IsDevanagari}+)";
        public int Start;
        public int Length;
        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                int l = text.Length;
                text = text.TrimStart(' ');
                Start = Start - (l - text.Length);
                text = text.Trim();
                Length = text.Length;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} ({1}){2}", Text, Start, Length);
        }

        public static int GetWordStart(ref string Text, int position)
        {
            if (!(Text.Length > 0)) return 0;
            if (position == Text.Length) position--;
            int wordStart = position;
            if (!char.IsLetterOrDigit(Text[wordStart]) && wordStart > 0) wordStart--;
            for (int i = wordStart; i >= 0; i--)
            {
                char ch = Text[i];
                //if (ch == ' ' || ch == '\n')
                //if (char.IsSeparator(ch) || char.IsPunctuation(ch))
                if (!CheckCharBelongsToWord(ch))
                {
                    break;
                }
                wordStart = i;
            }
            return wordStart;
        }

        public static int GetWordEnd(ref string Text, int position)
        {
            int wordEnd = Text.Length;
            for (int i = position; i < Text.Length && Text.Length > 0; i++)
            {
                char ch = Text[i];
                //if (ch == ' ' || ch == '\n')
                //if (char.IsSeparator(ch) || char.IsPunctuation(ch))
                if (!CheckCharBelongsToWord(ch))
                {
                    wordEnd = i;
                    break;
                }
            }
            return wordEnd;
        }


        private static bool CheckCharBelongsToWord(char ch)
        {
            return char.IsLetterOrDigit(ch) && !char.IsPunctuation(ch) ||
                System.Text.RegularExpressions.Regex.IsMatch(
                ch.ToString(
                  System.Threading.Thread.CurrentThread.CurrentCulture),
                  PATTERN_IS_DEVANAGARI
                  );
        }
        //return !char.IsSeparator(ch) && !char.IsPunctuation(ch) && !char.IsSeparator(ch);

        public static Word GetWordFromPosition(string Text, int position)
        {
            Word result = new Word();
            result.Start = Word.GetWordStart(ref Text, position);
            result.Length = Word.GetWordEnd(ref Text, result.Start) - result.Start;
            result.Text = Text.Substring(result.Start, result.Length).Replace("\n", "");
            return result;
        }

        internal void Reset()
        {
            Start = 0;
            Length = 0;
            text = "";
        }
    }
}