namespace PythonAILib.Resource {

    public class PythonAILibStringResourcesEn : PythonAILibStringResources {


        // PythonAILibManagerIsNotInitialized
        public override string PythonAILibManagerIsNotInitialized { get; } = "PythonAILibManager is not initialized";

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

        // VectorSearchProperties is empty
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

        public override string VectorDBItems { get; } = "Vector DB Items";

        // ベクトル検索リクエスト
        public override string VectorSearchRequest { get; } = "Vector Search Request";

        // Excelへのエクスポートを実行します
        public override string ExportToExcelExecute { get; } = "Export to Excel Execute";
        // Excelへのエクスポートが失敗しました
        public override string ExportToExcelFailed { get; } = "Export to Excel Failed";
        // Excelへのエクスポートが成功しました
        public override string ExportToExcelSuccess { get; } = "Export to Excel Success";

        // ファイルパス
        public override string FilePath { get; } = "ファイルパス";
        // データ
        public override string Data { get; } = "データ";



        // Text
        public override string Text { get; } = "Text";

        // Output
        public override string Response { get; } = "Response";

        // Execute OpenAI
        public override string OpenAIExecute { get; } = "Execute OpenAI";

        // GetTokenCountExecute
        public override string GetTokenCountExecute { get; } = "Execute GetTokenCount";

        // Chat history
        public override string ChatHistory { get; } = "Chat history";

        // UpdateVectorDBIndex実行
        public override string UpdateVectorDBIndexExecute { get; } = "Execute UpdateVectorDBIndex";

        // ベクトルDBのコレクション削除を実行
        public override string DeleteVectorDBCollectionExecute { get; } = "Execute DeleteVectorDBCollection";


        public override string TextExtracted { get; } = "Extracted Text";

        public override string UpdateDate { get; } = "Update Date";

        // VectorizedDate
        public override string VectorizedDate { get; } = "Vectorized Date";

        public override string Title { get; } = "Title";

        public override string SourceTitle { get; } = "Source Title";

        // SourcePath
        public override string SourcePath { get; } = "Source Path";

        public override string Pin { get; } = "Pin";

        // Document reliability
        public override string DocumentReliability { get; } = "Document reliability";

        // Document category summary
        public override string DocumentCategorySummary { get; } = "Document category summary";

        public override string Type { get; } = "Type";

        public override string CreationDateTime { get; } = "Creation Date Time";
        public override string SourceAppName { get; } = "Source App Name";
        public override string Pinned { get; } = "Pinned";

        // Tag
        public override string Tag { get; } = "Tag";
        public override string BackgroundInformation { get; } = "Background Information";

        // -- ScreenShotCheckCondition.cs --
        public override string CheckTypeEqual { get; } = "Equal";
        public override string CheckTypeNotEqual { get; } = "Not equal";
        public override string CheckTypeInclude { get; } = "Include";
        public override string CheckTypeNotInclude { get; } = "Not include";
        public override string CheckTypeStartWith { get; } = "Start with";
        public override string CheckTypeNotStartWith { get; } = "Not start with";
        public override string CheckTypeEndWith { get; } = "End with";
        public override string CheckTypeNotEndWith { get; } = "Not end with";
        public override string CheckTypeEmpty { get; } = "Empty";
        public override string CheckTypeCheckBox { get; } = "Check box";


        public override string SettingValueIs(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " is " + SettingValue;
        }
        public override string SettingValueIsNot(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " is Not " + SettingValue;
        }
        public override string SettingValueContains(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Contains " + SettingValue;
        }
        public override string SettingValueNotContain(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Does Not Contain " + SettingValue;
        }
        public override string SettingValueStartsWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Starts With " + SettingValue;
        }
        public override string SettingValueNotStartWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Does Not Start With " + SettingValue;
        }
        public override string SettingValueEndsWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Ends With " + SettingValue;
        }
        public override string SettingValueNotEndWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Does Not End With " + SettingValue;
        }
        public override string SettingValueIsEmpty(string SettingItem) {
            return "Setting Value of " + SettingItem + " is Empty";
        }
        public override string SettingValueIsChecked(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " is Checked " + SettingValue;
        }
        public override string NoRemoteRepositorySet { get; } = "No remote repository set";
        public override string NoWorkingDirectorySpecified { get; } = "No working directory specified";
        public override string SpecifiedDirectoryDoesNotExist { get; } = "Specified directory does not exist";
        public override string SpecifiedDirectoryIsNotAGitRepository { get; } = "Specified directory is not a Git repository";
        public override string NoVectorDBSet { get; } = "No vector DB set";
        public override string UnsupportedFileType { get; } = "Unsupported File Type";
        public override string SaveEmbedding { get; } = "Save Embedding";
        public override string SavedEmbedding { get; } = "Saved Embedding";
        public override string DeleteEmbedding { get; } = "Delete Embedding";
        public override string DeletedEmbedding { get; } = "Deleted Embedding";
        public override string SaveTextEmbeddingFromImage { get; } = "Save Text Embedding from Image";
        public override string SavedTextEmbeddingFromImage { get; } = "Saved Text Embedding from Image";
        public override string DeleteTextEmbeddingFromImage { get; } = "Delete Text Embedding from Image";
        public override string DeletedTextEmbeddingFromImage { get; } = "Deleted Text Embedding from Image";
        public override string GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions { get; } = "General Vector DB for Searching Past Documents Based on User Questions";
        // Execute the prompt template [promptName].
        public override string PromptTemplateExecute(string promptName) => $"Execute the prompt template [{promptName}].";

        // "The prompt template [promptName] has been executed."
        public override string PromptTemplateExecuted(string promptName) => $"The prompt template [{promptName}] has been executed.";

        // InputContentNotFound
        public override string InputContentNotFound { get; } = "Input content not found";

        public override string AddedItems { get; } = "Added Items";

        // Auto processing rule
        public override string ExtractText { get; } = "Extract text";
        public override string Ignore { get; } = "Ignore";
        public override string DoNothing { get; } = "Do Nothing";
        public override string CopyToFolder { get; } = "Copy to Folder";
        public override string CopyClipboardContentToSpecifiedFolder { get; } = "Copy Clipboard Content to Specified Folder";
        public override string MoveToFolder { get; } = "Move to Folder";
        public override string MoveClipboardContentToSpecifiedFolder { get; } = "Move Clipboard Content to Specified Folder";
        public override string ExtractClipboardText { get; } = "Extract Clipboard Text";
        public override string DataMasking { get; } = "Data Masking";
        public override string MaskClipboardText { get; } = "Mask Clipboard Text";
        public override string MergeItemsInFolder { get; } = "Merge Items in Folder";
        public override string MergeItemsInFolderDescription { get; } = "Merge Items in Folder Description";
        public override string MergeItemsWithTheSameSourceApplicationTitle { get; } = "Merge Items with the Same Source Application Title";
        public override string MergeItemsWithTheSameSourceApplicationTitleDescription { get; } = "Merge Items with the Same Source Application Title Description";
        public override string NoFolderSelected { get; } = "No Folder Selected";
        public override string CopyToFolderDescription { get; } = "Copy to Folder Description";
        public override string CannotOpenDirectoryAsNewFile { get; } = "Cannot Open Directory as New File";

        // Auto Process Rule
        public override string NoMatch { get; } = "No Match";
        public override string NoActionSet { get; } = "No Action Set";
        public override string Condition { get; } = "Condition";
        public override string Action { get; } = "Action";
        public override string ActionNone { get; } = "Action None";
        public override string FolderNone { get; } = "Folder None";
        public override string DetectedAnInfiniteLoop { get; } = "Detected an Infinite Loop";
        public override string RuleNameIsInvalid(string RuleName) {
            return RuleName + " is Invalid";
        }
        public override string DescriptionContains(string Keyword) {
            return "Description Contains " + Keyword;
        }
        public override string ContentContains(string Keyword) {
            return "Content Contains " + Keyword;
        }
        public override string SourceApplicationNameContains(string Keyword) {
            return "Source Application Name Contains " + Keyword;
        }
        public override string SourceApplicationTitleContains(string Keyword) {
            return "Source Application Title Contains " + Keyword;
        }
        public override string SourceApplicationPathContains(string Keyword) {
            return "Source Application Path Contains " + Keyword;
        }

        public override string AutoExtractImageText { get; } = "Executing auto image text extraction process";

        // ファイル
        public override string File { get; } = "File";

        // フォルダ
        public override string Folder { get; } = "Folder";

        public override string AutoSetBackgroundInfo { get; } = "Executing auto background information addition process";
        public override string AddBackgroundInfoFailed { get; } = "Failed to add background information";

        public override string AutoCreateSummary { get; } = "Executing auto summary creation process";

        // Execute automatic document reliability check process
        public override string AutoCheckDocumentReliability { get; } = "Executing auto document reliability check process";
        // Document reliability check process failed
        public override string CheckDocumentReliabilityFailed { get; } = "Document reliability check process failed";

        public override string CreateSummaryFailed { get; } = "Failed to create summary";

        // 自動課題リスト作成処理を実行します
        public override string AutoCreateTaskList { get; } = "Execute auto Task list creation process";
        // 課題リスト作成処理が失敗しました
        public override string CreateTaskListFailed { get; } = "Failed to create Task list";

        public override string ApplyAutoProcessing { get; } = "Apply Auto Processing";

        public override string ItemsDeletedByAutoProcessing { get; } = "Items Deleted by Auto Processing";

        #region Statistics and Logging

        // Daily token count
        public override string DailyTokenCount { get; } = "Daily Token Count";
        // Total token count
        public override string TotalTokenFormat(long tokens) {
            return $"Total Token Count: {tokens} tokens";
        }
        // Token count
        public override string TokenCount { get; } = "Token Count";

        public override string DailyTokenFormat(string date, long totalTokens) {
            return $"Token count for {date}: {totalTokens} tokens";
        }
        #endregion

    }
}