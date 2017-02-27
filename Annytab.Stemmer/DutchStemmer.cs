using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip dutch words to the steam
    /// This class is based on the dutch stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/dutch/stemmer.html
    /// </summary>
    public class DutchStemmer : Stemmer
    {
        #region Variables

        private char[] acuteUmlautAccents;
        private char[] accentReplacements;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new dutch stemmer with default properties
        /// </summary>
        public DutchStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', 'è' };
            this.acuteUmlautAccents = new char[] { 'ä', 'ë', 'ï', 'ö', 'ü', 'á', 'é', 'í', 'ó', 'ú' };
            this.accentReplacements = new char[] { 'a', 'e', 'i', 'o', 'u', 'a', 'e', 'i', 'o', 'u' };
            
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
            // Turn the word into lower case characters
            word = word.ToLowerInvariant();

            // Create a char array
            char[] chars = word.ToCharArray();

            // Replace all acute and umlaut accents
            for (int i = 0; i < chars.Length; i++)
            {
                for (int j = 0; j < this.acuteUmlautAccents.Length; j++)
                {
                    if (chars[i] == this.acuteUmlautAccents[j])
                    {
                        chars[i] = this.accentReplacements[j];
                    }
                }
            }

            // Put initial y, y after a vowel, and i between vowels into upper case
            Int32 charCount = chars.Length - 1;
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    if (chars[i] == 'y')
                    {
                        chars[i] = 'Y';
                    }
                }
                else if (i == charCount)
                {
                    if (chars[i] == 'y' && IsVowel(chars[i - 1]) == true)
                    {
                        chars[i] = 'Y';
                    }
                }
                else if (chars[i] == 'i' && IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true)
                {
                    chars[i] = 'I';
                }
                else if (chars[i] == 'y' && IsVowel(chars[i - 1]) == true)
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
            if(word.EndsWith("heden") == true)
            {
                // Replace with heid if in R1
                if (strR1.EndsWith("heden") == true)
                {
                    // Replace with heid if in R1
                    word = word.Remove(word.Length - 5);
                    word += "heid";
                }
            }
            else if (word.EndsWith("ene") == true)
            {
                // Delete if in R1 and preceded by a valid en-ending, and then undouble the ending
                if (strR1.EndsWith("ene") == true && word.Length > 3 && word.EndsWith("gemene") == false && IsVowel(word[word.Length - 4]) == false)
                {
                    word = word.Remove(word.Length - 3);

                    // Undouble the ending
                    word = UndoubleEnding(word);
                }
            }
            else if (word.EndsWith("en") == true)
            {
                // Delete if in R1 and preceded by a valid en-ending, and then undouble the ending
                if (strR1.EndsWith("en") == true && word.Length > 2 && word.EndsWith("gemene") == false && IsVowel(word[word.Length - 3]) == false)
                {
                    word = word.Remove(word.Length - 2);

                    // Undouble the ending
                    word = UndoubleEnding(word);
                }
            }
            else if (word.EndsWith("se") == true)
            {
                // Delete if in R1 and preceded by a valid s-ending
                if (strR1.EndsWith("se") == true && word.Length > 2 && word[word.Length - 3] != 'j' && IsVowel(word[word.Length - 3]) == false)
                {
                    word = word.Remove(word.Length - 2);
                }
            }
            else if (word.EndsWith("s") == true)
            {
                // Delete if in R1 and preceded by a valid s-ending
                if (strR1.EndsWith("s") == true && word.Length > 1 && word[word.Length - 2] != 'j' && IsVowel(word[word.Length - 2]) == false)
                {
                    word = word.Remove(word.Length - 1);
                }
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // Delete suffix e if in R1 and preceded by a non-vowel, and then undouble the ending
            bool ending_removed_step_2 = false;
            if(strR1.EndsWith("e") == true && word.Length > 1 && IsVowel(word[word.Length - 2]) == false)
            {
                word = word.Remove(word.Length - 1);
                ending_removed_step_2 = true;

                // Undouble the ending
                word = UndoubleEnding(word);
            }
            // **********************************************

            // **********************************************
            // Step 3
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // (a) Delete heid if in R2 and not preceded by c, and treat a preceding en as in step 1(b)
            if(strR2.EndsWith("heid") == true && word.Length > 4 && word[word.Length - 5] != 'c')
            {
                word = word.Remove(word.Length - 4);

                // Delete en if in R1 and preceded by a valid en-ending, and then undouble the ending
                if (strR1.EndsWith("enheid") == true && word.Length > 2 && word.EndsWith("gemene") == false && IsVowel(word[word.Length - 3]) == false)
                {
                    word = word.Remove(word.Length - 2);

                    // Undouble the ending
                    word = UndoubleEnding(word);
                }
            }

            // (b)
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            if(strR2.EndsWith("baar") == true)
            {
                // Delete if in R2
                word = word.Remove(word.Length - 4);
            }
            else if (strR2.EndsWith("lijk") == true)
            {
                // Delete if in R2, and then repeat step 2
                word = word.Remove(word.Length - 4);

                // Delete suffix e if in R1 and preceded by a non-vowel, and then undouble the ending
                if (strR1.EndsWith("elijk") == true && word.Length > 1 && IsVowel(word[word.Length - 2]) == false)
                {
                    word = word.Remove(word.Length - 1);

                    // Undouble the ending
                    word = UndoubleEnding(word);
                }
            }
            else if (strR2.EndsWith("bar") == true)
            {
                // Delete if in R2 and if step 2 actually removed an e
                if(ending_removed_step_2 == true)
                {
                    word = word.Remove(word.Length - 3);
                }
            }
            else if (strR2.EndsWith("end") == true || strR2.EndsWith("ing") == true)
            {
                // Delete if in R2
                word = word.Remove(word.Length - 3);

                strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
                strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

                // If preceded by ig, delete if in R2 and not preceded by e, otherwise undouble the ending
                if(strR2.EndsWith("ig") == true && word.EndsWith("eig") == false)
                {
                    word = word.Remove(word.Length - 2);
                }
                else
                {
                    // Undouble the ending
                    word = UndoubleEnding(word);
                }
            }
            else if (strR2.EndsWith("ig") == true)
            {
                // Delete if in R2 and not preceded by e
                if(word.EndsWith("eig") == false)
                {
                    word = word.Remove(word.Length - 2);
                }
            }
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // If the words ends CVD, where C is a non-vowel, D is a non-vowel other than I, and V is double a, e, o or u, 
            // remove one of the vowels from V (for example, maan -> man, brood -> brod).
            if(word.Length > 3)
            {
                // Get CVD
                char C = word[word.Length - 4];
                string V = word.Substring(word.Length - 3, 2);
                char D = word[word.Length - 1];

                if (D != 'I' && IsVowel(C) == false && IsVowel(D) == false && (V == "aa" || V == "ee" || V == "oo" || V == "uu"))
                {
                    word = word.Remove(word.Length - 2, 1);
                }
            }
            // **********************************************

            // Return the word
            return word.ToLowerInvariant();

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

        /// <summary>
        /// Undouble the ending
        /// </summary>
        /// <param name="word">A reference to the word</param>
        /// <returns>The word</returns>
        private string UndoubleEnding(string word)
        {
            // Undouble the ending
            if (word.EndsWith("kk") == true || word.EndsWith("dd") == true || word.EndsWith("tt") == true)
            {
                word = word.Remove(word.Length - 1);
            }

            // Return the word
            return word;

        } // End of the UndoubleEnding method

        #endregion

    } // End of the class

} // End of the namespace