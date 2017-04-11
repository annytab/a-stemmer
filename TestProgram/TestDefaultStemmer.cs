using Microsoft.VisualStudio.TestTools.UnitTesting;
using Annytab.Stemmer;

namespace TestProgram
{
    [TestClass]
    public class TestDefaultStemmer
    {
        [TestMethod]
        public void ExtensiveTestDefaultStemmer()
        {
            // Create an array with words to test
            string[] words = new string[] { "consign", "consigned", "consigning", "consignment", "consist", "consisted", "consistency", "consistent", "consistently", 
                "consisting", "consists", "consolation", "consolations", "consolatory", "console", "consoled", "consoles", "consolidate", "consolidated", "consolidating", 
                "consoling", "consolingly", "consols", "consonant", "consort", "consorted", "consorting", "conspicuous", "conspicuously", 
                "conspiracy", "conspirator", "conspirators", "conspire", "conspired", "conspiring", "constable", "constables", "constance", "constancy", "constant",
                "knack", "knackeries", "knacks", "knag", "knave", "knaves", "knavish", "kneaded", "kneading", "knee", "kneel", "kneeled", "kneeling", "kneels", 
                "knees", "knell", "knelt", "templates", "cry", "Sky", "absolutely" };

            // Create an array with correct steams
            string[] steams = new string[] { "consign", "consigned", "consigning", "consignment", "consist", "consisted", "consistency", "consistent", "consistently", 
                "consisting", "consists", "consolation", "consolations", "consolatory", "console", "consoled", "consoles", "consolidate", "consolidated", "consolidating", 
                "consoling", "consolingly", "consols", "consonant", "consort", "consorted", "consorting", "conspicuous", "conspicuously", 
                "conspiracy", "conspirator", "conspirators", "conspire", "conspired", "conspiring", "constable", "constables", "constance", "constancy", "constant",
                "knack", "knackeries", "knacks", "knag", "knave", "knaves", "knavish", "kneaded", "kneading", "knee", "kneel", "kneeled", "kneeling", "kneels", 
                "knees", "knell", "knelt", "templates", "cry", "Sky", "absolutely" };

            // Create a default stemmer
            IStemmer stemmer = new DefaultStemmer();

            // Test the stemmer
            for (int i = 0; i < words.Length; i++)
            {
                Assert.AreEqual(steams[i], stemmer.GetSteamWord(words[i]));
            }

        } // End of the ExtensiveTest method

    } // End of the class

} // End of the namespace