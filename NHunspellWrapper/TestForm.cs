using System;
using System.Windows.Forms;
using NHunspellComponent.Spelling;

namespace NHunspellComponent
{
   public partial class TestForm : Form
   {
      public TestForm()
      {
         InitializeComponent();
         customPaintRichText21.IsSpellingAutoEnabled = chAutoSpelling.Checked;
         SpellingWorker uWorker = new SpellingWorker(customPaintRichText21);
         SpellingWorker uWorker2 = new SpellingWorker(customMaskedTextBox1);
         customPaintRichText21.Invalidate(true);
      }

      private void bCkeckAll_Click(object sender, EventArgs e)
      {
         NHunspellWrapper.Instance.ShowCheckAllWindow();
      }

      private void chAutoSpelling_CheckedChanged(object sender, EventArgs e)
      {
         customPaintRichText21.IsSpellingAutoEnabled = chAutoSpelling.Checked;
      }
   }
}