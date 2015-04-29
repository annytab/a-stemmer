using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annytab
{
    /// <summary>
    /// This class represent the default stemmer, it does not do any stemming at all. 
    /// It can be used for languages that not are supported yet. 
    /// </summary>
    public class DefaultStemmer : Stemmer
    {
        #region Constructors

        /// <summary>
        /// Create a new default stemmer with default properties
        /// </summary>
        public DefaultStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[0];

        } // End of the constructor

        #endregion

        #region Methods

        /// <summary>
        /// Get steam words as a string array from words in a string array
        /// </summary>
        /// <param name="words">An array of words</param>
        /// <returns>An array of steam words</returns>
        public override string[] GetSteamWords(string[] words)
        {
            return words;

        } // End of the GetSteamWords method

        /// <summary>
        /// Get the default steam word
        /// </summary>
        /// <param name="word">The word to strip</param>
        /// <returns>The stripped word</returns>
        public override string GetSteamWord(string word)
        {
            return word;

        } // End of the GetSteamWord method

        #endregion

    } // End of the class

} // End of the namespace

