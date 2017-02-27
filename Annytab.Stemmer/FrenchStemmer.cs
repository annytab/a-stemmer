using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip french words to the steam
    /// This class is based on the french stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/french/stemmer.html
    /// </summary>
    public class FrenchStemmer : Stemmer
    {
        #region Variables

        private string[] endingsStep1;
        private string[] endingsStep2a;
        private string[] endingsStep2b;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new french stemmer with default properties
        /// </summary>
        public FrenchStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', 'â', 'à', 'ë', 'é', 'ê', 'è', 'ï', 'î', 'ô', 'û', 'ù' };
            this.endingsStep1 = new string[] { "issements", "issement", "atrices", "atrice", "ateurs", "ations", "logies", "usions", 
                "utions", "ements", "amment", "emment", "ances", "iqUes", "ismes", "ables", "istes", "ateur", "ation", "logie", "usion", 
                "ution", "ences", "ement", "euses", "ments", "ance", "iqUe", "isme", "able", "iste", "ence", "ités", "ives", "eaux", 
                "euse", "ment", "eux", "ité", "ive", "ifs", "aux", "if" };
            this.endingsStep2a = new string[] { "issantes", "issaIent", "issante", "issions", "issants", "iraIent", "issant", "irions", 
                "issiez", "issons", "issais", "issait", "issent", "iriez", "isses", "irais", "iront", "irait", "issez", "irons", 
                "irent", "irez", "iras", "îmes", "îtes", "isse", "irai", "ira", "ies", "ît", "is", "ir", "ie", "it", "i" };
            this.endingsStep2b = new string[] { "eraIent", "assions", "assiez", "erions", "assent", "erait", "èrent", "erons", 
                "antes", "eriez", "asses", "eront", "erais", "aIent", "asse", "ants", "ante", "erai", "eras", "âtes", "erez", 
                "âmes", "ions", "ait", "ant", "era", "iez", "ées", "ais", "ez", "ât", "ai", "er", "as", "és", "ée", "é", "a" };

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

            // Create a char array that can be used over an over again
            char[] chars = word.ToCharArray();

            // Put into upper case u or i preceded and followed by a vowel, and y preceded or followed by a vowel. u after q is also put into upper case.
            Int32 charCount = chars.Length - 1;
            for (int i = 0; i < chars.Length; i++)
            {
                if(i == 0)
                {
                    if (chars.Length > 1 && chars[i] == 'y' && IsVowel(chars[i + 1]) == true)
                    {
                        chars[i] = 'Y';
                    }
                }
                else if (i == charCount)
                {
                    if (chars[i] == 'u' && chars[i - 1] == 'q')
                    {
                        chars[i] = 'U';
                    }
                }
                else if(chars[i] == 'y' && ((IsVowel(chars[i - 1]) == true) || IsVowel(chars[i + 1]) == true))
                {
                    chars[i] = 'Y';
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

            // A boolean that indicates if the word has been altered
            bool wordIsAltered = false;

            // **********************************************
            // Step 1
            // **********************************************
            bool doStep2 = true;
            for (int i = 0; i < this.endingsStep1.Length; i++)
            {
                // Get the ending
                string end = this.endingsStep1[i];

                // Check if word ends with some of the predefined step 1 endings
                if (word.EndsWith(end))
                {
                    if (end == "ance" || end == "iqUe" || end == "isme" || end == "able" || end == "iste" || end == "eux" || end == "ances" || 
                        end == "iqUes" || end == "ismes" || end == "ables" || end == "istes")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "atrice" || end == "ateur" || end == "ation" || end == "atrices" || end == "ateurs" || end == "ations")
                    {
                        // Delete if in R2
                        // If preceded by ic, delete if in R2, else replace by iqU
                        if(strR2.EndsWith(end) == true)
                        {
                            // Remove the ending
                            word = word.Remove(word.Length - end.Length);

                            // Do further processing
                            if(strR2.EndsWith("ic" + end) == true)
                            {
                                word = word.Remove(word.Length - 2);
                            }
                            else if (word.EndsWith("ic") == true)
                            {
                                word = word.Remove(word.Length - 2);
                                word += "iqU";
                            }

                            // Break out from the loop and indicate that step 2 not should be done
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if(end == "logie" || end == "logies")
                    {
                        // Replace with log if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "log";
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "usion" || end == "ution" || end == "usions" || end == "utions")
                    {
                        // Replace with u if in R2
                        if (strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "u";
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "ences" || end == "ence")
                    {
                        // Replace with ent if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ent";
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "ements" || end == "ement")
                    {
                        // Delete if in RV
                        if(strRV.EndsWith(end) == true)
                        {
                            // Delete if in RV
                            word = word.Remove(word.Length - end.Length);

                            if (strR2.EndsWith("iv" + end) == true) // If preceded by iv, delete if in R2 (and if further preceded by at, delete if in R2)
                            {
                                word = word.Remove(word.Length - 2);

                                if(strR2.EndsWith("ativ" + end) == true)
                                {
                                    word = word.Remove(word.Length - 2);
                                }
                            }
                            else if(word.EndsWith("eus") == true) // If preceded by eus, delete if in R2, else replace by eux if in R1
                            {
                                if(strR2.EndsWith("eus" + end) == true)
                                {
                                    word = word.Remove(word.Length - 3);
                                }
                                else if (strR1.EndsWith("eus" + end) == true)
                                {
                                    word = word.Remove(word.Length - 3);
                                    word += "eux";
                                }
                            }
                            else if (strR2.EndsWith("abl" + end) == true || strR2.EndsWith("iqU" + end) == true) // If preceded by abl or iqU, delete if in R2
                            {
                                word = word.Remove(word.Length - 3);
                            }
                            else if (strRV.EndsWith("ièr" + end) == true || strRV.EndsWith("Ièr" + end) == true) // If preceded by ièr or Ièr, replace by i if in RV
                            {
                                word = word.Remove(word.Length - 3);
                                word += "i";
                            }

                            // Break out from the loop and indicate that step 2 not should be done
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "ités" || end == "ité")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);

                            if (word.EndsWith("abil") == true) // If preceded by abil, delete if in R2, else replace by abl
                            {
                                word = word.Remove(word.Length - 4);

                                if(strR2.EndsWith("abil" + end) == false)
                                {
                                    word += "abl";
                                }
                            }
                            else if (word.EndsWith("ic") == true) // If preceded by ic, delete if in R2, else replace by iqU
                            {
                                word = word.Remove(word.Length - 2);

                                if (strR2.EndsWith("ic" + end) == false)
                                {
                                    word += "iqU";
                                }
                            }
                            else if (strR2.EndsWith("iv" + end) == true) // If preceded by iv, delete if in R2   
                            {
                                word = word.Remove(word.Length - 2);
                            }

                            // Break out from the loop and indicate that step 2 not should be done
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "ives" || end == "ifs" || end == "ive" || end == "if")
                    {
                        // Delete if in R2
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);

                            if (word.EndsWith("at") == true) // If preceded by at, delete if in R2 (and if further preceded by ic, delete if in R2, else replace by iqU)
                            {
                                if(strR2.EndsWith("at" + end) == true)
                                {
                                    word = word.Remove(word.Length - 2);
                                }
                                
                                if(word.EndsWith("ic") == true)
                                {
                                    word = word.Remove(word.Length - 2);

                                    if (strR2.EndsWith("icat" + end) == false)
                                    {
                                        word += "iqU";
                                    }
                                }      
                            }

                            // Break out from the loop and indicate that step 2 not should be done
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "eaux")
                    {
                        // Replace with eau
                        word = word.Remove(word.Length - 1);
                        doStep2 = false;
                        wordIsAltered = true;
                    }
                    else if (end == "aux")
                    {
                        // Replace with al if in R1
                        if(strR1.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "al";
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "euses" || end == "euse")
                    {
                        // Delete if in R2, else replace by eux if in R1
                        if(strR2.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                        else if(strR1.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "eux";
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "issements" || end == "issement")
                    {
                        // Delete if in R1 and preceded by a non-vowel
                        if(word.Length > end.Length && strR1.EndsWith(end) == true && IsVowel(word[word.Length - end.Length - 1]) == false)
                        {
                            word = word.Remove(word.Length - end.Length);
                            doStep2 = false;
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "amment")
                    {
                        // Replace with ant if in RV
                        if(strRV.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ant";
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "emment")
                    {
                        // Replace with ent if in RV
                        if (strRV.EndsWith(end) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            word += "ent";
                            wordIsAltered = true;
                        }
                    }
                    else if (end == "ments" || end == "ment")
                    {
                        // Delete if preceded by a vowel in RV
                        if (strRV.Length > end.Length && strRV.EndsWith(end) == true && IsVowel(strRV[strRV.Length - end.Length - 1]) == true)
                        {
                            word = word.Remove(word.Length - end.Length);
                            wordIsAltered = true;                  
                        }
                    }

                    // Break out from the loop (the ending has been found)
                    break;
                }
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************

            // Recreate strings
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            if(doStep2 == true)
            {
                // Reset the word is altered boolean
                wordIsAltered = false;

                for (int i = 0; i < this.endingsStep2a.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep2a[i];

                    // Delete if found and preceded by a non-vowel in RV
                    if(strRV.Length > end.Length && strRV.EndsWith(end) && IsVowel(strRV[strRV.Length - end.Length - 1]) == false)
                    {
                        word = word.Remove(word.Length - end.Length);
                        doStep2 = false;
                        wordIsAltered = true;
                        break;
                    }
                }
            }

            if(doStep2 == true)
            {
                // Reset the word is altered boolean
                wordIsAltered = false;

                for (int i = 0; i < this.endingsStep2b.Length; i++)
                {
                    // Get the ending
                    string end = this.endingsStep2b[i];

                    // Check if the RV string ends with the ending
                    if(strRV.EndsWith(end) == true)
                    {
                        if(end == "ions")
                        {
                            if(strR2.EndsWith(end) == true)
                            {
                                word = word.Remove(word.Length - end.Length);
                                wordIsAltered = true;
                            }
                        }
                        else if(end == "é" || end == "ée" || end == "ées" || end == "és" || end == "èrent" || end == "er" || end == "era" || 
                            end == "erai" || end == "eraIent" || end == "erais" || end == "erait" || end == "eras" || end == "erez" || 
                            end == "eriez" || end == "erions" || end == "erons" || end == "eront" || end == "ez" || end == "iez")
                        {
                            word = word.Remove(word.Length - end.Length);
                            wordIsAltered = true;
                        }
                        else if (end == "âmes" || end == "ât" || end == "âtes" || end == "a" || end == "ai" || end == "aIent" || 
                            end == "ais" || end == "ait" || end == "ant" || end == "ante" || end == "antes" || end == "ants" || 
                            end == "as" || end == "asse" || end == "assent" || end == "asses" || end == "assiez" || end == "assions")
                        {
                            // Delete
                            word = word.Remove(word.Length - end.Length);

                            // Delete the preceding e
                            if (strRV.Length > end.Length && strRV[strRV.Length - end.Length - 1] == 'e')
                            {
                                word = word.Remove(word.Length - 1);
                            }

                            // The word has been altered
                            wordIsAltered = true;
                        }

                        // Break out from the loop (the ending has been found)
                        break;
                    }
                }
            }
            // **********************************************

            // **********************************************
            // Step 3
            // **********************************************

            // Recreate strings
            strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
            strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
            strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

            if(wordIsAltered == true)
            {
                // Get the final character
                char finalChar = word.Length > 0 ? word[word.Length - 1] : '\0';

                if(finalChar == 'Y')
                {
                    word = word.Remove(word.Length - 1);
                    word += "i";
                }
                else if(finalChar == 'ç')
                {
                    word = word.Remove(word.Length - 1);
                    word += "c";
                }

            }
            // **********************************************

            // **********************************************
            // Step 4
            // **********************************************
            if (wordIsAltered == false)
            {
                // Get the final character
                char finalChar = word.Length > 0 ? word[word.Length - 1] : '\0';
                char precedingChar = word.Length > 1 ? word[word.Length - 2] : '\0';

                // If the word ends s, not preceded by a, i, o, u, è or s, delete it. 
                if (finalChar == 's' && precedingChar != 'a' && precedingChar != 'i' && precedingChar != 'o'
                    && precedingChar != 'u' && precedingChar != 'è' && precedingChar != 's' && precedingChar != '\0')
                {
                    word = word.Remove(word.Length - 1);
                }

                // Recreate strings
                strR1 = partIndexR[0] < word.Length ? word.Substring(partIndexR[0]) : "";
                strR2 = partIndexR[1] < word.Length ? word.Substring(partIndexR[1]) : "";
                strRV = partIndexR[2] < word.Length ? word.Substring(partIndexR[2]) : "";

                if(strRV.EndsWith("Ière") == true || strRV.EndsWith("ière") == true)
                {
                    // Replace with i
                    word = word.Remove(word.Length - 4);
                    word += "i";
                }
                else if (strRV.EndsWith("Ier") == true || strRV.EndsWith("ier") == true)
                {
                    // Replace with i
                    word = word.Remove(word.Length - 3);
                    word += "i";
                }
                else if ((strRV.EndsWith("sion") == true || strRV.EndsWith("tion") == true) && strR2.EndsWith("ion") == true)
                {
                    // Delete ion
                    word = word.Remove(word.Length - 3);
                }
                else if(strRV.EndsWith("e") == true)
                {
                    word = word.Remove(word.Length - 1);
                }
                else if (strRV.EndsWith("ë") == true && word.EndsWith("guë") == true)
                {
                    word = word.Remove(word.Length - 1);
                }
            }
            // **********************************************

            // **********************************************
            // Step 5
            // **********************************************
            // If the word ends enn, onn, ett, ell or eill, delete the last letter
            if (word.EndsWith("eill") == true || word.EndsWith("ell") == true || word.EndsWith("ett") == true || word.EndsWith("onn") == true || word.EndsWith("enn") == true)
            {
                word = word.Remove(word.Length - 1);
            }

            // **********************************************

            // **********************************************
            // Step 6
            // **********************************************
            // If the words ends é or è followed by at least one non-vowel, remove the accent from the e.
            chars = word.ToCharArray();
            Int32 startIndex = chars.Length - 1;
            Int32 numberOfNonVowels = 0;
            Int32 steps = 0;
            for (int i = startIndex; i >= 0; i--)
            { 
                if ((chars[i] == 'é' || chars[i] == 'è') && numberOfNonVowels > 0 && numberOfNonVowels == steps)
                {
                    chars[i] = 'e';
                    word = new string(chars);
                    break;
                }

                if(IsVowel(chars[i]) == false)
                {
                    numberOfNonVowels += 1;
                }

                steps++;
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
            if (characters.Length > 3 && ((IsVowel(characters[0]) == true && IsVowel(characters[1]) == true) 
                || word.StartsWith("par") == true || word.StartsWith("col") == true || word.StartsWith("tap") == true))
            {
                    rV = 3;
            }
            else
            {
                for (int i = 1; i < characters.Length; i++)
                {
                    if (IsVowel(characters[i]) == true)
                    {
                        // Set the rV index
                        rV = i + 1;
                        break;
                    }
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