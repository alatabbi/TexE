using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using NHunspellComponent.Spelling.Interfaces;


namespace TestingApplication
{
   internal class CustomTextBox : TextBox, ISpellingControl
   {
      #region ISpellingControl Members

       int index = 0;
       public void GetNextLine()
       {
           ((ATABBI.TexE.TextDocument) this.Parent.Parent).goToNextLine();
           //index++;
           //this.Text = "new text" + index.ToString();
       }
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