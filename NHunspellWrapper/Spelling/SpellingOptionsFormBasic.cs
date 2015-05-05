using System;
using System.ComponentModel;
using System.Windows.Forms;
using NHunspellComponent.Spelling.Interfaces;

namespace NHunspellComponent.Spelling
{
   public partial class SpellingOptionsFormBasic : Form, ISpellingOptionsWindow
   {
      public event Options.OptionsChangedHandle OptionsChanged;
      private Options opts;

      public SpellingOptionsFormBasic()
      {
         InitializeComponent();
      }

      public void ShowCurOptionsWindow(Options options)
      {
         chIgnoreWordsWithDigits.Checked = options.IgnoreWordsWithDigits;
         chIgnoreWordsInUppercase.Checked = options.IgnoreWordsInUppercase;
         chUseAutoCorrection.Checked = options.UseAutoCorrection;
         ShowDialog();
      }

      private void chIgnoreWordsWithDigits_CheckedChanged(object sender, EventArgs e)
      {
         opts.IgnoreWordsWithDigits = chIgnoreWordsWithDigits.Checked;
      }

      private void chIgnoreWordsInUppercase_CheckedChanged(object sender, EventArgs e)
      {
         opts.IgnoreWordsInUppercase = chIgnoreWordsInUppercase.Checked;
      }

      private void chUseAutoCorrection_CheckedChanged(object sender, EventArgs e)
      {
         opts.UseAutoCorrection = chUseAutoCorrection.Checked;
      }

      private void bSave_Click(object sender, EventArgs e)
      {
         if (OptionsChanged != null)
         {
            OptionsChanged(opts);
         }
         Hide();
      }
   }
}