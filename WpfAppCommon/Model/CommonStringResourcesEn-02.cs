namespace WpfAppCommon.Model {
    public partial class CommonStringResourcesEn {

        // --- namespace WpfAppCommon.PythonIF ---

        // --- DefaultClipboardController.cs ---
        public override string ClipboardChangedMessage { get; } = "Clipboard content has been changed";
        public override string ProcessClipboardItem { get; } = "Process clipboard item";
        public override string AutoProcessing { get; } = "Auto processing in progress";
        public override string AddItemFailed { get; } = "Failed to add clipboard item";

        public override string AutoSetTitle { get; } = "Executing auto title setting process";
        public override string SetTitleFailed { get; } = "Failed to set title";

        public override string AutoSetBackgroundInfo { get; } = "Executing auto background information addition process";
        public override string AddBackgroundInfoFailed { get; } = "Failed to add background information";

        public override string AutoCreateSummary { get; } = "Executing auto summary creation process";
        public override string CreateSummaryFailed { get; } = "Failed to create summary";

        public override string AutoExtractImageText { get; } = "Executing auto image text extraction process";
        public override string ExtractImageTextFailed { get; } = "Failed to extract image text";

        public override string AutoSetTag { get; } = "Executing auto tag setting process";
        public override string SetTagFailed { get; } = "Failed to set tag";
        public override string AutoMerge { get; } = "Executing auto merge process";
        public override string MergeFailed { get; } = "Failed to merge";
        public override string OCR { get; } = "Executing OCR process";
        public override string OCRFailed { get; } = "Failed to perform OCR";

        public override string ExecuteAutoFileExtract { get; } = "Executing auto file extraction process";
        public override string AutoFileExtractFailed { get; } = "Failed to perform auto file extraction";

        // --- EmptyPythonFunctions.cs ---
        public override string PythonNotEnabledMessage { get; } = "Python is not enabled. Please set PythonExecute in the settings screen.";

        // --- PythonExecutor.cs ---
        public override string TemplateScript { get; } = "python/script_template.py";

        public override string WpfAppCommonUtilsScript { get; } = "python/ai_app.py";

        public override string TemplateScriptNotFound { get; } = "Template file not found";

        // --- PythonNetFunctions.cs ---
        public override string PythonDLLNotFound { get; } = "Python DLL not found. Please check the path of Python DLL:";
        public override string PythonInitFailed { get; } = "Failed to initialize Python";

        public override string FunctionNotFound(string function_name) {
            return $"Function {function_name} not found in the Python script file";
        }
        public override string PythonExecuteError { get; } = "Error occurred during execution of Python script";

        public override string ModuleNotFound { get; } = "Python module not found. Please install the module with pip install <module name>.";

        public override string PythonExecuteErrorDetail(Exception e) {
            return $"Message:\n{e.Message}\nStack Trace:\n{e.StackTrace}";
        }
        public override string SpacyModelNameNotSet { get; } = "Spacy model name is not set. Please set SPACY_MODEL_NAME in the settings screen";

        public override string MaskingResultNotFound { get; } = "Masking result not found";
        public override string MaskingResultFailed { get; } = "Failed to retrieve masked string";

        public override string UnmaskingResultNotFound { get; } = "Unmasking result not found";
        public override string UnmaskingResultFailed { get; } = "Failed to retrieve unmasked string";

        public override string ImageByteFailed { get; } = "Failed to convert image to byte array";

        public override string VectorDBItemsEmpty { get; } = "VectorDBItems is empty";

        public override string OpenAIResponseEmpty { get; } = "OpenAI response is empty";

        public override string FileNotFound { get; } = "File not found";

        // -- ClipboardApp.MainWindowDataGrid1 --
        public override string UpdateDate { get; } = "Update Date";
        public override string Title { get; } = "Title";

        public override string SourceTitle { get; } = "Source Title";
        public override string Pin { get; } = "Pin";

        public override string Type { get; } = "Type";

        // -- AutoProcessRule --
        public override string RuleName { get; } = "Rule Name";

        public override string Enable { get; } = "Enable";

        public override string TargetFolder { get; } = "Target Folder";

        public override string ApplyAllItems { get; } = "Apply to all items";

        public override string ApplyMatchedItems { get; } = "Apply to matched items";

        public override string ItemType { get; } = "Item Type";
        public override string ItemTypeText { get; } = "Item Type is Text";
        public override string LineOrMore { get; } = "Lines or more";

        public override string LineOrLess { get; } = "Lines or less";

        public override string ItemTypeFile { get; } = "Item Type is File";

        public override string ItemTypeImage { get; } = "Item Type is Image";

        public override string TitleContains { get; } = "Title contains";

        public override string BodyContains { get; } = "Body contains";

        public override string SourceAppContains { get; } = "Source App contains";

        public override string ExecuteProcess { get; } = "Execute process";

        public override string ExecuteNextProcess { get; } = "Execute next process";

        public override string CopyMoveMergeTarget { get; } = "Copy/Move/Merge target";

        public override string ExecutePythonScript { get; } = "Execute Python script";

        public override string ExecuteOpenAI { get; } = "Execute OpenAI prompt";

        public override string OpenAIMode { get; } = "OpenAI execution mode";

        public override string StoreVectorDB { get; } = "Store in VectorDB";

        public override string TargetFolderFullPath { get; } = "Target Folder (Full Path)";

        public override string FolderUnit { get; } = "Folder Unit";

        public override string Up { get; } = "Up";
        public override string Down { get; } = "Down";

        public override string SourceApp { get; } = "Source App for Clipboard Monitoring";

        public override string SourceAppExample { get; } = "Enter the names of the apps to monitor, separated by commas. Example: notepad.exe,Teams.exe";

        public override string IgnoreTextLessOrEqualToSpecifiedLines { get; } = "Ignore text items with specified lines or less";

        public override string AutoTitleGeneration { get; } = "Auto Title Generation";

        public override string DoNot { get; } = "Do not";

        public override string AutomaticallyGenerateTitleUsingOpenAI { get; } = "Automatically generate title using OpenAI";

        public override string AutomaticallyGenerateTags { get; } = "Automatically generate tags";

        public override string AutomaticallyGenerateTagsFromClipboardContent { get; } = "Automatically generate tags from clipboard content";

        public override string AutomaticallyMerge { get; } = "Automatically merge";

        public override string AutomaticallyMergeItemsIfSourceAppAndTitleAreTheSame { get; } = "Automatically merge items if source app and title are the same";

        public override string AutomaticallyEmbedding { get; } = "Automatically embedding";

        public override string AutomaticallyEmbeddingWhenSavingClipboardItems { get; } = "Automatically embed when saving clipboard items";

        public override string AutomaticallyExtractTextFromFile { get; } = "Automatically extract text from file";

        public override string AutomaticallyExtractTextFromFileIfClipboardItemIsFile { get; } = "Automatically extract text from file if clipboard item is file";

        public override string AutomaticallyExtractTextFromImage { get; } = "Automatically extract text from image";

        public override string ExtractTextUsingPyOCR { get; } = "Extract text using PyOCR";

        public override string ExtractTextUsingOpenAI { get; } = "Extract text using OpenAI";

        public override string EmbeddingWhenExtractingTextFromImage { get; } = "Embedding when extracting text from image";
        public override string EmbeddingWhenExtractingTextFromImageDescription { get; } = "Embedding when extracting text from image";

        public override string AutomaticallyAddBackgroundInformation { get; } = "Automatically add background information";

        public override string GenerateBackgroundInformationFromItemsInTheSameFolder { get; } = "Generate background information from items in the same folder";
        public override string AutomaticallyAddJapaneseSentenceAnalysisResultsToBackgroundInformation { get; } = "(Experimental Feature) Automatically add Japanese sentence analysis results to background information";
        public override string IncludeBackgroundInformationInEmbedding { get; } = "Include background information in embedding";

        public override string IncludeBackgroundInformationInEmbeddingTargetText { get; } = "Include background information in embedding target text";

        public override string AutomaticallyGenerateSummary { get; } = "Automatically generate summary";

        public override string GenerateSummaryTextFromContent { get; } = "Generate summary text from content";

        public override string SynchronizeClipboardItemsWithFoldersOnTheOS { get; } = "Synchronize clipboard items with folders on the OS";

        public override string SynchronizeClipboardItemsWithFoldersOnTheOSDescription { get; } = "Synchronize clipboard items with folders on the OS";

        public override string SyncTargetFolderName { get; } = "Sync Target Folder Name";

        public override string SpecifyTheFolderNameOnTheOSToSynchronizeTheClipboardItems { get; } = "Specify the folder name on the OS to synchronize the clipboard items";

        public override string IfTheSyncTargetFolderIsAGitRepositoryItWillAutomaticallyCommitWhenTheFileIsUpdated { get; } = "If the sync target folder is a Git repository, it will automatically commit when the file is updated";

        public override string EntityExtractionDataMasking { get; } = "Entity Extraction / Data Masking";

        public override string ExtractEntitiesAndMaskDataUsingSpacyFromClipboardContent { get; } = "Extract entities and mask data using Spacy from clipboard content";

        public override string MaskPersonalInformationInDataSentToOpenAI { get; } = "Mask personal information in data sent to OpenAI";

        public override string NewAutoProcessRule { get; } = "New Auto Process Rule";

        public override string SaveSystemCommonSettings { get; } = "Save system common settings";

        // -- FolderEditWindow --
        public override string EditClipboardFolder { get; } = "Edit Clipboard Folder";

        public override string Name { get; } = "Name";

        public override string Description { get; } = "Description";

        public override string SelectFolder { get; } = "Select Folder";

        public override string SelectFile { get; } = "Select File";

    }
}