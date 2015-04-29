using System;
using System.Collections.Generic;

namespace Annytab
{
    /// <summary>
    /// This class is used to strip spanish words to the steam
    /// This class is based on the spanish stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/spanish/stemmer.html
    /// </summary>
    public class SpanishStemmer : Stemmer
    {
        #region Variables

        private string[] endingsStep0;
        private string[] endingsStep1;
        private string[] endingsStep2a;
        private string[] endingsStep2b;
        private string[] endingsStep3;
        private string[] acuteAccents;
        private string[] acuteAccentsReplacements;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new spanish stemmer with default properties
        /// </summary>
        public SpanishStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'á', 'é', 'í', 'ó', 'ú', 'ü' };
            this.endingsStep0 = new string[] { "selas", "selos", "sela", "selo", "nos", "los", "les", "las", "se", "le", "lo", "me", "la" };
            this.endingsStep1 = new string[] { "amientos", "imientos", "imiento", "amiento", "uciones", "aciones", "adoras", "adores", "idades", 
                "amente", "encias", "ancias", "logías", "ución", "encia", "ancia", "antes", "mente", "istas", "ibles", "ables", "ación", "anzas", "adora", 
                "ismos", "logía", "ivos", "ante", "osas", "osos", "ismo", "icas", "idad", "ista", "icos", "ible", "anza", "able", "ivas", "ador", "osa", 
                "oso", "iva", "ivo", "ica", "ico" };
            this.endingsStep2a = new string[] { "yamos", "yendo", "yeron", "yais", "yes", "yas", "yan", "yen", "ye", "yó", "ya", "yo" };
            this.endingsStep2b = new string[] { "iésemos", "iéramos", "iríamos", "eríamos", "aríamos", "aremos", "ábamos", "iríais", "aríais", 
                "ásemos", "isteis", "asteis", "ieseis", "eremos", "ierais", "áramos", "iremos", "eríais", "arías", "aréis", "aseis", "arais", 
                "íamos", "abais", "iesen", "erían", "ieses", "ieran", "ieras", "eréis", "arían", "irían", "irías", "iréis", "iendo", "ieron", 
                "erías", "irás", "aron", "irán", "asen", "ando", "aran", "aría", "aban", "abas", "iste", "idos", "iese", "arás", "adas", "idas", 
                "arán", "iera", "ería", "aras", "erás", "erán", "ases", "amos", "imos", "emos", "aste", "íais", "iría", "ados", "iré", "ido", "ían", 
                "ado", "aré", "ase", "ará", "áis", "eré", "ara", "erá", "ida", "ada", "aba", "éis", "irá", "ías", "an", "es", "id", "ed", "ad", "ía", 
                "ís", "ar", "er", "ir", "as", "en", "ió" };
            this.endingsStep3 = new string[] { "os", "a", "o", "á", "í", "ó", "e", "é" };
            this.acuteAccents = new string[] { "á", "é", "í", "ó", "ú" };
            this.acuteAccentsReplacements = new string[] { "a", "e", "i", "o", "u" };

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

            // Get indexes for R1, R2 and RV
            Int32[] partIndexR = CalculateR1R2RV(word.ToCharArray());

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
                    if (strRV.EndsWith("iéndo" + end) == true)
                    {
                        // Deletion is followed by removing the acute accent (for example, haciéndola -> haciendo)
                        word = word.Remove(word.Length - end.Length - 5);
                        word += "iendo";
                    }
                    else if (strRV.EndsWith("ándo" + end) == true)
                    {
                        // Deletion is followed by removing the acute accent (for example, haciéndola -> haciendo)
                        word = word.Remove(word.Length - end.Length - 4);
                        word += "ando";
                    }
                    else if (strRV.EndsWith("ár" + end) == true)
                    {
                        // Deletion is followed by removing the acute accent (for example, haciéndola -> haciendo)
                        word = word.Remove(word.Length - end.Length - 2);
                        word += "ar";
                    }
                    else if (strRV.EndsWith("ér" + end) == true)
                    {
                        // Deletion is followed by removing the acute accent (for example, haciéndola -> haciendo)
                        word = word.Remove(word.Length - end.Length - 2);
                        word += "er";
                    }
                    else if (strRV.EndsWith("ír" + end) == true)
                    {
                        // Deletion is followed by removing the acute accent (for example, haciéndola -> haciendo)
                        word = word.Remove(word.Length - end.Length - 2);
                        word += "ir";
                    }
                    else if (strRV.EndsWith("iendo" + end) == true || strRV.EndsWith("ando" + end) == true ||
                        strRV.EndsWith("ar" + end) == true || strRV.EndsWith("er" + end) == true ||
                        strRV.EndsWith("ir" + end) == true)
                    {
                        // Delete the ending
                        word = word.Remove(word.Length - end.Length);
                    }
                    else if (strRV.EndsWith("yendo" + end) == true && word.EndsWith("uyendo" + end) == true)
                    {
                        // Delete the ending
                        word = word.Remove(word.Length - end.Length);
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
                    if (end == "anza" || end == "anzas" || end == "ico" || end == "ica" || end == "icos" || end == "icas" || 
                        end == "ismo" || end == "ismos" || end == "able" || end == "ables" || end == "ible" || end == "ibles" || 
                        end == "ista" || end == "istas" || end == "oso" || end == "osa" || end == "osos" || end == "osas" || 
                        end == "amiento" || end == "amientos" || end == "imiento" || end == "imientos")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "adora" || end == "ador" || end == "ación" || end == "adoras" || end == "adores" || 
                        end == "aciones" || end == "ante" || end == "antes" || end == "ancia" || end == "ancias")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
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
                    else if (end == "logía" || end == "logías")
                    {
                        // Replace with log if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "log";
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "ución" || end == "uciones")
                    {
                        // Replace with u if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "u";
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "encia" || end == "encias")
                    {
                        // Replace with ente if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ente";
                            ending_removed_step_1 = true;
                        }
                    }
                    else if (end == "amente")
                    {
                        // Delete if in R1
                        if(strR1.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by iv, delete if in R2 (and if further preceded by at, delete if in R2)
                            // If preceded by os, ic or ad, delete if in R2
                            if(strR2.EndsWith("iv" + end) == true || strR2.EndsWith("os" + end) == true || 
                                strR2.EndsWith("ic" + end) == true || strR2.EndsWith("ad" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);

                                if(strR2.EndsWith("ativ" + end) == true)
                                {
                                    word = word.Remove(word.Length - 2);
                                }
                            }
                        }
                    }
                    else if (end == "mente")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by ante, able or ible, delete if in R2
                            if (strR2.EndsWith("ante" + end) == true || strR2.EndsWith("able" + end) == true ||
                                strR2.EndsWith("ible" + end) == true)
                            {
                                word = word.Remove(word.Length - 4);
                            }
                        }
                    }
                    else if (end == "idades" || end == "idad")
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
                    else if (end == "ivos" || end == "ivas" || end == "iva" || end == "ivo")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed_step_1 = true;

                            // If preceded by at, delete if in R2
                            if(strR2.EndsWith("at" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }

                    // Break out from the loop if a ending has been removed
                    if (ending_removed_step_1 == true)
                    {
                        break;
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

            // (a)
            bool do_step_2b = true;
            if(ending_removed_step_1 == false)
            {
                for (int i = 0; i < this.endingsStep2a.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep2a[i];

                    // Check if the ending can be found
                    if (word.EndsWith(end) == true)
                    {
                        // Delete if in RV and preceded by u
                        if(strRV.EndsWith(end) == true && word.EndsWith("u" + end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            do_step_2b = false;
                            break;
                        }
                    }
                }
            }

            // (b)
            if (ending_removed_step_1 == false && do_step_2b == true)
            {
                for (int i = 0; i < this.endingsStep2b.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep2b[i];

                    // Check if the ending can be found
                    if (word.EndsWith(end) == true)
                    {
                        if (end == "en" || end == "es" || end == "éis" || end == "emos")
                        {
                            // Delete, and if preceded by gu delete the u (the gu need not be in RV)
                            if (strRV.EndsWith(end) == true)
                            {
                                word = word.Remove(word.Length - end.Length);

                                // If preceded by gu delete the u (the gu need not be in RV)
                                if (word.EndsWith("gu") == true)
                                {
                                    word = word.Remove(word.Length - 1);
                                }

                                break;
                            }
                        }
                        else
                        {
                            // Delete if in RV
                            if (strRV.EndsWith(end) == true)
                            {
                                word = word.Remove(word.Length - end.Length);
                                break;
                            }
                        }
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

            for (int i = 0; i < this.endingsStep3.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep3[i];

                // Check if the ending can be found
                if (word.EndsWith(end) == true)
                {
                    if(end == "é" || end == "e")
                    {
                        // Delete if in RV
                        if(strRV.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);

                            // If preceded by gu with the u in RV delete the u
                            if(strRV.EndsWith("u" + end) == true && word.EndsWith("gu") == true)
                            {
                                word = word.Remove(word.Length - 1);
                            }

                            break;
                        }
                    }
                    else
                    {
                        // Delete if in RV
                        if(strRV.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            break;
                        }
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            // Remove acute accents
            for (int i = 0; i < this.acuteAccents.Length; i++)
            {
                word = word.Replace(this.acuteAccents[i], this.acuteAccentsReplacements[i]);
            }

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