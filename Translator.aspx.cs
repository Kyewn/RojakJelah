using edu.stanford.nlp.ling;
using edu.stanford.nlp.pipeline;
using java.util;
using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Translator : System.Web.UI.Page
    {
        /// Hash function divisor for modulo operator
        private const int HashDivisor = 97;

        /// Session variables
        private const string StrRojakHashtable = "RojakHashtable";
        private const string StrCoreNlpPipeline = "CoreNlpPipeline";
        private const string StrTranslationHistory = "TranslationHistory";
        private const string StrMostRecentTranslation = "MostRecentTranslation";
        private const string StrTranslationHistoryCount = "TranslationHistoryCount";

        /// Control attributes
        private const string AttrTranslationId = "data-translationid";
        private const string AttrUseSession = "data-use-session";

        /// FontAwesome icon class strings
        private const string IconExclamation = "fa-solid fa-circle-exclamation";
        private const string IconAccessDenied = "fa-solid fa-ban";
        private const string IconFloppyDisk = "fa-solid fa-floppy-disk";
        private const string IconCheck = "fa-regular fa-circle-check";
        private const string IconSolidBookmark = "fa-solid fa-bookmark";
        private const string IconRegularBookmark = "fa-regular fa-bookmark";

        private class Token
        {
            public string Word;
            public string Tag;
            public string Shape;
        }

        private enum TokenShape
        {
            AllCaps,
            Proper,
            Normal
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide nav header
            HtmlControl navHeader = Master.FindControl("navHeader") as HtmlControl;
            navHeader.Style.Add("visibility", "hidden");

            // Reset state of any controls that may persist in postback (this is so stupid)
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "auto");
            notification.Style.Add("display", "none");
            mdlSavedTranslations.Style.Add("display", "none");
            mdlTranslationHistory.Style.Add("display", "none");
            mdlReport.Style.Add("display", "none");

            if (!IsPostBack || Session[StrRojakHashtable] == null)
            {
                Dictionary<string, List<string>>[] rojakHashtable = InitializeRojakHashtable();
                StanfordCoreNLP coreNlpPipeline = SetupCoreNlpPipeline();

                PrepopulateControls();
                ManageSession(rojakHashtable, coreNlpPipeline);
            }

            PopulateSavedTranslations();
            PopulateTranslationHistory();
        }

        /// <summary>
        /// Initializes a hash table to store Rojak dictionary entries.
        /// </summary>
        /// <returns>Hashtable representing rojak dictionary.</returns>
        protected Dictionary<string, List<string>>[] InitializeRojakHashtable()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Initialize array
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
                int index = currentTranslation[0] % HashDivisor;

                // If rojakHashtable already contains this translation, add the slang to existing slang list of the slang
                if (rojakHashtable[index].ContainsKey(currentTranslation))
                {
                    rojakHashtable[index][currentTranslation].Add(currentSlang);
                }
                // Else, create a new slang list before adding
                else
                {
                    List<string> slangList = new List<string>();
                    slangList.Add(currentSlang);
                    rojakHashtable[index].Add(currentTranslation, slangList);
                }
            }

            return rojakHashtable;
        }

        /// <summary>
        /// Sets up StanfordCoreNLP pipeline with required model directories, annotators, and options.
        /// </summary>
        /// <returns></returns>
        protected StanfordCoreNLP SetupCoreNlpPipeline()
        {
            // Root directory & models directory
            var jarRoot = @"C:\Users\wztho\Downloads"; /// only need to change this line
            var modelsDirectory = jarRoot + @"\stanford-corenlp-4.5.0-models/edu/stanford\nlp";

            // Model file paths
            var postaggerModel = modelsDirectory + @"\models\pos-tagger\english-left3words-distsim.tagger";

            // Set up pipeline properties
            Properties properties = new Properties();
            // Set the list of annotators to run
            properties.setProperty("annotators", "tokenize, pos");
            // Customize the annotator options
            properties.setProperty("tokenize.options", "splitHyphenated=false,americanize=false");
            properties.setProperty("pos.model", postaggerModel);

            // Build pipeline
            StanfordCoreNLP pipeline = new StanfordCoreNLP(properties);

            return pipeline;
        }

        /// <summary>
        /// Manages session variables.
        /// </summary>
        /// <param name="rojakHashtable">Hash table storing Rojak dictionary entries.</param>
        protected void ManageSession(Dictionary<string, List<string>>[] rojakHashtable, StanfordCoreNLP pipeline)
        {
            // Store rojakHashtable in session
            Session[StrRojakHashtable] = rojakHashtable;

            // Store Stanford CoreNLP pipeline in session
            Session[StrCoreNlpPipeline] = pipeline;

            // Retrieve translation history from session
            var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;
            if (translationHistory == null)
            {
                Session[StrTranslationHistory] = new List<SavedTranslation>();
                Session[StrTranslationHistoryCount] = 0;
            }
        }

        protected void BtnTranslate_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Reset save icon
            iconSave.Attributes.Add("class", IconRegularBookmark);
            // Clear output
            txtOutput.InnerText = "";

            #region Input Sanitization
            // Get trimmed input text
            string inputText = txtInput.Value.Trim();
            #endregion

            #region Tokenization & POS Tagging
            // Create CoreDocument and use the pipeline to annotate it
            StanfordCoreNLP pipeline = Session[StrCoreNlpPipeline] as StanfordCoreNLP;
            CoreDocument coreDoc = pipeline.processToCoreDocument(inputText);
            pipeline.annotate(coreDoc);
            
            // Split input into sentences
            var inputSentenceList = coreDoc.sentences().toArray();
            var tokenizedSentenceList = new List<List<Token>>();

            if ((inputSentenceList != null) && (inputSentenceList.Length > 0))
            {
                // Tokenize and POS tag each sentence
                foreach (CoreSentence sentence in inputSentenceList)
                {
                    var tokenizedSentence = new List<Token>();
                    var tokenList = sentence.tokens().toArray();

                    foreach (CoreLabel token in tokenList)
                    {
                        string word = token.word();
                        string tag = token.tag();

                        tokenizedSentence.Add(new Token()
                        {
                            Word = word,
                            Tag = tag,
                            /* Rules for dealing with letter casing:
                             * - If first letter of input word is UPPERCASE, convert first letter of output word to UPPERCASE
                             * - If input word is full UPPERCASE, convert output word to full UPPERCASE
                             * - If input word is full LOWERCASE, simply add the output word in its original form (LOWERCASE)
                             */
                            Shape = word.All(c => Char.IsUpper(c)) ? TokenShape.AllCaps.ToString() :
                                        Char.IsUpper(word[0]) ? TokenShape.Proper.ToString() :
                                        TokenShape.Normal.ToString()
                        });
                    }

                    tokenizedSentenceList.Add(tokenizedSentence);
                }
            }
            #endregion

            #region Tokenization Corrector
            var correctedSentenceList = new List<List<Token>>();

            if ((tokenizedSentenceList != null) && (tokenizedSentenceList.Count > 0))
            {
                // Correct the tokens in each sentence
                foreach (List<Token> tokenizedSentence in tokenizedSentenceList)
                {
                    int i = 0;
                    var correctedSentence = new List<Token>();

                    foreach (Token token in tokenizedSentence)
                    {
                        string correctedWord, correctedTag, correctedShape;

                        /* Check for these tags to be corrected
                         * - POS (possessive, e.g. Joe's mama)
                         * - VBZ (short form verb, e.g. Joe's going to school)
                         * - VBP (short form verb present, e.g. 've / 're / 'm / 's)
                         * - RB (short form adverb, e.g. n't)
                         * - PRP (personal pronoun, e.g. 's)
                         * - MD (modal, e.g. 'll)
                         * - VBD (short form verb past, e.g. 'd)
                         */
                        if ((token.Tag == "POS") || (token.Tag == "VBZ" && token.Word[0] == '\'') || (token.Tag == "VBP" && token.Word[0] == '\'') ||
                            (token.Tag == "RB" && (token.Word == "n't" || token.Word == "nt")) || (token.Tag == "PRP" && token.Word == "'s") ||
                            (token.Tag == "MD" && token.Word[0] == '\'') || (token.Tag == "VBD" && token.Word == "'d"))
                        {
                            Token previousToken = (i > 0) ? tokenizedSentence.ElementAt(i - 1) : tokenizedSentence.ElementAt(i);

                            correctedWord = (i > 0) ? previousToken.Word + token.Word : token.Word;
                            correctedTag = previousToken.Tag;
                            correctedShape = previousToken.Shape;

                            correctedSentence.RemoveAt(correctedSentence.Count - 1);
                        }
                        else
                        {
                            correctedWord = token.Word;
                            correctedTag = token.Tag;
                            correctedShape = token.Shape;
                        }

                        correctedSentence.Add(new Token()
                        {
                            Word = correctedWord,
                            Tag = correctedTag,
                            Shape = correctedShape
                        });

                        i++;
                    }

                    correctedSentenceList.Add(correctedSentence);
                }
            }
            #endregion

            #region Morphological Analyzing & Translation
            List<List<string>> translatedSentenceList = new List<List<string>>();

            if ((correctedSentenceList != null) && (correctedSentenceList.Count() > 0))
            {
                // Retrieve rojakHashtable, initialize Random object for random slang output
                var rojakHashtable = Session[StrRojakHashtable] as Dictionary<string, List<string>>[];
                System.Random randomizer = new System.Random();

                // Determine ngram (number of words of the longest phrase in the database)
                HashSet<string> translationList = new HashSet<string>();
                var dictionaryEntryList = dataContext.DictionaryEntries.ToList();
                dictionaryEntryList.ForEach(x => translationList.Add(x.Translation.WordValue));

                int ngram = 0;
                foreach (string translation in translationList)
                {
                    int wordCount = translation.Split(' ').Count();
                    ngram = (ngram < wordCount) ? wordCount : ngram;
                }

                // Translate each sentence
                foreach (List<Token> correctedSentence in correctedSentenceList)
                {
                    var translatedSentence = new List<string>();
                    int resultedIncrement = 0;

                    // Iterate through the tokens in each sentence
                    for (int i = 0; i < correctedSentence.Count; i += resultedIncrement)
                    {
                        int lowerBound = i;
                        int upperBound = (correctedSentence.Count < i + ngram) ? correctedSentence.Count : (i + ngram);
                        bool isTranslated = false;

                        /* From the current lowerBound, iterate until the upperBound searching for a matching slang word/phrase.
                         * upperBound is decremented each iteration, such that the length of word/phrase to search is decreased, until a translation is made.
                         * The result of this loop will only be 1 word or phrase.
                         */
                        while (!isTranslated)
                        {
                            string word = "";
                            int increment = 0;

                            for (int j = lowerBound; j < upperBound; j++)
                            {
                                word += correctedSentence.ElementAt(j).Word;
                                word += (j < upperBound - 1) ? " " : "";

                                increment++;
                            }

                            // Get hash index
                            int hashIndex = Char.ToLower(word[0]) % HashDivisor;

                            // If rojakHashtable contains the input word, add its corresponding slang into current output sentence
                            if ((hashIndex < rojakHashtable.Length) && (rojakHashtable[hashIndex].ContainsKey(word.ToLower())))
                            {
                                var matchedSlangList = rojakHashtable[hashIndex][word.ToLower()];

                                string matchedSlang = matchedSlangList.ElementAt(randomizer.Next(0, matchedSlangList.Count));
                                string shape = correctedSentence.ElementAt(i).Shape;

                                translatedSentence.Add(shape == TokenShape.AllCaps.ToString() ? matchedSlang.ToUpper() :
                                    shape == TokenShape.Proper.ToString() ? Char.ToUpper(matchedSlang[0]) + matchedSlang.Substring(1) :
                                    matchedSlang);

                                isTranslated = true;
                                resultedIncrement = increment;
                            }
                            else
                            {
                                // If the single word is reached with still no matched slang, add the word into current output sentence
                                if (increment == 1)
                                {
                                    translatedSentence.Add(word);

                                    isTranslated = true;
                                    resultedIncrement = increment;
                                }
                            }

                            upperBound--;
                        }
                    }

                    translatedSentenceList.Add(translatedSentence);
                }
            }
            #endregion

            #region Sentence Composer
            string outputText = "";

            if ((translatedSentenceList != null) && (translatedSentenceList.Count > 0))
            {
                int i = 0;

                foreach (List<string> translatedSentence in translatedSentenceList)
                {
                    int j = 0;

                    foreach (string word in translatedSentence)
                    {
                        outputText += word;

                        if ((j + 1) < translatedSentence.Count)
                        {
                            string nextWord = translatedSentence.ElementAt(j + 1);

                            outputText += (nextWord.Length == 1 && Char.IsPunctuation(nextWord[0])) ? "" : " ";
                        }

                        j++;
                    }

                    outputText += (i < translatedSentenceList.Count) ? " " : "";

                    i++;
                }
            }

            // Display output
            txtOutput.InnerText = outputText;
            #endregion

            // Save translation into session (most recent translation & translation history)
            List<SavedTranslation> translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;
            int translationId = (int) Session[StrTranslationHistoryCount];
            translationId++;
            Session[StrTranslationHistoryCount] = translationId;

            SavedTranslation mostRecentTranslation = new SavedTranslation()
            {
                Id = translationId,
                Input = inputText,
                Output = outputText,
                CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name) ?? null,
                CreationDate = DateTime.Now
            };

            Session[StrMostRecentTranslation] = mostRecentTranslation;
            translationHistory.Add(mostRecentTranslation);
        }

        protected void LnkSaveTranslation_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ShowNotification(IconAccessDenied, "Access denied", "You need to be logged in to use this feature.", true);
            }
            else
            {
                DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                string inputText = txtInput.InnerText.Trim();
                string outputText = txtOutput.InnerText.Trim();

                // Check if there is data to save
                if (String.IsNullOrWhiteSpace(inputText) || String.IsNullOrWhiteSpace(outputText))
                {
                    // Show error notification
                    ShowNotification(IconExclamation, "Unable to save", "Source message and translation output cannot be empty.", true);
                }
                else
                {
                    // Compare current input text with most recently translated input text
                    string notificationMessage;
                    var mostRecentTranslation = Session[StrMostRecentTranslation] as SavedTranslation;

                    if (!String.Equals(mostRecentTranslation.Input, inputText))
                    {
                        notificationMessage = "The source message has been changed since last translation, so the most recently translated source message is saved instead.";
                    }
                    else
                    {
                        notificationMessage = "This translation has been saved successfully.";
                    }

                    // Save translation into database
                    SavedTranslation newSavedTranslation = new SavedTranslation()
                    {
                        Input = mostRecentTranslation.Input,
                        Output = mostRecentTranslation.Output,
                        CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name),
                        CreationDate = DateTime.Now
                    };

                    dataContext.SavedTranslations.Add(newSavedTranslation);
                    dataContext.SaveChanges();

                    // Update save icon
                    iconSave.Attributes.Add("class", IconSolidBookmark);

                    // Show success notification
                    ShowNotification(IconFloppyDisk, "Save success", notificationMessage, false);
                }
            }
        }

        /// <summary>
        /// Populates and shows the saved translations modal with translations saved in the database.
        /// </summary>
        protected void LnkViewSavedTranslations_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ShowNotification(IconAccessDenied, "Access denied", "You need to be logged in to use this feature.", true);
            }
            else
            {
                PopulateSavedTranslations();
                ShowModal(mdlSavedTranslations);
            }
        }

        /// <summary>
        /// Populates and shows the translation history modal with translations saved in session.
        /// </summary>
        protected void LnkViewTranslationHistory_Click(Object sender, EventArgs e)
        {
            PopulateTranslationHistory();
            ShowModal(mdlTranslationHistory);
        }

        /// <summary>
        /// Opens report modal.
        /// </summary>
        protected void LnkReport_Click(object sender, EventArgs e)
        {
            ShowModal(mdlReport);
        }

        /// <summary>
        /// Deletes a specified SavedTranslation in either "Saved Translations" or "Translation History"
        /// </summary>
        protected void BtnDeleteTranslation_Click(object sender, EventArgs e)
        {
            try
            {
                // Get saved translation ID
                Button sourceButton = sender as Button;
                int translationId = Int32.Parse(sourceButton.Attributes[AttrTranslationId]);

                // Determine if it is saved translation or translation history
                if (sourceButton.Attributes[AttrUseSession] == null)
                {
                    // Saved translation
                    if (User.Identity.IsAuthenticated)
                    {
                        DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
                        dataContext.SavedTranslations.Remove(dataContext.SavedTranslations.SingleOrDefault(x => x.Id == translationId));
                        dataContext.SaveChanges();

                        LnkViewSavedTranslations_Click(sender, e);

                        ShowNotification(IconCheck, "Delete succcess", "The translation has been deleted successfully.", false);
                    }
                    else
                    {
                        ShowNotification(IconAccessDenied, "Access denied", "You do not have permission to delete translations.", true);
                    }
                }
                else
                {
                    // Translation history
                    var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;
                    translationHistory.Remove(translationHistory.SingleOrDefault(x => x.Id == translationId));

                    LnkViewTranslationHistory_Click(sender, e);

                    ShowNotification(IconCheck, "Delete succcess", "The translation has been deleted successfully.", false);
                }
            }
            catch (Exception ex)
            {
                ShowNotification();
            }
        }

        /// <summary>
        /// Generates and lets user download text file of the user's saved translations
        /// </summary>
        protected void LnkDownloadSavedTranslations_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Retrieve the user's saved translations
            string username = User.Identity.Name;
            User currentUser = dataContext.Users.SingleOrDefault(x => x.Username == username);
            var savedTranslationList = dataContext.SavedTranslations.ToList().FindAll(x => x.CreatedBy.Id == currentUser.Id);

            // File header and details
            string fileHeader = "RojakJelah Saved Translations\n\n";
            string fileDetails = $"User: {username}\nDownload date: {DateTime.Now}\n\n";

            // File contents
            int count = 1;
            string fileContents = "";

            foreach (var savedTranslation in savedTranslationList)
            {
                fileContents += $"[{count}.]\n" +
                    $"Input: {savedTranslation.Input}\n" +
                    $"Output: {savedTranslation.Output}\n" +
                    $"Date: {savedTranslation.CreationDate}\n\n";

                count++;
            }

            // Compile overall file text
            string fileText = fileHeader + fileDetails + fileContents;

            // Create text file and output for user to download
            Response.Clear();
            Response.AddHeader("content-disposition", $"attachment; filename=rojakjelah-{username}-savedtranslations.txt");
            Response.AddHeader("content-type", "text/plain");

            using (StreamWriter writer = new StreamWriter(Response.OutputStream))
            {
                writer.WriteLine(fileText);
            }

            Response.End();
        }

        /// <summary>
        /// Generates and lets user download text file of the user's translation history
        /// </summary>
        protected void LnkDownloadTranslationHistory_Click(object sender, EventArgs e)
        {
            // Retrieve the user's translation history
            string username = User.Identity.IsAuthenticated ? User.Identity.Name : "Unknown";
            var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;

            // File header and details
            string fileHeader = "RojakJelah Translation History\n\n";
            string fileDetails = $"User: {username}\nDownload date: {DateTime.Now}\n\n";

            // File contents
            int count = 1;
            string fileContents = "";

            foreach (var savedTranslation in translationHistory)
            {
                fileContents += $"[{count}.]\n" +
                    $"Input: {savedTranslation.Input}\n" +
                    $"Output: {savedTranslation.Output}\n" +
                    $"Date: {savedTranslation.CreationDate}\n\n";

                count++;
            }

            // Compile overall file text
            string fileText = fileHeader + fileDetails + fileContents;

            // Create text file and output for user to download
            Response.Clear();
            Response.AddHeader("content-disposition", $"attachment; filename=rojakjelah-{username}-translationhistory.txt");
            Response.AddHeader("content-type", "text/plain");

            using (StreamWriter writer = new StreamWriter(Response.OutputStream))
            {
                writer.WriteLine(fileText);
            }

            Response.End();
        }

        protected void LnkClearTranslationHistory_Click(object sender, EventArgs e)
        {
            Session[StrTranslationHistory] = new List<SavedTranslation>();
            Session[StrTranslationHistoryCount] = 0;

            PopulateTranslationHistory();
            ShowModal(mdlTranslationHistory);
        }

        /// <summary>
        /// Prepopulates page controls with options.
        /// </summary>
        protected void PrepopulateControls()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Issue Category dropddown
            var reportCategoryList = dataContext.ReportCategories.OrderBy(x => x.Id).ToList();

            foreach (var reportCategory in reportCategoryList)
            {
                ddlReportCategory.Items.Add(new ListItem(reportCategory.Name, reportCategory.Id.ToString()));
            }
        }

        protected void PopulateSavedTranslations()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            var savedTranslationList = dataContext.SavedTranslations.Where(x => x.CreatedBy.Username == User.Identity.Name).ToList();

            divSavedTranslationsModalBody.Controls.Clear();

            if (savedTranslationList.Count == 0)
            {
                FillEmptyModal(divSavedTranslationsModalBody);
                lnkDownloadSavedTranslations.Style.Add("display", "none");
                savedTranslationFooterText.InnerText = "";
            }
            else
            {
                int count = 0;

                foreach (var savedTranslation in savedTranslationList)
                {
                    count++;
                    AddModalItem(divSavedTranslationsModalBody, savedTranslation, false);
                }

                lnkDownloadSavedTranslations.Style.Add("display", "block");
                savedTranslationFooterText.InnerText = savedTranslationList.Count + " translation" + (savedTranslationList.Count > 1 ? "s " : " ") + "found";
            }
        }

        protected void PopulateTranslationHistory()
        {
            var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;

            divTranslationHistoryModalBody.Controls.Clear();

            if ((translationHistory != null ) && (translationHistory.Count == 0))
            {
                FillEmptyModal(divTranslationHistoryModalBody);
                lnkDownloadTranslationHistory.Style.Add("display", "none");
                lnkClearTranslationHistory.Style.Add("display", "none");
                translationHistoryFooterText.InnerText = "";
            }
            else
            {
                int count = 0;

                foreach (var savedTranslation in translationHistory)
                {
                    count++;
                    AddModalItem(divTranslationHistoryModalBody, savedTranslation, true);
                }

                lnkDownloadTranslationHistory.Style.Add("display", "block");
                lnkClearTranslationHistory.Style.Add("display", "block");
                translationHistoryFooterText.InnerText = translationHistory.Count + " translation" + (translationHistory.Count > 1 ? "s " : " ") + "found";
            }
        }

        /// <summary>
        /// Shows the specified modal.
        /// </summary>
        /// <param name="modal">Modal to show.</param>
        protected void ShowModal(HtmlGenericControl modal)
        {
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "hidden");
            modal.Style.Add("display", "flex");
        }

        protected void FillEmptyModal(HtmlGenericControl modal)
        {
            LiteralControl modalEmpty = new LiteralControl($@"
                <div class='modal-empty'>
                    <i class='fa-solid fa-file-excel'></i>
                    <h1>No translations found</h1>
                </div>
            ");

            modal.Controls.Add(modalEmpty);
        }

        protected void AddModalItem(HtmlGenericControl modal, SavedTranslation savedTranslation, bool useSession)
        {
            // Modal item container
            Panel modalItem = new Panel();
            modalItem.CssClass = "modal-item";

            // Modal item content
            LiteralControl modalItemContent = new LiteralControl($@"
                <div class='modal-item-content'>
                    <h2 class='modal-item-title'>{savedTranslation.Input}</h2>
                    <div class='modal-item-text'>
                        <h3 class='text-title'>Translation</h3>
                        <h3 class='text-content'>{savedTranslation.Output}</h3>
                    </div>
                    <small>Date: {savedTranslation.CreationDate}</small>
                </div>
            ");

            // Modal item controls
            Panel modalItemControls = new Panel();
            modalItemControls.CssClass = "modal-item-controls";

            Button btnDelete = new Button();
            btnDelete.ID = (!useSession ? "btnDeleteSaved" : "btnDeleteHistory") + savedTranslation.Id;
            btnDelete.ClientIDMode = ClientIDMode.Static;
            btnDelete.CssClass = "modal-item-btn";
            btnDelete.Text = "Delete";
            btnDelete.Attributes.Add(AttrTranslationId, savedTranslation.Id.ToString());
            btnDelete.Click += new EventHandler(BtnDeleteTranslation_Click);
            btnDelete.OnClientClick = $"confirmDelete(event, '{btnDelete.ID}');";

            if (useSession)
            {
                btnDelete.Attributes.Add(AttrUseSession, "true");
            }

            modalItemControls.Controls.Add(btnDelete);

            // Add item content and item controls to item container
            modalItem.Controls.Add(modalItemContent);
            modalItem.Controls.Add(modalItemControls);

            // Add item to modal
            modal.Controls.Add(modalItem);
        }

        /// <summary>
        /// Displays status notification popup.
        /// </summary>
        /// <param name="icon">Icon of notification.</param>
        /// <param name="title">Title of status.</param>
        /// <param name="message">Status message.</param>
        /// <param name="isError">Is error notification or not.</param>
        protected void ShowNotification(string icon = IconExclamation, string title = "", string message = "", bool isError = true)
        {
            if (isError)
            {
                notification.Style.Add("background-color", "var(--notification-error)");
            }
            else
            {
                notification.Style.Add("background-color", "var(--notification-success)");
            }

            if (String.IsNullOrWhiteSpace(title) && String.IsNullOrWhiteSpace(message))
            {
                title = "Unknown error";
                message = "An unexpected error has occurred, please contact support.";
            }

            notificationIcon.Attributes.Add("class", icon);
            notificationTitle.InnerText = title;
            notificationMessage.InnerHtml = message;
            notification.Style.Add("display", "block");
        }
    }
}