using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace NHunspellComponent.Spelling.Interfaces
{
   public interface IUnderlineable
   {
      [Browsable(true)]
      bool IsSpellingAutoEnabled { get; set; }

      event KeyEventHandler KeyDown;
      event EventHandler SelectionChanged;

      Dictionary<int, int> UnderlinedSections { get; set; }
      Dictionary<int, int> ProtectedSections { set; }

      void RemoveWordFromUnderliningList(int wordStart);

      void CustomPaint();
   }
}