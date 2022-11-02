using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Translator : System.Web.UI.Page
    {
        /// Hash function divisor for modulo operator
        private const int hashDivisor = 97;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataContext dataContext = new DataContext("server=localhost;user=root;database=xx;port=3306;password=******");

                // Initialize Rojak HashTable
                Dictionary<string, List<string>>[] rojakHashtable = new Dictionary<string, List<string>>[26];
                for (int i = 0; i < rojakHashtable.Length; i++)
                {
                    rojakHashtable[i] = new Dictionary<string, List<string>>();
                }

                // Query database for all dictionary entries
                var dictionaryEntries = dataContext.DictionaryEntries.ToList();

                // Sort dictionary entries into rojakHashtable by hashing
                foreach (var dictionaryEntry in dictionaryEntries)
                {
                    string currentTranslation = dictionaryEntry.Translation.WordValue;
                    string currentSlang = dictionaryEntry.Slang.WordValue;

                    // Get hash index (a = 0, b = 1, c = 2 ...)
                    int index = (int)(currentTranslation[0]) % hashDivisor;

                    // If rojakHashtable already contains this slang, add the translation to existing translation list of the slang
                    if (rojakHashtable[index].ContainsKey(currentTranslation))
                    {
                        rojakHashtable[index][currentTranslation].Add(currentSlang);
                    }
                    // Else, create a new translation list before adding
                    else
                    {
                        List<string> slangList = new List<string>();
                        slangList.Add(currentSlang);
                        rojakHashtable[index].Add(currentTranslation, slangList);
                    }
                }

                // Initialize session variables
                Session["RojakHashtable"] = rojakHashtable;
                Session["SavedTranslations"] = new List<SavedTranslation>();
            }
        }

        protected void BtnTranslate_Click(Object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext("server=localhost;user=root;database=xx;port=3306;password=******");

            // Clear output
            txtOutput.InnerText = "";

            // Get trimmed input text
            string inputText = txtInput.Value.Trim();
            // Split input text into individual words
            var inputWords = !String.IsNullOrWhiteSpace(inputText) ? inputText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries) : null;

            // List to store output words
            List<string> outputWords = new List<string>();

            // Perform translation
            if (inputWords != null && inputWords.Count() > 0)
            {
                var rojakHashtable = Session["RojakHashtable"] as Dictionary<string, List<string>>[];
                Random randomizer = new Random();

                foreach (string word in inputWords)
                {
                    // Get hash index
                    int index = (int)(Char.ToLower(word[0])) % hashDivisor;

                    // If rojakHashtable contains the input word, add its corresponding slang to output words
                    if (index < rojakHashtable.Length && rojakHashtable[index].ContainsKey(word.ToLower()))
                    {
                        var matchedSlangList = rojakHashtable[index][word.ToLower()];

                        string matchedSlang = matchedSlangList.ElementAt(randomizer.Next(0, matchedSlangList.Count));

                        /* Rules for dealing with letter casing:
                         * 1. If first letter of input word is UPPERCASE, convert first letter of output word to UPPERCASE
                         * 2. If input word is full UPPERCASE, convert output word to full UPPERCASE
                         * 3. If input word is full LOWERCASE, simply add the output word in its original form (LOWERCASE)
                         */
                        outputWords.Add(word.All(c => Char.IsUpper(c)) ? matchedSlang.ToUpper() :
                                        Char.IsUpper(word[0]) ? Char.ToUpper(matchedSlang[0]) + matchedSlang.Substring(1) : matchedSlang);
                    }
                    // Else, simply add the original input word back into output words
                    else
                    {
                        outputWords.Add(word);
                    }
                }

                // Compose sentence
                for (int i = 0; i < outputWords.Count; i++)
                {
                    txtOutput.InnerText += outputWords.ElementAt(i);
                    txtOutput.InnerText += (i + 1) < outputWords.Count ? " " : "";
                }

                // Save translation into session-based translation history
                List<SavedTranslation> sessionTranslations = Session["SavedTranslations"] as List<SavedTranslation>;

                sessionTranslations.Add(new SavedTranslation()
                {
                    Input = inputText,
                    Output = txtOutput.InnerText,
                    CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == "System"),
                    CreationDate = DateTime.Now
                });

                //translationHistory.InnerText = "";
                //int n = 1;
                //foreach (var translation in sessionTranslations)
                //{
                //    translationHistory.InnerHtml += n + ". Input: " + translation.Input + "<br />"
                //        + "Output: " + translation.Output + "<br />";
                //    n++;
                //}
            }
        }

        protected void BtnSave_Click(Object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext("server=localhost;user=root;database=xx;port=3306;password=******");

            string inputText = txtInput.InnerText;
            string outputText = txtOutput.InnerText;

            if (String.IsNullOrWhiteSpace(inputText) || String.IsNullOrWhiteSpace(outputText))
            {
                // Show error message
            }
            else
            {
                SavedTranslation newSavedTranslation = new SavedTranslation()
                {
                    Input = inputText,
                    Output = outputText,
                    CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == "System"),
                    CreationDate = DateTime.Now
                };

                dataContext.SavedTranslations.Add(newSavedTranslation);

                dataContext.SaveChanges();
            }
        }
    }
}