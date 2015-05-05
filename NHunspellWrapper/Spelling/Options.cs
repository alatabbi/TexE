using System;

namespace NHunspellComponent.Spelling
{
   [Serializable]
   public struct Options
   {
      public bool IgnoreWordsWithDigits;
      public bool IgnoreWordsInUppercase;
      public bool UseAutoCorrection;

      public delegate void OptionsChangedHandle(Options opts);
   }
}