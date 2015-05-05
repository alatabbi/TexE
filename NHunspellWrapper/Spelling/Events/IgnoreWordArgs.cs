namespace NHunspellComponent.Spelling.Events
{
   public class IgnoreEventArgs : SpellingEventArgs
   {
      private bool ignoreAllWords;

      public IgnoreEventArgs(string word, int textIndex, bool allWords)
         : base(word, textIndex)
      {
         ignoreAllWords = allWords;
      }

      public bool IgnoreAllWords
      {
         get { return ignoreAllWords; }
      }
   }
}