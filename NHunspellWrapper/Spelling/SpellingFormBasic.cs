using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NHunspellComponent.Spelling.Events;
using NHunspellComponent.Spelling.Interfaces;
using NHunspellComponent.SupportClasses;

namespace NHunspellComponent.Spelling
{
   public partial class SpellingFormBasic : Form, ISpellingWindow
   {
      private ISpellingOptionsWindow optionsWindow;
      public event NHunspellWrapper.AddedWordHandler AdddedWord;
      public event NHunspellWrapper.IgnoredWordHandler IgnoredWord;
      public event NHunspellWrapper.ChangedWordHandler ChangedWord;
      public event NHunspellWrapper.ChangedLineHandler ChangedLine;

      public event Options.OptionsChangedHandle OptionsChanged;

      public SpellingFormBasic()
      {
         InitializeComponent();
         OptionsWindow = new SpellingOptionsFormBasic();
         OptionsWindow.OptionsChanged += OptionsWindow_OptionsChanged;
      }

      public string TextBeingSpelled
      {
         set { textShowBox.Text = value; }
      }

      private CharacterRange curRange;
      private Options optionsOfCurEditor;

      public CharacterRange HighlightedRange
      {
         set
         {
            textShowBox.Select(0, textShowBox.Text.Length);
            textShowBox.SelectionColor = Color.Black;
            textShowBox.Select(value.First, value.Length);
            textShowBox.SelectionColor = Color.Red;
            textShowBox.Select(value.First + value.Length, 0);
            curRange = value;
         }
      }

      public List<string> SuggestionsInBox
      {
         set
         {
            suggestionBox.Items.Clear();
            if (value.Count != 0)
            {
               suggestionBox.Items.AddRange(value.ToArray());
            }
            else
            {
               suggestionBox.Items.Add(Constants.NoSuggestions);
            }
            suggestionBox.SelectedIndex = 0;
         }
      }

      public ISpellingOptionsWindow OptionsWindow
      {
         get { return optionsWindow; }
         set { optionsWindow = value; }
      }

      public Options OptionsOfCurEditor
      {
         set { optionsOfCurEditor = value; }
      }

      private void bIgnoreOnce_Click(object sender, EventArgs e)
      {
         if (IgnoredWord != null)
         {
            IgnoredWord(sender,
                        new IgnoreEventArgs(textShowBox.Text.Substring(curRange.First, curRange.Length), curRange.First,
                                            false));
         }
      }

      private void bIgnoreAll_Click(object sender, EventArgs e)
      {
         if (IgnoredWord != null)
         {
            IgnoredWord(sender,
                        new IgnoreEventArgs(textShowBox.Text.Substring(curRange.First, curRange.Length), curRange.First,
                                            true));
         }
      }

      private void bAddToDictionary_Click(object sender, EventArgs e)
      {
         if (AdddedWord != null)
         {
            AdddedWord(sender, new SpellingEventArgs(suggestionBox.SelectedItem.ToString(), curRange.First));
         }
      }

      private void bChange_Click(object sender, EventArgs e)
      {
         if (ChangedWord != null && (string)suggestionBox.SelectedItem != Constants.NoSuggestions)
         {
            ChangedWord(sender, new ChangeEventArgs(textShowBox.Text.Substring(curRange.First, curRange.Length),
                                                    curRange.First, (string)suggestionBox.SelectedItem, false));
         }
         else
         {
            bIgnoreOnce_Click(sender, e);
         }
      }

      private void bChangeAll_Click(object sender, EventArgs e)
      {
         if (ChangedWord != null && (string)suggestionBox.SelectedItem != Constants.NoSuggestions)
         {
            ChangedWord(sender, new ChangeEventArgs(textShowBox.Text.Substring(curRange.First, curRange.Length),
                                                    curRange.First, (string)suggestionBox.SelectedItem, true));
         }
         else
         {
            bIgnoreAll_Click(sender, e);
         }
      }

      private void bAutoCorrect_Click(object sender, EventArgs e)
      {

         MessageBox.Show(
            "Here we should add logic for autoreplacing words.\nFor example: hashtable of words and it's replacings.");
      }
      private void buttonNextLine_Click(object sender, EventArgs e)
      {
          ChangedLine(sender, new ChangeEventArgs(textShowBox.Text.Substring(curRange.First, curRange.Length), curRange.First, (string)suggestionBox.SelectedItem, true));
      }

      private void bOptions_Click(object sender, EventArgs e)
      {
         OptionsWindow.ShowCurOptionsWindow(optionsOfCurEditor);
      }

      private void bUndo_Click(object sender, EventArgs e)
      {
         MessageBox.Show("Undo logic should be here.");
      }

      private void bCancel_Click(object sender, EventArgs e)
      {
         Hide();
      }

      private void OptionsWindow_OptionsChanged(Options options)
      {
         optionsOfCurEditor = options;
         NotifyOptionsChanged(options);
      }

      private void NotifyOptionsChanged(Options options)
      {
         if (OptionsChanged != null)
         {
            OptionsChanged(options);
         }
      }

      
   }
}