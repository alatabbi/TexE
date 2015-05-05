
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using NHunspell;

/// <summary>
/// This class holds the text in the textbox, along with which words are spelling errors.
/// This class will also return the requested number of suggestions for misspelled words.
/// </summary>
/// <remarks></remarks>
public class SpellCheckControl
{


    #region "Variables"
    private string FullText;
    private string[,] _Text;

    public Hunspell myNHunspell = null;
    private string[] _spellingErrors;
    private CharacterRange[] _spellingErrorRanges;
    private bool _setTextCalledFirst;
    private CharacterRange[] _ignoreRange;
    #endregion
    private bool _dontResetIgnoreRanges;


    #region "New"
    public SpellCheckControl(ref Hunspell NHunspellObject)
    {
        _Text = new string[2, -1 + 1];
        _spellingErrors = new string[-1 + 1];
        FullText = "";
        myNHunspell = NHunspellObject;
        _setTextCalledFirst = false;
        _ignoreRange = new CharacterRange[-1 + 1];
        _dontResetIgnoreRanges = false;
    }
    #endregion


    #region "Adding or Removing Text"
    /// <summary>
    /// Adds a character directly after the selection start and checks the new word
    /// for spelling errors
    /// </summary>
    /// <param name="Input">The character to be added</param>
    /// <param name="SelectionStart">The position to add the character after</param>
    /// <remarks></remarks>
    public void AddText(string Input, long SelectionStart)
    {
        //Sometimes, the setText gets called first.
        if (_setTextCalledFirst)
        {
            _setTextCalledFirst = false;
            return;
        }

        //If we are allowed to reset the ignore ranges, reset them
        if (!_dontResetIgnoreRanges)
        {
            _ignoreRange = new CharacterRange[-1 + 1];
        }

        //Check if the input is a letter or digit and if not, see if it is splitting up
        //a previous word.  If not, we don't need to do anything further
        if (!char.IsLetterOrDigit(Input))
        {
            //We're going to see if it's being added at the beginning or at the end.
            //If it is, then we don't need to do anything about it.
            //Then we're going to see if the character preceding it or following after
            //it is a non letter or digit.  If it is, then we don't need to go any further

            if (SelectionStart == 0)
                goto SaveFullText;
            if (SelectionStart >= FullText.Length)
                goto SaveFullText;
            if (!char.IsLetterOrDigit(FullText(SelectionStart - 1)) | !char.IsLetterOrDigit(FullText(SelectionStart)))
                goto SaveFullText;
        }



        //Now we need to figure out what the original word was and what the new word will be
        string originalWord = null;
        string newWord = null;
        int endOfWord = 0;
        int beginning = 0;
        bool resetSpellingRanges = false;



        //Start with the case that we're at the beginning of the text
        if (SelectionStart == 0)
        {
            //Now make sure there is a letter or digit currently at the beginning of the original text
            if (!char.IsLetterOrDigit(Strings.Left(FullText, 1)))
            {
                originalWord = "";
                newWord = Input;
            }
            else
            {
                //Find the end of the word that begins the FullText
                endOfWord = FindLastLetterOrDigitFromPosition(SelectionStart);
                originalWord = Strings.Left(FullText, (endOfWord - SelectionStart + 1));
                newWord = Input + originalWord;
            }



        }
        else if (SelectionStart == FullText.Length)
        {
            //Now check if we're at the end
            //Make sure there is a letter or digit at the end of the original text
            if (!char.IsLetterOrDigit(Strings.Right(FullText, 1)))
            {
                originalWord = "";
                newWord = Input;
            }
            else
            {
                beginning = FindFirstLetterOrDigitFromPosition(SelectionStart);
                originalWord = Strings.Right(FullText, (SelectionStart - beginning));
                newWord = originalWord + Input;
            }



            //We're somewhere in the middle of the text
        }
        else
        {
            beginning = FindFirstLetterOrDigitFromPosition(SelectionStart);
            endOfWord = FindLastLetterOrDigitFromPosition(SelectionStart);

            //Example: "This" inserting after 'h'
            //SelectionStart = 2
            //beginning = 0
            //endOfWord = 3
            originalWord = Strings.Mid(FullText, beginning + 1, (endOfWord - beginning + 1));

            //Check if the original word is actually a word
            //If there are not non letters or digits (like two spaces in a row)
            //and someone is putting a letter or digit between them, original word will be
            //one of the non letters or digits
            if (originalWord.Length == 1)
            {
                if (!char.IsLetterOrDigit(originalWord(0)))
                {
                    originalWord = "";
                }
            }

            if (string.IsNullOrEmpty(originalWord))
            {
                newWord = Input;
            }
            else
            {
                newWord = Strings.Mid(FullText, beginning + 1, (SelectionStart - beginning)) + Input + Strings.Mid(FullText, SelectionStart + 1, (endOfWord - SelectionStart + 1));
            }

        }

        //Check if the original word was added already (original word could be a space which would not
        //have been added)

        if (Information.UBound(_Text, 2) > -1)
        {


            if (originalWord.Length > 0)
            {


                for (i = 0; i <= Information.UBound(_Text, 2); i++)
                {

                    if (_Text(0, i) == originalWord)
                    {
                        //Yay, we found it!

                        //Check if the original word is being split up
                        //into two words.  If we get here, and the char is not a digit
                        //or letter, then it is because we've already checked a non-letter
                        //or digit being added at the beginning or end of a word

                        if (!char.IsLetterOrDigit(Input))
                        {

                            //we have two words now
                            string word1 = null;
                            string word2 = null;
                            word1 = Strings.Left(newWord, (SelectionStart - beginning));
                            word2 = Strings.Right(newWord, (endOfWord - SelectionStart + 1));

                            //Replace the original word with the first new word
                            //Check if the original word is in the sentence more than once

                            if (_Text(1, i) > 1)
                            {

                                //If it is, then we're subtracting one instance of the original
                                //word and adding a newword
                                _Text(1, i) = _Text(1, i) - 1;

                                //See if word1 was has already been added
                                bool foundWord1 = false;

                                for (j = 0; j <= Information.UBound(_Text, 2); j++)
                                {
                                    if (_Text(0, j) == word1)
                                    {
                                        foundWord1 = true;
                                        _Text(1, j) = _Text(1, j) + 1;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }

                                if (!foundWord1)
                                {
                                    _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
                                    _Text(0, Information.UBound(_Text, 2)) = word1;
                                    _Text(1, Information.UBound(_Text, 2)) = 1;
                                }


                            }
                            else
                            {
                                //If the original is only in once, we can either replace it, or remove it
                                //First wee need to see if word1 is in already
                                //See if word1 was has already been added
                                bool foundWord1 = false;

                                for (j = 0; j <= Information.UBound(_Text, 2); j++)
                                {
                                    if (_Text(0, j) == word1)
                                    {
                                        foundWord1 = true;
                                        _Text(1, j) = _Text(1, j) + 1;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }

                                if (!foundWord1)
                                {
                                    //If we didn't find it already in the array, just replace the
                                    //original with the new word
                                    _Text(0, i) = word1;
                                    //We did find word1 in the array, so remove the original from it
                                }
                                else
                                {
                                    for (j = i + 1; j <= Information.UBound(_Text, 2); j++)
                                    {
                                        _Text(0, j - 1) = _Text(0, j);
                                        _Text(1, j - 1) = _Text(1, j);
                                    }

                                    _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);
                                }

                                //See if the original word was a spelling error and remove it
                                for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                                {
                                    if (_spellingErrors(j) == originalWord)
                                    {
                                        if (Information.UBound(_spellingErrors) > 0)
                                        {
                                            //If there is more than one entry in _spellingErrors
                                            //then move the entries above this one down one
                                            for (k = (j + 1); k <= Information.UBound(_spellingErrors); k++)
                                            {
                                                _spellingErrors(k - 1) = _spellingErrors(k);
                                            }
                                        }
                                        Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));

                                        resetSpellingRanges = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }

                            //see if word2 has already been added
                            bool foundWord2 = false;
                            for (j = 0; j <= Information.UBound(_Text, 2); j++)
                            {
                                if (_Text(0, j) == word2)
                                {
                                    foundWord2 = true;
                                    _Text(1, j) = _Text(1, j) + 1;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!foundWord2)
                            {
                                _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
                                _Text(0, Information.UBound(_Text, 2)) = word2;
                                _Text(1, Information.UBound(_Text, 2)) = 1;
                            }

                            //Spell check both words
                            bool foundSpellingWord1 = false;
                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (_spellingErrors(j) == word1)
                                {
                                    foundSpellingWord1 = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!myNHunspell.Spell(word1) & !foundSpellingWord1)
                            {
                                Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                                _spellingErrors(Information.UBound(_spellingErrors)) = word1;
                                resetSpellingRanges = true;
                            }
                            else if (!myNHunspell.Spell(word1))
                            {
                                resetSpellingRanges = true;
                            }

                            bool foundSpellingWord2 = false;
                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (_spellingErrors(j) == word2)
                                {
                                    foundSpellingWord2 = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!myNHunspell.Spell(word2) & !foundSpellingWord2)
                            {
                                Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                                _spellingErrors(Information.UBound(_spellingErrors)) = word2;
                                resetSpellingRanges = true;
                            }
                            else if (!myNHunspell.Spell(word2))
                            {
                                resetSpellingRanges = true;
                            }

                            //We've handled everything, we can GoTo SaveFullText
                            goto SaveFullText;
                        }



                        //We get here if the original word is not being split into two
                        //so just replace the original word in the array with the new one
                        if (_Text(1, i) > 1)
                        {
                            //If the original word is in the text more than once, subtract one
                            //instance of it
                            _Text(1, i) = _Text(1, i) - 1;

                            //See if the new word is in the array already
                            bool foundNewWord = false;

                            for (j = 0; j <= Information.UBound(_Text, 2); j++)
                            {
                                if (_Text(0, j) == newWord)
                                {
                                    _Text(1, j) = _Text(1, j) + 1;
                                    foundNewWord = true;
                                }
                            }

                            if (!foundNewWord)
                            {
                                //Add a new word to the array
                                _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
                                _Text(0, Information.UBound(_Text, 2)) = newWord;
                                _Text(1, Information.UBound(_Text, 2)) = 1;

                                //Spell check it
                                bool foundSpellNewWord = false;
                                for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                                {
                                    if (_spellingErrors(j) == newWord)
                                    {
                                        foundSpellNewWord = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }

                                //Check if the new word is a spelling error
                                if (!myNHunspell.Spell(newWord) & !foundSpellNewWord)
                                {
                                    Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                                    _spellingErrors(Information.UBound(_spellingErrors)) = newWord;
                                    resetSpellingRanges = true;
                                }
                                else if (!myNHunspell.Spell(newWord))
                                {
                                    resetSpellingRanges = true;
                                }
                            }

                            //We've handled everything, we can GoTo SaveFullText
                            goto SaveFullText;


                        }
                        else
                        {
                            //We get here if the original word is only in the text once and it's not
                            //being split into two words

                            //Check if the new word is already added
                            bool foundNewWord = false;

                            for (j = 0; j <= Information.UBound(_Text, 2); j++)
                            {
                                if (_Text(0, j) == newWord)
                                {
                                    _Text(1, j) = _Text(1, j) + 1;
                                    foundNewWord = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!foundNewWord)
                            {
                                //If the new word is not in the array, then we can just replace
                                //the original word with it
                                _Text(0, i) = newWord;

                                //Spell check the new word (just a double check)
                                bool foundNewWordinSpell = false;

                                for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                                {
                                    if (_spellingErrors(j) == newWord)
                                    {
                                        foundNewWordinSpell = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }

                                //Check if the new word is a spelling error
                                if (!myNHunspell.Spell(newWord) & !foundNewWordinSpell)
                                {
                                    Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                                    _spellingErrors(Information.UBound(_spellingErrors)) = newWord;
                                    resetSpellingRanges = true;
                                }
                                else if (!myNHunspell.Spell(newWord))
                                {
                                    resetSpellingRanges = true;
                                }
                            }
                            else
                            {
                                //We did find the new word and we've already added one instance
                                //The only thing left is to remove the original word
                                for (j = i + 1; j <= Information.UBound(_Text, 2); j++)
                                {
                                    _Text(0, j - 1) = _Text(0, j);
                                    _Text(1, j - 1) = _Text(1, j);
                                }
                                _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);
                            }

                            //See if the original word was a spelling error and remove it
                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (_spellingErrors(j) == originalWord)
                                {
                                    if (Information.UBound(_spellingErrors) > 0)
                                    {
                                        //If there is more than one entry in _spellingErrors
                                        //then move the entries above this one down one
                                        for (k = (j + 1); k <= Information.UBound(_spellingErrors); k++)
                                        {
                                            _spellingErrors(k - 1) = _spellingErrors(k);
                                        }
                                    }
                                    Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));
                                    resetSpellingRanges = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!myNHunspell.Spell(newWord))
                                resetSpellingRanges = true;

                            //We've handled everything so we can GoTo SaveFullText
                            goto SaveFullText;
                        }
                    }
                }



                //If we get past the Next, then something went wrong
            }
            else
            {
                //If we get here, then original word is blank
                //See if the new word is in the array
                bool foundNewWord = false;

                for (i = 0; i <= Information.UBound(_Text, 2); i++)
                {
                    if (_Text(0, i) == newWord)
                    {
                        foundNewWord = true;
                        _Text(1, i) = _Text(1, i) + 1;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                if (!foundNewWord)
                {
                    _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
                    _Text(0, Information.UBound(_Text, 2)) = newWord;
                    _Text(1, Information.UBound(_Text, 2)) = 1;

                    //Check if the new word is a spelling error
                    if (!myNHunspell.Spell(newWord))
                    {
                        Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                        _spellingErrors(Information.UBound(_spellingErrors)) = newWord;
                        resetSpellingRanges = true;
                    }
                }
            }


        }
        else
        {
            //If we get here, then there is nothing in the Text array yet
            _Text = new string[2, 1];
            _Text(0, 0) = newWord;
            _Text(1, 0) = 1;

            //Check if the new word is a spelling error
            if (!myNHunspell.Spell(newWord))
            {
                _spellingErrors = new string[1];
                _spellingErrors(0) = newWord;
                resetSpellingRanges = true;
            }
        }
    SaveFullText:



        //Save FullText
        if (SelectionStart == 0)
        {
            //We're at the beginning of the text
            FullText = Input + FullText;
        }
        else if (SelectionStart == FullText.Length)
        {
            //We're at the end of the text
            FullText = FullText + Input;
        }
        else
        {
            //We're somewhere in the middle
            FullText = Strings.Left(FullText, SelectionStart) + Input + Strings.Right(FullText, FullText.Length - SelectionStart);
        }

        //Reset the spelling error ranges
        if (resetSpellingRanges)
        {
            SetSpellingErrorRanges();
        }
    }


    /// <summary>
    /// Removes the character after the selection start (which is 0-based)
    /// </summary>
    /// <param name="SelectionStart">The position directly before the character to be removed</param>
    /// <remarks></remarks>
    public void RemoveText(int SelectionStart)
    {
        //Remove Text is going to function as delete key
        //If the position given to us is at the end, this won't work
        //Also, if there is nothing in fulltext, then there's nothing to delete
        if (SelectionStart == (FullText.Length) | FullText.Length == 0 | SelectionStart == -1)
            return;

        //Sometimes the SetText is called first
        if (_setTextCalledFirst == true)
        {
            _setTextCalledFirst = false;
            return;
        }

        //If we can reset the ignoreRanges, then do that
        if (!_dontResetIgnoreRanges)
        {
            _ignoreRange = new CharacterRange[-1 + 1];
        }


        //If there is only one char in FullText, we can just reset it
        if (FullText.Length == 1)
        {
            FullText = "";
            _Text = new string[2, -1 + 1];
            _spellingErrors = new string[-1 + 1];
            return;
        }

        string originalWord = null;
        string newWord = null;
        int endOfWord = 0;
        int beginning = 0;
        bool resetSpellingRanges = false;

        //Check if we're deleting a non letter or digit

        if (!char.IsLetterOrDigit(FullText(SelectionStart)))
        {

            //see if there are no letters or digits around it...if so, we just
            //update fulltext and move on

            //Check if at the end
            if (SelectionStart == (FullText.Length - 1) & !char.IsLetterOrDigit(FullText(SelectionStart)))
            {
                goto SaveFullText;


            }
            else if (SelectionStart == 0)
            {
                //We're at the beginning
                goto SaveFullText;


            }
            else
            {
                //We're in the middle

                //Check the char on either side...if one of them is a non letter or digit
                //then we can also just save the full text.  We're not combining two words
                //into one,
                //Example: "This is. A" deleting the period
                if (!char.IsLetterOrDigit(FullText(SelectionStart - 1)) | !char.IsLetterOrDigit(FullText(SelectionStart + 1)))
                {
                    goto SaveFullText;


                }
                else
                {
                    //If both of the char on either side are letters or digits, then
                    //we're merging two words into one...this is a special case where
                    //we will need to delete both of the original words and create a new one

                    //We need to get the two original words
                    //Example "this tle" deleting the space
                    //SelectionStart = 4
                    //firstWord beginning = 0
                    //firstWord end = 3
                    //secondWord beginning = 5
                    //secondWord end = 7

                    string firstWord = null;
                    string secondWord = null;

                    //Get the first word (to do this, use SelectionStart -1)
                    beginning = FindFirstLetterOrDigitFromPosition(SelectionStart - 1);
                    firstWord = Strings.Mid(FullText, beginning + 1, SelectionStart - beginning);

                    //Get the second word (to do this, use Selection Start +1)
                    endOfWord = FindLastLetterOrDigitFromPosition(SelectionStart + 1);
                    secondWord = Strings.Mid(FullText, SelectionStart + 2, endOfWord - SelectionStart);

                    //The new word is just firstWord & secondWord
                    newWord = firstWord + secondWord;

                    //Find the first word and remove one instance of it

                    for (i = 0; i <= Information.UBound(_Text, 2); i++)
                    {

                        if (_Text(0, i) == firstWord & _Text(1, i) == 1)
                        {
                            //Remove the word from Text and _spellingErrors
                            for (j = i + 1; j <= Information.UBound(_Text, 2); j++)
                            {
                                _Text(0, j - 1) = _Text(0, j);
                                _Text(1, j - 1) = _Text(1, j);
                            }

                            _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);

                            //now remove it from the spell check
                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (Information.UBound(_spellingErrors) > 0)
                                {
                                    for (k = j + 1; k <= Information.UBound(_spellingErrors); k++)
                                    {
                                        _spellingErrors(k - 1) = _spellingErrors(k);
                                    }
                                }
                                Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));
                                resetSpellingRanges = true;
                                break; // TODO: might not be correct. Was : Exit For
                            }

                            break; // TODO: might not be correct. Was : Exit For


                        }
                        else if (_Text(0, i) == firstWord & _Text(1, i) == 1)
                        {
                            //just remove an instance of the word
                            _Text(1, i) = _Text(1, i) - 1;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }



                    //Find the second word and remove one instance of it

                    for (i = 0; i <= Information.UBound(_Text, 2); i++)
                    {

                        if (_Text(0, i) == secondWord & _Text(1, i) == 1)
                        {
                            //Remove the word from Text and _spellingErrors
                            for (j = i + 1; j <= Information.UBound(_Text, 2); j++)
                            {
                                _Text(0, j - 1) = _Text(0, j);
                                _Text(1, j - 1) = _Text(1, j);
                            }

                            _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);

                            //now remove it from the spell check
                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (Information.UBound(_spellingErrors) > 0)
                                {
                                    for (k = j + 1; k <= Information.UBound(_spellingErrors); k++)
                                    {
                                        _spellingErrors(k - 1) = _spellingErrors(k);
                                    }
                                }
                                Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));
                                resetSpellingRanges = true;
                                break; // TODO: might not be correct. Was : Exit For
                            }

                            break; // TODO: might not be correct. Was : Exit For


                        }
                        else if (_Text(0, i) == firstWord & _Text(1, i) == 1)
                        {
                            //just remove an instance of the word
                            _Text(1, i) = _Text(1, i) - 1;
                            break; // TODO: might not be correct. Was : Exit For
                        }


                    }

                    //Now add the new word
                    _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
                    _Text(0, Information.UBound(_Text, 2)) = newWord;
                    _Text(1, Information.UBound(_Text, 2)) = 1;

                    //Now spell check it
                    if (!myNHunspell.Spell(newWord))
                    {
                        Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                        _spellingErrors(Information.UBound(_spellingErrors)) = newWord;
                        resetSpellingRanges = true;
                    }
                }

                goto SaveFullText;
            }

        }



        //Now we need to figure out what the original word was and what the new word will be

        //Start with the case that we're at the beginning of the text

        if (SelectionStart == 0)
        {

            //Now make sure there is a letter or digit currently at the beginning of the text
            if (!char.IsLetterOrDigit(Strings.Left(FullText, 1)))
            {
                //Example: " This" deleting the leading whitespace
                originalWord = "";
                newWord = "";
            }
            else
            {
                //Example: "This" deleting T
                //SelectionStart = 0
                //endOfWord = 3

                //Find the end of the word that begins the FullText
                endOfWord = FindLastLetterOrDigitFromPosition(SelectionStart);
                originalWord = Strings.Left(FullText, (endOfWord - SelectionStart + 1));
                newWord = Strings.Right(originalWord, originalWord.Length - 1);
            }



        }
        else if (SelectionStart == FullText.Length - 1)
        {

            //Now check if we're at the end
            //Make sure there is a letter or digit at the end of the text
            if (!char.IsLetterOrDigit(Strings.Right(FullText, 1)))
            {
                //Example: "This " deleting preceding white space
                originalWord = "";
                newWord = "";
            }
            else
            {
                //Example: "This" deleting s
                //SelectionStart = 3
                //beginning = 0

                beginning = FindFirstLetterOrDigitFromPosition(SelectionStart);
                originalWord = Strings.Right(FullText, ((SelectionStart + 1) - beginning));
                newWord = Strings.Left(originalWord, originalWord.Length - 1);
            }


            //We're somewhere in the middle of the text
        }
        else
        {


            beginning = FindFirstLetterOrDigitFromPosition(SelectionStart);
            endOfWord = FindLastLetterOrDigitFromPosition(SelectionStart);

            //Example: "This" deleting i         "This will" deleting s
            //SelectionStart = 2                 SelectionStart = 3
            //beginning = 0                      beginning = 0
            //endOfWord = 3                      endOfWord = 3
            originalWord = Strings.Mid(FullText, beginning + 1, (endOfWord - beginning + 1));

            newWord = Strings.Mid(FullText, beginning + 1, SelectionStart - beginning) + Strings.Mid(FullText, SelectionStart + 2, (endOfWord - SelectionStart));
        }




        if (Information.UBound(_Text, 2) > -1)
        {


            for (i = 0; i <= Information.UBound(_Text, 2); i++)
            {


                if (_Text(0, i) == originalWord & _Text(1, i) == 1)
                {

                    //Make sure there is a new word and we weren't deleting a single char word
                    if (newWord.Length > 0)
                    {
                        //Check if the word already exists
                        bool foundNewWord = false;

                        for (j = 0; j <= Information.UBound(_Text, 2); j++)
                        {
                            if (_Text(0, j) == newWord)
                            {
                                foundNewWord = true;
                                _Text(1, j) = _Text(1, j) + 1;

                                //we can also delete the original word
                                //See if the original word was a spelling error and remove it
                                for (l = 0; l <= Information.UBound(_spellingErrors); l++)
                                {
                                    if (_spellingErrors(l) == originalWord)
                                    {
                                        if (Information.UBound(_spellingErrors) > 0)
                                        {
                                            for (k = l + 1; k <= Information.UBound(_spellingErrors); k++)
                                            {
                                                _spellingErrors(k - 1) = _spellingErrors(k);
                                            }

                                            Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));
                                            resetSpellingRanges = true;
                                        }
                                        else
                                        {
                                            _spellingErrors = new string[-1 + 1];
                                            resetSpellingRanges = true;
                                        }
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }

                                //Move all entries in array after this down one
                                for (l = i + 1; l <= Information.UBound(_Text, 2); l++)
                                {
                                    _Text(0, l - 1) = _Text(0, l);
                                    _Text(1, l - 1) = _Text(1, l);
                                }

                                _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);

                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }



                        if (!foundNewWord)
                        {
                            //replace the originalword with the newword
                            _Text(0, i) = newWord;

                            //See if the original word was a spelling error and remove it
                            for (l = 0; l <= Information.UBound(_spellingErrors); l++)
                            {
                                if (_spellingErrors(l) == originalWord)
                                {
                                    if (Information.UBound(_spellingErrors) > 0)
                                    {
                                        for (k = l + 1; k <= Information.UBound(_spellingErrors); k++)
                                        {
                                            _spellingErrors(k - 1) = _spellingErrors(k);
                                        }

                                        Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));
                                        resetSpellingRanges = true;
                                    }
                                    else
                                    {
                                        _spellingErrors = new string[-1 + 1];
                                        resetSpellingRanges = true;
                                    }
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            //Spell check the new word
                            bool foundSpellNewWord = false;

                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (_spellingErrors(j) == newWord)
                                {
                                    foundSpellNewWord = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!foundSpellNewWord & !myNHunspell.Spell(newWord))
                            {
                                Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
                                _spellingErrors(Information.UBound(_spellingErrors)) = newWord;
                                resetSpellingRanges = true;
                            }
                        }

                        break; // TODO: might not be correct. Was : Exit For


                    }
                    else
                    {
                        //There is no newWord...just delete the original word
                        for (j = i + 1; j <= Information.UBound(_Text, 2); j++)
                        {
                            _Text(0, j - 1) = _Text(0, j);
                            _Text(1, j - 1) = _Text(1, j);
                        }

                        _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);

                        //See if the original was a spelling error
                        for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                        {
                            if (_spellingErrors(j) == originalWord)
                            {
                                if (Information.UBound(_spellingErrors) > 0)
                                {
                                    for (k = j + 1; k <= Information.UBound(_spellingErrors); k++)
                                    {
                                        _spellingErrors(k - 1) = _spellingErrors(k);
                                    }

                                    Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors));
                                    resetSpellingRanges = true;
                                }
                                else
                                {
                                    _spellingErrors = new string[-1 + 1];
                                    resetSpellingRanges = true;
                                }
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }

                        break; // TODO: might not be correct. Was : Exit For


                    }


                }
                else if (_Text(0, i) == originalWord & _Text(1, i) > 1)
                {
                    //Reduce the number of duplicate entries by one
                    _Text(1, i) = _Text(1, i) - 1;

                    //see if the new word is an already added word
                    bool FoundNewWord = false;

                    for (j = 0; j <= Information.UBound(_Text, 2); j++)
                    {
                        if (_Text(0, j) == newWord)
                        {
                            FoundNewWord = true;
                            _Text(1, j) = _Text(1, j) + 1;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }

                    if (!FoundNewWord)
                    {
                        //Make sure there is a new word and we weren't deleting a single char word
                        if (newWord.Length > 0)
                        {
                            _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
                            _Text(0, Information.UBound(_Text, 2)) = newWord;
                            _Text(1, Information.UBound(_Text, 2)) = 1;

                            //Spell check newWord
                            FoundNewWord = false;

                            for (j = 0; j <= Information.UBound(_spellingErrors); j++)
                            {
                                if (_spellingErrors(j) == newWord)
                                {
                                    FoundNewWord = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }

                            if (!FoundNewWord)
                            {
                                if (!myNHunspell.Spell(newWord))
                                {
                                    _spellingErrors = new string[Information.UBound(_spellingErrors) + 2];
                                    _spellingErrors(Information.UBound(_spellingErrors)) = newWord;
                                    resetSpellingRanges = true;
                                }
                            }
                            break; // TODO: might not be correct. Was : Exit For


                        }
                        else
                        {
                            //If there is no new word, then move any word after it down the array and
                            //resize the array
                            for (j = i + 1; j >= Information.UBound(_Text, 2); j += -1)
                            {
                                _Text(0, j - 1) = _Text(0, j);
                                _Text(1, j - 1) = _Text(1, j);
                            }
                            _Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2)]);
                            break; // TODO: might not be correct. Was : Exit For
                        }


                    }


                }


            }


        }
    SaveFullText:

        //Save FullText
        if (SelectionStart == 0)
        {
            //we're at the beginning
            FullText = Strings.Right(FullText, FullText.Length - 1);
        }
        else if (SelectionStart == FullText.Length - 1)
        {
            //Deleting the last character
            FullText = Strings.Left(FullText, FullText.Length - 1);
        }
        else
        {
            //Deleting somewhere in the middle
            FullText = Strings.Left(FullText, SelectionStart) + Strings.Right(FullText, (FullText.Length - SelectionStart - 1));
        }

        //Reset the spelling error ranges
        if (resetSpellingRanges)
        {
            SetSpellingErrorRanges();
        }
    }


    /// <summary>
    /// Parse the input string into its separate words
    /// </summary>
    /// <param name="Input"></param>
    /// <remarks></remarks>
    public void SetText(string Input)
	{
		//If we have already handled this with the keypress or keydown events
		//This will allow for the text to change based on non-user input
		if (FullText == Input)
			return;

		//If we can reset the ignore ranges, then do that
		if (!_dontResetIgnoreRanges) {
			_ignoreRange = new CharacterRange[-1 + 1];
		}

		_setTextCalledFirst = true;

		//The idea here is that we need to know the start of a new word, and if the last letter
		//was part of another word.  wordStarted is used to determine if we have already had
		//a letter or digit preceding the current char.
		int wordStart = 1;
		bool wordStarted = false;
		_Text = new string[2, -1 + 1];
		_spellingErrors = new string[-1 + 1];
		_spellingErrorRanges = new CharacterRange[-1 + 1];

		//set FullText
		FullText = Input;
		bool resetSpellingRanges = false;


		//Go through every char in the textbox one by one

		for (i = 1; i <= Input.Length; i++) {

			if (!char.IsLetterOrDigit(Strings.Mid(Input, i, 1)) & wordStarted == true) {
				//We know it's not a letter or digit so it could be the end of a word


				//Check if it's an apostrophe or hyphen, if it is, it's not the end of a word
				if ((Strings.Mid(Input, i, 1) == "'" | Strings.Mid(Input, i, 1) == "-") & i != Input.Length) {
					if (char.IsLetterOrDigit(Strings.Mid(Input, i + 1, 1))) {
						//is an apostrophe or hyphen, then we just go to the next character
						goto FoundApostrophe;
					}
				}

				//Check if we think this is the beginning of a word.  If wordStart = i, then
				//we're possibly at the beginning of a word
				if (wordStart != i) {
					wordStarted = false;

					//Now see if the word has already been added (we're not adding words
					//more than once.  This way we only have to spell check each word once)
					bool boolFound = false;

					for (j = 0; j <= Information.UBound(_Text, 2); j++) {
						if (_Text(0, j) == Strings.Trim(Strings.Mid(Input, wordStart, i - wordStart))) {
							boolFound = true;
							_Text(1, j) = _Text(1, j) + 1;
							break; // TODO: might not be correct. Was : Exit For
						}
					}


					//If the word hasn't been added, add it and then spell check it
					if (!boolFound) {
						_Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
						_Text(0, Information.UBound(_Text, 2)) = Strings.Trim(Strings.Mid(Input, wordStart, i - wordStart));
						_Text(1, Information.UBound(_Text, 2)) = 1;

						//Spell check it
						bool foundWord = false;

						for (j = 0; j <= Information.UBound(_spellingErrors); j++) {
							if (_spellingErrors(j) == Strings.Trim(Strings.Mid(Input, wordStart, i - wordStart))) {
								foundWord = true;
								break; // TODO: might not be correct. Was : Exit For
							}
						}

						if (!myNHunspell.Spell(_Text(0, Information.UBound(_Text, 2))) & !foundWord) {
							Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
							_spellingErrors(Information.UBound(_spellingErrors)) = _Text(0, Information.UBound(_Text, 2));
							resetSpellingRanges = true;
						}
					}
					wordStart = i + 1;
				}
			} else if (char.IsLetterOrDigit(Strings.Mid(Input, i, 1))) {
				if (!wordStarted)
					wordStart = i;
				wordStarted = true;
			}
			FoundApostrophe:
		}



		//We have to check the last character separately or the last word won't be added
		if (!char.IsLetterOrDigit(Strings.Right(Input, 1))) {
			goto ResetSpelling;
		}

		//Check the last word and see it is had been added
		bool boolFound2 = false;

		for (j = 0; j <= Information.UBound(_Text, 2); j++) {
			if (_Text(0, j) == Strings.Trim(Strings.Mid(Input, wordStart, Input.Length - wordStart + 1))) {
				boolFound2 = true;
				_Text(1, j) = _Text(1, j) + 1;
				break; // TODO: might not be correct. Was : Exit For
			}
		}

		//If it hasn't been added, add it and spell check it
		if (!boolFound2) {
			_Text = (string[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(_Text, new string[2, Information.UBound(_Text, 2) + 2]);
			_Text(0, Information.UBound(_Text, 2)) = Strings.Trim(Strings.Mid(Input, wordStart, Input.Length - wordStart + 1));
			_Text(1, Information.UBound(_Text, 2)) = 1;

			//Spell check it
			bool foundWord = false;

			for (j = 0; j <= Information.UBound(_spellingErrors); j++) {
				if (_spellingErrors(j) == _Text(0, Information.UBound(_Text, 2))) {
					foundWord = true;
					break; // TODO: might not be correct. Was : Exit For
				}
			}

			if (!myNHunspell.Spell(_Text(0, Information.UBound(_Text, 2))) & !foundWord) {
				Array.Resize(ref _spellingErrors, Information.UBound(_spellingErrors) + 2);
				_spellingErrors(Information.UBound(_spellingErrors)) = _Text(0, Information.UBound(_Text, 2));
				resetSpellingRanges = true;
			}
		}
		ResetSpelling:
		if (resetSpellingRanges) {
			SetSpellingErrorRanges();
		}
	}


    #endregion


    #region "FindPositions"
    /// <summary>
    /// Given a starting point, we're looking at the characters before it to find the
    /// position of the first character in the word containing the starting point
    /// </summary>
    /// <param name="SelectionStart">0-based starting point</param>
    /// <returns>0-based index of the first character in the word</returns>
    /// <remarks></remarks>
    private long FindFirstLetterOrDigitFromPosition(long SelectionStart)
    {
        for (i = SelectionStart - 1; i >= 0; i += -1)
        {
            if (!char.IsLetterOrDigit(FullText(i)))
            {
                //Need to check if it's an apostrophe or hyphen
                bool foundApOrHyph = false;

                if ((FullText(i) == "'" | FullText(i) == "-") & i != 0)
                {
                    if (char.IsLetterOrDigit(FullText(i - 1)))
                    {
                        foundApOrHyph = true;
                    }
                }

                if (!foundApOrHyph)
                {
                    return i + 1;
                }
            }
        }

        return 0;
    }


    /// <summary>
    /// Given a starting position, this will return the 0-based index representing
    /// the end of a word containing the character at the starting position
    /// </summary>
    /// <param name="SelectionStart">0-based index</param>
    /// <returns>0-based index of the last character in the word</returns>
    /// <remarks></remarks>
    private long FindLastLetterOrDigitFromPosition(long SelectionStart)
    {
        //Character array is 0 based in this function
        for (i = SelectionStart; i <= FullText.Length - 1; i++)
        {
            if (!char.IsLetterOrDigit(FullText(i)))
            {
                //Need to check if it's an apostrophe or hyphen 
                bool foundApOrHyph = false;

                if ((FullText(i) == "'" | FullText(i) == "-") & i != FullText.Length)
                {
                    if (char.IsLetterOrDigit(FullText(i + 1)))
                    {
                        foundApOrHyph = true;
                    }
                }

                if (!foundApOrHyph)
                {
                    //We found the character after the end of the last word
                    return i - 1;
                }
            }
        }

        return FullText.Length - 1;
    }


    #endregion


    #region "Spelling Functions and Subs"
    /// <summary>
    /// Add the range of a word to ignore.
    /// </summary>
    /// <param name="IgnoreRange"></param>
    /// <remarks></remarks>
    public void AddRangeToIgnore(CharacterRange IgnoreRange)
    {
        Array.Resize(ref _ignoreRange, Information.UBound(_ignoreRange) + 2);
        _ignoreRange(Information.UBound(_ignoreRange)) = IgnoreRange;
    }


    public void ClearIgnoreRanges()
    {
        _ignoreRange = new CharacterRange[-1 + 1];
    }


    public void DontResetIgnoreRanges(bool DontReset = true)
    {
        _dontResetIgnoreRanges = DontReset;
    }


    public CharacterRange[] GetIgnoreRanges()
    {
        return _ignoreRange;
    }


    /// <summary>
    /// Returns the ranges of characters associated with misspelled words.
    /// This is used by the CustomPaint to know where to paint the squiggly lines
    /// </summary>
    /// <returns>CharacterRange</returns>
    /// <remarks></remarks>
    public CharacterRange[] GetSpellingErrorRanges()
    {
        return _spellingErrorRanges;
    }


    /// <summary>
    /// Return the words that are spelling errors
    /// </summary>
    /// <returns>Array of strings</returns>
    /// <remarks></remarks>
    public string[] GetSpellingErrors()
    {
        return _spellingErrors;
    }


    /// <summary>
    /// Returns the requested number of suggestions based on the inputted word
    /// </summary>
    /// <param name="Word">Word we need suggestions for</param>
    /// <param name="NumberOfSuggestions">How many suggestions to return</param>
    /// <returns>Array of strings with the suggestions</returns>
    /// <remarks></remarks>
    public string[] GetSuggestions(string Word, int NumberOfSuggestions)
    {
        string[] suggestions = null;
        List<string> NHunspellSugg = new List<string>();
        suggestions = new string[-1 + 1];
        NHunspellSugg = myNHunspell.Suggest(Word);

        for (i = 0; i <= NumberOfSuggestions - 1; i++)
        {
            if (i < NHunspellSugg.Count)
            {
                Array.Resize(ref suggestions, Information.UBound(suggestions) + 2);
                suggestions(Information.UBound(suggestions)) = NHunspellSugg.Item(i);
            }
        }

        return suggestions;
    }


    /// <summary>
    /// Given a 0-based char index, returns the misspelled word that that character is part of
    /// </summary>
    /// <param name="CharIndex">0-based Index</param>
    /// <returns>Strings.String Type</returns>
    /// <remarks></remarks>
    public string GetMisspelledWordAtPosition(int CharIndex)
	{
		CharacterRange currentRange = default(CharacterRange);

		foreach ( currentRange in _spellingErrorRanges) {
			if ((CharIndex >= currentRange.First) & (CharIndex <= (currentRange.First + currentRange.Length + 1))) {
				return Strings.Mid(FullText, currentRange.First + 1, currentRange.Length);
			}
		}

		return "";
	}


    /// <summary>
    /// Returns whether or not the text has any spelling errors
    /// </summary>
    /// <returns>A boolean representing whether there are spelling errors</returns>
    /// <remarks></remarks>
    public bool HasSpellingErrors()
    {
        return (Information.UBound(_spellingErrors) > -1);
    }


    /// <summary>
    /// Given a 0-based character index, returns whether the item is part of a misspelled word
    /// </summary>
    /// <param name="CharIndex">0-based index</param>
    /// <returns>Boolean</returns>
    /// <remarks></remarks>
    public bool IsPartOfSpellingError(int CharIndex)
	{
		CharacterRange currentRange = default(CharacterRange);
		bool result = false;

		foreach ( currentRange in _spellingErrorRanges) {
			if ((CharIndex >= currentRange.First) & (CharIndex <= (currentRange.First + currentRange.Length + 1))) {
				result = true;
				break; // TODO: might not be correct. Was : Exit For
			}
		}

		if (result) {
			//we need to check if it's part of an ignore range
			foreach ( currentRange in _ignoreRange) {
				if (CharIndex >= currentRange.First & (CharIndex <= (currentRange.First + currentRange.Length + 1))) {
					result = false;
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}

		return result;
	}


    /// <summary>
    /// Sets the character ranges of the spelling errors
    /// </summary>
    /// <remarks></remarks>
    public void SetSpellingErrorRanges()
	{
		_spellingErrorRanges = new CharacterRange[-1 + 1];

		if (!HasSpellingErrors())
			return;

		//The idea here is that we need to know the start of a new word, and if the last letter
		//was part of another word
		int wordStart = 1;
		bool wordStarted = false;

		//Go through every char in FullText one by one
		for (i = 1; i <= FullText.Length; i++) {
			if (!char.IsLetterOrDigit(Strings.Mid(FullText, i, 1))) {
				//We know it's not a letter or digit so it could be the end of a word

				//Check if it's an apostrophe or hyphen, if it is, it's not the end of a word
				if ((Strings.Mid(FullText, i, 1) == "'" | Strings.Mid(FullText, i, 1) == "-") & i != FullText.Length) {
					if (char.IsLetterOrDigit(Strings.Mid(FullText, i + 1, 1))) {
						//is an apostrophe or hyphen
						goto FoundApostrophe;
					}
				}

				//Check if we think this is the beginning of a word
				if (wordStart != i) {
					wordStarted = false;

					string currentWord = Strings.Trim(Strings.Mid(FullText, wordStart, i - wordStart));

					//Spell check it
					for (j = 0; j <= Information.UBound(_spellingErrors); j++) {
						if (_spellingErrors(j) == currentWord) {
							//Add it to the array
							Array.Resize(ref _spellingErrorRanges, Information.UBound(_spellingErrorRanges) + 2);
							_spellingErrorRanges(Information.UBound(_spellingErrorRanges)) = new CharacterRange(wordStart - 1, currentWord.Length);
						}
					}
				}
			} else {
				if (!wordStarted)
					wordStart = i;
				wordStarted = true;
			}
			FoundApostrophe:
		}

		//We have to check the last character separately or the last word won't be added
		if (!char.IsLetterOrDigit(Strings.Right(FullText, 1))) {
			return;
		}

		string lastWord = Strings.Trim(Strings.Mid(FullText, wordStart, FullText.Length - wordStart + 1));

		//Spell check it
		for (j = 0; j <= Information.UBound(_spellingErrors); j++) {
			if (_spellingErrors(j) == lastWord) {
				//Add it to the array
				Array.Resize(ref _spellingErrorRanges, Information.UBound(_spellingErrorRanges) + 2);
				_spellingErrorRanges(Information.UBound(_spellingErrorRanges)) = new CharacterRange(wordStart - 1, lastWord.Length);
			}
		}
	}


    #endregion


    public void UpdateHunspell(ref Hunspell newHunspell)
    {
        myNHunspell = newHunspell;
    }

}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
