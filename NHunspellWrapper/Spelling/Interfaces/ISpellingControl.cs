using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace NHunspellComponent.Spelling.Interfaces
{
   /// <summary>
   /// Summary description for ISpellingControl.
   /// </summary>
   public interface ISpellingControl
   {
      [Browsable(true)]
      bool IsSpellingEnabled { get; set; }

      int SelectionStart { get; set; }
      int SelectionLength { get; set; }
      string SelectedText { get; set; }
      string Text { get; set; }
      ContextMenuStrip ContextMenuStrip { get; set; }
      Control Parent { get; set; }
      event EventHandler Disposed;
      event EventHandler Enter;
      event EventHandler TextChanged;
      bool ReadOnly { get; set; }
      bool IsPassWordProtected { get; }

      void Select(int start, int length);
      bool Focus();
      void Invalidate(bool invalidateChildren);

      Dictionary<int, int> IgnoredSections { set; }
      void GetNextLine();
   }
}