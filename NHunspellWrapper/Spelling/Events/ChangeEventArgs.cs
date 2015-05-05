namespace NHunspellComponent.Spelling.Events
{
   public class ChangeEventArgs : SpellingEventArgs
   {
      private string changeWord;
      private bool applyToAllWords;

      public ChangeEventArgs(string word, int textIndex, string newWord, bool allWords)
         : base(word, textIndex)
      {
         changeWord = newWord;
         applyToAllWords = allWords;
      }

      public string NewWord
      {
         get { return changeWord; }
      }

      public bool ApplyToAllWords
      {
         get { return applyToAllWords; }
      }
   }
}