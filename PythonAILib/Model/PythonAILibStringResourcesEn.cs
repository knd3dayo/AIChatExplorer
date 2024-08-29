namespace PythonAILib.Model {

    public class PythonAILibStringResourcesEn : PythonAILibStringResources {

        // --- namespace WpfAppCommon.PythonIF ---

        // Clipboard content has been changed
        public override string ClipboardChangedMessage { get; } = "Clipboard content has been changed";
        // Processing clipboard item
        public override string ProcessClipboardItem { get; } = "Processing clipboard item";
        // Running automatic processing
        public override string AutoProcessing { get; } = "Running automatic processing";
        // Failed to add clipboard item.
        public override string AddItemFailed { get; } = "Failed to add clipboard item.";

        // Running automatic title setting process
        public override string AutoSetTitle { get; } = "Running automatic title setting process";
        // Failed to set title
        public override string SetTitleFailed { get; } = "Failed to set title";
        // Running automatic tag setting process
        public override string AutoSetTag { get; } = "Running automatic tag setting process";
        // Failed to set tag
        public override string SetTagFailed { get; } = "Failed to set tag";
        // Running automatic merge process
        public override string AutoMerge { get; } = "Running automatic merge process";
        // Failed to merge
        public override string MergeFailed { get; } = "Failed to merge";
        // Running OCR process
        public override string OCR { get; } = "Running OCR process";
        // Failed to run OCR process
        public override string OCRFailed { get; } = "Failed to run OCR process";

        // Running automatic file extraction process
        public override string ExecuteAutoFileExtract { get; } = "Running automatic file extraction process";
        // Failed to execute automatic file extraction process
        public override string AutoFileExtractFailed { get; } = "Failed to execute automatic file extraction process";

        // --- EmptyPythonFunctions.cs ---
        // Python is not enabled. Please set PythonExecute in the settings screen.
        public override string PythonNotEnabledMessage { get; } = "Python is not enabled. Please set PythonExecute in the settings screen.";

        // --- PythonExecutor.cs ---
        // Custom Python script template file
        public override string TemplateScript { get; } = "python/script_template.py";

        // Python script for OpenAI
        public override string WpfAppCommonOpenAIScript { get; } = "python/ai_app.py";

        // Python script for miscellaneous
        public override string WpfAppCommonMiscScript { get; } = "python/dev/misc_app.py";

        // Template file not found
        public override string TemplateScriptNotFound { get; } = "Template file not found";

        // --- PythonNetFunctions.cs ---
        // Python DLL not found. Please check the path to the Python DLL:
        public override string PythonDLLNotFound { get; } = "Python DLL not found. Please check the path to the Python DLL:";

        // Python venv environment not found. Please check the path to Python venv:
        public override string PythonVenvNotFound { get; } = "Python venv environment not found. Please check the path to Python venv:";
        // Failed to initialize Python.
        public override string PythonInitFailed { get; } = "Failed to initialize Python.";

        // {function_name} function not found in Python script file
        public override string FunctionNotFound(string function_name) {
            return $"Function '{function_name}' not found in Python script file";
        }
        // An error occurred while executing the Python script
        public override string PythonExecuteError { get; } = "An error occurred while executing the Python script";

        // Python module not found. Please install the module using pip install <module_name>.
        public override string ModuleNotFound { get; } = "Python module not found. Please install the module using pip install <module_name>.";

        // Message:\n{e.Message}\nStack Trace:\n{e.StackTrace}
        public override string PythonExecuteErrorDetail(Exception e) {
            return $"Message:\n{e.Message}\nStack Trace:\n{e.StackTrace}";
        }
        // Spacy model name not set. Please set SPACY_MODEL_NAME from the settings screen
        public override string SpacyModelNameNotSet { get; } = "Spacy model name not set. Please set SPACY_MODEL_NAME from the settings screen";

        // No masking result found
        public override string MaskingResultNotFound { get; } = "No masking result found";

        // Failed to retrieve masked string
        public override string MaskingResultFailed { get; } = "Failed to retrieve masked string";

        // No unmasking result found
        public override string UnmaskingResultNotFound { get; } = "No unmasking result found";
        // Failed to retrieve unmasked string
        public override string UnmaskingResultFailed { get; } = "Failed to retrieve unmasked string";

        // Failed to convert image to byte array
        public override string ImageByteFailed { get; } = "Failed to convert image to byte array";

        // VectorDBItems is empty
        public override string VectorDBItemsEmpty { get; } = "VectorDBItems is empty";

        // No response from OpenAI
        public override string OpenAIResponseEmpty { get; } = "No response from OpenAI";

        // File not found
        public override string FileNotFound { get; } = "File not found";

        // --- ChatItem.cs ---

        // <Source Document Root>
        public override string SourceDocumentRoot { get; } = "<Source Document Root>";

        // --- ChatRequest.cs ---
        // \n--------- Below is the main content ------\n
        public override string ContentHeader { get; } = "\n--------- Below is the main content ------\n";

        // \n--------- Below is related information ------\n
        public override string SourcesHeader { get; } = "\n--------- Below is related information ------\n";

        // Unknown image format.
        public override string UnknownImageFormat { get; } = "Unknown image format.";

        // For any unclear points about the above text, please refer to the following related information
        public override string UnknownContent { get; } = "For any unclear points about the above text, please refer to the following related information";

        // Please analyze the following text and identify sentences containing undefined terms. " +
        // "Undefined terms are those whose genus, specific difference, cause, purpose, function, or components are unclear." +
        // "Return the output as a list in the following JSON format. If there is no text to analyze or analysis is not possible, return an empty list\n" +
        // "{'result':[{'sentence':'Sentence containing undefined term','reason':'Reason for determining that the term is undefined'}]}"

        public override string AnalyzeAndDictionarizeRequest { get; } = "Please analyze the following text and identify sentences containing undefined terms." +
            "Undefined terms are those whose genus, specific difference, cause, purpose, function, or components are unclear." +
            "Return the output as a list in the following JSON format. If there is no text to analyze or analysis is not possible, return an empty list\n" +
            "{'result':[{'sentence':'Sentence containing undefined term','reason':'Reason for determining that the term is undefined'}]}";

        // ChatResult is null.
        public override string ChatResultNull { get; } = "ChatResult is null.";

        // Invalid ChatResult response.
        public override string ChatResultResponseInvalid { get; } = "Invalid ChatResult response.";

        // Result not found in ChatResult response.
        public override string ChatResultResponseResultNotFound { get; } = "Result not found in ChatResult response.";

        // For unclear sentences, please refer to the following explanation
        public override string UnknownContentDescription { get; } = "For unclear sentences, please refer to the following explanation";

        // Please generate a summary of 100 to 200 characters from the following text.\n
        public override string SummarizeRequest { get; } = "Please generate a summary of 100 to 200 characters from the following text.\n";

        // Please generate background information (such as circumstances, purpose, cause, components, who, when, where, etc.) from the following text.\n
        public override string BackgroundInfoRequest { get; } = "Please generate background information (such as circumstances, purpose, cause, components, who, when, where, etc.) from the following text.\n";

        public override string AnalyzeJapaneseSentenceRequest { get; } = "* A proposition is a sentence that expresses that the subject (or topic) is something.\r\n" +
            "* In general, Japanese has the following structure:\r\n " +
            "Japanese structure = [Topic] is [something + particle] + predicate + [tense, modality, aspect]\r\n" +
            "* In Japanese, parts that already have common recognition between the speaker and the listener may be omitted, and you can have a conversation with just the predicate.\r\n" +
            "* \"Modality\" refers to the way the speaker expresses their judgment about the content of the sentence or how they convey it to the listener.\r\n" +
            "* Modality can be broadly classified into four types:\r\n" +
            "   - Modality that expresses communicative differentiation of the sentence: Expressive modality\r\n" +
            "   - Modality that expresses the way of perceiving the situation: Evaluative modality, Recognitional modality\r\n" +
            "   - Modality that expresses the association between the sentence and the preceding context: Explanatory modality\r\n" +
            "   - Modality that expresses the way of conveying to the listener: Politeness modality, Attitudinal modality\r\n" +
            "* \"Expressive modality\" represents the basic nature of the sentence, such as [narrative], [volition], [imperative], and [interrogative].\r\n" +
            "  - [Narrative] Read the textbook.\r\n  - [Volition] Let's read the textbook.\r\n  - [Imperative] Read the textbook.\r\n  - [Interrogative] Will you read the textbook?\r\n" +
            "* \"Evaluative modality\" represents the speaker's evaluative perception of the situation, such as necessary or unnecessary.\r\n" +
            "  - In the library, you must be quiet.\r\n" +
            "* \"Recognitional modality\" represents how the speaker perceives the content of the proposition.\r\n" +
            "  - It will probably be sunny tomorrow.\r\n" +
            "* \"Explanatory modality\" represents that the modality in the sentence is related to the preceding sentence.\r\n" +
            "  - Is it snowing? No wonder it's cold.\r\n" +
            "* \"Politeness modality\" refers to the style choice of whether to convey the sentence in [plain form] or [polite form] to the listener.\r\n" +
            "  - [Plain form] I read this book today.\r\n  - [Polite form] I read this book today.\r\n" +
            "* \"Attitudinal modality\" refers to fine adjustments when conveying to the listener or expressing the speaker's recognition state.\r\n" +
            "  - Look at this.\r\n  - What a beautiful view.\r\n\r\n" +
            "Please perform the following processing for the following sentence:\r\n" +
            "- Complete the omitted parts and create a list of propositions in the format of the above \"Japanese structure\". Also, explain the modality of each proposition.\r\n" +
            "  If there are multiple expected results, list up to 10 of the most likely ones.";

        public override string GenerateQuestionRequest { get; } = "Please analyze the text and generate questions.\r\nExample:\r\n# Questions about definition (genus and specific difference)\r\n Text: Ponchororin soup is delicious.\r\n Question: Does Ponchororin soup belong to the category of soup dishes? Or what makes it different from other items in the category?\r\n# Questions about purpose and reason\r\n Text: The task of XX must be completed by the end of today.\r\n Question: What is the reason for needing to complete the task of XX by the end of today? Also, what is the purpose of performing the task of XX?\r\n# Questions about cause, background, and history\r\n Text: Tokugawa Ieyasu is a shogun.\r\n Question: What is the cause of Tokugawa Ieyasu becoming a shogun?\r\n# Questions about components and functions\r\n Text: Ponchororin soup is good for health.\r\n Question: What ingredients are used to make Ponchororin soup? And what effects does it have?";

        public override string AnswerRequest { get; } = "Please answer the following questions.\n";

        // Please generate a title from the following text.\n
        public override string TitleRequest { get; } = "Please generate a title from the following text.\n";

        // Please extract the text from this image.\n
        public override string ExtractTextRequest { get; } = "Please extract the text from this image.\n";

        // --- VectorDBItem.cs ---
        // A general-purpose vector DB used to search past documents based on user questions.
        public override string VectorDBDescription { get; } = "A general-purpose vector DB used to search past documents based on user questions.";


        // --- PythonNetFunctions.cs ---

        // Execute embedding
        public override string EmbeddingExecute { get; } = "Execute embedding";

        // Property information
        public override string PropertyInfo { get; } = "Property information";

        // Text
        public override string Text { get; } = "Text";

        // Response
        public override string Response { get; } = "Response";

        // Execute OpenAI
        public override string OpenAIExecute { get; } = "Execute OpenAI";

        // Chat history
        public override string ChatHistory { get; } = "Chat history";

    }
}