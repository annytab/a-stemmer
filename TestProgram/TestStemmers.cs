using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Annytab;

namespace TestProgram
{
    [TestClass]
    public class TestStemmers
    {
        [TestMethod]
        public void TestSwedishStemmer()
        {
            // Create an array with words to test
            string[] words = new string[] {"klo", "kloaken", "klock", "klocka", "klockan", "klockans", "klockare", "klockaren", "klockarens",
                    "klockarfar", "klockarn", "klockarsonen", "klockas", "klockkedjan", "klocklikt", "klockor", "klockorna", "klok", "kloka", 
                    "klokare", "klokast", "klokaste", "kloke", "klokhet", "klokheten", "bilar", "bilarna", "bilen", "Bilens", "Semesterlagen", 
                    "Ansvarslöst", "Ansvarsfullt", "RAKARE", "JAPANSKA", "Mallen", "1990", "Och", "Utan"};

            // Create an array with correct steams
            string[] steams = new string[] {"klo", "kloak", "klock", "klock", "klockan", "klockan", "klock", "klock", "klock",
                    "klockarf", "klockarn", "klockarson", "klock", "klockkedjan", "klocklik", "klock", "klock", "klok", "klok", 
                    "klok", "klok", "klok", "klok", "klok", "klok", "bil", "bil", "bil", "Bil", "Semesterlag", 
                    "Ansvarslös", "Ansvarsfull", "RAK", "JAPANSK", "Mall", "1990", "Och", "Utan"};

            // Create a swedish stemmer
            Stemmer stemmer = new SwedishStemmer();

            // Test the stemmer
            for(int i = 0; i < words.Length; i++)
            {
                Assert.AreEqual(steams[i], stemmer.GetSteamWord(words[i]));
            }

        } // End of the TestSwedishStemmer method

        [TestMethod]
        public void TestEnglishStemmer()
        {
            // Create an array with words to test
            string[] words = new string[] { "consign", "consigned", "consigning", "consignment", "consist", "consisted", "consistency", "consistent", "consistently", 
                "consisting", "consists", "consolation", "consolations", "consolatory", "console", "consoled", "consoles", "consolidate", "consolidated", "consolidating", 
                "consoling", "consolingly", "consols", "consonant", "consort", "consorted", "consorting", "conspicuous", "conspicuously", 
                "conspiracy", "conspirator", "conspirators", "conspire", "conspired", "conspiring", "constable", "constables", "constance", "constancy", "constant",
                "knack", "knackeries", "knacks", "knag", "knave", "knaves", "knavish", "kneaded", "kneading", "knee", "kneel", "kneeled", "kneeling", "kneels", 
                "knees", "knell", "knelt", "templates", "cry", "Sky", "absolutely"};

            // Create an array with correct steams
            string[] steams = new string[] { "consign", "consign", "consign", "consign", "consist", "consist", "consist", "consist", "consist", 
                "consist", "consist", "consol", "consol", "consol", "consol", "consol", "consol", "consolid", "consolid", "consolid", 
                "consol", "consol", "consol", "conson", "consort", "consort", "consort", "conspicu", "conspicu", 
                "conspir", "conspir", "conspir", "conspir", "conspir", "conspir", "constabl", "constabl", "constanc", "constanc", "constant",
                "knack", "knackeri", "knack", "knag", "knav", "knav", "knavish", "knead", "knead", "knee", "kneel", "kneel", "kneel", "kneel", 
                "knee", "knell", "knelt", "templat", "cri", "sky", "absolut"};

            // Create a english stemmer
            Stemmer stemmer = new EnglishStemmer();

            // Test the stemmer
            for (int i = 0; i < words.Length; i++)
            {
                Assert.AreEqual(steams[i], stemmer.GetSteamWord(words[i]));
            }

        } // End of the TestEnglishStemmer method

    } // End of the class

} // End of the namespace
