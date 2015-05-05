using System.ComponentModel;

namespace NHunspellComponent.Spelling.Interfaces
{
   public interface ISpellingOptionsWindow
   {
      void ShowCurOptionsWindow(Options options);
      event Options.OptionsChangedHandle OptionsChanged;
   }
}