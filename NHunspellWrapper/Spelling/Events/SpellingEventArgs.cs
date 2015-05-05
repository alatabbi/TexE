using System;

namespace NHunspellComponent.Spelling.Events
{
   /// <summary>
   ///     Class sent to the event handler when we manually Adding or Ignoring word.
   /// </summary>
   public class SpellingEventArgs : EventArgs
   {
      private int _TextIndex;
      private string _Word;

      public SpellingEventArgs(string word, int textIndex)
      {
         _Word = word;
         _TextIndex = textIndex;
      }

      public int TextIndex
      {
         get { return _TextIndex; }
      }

      public string Word
      {
         get { return _Word; }
      }
   }
}