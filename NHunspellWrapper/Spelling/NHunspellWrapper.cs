using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NHunspell;
using NHunspellComponent.Spelling.Events;
using NHunspellComponent.Spelling.Interfaces;
using NHunspellComponent.SupportClasses;

namespace NHunspellComponent.Spelling
{
   public class NHunspellWrapper
   {
      public delegate void EmptyNotificartion();

      public event EmptyNotificartion PropertiesChanged;
      public event Options.OptionsChangedHandle OptionsChanged;

      public delegate void AddedWordHandler(object sender, SpellingEventArgs e);

      public delegate void IgnoredWordHandler(object sender, IgnoreEventArgs e);

      public delegate void ChangedWordHandler(object sender, ChangeEventArgs e);
      public delegate void ChangedLineHandler(object sender, ChangeEventArgs e);

      private readonly Hunspell hunspell;
      private readonly ISpellingWindow spellingForm;
      private ISpellingControl editor;

      private int maxSuggestions;
      private int maxSuggestionsBackup;
      private List<string> ignoreList;
      private Options options;
      //bool spellingWindowIsOpen;
      //It shouldn't do any underlinings check when we are working in SuggestionsWindow.

      private readonly Regex wordEx = new Regex(@"\b[A-Za-z']+\b", RegexOptions.Compiled);
      //private Regex wordEx = new Regex(@"\b[A-Za-z0-9_'А-я]+\b", RegexOptions.Compiled);
      //private Regex numberRegex = new Regex(@"\b^[0-9][0-9]*\b", RegexOptions.Compiled);
      private readonly Regex numberRegex = new Regex(@"\b^\d\d*\b", RegexOptions.Compiled);
      private readonly Regex digitRegex = new Regex(@".\d.", RegexOptions.Compiled);

      #region ctors

      //
      // Constructor used by default. Always keep one in your classes until you understand it's not necessary.
      //
      public NHunspellWrapper()
          : this("en_GB.aff", "en_GB.dic", null, new SpellingFormBasic())
      {
      }

      /// <param name="affFile">"en_us.aff"</param>
      /// <param name="dicFile">"en_us.dic"</param>
      public NHunspellWrapper(string affFile, string dicFile,
                              ISpellingControl textEditor, ISpellingWindow spellForm)
      {
         try
         {
            hunspell = new Hunspell(affFile, dicFile);
            spellingForm = spellForm;
            InitializeSpellingForm();
            Editor = textEditor;
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message + "\nAutoSpellchecking won't work. Place dictionary file to program directory."
                            , "Error loading spellchecker.",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.DefaultDesktopOnly);
         }
      }

      #endregion

      [ThreadStatic]
      private static NHunspellWrapper _instance;

      public static NHunspellWrapper Instance
      {
         //[STAThread]
         get
         {
            if (_instance == null)
               _instance = new NHunspellWrapper();
            return _instance;
         }
      }

      public  Hunspell Hunspeller
      {
          get
          {
              return hunspell;
          }

      }

      public ISpellingControl Editor
      {
         get
         {
            return editor;
         }
         set
         {
            if (editor != null)
            {
               editor.ContextMenuStrip.Opening -= ContextMenuStrip_Opening;
               editor.ContextMenuStrip.Closing -= ContextMenuStrip_Closing;
            }

            if (value != null)
            {
               editor = value;

               if (editor.ContextMenuStrip == null)
               {
                  editor.ContextMenuStrip = new ContextMenuStrip();
                  editor.ContextMenuStrip.ShowImageMargin = false;
               }
               editor.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
               editor.ContextMenuStrip.Closing += ContextMenuStrip_Closing;

               editor.Disposed += editor_Disposed;
            }
         }
      }

      public Options OptionsOfCurEditor
      {
         set
         {
            options = value;
         }
      }

      private void InitializeSpellingForm()
      {
         spellingForm.VisibleChanged += spellingForm_VisibleChanged;
         spellingForm.Closing += spellingForm_Closing;
         spellingForm.AdddedWord += spellingForm_AdddedWord;
         spellingForm.IgnoredWord += spellingForm_IgnoredWord;
         spellingForm.ChangedWord += spellingForm_ChangedWord;
         spellingForm.ChangedLine += spellingForm_ChangedLine;
         spellingForm.OptionsChanged += spellingForm_OptionsChanged;
      }

      private void spellingForm_AdddedWord(object sender, SpellingEventArgs e)
      {
         if (hunspell != null) hunspell.Add(e.Word);
         UpdateSpellingForm(e.TextIndex);
         CallPropertiesChanged();
      }

      private void spellingForm_IgnoredWord(object sender, IgnoreEventArgs e)
      {
         AddToIgnoreList(e.Word);
         UpdateSpellingForm(e.TextIndex);
         CallPropertiesChanged();
      }

      private void AddToIgnoreList(string word)
      {
         if (!ignoreList.Contains(word))
         {
            ignoreList.Add(word);
         }
      }

      private void spellingForm_ChangedWord(object sender, ChangeEventArgs e)
      {
         if (!e.ApplyToAllWords)
         {
            ReplaceWord(e.TextIndex, e.NewWord);
            UpdateSpellingForm(e.TextIndex);
         }
         else
         {
            List<int> wordIndeces = new List<int>();
            MatchCollection mcol = CalculateWords(editor.Text);
            foreach (Match m in mcol)
            {
               if (m.Value == e.Word)
               {
                  wordIndeces.Add(GetWordIndex(editor.Text, m.Index));
               }
            }
            foreach (int curWordTextIndex in wordIndeces)
            {
               ReplaceWord(GetTextIndexFromWordIndex(editor.Text, curWordTextIndex), e.NewWord);
            }
            UpdateSpellingForm(e.TextIndex);
         }
         CallPropertiesChanged();
      }
      private void spellingForm_ChangedLine(object sender, ChangeEventArgs e)
      { 
          this.editor.GetNextLine();
          this.spellingForm.TextBeingSpelled = this.editor.Text;
          SpellingFormHighlight(0);
      
      }
      private void spellingForm_OptionsChanged(Options opts)
      {
         options = opts;
         if (OptionsChanged != null)
         {
            OptionsChanged(options);
         }
      }

      private void CallPropertiesChanged()
      {
         if (PropertiesChanged != null)
         {
            PropertiesChanged();
         }
         editor.Invalidate(true);
      }

      private void UpdateSpellingForm(int index)
      {
         spellingForm.TextBeingSpelled = editor.Text;
         SpellingFormHighlight(index);
      }

      private void SpellingFormHighlight(int startIndex)
      {
         Word firstWord = GetFirstMisspelledWord(editor.Text, startIndex);
         spellingForm.HighlightedRange = new CharacterRange(firstWord.Start, firstWord.Length);
         if (hunspell != null) 
             spellingForm.SuggestionsInBox = hunspell.Suggest(firstWord.Text ?? "");
      }

      private void spellingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (editor != null)
         {
            editor.Focus();
         }
         CallPropertiesChanged();
         e.Cancel = true;
         spellingForm.Visible = false;
      }

      private void spellingForm_VisibleChanged(object sender, EventArgs e)
      {
         spellingForm.TextBeingSpelled = editor.Text;
         spellingForm.OptionsOfCurEditor = options;

         if (!spellingForm.Visible) //Hidding Form
            spellingForm.Owner = null;
         else //Showing Form
         {
            if (editor != null && editor.Parent != null)
            {
               //Mega feature - thanks to Resharper! Made 1 short line from 3 extra large ones.
               Form editorParent = editor.Parent as Form ?? editor.Parent.Parent as Form;
               spellingForm.Owner = editorParent;
               if (editorParent != null)
                  editorParent.FormClosing += editorParent_FormClosing;
               SpellingFormHighlight(0);
            }
         }
      }

      private List<string> IgnoreList
      {
         get
         {
            if (ignoreList == null)
            {
               ignoreList = new List<string>();
            }
            return ignoreList;
         }
         set { ignoreList = value; }
      }

      private void editor_Disposed(object sender, EventArgs e)
      {
         Editor = null;
         if (spellingForm != null && spellingForm.Visible)
         {
            spellingForm.Visible = false;
         }
      }

      private void editorParent_FormClosing(object sender, FormClosingEventArgs e)
      {
         OnAppClose();
      }

      public void OnAppClose()
      {
         spellingForm.Owner = null;
      }

      private void ContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
      {
         foreach (ToolStripItem ts in tsList)
            editor.ContextMenuStrip.Items.Remove(ts);
         tsList.Clear();
         //if (e.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
         //   editor.SelectionStart = editor.SelectionStart;
      }

      private readonly List<ToolStripItem> tsList = new List<ToolStripItem>();

      private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
      {
         //Selected more than 1 word or
         //ToolStripItems List was not cleared. No ContextMenuStrip_Closing fired.
         if (wordEx.Matches(editor.SelectedText).Count > 1 || tsList.Count > 0)
         {
            return;
         }

         Word word = GetWordFromPosition();
         if (!HunspellSpell(word))
         {
            e.Cancel = false;
            BackupMaxSuggestions();
            maxSuggestions = 4;
            if (hunspell != null)
            {
               List<string> Suggestions = hunspell.Suggest(word.Text);
               foreach (string sug in Suggestions)
               {
                  ToolStripItem item = new ToolStripMenuItem(sug, null, new EventHandler(Suggestion_Click));
                  //item.Click += new EventHandler(Suggestion_Click);
                  tsList.Add(item);
                  if (tsList.Count >= maxSuggestions) break;
               }
               if (Suggestions.Count == 0)
               {
                  ToolStripItem item = new ToolStripMenuItem();
                  item.Text = Constants.NoSuggestions;
                  item.Enabled = false;
                  tsList.Add(item);
               }
               else
               {
                  ToolStripItem item = new ToolStripMenuItem("All suggestions", null,
                                                             new EventHandler(CallShowCurrentWordWindow));
                  tsList.Add(item);
               }
            }

            if (tsList.Count > 0)
            {
               if (editor.ContextMenuStrip.Items.Count > 0)
               {
                  tsList.Add(new ToolStripSeparator());
               }
               for (int i = 0; i < tsList.Count; i++)
               {
                  editor.ContextMenuStrip.Items.Insert(i, tsList[i]);
               }
               //editor.ContextMenuStrip.Items.AddRange(tsList.ToArray());
            }
            else
               e.Cancel = true;
         }
      }

      /// <summary>
      /// Check if the word is spelled correctly.
      /// </summary>
      /// <param name="word"></param>
      /// <returns>true if word is correct</returns>
      private bool HunspellSpell(Word word)
      {
         bool result = true;
         if (word.Length > 0)
         {
            bool wordIsNumber = numberRegex.IsMatch(word.Text);
            bool wordContainsDigits = digitRegex.IsMatch(word.Text);
            bool wordIgnored = IgnoreList.Contains(word.Text);
            if (!wordIgnored && !wordIsNumber &&
                (!wordContainsDigits || (wordContainsDigits && !options.IgnoreWordsWithDigits)))
            {
               if (hunspell != null) result = hunspell.Spell(word.Text);
            }
         }
         return result;
      }

      private bool spellCheckAllowed;

      public bool SpellCheckAllowed
      {
         get { return spellCheckAllowed; }
         set
         {
            spellCheckAllowed = value;
            if (spellCheckAllowed == false)
            {
               autoSpellCheck = false;
            }
         }
      }

      private bool autoSpellCheck;

      public bool AutoSpellCheck
      {
         get { return autoSpellCheck; }
         set { autoSpellCheck = value; }
      }

      private void BackupMaxSuggestions()
      {
         maxSuggestionsBackup = this.maxSuggestions;
      }

      private void RestoreMaxSuggestions()
      {
         maxSuggestions = maxSuggestionsBackup;
      }

      private Word GetWordFromPosition()
      {
         return Word.GetWordFromPosition(editor.Text, editor.SelectionStart);
      }

      public CharacterRange GetCharRangeFromPosition(int position)
      {
         Word word = Word.GetWordFromPosition(editor.Text, position);
         return new CharacterRange(word.Start, word.Length);
      }

      private void Suggestion_Click(object sender, EventArgs e)
      {
         ToolStripItem suggestionItem = sender as ToolStripItem;
         if (suggestionItem != null)
         {
            string replacementWord = suggestionItem.Text;
            ReplaceWord(editor.SelectionStart, replacementWord);
         }
      }

      private void ReplaceWord(int position, string replacementWord)
      {
         Word word = Word.GetWordFromPosition(editor.Text, position);
         string tmpText = editor.Text.Remove(word.Start, word.Length);
         editor.Text = tmpText.Insert(word.Start, replacementWord);
         //editor.Select(word.Start, replacementWord.Length);
         //editor.SelectionLength = 0;
         editor.SelectionStart = word.Start + replacementWord.Length;
      }

      public void ShowCheckAllWindow()
      {
         RestoreMaxSuggestions();
         if (editor != null && !editor.ReadOnly && !editor.IsPassWordProtected)
         {
            spellingForm.Visible = true;
         }
      }

      private void ShowCurrentWordWindow()
      {
         RestoreMaxSuggestions();
         //spelling.Text = editor.Text;
         //usefull when we should check only one word at cursor position.
         int wordIndex = GetWordIndex(editor.Text, editor.SelectionStart);
         //spelling.SpellCheck(wordIndex);
      }

      private void CallShowCurrentWordWindow(object sender, EventArgs e)
      {
         ShowCurrentWordWindow();
      }

      public Dictionary<int, int> CheckWordAtPosition()
      {
         Dictionary<int, int> result = new Dictionary<int, int>();
         Word word = GetWordFromPosition();
         if (!HunspellSpell(word))
         {
            result.Add(word.Start, word.Length);
         }
         return result;
      }

      public int GetWordIndex(string text, int position)
      {
         int result = 0;
         MatchCollection words = CalculateWords(text);
         result = words.Count - CalculateWords(text.Substring(position)).Count;
         return result;
      }

      public int GetTextIndexFromWordIndex(string text, int wordIndex)
      {
         MatchCollection words = CalculateWords(text);
         return words[wordIndex].Index;
      }

      //
      // Returns populated collection of matches.
      //
      private MatchCollection CalculateWords(string text)
      {
         return wordEx.Matches(text);
      }

      internal bool HasSpellingErrors()
      {
         return GetFirstMisspelledWord(editor.Text, 0).Text != "";
      }

      public Dictionary<int, int> GetMisspelledWordsRanges()
      {
         return GetMisspelledWordsRanges(0);
      }

      public Dictionary<int, int> GetMisspelledWordsRanges(int startIndex)
      {
         Dictionary<int, int> result = new Dictionary<int, int>();
         for (int i = startIndex; i < editor.Text.Length; i++)
         {
            Word word = Word.GetWordFromPosition(editor.Text, i);
            if (word.Text != "" && !HunspellSpell(word))
            {
               result.Add(word.Start, word.Length);
            }
            i += word.Length;
         }
         return result;
      }

      internal Word GetFirstMisspelledWord(string text, int startIndex)
      {
         Word result = new Word();
         for (int i = startIndex; i < editor.Text.Length; i++)
         {
            result = Word.GetWordFromPosition(text, i);
            if (result.Text != "" && !HunspellSpell(result) && !ignoreList.Contains(result.Text))
            {
               break;
            }
            result.Reset();
         }
         return result;
      }

      //
      // Used to get list containing range of each word. It is not used now but can still be helpful.
      //
      internal List<CharacterRange> GetAllWordsRanges(string p)
      {
         List<CharacterRange> result = new List<CharacterRange>();
         MatchCollection allWords = wordEx.Matches(p);
         foreach (Match m in allWords)
         {
            result.Add(new CharacterRange(m.Index, m.Length));
         }
         return result;
      }
   }
}

