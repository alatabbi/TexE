using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using NHunspellComponent.Spelling.Interfaces;

namespace NHunspellComponent
{
   internal class CustomMaskedTextBox : MaskedTextBox, ISpellingControl
   {
      #region ISpellingControl Members

      private bool spellingEnabled;
      private bool spellingAutoEnabled;
      private bool isPassWordProtected;

      public bool IsSpellingEnabled
      {
         get { return spellingEnabled; }
         set { spellingEnabled = value; }
      }

      [Browsable(false)]
      public bool IsSpellingAutoEnabled
      {
         get { return false; }
         set { spellingAutoEnabled = value; }
      }

      public bool IsPassWordProtected
      {
         get { return isPassWordProtected; }
         set { isPassWordProtected = value; }
      }

      public Dictionary<int, int> IgnoredSections
      {
         set { }
      }

      #endregion
   }
}