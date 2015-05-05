using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NHunspellComponent.Spelling.Interfaces
{
   public interface ISpellingWindow
   {
      ISpellingOptionsWindow OptionsWindow { get; set; }
      Options OptionsOfCurEditor {set;}

      bool Visible { get; set; }
      Form Owner { get; set; }
      event EventHandler VisibleChanged;
      event CancelEventHandler Closing;
      string TextBeingSpelled { set; }
      CharacterRange HighlightedRange { set; }
      List<string> SuggestionsInBox { set; }
      

      event NHunspellWrapper.AddedWordHandler AdddedWord;
      event NHunspellWrapper.IgnoredWordHandler IgnoredWord;
      event NHunspellWrapper.ChangedWordHandler ChangedWord;
      event NHunspellWrapper.ChangedLineHandler ChangedLine;
      event Options.OptionsChangedHandle OptionsChanged;
      
   }
}