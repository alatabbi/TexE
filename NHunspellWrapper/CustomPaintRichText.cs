using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NHunspellComponent.Spelling.Interfaces;
using NHunspellComponent.SupportClasses;

namespace NHunspellComponent
{
   public class CustomPaintRichText : RichTextBox, IUnderlineableSpellingControl
   {
      public Dictionary<int, int> underlinedSections;
      public Dictionary<int, int> protectedSections;
      public Dictionary<int, int> ignoredSections;

      public Dictionary<int, int> UnderlinedSections
      {
         get
         {
            if (underlinedSections == null)
               underlinedSections = new Dictionary<int, int>();
            return underlinedSections;
         }
         set { underlinedSections = value; }
      }

      public Dictionary<int, int> ProtectedSections
      {
         set { protectedSections = value; }
      }

      public Dictionary<int, int> IgnoredSections
      {
         set { ignoredSections = value; }
      }

      #region ISpellingControl Members

      private bool spellingEnabled;
      private bool spellingAutoEnabled;
      private bool isPassWordProtected;

      public bool IsSpellingEnabled
      {
         get { return spellingEnabled; }
         set { spellingEnabled = value; }
      }

      public bool IsSpellingAutoEnabled
      {
         get { return spellingAutoEnabled; }
         set
         {
            spellingAutoEnabled = value;
            if (!spellingEnabled) spellingEnabled = value;
         }
      }

      public bool IsPassWordProtected
      {
         get { return isPassWordProtected; }
         set { isPassWordProtected = value; }
      }

      #endregion

      /// <summary>
      /// This is called when the textbox is being redrawn.
      /// When it is, for the textbox to get refreshed, call it's default
      /// paint method and then call our method
      /// </summary>
      /// <param name="m">The windows message</param>
      /// <remarks></remarks>
      protected override void WndProc(ref System.Windows.Forms.Message m)
      {
         switch (m.Msg)
         {
            case 15:
               //This is the WM_PAINT message
               //Invalidate the textBoxBase so that it gets refreshed properly
               this.Invalidate();

               //call the default win32 Paint method for the TextBoxBase first
               base.WndProc(ref m);

               //now use our code to draw the extra stuff
               if (!this.ReadOnly && IsSpellingAutoEnabled)
               {
                  this.CustomPaint();
               }
               break;
            default:
               base.WndProc(ref m);
               break;
         }
      }

      //
      // Draws wavy underlines in editor.
      // Capable to draw underlines for multiline words.
      //
      public void CustomPaint()
      {
         Bitmap wavyUnderlinesBitmap;
         Graphics editorGraphics;
         Graphics bufferGraphics;

         //Create a bitmap with the same dimensions as the textbox
         wavyUnderlinesBitmap = new Bitmap(this.Width, this.Height);

         //Create the graphics object from this bitmpa...this is where we will draw the lines to start with
         bufferGraphics = Graphics.FromImage(wavyUnderlinesBitmap);
         bufferGraphics.Clip = new Region(this.ClientRectangle);

         //Get the graphics object for the textbox.  We use this to draw the bufferGraphics
         editorGraphics = Graphics.FromHwnd(this.Handle);

         // clear the graphics buffer
         bufferGraphics.Clear(Color.Transparent);

         foreach (int wordStart in UnderlinedSections.Keys)
         {
            if (ignoredSections != null && ignoredSections.ContainsKey(wordStart))
            {
               continue;
            }

            int wordEndIndex = wordStart + UnderlinedSections[wordStart] - 1;
            Point start = this.GetPositionFromCharIndex(wordStart);
            Point end = this.GetPositionFromCharIndex(wordEndIndex);

            int curIndex = wordStart;
            int safetyDrewOnce = -1;
            if (curIndex < Text.Length)
               do
               {
                  start = this.GetPositionFromCharIndex(curIndex);
                  //Determine the first line of waves to draw
                  while (curIndex <= wordEndIndex)
                  {
                     if (curIndex < Text.Length && this.GetPositionFromCharIndex(curIndex).Y == start.Y)
                     {
                        curIndex += 1;
                     }
                     else
                     {
                        curIndex--;
                        break;
                     }
                  }
                  end = this.GetPositionFromCharIndex(curIndex);

                  // The position above now points to the top left corner of the character.
                  // We need to account for the character height so the underlines go
                  // to the right place.
                  end.X += 1;
                  int yOffset = TextBoxAPIHelper.GetBaselineOffsetAtCharIndex(this, wordStart);
                  start.Y += yOffset;
                  end.Y += yOffset;

                  //Add a new wavy line using the starting and ending point
                  DrawWave(bufferGraphics, start, end);
                  if (safetyDrewOnce != curIndex)
                  {
                     safetyDrewOnce = curIndex;
                  }
                  else
                  {
                     break;
                  }
                  curIndex += 1;
               } //TODO: something with indeces
                  //Replace words in text with empty words.
               while (curIndex <= wordEndIndex);
         }
         // Now we just draw our internal buffer on top of the TextBox.
         // Everything should be at the right place.
         editorGraphics.DrawImageUnscaled(wavyUnderlinesBitmap, 0, 0);
      }

      /// <summary>
      /// Draws the wavy red line given a starting point and an ending point
      /// </summary>
      /// <param name="StartOfLine">A Point representing the starting point</param>
      /// <param name="EndOfLine">A Point representing the ending point</param>
      /// <remarks></remarks>
      private void DrawWave(Graphics graphics, Point StartOfLine, Point EndOfLine)
      {
         //correction to draw line closer to text
         StartOfLine.Y--;
         EndOfLine.Y--;

         Pen newPen = Pens.Red;

         if ((EndOfLine.X - StartOfLine.X) > 4)
         {
            ArrayList pl = new ArrayList();
            for (int i = StartOfLine.X; i <= (EndOfLine.X - 2); i += 4)
            {
               pl.Add(new Point(i, StartOfLine.Y));
               pl.Add(new Point(i + 2, StartOfLine.Y + 2));
            }

            Point[] p = (Point[]) pl.ToArray(typeof (Point));
            graphics.DrawLines(newPen, p);
         }
         else
         {
            graphics.DrawLine(newPen, StartOfLine, EndOfLine);
         }
      }

      public void AddUnderlinedSection(int s, int l)
      {
         if (UnderlinedSections.ContainsKey(s))
         {
            underlinedSections.Remove(s);
         }
         underlinedSections.Add(s, l);
      }

      public void RemoveWordFromUnderliningList(int wordStart)
      {
         if (underlinedSections.ContainsKey(wordStart))
         {
            underlinedSections.Remove(wordStart);
            //this.Invalidate();
         }
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Right)
         {
            int position = this.GetCharIndexFromPosition(e.Location);
            if (position == Text.Length - 1)
            {
               position++;
            }
            if (position < this.SelectionStart ||
                position > this.SelectionStart + this.SelectionLength)
            {
               this.Select(position, 0);
            }
         }
         base.OnMouseDown(e);
      }

      internal void AddToIgnoreList(int start, int length)
      {
         if (ignoredSections != null && ignoredSections.ContainsKey(start))
         {
            ignoredSections.Remove(start);
         }
         if (ignoredSections != null)
            ignoredSections.Add(start, length);
      }
   }
}