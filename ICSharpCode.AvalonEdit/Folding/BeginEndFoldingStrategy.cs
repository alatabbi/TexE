// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using System.Windows;
 

namespace AvalonEdit.Sample
{

   

   	/// <summary>
	/// Allows producing foldings from a document based on braces.
	/// </summary>
	public class BeginEndFoldingStrategy : AbstractFoldingStrategy
	{
		/// <summary>
		/// Gets/Sets the opening brace. The default value is '{'.
		/// </summary>
		public string OpeningBrace { get; set; }
		
		/// <summary>
		/// Gets/Sets the closing brace. The default value is '}'.
		/// </summary>
		public string ClosingBrace { get; set; }
		
		/// <summary>
		/// Creates a new BraceFoldingStrategy.
		/// </summary>
		public BeginEndFoldingStrategy()
		{
            this.OpeningBrace = Environment.NewLine + @"\begin{";
            this.ClosingBrace = Environment.NewLine + @"\end{";
		}
		
		/// <summary>
		/// Create <see cref="NewFolding"/>s for the specified document.
		/// </summary>
		public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
		{
			firstErrorOffset = -1;
			return CreateNewFoldings(document);
		}
		
		/// <summary>
		/// Create <see cref="NewFolding"/>s for the specified document.
		/// </summary>
		public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
		{
			List<NewFolding> newFoldings = new List<NewFolding>();
			
			Stack<int> startOffsets = new Stack<int>();
            List<int> UsedClosingEnds= new List<int>();
			//int lastNewLineOffset = 0;
			string openingBrace = this.OpeningBrace;
			string closingBrace = this.ClosingBrace;
            int ind=document.Text.IndexOf(openingBrace, 0);
            while (ind > 0)
            {
                startOffsets.Push(ind);
                ind = document.Text.IndexOf(openingBrace, ind+1);
            }

            //List<int> test = new List<int>();
            //int indTest = document.Text.IndexOf(closingBrace, 0);
            //while (indTest > 0)
            //{
            //    test.Add(indTest);
            //    indTest = document.Text.IndexOf(closingBrace, indTest + 1);
            //}
            while (startOffsets.Count > 0)
            {
                int startOffset = startOffsets.Pop();
                int closingInd =document.Text.IndexOf(closingBrace, startOffset + 1);   
                while (closingInd>-1)
                {
                    if (!UsedClosingEnds.Contains(closingInd))
                    {
                        newFoldings.Add(new NewFolding(startOffset+2, closingInd+2));
                        UsedClosingEnds.Add(closingInd);
                        break;
                    }
                    closingInd = document.Text.IndexOf(closingBrace, closingInd + 1);
                }
            }
            #region old code
            //for (int i = 0; i < document.TextLength; i++) {
            //    char c = document.GetCharAt(i);
            //if (c == openingBrace) {
            //    startOffsets.Push(i);
            //} 
            //else if (c == closingBrace && startOffsets.Count > 0) {
            //    int startOffset = startOffsets.Pop();
            //    // don't fold if opening and closing brace are on the same line
            //    if (startOffset < lastNewLineOffset) {
            //        newFoldings.Add(new NewFolding(, startOffseti + 1));
            //    }
            //} 
            //else if (c == '\n' || c == '\r') {
            //    lastNewLineOffset = i + 1;
            //}
            //} 
            #endregion
			newFoldings.Sort((a,b) => a.StartOffset.CompareTo(b.StartOffset));
			return newFoldings;
		}
	}
}
