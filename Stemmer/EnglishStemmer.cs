using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Annytab
{
    /// <summary>
    /// This class is used to strip english words to the steam
    /// This class is based on the Porter2 English stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/english/stemmer.html
    /// </summary>
    public class EnglishStemmer : Stemmer
    {

        #region Variables

        private char[] vowels;
        private string[] doubles;
        private string[] valid_li_endings;
        private string[,] step1Replacements;
        private string[,] step2Replacements;
        private string[,] step3Replacements;
        private string[] step4Replacements;
        private string[,] exceptions;
        private string[] exceptions2;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new swedish stemmer with default properties
        /// </summary>
        public EnglishStemmer()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
            this.valid_li_endings = new string[] { "c", "d", "e", "g", "h", "k", "m", "n", "r", "t" };
            this.doubles = new string[] { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
            this.step1Replacements = new string[,] { { "eedly", "ee" }, { "ingly", "" }, { "edly", "" }, { "eed", "ee" }, { "ing", "" }, { "ed", "" } };
            this.step2Replacements = new string[,] {{"ization","ize"},{"iveness","ive"},{"fulness","ful"},{"ational","ate"},{"ousness","ous"},{"biliti","ble"},
            {"tional","tion"},{"lessli","less"},{"fulli","ful"},{"entli","ent"},{"ation","ate"},{"aliti","al"},{"iviti","ive"},{"ousli","ous"},{"alism","al"},
            {"abli","able"},{"anci","ance"},{"alli","al"},{"izer","ize"},{"enci","ence"},{"ator","ate"},{"bli","ble"},{"ogi","og"},{"li",""}};
            this.step3Replacements = new string[,]{{"ational","ate"},{"tional","tion"},{"alize","al"},{"icate","ic"},{"iciti","ic"},{"ative",""},{"ical","ic"},{"ness",""},
            {"ful",""}};
            this.step4Replacements = new string[] { "atori", "ement", "ment", "able", "ible", "ance", "ence", "ate", "aci", "iti", "ion", "ize", "ive", "ous", "ant", "ism", "ent", "al", "er", "ic" };
            this.exceptions = new string[,]{{"skis","ski"},{"skies","sky"},{"dying","die"},{"lying","lie"},{"tying","tie"},{"idly","idl"},{"gently","gentl"},{"ugly","ugly"},
            {"early","early"},{"only","only"},{"singly","singl"},{"sky","sky"},{"news","news"},{"howe","howe"},{"atlas","atlas"},{"cosmos","cosmos"},{"bias","bias"},{"andes","andes"}};
            this.exceptions2 = new string[] { "inning", "outing", "canning", "herring", "earring", "proceed", "exceed", "succeed" };

        } // End of the constructor

        #endregion

        #region Methods

        /// <summary>
        /// Get steam words as a string array from words in a string array
        /// </summary>
        /// <param name="words">An array of words</param>
        /// <returns>An array of steam words</returns>
        public string[] GetSteamWords(string[] words)
        {
            // Create the string array to return
            string[] steamWords = new string[words.Length];

            // Loop the list of words
            for (int i = 0; i < words.Length; i++)
            {
                steamWords[i] = GetSteamWord(words[i]);
            }

            // Return the steam word array
            return steamWords;

        } // End of the GetSteamWords method

        /// <summary>
        /// Get the englist steam word from a specific word
        /// </summary>
        /// <param name="word">The word to strip</param>
        /// <returns>The stripped word</returns>
        public string GetSteamWord(string word)
        {
            // Just return the word if it is 2 letters or less
            if (word.Length < 3)
                return word;

            // Create a char array that can be used over an over again
            char[] chars;

            // Set all characters to lower in the word
            word = word.ToLowerInvariant();

            // Remove a initial apostroph if it is present
            if (word.StartsWith("'") == true)
                word = word.Substring(1);

            // Check for first part exceptions
            for (int i = 0; i < 18; ++i)
            {
                // Check if the word is a exception
                if (word == exceptions[i, 0])
                {
                    // Return the word
                    return exceptions[i, 1];
                }
            }

            // Get a char array of each letter in the word
            chars = word.ToCharArray();

            // Set initial y to Y
            if (chars.Length >= 1 && chars[0] == 'y')
            {
                chars[0] = 'Y';
            }

            // Set y after a vowel to Y
            if (chars.Length >= 2)
            {
                for (int i = 1; i < chars.Length; i++)
                {
                    for (int j = 0; j < this.vowels.Length; j++)
                    {
                        if (chars[i] == 'y' && chars[i - 1] == this.vowels[j])
                        {
                            chars[i] = 'Y';
                        }
                    }
                }
            }

            // Get the modified word from the char array
            word = new string(chars);

            // Calculate indexes for r1 and r2
            int[] partIndexR = CalculateR1R2(word);

            // ********************************************************
            // Step 0 - Remove endings
            // ********************************************************
            if (word.Length >= 3 && word.EndsWith("'s'") == true)
            {
                word = word.Remove(word.Length - 3, 3);
            }
            else if (word.Length >= 2 && word.EndsWith("'s") == true)
            {
                word = word.Remove(word.Length - 2, 2);
            }
            else if (word.Length >= 1 && word.EndsWith("'") == true)
            {
                word = word.Remove(word.Length - 1, 1);
            }
            // ********************************************************

            // ********************************************************
            // Step 1a - Remove endings
            // ********************************************************
            if (word.EndsWith("sses") == true)
            {
                word = word.Remove(word.Length - 2);
            }
            else if (word.EndsWith("ied") == true || word.EndsWith("ies") == true)
            {
                if (word.Length > 4)
                {
                    word = word.Remove(word.Length - 2);
                }
                else
                {
                    word = word.Remove(word.Length - 1);
                }

            }
            else if (word.EndsWith("us") == true || word.EndsWith("ss") == true)
            {
                // Do nothing
            }
            else if (word.EndsWith("s") == true)
            {
                // Convert the word to a character array
                chars = word.ToCharArray();

                // Make sure that the word is long enough
                if (chars.Length >= 2)
                {
                    for (int i = 0; i < chars.Length - 2; i++)
                    {
                        if (IsVowel(chars[i]) == true)
                        {
                            word = word.Remove(word.Length - 1, 1);
                            break;
                        }
                    }
                }
            }
            // ********************************************************

            // Check for second part exceptions
            for (int i = 0; i < this.exceptions2.Length; i++)
            {
                if (word == this.exceptions2[i])
                {
                    return exceptions2[i];
                }
            }

            // Create the r1 and r2 string
            string strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            string strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // ********************************************************
            // Step 1b - Remove endings
            // ********************************************************
            for (int i = 0; i < 6; i++)
            {
                if (step1Replacements[i, 0] == "eedly" && strR1.EndsWith("eedly") == true)
                {
                    word = word.Length >= 2 ? word.Remove(word.Length - 2, 2) : word;
                    break;
                }
                else if (step1Replacements[i, 0] == "eed" && strR1.EndsWith("eed") == true)
                {
                    word = word.Length >= 1 ? word.Remove(word.Length - 1, 1) : word;
                    break;
                }
                else if (word.EndsWith(step1Replacements[i, 0]))
                {
                    // Create a character array
                    chars = word.ToCharArray();

                    bool vowelIsFound = false;

                    // Check if we can find a vowel in the preceding word part
                    if (chars.Length > step1Replacements[i, 0].Length)
                    {
                        for (int j = 0; j < chars.Length - step1Replacements[i, 0].Length; j++)
                        {
                            if (IsVowel(chars[j]))
                            {
                                word = word.Remove(word.Length - step1Replacements[i, 0].Length);
                                vowelIsFound = true;
                                break;
                            }
                        }
                    }

                    if (vowelIsFound == true)
                    {
                        // Check if the word ends with one of these doubles
                        if (word.EndsWith("at") || word.EndsWith("bl") || word.EndsWith("iz"))
                        {
                            word += "e";
                        }

                        // Check if the word ends with a double
                        for (int j = 0; j < this.doubles.Length; j++)
                        {
                            if (word.Length >= 2 && this.doubles[j] == word.Substring(word.Length - 2))
                            {
                                word = word.Remove(word.Length - 1, 1);
                                break;
                            }
                        }

                        if (word.Length >= 2 && IsShortWord(word, strR1) == true)
                        {
                            word += "e";
                        }


                    }

                    // Break out from the loop
                    break;
                }
            }
            // ********************************************************

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // ********************************************************
            // Step 1c - Replace ending
            // ********************************************************
            chars = word.ToCharArray();
            if (word.Length >= 3 && (word.EndsWith("y") || word.EndsWith("Y")) && IsVowel(chars[chars.Length - 2]) == false)
            {
                chars[chars.Length - 1] = 'i';
                word = new string(chars);

            }
            // ********************************************************

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // ********************************************************
            // Step 2 - Remove endings
            // ********************************************************
            for (int i = 0; i < 24; ++i)
            {
                // Check if we can find the ending in R1
                if (strR1.EndsWith(step2Replacements[i, 0]))
                {
                    if (step2Replacements[i, 0] == "ogi")
                    {
                        if (word.EndsWith("logi"))
                        {
                            word = word.Remove(word.Length - 1);
                        }

                        // Break out from the outer loop
                        break;
                    }
                    else if (step2Replacements[i, 0] == "li")
                    {
                        if (word.Length >= 3)
                        {
                            string liEnding = word.Substring(word.Length - 3, 1);
                            for (int j = 0; j < this.valid_li_endings.Length; j++)
                            {
                                if (liEnding == this.valid_li_endings[j])
                                {
                                    word = word.Remove(word.Length - 2);
                                    break;
                                }
                            }
                        }

                        // Break out from the outer loop
                        break;
                    }
                    else if (word.Length >= step2Replacements[i, 0].Length)
                    {
                        word = word.Remove(word.Length - step2Replacements[i, 0].Length);
                        word += step2Replacements[i, 1];

                        // Break out from the outer loop
                        break;
                    }
                }
            }
            // ********************************************************

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // ********************************************************
            // Step 3 - Remove endings
            // ********************************************************
            for (int i = 0; i < 9; ++i)
            {

                if (strR1.EndsWith(step3Replacements[i, 0]))
                {
                    if (step3Replacements[i, 0] == "ative")
                    {
                        if (word.Length >= 5 && strR2.EndsWith("ative"))
                        {
                            word = word.Remove(word.Length - step3Replacements[i, 0].Length);
                        }

                        // Break out from the outer loop
                        break;
                    }
                    else if (word.Length >= 5)
                    {
                        word = word.Remove(word.Length - step3Replacements[i, 0].Length);
                        word += step2Replacements[i, 1];

                        // Break out from the outer loop
                        break;
                    }
                }
            }
            // ********************************************************

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // ********************************************************
            // Step 4 - Remove endings
            // ********************************************************
            for (int i = 0; i < 20; ++i)
            {
                if (strR2.EndsWith(step4Replacements[i]))
                {
                    if (step4Replacements[i] == "ion")
                    {
                        if (word.Length >= 4)
                        {
                            string preChar = word.Substring(word.Length - 4, 1);

                            if (preChar == "s" || preChar == "t")
                            {
                                word = word.Remove(word.Length - step4Replacements[i].Length);
                            }
                        }

                        // Break out from the outer loop
                        break;

                    }
                    else if (word.Length >= 3)
                    {
                        word = word.Remove(word.Length - step4Replacements[i].Length);

                        // Break out from the outer loop
                        break;
                    }
                }
            }
            // ********************************************************

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // ********************************************************
            // Step 5 - Remove endings
            // ********************************************************
            if (word.Length >= 2 && strR1.EndsWith("e") && IsShortSyllable(word.ToCharArray(), word.Length - 2) == false)
            {
                word = word.Remove(word.Length - 1);
            }
            else if (strR2.EndsWith("l") && word.EndsWith("ll"))
            {
                word = word.Remove(word.Length - 1);
            }
            // ********************************************************

            // Return the stripped word
            return word.ToLower();

        } // End of the GetSteamWord method

        #endregion

        #region Helpers

        /// <summary>
        /// Check if a character is a vowel
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>A boolean that indicates if the character is a vowel</returns>
        private bool IsVowel(char character)
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
        /// Calculate the R1 and R2 part for a word
        /// </summary>
        /// <param name="word">The word to calculate R1 and R2 for</param>
        /// <returns>An int array with the r1 and r2 index</returns>
        private int[] CalculateR1R2(string word)
        {
            // Create the int array to return
            int r1 = word.Length;
            int r2 = word.Length;

            // Calculate R1
            if (word.StartsWith("gener") || word.StartsWith("arsen"))
            {
                r1 = 5;
            }
            else if (word.StartsWith("commun"))
            {
                r1 = 6;
            }
            else
            {
                // Convert the word to a char array
                char[] characters = word.ToCharArray();

                // Loop the characters
                for (int i = 1; i < characters.Length; i++)
                {
                    if (IsVowel(characters[i]) == false && IsVowel(characters[i - 1]) == true)
                    {
                        // Set the r1 index
                        r1 = i + 1;
                        break;
                    }
                }
            }

            // Calculate R2
            char[] chars = word.ToCharArray();
            for (int i = r1; i < chars.Length; ++i)
            {
                if (IsVowel(chars[i]) == false && IsVowel(chars[i - 1]) == true)
                {
                    // Set the r2 index
                    r2 = i + 1;
                    break;
                }
            }

            // Return the int array
            return new int[] { r1, r2 };

        } // End of the calculateR1R2 method

        /// <summary>
        /// Check if a character is a short syllable
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>A boolean that indicates if the character is a short syllable</returns>
        private bool IsShortSyllable(char[] characters, int index)
        {
            // Create the boolean to return
            bool isShortSyllable = false;

            // Indexes
            int plusOneIndex = index + 1;
            int minusOneIndex = index - 1;

            if (index == 0 && IsVowel(characters[index]) == true && IsVowel(characters[plusOneIndex]) == false)
            {
                isShortSyllable = true;
            }
            else if (IsVowel(characters[index]) == true && IsVowel(characters[plusOneIndex]) == false && characters[plusOneIndex] != 'w' && characters[plusOneIndex] != 'x'
                && characters[plusOneIndex] != 'Y' && IsVowel(characters[minusOneIndex]) == false)
            {
                isShortSyllable = true;
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
        private bool IsShortWord(string word, string strR1)
        {
            // Create the boolean to return
            bool isShortWord = false;

            // Check if the word is a short word
            if (strR1 == "" && IsShortSyllable(word.ToCharArray(), word.Length - 2))
            {
                isShortWord = true;
            }

            // Return the boolean
            return isShortWord;

        } // End of the IsShortWord method

        #endregion

    } // End of the class

} // End of the namespace