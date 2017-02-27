using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip english words to the steam
    /// This class is based on the Porter2 english stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/english/stemmer.html
    /// </summary>
    public class EnglishStemmer : Stemmer
    {

        #region Variables

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
            : base()
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
            this.step4Replacements = new string[] { "ement", "ment", "ence", "able", "ible", "ance", "ism", "ent", "ate", "iti", "ant", "ous", "ive", "ize", "ion", "ic", "er", "al" };
            this.exceptions = new string[,]{{"skis","ski"},{"skies","sky"},{"dying","die"},{"lying","lie"},{"tying","tie"},{"idly","idl"},{"gently","gentl"},{"ugly","ugly"}, 
                {"early","early"}, {"only","only"},{"singly","singl"},{"sky","sky"},{"news","news"},{"howe","howe"},{"atlas","atlas"},{"cosmos","cosmos"},{"bias","bias"},{"andes","andes"}};
            this.exceptions2 = new string[] { "inning", "outing", "canning", "herring", "earring", "proceed", "exceed", "succeed" };

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
        public override string GetSteamWord(string word)
        {
            // Just return the word if it is 2 letters or less
            if (word.Length < 3)
            {
                return word;
            }

            // Set all characters to lower in the word
            word = word.ToLowerInvariant();

            // Remove a initial apostroph if it is present
            if (word.StartsWith("'") == true)
            {
                word = word.Substring(1);
            }

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
            char[] chars = word.ToCharArray();

            // Set initial y to Y
            if (chars.Length > 0 && chars[0] == 'y')
            {
                chars[0] = 'Y';
            }

            // Set y after a vowel to Y
            if (chars.Length > 1)
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
            Int32[] partIndexR = CalculateR1R2(word);

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
                        // Recreate the r1 and r2 string
                        strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
                        strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

                        // Check if further processing should be done
                        bool continue_processing = true;

                        // Check if the word ends with at, bl or iz
                        if (word.EndsWith("at") || word.EndsWith("bl") || word.EndsWith("iz"))
                        {
                            word += "e";
                            continue_processing = false;
                        }

                        // Check if the word ends with a double
                        if(continue_processing == true)
                        {
                            for (int j = 0; j < this.doubles.Length; j++)
                            {
                                if (word.EndsWith(doubles[j]) == true)
                                {
                                    word = word.Remove(word.Length - 1, 1);
                                    continue_processing = false;
                                    break;
                                }
                            }
                        }

                        // Check if the word is short
                        if (continue_processing == true && IsShortWord(word, strR1) == true)
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
            if (word.Length > 2 && (word.EndsWith("y") || word.EndsWith("Y")) && IsVowel(word[word.Length - 2]) == false)
            {
                word = word.Remove(word.Length - 1);
                word += "i";

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
                // Check if we can find the ending
                if (word.EndsWith(step2Replacements[i, 0]))
                {
                    // Make sure that the ending can be found in R1
                    if(strR1.EndsWith(step2Replacements[i, 0]))
                    {
                        if (step2Replacements[i, 0] == "ogi")
                        {
                            if (word.EndsWith("logi"))
                            {
                                word = word.Remove(word.Length - 1);
                            }
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
                        }
                        else if (word.Length >= step2Replacements[i, 0].Length)
                        {
                            word = word.Remove(word.Length - step2Replacements[i, 0].Length);
                            word += step2Replacements[i, 1];
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
            // Step 3 - Remove endings
            // ********************************************************
            for (int i = 0; i < 9; ++i)
            {
                if (strR1.EndsWith(step3Replacements[i, 0]))
                {
                    if (step3Replacements[i, 0] == "ative")
                    {
                        if (strR2.EndsWith("ative"))
                        {
                            word = word.Remove(word.Length - step3Replacements[i, 0].Length);
                        }
                    }
                    else
                    {
                        word = word.Remove(word.Length - step3Replacements[i, 0].Length);
                        word += step3Replacements[i, 1];
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
            // Step 4 - Remove endings
            // ********************************************************
            for (int i = 0; i < step4Replacements.Length; ++i)
            {
                // Get the end
                string end = step4Replacements[i];

                // Check if the word ends with the ending
                if (word.EndsWith(end) == true)
                {
                    if (strR2.EndsWith(end) == true)
                    {
                        if (end == "ion")
                        {
                            char preChar = word.Length > 4 ? word[word.Length - 4] : '\0';

                            if (preChar == 's' || preChar == 't')
                            {
                                word = word.Remove(word.Length - step4Replacements[i].Length);
                            }
                        }
                        else
                        {
                            word = word.Remove(word.Length - step4Replacements[i].Length);
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
            // Step 5 - Remove endings
            // ********************************************************
            if (strR2.EndsWith("e") || (strR1.EndsWith("e") && IsShortSyllable(word.ToCharArray(), word.Length - 3) == false))
            {
                word = word.Remove(word.Length - 1);
            }
            else if (strR2.EndsWith("l") && word.EndsWith("ll"))
            {
                word = word.Remove(word.Length - 1);
            }
            // ********************************************************

            // Return the stripped word
            return word.ToLowerInvariant();

        } // End of the GetSteamWord method

        #endregion

        #region Helper methods

        /// <summary>
        /// Calculate the R1 and R2 part for a word
        /// </summary>
        /// <param name="word">The word to calculate R1 and R2 for</param>
        /// <returns>An int array with the r1 and r2 index</returns>
        public Int32[] CalculateR1R2(string word)
        {
            // Create the int array to return
            Int32 r1 = word.Length;
            Int32 r2 = word.Length;

            // Convert the word to a char array
            char[] characters = word.ToCharArray();

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
            for (int i = r1; i < characters.Length; ++i)
            {
                if (IsVowel(characters[i]) == false && IsVowel(characters[i - 1]) == true)
                {
                    // Set the r2 index
                    r2 = i + 1;
                    break;
                }
            }

            // Return the int array
            return new Int32[] { r1, r2 };

        } // End of the calculateR1R2 method

        #endregion

    } // End of the class

} // End of the namespace