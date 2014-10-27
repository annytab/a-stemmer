using System;
using Annytab;

namespace TestProgram
{
    /// <summary>
    /// This class is used to test stemmers
    /// </summary>
    public class StemmerTest
    {
        static int Main(string[] args)
        {
            string[] words = new string[] {"klo", "kloaken", "klock", "klocka", "klockan", "klockans", "klockare", "klockaren", "klockarens",
                            "klockarfar", "klockarn", "klockarsonen", "klockas", "klockkedjan", "klocklikt", "klockor", "klockorna", "klok", "kloka", 
                            "klokare", "klokast", "klokaste", "kloke", "klokhet", "klokheten", "bilar", "bilarna", "bilen", "Bilens", "Semesterlagen", 
                            "Ansvarslöst", "Ansvarsfullt", "RAKARE", "JAPANSKA", "Mallen", "1990", "Och", "Utan"};

            //string[] words = new string[] { "consign", "consigned", "consigning", "consignment", "consist", "consisted", "consistency", "consistent", "consistently", 
            //    "consisting", "consists", "consolation", "consolations", "consolatory", "console", "consoled", "consoles", "consolidate", "consolidated", "consolidating", 
            //    "consoling", "consolingly", "consols", "consonant", "consort", "consorted", "consorting", "conspicuous", "conspicuously", 
            //    "conspiracy", "conspirator", "conspirators", "conspire", "conspired", "conspiring", "constable", "constables", "constance", "constancy", "constant",
            //    "knack", "knackeries", "knacks", "knag", "knave", "knaves", "knavish", "kneaded", "kneading", "knee", "kneel", "kneeled", "kneeling", "kneels", 
            //    "knees", "knell", "knelt", "templates", "cry", "Sky", "absolutely"};

            //string[] words = new string[] { "Beautiful"};

            Stemmer stemmer = new SwedishStemmer();

            for (int i = 0; i < words.Length; i++)
            {
                string strippedWord = stemmer.GetSteamWord(words[i]);
                Console.WriteLine(words[i] + "\t\t\t\t" + strippedWord);
            }

            // Return exit code
            return 0;

        } // End of the main method

    } // End of the class

} // End of the namespace
