using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip german words to the steam
    /// This class is based on the german stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/german/stemmer.html
    /// </summary>
    public class GermanStemmer : Stemmer
    {
        #region Variables

        private char[] valid_s_endings;
        private char[] valid_st_endings;
        private string[] endingsStep1a;
        private string[] endingsStep1b;
        private string[] endingsStep2;
        private string[] endingsStep3;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new german stemmer with default properties
        /// </summary>
        public GermanStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', 'ä', 'ö', 'ü' };
            this.valid_s_endings = new char[] { 'b', 'd', 'f', 'g', 'h', 'k', 'l', 'm', 'n', 'r', 't' };
            this.valid_st_endings = new char[] { 'b', 'd', 'f', 'g', 'h', 'k', 'l', 'm', 'n', 't' };
            this.endingsStep1a = new string[] { "ern", "em", "er" };
            this.endingsStep1b = new string[] { "es", "en", "e" };
            this.endingsStep2 = new string[] { "est", "en", "er" };
            this.endingsStep3 = new string[] { "keit", "heit", "lich", "isch", "ung", "end", "ik", "ig" };

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
        /// Get the steam word from a specific word
        /// </summary>
        /// <param name="word">The word to strip</param>
        /// <returns>The stripped word</returns>
        public override string GetSteamWord(string word)
        {
            // Replace ß by ss
            word = word.Replace("ß", "ss");

            // Make sure that the word is in lower case characters
            word = word.ToLowerInvariant();

            // Create a char array that can be used over an over again
            char[] chars = word.ToCharArray();

            // Put u and y between vowels into upper case
            Int32 charCount = chars.Length - 1;
            for (int i = 1; i < charCount; i++)
            {
                if (chars[i] == 'u' && IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true)
                {
                    chars[i] = 'U';
                }
                else if (chars[i] == 'y' && IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true)
                {
                    chars[i] = 'Y';
                }
            }

            // Get indexes for R1 and R2
            Int32[] partIndexR = CalculateR1R2(chars);

            // Recreate the word
            word = new string(chars);

            // Create the r1 and r2 string
            string strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            string strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // **********************************************
            // Step 1
            // **********************************************
            bool continue_step_1 = true;
            for (int i = 0; i < this.endingsStep1a.Length; i++)
            {
                // Check if the endings matches
                if (strR1.EndsWith(this.endingsStep1a[i]))
                {
                    // Delete the ending
                    word = word.Remove(word.Length - this.endingsStep1a[i].Length);
                    continue_step_1 = false;
                    break;
                }
            }

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            if(continue_step_1 == true)
            {
                for (int i = 0; i < this.endingsStep1b.Length; i++)
                {
                    // Check if the endings matches
                    if (strR1.EndsWith(this.endingsStep1b[i]))
                    {
                        // Delete the ending
                        word = word.Remove(word.Length - this.endingsStep1b[i].Length);

                        if (word.EndsWith("niss") == true)
                        {
                            word = word.Remove(word.Length - 1);
                        }
                        continue_step_1 = false;
                        break;
                    }
                }
            }

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // Delete a s in the end if the s is preceded by a valid s-ending, 
            if (continue_step_1 == true && strR1.EndsWith("s") == true)
            {
                // Get the preceding char before the s
                char precedingChar = word.Length > 1 ? word[word.Length - 2] : '\0';

                // Check if the preceding char is a valid s-ending
                for (int i = 0; i < this.valid_s_endings.Length; i++)
                {
                    if (precedingChar == this.valid_s_endings[i])
                    {
                        // Delete the s
                        word = word.Remove(word.Length - 1);
                        break;
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************
            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            bool check_ends_with_st = true;
            for (int i = 0; i < this.endingsStep2.Length; i++)
            {
                // Make sure that we compare on lowercase letters
                if (strR1.EndsWith(this.endingsStep2[i]))
                {
                    // Delete the ending
                    word = word.Remove(word.Length - this.endingsStep2[i].Length);
                    check_ends_with_st = false;
                    break;
                }
            }

            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // Delete st in the end if st is preceded by a valid st-ending, itself preceded by at least 3 letters 
            if (check_ends_with_st == true && word.Length > 5 && strR1.EndsWith("st") == true)
            {
                // Get the preceding char before the s
                char precedingChar = word[word.Length - 3];

                for (int i = 0; i < this.valid_st_endings.Length; i++)
                {
                    // Check if the preceding char is a valid s-ending
                    if (precedingChar == this.valid_st_endings[i])
                    {
                        // Delete the st
                        word = word.Remove(word.Length - 2);
                        break;
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 3
            // **********************************************
            // Recreate the r1 and r2 string
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            for (int i = 0; i < this.endingsStep3.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep3[i];

                // Check if the word ends with the ending
                if(word.EndsWith(end) == true)
                {

                    if (end == "end" || end == "ung")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);

                            // If preceded by ig, delete if in R2 and not preceded by e
                            if (strR2.EndsWith("ig" + end) == true && word.EndsWith("eig") == false)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }
                    else if (end == "ig" || end == "ik" || end == "isch")
                    {
                        // Delete if in R2 and not preceded by e
                        if(strR2.EndsWith(end) == true && word.EndsWith("e" + end) == false)
                        {
                            word = word.Remove(word.Length - end.Length);
                        }
                    }
                    else if (end == "lich" || end == "heit")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);

                            // If preceded by er or en, delete if in R1 
                            if (strR1.EndsWith("en" + end) == true || strR1.EndsWith("er" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }
                    else if (end == "keit")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);

                            // If preceded by lich or ig, delete if in R2
                            if(strR2.EndsWith("lich" + end) == true)
                            {
                                word = word.Remove(word.Length - 4);
                            }
                            else if (strR2.EndsWith("ig" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        } 
                    }

                    // Break out from the loop, the ending has been found
                    break;
                }
            }
            // **********************************************

            // Turn the word to lower case
            word = word.ToLowerInvariant();

            // Replace the umlaut accent from a, o and u.
            word = word.Replace('ä', 'a').Replace('ü', 'u').Replace('ö', 'o');

            // Return the word
            return word;

        } // End of the GetSteamWord method

        #endregion

        #region Helper methods

        /// <summary>
        /// Calculate the R1 and R2 part for a word
        /// </summary>
        /// <param name="characters">An array of characters</param>
        /// <returns>An int array with the r1 and r2 index</returns>
        public Int32[] CalculateR1R2(char[] characters)
        {
            // Create the int array to return
            Int32 r1 = characters.Length;
            Int32 r2 = characters.Length;

            // Calculate R1
            for (int i = 1; i < characters.Length; i++)
            {
                if (IsVowel(characters[i]) == false && IsVowel(characters[i - 1]) == true)
                {
                    // Set the r1 index
                    r1 = i + 1;
                    break;
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

            // Adjust R1
            if(r1 < 3)
            {
                r1 = 3;
            }

            // Return the int array
            return new Int32[] { r1, r2 };

        } // End of the calculateR1R2 method

        #endregion

    } // End of the class

} // End of the namespace