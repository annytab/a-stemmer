using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip romanian words to the steam
    /// This class is based on the romanian stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/romanian/stemmer.html
    /// </summary>
    public class RomanianStemmer : Stemmer
    {
        #region Variables

        private string[] endingsStep0;
        private string[] endingsStep1;
        private string[] endingsStep2;
        private string[] endingsStep3;
        private string[] endingsStep4;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new romanian stemmer with default properties
        /// </summary>
        public RomanianStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'ă', 'â', 'e', 'i', 'î', 'o', 'u' };
            this.endingsStep0 = new string[] { "iilor", "aţia", "aţie", "atei", "elor", "ilor", "iile", "ului", "iua", "aua", "ile", "ele", "iei", "ii", "ea", "ul" };
            this.endingsStep1 = new string[] { "abilitate", "abilităţi", "ibilitate", "abilitati", "abilităi", "ivităţi", "icităţi", "ivitate", "ivitati", "icitati", 
                "icatori", "icitate", "itoare", "iţiune", "ivităi", "icator", "ătoare", "icităi", "atoare", "aţiune", "itori", "icală", "ativa", "icale", "icala", "itiva", 
                "icivă", "icivi", "icive", "iciva", "itive", "ative", "ativi", "ativă", "itivi", "atori", "itivă", "ători", "icali", "ativ", "ical", "iciv", 
                "ator", "ător", "itiv", "itor" };
            this.endingsStep2 = new string[] { "ibile", "abila", "abile", "abili", "abilă", "ibila", "itati", "ibili", "ibilă", "atori", "itate", "ităţi", "oasă", "oasa", 
                "iuni", "abil", "ităi", "isme", "işti", "iune", "ator", "antă", "anti", "ante", "anta", "ibil", "ista", "iste", "isti", "oase", "istă", "ica", "uta", "ive", 
                "ivi", "ivă", "ant", "oşi", "osi", "iva", "ata", "ism", "ist", "ică", "ici", "ice", "ată", "ate", "ite", "iti", "ită", "ita", "ati", "ute", "uti", "ută", "iv", 
                "os", "ic", "it", "ut", "at" };
            this.endingsStep3 = new string[] { "seserăţi", "âserăţi", "seserăm", "userăţi", "iserăţi", "aserăţi", "iserăm", "seseşi", "userăm", "serăţi", "âserăm", "seseră", 
                "aserăm", "urăţi", "irăţi", "serăm", "ârăţi", "aseşi", "âseră", "useşi", "aseră", "iseşi", "iseră", "useră", "ească", "âseşi", "arăţi", "sesem", "usem", "iaţi", 
                "indu", "âsem", "seră", "irăm", "ăşte", "ăşti", "sese", "isem", "urăm", "eaţi", "eşte", "eşti", "arăm", "asem", "ârăm", "seşi", "ează", "ându", "âse", "use", "ând", 
                "aţi", "ind", "ise", "eţi", "âre", "iţi", "ăsc", "ase", "âţi", "âră", "ere", "âşi", "esc", "iră", "işi", "ură", "sei", "uşi", "ară", "ire", "aşi", "ezi", "iau", 
                "are", "iai", "iam", "eau", "eze", "eai", "eam", "ăm", "em", "im", "âm", "âi", "ui", "ez", "ea", "au", "ai", "am", "se", "ia" };
            this.endingsStep4 = new string[] { "ie", "a", "e", "i", "ă"};

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

            // Create a char array
            char[] chars = word.ToCharArray();

            // First, i and u between vowels are put into upper case (so that they are treated as consonants)
            Int32 charCount = chars.Length - 1;
            for (int i = 1; i < charCount; i++)
            {
                if (chars[i] == 'i' && IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true)
                {
                    chars[i] = 'I';
                }
                else if (chars[i] == 'u' && IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true)
                {
                    chars[i] = 'U';
                }
            }

            // Get indexes for R1, R2 and RV
            Int32[] partIndexR = CalculateR1R2RV(chars);

            // Recreate the word
            word = new string(chars);

            // Create strings
            string strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            string strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            string strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // **********************************************
            // Step 0
            // **********************************************
            for (int i = 0; i < this.endingsStep0.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep0[i];
                
                // Check if word ends with some of the predefined step 0 endings
                if (word.EndsWith(end) == true)
                {
                    if (end == "ul" || end == "ului")
                    {
                        if(strR1.EndsWith(end) == true)
                        {
                            // Delete the ending
                            word = word.Remove(word.Length - end.Length);
                        }
                    }
                    else if (end == "aua")
                    {
                        if(strR1.EndsWith(end) == true)
                        {
                            // Replace with a
                            word = word.Remove(word.Length - end.Length);
                            word += "a";
                        }
                    }
                    else if (end == "ea" || end == "ele" || end == "elor")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with e
                            word = word.Remove(word.Length - end.Length);
                            word += "e";
                        }
                    }
                    else if (end == "ile")
                    {
                        if (strR1.EndsWith(end) == true && word.EndsWith("ab" + end) == false)
                        {
                            // Replace with i if not preceded by ab
                            word = word.Remove(word.Length - end.Length);
                            word += "i";
                        }
                    }
                    else if (end == "atei")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with at
                            word = word.Remove(word.Length - end.Length);
                            word += "at";
                        }
                    }
                    else if (end == "aţie" || end == "aţia")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with aţi
                            word = word.Remove(word.Length - end.Length);
                            word += "aţi";
                        }
                    }
                    else
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with i
                            word = word.Remove(word.Length - end.Length);
                            word += "i";
                        }
                    }

                    // Break out from the loop (the ending has been found)
                    break;
                }
            }
            // **********************************************

            // **********************************************
            // Step 1
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            bool ending_removed = false;
            for (int i = 0; i < this.endingsStep1.Length; i++)
            {
                // A boolean that indicates if the loop should be restarted
                bool restartLoop = false;

                // Get the ending
                string end = this.endingsStep1[i];

                // Check if word ends with some of the predefined step 1 endings
                if (word.EndsWith(end) == true)
                {
                    if (end == "abilitate" || end == "abilitati" || end == "abilităi" || end == "abilităţi")
                    {
                        if(strR1.EndsWith(end) == true)
                        {
                            // Replace with abil
                            word = word.Remove(word.Length - end.Length);
                            word += "abil";
                            ending_removed = true;
                            restartLoop = true;
                        }
                    }
                    else if (end == "ibilitate")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with ibil
                            word = word.Remove(word.Length - end.Length);
                            word += "ibil";
                            ending_removed = true;
                            restartLoop = true;
                        }
                    }
                    else if (end == "ivitate" || end == "ivitati" || end == "ivităi" || end == "ivităţi")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with iv
                            word = word.Remove(word.Length - end.Length);
                            word += "iv";
                            ending_removed = true;
                            restartLoop = true;
                        }
                    }
                    else if (end == "ativ" || end == "ativa" || end == "ative" || end == "ativi" || end == "ativă" || 
                        end == "aţiune" || end == "atoare" || end == "ator" || end == "atori" || end == "ătoare" || 
                        end == "ător" || end == "ători")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with at
                            word = word.Remove(word.Length - end.Length);
                            word += "at";
                            ending_removed = true;
                            restartLoop = true;
                        } 
                    }
                    else if (end == "itiv" || end == "itiva" || end == "itive" || end == "itivi" || end == "itivă" || 
                        end == "iţiune" || end == "itoare" || end == "itor" || end == "itori")
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with it
                            word = word.Remove(word.Length - end.Length);
                            word += "it";
                            ending_removed = true;
                            restartLoop = true;
                        }
                    }
                    else
                    {
                        if (strR1.EndsWith(end) == true)
                        {
                            // Replace with ic
                            word = word.Remove(word.Length - end.Length);
                            word += "ic";
                            ending_removed = true;
                            restartLoop = true;
                        }
                    }

                    // Check if we should restart the loop
                    if(restartLoop == true)
                    {
                        // Restart the loop
                        strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
                        strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
                        strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";
                        i = 0;
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            for (int i = 0; i < this.endingsStep2.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep2[i];

                // Check if the ending can be found
                if (word.EndsWith(end) == true)
                {
                    if (end == "iune" || end == "iuni")
                    {
                        // Delete if in R2 and preceded by ţ, and replace the ţ by t.
                        if(strR2.EndsWith(end) == true && word.EndsWith("ţ" + end) == true)
                        {
                            word = word.Remove(word.Length - 5);
                            word += "t";
                            ending_removed = true;
                        }
                    }
                    else if (end == "ism" || end == "isme" || end == "ist" || end == "ista" || end == "iste" ||
                        end == "isti" || end == "istă" || end == "işti")
                    {
                        if(strR2.EndsWith(end) == true)
                        {
                            // Replace with ist
                            word = word.Remove(word.Length - end.Length);
                            word += "ist";
                            ending_removed = true;
                        }
                    }
                    else
                    {
                        if (strR2.EndsWith(end) == true)
                        {
                            // Delete
                            word = word.Remove(word.Length - end.Length);
                            ending_removed = true;
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
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // Do step 3 if no suffix was removed either by step 1 or step 2
            if(ending_removed == false)
            {
                for (int i = 0; i < this.endingsStep3.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep3[i];

                    // Check if the ending can be found
                    if (word.EndsWith(end) == true)
                    {
                        if (end == "ăm" || end == "aţi" || end == "em" || end == "eţi" || end == "im" || end == "iţi" || 
                            end == "âm" || end == "âţi" || end == "seşi" || end == "serăm" || end == "serăţi" || end == "seră" || 
                            end == "sei" || end == "se" || end == "sesem" || end == "seseşi" || end == "sese" || end == "seserăm" || 
                            end == "seserăţi" || end == "seseră")
                        {
                            // Delete if in RV
                            if(strRV.EndsWith(end) == true)
                            {
                                word = word.Remove(word.Length - end.Length);
                                break;
                            }
                        }
                        else
                        {
                            // Delete if preceded in RV by a consonant or u
                            if(strRV.EndsWith(end) == true)
                            {
                                char before = strRV.Length > end.Length ? strRV[strRV.Length - end.Length - 1] : 'a';
                                if(IsVowel(before) == false || before == 'u')
                                {
                                    word = word.Remove(word.Length - end.Length);   
                                }

                                // Break out from the loop
                                break;
                            }
                        }
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            for (int i = 0; i < this.endingsStep4.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep4[i];

                if(word.EndsWith(end) == true)
                {
                    // Delete if in RV
                    if (strRV.EndsWith(end) == true)
                    {
                        word = word.Remove(word.Length - end.Length);
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
        /// Calculate the R1, R2 and RV part for a word
        /// </summary>
        /// <param name="characters">The char array to calculate indexes for</param>
        /// <returns>An int array with the r1, r2 and rV index</returns>
        private Int32[] CalculateR1R2RV(char[] characters)
        {
            // Create ints
            Int32 r1 = characters.Length;
            Int32 r2 = characters.Length;
            Int32 rV = characters.Length;

            // Create a word from the characters array
            string word = new string(characters);

            // Calculate RV
            // If the second letter is a consonant, RV is the region after the next following vowel, or if the first two letters are vowels, 
            // RV is the region after the next consonant, and otherwise (consonant-vowel case) RV is the region after the third letter. 
            // But RV is the end of the word if these positions cannot be found.
            if (characters.Length > 3)
            {
                if (IsVowel(characters[1]) == false)
                {
                    // Find the next vowel
                    for (int i = 2; i < characters.Length; i++)
                    {
                        if (IsVowel(characters[i]) == true)
                        {
                            rV = i + 1;
                            break;
                        }
                    }
                }
                else if (IsVowel(characters[0]) == true && IsVowel(characters[1]) == true)
                {
                    // Find the next consonant
                    for (int i = 2; i < characters.Length; i++)
                    {
                        if (IsVowel(characters[i]) == false)
                        {
                            rV = i + 1;
                            break;
                        }
                    }
                }
                else if (IsVowel(characters[0]) == false && IsVowel(characters[1]) == true)
                {
                    rV = 3;
                }
            }
            
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
            return new Int32[] { r1, r2, rV };

        } // End of the CalculateR1R2RV method

        #endregion

    } // End of the class

} // End of the namespace