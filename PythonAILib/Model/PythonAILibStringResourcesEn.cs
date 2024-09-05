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


        // ChatResult is null.
        public override string ChatResultNull { get; } = "ChatResult is null.";

        // Invalid ChatResult response.
        public override string ChatResultResponseInvalid { get; } = "Invalid ChatResult response.";

        // Result not found in ChatResult response.
        public override string ChatResultResponseResultNotFound { get; } = "Result not found in ChatResult response.";

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