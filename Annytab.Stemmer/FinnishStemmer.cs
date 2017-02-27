using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip finnish words to the steam
    /// This class is based on the finnish stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/finnish/stemmer.html
    /// </summary>
    public class FinnishStemmer : Stemmer
    {
        #region Variables

        private string[] endingsStep1;
        private string[] endingsStep2;
        private string[] endingsStep3;
        private string[] endingsStep4;
        private char[] restrictedVowels;
        private string[] longVowels;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new finnish stemmer with default properties
        /// </summary>
        public FinnishStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', 'ä', 'ö' };
            this.endingsStep1 = new string[] { "kään", "kaan", "hän", "han", "kin", "pä", "pa", "kö", "ko" };
            this.endingsStep2 = new string[] { "nsa", "nsä", "mme", "nne", "si", "ni", "an", "än", "en" };
            this.endingsStep3 = new string[] { "seen", "siin", "tten", "han", "hon", "lle", "ltä", "lta", "llä", 
                "lla", "stä", "hin", "ssä", "ssa", "hen", "hän", "ttä", "tta", "hön", "ksi", "ine", "den", "sta", 
                "nä", "tä", "ta", "na", "a", "ä", "n" };
            this.endingsStep4 = new string[] { "impi", "impä", "immi", "imma", "immä", "impa", "mpi", "eja", "mpa", "mpä", "mmi", "mma", "mmä", "ejä" };
            this.restrictedVowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'ä', 'ö' };
            this.longVowels = new string[] { "aa", "ee", "ii", "oo", "uu", "ää", "öö" };

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
            // Make sure that the word is in lower case characters
            word = word.ToLowerInvariant();

            // Get indexes for R1 and R2
            Int32[] partIndexR = CalculateR1R2(word.ToCharArray());

            // Create strings
            string strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            string strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // **********************************************
            // Step 1
            // **********************************************
            // (a)
            bool continue_step_1 = true;
            for (int i = 0; i < this.endingsStep1.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep1[i];

                // Check if word ends with some of the predefined step 1 endings
                if (word.EndsWith(end) == true)
                {
                    // Get the preceding character
                    char precedingChar = word.Length > end.Length ? word[word.Length - end.Length - 1] : '\0';

                    // Delete if in R1 and preceded by n, t or a vowel
                    if(strR1.EndsWith(end) == true && (precedingChar == 'n' || precedingChar == 't' || IsVowel(precedingChar) == true))
                    {
                        word = word.Remove(word.Length - end.Length);
                        continue_step_1 = false;
                    }

                    // Break out from the loop (the ending has been found)
                    break;
                }
            }

            // Recreate strings
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // (b)
            if(continue_step_1 == true && strR2.EndsWith("sti") == true)
            {
                word = word.Remove(word.Length - 3);
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            for (int i = 0; i < this.endingsStep2.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep2[i];

                // Check if R1 ends with some of the predefined step 2 endings
                if (strR1.EndsWith(end) == true)
                {
                    if(end == "nsa" || end == "nsä" || end == "mme" || end == "nne") 
                    {
                        // Delete
                        word = word.Remove(word.Length - end.Length);
                    }
                    else if(end == "si")
                    {
                        // Delete if not preceded by k
                        if(word.EndsWith("ksi") == false)
                        {
                            word = word.Remove(word.Length - end.Length);
                        }
                    }
                    else if (end == "ni")
                    {
                        // Delete
                        word = word.Remove(word.Length - end.Length);

                        // If preceded by kse, replace with ksi
                        if(word.EndsWith("kse") == true)
                        {
                            word = word.Remove(word.Length -1);
                            word += "i";
                        }
                    }
                    else if (end == "an")
                    {
                        // Delete if preceded by one of:   ta   ssa   sta   lla   lta   na
                        if (word.EndsWith("taan") == true || word.EndsWith("ssaan") == true || word.EndsWith("staan") == true || word.EndsWith("llaan") == true
                            || word.EndsWith("ltaan") == true || word.EndsWith("naan") == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                        }
                    }
                    else if (end == "än")
                    {
                        // Delete if preceded by one of:   tä   ssä   stä   llä   ltä   nä
                        if (word.EndsWith("tään") == true || word.EndsWith("ssään") == true || word.EndsWith("stään") == true || word.EndsWith("llään") == true
                            || word.EndsWith("ltään") == true || word.EndsWith("nään") == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                        }
                    }
                    else if (end == "en")
                    {
                        // Delete if preceded by one of:   lle   ine
                        if (word.EndsWith("lleen") == true || word.EndsWith("ineen") == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                        }
                    }

                    // Break out from the loop
                    break;
                }
            }
            // **********************************************

            // **********************************************
            // Step 3
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            bool ending_removed_step_3 = false;
            for (int i = 0; i < this.endingsStep3.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep3[i];

                // Check if R1 ends with some of the predefined step 3 endings
                if (strR1.EndsWith(end) == true)
                {
                    if (end == "han" || end == "hen" || end == "hin" || end == "hon" || end == "hän" || end == "hön")
                    {
                        ending_removed_step_3 = true;

                        // Get the middle character
                        string middleCharacter = end.Substring(1, 1);

                        // Delete if preceded by X, where X is a V other than u (a/han, e/hen etc)
                        if(word.EndsWith(middleCharacter + end) == true)
                        {
                            // Delete
                            word = word.Remove(word.Length - end.Length);
                            
                        }
                    }
                    else if (end == "siin" == true || end == "tten" == true || end == "den" == true)
                    {
                        // Get the preceding two letters
                        string precedingString = word.Length > (end.Length + 1) ? word.Substring(word.Length - end.Length - 2, 2) : "";

                        // Delete if preceded by Vi
                        for(int j = 0; j < this.restrictedVowels.Length; j++)
                        {
                            if (precedingString == this.restrictedVowels[j].ToString() + "i")
                            {
                                word = word.Remove(word.Length - end.Length);
                                ending_removed_step_3 = true;
                                break;
                            }
                        }

                    }
                    else if (end == "seen")
                    {
                        // Get the preceding two letters
                        string precedingString = word.Length > (end.Length + 1) ? word.Substring(word.Length - end.Length - 2, 2) : "";

                        // Delete if preceded by LV
                        for (int j = 0; j < this.longVowels.Length; j++)
                        {
                            if (precedingString == this.longVowels[j])
                            {
                                word = word.Remove(word.Length - end.Length);
                                ending_removed_step_3 = true;
                                break;
                            }
                        }
                    }
                    else if (end == "a" || end == "ä")
                    {
                        // Get the preciding two letters
                        char before1 = word.Length > 1 ? word[word.Length - 2] : '\0';
                        char before2 = word.Length > 2 ? word[word.Length - 3] : '\0';

                        // Delete if preceded by cv
                        if(word.Length > 2 && IsVowel(before2) == false && IsVowel(before1) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_3 = true;
                        }
                    }
                    else if (end == "tta" || end == "ttä")
                    {
                        // Delete if preceded by e
                        if(word.EndsWith("e" + end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_3 = true;
                        }
                    }
                    else if (end == "ta" || end == "tä" || end == "ssa" || end == "ssä" || end == "sta" || 
                        end == "stä" || end == "lla" || end == "llä" || end == "lta" || end == "ltä" || 
                        end == "lle" || end == "na" || end == "nä" || end == "ksi" || end == "ine")
                    {
                        // Delete
                        word = word.Remove(word.Length - end.Length);
                        ending_removed_step_3 = true;
                    }
                    else if (end == "n")
                    {
                        // Delete
                        word = word.Remove(word.Length - end.Length);
                        ending_removed_step_3 = true;

                        // If preceded by LV or ie, delete the last vowel
                        if(word.EndsWith("ie") == true)
                        {
                            word = word.Remove(word.Length - 1);  
                        }
                        else
                        {
                            // Get the preceding two letters
                            string lastTwoLetters = word.Length > 1 ? word.Substring(word.Length - 2, 2) : "";

                            for (int j = 0; j < this.longVowels.Length; j++)
                            {
                                if (lastTwoLetters == this.longVowels[j])
                                {
                                    word = word.Remove(word.Length - 1);
                                    break;
                                }
                            }
                        }
                    }
                }

                // Break out from the loop if a ending has been removed
                if (ending_removed_step_3 == true)
                {
                    break;
                }     
            }
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            for (int i = 0; i < this.endingsStep4.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep4[i];

                if(strR2.EndsWith(end) == true)
                {
                    if(end == "mpi" || end == "mpa" || end == "mpä" || end == "mmi" || end == "mma" || end == "mmä")
                    {
                        // Delete if not preceded by po
                        if(word.EndsWith("po" + end) == false)
                        {
                            word = word.Remove(word.Length - end.Length);
                        }
                    }
                    else if (end == "impi" || end == "impa" || end == "impä" || end == "immi" || end == "imma" || 
                        end == "immä" || end == "eja" || end == "ejä")
                    {
                        // Delete
                        word = word.Remove(word.Length - end.Length);
                    }

                    // Break out from the loop
                    break;
                }
            }
            // **********************************************

            // **********************************************
            // Step 5
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            if (ending_removed_step_3 == true)
            {
                // If an ending was removed in step 3, delete a final i or j if in R1
                if(strR1.EndsWith("i") == true || strR1.EndsWith("j") == true)
                {
                    word = word.Remove(word.Length - 1);
                }
            }
            else
            {
                // Delete a final t in R1 if it follows a vowel
                if(strR1.EndsWith("t") == true)
                {
                    // Get the preceding char
                    char before = word.Length > 1 ? word[word.Length - 2] : '\0';

                    if(IsVowel(before) == true)
                    {
                        word = word.Remove(word.Length - 1);

                        strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
                        strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

                        // If a t is removed, delete a final mma or imma in R2, unless the mma is preceded by po
                        if(strR2.EndsWith("imma") == true)
                        {
                            word = word.Remove(word.Length - 4);
                        }
                        else if(strR2.EndsWith("mma") == true && word.EndsWith("poma") == false)
                        {
                            word = word.Remove(word.Length - 3);
                        }
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 6
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // a) If R1 ends LV delete the last letter
            for (int i = 0; i < this.longVowels.Length; i++)
            {
                if(strR1.EndsWith(this.longVowels[i]) == true )
                {
                    word = word.Remove(word.Length - 1);
                    break;
                }
            }

            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // b) If R1 ends cX, c a consonant and X one of: a   ä   e   i, delete the last letter
            char c = strR1.Length > 1 ? strR1[strR1.Length - 2] : '\0';
            if(c != '\0' && IsVowel(c) == false && (strR1.EndsWith("a") || strR1.EndsWith("ä") || strR1.EndsWith("e") || strR1.EndsWith("i")))
            {
                word = word.Remove(word.Length - 1);
            }

            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // c) If R1 ends oj or uj delete the last letter
            if(strR1.EndsWith("oj") == true || strR1.EndsWith("uj") == true)
            {
                word = word.Remove(word.Length - 1);
            }

            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";

            // d) If R1 ends jo delete the last letter
            if (strR1.EndsWith("jo") == true)
            {
                word = word.Remove(word.Length - 1);
            }

            // e) If the word ends with a double consonant followed by zero or more vowels, remove the last consonant (so eläkk -> eläk, aatonaatto -> aatonaato)
            Int32 startIndex = word.Length - 1;
            for (int i = startIndex; i > -1; i--)
            {
                // Try to find a double consonant
                if(i > 0 && word[i] == word[i-1] && IsVowel(word[i]) == false && IsVowel(word[i-1]) == false)
                {
                    // Get the count of characters that follows the double consonant
                    Int32 count = startIndex - i;
                    Int32 vowelCount = 0;

                    // Count the number of vowels
                    for(int j = i; j < word.Length; j++)
                    {
                        if(IsVowel(word[j]) == true)
                        {
                            vowelCount += 1;
                        }
                    }

                    // Remove the last consonant
                    if(count == vowelCount)
                    {
                        word = word.Remove(i, 1);
                    }

                    // Break out from the loop
                    break;
                }
            }
            // **********************************************

            // Return the word
            return word.ToLowerInvariant();

        } // End of the GetSteamWord method

        #endregion

        #region Helper methods

        /// <summary>
        /// Calculate indexes for R1 and R2
        /// </summary>
        /// <param name="characters">The char array to calculate indexes for</param>
        /// <returns>An int array with the r1 and r2 index</returns>
        private Int32[] CalculateR1R2(char[] characters)
        {
            // Create ints
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

            // Return the int array
            return new Int32[] { r1, r2 };

        } // End of the CalculateR1R2 method

        #endregion

    } // End of the class

} // End of the namespace