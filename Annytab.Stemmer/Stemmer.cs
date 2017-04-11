using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This is the base class for a stemmer
    /// </summary>
    public abstract class Stemmer : IStemmer
    {
        #region Variables

        public char[] vowels;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Stemmer
        /// </summary>
        public Stemmer()
        {
            // Set values for instance variables
            this.vowels = new char[0];

        } // End of the constructor

        #endregion

        #region Abstract methods

        /// <summary>
        /// Get steam words as a string array from words in a string array
        /// </summary>
        /// <param name="words">An array of words</param>
        /// <returns>An array of steam words</returns>
        public abstract string[] GetSteamWords(string[] words);

        /// <summary>
        /// Get the steam word from a specific word
        /// </summary>
        /// <param name="word">The word to strip</param>
        /// <returns>The stripped word</returns>
        public abstract string GetSteamWord(string word);

        #endregion

        #region Helper methods

        /// <summary>
        /// Check if a character is a vowel
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>A boolean that indicates if the character is a vowel</returns>
        public virtual bool IsVowel(char character)
        {
            // Create the boolean to return
            bool isVowel = false;

            // Loop the vowel array
            for (int i = 0; i < this.vowels.Length; i++)
            {
                // Check if the character is a vowel
                if (character == this.vowels[i])
                {
                    isVowel = true;
                }
            }

            // Return the boolean
            return isVowel;

        } // End of the isVowel method

        /// <summary>
        /// Check if a character is a short syllable
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>A boolean that indicates if the character is a short syllable</returns>
        public virtual bool IsShortSyllable(char[] characters, Int32 index)
        {
            // Create the boolean to return
            bool isShortSyllable = false;

            // Indexes
            Int32 plusOneIndex = index + 1;
            Int32 minusOneIndex = index - 1;

            if(index == 0 && characters.Length > 1)
            {
                if (index == 0 && IsVowel(characters[index]) == true && IsVowel(characters[plusOneIndex]) == false)
                {
                    isShortSyllable = true;
                }
            }
            else if (minusOneIndex > -1 && plusOneIndex < characters.Length)
            {
                if (IsVowel(characters[index]) == true && IsVowel(characters[plusOneIndex]) == false && characters[plusOneIndex] != 'w' && characters[plusOneIndex] != 'x'
                    && characters[plusOneIndex] != 'Y' && IsVowel(characters[minusOneIndex]) == false)
                {
                    isShortSyllable = true;
                }
            }

            // Return the boolean
            return isShortSyllable;

        } // End of the IsShortSyllable method

        /// <summary>
        /// Check if a word is a short word
        /// </summary>
        /// <param name="word">The word to check</param>
        /// <param name="strR1">The r1 string</param>
        /// <returns>A boolean that indicates if the word is a short word</returns>
        public virtual bool IsShortWord(string word, string strR1)
        {
            // Create the boolean to return
            bool isShortWord = false;

            // Check if the word is a short word
            if (strR1 == "" && IsShortSyllable(word.ToCharArray(), word.Length - 2) == true)
            {
                isShortWord = true;
            }

            // Return the boolean
            return isShortWord;

        } // End of the IsShortWord method

        #endregion

    } // End of the class

} // End of the namespace