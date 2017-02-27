using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip italian words to the steam
    /// This class is based on the italian stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/italian/stemmer.html
    /// </summary>
    public class ItalianStemmer : Stemmer
    {
        #region Variables

        private char[] acuteAccents;
        private char[] graveAccents;
        private string[] endingsStep0;
        private string[] endingsStep1;
        private string[] endingsStep2;
        private string[] endingsStep3a;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new italian stemmer with default properties
        /// </summary>
        public ItalianStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'à', 'è', 'ì', 'ò', 'ù' };
            this.acuteAccents = new char[] { 'á', 'é', 'í', 'ó', 'ú' };
            this.graveAccents = new char[] { 'à', 'è', 'ì', 'ò', 'ù' };
            this.endingsStep0 = new string[] { "gliela", "gliele", "glieli", "glielo", "gliene", "mele", "celo", "celi", "cele", "cela", 
                "tene", "telo", "teli", "tele", "tela", "mene", "melo", "meli", "vene", "mela", "cene", "vela", "vele", "veli", "velo", 
                "sene", "gli", "vi", "ti", "si", "ne", "la", "lo", "li", "le", "ci", "mi" };
            this.endingsStep1 = new string[] { "atrice", "usione", "uzioni", "uzione", "azione", "amente", "imenti", "imento", "amenti", 
                "amento", "azioni", "atrici", "usioni", "abile", "abili", "ibile", "ibili", "logie", "logia", "atori", "atore", "mente", "anti", 
                "ante", "enza", "anze", "enze", "ichi", "iche", "anza", "ismi", "istì", "istè", "istà", "isti", "iste", "ista", "ismo", "ose", "osa", 
                "osi", "oso", "ità", "ivo", "ico", "iva", "ice", "ica", "ici", "ive", "ivi" };
            this.endingsStep2 = new string[] { "irebbero", "erebbero", "eresti", "assimo", "iscono", "iranno", "iresti", "eranno", "erebbe", "essero", 
                "iscano", "ireste", "assero", "eremmo", "irebbe", "ereste", "iremmo", "issero", "avamo", "avano", "avate", "ivate", "ivano", "ivamo", 
                "eremo", "iremo", "erete", "erono", "evamo", "irono", "evano", "irete", "evate", "arono", "endi", "irei", "irai", "ende", "immo", "iamo", 
                "Yamo", "erei", "enda", "ando", "emmo", "isca", "ammo", "asse", "isce", "isci", "erai", "isco", "assi", "endo", "erà", "irà", "evo", 
                "evi", "irò", "eva", "ete", "erò", "ita", "ite", "iti", "ito", "iva", "ivi", "ere", "ivo", "ano", "avo", "avi", "ono", "uta", "ute", 
                "ava", "ato", "ati", "ate", "ata", "uti", "uto", "ire", "are", "ar", "ir" };
            this.endingsStep3a = new string[] { "a", "e", "i", "o", "à", "è", "ì", "ò" };

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

            // Replace all acute accents by grave accents
            for(int i = 0; i < chars.Length; i++)
            {
                for(int j = 0; j < this.acuteAccents.Length; j++)
                {
                    if(chars[i] == this.acuteAccents[j])
                    {
                        chars[i] = this.graveAccents[j];
                    }
                }
            }

            // Put u after q, and u, i between vowels into upper case
            Int32 charCount = chars.Length - 1;
            for (int i = 1; i < chars.Length; i++)
            {
                if (i == charCount)
                {
                    if (chars[i] == 'u' && chars[i - 1] == 'q')
                    {
                        chars[i] = 'U';
                    } 
                }
                else if (chars[i] == 'u' && ((IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true) || chars[i - 1] == 'q'))
                {
                    chars[i] = 'U';
                }
                else if (chars[i] == 'i' && IsVowel(chars[i - 1]) == true && IsVowel(chars[i + 1]) == true)
                {
                    chars[i] = 'I';
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
                    if (strRV.EndsWith("ando" + end) == true || strRV.EndsWith("endo" + end) == true)
                    {
                        // (a) Delete the suffix
                        word = word.Remove(word.Length - end.Length);
                    }
                    else if (strRV.EndsWith("ar" + end) == true || strRV.EndsWith("er" + end) == true || strRV.EndsWith("ir" + end) == true)
                    {
                        // (b) Replace the suffix with e
                        word = word.Remove(word.Length - end.Length);
                        word += "e";
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

            bool ending_removed_step_1 = false;
            for (int i = 0; i < this.endingsStep1.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep1[i];

                // Check if word ends with some of the predefined step 1 endings
                if (word.EndsWith(end) == true)
                {
                    if (end == "azione" || end == "azioni" || end == "atore" || end == "atori")
                    {
                        // Delete if in R2
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by ic, delete if in R2
                            if(strR2.EndsWith("ic" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }
                    else if (end == "logia" || end == "logie")
                    {
                        // Replace with log if in R2
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "log";
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "uzione" || end == "uzioni" || end == "usione" || end == "usioni")
                    {
                        // Replace with u if in R2 
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "u";
                            ending_removed_step_1 = true;
                        } 
                    }
                    else if (end == "enza" || end == "enze")
                    {
                        // Replace with ente if in R2 
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ente";
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "amento" || end == "amenti" || end == "imento" || end == "imenti")
                    {
                        // Delete if in RV
                        if(strRV.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "amente")
                    {
                        // Delete if in R1
                        if (strR1.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by iv, delete if in R2 (and if further preceded by at, delete if in R2)
                            // If preceded by os, ic or abil, delete if in R2 
                            if (strR2.EndsWith("iv" + end) == true || strR2.EndsWith("os" + end) == true ||
                                strR2.EndsWith("ic" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);

                                if (strR2.EndsWith("ativ" + end) == true)
                                {
                                    word = word.Remove(word.Length - 2);
                                }
                            }
                            else if (strR2.EndsWith("abil" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }
                    else if (end == "ità")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by abil, ic or iv, delete if in R2
                            if(strR2.EndsWith("abil" + end) == true)
                            {
                                word = word.Remove(word.Length - 4);
                            }
                            else if (strR2.EndsWith("ic" + end) == true || strR2.EndsWith("iv" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }
                    else if (end == "ivo" || end == "ivi" || end == "iva" || end == "ive")
                    {
                        // Delete if in R2
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by at, delete if in R2 (and if further preceded by ic, delete if in R2)
                            if (strR2.EndsWith("at" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);

                                if(strR2.EndsWith("icat" + end) == true)
                                {
                                    word = word.Remove(word.Length - 2);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;
                        }
                    }

                    // Break out from the loop
                    break;
                }
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            if(ending_removed_step_1 == false)
            {
                for (int i = 0; i < this.endingsStep2.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep2[i];

                    // Delete if in RV
                    if (strRV.EndsWith(end) == true)
                    {
                        word = word.Remove(word.Length - end.Length);
                        break;
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 3
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // (a)
            for (int i = 0; i < this.endingsStep3a.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep3a[i];

                // Delete a final a, e, i, o, à, è, ì or ò if it is in RV
                if (strRV.EndsWith(end) == true)
                {
                    word = word.Remove(word.Length - end.Length);

                    // Delete a preceding i if it is in RV
                    if(strRV.EndsWith("i" + end) == true)
                    {
                        word = word.Remove(word.Length - 1);
                    }

                    break;
                }
            }

            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // (b)
            if (strRV.EndsWith("ch") == true || strRV.EndsWith("gh") == true)
            {
                word = word.Remove(word.Length - 1);
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
                        if(IsVowel(characters[i]) == false)
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