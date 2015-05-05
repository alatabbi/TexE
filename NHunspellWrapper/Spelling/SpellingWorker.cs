using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NHunspellComponent.Spelling.Interfaces;

namespace NHunspellComponent.Spelling
{
   //[ToolboxBitmap("SpellingWorker.ico")] //Unnecessary attribute
   // Just put SpellingWorker.bmp into the project. Filename should just have the same name as component class and 
   // be compiled as Embedded Resource.
   public class SpellingWorker : Component
   {
      public Dictionary<int, int> UnderlinedSections;
      public Dictionary<int, int> protectedSections;
      public Dictionary<int, int> ignoredSections;
      private bool whiteSpacePressed;
      private bool otherSignificantPressed;
      private CharacterRange lastSelectedWord;
      private string backupText = "";
      private ISpellingControl editor;
      private Options options;
      private bool initialized;

      public delegate void EmptyNotificartion();

      public event EmptyNotificartion UnderliningChanged;

      private IUnderlineableSpellingControl UnderlinableEditor
      {
         get { return editor as IUnderlineableSpellingControl; }
      }

      public SpellingWorker()
      {
      }

      public SpellingWorker(ISpellingControl control)
      {
         Editor = control;
      }

      [Browsable(true), Category("Spelling")]
      [Description("Gets or sets the editor for current SpellingWorker")]
      public ISpellingControl Editor
      {
         get { return editor; }
         set
         {
            if (value != null)
            {
               editor = value;
               editor.Enter += editor_Enter;
            }
            editor = value;
         }
      }

      [Browsable(true), Category("Spelling"), DefaultValue(true)]
      [Description("Specifies wheather")]
      public bool IsEditorSpellingEnabled
      {
         get { return Editor != null && Editor.IsSpellingEnabled; }
         set { if (Editor != null) Editor.IsSpellingEnabled = value; }
      }

      [Browsable(true), Category("Spelling"), DefaultValue(true)]
      [Description("Gets or sets the name in the text box")]
      public bool IsEditorSpellingAutoEnabled
      {
         get { return UnderlinableEditor != null && UnderlinableEditor.IsSpellingAutoEnabled; }
         set { if (UnderlinableEditor != null) UnderlinableEditor.IsSpellingAutoEnabled = value; }
      }

      //
      // Notify the NHunspellWrapper about editor it will work with.
      //
      private void editor_Enter(object sender, EventArgs e)
      {
         NHunspellWrapper.Instance.Editor = editor;
         InitializeEditor();
         NHunspellWrapper.Instance.OptionsOfCurEditor = options;
      }

      private void InitializeEditor()
      {
         if (!initialized && editor != null && editor.IsSpellingEnabled)
         {
            NHunspellWrapper.Instance.PropertiesChanged += Instance_PropertiesChanged;
            NHunspellWrapper.Instance.OptionsChanged += Instance_OptionsChanged;

            if (UnderlinableEditor != null)
            {
               NHunspellWrapper.Instance.AutoSpellCheck = UnderlinableEditor.IsSpellingAutoEnabled;
               UnderlinableEditor.KeyDown -= editor_KeyDown;
               UnderlinableEditor.SelectionChanged -= editor_SelectionChanged;
               UnderlinableEditor.KeyDown += editor_KeyDown;
               UnderlinableEditor.SelectionChanged += editor_SelectionChanged;
               UnderlinableEditor.TextChanged += UnderlinableEditor_TextChanged;

               #region CheckTextBoxInitial

               UnderlineIncorrectWords(UnderlinableEditor);

               #endregion
            }
            initialized = true;
         }
      }

      private void Instance_OptionsChanged(Options opts)
      {
         if (editor == NHunspellWrapper.Instance.Editor)
         {
            options = opts;
         }
      }

      //
      // When pressed Ignore, Add, Change in spellingForm or closed it we should notify
      // spelling worker should make some action to update editor.
      //
      private void Instance_PropertiesChanged()
      {
         UnderlineNewPositionWords(UnderlinableEditor, false);
      }

      //
      // The place where we detect keys to which spelling criteria should react later.
      //
      private void editor_KeyDown(object sender, KeyEventArgs e)
      {
         if (char.IsWhiteSpace((char) e.KeyValue) || char.IsPunctuation((char) e.KeyValue) &&
                                                     char.IsSymbol((char) e.KeyValue))
         {
            whiteSpacePressed = true;
         }
         if (e.KeyCode == Keys.Delete)
         {
            otherSignificantPressed = true;
         }
      }

      // 
      // The place where we detect weather we should checkspelling or not.
      // 
      private void editor_SelectionChanged(object sender, EventArgs e)
      {
         bool needUnderline = false;
         if (whiteSpacePressed)
         {
            whiteSpacePressed = false;
            needUnderline = true;
         }
         else
         {
            CharacterRange curWordRange = NHunspellWrapper.Instance.GetCharRangeFromPosition(editor.SelectionStart);
            if (lastSelectedWord.First != curWordRange.First) //mouse click in different position
            {
               if (!UnderlinableEditor.UnderlinedSections.ContainsKey(lastSelectedWord.First))
               {
                  UnderlineIncorrectWords(UnderlinableEditor);
               }
            }
            else //Remove underlining under cursor if length of curWord has changed
            {
               if (UnderlinableEditor.UnderlinedSections.ContainsKey(curWordRange.First) &&
                   UnderlinableEditor.UnderlinedSections[curWordRange.First] != curWordRange.Length ||
                   lastSelectedWord.Length != curWordRange.Length) //the same word length is different now
               {
                  //Should determine every offset after text changes and make offsets for underlining list
                  //OR as a variant unconditionnaly underline all incorrect words
                  UnderlineNewPositionWords(UnderlinableEditor, true);
                  RemoveUnderliningAt(UnderlinableEditor, curWordRange.First);
               }
            }
            lastSelectedWord = curWordRange;

            int collapseIndex = findCollapseIndex(backupText, editor.Text);
            if (collapseIndex < editor.Text.Length)
            {
               UnderlineNewPositionWords(UnderlinableEditor, true);
            }
         }

         if (needUnderline)
         {
            //UnderlineIncorrectWords(editor);
            UnderlineNewPositionWords(UnderlinableEditor, true);
         }
      }


      private void UnderlinableEditor_TextChanged(object sender, EventArgs e)
      {
         if (otherSignificantPressed)
         {
            UnderlineNewPositionWords(UnderlinableEditor, true);
            otherSignificantPressed = false;
         }
      }

      //
      // Update UnderlinedSections variable of the editor.
      //
      private void UnderlineIncorrectWords(IUnderlineableSpellingControl editor)
      {
         if (editor.IsSpellingAutoEnabled)
         {
            UnderlineNewPositionWords(editor, false);
            //Should Invalidate becasue of no Invalidate called on cursor position changed.
            editor.Invalidate(true);
         }
         backupText = editor.Text;
      }

      //
      // Update UnderlinedSections variable of the editor, but depending on text changing logic.
      // Only words that changed it's position are rechecked.
      // Logic is something that can be always improved.
      //
      private void UnderlineNewPositionWords(IUnderlineableSpellingControl editor, bool newPositionWordsOnly)
      {
         if (UnderlinableEditor != null && UnderlinableEditor.IsSpellingAutoEnabled)
         {
            if (newPositionWordsOnly)
            {
               int collapseIndex = findCollapseIndex(backupText, editor.Text);
               collapseIndex = NHunspellWrapper.Instance.GetCharRangeFromPosition(collapseIndex).First;

               Dictionary<int, int> tmp = new Dictionary<int, int>();
               foreach (int key in editor.UnderlinedSections.Keys)
               {
                  if (key < collapseIndex)
                  {
                     tmp.Add(key, editor.UnderlinedSections[key]);
                  }
               }
               editor.UnderlinedSections = tmp;

               Dictionary<int, int> newMisspelledWordRanges =
                  NHunspellWrapper.Instance.GetMisspelledWordsRanges(collapseIndex);

               foreach (int key in newMisspelledWordRanges.Keys)
               {
                  if (!editor.UnderlinedSections.ContainsKey(key))
                  {
                     editor.UnderlinedSections.Add(key, newMisspelledWordRanges[key]);
                  }
               }
            }
            else
            {
               editor.UnderlinedSections =
                  NHunspellWrapper.Instance.GetMisspelledWordsRanges();
            }
            backupText = UnderlinableEditor.Text;
            if (UnderliningChanged != null)
            {
               UnderliningChanged();
            }
         }
      }

      //
      // Remove underlining under word.
      //
      public bool RemoveUnderliningAt(IUnderlineableSpellingControl editor, int wordCharIndex)
      {
         bool result = false;
         int closestKeyLeft = int.MaxValue;
         foreach (int key in editor.UnderlinedSections.Keys)
         {
            if (wordCharIndex - key >= 0)
            {
               closestKeyLeft = key;
            }
            else
            {
               break;
            }
         }
         CharacterRange curWordRange = NHunspellWrapper.Instance.GetCharRangeFromPosition(editor.SelectionStart);
         if ((curWordRange.First <= wordCharIndex && wordCharIndex < curWordRange.First + curWordRange.Length) &&
             editor.UnderlinedSections.ContainsKey(curWordRange.First))
         {
            editor.RemoveWordFromUnderliningList(closestKeyLeft);
            result = true;
         }
         return result;
      }

      //
      // Detects the place where text changed.
      //
      private int findCollapseIndex(string initialString, string newString)
      {
         int len = Math.Min(initialString.Length, newString.Length);
         int result = len;

         for (int i = 0; i < len; i++)
         {
            if (initialString[i] != newString[i])
            {
               result = i;
               break;
            }
         }
         return result;
      }
   }
}