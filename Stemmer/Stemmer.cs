using System;
using System.Collections.Generic;

namespace Annytab
{
    /// <summary>
    /// This is the interface for a stemmer
    /// </summary>
    public interface Stemmer
    {

        #region Methods

        /// <summary>
        /// Get steam words as a string array from words in a string array
        /// </summary>
        /// <param name="words">An array of words</param>
        /// <returns>An array of steam words</returns>
        string[] GetSteamWords(string[] words);

        /// <summary>
        /// Get the steam word from a specific word
        /// </summary>
        /// <param name="word">The word to strip</param>
        /// <returns>The stripped word</returns>
        string GetSteamWord(string word);

        #endregion

    } // End of the class

} // End of the namespace