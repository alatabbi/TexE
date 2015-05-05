﻿
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using NHunspell;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

[ToolboxBitmap(typeof(NHunspellTextBoxExtender), "spellcheck.png"), ProvideProperty("SpellCheckEnabled", typeof(Control))]
public class NHunspellTextBoxExtender : Component, IExtenderProvider, ISupportInitialize
{

	//Private Const MessageLogPath As String = "D:\Messagelog6.txt"


	#region "Private Classes"


	/// <summary>
	/// This is the class that handles painting the wavy red lines.
	/// 
	/// It utilizes the NativeWindow to find out when it needs to draw
	/// </summary>
	/// <remarks></remarks>
	private class CustomPaintTextBox : NativeWindow
	{

		private TextBoxBase parentTextBox;
		private Bitmap myBitmap;
		private Graphics textBoxGraphics;
		private Graphics bufferGraphics;
		private SpellCheckControl mySpellCheckControl;

		private NHunspellTextBoxExtender myParent;
		public event CustomPaintCompleteEventHandler CustomPaintComplete;
		public delegate void CustomPaintCompleteEventHandler(TextBoxBase sender, long Milliseconds);

		/// <summary>
		/// This is called when the textbox is being redrawn.
		/// When it is, for the textbox to get refreshed, call it's default
		/// paint method and then call our method
		/// </summary>
		/// <param name="m">The windows message</param>
		/// <remarks></remarks>
		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			switch (m.Msg) {
				case 15:
					//This is the WM_PAINT message
					//Invalidate the textBoxBase so that it gets refreshed properly
					parentTextBox.Invalidate();

					//call the default win32 Paint method for the TextBoxBase first
					base.WndProc(m);

					//now use our code to draw the extra stuff
					this.CustomPaint();

					break;
				default:
					base.WndProc(m);
					break;
			}
		}

		public CustomPaintTextBox(ref TextBoxBase CallingTextBox, ref SpellCheckControl ThisSpellCheckControl, ref NHunspellTextBoxExtender Parent)
		{
			//Set up the CustomPaintTextBox
			parentTextBox = CallingTextBox;

			//Create the link to the parent
			myParent = Parent;

			//Create a bitmap with the same dimensions as the textbox
			myBitmap = new Bitmap(parentTextBox.Width, parentTextBox.Height);

			//Create the graphics object from this bitmpa...this is where we will draw the lines to start with
			bufferGraphics = Graphics.FromImage(myBitmap);
			bufferGraphics.Clip = new Region(parentTextBox.ClientRectangle);

			//Get the graphics object for the textbox.  We use this to draw the bufferGraphics
			textBoxGraphics = Graphics.FromHwnd(parentTextBox.Handle);

			//Assign a handle for this class and set it to the handle for the textbox
			this.AssignHandle(parentTextBox.Handle);

			//We also need to make sure we update the handle if the handle for the textbox changes
			//This occurs if wordWrap is turned off for a RichTextBox
			parentTextBox.HandleCreated += TextBoxBase_HandleCreated;

			//We need to add a handler to change the clip rectangle if the textBox is resized
			parentTextBox.ClientSizeChanged += TextBoxBase_ClientSizeChanged;

			//Now set our spellchecker
			mySpellCheckControl = ThisSpellCheckControl;
		}

		/// <summary>
		/// Gets the ranges of chars that represent the spelling errors and then draw a wavy red line underneath
		/// them.
		/// </summary>
		/// <remarks></remarks>
		//ByVal sender As Object, ByVal e As DoWorkEventArgs)
		private void CustomPaint()
		{
			//Determine if we need to draw anything
			if (!mySpellCheckControl.HasSpellingErrors)
				return;
			if (!myParent.IsEnabled(ref parentTextBox))
				return;
			if (!myParent.SpellAsYouType)
				return;

			//Benchmarking
			DateTime startTime = Now;

			RichTextBox tempRTB = null;

			if (parentTextBox is RichTextBox) {
				tempRTB = new RichTextBox();
				tempRTB.Rtf = ((RichTextBox)parentTextBox).Rtf;
			}

			//Clear the graphics buffer
			bufferGraphics.Clear(Color.Transparent);

			//Now, find out if any of the spelling errors are within the visible part of the textbox
			CharacterRange[] CharRanges = mySpellCheckControl.GetSpellingErrorRanges;
			CharacterRange[] visibleRanges = null;
			 // ERROR: Not supported in C#: ReDimStatement


			//First get the ranges of characters visible in the textbox
			Point startPoint = new Point(0, 0);
			Point endPoint = new Point(parentTextBox.ClientRectangle.Width, parentTextBox.ClientRectangle.Height);
			long startIndex = parentTextBox.GetCharIndexFromPosition(startPoint);
			long endIndex = parentTextBox.GetCharIndexFromPosition(endPoint);

			//Benchmarking
			//Dim visibleStartTime As DateTime = Now

			//Go through each of the charRanges that were returned and see if they're visible

			for (i = 0; i <= Information.UBound(CharRanges); i++) {
				int rangeStart = -1;
				int rangeEnd = -1;

				//See if it's an ignore range
				CharacterRange currentIgnoreRange = default(CharacterRange);
				bool ignoreRange = false;

				foreach (CharacterRange  currentIgnoreRange in mySpellCheckControl.GetIgnoreRanges) {
					if ((currentIgnoreRange.First == CharRanges(i).First) & (currentIgnoreRange.Length == CharRanges(i).Length)) {
						ignoreRange = true;
					}
				}

				//If it's not a range we're to ignore, then see if it's visible
				if (!ignoreRange) {
					for (j = CharRanges(i).First; j <= (CharRanges(i).First + (CharRanges(i).Length) - 1); j++) {
						var _with1 = parentTextBox;
						if (j >= startIndex & j <= endIndex) {
							if (rangeStart == -1) {
								rangeStart = j;
							} else {
								rangeEnd = j;
							}
						}
					}

					//Now add a new visibleRange to the array
					if (rangeStart != -1 & rangeEnd != -1) {
						CharacterRange newRange = new CharacterRange(rangeStart, rangeEnd - rangeStart + 1);
						Array.Resize(ref visibleRanges, Information.UBound(visibleRanges) + 2);
						visibleRanges(Information.UBound(visibleRanges)) = newRange;
					}
				}
			}

			//Dim visibleDiff As TimeSpan = Now.Subtract(visibleStartTime)
			//Debug.Print("VisibleRanges: " & visibleDiff.TotalMilliseconds & " Milliseconds")

			//Now that we have the ranges that are visible, we're going to create the end points
			//to call the drawWave
			CharacterRange currentRange = default(CharacterRange);


			//Benchmarking
			//Dim drawStartTime As DateTime = Now

			foreach ( currentRange in visibleRanges) {
				//Get the X, Y of the start and end characters
				startPoint = parentTextBox.GetPositionFromCharIndex(currentRange.First);
				endPoint = parentTextBox.GetPositionFromCharIndex(currentRange.First + currentRange.Length - 1);

				if (startPoint.Y != endPoint.Y) {
					//We have a word on multiple lines
					int curIndex = 0;
					int startingIndex = 0;
					curIndex = currentRange.First;
					startingIndex = curIndex;
					GetNextLine:

					//Determine the first line of waves to draw
					while ((parentTextBox.GetPositionFromCharIndex(curIndex).Y == startPoint.Y) & (curIndex <= (currentRange.First + currentRange.Length - 1))) {
						curIndex += 1;
					}

					//Go back to the previous character
					curIndex -= 1;

					endPoint = parentTextBox.GetPositionFromCharIndex(curIndex);
					Point offsets = GetOffsets(ref parentTextBox, startingIndex, curIndex, tempRTB);

					//Dim offsetsDiff As TimeSpan = Now.Subtract(startTime)
					//Debug.Print("Get Offsets: " & offsetsDiff.TotalMilliseconds & " Milliseconds")

					//If we're using a RichTextBox, we have to account for the zoom factor
					if (parentTextBox is RichTextBox)
						offsets.Y *= ((RichTextBox)parentTextBox).ZoomFactor;

					//Reset the starting and ending points to make sure we're underneath the word
					//(The measurestring adds some margin, so remove them)
					startPoint.Y += offsets.Y - 2;
					endPoint.Y += offsets.Y - 2;
					endPoint.X += offsets.X - 0;

					//Add a new wavy line using the starting and ending point
					DrawWave(startPoint, endPoint);

					//Dim drawWaveDiff As TimeSpan = Now.Subtract(startTime)
					//Debug.Print("DrawWave: " & drawWaveDiff.TotalMilliseconds & " Milliseconds")

					startingIndex = curIndex + 1;
					curIndex += 1;
					startPoint = parentTextBox.GetPositionFromCharIndex(curIndex);

					if (curIndex <= (currentRange.First + currentRange.Length - 1)) {
						goto GetNextLine;
					}
				} else {
					Point offsets = GetOffsets(ref parentTextBox, currentRange.First, (currentRange.First + currentRange.Length - 1), tempRTB);

					//Dim offsetsDiff As TimeSpan = Now.Subtract(startTime)
					//Debug.Print("Get Offsets: " & offsetsDiff.TotalMilliseconds & " Milliseconds")

					//If we're using a RichTextBox, we have to account for the zoom factor
					if (parentTextBox is RichTextBox)
						offsets.Y *= ((RichTextBox)parentTextBox).ZoomFactor;

					//Reset the starting and ending points to make sure we're underneath the word
					//(The measurestring adds some margin, so remove them)
					startPoint.Y += offsets.Y - 2;
					endPoint.Y += offsets.Y - 2;
					endPoint.X += offsets.X - 4;

					//Add a new wavy line using the starting and ending point
					//If e.Cancel Then Return
					DrawWave(startPoint, endPoint);

					//Dim drawWaveDiff As TimeSpan = Now.Subtract(startTime)
					//Debug.Print("DrawWave: " & drawWaveDiff.TotalMilliseconds & " Milliseconds")
				}
			}

			//Dim drawDiff As TimeSpan = Now.Subtract(drawStartTime)
			//Debug.Print("Draw: " & drawDiff.TotalMilliseconds & " Milliseconds")

			//We've drawn all of the wavy lines, so draw that image over the textbox
			textBoxGraphics.DrawImageUnscaled(myBitmap, 0, 0);

			//Dim dateDiff As TimeSpan = Now.Subtract(startTime)
			//Debug.Print("----TotalTime: " & dateDiff.Seconds & " Seconds, " & dateDiff.Milliseconds & " Milliseconds------------")

			if (CustomPaintComplete != null) {
				CustomPaintComplete(parentTextBox, Now.Subtract(startTime).TotalMilliseconds);
			}
		}

		/// <summary>
		/// Determines the X and Y offsets to use based on font height last letter width
		/// </summary>
		/// <param name="curTextBox"></param>
		/// <param name="startingIndex"></param>
		/// <param name="endingIndex"></param>
		/// <param name="tempRTB"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private Point GetOffsets(ref TextBoxBase curTextBox, int startingIndex, int endingIndex, RichTextBox tempRTB = null)
		{
			DateTime startTime = Now;

			//We now have the top left point of the characters, now we need to add the offsets
			int offsetY = 0;
			Font fontToUse = curTextBox.Font;
			Point offsets = default(Point);

			fontToUse = new Font(fontToUse.FontFamily, 0.1, fontToUse.Style, fontToUse.Unit, fontToUse.GdiCharSet, fontToUse.GdiVerticalFont);

			//If it's a RichTextBox, we have to do some extra things
			if (curTextBox is RichTextBox) {
				//We need to go through every character where we will draw the lines and get the tallest
				//font height

				//Benchmarking
				//Dim beforeCreateTextBoxDiff As TimeSpan = Now.Subtract(startTime)
				//Debug.Print("    Before Create TextBox: " & beforeCreateTextBoxDiff.TotalMilliseconds & " Milliseconds")

				//Create a temporary textbox for getting the RTF info so that we don't have to select and
				//de-select a lot of text and cause the screen to have to refresh
				if (tempRTB == null) {
					tempRTB = new RichTextBox();
					tempRTB.Rtf = ((RichTextBox)curTextBox).Rtf;
				}

				//Benchmarking
				//Dim createTextBoxDiff As TimeSpan = Now.Subtract(startTime)
				//Debug.Print("    Create TextBox: " & createTextBoxDiff.TotalMilliseconds & " Milliseconds")

				var _with2 = tempRTB;
				if (_with2.Text.Length > 0) {
					//Have to find the first visible character on that line
					long firstCharInLine = 0;
					long lastCharInLine = 0;
					long curCharLine = 0;
					curCharLine = _with2.GetLineFromCharIndex(startingIndex);
					firstCharInLine = _with2.GetFirstCharIndexFromLine(curCharLine);
					lastCharInLine = _with2.GetFirstCharIndexFromLine(curCharLine + 1);

					if (lastCharInLine == -1)
						lastCharInLine = curTextBox.TextLength;

					DateTime getFontHeightStart = Now;

					//Now go through every character that is visible and get the biggest font height
					//Use the tempRTB for this
					for (i = firstCharInLine + 1; i <= (lastCharInLine + 1); i++) {
						_with2.SelectionStart = i;
						_with2.SelectionLength = 1;
						if (_with2.SelectionFont.Height > fontToUse.Height) {
							//fontHeight = .SelectionFont.Height
							fontToUse = _with2.SelectionFont;
						}
					}

					//Benchmarking
					//Dim foundHeightdiff As TimeSpan = Now.Subtract(startTime)
					//Debug.Print("    Get Font Height: " & foundHeightdiff.TotalMilliseconds & " Milliseconds")

					offsetY = fontToUse.Height;
				}

			} else {
				//If we get here, it's just a standard textbox and we can just use the font height
				fontToUse = curTextBox.Font;

				offsetY = curTextBox.Font.Height;
			}

			//Now find out how wide the last character is
			int offsetX = 0;
			offsetX = textBoxGraphics.MeasureString(curTextBox.Text(startingIndex + (endingIndex - startingIndex)), fontToUse).Width;

			offsets = new Point(offsetX, offsetY);

			//Benchmarking
			//Dim timeDiff As TimeSpan = Now.Subtract(startTime)
			//Debug.Print("GetOffsets: " & timeDiff.TotalMilliseconds & " Milliseconds")

			return offsets;
		}

		/// <summary>
		/// The textbox is not redrawn much, so this will force the textbox to call the custom paint function.
		/// Otherwise, text can be entered and no wavy red lines will appear
		/// </summary>
		/// <remarks></remarks>
		public void ForcePaint()
		{
			parentTextBox.Invalidate();
		}

		/// <summary>
		/// Draws the wavy red line given a starting point and an ending point
		/// </summary>
		/// <param name="StartOfLine">A Point representing the starting point</param>
		/// <param name="EndOfLine">A Point representing the ending point</param>
		/// <remarks></remarks>
		private void DrawWave(Point StartOfLine, Point EndOfLine)
		{
			Pen newPen = Pens.Red;

			if ((EndOfLine.X - StartOfLine.X) > 4) {
				ArrayList pl = new ArrayList();
				for (i = StartOfLine.X; i <= (EndOfLine.X - 2); i += 4) {
					pl.Add(new Point(i, StartOfLine.Y));
					pl.Add(new Point(i + 2, StartOfLine.Y + 2));
				}

				Point[] p = (Point[])pl.ToArray(typeof(Point));
				bufferGraphics.DrawLines(newPen, p);
			} else {
				bufferGraphics.DrawLine(newPen, StartOfLine, EndOfLine);
			}
		}

		/// <summary>
		/// Reassign this classes handle and the graphics object anytime the textbox's handle is changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void TextBoxBase_HandleCreated(object sender, System.EventArgs e)
		{
			this.AssignHandle(parentTextBox.Handle);
			textBoxGraphics = Graphics.FromHwnd(parentTextBox.Handle);
		}

		/// <summary>
		/// When the TextBoxBase is resized, this will reset the objects that are used to draw
		/// the wavy, red line.  Without this, anything outside of the original bounds will not
		/// be drawn
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void TextBoxBase_ClientSizeChanged(object sender, System.EventArgs e)
		{
			TextBoxBase tempTextBox = sender;

			//Create a bitmap with the same dimensions as the textbox
			myBitmap = new Bitmap(tempTextBox.Width, tempTextBox.Height);

			//Create the graphics object from this bitmpa...this is where we will draw the lines to start with
			bufferGraphics = Graphics.FromImage(myBitmap);
			bufferGraphics.Clip = new Region(tempTextBox.ClientRectangle);

			//Get the graphics object for the textbox.  We use this to draw the bufferGraphics
			textBoxGraphics = Graphics.FromHwnd(tempTextBox.Handle);
		}
	}


	#endregion




	#region "Variables"

	private Hunspell myNHunspell = null;
	//Hashtables
	private Hashtable controlEnabled;
	private Hashtable mySpellCheckers;
	private Hashtable myCustomPaintingTextBoxes;
	private Hashtable myContextMenus;
	//Private testHash As Hashtable

	//Other
	private bool controlPressed = false;
	private CustomPaintTextBox drawTest;
	private Control[] myControls;
	private bool boolDisableAddWordPrompt = false;

	private bool initializing = false;
	//Property values
	private bool _SpellAsYouType;
	private Shortcut _shortcutKey;
	private int myNumOfSuggestions;

	private string _Language;
	[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	//Declared functions
	private static extern int GetScrollPos(IntPtr hWnd, int nBar);
	[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
	[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	private static extern bool PostMessageA(IntPtr hwnd, int wMsg, int wParam, int lParam);
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
	private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

	// Scrollbar direction
	const  SBS_HORZ = 0;

	const  SBS_VERT = 1;
	// Windows Messages
	const  WM_VSCROLL = 0x115;
	const  WM_HSCROLL = 0x114;

	const  SB_THUMBPOSITION = 4;
	//Redrawing
		#endregion
	private const Int32 WM_SETREDRAW = 0xb;




	#region "ISupportInitialize"
	public void BeginInit()
	{
		initializing = true;
	}

	public void EndInit()
	{
		initializing = false;
	}
	#endregion




	#region "New and CanExtend Methods"



	/// <summary>
	/// Determines which items this extender can extend.  It is only objects that implement TextBoxBase
	/// </summary>
	/// <param name="extendee">The control being checked</param>
	/// <returns>A boolean value indicating whether it can be extended</returns>
	/// <remarks></remarks>
	public bool CanExtend(object extendee)
	{
		return (extendee is TextBoxBase) & ((myNHunspell != null));
	}




	/// <summary>
	/// We need to make sure that the dic and aff files are on the disk.  Then, we try to create
	/// the Hunspell object.  After that, we set up the hashtables and tooltip
	/// </summary>
	/// <remarks></remarks>
	public NHunspellTextBoxExtender()
	{
		//Biggest problem is the requirement to have two dictionary files on the HDD along with
		//either the x64 or x86 Hunspell DLL which are not .NET dlls.
		//To get around this, we find the directory that the program is being called from and add
		//the dictionary files.
		//Then, we try to create the Hunspell and if a "DLL not found" error is thrown, we find out
		//where the dll's were supposed to be and then add them.

		this.LanguageChanged += MyLanguageChanged;
		MaintainUserChoice = true;

		string USdic = null;
		string USaff = null;

		//Get the calling assembly's location
		string callingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location);

		//Try and write to the directory to see if we can
		bool boolFailed = false;

		try {
			Directory.CreateDirectory(callingDir + "\\Test");
		} catch (Exception ex) {
			boolFailed = true;
		} finally {
			Directory.Delete(callingDir + "\\Test");
		}

		if (boolFailed) {
			callingDir = "C:\\Windows\\Temp";
		}


		//First see if there is a registry value that tells us where to get the dictionary from.
		RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender");


		if (regKey == null) {
			regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("NHunspellTextBoxExtender");
			RegistryKey regKeyLanguage = regKey.CreateSubKey("Languages");
			regKeyLanguage.SetValue("Default", "English");

			//Set the paths for the dic and aff files
			USdic = callingDir + "\\SpellCheck\\en_US.dic";
			USaff = callingDir + "\\SpellCheck\\en_US.aff";

			//Check if the spell check directory already exists.  If not, add it
			if (!Directory.Exists(callingDir + "\\SpellCheck")) {
				Directory.CreateDirectory(callingDir + "\\SpellCheck");
				DirectoryInfo newDirInfo = new DirectoryInfo(callingDir + "\\SpellCheck");
				newDirInfo.Attributes = FileAttributes.Hidden;
			}

			//Check if the spell check files already exist.  If not, add it
			if (!File.Exists(USaff)) {
				try {
					File.WriteAllBytes(USaff, My.Resources.en_US);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.aff file!" + Constants.vbNewLine + ex.Message);
				}
			}

			if (!File.Exists(USdic)) {
				try {
					File.WriteAllBytes(USdic, My.Resources.en_US_dic);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.dic file!" + Constants.vbNewLine + ex.Message);
				}
			}

			string[] paths = {
				USaff,
				USdic
			};
			regKeyLanguage.SetValue("English", paths, RegistryValueKind.MultiString);

			string[] languages = { "English" };
			regKeyLanguage.SetValue("LanguageList", languages, RegistryValueKind.MultiString);

			regKeyLanguage.Close();
			regKeyLanguage.Dispose();

			_Language = "English";
		} else {
			//Get the default language
			RegistryKey regKeyLanguage = regKey.OpenSubKey("Languages", true);

			string defaultLanguage = regKeyLanguage.GetValue("Default");

			string[] paths = null;

			paths = regKeyLanguage.GetValue(defaultLanguage) as string[];

			_Language = defaultLanguage;

			if (regKeyLanguage.GetValue(defaultLanguage) == null) {
				//Check if English is there and use it...otherwise, check if another language is available
				paths = regKeyLanguage.GetValue("English") as string[];

				if (regKeyLanguage.GetValue("English") == null) {
					//Set the paths for the dic and aff files
					USdic = callingDir + "\\SpellCheck\\en_US.dic";
					USaff = callingDir + "\\SpellCheck\\en_US.aff";

					//Check if the spell check directory already exists.  If not, add it
					if (!Directory.Exists(callingDir + "\\SpellCheck")) {
						Directory.CreateDirectory(callingDir + "\\SpellCheck");
						DirectoryInfo newDirInfo = new DirectoryInfo(callingDir + "\\SpellCheck");
						newDirInfo.Attributes = FileAttributes.Hidden;
					}

					//Check if the spell check files already exist.  If not, add it
					if (!File.Exists(USaff)) {
						try {
							File.WriteAllBytes(USaff, My.Resources.en_US);
						} catch (Exception ex) {
							MessageBox.Show("Error writing en_US.aff file!" + Constants.vbNewLine + ex.Message);
						}
					}

					if (!File.Exists(USdic)) {
						try {
							File.WriteAllBytes(USdic, My.Resources.en_US_dic);
						} catch (Exception ex) {
							MessageBox.Show("Error writing en_US.dic file!" + Constants.vbNewLine + ex.Message);
						}
					}

					paths =new string[] {
						USaff,
						USdic
					};
					_Language = "English";
				} else {
					if (!File.Exists(paths(0))) {
						USaff = callingDir + "\\SpellCheck\\en_US.aff";

						try {
							File.WriteAllBytes(USaff, My.Resources.en_US);
						} catch (Exception ex) {
							MessageBox.Show("Error writing en_US.aff file!" + Constants.vbNewLine + ex.Message);
						}

						paths(0) = USaff;
					}

					if (!File.Exists(paths(1))) {
						USdic = callingDir + "\\SpellCheck\\en_US.dic";

						try {
							File.WriteAllBytes(USdic, My.Resources.en_US_dic);
						} catch (Exception ex) {
							MessageBox.Show("Error writing en_US.dic file!" + Constants.vbNewLine + ex.Message);
						}

						paths(1) = USdic;
					}
				}

				_Language = "English";
			}
			USaff = paths(0);
			USdic = paths(1);

			//check if these files exist
			if (!File.Exists(USaff)) {
				USaff = callingDir + "\\SpellCheck\\en_US.aff";

				try {
					File.WriteAllBytes(USaff, My.Resources.en_US);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.aff file!" + Constants.vbNewLine + ex.Message);
				}

				paths(0) = USaff;

				regKeyLanguage.SetValue(_Language, paths, RegistryValueKind.MultiString);
			}
			if (!File.Exists(USdic)) {
				USdic = callingDir + "\\SpellCheck\\en_US.dic";

				try {
					File.WriteAllBytes(USdic, My.Resources.en_US_dic);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.dic file!" + Constants.vbNewLine + ex.Message);
				}

				paths(1) = USdic;

				regKeyLanguage.SetValue(_Language, paths, RegistryValueKind.MultiString);
			}

			regKeyLanguage.Flush();
			regKeyLanguage.Close();
			regKeyLanguage.Dispose();
		}

		regKey.Close();
		regKey.Dispose();
		CreateNewHunspell:

		//Create the new hunspell
		try {
			//Hunspell.NativeDllPath = "D:\Temp"

			myNHunspell = new Hunspell(USaff, USdic);
		} catch (Exception ex) {
			if (ex is System.DllNotFoundException) {
				//Get where the dll is supposed to be
				string DLLpath = Strings.Trim(Strings.Mid(ex.Message, Strings.InStr(ex.Message, "DLL not found:") + 14));
				string DLLName = Path.GetFileName(DLLpath);

				//Find out which DLL is missing
				if (DLLName == "Hunspellx64.dll") {
					//Copy the dll to the directory
					try {
						File.WriteAllBytes(DLLpath, My.Resources.Hunspellx64);
					} catch (Exception ex2) {
						MessageBox.Show("Error writing Hunspellx64.dll" + Constants.vbNewLine + ex2.Message);
					}

					//Try again
					goto CreateNewHunspell;
				//x86 dll
				} else if (DLLName == "Hunspellx86.dll") {
					//Copy the dll to the directory
					try {
						File.WriteAllBytes(DLLpath, My.Resources.Hunspellx86);
					} catch (Exception ex3) {
						MessageBox.Show("Error writing Hunspellx86.dll" + Constants.vbNewLine + ex3.Message);
					}

					//Try again
					goto CreateNewHunspell;
				} else if (DLLName == "NHunspell.dll") {
					try {
						File.WriteAllBytes(DLLpath, My.Resources.NHunspell);
					} catch (Exception ex4) {
						MessageBox.Show("Error writing NHunspell.dll" + Constants.vbNewLine + ex4.Message);
					}
				} else {
					MessageBox.Show(ex.Message + ex.StackTrace);
				}
			} else {
				MessageBox.Show("SpellChecker cannot be created." + Constants.vbNewLine + "Spell checking will be disabled." + Constants.vbNewLine + Constants.vbNewLine + ex.Message + ex.StackTrace);
				myNHunspell = null;
			}
		}

		//myNHunspell = FromAssembly()

		//See if there are any words to add
		if (File.Exists(callingDir + "\\SpellCheck\\" + _Language + "AddedWords.dat")) {
			using (StreamReader r = new StreamReader(callingDir + "\\SpellCheck\\" + _Language + "AddedWords.dat")) {
				while (!r.EndOfStream) {
					myNHunspell.Add(Strings.Trim(Strings.Replace(r.ReadLine, Constants.vbNewLine, "")));
				}
				r.Close();
			}
		}

		//Set up Hashtables
		controlEnabled = new Hashtable();
		mySpellCheckers = new Hashtable();
		myCustomPaintingTextBoxes = new Hashtable();
		myContextMenus = new Hashtable();

		//Set the initial properties
		myNumOfSuggestions = 5;
		_SpellAsYouType = true;
		_shortcutKey = Shortcut.F7;
		myControls = new Control[-1 + 1];
	}


	//Private Shared Function FromAssembly() As Object
	//Dim a As Assembly = Assembly.Load(My.Resources.NHunspell)
	//Dim type_l As Type = a.GetType("NHunspell.Hunspell")
	//Dim types(1) As Type
	//types(0) = GetType(String)
	//types(1) = GetType(String)
	//Dim ctor As ConstructorInfo = type_l.GetConstructor(types)

	//Dim USdic, USaff As String
	//Dim callingDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location)

	//'Set the paths for the dic and aff files
	//USdic = callingDir & "\SpellCheck\en_US.dic"
	//USaff = callingDir & "\SpellCheck\en_US.aff"

	//'Check if the spell check directory already exists.  If not, add it
	//If Not Directory.Exists(callingDir & "\SpellCheck") Then
	//Directory.CreateDirectory(callingDir & "\SpellCheck")
	//Dim newDirInfo As New DirectoryInfo(callingDir & "\SpellCheck")
	//newDirInfo.Attributes = FileAttributes.Hidden
	//End If

	//'Check if the spell check files already exist.  If not, add it
	//If Not File.Exists(USaff) Then
	//Try
	//File.WriteAllBytes(USaff, My.Resources.en_US)
	//Catch ex As Exception
	//MessageBox.Show("Error writing en_US.aff file!" & vbNewLine & ex.Message)
	//End Try
	//End If

	//If Not File.Exists(USdic) Then
	//Try
	//File.WriteAllBytes(USdic, My.Resources.en_US_dic)
	//Catch ex As Exception
	//MessageBox.Show("Error writing en_US.dic file!" & vbNewLine & ex.Message)
	//End Try
	//End If

	//Dim params(1) As Object
	//params(0) = USaff
	//params(1) = USdic

	//Dim result As Object = Nothing

	//CreateNewHunspell:
	//Try
	//result = ctor.Invoke(params)
	//Catch ex As Exception
	//If TypeOf ex.InnerException Is System.DllNotFoundException Then
	//'Get where the dll is supposed to be
	//Dim DLLpath As String = Trim(Strings.Mid(ex.InnerException.Message, InStr(ex.InnerException.Message, "DLL not found:") + 14))
	//Dim DLLName As String = Path.GetFileName(DLLpath)

	//'Find out which DLL is missing
	//If DLLName = "Hunspellx64.dll" Then
	//'Copy the dll to the directory
	//Try
	//File.WriteAllBytes(DLLpath, My.Resources.Hunspellx64)
	//Catch ex2 As Exception
	//MessageBox.Show("Error writing Hunspellx64.dll" & vbNewLine & ex2.Message)
	//End Try

	//'Try again
	//GoTo CreateNewHunspell
	//ElseIf DLLName = "Hunspellx86.dll" Then 'x86 dll
	//'Copy the dll to the directory
	//Try
	//File.WriteAllBytes(DLLpath, My.Resources.Hunspellx86)
	//Catch ex3 As Exception
	//MessageBox.Show("Error writing Hunspellx86.dll" & vbNewLine & ex3.Message)
	//End Try

	//'Try again
	//GoTo CreateNewHunspell
	//'ElseIf DLLName = "NHunspell.dll" Then
	//'Try
	//'File.WriteAllBytes(DLLpath, My.Resources.NHunspell)
	//'Catch ex4 As Exception
	//'MessageBox.Show("Error writing NHunspell.dll" & vbNewLine & ex4.Message)
	//'End Try
	//Else
	//MessageBox.Show(ex.Message & ex.StackTrace)
	//End If
	//Else
	//MessageBox.Show("SpellChecker cannot be created." & vbNewLine & "Spell checking will be disabled." & _
	//vbNewLine & vbNewLine & ex.Message & ex.StackTrace)
	//Return Nothing
	//End If
	//End Try

	//Return result
	//End Function
	public static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
	{
		for (i = 0; i <= AppDomain.CurrentDomain.GetAssemblies.Count - 1; i++) {
			MessageBox.Show(args.Name + Constants.vbNewLine + AppDomain.CurrentDomain.GetAssemblies(i).GetName().Name);

			if (AppDomain.CurrentDomain.GetAssemblies(i).GetName().Name == args.Name) {
				return AppDomain.CurrentDomain.GetAssemblies(i);
			}
		}

		return null;
	}

	#endregion




	#region "Enable/Disable"



	/// <summary>
	/// Allows this class the be enabled programatically
	/// </summary>
	/// <param name="TextBoxesToEnable">
	/// Allows the programmer to add as many TextBoxBases as they want at once.
	/// </param>
	/// <remarks>
	/// Examples:
	/// EnableTextBoxBase(TextBox1)
	/// EnableTextBoxBase(RichTextBox1, RichTextBox2, TextBox1)
	/// </remarks>
	//ByRef TextBoxToEnable As TextBoxBase)
	public void EnableTextBoxBase(params TextBoxBase[] TextBoxesToEnable)
	{
		for (c = 0; c <= Information.UBound(TextBoxesToEnable); c++) {

			if ((TextBoxesToEnable(c)) is TextBoxBase) {
				TextBoxBase TextBoxToEnable = TextBoxesToEnable(c);

				//Set the hashtables
				if (controlEnabled(TextBoxToEnable) == null) {
					controlEnabled.Add(TextBoxToEnable, true);
					mySpellCheckers.Add(TextBoxToEnable, new SpellCheckControl(myNHunspell));
					myCustomPaintingTextBoxes.Add(TextBoxToEnable, new CustomPaintTextBox((TextBoxBase)TextBoxToEnable, (SpellCheckControl)mySpellCheckers(TextBoxToEnable), this));
					((CustomPaintTextBox)myCustomPaintingTextBoxes(TextBoxToEnable)).CustomPaintComplete += OnCustomPaintComplete;

					if (TextBoxToEnable.ContextMenuStrip == null) {
						TextBoxToEnable.ContextMenuStrip = new ContextMenuStrip();
					}
					TextBoxToEnable.ContextMenuStrip.Tag = TextBoxToEnable.Name;

					myContextMenus.Add(TextBoxToEnable, TextBoxToEnable.ContextMenuStrip);

					bool boolFound = false;
					for (i = 0; i <= Information.UBound(myControls); i++) {
						if (myControls(i).Name == TextBoxToEnable.Name) {
							boolFound = true;
							break; // TODO: might not be correct. Was : Exit For
						}
					}

					if (!boolFound) {
						Array.Resize(ref myControls, Information.UBound(myControls) + 2);
						myControls(Information.UBound(myControls)) = TextBoxToEnable;
					}

					//Set up all of the handlers
					TextBoxToEnable.TextChanged += TextBox_TextChanged;
					TextBoxToEnable.KeyDown += TextBox_KeyDown;
					TextBoxToEnable.KeyPress += TextBox_KeyPress;
					TextBoxToEnable.MouseMove += TextBox_MouseMove;
					TextBoxToEnable.ContextMenuStrip.Opening += ContextMenu_Opening;
					TextBoxToEnable.ContextMenuStrip.Closed += ContextMenu_Closed;

					((SpellCheckControl)mySpellCheckers(TextBoxToEnable)).SetText(TextBoxToEnable.Text);
				} else {
					controlEnabled(TextBoxToEnable) = true;
				}

				TextBoxToEnable.Invalidate();
			}
		}
	}



	/// <summary>
	/// Allows this class to be disabled programatically
	/// </summary>
	/// <param name="TextBoxToDisable"></param>
	/// <remarks></remarks>
	public void DisableTextBoxBase(ref TextBoxBase TextBoxToDisable)
	{
		controlEnabled(TextBoxToDisable) = false;
		TextBoxToDisable.Invalidate();
	}




	public bool IsEnabled(ref TextBoxBase TextBoxBaseToCheck)
	{
		return controlEnabled(TextBoxBaseToCheck);
	}




	#endregion




	#region "Provided Properties"


	#region "Enabled"


	/// <summary>
	/// This will return whether the spell checker is enabled for the requested textbox.
	/// The default value is false, otherwise, the SetSpellCheckEnabled will never be called
	/// and there will be no way to set up the event handlers
	/// </summary>
	/// <param name="extendee">The control being tested</param>
	/// <returns>A boolean representing whether spell check is enabled</returns>
	/// <remarks></remarks>
	[Category("SpellCheck"), DefaultValue(false)]
	public bool GetSpellCheckEnabled(Control extendee)
	{
		if (controlEnabled(extendee) == null) {
			controlEnabled.Add(extendee, false);
		}

		return controlEnabled(extendee);
	}


	/// <summary>
	/// Sets whether the spellcheck is enabled.  This is only called if the requested value
	/// is different from the default value (therefore if the spell check is enabled).
	/// Once we set the enabled property, we then set up the event handlers
	/// 
	/// In case the spellchecker is disabled programatically, we include the options for 
	/// removing the event handlers as well.
	/// </summary>
	/// <param name="extendee">The control associated with the enabled request</param>
	/// <param name="Input">A boolean representing whether spell check is enabled</param>
	/// <remarks></remarks>
	public void SetSpellCheckEnabled(Control extendee, bool Input)
	{
		if (myNHunspell == null) {
			controlEnabled.Add(extendee, false);
			return;
		}

		//Set the hashtables
		if (controlEnabled(extendee) == null) {
			controlEnabled.Add(extendee, (Input & ((myNHunspell != null))));

			mySpellCheckers.Add(extendee, new SpellCheckControl(myNHunspell));
			myCustomPaintingTextBoxes.Add(extendee, new CustomPaintTextBox((TextBoxBase)extendee, (SpellCheckControl)mySpellCheckers(extendee), this));
			((CustomPaintTextBox)myCustomPaintingTextBoxes(extendee)).CustomPaintComplete += OnCustomPaintComplete;

			if ((((TextBoxBase)extendee).ContextMenuStrip) == null) {
				((TextBoxBase)extendee).ContextMenuStrip = new ContextMenuStrip();
			}

			((TextBoxBase)extendee).ContextMenuStrip.Opening += ContextMenu_Opening;
			((TextBoxBase)extendee).ContextMenuStrip.Closed += ContextMenu_Closed;

			myContextMenus.Add(extendee, ((TextBoxBase)extendee).ContextMenuStrip);

			Array.Resize(ref myControls, Information.UBound(myControls) + 2);
			myControls(Information.UBound(myControls)) = extendee;
		} else {
			controlEnabled(extendee) = (Input & ((myNHunspell != null)));
		}

		//Get the handlers
		if (Input == true & (myNHunspell != null)) {
			((TextBoxBase)extendee).TextChanged += TextBox_TextChanged;
			((TextBoxBase)extendee).KeyDown += TextBox_KeyDown;
			((TextBoxBase)extendee).KeyPress += TextBox_KeyPress;
			((TextBoxBase)extendee).MouseMove += TextBox_MouseMove;
		} else {
			((TextBoxBase)extendee).TextChanged -= TextBox_TextChanged;
			((TextBoxBase)extendee).KeyDown -= TextBox_KeyDown;
			((TextBoxBase)extendee).KeyPress -= TextBox_KeyPress;
			((TextBoxBase)extendee).MouseMove -= TextBox_MouseMove;
		}

	}

	#endregion


	#endregion




	#region "Properties"



	public enum SuggestionNumbers
	{
		One,
		Two,
		Three,
		Four,
		Five
	}



	[Description("Sets the key that will bring up the full spell check dialog"), Browsable(true)]
	public Shortcut ShortcutKey {
		get { return _shortcutKey; }
		set { _shortcutKey = value; }
	}



	[Description("Sets the number of suggestions that will be returned on a right-click"), Browsable(true), DefaultValue(SuggestionNumbers.Five)]
	public SuggestionNumbers NumberofSuggestions {
		get {
			switch (myNumOfSuggestions) {
				case 1:
					return SuggestionNumbers.One;
				case 2:
					return SuggestionNumbers.Two;
				case 3:
					return SuggestionNumbers.Three;
				case 4:
					return SuggestionNumbers.Four;
				default:
					return SuggestionNumbers.Five;
			}
		}
		set {
			switch (value) {
				case SuggestionNumbers.One:
					myNumOfSuggestions = 1;
					break;
				case SuggestionNumbers.Two:
					myNumOfSuggestions = 2;
					break;
				case SuggestionNumbers.Three:
					myNumOfSuggestions = 3;
					break;
				case SuggestionNumbers.Four:
					myNumOfSuggestions = 4;
					break;
				case SuggestionNumbers.Five:
					myNumOfSuggestions = 5;
					break;
			}
		}
	}



	[Description("Enables or disables spell checking as the user types.")]
	public bool SpellAsYouType {
		get { return _SpellAsYouType; }
		set {
			_SpellAsYouType = value;

			foreach (TextBoxBase txtBox in myControls) {
				txtBox.Invalidate();
			}
		}
	}


	public event LanguageChangedEventHandler LanguageChanged;
	public delegate void LanguageChangedEventHandler(object sender, string NewLanguage);


	[Description("Selects the language for spell checking. (Will only change the language on the developer's computer)")]
	[EditorAttribute(typeof(LanguageEditor), typeof(Drawing.Design.UITypeEditor))]
	public string Language {
		get { return _Language; }
		set {
			if (MaintainUserChoice & !this.DesignMode)
				return;

			bool boolFound = false;

			foreach (void lang_loopVariable in GetAvailableLanguages()) {
				lang = lang_loopVariable;
				if (lang == value)
					boolFound = true;
			}

			if (!boolFound)
				return;

			bool raiseChangeEvent = (value != _Language);

			_Language = value;

			if (raiseChangeEvent) {
				if (LanguageChanged != null) {
					LanguageChanged(this, value);
				}
			}
		}
	}


	[Description("If selected to false, whenever the program starts up, it will default to the designer selection" + Constants.vbNewLine + "If selected to true, will disable any direct calls to change the language." + "To change the language, use the SelectLanguage method")]
	public bool MaintainUserChoice { get; set; }

	#endregion




	#region "Events"



	#region "CustomEvents"
	public event CustomPaintCompleteEventHandler CustomPaintComplete;
	public delegate void CustomPaintCompleteEventHandler(TextBoxBase sender, long Milliseconds);

	public void OnCustomPaintComplete(TextBoxBase sender, long Milliseconds)
	{
		if (CustomPaintComplete != null) {
			CustomPaintComplete(sender, Milliseconds);
		}
	}
	#endregion



	#region "TextBox Events"


	/// <summary>
	/// When the text changes, check all of the words in the text box.  If there is a spelling error
	/// then inform the user of that error.
	/// </summary>
	/// <param name="sender">The textbox that is being typed in</param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void TextBox_TextChanged(object sender, EventArgs e)
	{
		//If Not _SpellAsYouType Then Return

		var _with3 = (SpellCheckControl)mySpellCheckers(sender);
		_with3.SetText(((TextBoxBase)sender).Text);

		((TextBoxBase)sender).Invalidate();
	}



	/// <summary>
	/// Handles the shortcuts (have to use KeyDown because KeyPress doesn't capture the function keys or delete)
	/// </summary>
	/// <param name="sender">The TextBox being typed in</param>
	/// <param name="e">The key being pushed down</param>
	/// <remarks></remarks>
	private void TextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		if (ShortcutKey == e.KeyCode) {
			//If Not _SpellAsYouType Then
			//CType(mySpellCheckers(sender), SpellCheckControl).SetText(CType(sender, TextBoxBase).Text)
			//End If

			if (!((SpellCheckControl)mySpellCheckers(sender)).HasSpellingErrors)
				return;

			RunFullSpellChecker(ref (TextBoxBase)sender);
			return;
		}

		//If Not _SpellAsYouType Then Return

		if (e.Control | e.Alt) {
			controlPressed = true;
		} else {
			controlPressed = false;
		}

		if (e.KeyCode == Keys.Delete) {
			for (i = 1; i <= ((TextBoxBase)sender).SelectionLength + 1; i++) {
				((SpellCheckControl)mySpellCheckers(sender)).RemoveText(((TextBoxBase)sender).SelectionStart);
			}

			var _with4 = (CustomPaintTextBox)myCustomPaintingTextBoxes(sender);
			_with4.ForcePaint();
		}
	}



	/// <summary>
	/// Handles the backspace and adding characters
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void TextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
	{
		//If Not _SpellAsYouType Then Return

		var _with5 = (SpellCheckControl)mySpellCheckers(sender);
		if (controlPressed)
			return;

		if (((TextBoxBase)sender).SelectionLength > 0) {
			for (i = 1; i <= ((TextBoxBase)sender).SelectionLength; i++) {
				_with5.RemoveText(((TextBoxBase)sender).SelectionStart);
			}
		}

		if (Strings.Asc(e.KeyChar) == Keys.Back) {
			_with5.RemoveText(((TextBoxBase)sender).SelectionStart - 1);
		} else {
			_with5.AddText(e.KeyChar, ((TextBoxBase)sender).SelectionStart);
		}

		var _with6 = (CustomPaintTextBox)myCustomPaintingTextBoxes(sender);
		_with6.ForcePaint();
	}


	#endregion



	#region "Mouse Events"


	private Point currentMouseLocation;

	private TextBoxBase currentTextBox;

	private void TextBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		currentMouseLocation = e.Location;
		currentTextBox = (TextBoxBase)sender;
	}



	#endregion



	private void MyLanguageChanged(object sender, string NewLanguage)
	{
		((NHunspellTextBoxExtender)sender).SetLanguage(NewLanguage);
	}
	private ToolStripMenuItem withEventsField_Suggestion1;


	#endregion




	#region "ContextMenu"

	internal ToolStripMenuItem Suggestion1 {
		get { return withEventsField_Suggestion1; }
		set {
			if (withEventsField_Suggestion1 != null) {
				withEventsField_Suggestion1.Click -= ContextMenuItem_Click;
			}
			withEventsField_Suggestion1 = value;
			if (withEventsField_Suggestion1 != null) {
				withEventsField_Suggestion1.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_Suggestion2;
	internal ToolStripMenuItem Suggestion2 {
		get { return withEventsField_Suggestion2; }
		set {
			if (withEventsField_Suggestion2 != null) {
				withEventsField_Suggestion2.Click -= ContextMenuItem_Click;
			}
			withEventsField_Suggestion2 = value;
			if (withEventsField_Suggestion2 != null) {
				withEventsField_Suggestion2.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_Suggestion3;
	internal ToolStripMenuItem Suggestion3 {
		get { return withEventsField_Suggestion3; }
		set {
			if (withEventsField_Suggestion3 != null) {
				withEventsField_Suggestion3.Click -= ContextMenuItem_Click;
			}
			withEventsField_Suggestion3 = value;
			if (withEventsField_Suggestion3 != null) {
				withEventsField_Suggestion3.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_Suggestion4;
	internal ToolStripMenuItem Suggestion4 {
		get { return withEventsField_Suggestion4; }
		set {
			if (withEventsField_Suggestion4 != null) {
				withEventsField_Suggestion4.Click -= ContextMenuItem_Click;
			}
			withEventsField_Suggestion4 = value;
			if (withEventsField_Suggestion4 != null) {
				withEventsField_Suggestion4.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_Suggestion5;
	internal ToolStripMenuItem Suggestion5 {
		get { return withEventsField_Suggestion5; }
		set {
			if (withEventsField_Suggestion5 != null) {
				withEventsField_Suggestion5.Click -= ContextMenuItem_Click;
			}
			withEventsField_Suggestion5 = value;
			if (withEventsField_Suggestion5 != null) {
				withEventsField_Suggestion5.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_AddWord;
	internal ToolStripMenuItem AddWord {
		get { return withEventsField_AddWord; }
		set {
			if (withEventsField_AddWord != null) {
				withEventsField_AddWord.Click -= ContextMenuItem_Click;
			}
			withEventsField_AddWord = value;
			if (withEventsField_AddWord != null) {
				withEventsField_AddWord.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_IgnoreWord;
	internal ToolStripMenuItem IgnoreWord {
		get { return withEventsField_IgnoreWord; }
		set {
			if (withEventsField_IgnoreWord != null) {
				withEventsField_IgnoreWord.Click -= ContextMenuItem_Click;
			}
			withEventsField_IgnoreWord = value;
			if (withEventsField_IgnoreWord != null) {
				withEventsField_IgnoreWord.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_IgnoreAll;
	internal ToolStripMenuItem IgnoreAll {
		get { return withEventsField_IgnoreAll; }
		set {
			if (withEventsField_IgnoreAll != null) {
				withEventsField_IgnoreAll.Click -= ContextMenuItem_Click;
			}
			withEventsField_IgnoreAll = value;
			if (withEventsField_IgnoreAll != null) {
				withEventsField_IgnoreAll.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripMenuItem withEventsField_Spelling;
	internal ToolStripMenuItem Spelling {
		get { return withEventsField_Spelling; }
		set {
			if (withEventsField_Spelling != null) {
				withEventsField_Spelling.Click -= ContextMenuItem_Click;
			}
			withEventsField_Spelling = value;
			if (withEventsField_Spelling != null) {
				withEventsField_Spelling.Click += ContextMenuItem_Click;
			}
		}
	}
	private ToolStripSeparator Separator1;
	private ToolStripSeparator Separator2;
	private Point originalMouseLocation;

	private TextBoxBase ownerTextBox;
	/// <summary>
	/// Controls all of the contextmenuitem clicks
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	public void ContextMenuItem_Click(System.Object sender, System.EventArgs e)
	{
		//Get which button was clicked then perform its action.  Afterwards, remove all buttons

		switch (((ToolStripMenuItem)sender).Name) {
			//If it's a Spell1 through Spell5, then it's a suggestion item
			case "qwr3Spell1":
				ReplaceWord(Suggestion1.Tag, Suggestion1.Text);
				break;
			case "qwr3Spell2":
				ReplaceWord(Suggestion2.Tag, Suggestion2.Text);
				break;
			case "qwr3Spell3":
				ReplaceWord(Suggestion3.Tag, Suggestion3.Text);
				break;
			case "qwr3Spell4":
				ReplaceWord(Suggestion4.Tag, Suggestion4.Text);
				break;
			case "qwr3Spell5":
				ReplaceWord(Suggestion5.Tag, Suggestion5.Text);
				break;
			case "qwr3Add":
				AddWordToDictionary(AddWord.Tag);
				break;
			case "qwr3Ignore":
				IgnoreSelectedWord(IgnoreWord.Tag, ((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).Left, ((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).Top);
				break;
			case "qwr3IgnoreAll":
				IgnoreAllWord(IgnoreAll.Tag);
				break;
			case "qwr3Spelling":
				TextBoxBase ownerTextBoxBase = null;

				for (i = 0; i <= Information.UBound(myControls); i++) {
					if (myControls(i).Name == Spelling.Tag) {
						ownerTextBoxBase = myControls(i);
					}
				}


				RunFullSpellChecker(ref ownerTextBoxBase);
				break;
		}
	}


	/// <summary>
	/// If it was closed by not clicking on an item, then we remove the items and reset them
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void ContextMenu_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e)
	{
		if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
			return;

		var _with7 = (ContextMenuStrip)sender;
		if ((Suggestion1 != null)) {
			_with7.Items.Remove(Suggestion1);
			Suggestion1 = null;
		}
		if ((Suggestion2 != null)) {
			_with7.Items.Remove(Suggestion2);
			Suggestion2 = null;
		}
		if ((Suggestion3 != null)) {
			_with7.Items.Remove(Suggestion3);
			Suggestion3 = null;
		}
		if ((Suggestion4 != null)) {
			_with7.Items.Remove(Suggestion4);
			Suggestion4 = null;
		}
		if ((Suggestion5 != null)) {
			_with7.Items.Remove(Suggestion5);
			Suggestion5 = null;
		}
		if ((AddWord != null)) {
			_with7.Items.Remove(AddWord);
			AddWord = null;
		}
		if ((IgnoreWord != null)) {
			_with7.Items.Remove(IgnoreWord);
			IgnoreWord = null;
		}
		if ((IgnoreAll != null)) {
			_with7.Items.Remove(IgnoreAll);
			IgnoreAll = null;
		}
		if ((Spelling != null)) {
			_with7.Items.Remove(Spelling);
			Spelling = null;
		}
		if ((Separator1 != null)) {
			_with7.Items.Remove(Separator1);
			Separator1 = null;
		}
		if ((Separator2 != null)) {
			_with7.Items.Remove(Separator2);
			Separator2 = null;
		}
	}


	/// <summary>
	/// If we are spell checking as the user types, add items to the textbox's context menu
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void ContextMenu_Opening(System.Object sender, System.ComponentModel.CancelEventArgs e)
	{
		if (!_SpellAsYouType)
			return;

		//MessageBox.Show(CType(sender, ContextMenuStrip).MouseButtons.ToString)

		e.Cancel = true;

		//Make sure that none of the items are still in the menu
		var _with8 = (ContextMenuStrip)sender;
		if ((Suggestion1 != null)) {
			_with8.Items.Remove(Suggestion1);
			Suggestion1 = null;
		}
		if ((Suggestion2 != null)) {
			_with8.Items.Remove(Suggestion2);
			Suggestion2 = null;
		}
		if ((Suggestion3 != null)) {
			_with8.Items.Remove(Suggestion3);
			Suggestion3 = null;
		}
		if ((Suggestion4 != null)) {
			_with8.Items.Remove(Suggestion4);
			Suggestion4 = null;
		}
		if ((Suggestion5 != null)) {
			_with8.Items.Remove(Suggestion5);
			Suggestion5 = null;
		}

		if ((AddWord != null)) {
			_with8.Items.Remove(AddWord);
		}
		if ((IgnoreWord != null)) {
			_with8.Items.Remove(IgnoreWord);
		}
		if ((IgnoreAll != null)) {
			_with8.Items.Remove(IgnoreAll);
		}
		if ((Spelling != null)) {
			_with8.Items.Remove(Spelling);
		}

		if ((Separator1 != null)) {
			_with8.Items.Remove(Separator1);
			Separator1 = null;
		}
		if ((Separator2 != null)) {
			_with8.Items.Remove(Separator2);
			Separator2 = null;
		}

		//See if we're over a spelling error

		//get the textbox
		ownerTextBox = currentTextBox;

		//See if spell-checking is enabled
		if (!controlEnabled(ownerTextBox)) {
			if (((ContextMenuStrip)sender).Items.Count != 0)
				e.Cancel = false;

			return;
		}

		//first see if there are any spelling errors
		if (!((SpellCheckControl)mySpellCheckers(ownerTextBox)).HasSpellingErrors) {
			if (((ContextMenuStrip)sender).Items.Count != 0)
				e.Cancel = false;

			return;
		}

		int charIndex = 0;

		//Get the location of the word based on the starting point for the context menu
		originalMouseLocation = currentMouseLocation;

		charIndex = ownerTextBox.GetCharIndexFromPosition(currentMouseLocation);

		//This will actually still return a character even if not directly over one.  We need to check the character
		//height against the mouseLocation.Y

		if (ownerTextBox is RichTextBox) {
			var _with9 = (RichTextBox)ownerTextBox;
			//We're going to find the font that was the tallest used
			int fontHeight = 0;
			Font selFont = default(Font);
			selFont = _with9.Font;

			long firstCharInLine = 0;
			long lastCharInLine = 0;
			long curCharLine = 0;
			curCharLine = _with9.GetLineFromCharIndex(charIndex);
			firstCharInLine = _with9.GetFirstCharIndexFromLine(curCharLine);
			lastCharInLine = _with9.GetFirstCharIndexFromLine(curCharLine + 1);

			if (lastCharInLine == -1)
				lastCharInLine = ownerTextBox.TextLength;

			RichTextBox tempRTB = new RichTextBox();
			tempRTB.Rtf = _with9.Rtf;

			var _with10 = tempRTB;
			//Now find the tallest font
			for (i = (firstCharInLine + 1); i <= (lastCharInLine + 1); i++) {
				_with10.SelectionStart = i;
				_with10.SelectionLength = 1;
				if (_with10.SelectionFont.Height > fontHeight) {
					fontHeight = _with10.SelectionFont.Height;
					selFont = _with10.SelectionFont;
				}
			}

			//Now find out if the mouse could be over the word or is in blank space
			using (Graphics g = _with9.CreateGraphics) {
				int y = 0;
				y = g.MeasureString(_with9.GetCharFromPosition(currentMouseLocation), selFont).Height + _with9.GetPositionFromCharIndex(charIndex).Y;
				if (currentMouseLocation.Y > y | currentMouseLocation.Y < 0) {
					if (((ContextMenuStrip)sender).Items.Count != 0) {
						e.Cancel = false;
					}

					return;
				}
			}
		} else {
			//We get here if it's a regular textbox.  We can juse use it's font height and see if
			//we're over an item or blank space
			using (Graphics g = ownerTextBox.CreateGraphics) {
				int y = 0;

				long currentIndex = ownerTextBox.GetCharIndexFromPosition(currentMouseLocation);
				long currentLine = ownerTextBox.GetLineFromCharIndex(currentIndex);

				y = ownerTextBox.Font.Height * (currentLine + 1);
				if (currentMouseLocation.Y > y | currentMouseLocation.Y < 0) {
					//We're not actually over an item

					if (((ContextMenuStrip)sender).Items.Count != 0) {
						e.Cancel = false;
					}

					return;
				}
			}
		}

		//If the current charIndex is not part of a misspelled word, just exit
		if (!((SpellCheckControl)mySpellCheckers(ownerTextBox)).IsPartOfSpellingError(charIndex)) {
			e.Cancel = true;
			return;
		}

		//Otherwise...


		//Set up the contextmenu
		if (((ContextMenuStrip)sender).Items.Count > 0) {
			//We're adding to the user created context menu, so add a line
			Separator1 = new ToolStripSeparator();
			((ContextMenuStrip)sender).Items.Add(Separator1);
		}

		//Get the suggestions
		string[] suggestions = null;
		suggestions = new string[-1 + 1];
		string misspelledWord = null;
		var _with11 = (SpellCheckControl)mySpellCheckers(ownerTextBox);
		misspelledWord = _with11.GetMisspelledWordAtPosition(charIndex);
		suggestions = _with11.GetSuggestions(misspelledWord, myNumOfSuggestions);

		//Add the suggestion buttons
		var _with12 = (ContextMenuStrip)sender;
		if (Information.UBound(suggestions) == -1) {
			Suggestion1 = new ToolStripMenuItem("No suggestions found");
			Suggestion1.Name = "qwr3Spell1";
			Suggestion1.ToolTipText = "No suggestions found";
			Suggestion1.Font = new Font(Suggestion1.Font, FontStyle.Italic);
			Suggestion1.Tag = misspelledWord;
			Suggestion1.Enabled = false;
			_with12.Items.Add(Suggestion1);
		} else {
			for (i = 0; i <= Information.UBound(suggestions); i++) {
				EventHandler onClickHandler = new EventHandler(ContextMenuItem_Click);

				//The tag on the suggestion items is the misspelled word
				switch (i) {
					case 0:
						Suggestion1 = new ToolStripMenuItem(suggestions(i));
						Suggestion1.Name = "qwr3Spell1";
						Suggestion1.ToolTipText = suggestions(i);
						Suggestion1.Font = new Font(Suggestion1.Font, FontStyle.Bold);
						Suggestion1.Tag = misspelledWord;
						_with12.Items.Add(Suggestion1);
						break;
					case 1:
						Suggestion2 = new ToolStripMenuItem(suggestions(i));
						Suggestion2.Name = "qwr3Spell2";
						Suggestion2.ToolTipText = suggestions(i);
						Suggestion2.Font = new Font(Suggestion2.Font, FontStyle.Bold);
						Suggestion2.Tag = misspelledWord;
						_with12.Items.Add(Suggestion2);
						break;
					case 2:
						Suggestion3 = new ToolStripMenuItem(suggestions(i));
						Suggestion3.Name = "qwr3Spell3";
						Suggestion3.ToolTipText = suggestions(i);
						Suggestion3.Font = new Font(Suggestion3.Font, FontStyle.Bold);
						Suggestion3.Tag = misspelledWord;
						_with12.Items.Add(Suggestion3);
						break;
					case 3:
						Suggestion4 = new ToolStripMenuItem(suggestions(i));
						Suggestion4.Name = "qwr3Spell4";
						Suggestion4.ToolTipText = suggestions(i);
						Suggestion4.Font = new Font(Suggestion4.Font, FontStyle.Bold);
						Suggestion4.Tag = misspelledWord;
						_with12.Items.Add(Suggestion4);
						break;
					case 4:
						Suggestion5 = new ToolStripMenuItem(suggestions(i));
						Suggestion5.Name = "qwr3Spell5";
						Suggestion5.ToolTipText = suggestions(i);
						Suggestion5.Font = new Font(Suggestion5.Font, FontStyle.Bold);
						Suggestion5.Tag = misspelledWord;
						_with12.Items.Add(Suggestion5);
						break;
				}
			}
		}

		Separator2 = new ToolStripSeparator();
		_with12.Items.Add(Separator2);

		//Now add the add and ignore buttons
		if (AddWord == null) {
			AddWord = new ToolStripMenuItem("Add Word...");
			AddWord.Name = "qwr3Add";
			AddWord.ToolTipText = "Add the word to the dictionary";
		}
		//The addWord Tag is the misspelled word
		AddWord.Tag = misspelledWord;
		_with12.Items.Add(AddWord);

		if (IgnoreWord == null) {
			IgnoreWord = new ToolStripMenuItem("Ignore Once...");
			IgnoreWord.Name = "qwr3Ignore";
			IgnoreWord.ToolTipText = "Ignore this instance of the currently selected word";
		}

		//The ignore once tag is the name of the textbox
		IgnoreWord.Tag = ownerTextBox.Name;
		_with12.Items.Add(IgnoreWord);

		if (IgnoreAll == null) {
			IgnoreAll = new ToolStripMenuItem("Ignore All...");
			IgnoreAll.Name = "qwr3IgnoreAll";
			IgnoreAll.ToolTipText = "Ignore all instances of the currently selected word";
		}
		//The ignore all Tag is the misspelled word
		IgnoreAll.Tag = misspelledWord;
		_with12.Items.Add(IgnoreAll);

		//Now add the spelling button
		if (Spelling == null) {
			Spelling = new ToolStripMenuItem("Run Spell Checker...");
			Spelling.Name = "qwr3Spelling";
			Spelling.ToolTipText = "Runs the full spell checker on this text.";
		}
		//The Spelling tag is the name of the textbox
		Spelling.Tag = ownerTextBox.Name;
		_with12.Items.Add(Spelling);

		e.Cancel = false;
	}


	/// <summary>
	/// Replaces the word that was clicked on with a word from the suggestions
	/// </summary>
	/// <param name="OriginalWord"></param>
	/// <param name="NewWord"></param>
	/// <remarks></remarks>
	private void ReplaceWord(string OriginalWord, string NewWord)
	{
		//Get original scroll position
		dynamic Position = GetScrollPos(ownerTextBox.Handle, SBS_VERT);

		//Disable redraw
		SendMessage(ownerTextBox.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(false)), IntPtr.Zero);

		//Get the location of the original word to replace from the contextmenustrip
		int charIndex = ownerTextBox.GetCharIndexFromPosition(originalMouseLocation);

		CharacterRange[] charRanges = null;

		var _with13 = (SpellCheckControl)mySpellCheckers(ownerTextBox);
		charRanges = _with13.GetSpellingErrorRanges();

		int wordStart = 0;
		wordStart = -1;

		CharacterRange currentRange = default(CharacterRange);

		foreach ( currentRange in charRanges) {
			if (charIndex >= currentRange.First & charIndex <= (currentRange.First + currentRange.Length - 1)) {
				wordStart = currentRange.First;
			}
		}

		if (wordStart == -1)
			return;

		if (ownerTextBox is RichTextBox) {
			double _zoomFactor = ((RichTextBox)ownerTextBox).ZoomFactor;

			RichTextBox tempRTB = new RichTextBox();
			tempRTB.Rtf = ((RichTextBox)ownerTextBox).Rtf;

			tempRTB.SelectionStart = wordStart;
			tempRTB.SelectionLength = OriginalWord.Length;
			tempRTB.SelectedText = NewWord;

			((RichTextBox)ownerTextBox).Rtf = tempRTB.Rtf;
			((RichTextBox)ownerTextBox).ZoomFactor = 1;
			((RichTextBox)ownerTextBox).ZoomFactor = _zoomFactor;
		} else {
			if (wordStart == 0) {
				ownerTextBox.Text = Microsoft.VisualBasic.Strings.Replace(ownerTextBox.Text, OriginalWord, NewWord, 1);
			} else {
				ownerTextBox.Text = Microsoft.VisualBasic.Strings.Left(ownerTextBox.Text, wordStart - 1) + Microsoft.VisualBasic.Strings.Replace(ownerTextBox.Text, OriginalWord, NewWord, wordStart, 1);
			}
		}

		ownerTextBox.SelectionStart = wordStart + NewWord.Length;
		ownerTextBox.SelectionLength = 0;

		//Reset scroll position
		if ((SetScrollPos(ownerTextBox.Handle, SBS_VERT, Position, true) != -1)) {
			PostMessageA(ownerTextBox.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * Position, null);
		}

		//Enable redraw
		SendMessage(ownerTextBox.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(true)), IntPtr.Zero);

		ownerTextBox.Refresh();
	}


	/// <summary>
	/// Adds the word to the dictionary in memory and to a file on disk
	/// </summary>
	/// <param name="WordToAdd"></param>
	/// <remarks></remarks>
	private void AddWordToDictionary(string WordToAdd)
	{
		if (!boolDisableAddWordPrompt) {
			DialogResult result = default(DialogResult);
			result = MyDialog.Show("This will add the word " + Strings.Chr(34) + WordToAdd + Strings.Chr(34) + "." + Constants.vbNewLine + Constants.vbNewLine + "Are you sure?", "Add Word to Dictionary");

			//result = MessageBox.Show("This will add the word " & Chr(34) & WordToAdd & Chr(34) & "." & _
			//vbNewLine & vbNewLine & "Are you sure?", "Add Word to Dictionary", _
			//MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)

			//Check if we're to disable future prompts
			if (result == (DialogResult.Ignore + DialogResult.No)) {
				boolDisableAddWordPrompt = true;
				return;
			} else if (result == (DialogResult.Yes + DialogResult.Ignore)) {
				boolDisableAddWordPrompt = true;
			} else if (result == DialogResult.No) {
				return;
			}
		}

		//Add it to the dictionary in memory
		myNHunspell.Add(WordToAdd);

		//Add it to the file on disk
		string callingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location);

		//Try and write to the directory to see if we can
		bool boolFailed = false;

		try {
			Directory.CreateDirectory(callingDir + "\\Test");
		} catch (Exception ex) {
			boolFailed = true;
		} finally {
			Directory.Delete(callingDir + "\\Test");
		}

		if (boolFailed) {
			callingDir = "C:\\Windows\\Temp";
		}

		if (!Directory.Exists(callingDir + "\\SpellCheck")) {
			Directory.CreateDirectory(callingDir + "\\SpellCheck");
		}

		using (StreamWriter w = new StreamWriter(callingDir + "\\SpellCheck\\" + Language + "AddedWords.dat", true)) {
			w.WriteLine(WordToAdd);
			w.Flush();
			w.Close();
		}

		//Reset all of the textboxes to refresh the spelling
		for (i = 0; i <= Information.UBound(myControls); i++) {
			if (myControls(i) is TextBoxBase) {
				var _with14 = (TextBoxBase)myControls(i);
				//Get original scroll position
				dynamic Position = GetScrollPos(_with14.Handle, SBS_VERT);

				//Disable redraw
				SendMessage(_with14.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(false)), IntPtr.Zero);

				string controlText = null;
				string controlRTF = null;
				controlText = _with14.Text;
				controlRTF = "";

				if (myControls(i) is RichTextBox) {
					controlRTF = ((RichTextBox)myControls(i)).Rtf;
				}

				int selectionStart = 0;
				int selectionLength = 0;
				selectionLength = _with14.SelectionLength;
				selectionStart = _with14.SelectionStart;

				_with14.ResetText();
				_with14.Text = controlText;

				if (!string.IsNullOrEmpty(controlRTF)) {
					((RichTextBox)myControls(i)).Rtf = controlRTF;
				}

				_with14.SelectionStart = selectionStart;
				_with14.SelectionLength = selectionLength;

				//Reset scroll position
				if ((SetScrollPos(_with14.Handle, SBS_VERT, Position, true) != -1)) {
					PostMessageA(_with14.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * Position, null);
				}

				//Enable redraw
				SendMessage(_with14.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(true)), IntPtr.Zero);

				_with14.Refresh();
			}
		}


	}


	/// <summary>
	/// Ignores the selected word once
	/// </summary>
	/// <param name="callingTextBoxName"></param>
	/// <param name="LeftLocation"></param>
	/// <param name="TopLocation"></param>
	/// <remarks></remarks>
	private void IgnoreSelectedWord(string callingTextBoxName, int LeftLocation, int TopLocation)
	{
		//We're only ignoring the currently selected word, so we need to get the range to add it to the spell checker
		TextBoxBase callingTextBox = null;

		for (i = 0; i <= Information.UBound(myControls); i++) {
			if (myControls(i).Name == callingTextBoxName) {
				callingTextBox = myControls(i);
			}
		}

		if (callingTextBox == null)
			return;

		//Get the range of the original word
		int charIndex = callingTextBox.GetCharIndexFromPosition(originalMouseLocation);

		CharacterRange misspelledRange = new CharacterRange(-1, -1);
		CharacterRange currentRange = default(CharacterRange);

		foreach ( currentRange in ((SpellCheckControl)mySpellCheckers(callingTextBox)).GetSpellingErrorRanges) {
			if (currentRange.First <= charIndex & (currentRange.First + currentRange.Length + 1) >= charIndex) {
				misspelledRange = currentRange;
				break; // TODO: might not be correct. Was : Exit For
			}
		}

		if (misspelledRange.First == -1)
			return;

		//Add the range to the ignore words list
		((SpellCheckControl)mySpellCheckers(callingTextBox)).AddRangeToIgnore(misspelledRange);

		//repaint the textbox
		callingTextBox.Invalidate();
	}


	/// <summary>
	/// Ignore all instances of the word.  This will add the word to the dictionary in memory, but not on disk
	/// </summary>
	/// <param name="WordToIgnore"></param>
	/// <remarks></remarks>

	private void IgnoreAllWord(string WordToIgnore)
	{

		//Add the word to the dictionary in memory
		myNHunspell.Add(WordToIgnore);

		//Reset all of the textboxes to refresh the spelling
		for (i = 0; i <= Information.UBound(myControls); i++) {
			if (myControls(i) is TextBoxBase) {
				var _with15 = (TextBoxBase)myControls(i);
				//Get original scroll position
				dynamic Position = GetScrollPos(_with15.Handle, SBS_VERT);

				//Disable redraw
				SendMessage(_with15.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(false)), IntPtr.Zero);

				string controlText = null;
				string controlRTF = null;
				controlText = _with15.Text;
				controlRTF = "";

				if (myControls(i) is RichTextBox) {
					controlRTF = ((RichTextBox)myControls(i)).Rtf;
				}

				int selectionStart = 0;
				int selectionLength = 0;
				selectionLength = _with15.SelectionLength;
				selectionStart = _with15.SelectionStart;

				_with15.ResetText();
				_with15.Text = controlText;

				if (!string.IsNullOrEmpty(controlRTF)) {
					((RichTextBox)myControls(i)).Rtf = controlRTF;
				}

				_with15.SelectionStart = selectionStart;
				_with15.SelectionLength = selectionLength;

				//Reset scroll position
				if ((SetScrollPos(_with15.Handle, SBS_VERT, Position, true) != -1)) {
					PostMessageA(_with15.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * Position, null);
				}

				//Enable redraw
				SendMessage(_with15.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(true)), IntPtr.Zero);

				_with15.Refresh();
			}
		}


	}


	#endregion




	#region "Change Language"
	public string[] GetAvailableLanguages()
	{
		string[] languageList = null;

		RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender\\Languages");
		languageList = regKey.GetValue("LanguageList") as string[];

		regKey.Close();
		regKey.Dispose();

		return languageList;
	}

	public void SetLanguage(string NewLanguage)
	{
		//Check if the language is in the registry
		bool boolFound = false;

		foreach (string st in GetAvailableLanguages()) {
			if (st == NewLanguage)
				boolFound = true;
		}

		if (!boolFound) {
			throw new ArgumentException("LanguageToRemove does not exist!", "LanguageToRemove", new Exception("The language " + NewLanguage + " is not currently loaded."));
		}

		//Open the registry
		RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender\\Languages", true);

		string[] paths = regKey.GetValue(NewLanguage) as string[];

		if (regKey.GetValue(NewLanguage) == null) {
			//If we get there, then the Aff and Dic files don't exist

			string[] languages = regKey.GetValue("LanguageList") as string[];
			string[] newLanguageList = new string[Information.UBound(languages)];
			int count = 0;

			foreach (void Lang_loopVariable in languages) {
				Lang = Lang_loopVariable;
				if (Lang != NewLanguage) {
					newLanguageList(count) = Lang;
					count += 1;
				}
			}

			regKey.SetValue("LanguageList", newLanguageList, Microsoft.Win32.RegistryValueKind.MultiString);
			regKey.DeleteValue(NewLanguage);

			regKey.Close();
			regKey.Dispose();

			throw new FileNotFoundException("Aff and Dic files are missing");
		} else {
			foreach (void path_loopVariable in paths) {
				path = path_loopVariable;
				if (!System.IO.File.Exists(path)) {
					//System.Windows.Forms.MessageBox.Show("Aff and Dic files are missing")

					string[] languages = regKey.GetValue("LanguageList") as string[];
					string[] newLanguageList = new string[Information.UBound(languages)];
					int count = 0;

					foreach (void Lang_loopVariable in languages) {
						Lang = Lang_loopVariable;
						if (Lang != NewLanguage) {
							newLanguageList(count) = Lang;
							count += 1;
						}
					}

					regKey.SetValue("LanguageList", newLanguageList, Microsoft.Win32.RegistryValueKind.MultiString);
					regKey.DeleteValue(NewLanguage);

					regKey.Close();
					regKey.Dispose();

					throw new FileNotFoundException("File not found", path);
				}
			}
		}

		//If we get here, then the paths and language are valid
		//Now try to create the object
		try {
			myNHunspell = new Hunspell(paths(0), paths(1));
		} catch (Exception ex) {
			//MessageBox.Show("Could not create the new Hunspell using the specified language")

			string[] languages = regKey.GetValue("LanguageList") as string[];
			string[] newLanguageList = new string[Information.UBound(languages)];
			int count = 0;

			foreach (void Lang_loopVariable in languages) {
				Lang = Lang_loopVariable;
				if (Lang != NewLanguage) {
					newLanguageList(count) = Lang;
					count += 1;
				}
			}

			regKey.SetValue("LanguageList", newLanguageList, Microsoft.Win32.RegistryValueKind.MultiString);
			regKey.DeleteValue(NewLanguage);

			regKey.Close();
			regKey.Dispose();

			throw ex;
		}

		//See if there are any words to add
		if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location) + "\\SpellCheck\\" + _Language + "AddedWords.dat")) {
			using (StreamReader r = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location) + "\\SpellCheck\\" + _Language + "AddedWords.dat")) {
				while (!r.EndOfStream) {
					myNHunspell.Add(Strings.Trim(Strings.Replace(r.ReadLine, Constants.vbNewLine, "")));
				}
				r.Close();
			}
		}

		_Language = NewLanguage;
		regKey.SetValue("Default", NewLanguage);
		regKey.Close();
		regKey.Dispose();

		//Reset all of the textboxes to refresh the spelling
		for (i = 0; i <= Information.UBound(myControls); i++) {
			if (myControls(i) is TextBoxBase) {
				var _with16 = (TextBoxBase)myControls(i);
				var _with17 = (SpellCheckControl)mySpellCheckers(myControls(i));
				_with17.UpdateHunspell(myNHunspell);


				//Get original scroll position
				dynamic Position = GetScrollPos(_with16.Handle, SBS_VERT);

				//Disable redraw
				SendMessage(_with16.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(false)), IntPtr.Zero);

				string controlText = null;
				string controlRTF = null;
				controlText = _with16.Text;
				controlRTF = "";

				if (myControls(i) is RichTextBox) {
					controlRTF = ((RichTextBox)myControls(i)).Rtf;
				}

				int selectionStart = 0;
				int selectionLength = 0;
				selectionLength = _with16.SelectionLength;
				selectionStart = _with16.SelectionStart;

				_with16.ResetText();
				_with16.Text = controlText;

				if (!string.IsNullOrEmpty(controlRTF)) {
					((RichTextBox)myControls(i)).Rtf = controlRTF;
				}

				_with16.SelectionStart = selectionStart;
				_with16.SelectionLength = selectionLength;

				//Reset scroll position
				if ((SetScrollPos(_with16.Handle, SBS_VERT, Position, true) != -1)) {
					PostMessageA(_with16.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * Position, null);
				}

				//Enable redraw
				SendMessage(_with16.Handle, WM_SETREDRAW, new IntPtr(Convert.ToInt32(true)), IntPtr.Zero);

				_with16.Refresh();
			}
		}
	}

	public bool AddNewLanguage()
	{
		//Open an AddLanguage Form
		AddLanguage newAddLanguage = new AddLanguage();
		newAddLanguage.ShowDialog();

		if (newAddLanguage.Result == DialogResult.Cancel)
			return false;

		//Add the Item to the registry
		Microsoft.Win32.RegistryKey regKey = default(Microsoft.Win32.RegistryKey);
		regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender\\Languages", true);

		string[] languages = regKey.GetValue("LanguageList") as string[];

		bool boolFound = false;
		foreach (void lang_loopVariable in languages) {
			lang = lang_loopVariable;
			if (lang == newAddLanguage.txtName.Text) {
				boolFound = true;
				break; // TODO: might not be correct. Was : Exit For
			}
		}

		if (!boolFound) {
			Array.Resize(ref languages, Information.UBound(languages) + 2);
			languages(Information.UBound(languages)) = newAddLanguage.txtName.Text;

			regKey.SetValue("LanguageList", languages, Microsoft.Win32.RegistryValueKind.MultiString);
		}

		string[] paths = new string[2];

		paths(0) = newAddLanguage.txtAff.Text;
		paths(1) = newAddLanguage.txtDic.Text;

		regKey.SetValue(newAddLanguage.txtName.Text, paths, Microsoft.Win32.RegistryValueKind.MultiString);

		regKey.Close();
		regKey.Dispose();

		return true;
	}

	public void RemoveLanguage(string LanguageToRemove)
	{
		//Check if the language is in the registry
		bool boolFound = false;

		foreach (string st in GetAvailableLanguages()) {
			if (st == LanguageToRemove)
				boolFound = true;
		}

		if (!boolFound) {
			throw new ArgumentException("LanguageToRemove does not exist!", "LanguageToRemove", new Exception("The language " + LanguageToRemove + " is not currently loaded."));
		}

		//Open the registry
		RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender\\Languages", true);

		//Remove the language from the LanguageList
		string[] languages = regKey.GetValue("LanguageList") as string[];

		if (Information.UBound(languages) == -1) {
			throw new Exception("Unable to retrieve Language list from registry");
		}

		string[] newLanguageList = new string[Information.UBound(languages)];
		int count = 0;

		foreach (void lang_loopVariable in languages) {
			lang = lang_loopVariable;
			if (lang != LanguageToRemove) {
				newLanguageList(count) = lang;
				count += 1;
			}
		}

		//Update the registry
		regKey.SetValue("LanguageList", newLanguageList, RegistryValueKind.MultiString);

		if (regKey.GetValue(LanguageToRemove) != null) {
			regKey.DeleteValue(LanguageToRemove);
		}

		//Check if the default was the language removed
		if (regKey.GetValue("Default") == LanguageToRemove) {
			if (regKey.GetValue("English") != null) {
				SetLanguage("English");
			} else {
				if (GetAvailableLanguages().Count == 0) {
					//Default to English
					ResetLanguages();
				} else {
					SetLanguage(GetAvailableLanguages()(0));
				}
			}
		}

		//Check if all of the languagues were removed
		if (newLanguageList.Count == 0) {
			ResetLanguages();
		}

		regKey.Close();
		regKey.Dispose();
	}

	private void ResetLanguages()
	{
		string USdic = null;
		string USaff = null;

		//Get the calling assembly's location
		string callingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location);

		RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender\\Languages", true);

		string[] paths = regKey.GetValue("English") as string[];

		if (regKey.GetValue("English") == null) {
			//Set the paths for the dic and aff files
			USdic = callingDir + "\\SpellCheck\\en_US.dic";
			USaff = callingDir + "\\SpellCheck\\en_US.aff";

			//Check if the spell check directory already exists.  If not, add it
			if (!Directory.Exists(callingDir + "\\SpellCheck")) {
				Directory.CreateDirectory(callingDir + "\\SpellCheck");
				DirectoryInfo newDirInfo = new DirectoryInfo(callingDir + "\\SpellCheck");
				newDirInfo.Attributes = FileAttributes.Hidden;
			}

			//Check if the spell check files already exist.  If not, add it
			if (!File.Exists(USaff)) {
				try {
					File.WriteAllBytes(USaff, My.Resources.en_US);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.aff file!" + Constants.vbNewLine + ex.Message);
				}
			}

			if (!File.Exists(USdic)) {
				try {
					File.WriteAllBytes(USdic, My.Resources.en_US_dic);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.dic file!" + Constants.vbNewLine + ex.Message);
				}
			}

			paths = new string []{
				USaff,
				USdic
			};
		} else {
			if (!File.Exists(paths(0))) {
				USaff = callingDir + "\\SpellCheck\\en_US.aff";

				try {
					File.WriteAllBytes(USaff, My.Resources.en_US);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.aff file!" + Constants.vbNewLine + ex.Message);
				}

				paths(0) = USaff;
			}

			if (!File.Exists(paths(1))) {
				USdic = callingDir + "\\SpellCheck\\en_US.dic";

				try {
					File.WriteAllBytes(USdic, My.Resources.en_US_dic);
				} catch (Exception ex) {
					MessageBox.Show("Error writing en_US.dic file!" + Constants.vbNewLine + ex.Message);
				}

				paths(1) = USdic;
			}
		}

		_Language = "English";
		regKey.SetValue("Default", "English");
		regKey.SetValue("English", paths, RegistryValueKind.MultiString);
		regKey.SetValue("LanguageList",  "English" , RegistryValueKind.MultiString);
		regKey.Close();
		regKey.Dispose();

		SetLanguage("English");

	}

	public void UpdateLanguageFiles(string LanguageToUpdate, string NewAffFileLocation, string NewDicFileLocation, bool OverwriteExistingFiles = false, bool RemoveOlderFiles = false)
	{
		//Check if the language exists
		bool boolFound = false;

		foreach (string st in GetAvailableLanguages()) {
			if (st == LanguageToUpdate)
				boolFound = true;
		}

		if (!boolFound) {
			throw new ArgumentException("LanguageToRemove does not exist!", "LanguageToRemove", new Exception("The language " + LanguageToUpdate + " is not currently loaded and cannot be updated." + Constants.vbNewLine + "If you are trying to add a new language, use teh AddLanguage() method"));
		}

		//Check if the new file paths are valid
		if (!File.Exists(NewAffFileLocation)) {
			throw new FileNotFoundException("File could not be found", NewAffFileLocation);
		}

		if (!File.Exists(NewDicFileLocation)) {
			throw new FileNotFoundException("File could not be found", NewDicFileLocation);
		}

		//Open the registry key
		RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NHunspellTextBoxExtender\\Languages", true);

		string[] paths = null;

		if (OverwriteExistingFiles) {
			//Get the original file locations
			paths = regKey.GetValue(LanguageToUpdate) as string[];

			//If nothing was returned, we can just overwrite the registry
			if (regKey.GetValue(LanguageToUpdate) == null) {
				paths = new string[2];
				paths(0) = NewAffFileLocation;
				paths(1) = NewDicFileLocation;
			} else {
				//If we are overwriting, we need to check that the originals exist
				if (string.IsNullOrEmpty(paths(0))) {
					paths(0) = NewAffFileLocation;
				} else {
					File.Copy(NewAffFileLocation, paths(0));
				}

				if (string.IsNullOrEmpty(paths(1))) {
					paths(1) = NewDicFileLocation;
				} else {
					File.Copy(NewDicFileLocation, paths(1));
				}
			}

			//Save the new paths
			regKey.SetValue(LanguageToUpdate, paths, RegistryValueKind.MultiString);
			regKey.Close();
			regKey.Dispose();
		} else {
			//If we are removing the older files, check if they exist, then delete them
			if (RemoveOlderFiles) {
				paths = regKey.GetValue(LanguageToUpdate) as string[];

				if (!string.IsNullOrEmpty(paths(0))) {
					if (File.Exists(paths(0)))
						File.Delete(paths(0));

					if (File.Exists(paths(1)))
						File.Delete(paths(1));
				}
			}

			//Reset the registry
			paths = new string[2];
			paths(0) = NewAffFileLocation;
			paths(1) = NewDicFileLocation;

			regKey.SetValue(LanguageToUpdate, paths, RegistryValueKind.MultiString);
			regKey.Close();
			regKey.Dispose();
		}
	}
	#endregion




	#region "SpellCheckForm"

	public void RunFullSpellChecker(ref TextBoxBase callingTextBox)
	{
		//first see if there is anything misspelled
		if (!((SpellCheckControl)mySpellCheckers(callingTextBox)).HasSpellingErrors) {
			MessageBox.Show("No spelling errors were detected." + Constants.vbNewLine + Constants.vbNewLine + "Spell check is complete.");
			return;
		}

		//If the textbox is a rich text box, we have to get the selection fonts
		Hashtable fontHashTable = new Hashtable();
		string rtf = "";
		double zoomFactor = 1;

		if (callingTextBox is RichTextBox) {
			rtf = ((RichTextBox)callingTextBox).Rtf;
			zoomFactor = ((RichTextBox)callingTextBox).ZoomFactor;
		}

		//Create the new spell checking form
		SpellCheckForm newSpellCheckForm = new SpellCheckForm(callingTextBox, (SpellCheckControl)mySpellCheckers(callingTextBox), boolDisableAddWordPrompt);

		//Show the form
		newSpellCheckForm.ShowDialog();

		//We get here when the form is closed.
		//We're going to refresh the text in the textbox
		boolDisableAddWordPrompt = newSpellCheckForm.DisableConfirmationPrompt;

		//First make sure that the ignore ranges are not reset
		((SpellCheckControl)mySpellCheckers(callingTextBox)).DontResetIgnoreRanges();

		//Clear the text in the textbox and reset it
		callingTextBox.Clear();

		callingTextBox.Text = newSpellCheckForm.NewText;

		if (callingTextBox is RichTextBox) {
			var _with18 = (RichTextBox)callingTextBox;
			_with18.Rtf = newSpellCheckForm.GetRTF;
			_with18.ZoomFactor = 1;
			_with18.ZoomFactor = zoomFactor;
		}

		callingTextBox.SelectionStart = callingTextBox.TextLength;

		((SpellCheckControl)mySpellCheckers(callingTextBox)).DontResetIgnoreRanges(false);

		callingTextBox.Invalidate();

		//Reset all of the textboxes to refresh the spelling
		for (i = 0; i <= Information.UBound(myControls); i++) {
			if (myControls(i).Name != callingTextBox.Name) {
				if (myControls(i) is TextBoxBase) {
					var _with19 = (TextBoxBase)myControls(i);
					string controlText = null;
					string controlRTF = null;
					controlText = _with19.Text;
					controlRTF = "";

					if (myControls(i) is RichTextBox) {
						controlRTF = ((RichTextBox)myControls(i)).Rtf;
					}

					int selectionStart = 0;
					int selectionLength = 0;
					selectionLength = _with19.SelectionLength;
					selectionStart = _with19.SelectionStart;

					_with19.ResetText();
					_with19.Text = controlText;

					if (!string.IsNullOrEmpty(controlRTF)) {
						((RichTextBox)myControls(i)).Rtf = controlRTF;
					}

					_with19.SelectionStart = selectionStart;
					_with19.SelectionLength = selectionLength;
				}
			}
		}
	}


	#endregion



}



//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
