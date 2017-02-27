using System;

namespace Annytab.Stemmer
{
    /// <summary>
    /// This class is used to strip norwegian words to the steam
    /// This class is based on the norwegian stemming algorithm from Snowball
    /// http://snowball.tartarus.org/algorithms/norwegian/stemmer.html
    /// </summary>
    public class NorwegianStemmer : Stemmer
    {
        #region Variables

        private char[] valid_s_endings;
        private string[] endingsStep1;
        private string[] endingsStep2;
        private string[] endingsStep3;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new norwegian stemmer with default properties
        /// </summary>
        public NorwegianStemmer()
            : base()
        {
            // Set values for instance variables
            this.vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', 'æ', 'å', 'ø' };
            this.valid_s_endings = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'l', 'm', 'n', 'o', 'p', 'r', 't', 'v', 'y', 'z' }; // or k not preceded by a vowel
            this.endingsStep1 = new string[] { "hetenes", "hetens", "hetene", "heter", "heten", "endes", "edes", "enes", "ande", "ende", "ane", "ene", 
                "ens", "ers", "ets", "het", "ast", "ede", "en", "ar", "er", "et", "as", "es", "a", "e" };
            this.endingsStep2 = new string[] { "dt", "vt" };
            this.endingsStep3 = new string[] { "hetslov", "eleg", "elov", "slov", "elig", "lig", "els", "leg", "eig", "lov", "ig" };

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
            // Adjust the word to lower case
            word = word.ToLowerInvariant();

            // Get a char array of each letter in the word
            char[] characters = word.ToCharArray();

            // Create two parts for the word
            string part1 = "";
            string part2 = "";

            // Get the index of the first non-vowel after the first vowel (R1)
            Int32 firstNonVowel = CalculateR1(characters);

            // Split the word in two parts if a non-vowel not was found
            if (firstNonVowel < characters.Length)
            {
                // Get the first and the second part of the word
                part1 = word.Substring(0, firstNonVowel);
                part2 = word.Substring(firstNonVowel);
            }
            else
            {
                part1 = word;
            }

            // **********************************************
            // Step 1
            // **********************************************
            // Replace endings in part 2
            bool continue_step_1 = true;
            for (int i = 0; i < this.endingsStep1.Length; i++)
            {
                if (part2.EndsWith(this.endingsStep1[i]))
                {
                    // Delete the ending in part 2
                    part2 = part2.Remove(part2.Length - this.endingsStep1[i].Length);
                    continue_step_1 = false;
                    break;
                }
            }

            // Delete a s in the end if the s is preceded by a valid s-ending, 
            if (continue_step_1 == true && part2.EndsWith("s") == true)
            {
                // Create a full string of part1 and part2
                word = part1 + part2;

                // Get the preceding char before the s
                char precedingChar = word.Length > 1 ? word[word.Length - 2] : '\0';

                for (int i = 0; i < this.valid_s_endings.Length; i++)
                {
                    // Check if the preceding char is a valid s-ending
                    if (precedingChar == this.valid_s_endings[i])
                    {
                        // Delete the s
                        part2 = part2.Remove(part2.Length - 1);
                        continue_step_1 = false;
                        break;
                    }

                    // Check if the preceding char is k
                    if (precedingChar == 'k')
                    {
                        char charBeforeK = word.Length > 2 ? word[word.Length - 3] : '\0';

                        // Make sure that the char before k not is a vowel
                        if (IsVowel(charBeforeK) == false)
                        {
                            // Delete the s
                            part2 = part2.Remove(part2.Length - 1);
                            continue_step_1 = false;
                            break;
                        }
                    }
                }     
            }
            else
            {
                continue_step_1 = true;
            }

            // Check for an erte or an ert ending
            if(continue_step_1 == true)
            {
                if (part2.EndsWith("erte") == true)
                {
                    part2 = part2.Remove(part2.Length - 2);
                }
                else if (part2.EndsWith("ert") == true)
                {
                    part2 = part2.Remove(part2.Length - 1);
                }
            }
            // **********************************************

            // **********************************************
            // Step 2
            // **********************************************
            for (int i = 0; i < this.endingsStep2.Length; i++)
            {
                if (part2.EndsWith(this.endingsStep2[i]))
                {
                    // Delete the ending
                    part2 = part2.Remove(part2.Length - 1);
                    break;
                }
            }
            // **********************************************

            // **********************************************
            // Step 3
            // **********************************************
            for (int i = 0; i < this.endingsStep3.Length; i++)
            {
                if (part2.EndsWith(this.endingsStep3[i]))
                {
                    // Delete the ending
                    part2 = part2.Remove(part2.Length - this.endingsStep3[i].Length);
                    break;
                }
            }

            // Return the stripped word
            return part1 + part2;

        } // End of the GetSteamWord method

        #endregion

        #region Helper methods

        /// <summary>
        /// Calculate the R1 part for a word
        /// </summary>
        /// <param name="characters">An array of characters</param>
        /// <returns>An int with the r1 index</returns>
        public Int32 CalculateR1(char[] characters)
        {
            // Create the int array to return
            Int32 r1 = characters.Length;

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

            // Adjust R1
            if (r1 < 3)
            {
                r1 = 3;
            }

            // Return the int array
            return r1;

        } // End of the calculateR1R2 method

        #endregion

    } // End of the class

} // End of the namespace