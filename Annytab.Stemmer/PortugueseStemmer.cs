using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip portuguese words to the steam
    /// This class is based on the portuguese stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/portuguese/stemmer.html
    /// </summary>
    public class PortugueseStemmer : Stemmer
    {
        #region Variables

        private string[] endingsStep1;
        private string[] endingsStep2;
        private string[] endingsStep4;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new portuguese stemmer with default properties
        /// </summary>
        public PortugueseStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'á', 'é', 'í', 'ó', 'ú', 'â', 'ê', 'ô' };
            this.endingsStep1 = new string[] { "imentos", "uciones", "amentos", "idades", "amento", "imento", "aço~es", "adores", 
                "ância", "amente", "ências", "adoras", "logías", "aça~o", "ución", "logía", "mente", "antes", "idade", "istas", 
                "ismos", "adora", "ência", "ador", "ezas", "ante", "iras", "ismo", "ivas", "osas", "osos", "icas", "icos", "ivos", 
                "ista", "ível", "ável", "iva", "ivo", "osa", "oso", "ira", "ica", "ico", "eza" };

            this.endingsStep2 = new string[] { "íssemos", "iríamos", "ássemos", "êssemos", "eríamos", "aríamos", "ísseis", "ésseis", 
                "ásseis", "éramos", "iríeis", "eríeis", "iremos", "eremos", "aremos", "aríeis", "ávamos", "íramos", "áramos", "era~o", 
                "issem", "essem", "armos", "íamos", "ara~o", "asses", "ira~o", "arias", "áveis", "ermos", "irmos", "assem", "ireis", 
                "íreis", "ereis", "éreis", "areis", "áreis", "iriam", "eriam", "ariam", "erias", "irias", "ardes", "erdes", "istes", 
                "estes", "astes", "isses", "esses", "irdes", "irás", "eres", "ares", "ires", "aram", "esse", "asse", "avas", "aria", 
                "eras", "erás", "aras", "arás", "iras", "íeis", "ados", "iria", "idas", "adas", "irei", "erei", "arei", "eria", "idos", 
                "ámos", "amos", "indo", "endo", "ando", "iste", "este", "emos", "imos", "aste", "irem", "erem", "arem", "isse", "avam", 
                "iram", "eram", "ada", "ais", "ará", "ida", "eis", "ias", "ara", "iam", "era", "irá", "ido", "ado", "ava", "ira", "erá", 
                "ou", "iu", "ia", "am", "ei", "eu", "ar", "er", "ir", "as", "es", "is", "em" };

            this.endingsStep4 = new string[] { "os", "a", "i", "o", "á", "í", "ó" };

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
            // Make sure that the word is in lower case letters
            word = word.ToLowerInvariant();

            // Replace ã and õ by a~ and o~ in the word
            word = word.Replace("ã", "a~");
            word = word.Replace("õ", "o~");

            // Get indexes for R1, R2 and RV
            Int32[] partIndexR = CalculateR1R2RV(word.ToCharArray());

            // Create strings
            string strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            string strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            string strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // **********************************************
            // Step 1
            // **********************************************
            bool ending_removed = false;
            for (int i = 0; i < this.endingsStep1.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep1[i];

                // Check if word ends with some of the predefined step 1 endings
                if (word.EndsWith(end) == true)
                {
                    if (end == "logías" || end == "logía")
                    {
                        // Replace with log if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "log";
                            ending_removed = true;
                        }
                    }
                    else if (end == "uciones" || end == "ución")
                    {
                        // Replace with u if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "u";
                            ending_removed = true;
                        }
                    }
                    else if (end == "ências" || end == "ência")
                    {
                        // Replace with ente if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ente";
                            ending_removed = true;
                        }
                    }
                    else if (end == "amente")
                    {
                        // Delete if in R1
                        if(strR1.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed = true;

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
                            ending_removed = true;

                            // If preceded by ante, avel or ível, delete if in R2
                            if (strR2.EndsWith("ante" + end) == true || strR2.EndsWith("avel" + end) == true ||
                                strR2.EndsWith("ível" + end) == true)
                            {
                                word = word.Remove(word.Length - 4);
                            }
                        }
                    }
                    else if (end == "idades" || end == "idade")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed = true;

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
                    else if (end == "ivos" || end == "ivas" || end == "ivo" || end == "iva")     
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed = true;

                            // If preceded by at, delete if in R2
                            if(strR2.EndsWith("at" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                        }
                    }
                    else if (end == "iras" || end == "ira")
                    {
                        // Replace with ir if in RV and preceded by e
                        if (strRV.EndsWith(end) == true && word.EndsWith("e" + end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ir";
                            ending_removed = true;
                        }
                    }
                    else
                    {
                        // Delete if in R2
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed = true;
                        }
                    }

                    // Break out from the loop if a ending has been removed
                    if (ending_removed == true)
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

            // Do step 2 if no ending was removed in step 1
            if(ending_removed == false)
            {
                for (int i = 0; i < this.endingsStep2.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep2[i];

                    // Check if the ending can be found
                    if (word.EndsWith(end) == true)
                    {
                        // Delete if in RV
                        if(strRV.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            ending_removed = true;
                            break;
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

            // Do step 3 if an ending was removed in step 1 or 2
            if(ending_removed == true)
            {
                if(strRV.EndsWith("i") == true && word.EndsWith("ci") == true)
                {
                    word = word.Remove(word.Length - 1);
                }
            }
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // Do step 4 if an ending not was removed in step 1 and 2
            if(ending_removed == false)
            {
                for (int i = 0; i < this.endingsStep4.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep4[i];

                    // Check if the ending can be found
                    if (word.EndsWith(end) == true)
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
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            // If the word ends with one of e, é, ê in RV, delete it
            if (strRV.EndsWith("e") == true || strRV.EndsWith("é") == true || strRV.EndsWith("ê") == true)
            {
                word = word = word.Remove(word.Length - 1);

                strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
                strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
                strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

                // If preceded by gu (or ci) with the u (or i) in RV, delete the u (or i)
                if((word.EndsWith("gu") == true && strRV.EndsWith("u") == true) || (word.EndsWith("ci") == true && strRV.EndsWith("i") == true))
                {
                    word = word = word.Remove(word.Length - 1);
                }
            }
            else if (word.EndsWith("ç") == true)
            {
                word = word = word.Remove(word.Length - 1);
                word += "c";
            }
            // **********************************************

            // Turn a~, o~ back into ã, õ
            word = word.Replace("a~", "ã");
            word = word.Replace("o~", "õ");

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