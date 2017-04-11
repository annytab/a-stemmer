using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This interface represent a stemmer
    /// </summary>
    public interface IStemmer
    {
        string[] GetSteamWords(string[] words);
        string GetSteamWord(string word);
        bool IsVowel(char character);
        bool IsShortSyllable(char[] characters, Int32 index);
        bool IsShortWord(string word, string strR1);

    } // End of the interface

} // End of the namespace