using WpfAppCommon.Model;

namespace QAChat.Resource {
    public  class CommonStringResourcesEn : CommonStringResources {

        // -- SettingsUserControl.xaml --
        // Restart the application to apply the changes.
        public override string RestartAppToApplyChanges { get; } = "Restart the application to apply the changes.";

        // Basic Settings
        public override string BasicSettings { get; } = "Basic Settings";
        // Specify the python3**.dll location for Python installation
        public override string SpecifyPython3Dll { get; } = "Specify the python3**.dll location for Python installation";

        // Python DLL Path
        public override string PythonDLLPath { get; } = "Python DLL Path";

        // Python virtual environment location
        public override string PythonVenvPath { get; } = "Python virtual environment location";

        // Specify the venv location if using Python venv
        public override string SpecifyVenvPath { get; } = "Specify the venv location if using Python venv";

        // Clipboard DB backup generations
        public override string ClipboardDBBackupGenerations { get; } = "Clipboard DB backup generations";

        // Backup generations of clipbord.db and clipboard-log.db
        public override string ClipboardDBBackupGenerationsDescription { get; } = "Backup generations of clipbord.db and clipboard-log.db";

        // OpenAI Settings
        public override string OpenAISettings { get; } = "OpenAI Settings";

        // OpenAI API Key
        public override string OpenAIKey { get; } = "OpenAI API Key";

        // Set OpenAI Key for OpenAI or Azure OpenAI
        public override string SetOpenAIKey { get; } = "Set OpenAI Key for OpenAI or Azure OpenAI";

        // Use Azure OpenAI
        public override string UseAzureOpenAI { get; } = "Use Azure OpenAI";

        // Use Azure OpenAI instead of OpenAI
        public override string UseAzureOpenAIInsteadOfOpenAI { get; } = "Use Azure OpenAI instead of OpenAI";

        // Azure OpenAI Endpoint
        public override string AzureOpenAIEndpoint { get; } = "Azure OpenAI Endpoint";

        // Set Azure OpenAI Endpoint if using Azure OpenAI
        public override string SetAzureOpenAIEndpoint { get; } = "Set Azure OpenAI Endpoint if using Azure OpenAI";

        // Model for OpenAI Chat
        public override string OpenAIModel { get; } = "Model for OpenAI Chat";

        // Set OpenAI or Azure OpenAI chat model. Example: gpt-4-turbo, gpt-4-1106-preview, etc.
        public override string SetOpenAIModel { get; } = "Set OpenAI or Azure OpenAI chat model. Example: gpt-4-turbo, gpt-4-1106-preview, etc.";

        // Model for OpenAI Embedding
        public override string OpenAIEmbeddingModel { get; } = "Model for OpenAI Embedding";

        // Set OpenAI or Azure OpenAI embedding model. Example: text-embedding-ada-002, text-embedding-3-small, etc.
        public override string SetOpenAIEmbeddingModel { get; } = "Set OpenAI or Azure OpenAI embedding model. Example: text-embedding-ada-002, text-embedding-3-small, etc.";

        // Base URL for OpenAI Chat model
        public override string OpenAIChatBaseURL { get; } = "Base URL for OpenAI Chat model";

        // Set different endpoint for OpenAI Chat model than the default endpoint or Azure OpenAI endpoint
        public override string SetOpenAIChatBaseURL { get; } = "Set different endpoint for OpenAI Chat model than the default endpoint or Azure OpenAI endpoint";

        // Base URL for OpenAI Embedding model
        public override string OpenAIEmbeddingBaseURL { get; } = "Base URL for OpenAI Embedding model";

        // Set different endpoint for OpenAI Embedding model than the default endpoint or Azure OpenAI endpoint
        public override string SetOpenAIEmbeddingBaseURL { get; } = "Set different endpoint for OpenAI Embedding model than the default endpoint or Azure OpenAI endpoint";

        // Python Spacy Settings
        public override string PythonSpacySettings { get; } = "Python Spacy Settings";

        // Spacy Model Name
        public override string SpacyModelName { get; } = "Spacy Model Name";

        // Set installed Spacy model name. Example: ja_core_news_sm, ja_core_news_lg, etc.
        public override string SetSpacyModelName { get; } = "Set installed Spacy model name. Example: ja_core_news_sm, ja_core_news_lg, etc.";

        // Python OCR Settings
        public override string PythonOCRSettings { get; } = "Python OCR Settings";

        // Tesseract Path
        public override string TesseractPath { get; } = "Tesseract Path";

        // Other
        public override string Other { get; } = "Other";

        // Enable development features
        public override string EnableDevelopmentFeatures { get; } = "Enable development features";

        // Check settings
        public override string CheckSettings { get; } = "Check settings";

        public override string AppName { get; } = "RAG Clipboard";
        // File
        public override string File { get; } = "File";

        // File / Image
        public override string FileOrImage { get; } = "File/Image";
        // Create
        public override string Create { get; } = "Create";
        // Create Item
        public override string CreateItem { get; } = "Create Item";
        // Exit
        public override string Exit { get; } = "Exit";
        // Edit
        public override string Edit { get; } = "Edit";

        #region Prompt Menu
        public override string PromptMenu { get; } = "Prompt Menu";

        // Generate Title
        public override string GenerateTitle { get; } = "Generate Title";

        // Background Information
        public override string BackgroundInformation { get; } = "Background Information";

        // Generate Background Info
        public override string GenerateBackgroundInfo { get; } = "Generate Background Info";

        // Generate Summary
        public override string GenerateSummary { get; } = "Generate Summary";
        // TaskList
        public override string TasksList { get; } = "Task List";

        // "Generate the list of Tasks"
        public virtual string GenerateTTasks { get; } = "Generate the list of Tasks";

        // "The list of Tasks has been generated"
        public virtual string GeneratedTTasks { get; } = "The list of Tasks has been generated";

        #endregion

        // Generate Vector
        public override string GenerateVector { get; } = "Generate Vector";

        // Vector Search
        public override string VectorSearch { get; } = "Vector Search";

        // Start
        public override string Start { get; } = "Start";
        // Stop
        public override string Stop { get; } = "Stop";
        // Select
        public override string Select { get; } = "Select";
        // Help
        public override string Help { get; } = "Help";
        // Version Info
        public override string VersionInfo { get; } = "Version Info";

        // View
        public override string View { get; } = "View";

        // Start Clipboard Watch
        public override string StartClipboardWatch { get; } = "Start Clipboard Watch";
        // Stop Clipboard Watch
        public override string StopClipboardWatch { get; } = "Stop Clipboard Watch";
        // Start Notification Watch
        public override string StartNotificationWatch { get; } = "Start Notification Watch";
        // Stop Notification Watch
        public override string StopNotificationWatch { get; } = "Stop Notification Watch";

        // Started Clipboard Watch
        public override string StartClipboardWatchMessage { get; } = "Started Clipboard Watch";
        // Stopped Clipboard Watch
        public override string StopClipboardWatchMessage { get; } = "Stopped Clipboard Watch";
        // Started Notification Watch
        public override string StartNotificationWatchMessage { get; } = "Started Notification Watch";
        // Stopped Notification Watch
        public override string StopNotificationWatchMessage { get; } = "Stopped Notification Watch";

        // Edit Tag
        public override string EditTag { get; } = "Edit Tag";
        // Edit Auto Process Rule
        public override string EditAutoProcessRule { get; } = "Edit Auto Process Rule";
        // Edit Python Script
        public override string EditPythonScript { get; } = "Edit Python Script";
        // Edit Prompt Template
        public override string EditPromptTemplate { get; } = "Edit Prompt Template";
        // Edit Git RAG Source
        public override string EditGitRagSource { get; } = "Edit Git RAG Source";

        // -- View Menu --
        // Wrap text at the right edge
        public override string TextWrapping { get; } = "Wrap text at the right edge";
        // Enable Preview Mode
        public override string PreviewMode { get; } = "Enable Preview Mode";
        // Enable Compact Mode
        public override string CompactMode { get; } = "Enable Compact Mode";

        // Tool
        public override string Tool { get; } = "Tool";
        // OpenAI Chat
        public override string OpenAIChat { get; } = "OpenAI Chat";
        // BitmapImage Chat
        public override string ImageChat { get; } = "Image Chat";

        // Search
        public override string Search { get; } = "Search";
        // Setting
        public override string Setting { get; } = "Setting";
        // Delete
        public override string Delete { get; } = "Delete";
        // Add
        public override string Add { get; } = "Add";
        // OK
        public override string OK { get; } = "OK";
        // Cancel
        public override string Cancel { get; } = "Cancel";
        // Close
        public override string Close { get; } = "Close";

        // Export/Import
        public override string ExportImport { get; } = "Export/Import";

        // Export
        public override string Export { get; } = "Export";
        // Import
        public override string Import { get; } = "Import";

        // Backup/Restore
        public override string BackupRestore { get; } = "Backup/Restore";

        // Backup Items
        public override string BackupItem { get; } = "Backup Items";
        // Restore Items
        public override string RestoreItem { get; } = "Restore Items";

        // List of Auto Process Rules
        public override string ListAutoProcessRule { get; } = "List of Auto Process Rules";
        // List of Python Scripts
        public override string ListPythonScript { get; } = "List of Python Scripts";

        // List of Tags
        public override string ListTag { get; } = "List of Tags";

        // New Tag
        public override string NewTag { get; } = "New Tag";
        // Tag
        public override string Tag { get; } = "Tag";

        // List of Vector DBs
        public override string ListVectorDB { get; } = "List of Vector DBs";
        // Edit Vector DB
        public override string EditVectorDB { get; } = "Edit Vector DB";

        // --- ToolTip ---
        // Start: Start Clipboard Watch. Stop: Stop Clipboard Watch.
        public override string ToggleClipboardWatchToolTop { get; } = "Start: Start Clipboard Watch. Stop: Stop Clipboard Watch.";

        // Start: Start Notification Watch. Stop: Stop Notification Watch.
        public override string ToggleNotificationWatchToolTop { get; } = "Start: Start Notification Watch. Stop: Stop Notification Watch.";

        // Create items in the selected folder.
        public override string CreateItemToolTip { get; } = "Create items in the selected folder.";

        // Exit the application.
        public override string ExitToolTip { get; } = "Exit the application.";
        // Edit tags.
        public override string EditTagToolTip { get; } = "Edit tags.";

        // Delete selected tags.
        public override string DeleteSelectedTag { get; } = "Delete selected tags";
        // Select all.
        public override string SelectAll { get; } = "Select all";
        // Unselect all.
        public override string UnselectAll { get; } = "Unselect all";

        // --- Window Titles ---

        // List of Auto Process Rules
        public override string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // Edit Auto Process Rule
        public override string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }
        // List of Python Scripts
        public override string ListPythonScriptWindowTitle {
            get {
                return $"{AppName} - {ListPythonScript}";
            }
        }
        // Edit Python Script
        public override string EditPythonScriptWindowTitle {
            get {
                return $"{AppName} - {EditPythonScript}";
            }
        }
        // Setting
        public override string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // Setting Check Result
        public override string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - Setting Check Result";
            }
        }

        // Edit Git RAG Source
        public override string EditGitRagSourceWindowTitle {
            get {
                return $"{AppName} - {EditGitRagSource}";
            }
        }
        // List of Git RAG Sources
        public override string ListGitRagSourceWindowTitle {
            get {
                return $"{AppName} - List of Git RAG Sources";
            }
        }
        // List of Vector DBs
        public override string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }
        // Edit Vector DB
        public override string EditVectorDBWindowTitle {
            get {
                return $"{AppName} - {EditVectorDB}";
            }
        }
        // Select Commit
        public override string SelectCommitWindowTitle {
            get {
                return $"{AppName} - Select Commit";
            }
        }
        // QA Chat
        public override string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // List of Tags
        public override string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // Log Display
        public override string LogWindowTitle {
            get {
                return $"{AppName} - Log Display";
            }
        }
        // Prompt Generation for Screenshot Check
        public override string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - Prompt Generation for Screenshot Check";
            }
        }
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

        // 自動課題リスト作成処理を実行します
        public override string AutoCreateTaskList { get; } = "Execute auto Task list creation process";
        // 課題リスト作成処理が失敗しました
        public override string CreateTaskListFailed { get; } = "Failed to create Task list";


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

        public override string AutomaticallyAddAutoQAResultsToBackgroundInformation { get; } = "(Experimental Feature) Automatically add auto QA results to background information";
        public override string IncludeBackgroundInformationInEmbedding { get; } = "Include background information in embedding";

        public override string IncludeBackgroundInformationInEmbeddingTargetText { get; } = "Include background information in embedding target text";

        public override string AutomaticallyGenerateSummary { get; } = "Automatically generate summary";

        public override string GenerateSummaryTextFromContent { get; } = "Generate summary text from content";

        // Automatically generate Task list
        public override string AutomaticallyGenerateTaskList { get; } = "Automatically generate Task list";

        // Generate Task list from content
        public override string GenerateTaskListFromContent { get; } = "Generate Task list from content";
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

        // 自動処理時の設定
        public override string AutoProcessSetting { get; } = "Auto Process Setting";
        // Chatタイプ
        public override string ChatType { get; } = "Chat Type";
        // 出力形式
        public override string OutputType { get; } = "Output Type";
        // 文字列
        public override string StringType { get; } = "String";
        // リスト
        public override string ListType { get; } = "List";
        // テーブル
        public override string TableType { get; } = "Table";
        // 出力先
        public override string OutputDestination { get; } = "Output Destination";
        // 新規タブ
        public override string NewTab { get; } = "New Tab";
        // 本文を上書き
        public override string OverwriteContent { get; } = "Overwrite Content";
        // タイトルを上書き
        public override string OverwriteTitle { get; } = "Overwrite Title";

        public override string SelectFolder { get; } = "Select Folder";

        public override string SelectFile { get; } = "Select File";

        // -- EditItemWindow --
        public override string OpenTextAsFile { get; } = "Open text as file";
        public override string OpenFile { get; } = "Open file";
        public override string OpenAsNewFile { get; } = "Open as new file";
        public override string OpenFolder { get; } = "Open folder";
        public override string ExtractText { get; } = "Extract text";
        public override string OpenImage { get; } = "Open image";
        public override string ExtractTextFromImage { get; } = "Extract text from image";
        public override string MaskData { get; } = "Mask data";
        public override string ClickHereToOpenTheTagEditScreen { get; } = "Click here to open the tag edit screen";
        public override string Text { get; } = "Text";
        public override string FilePath { get; } = "File path";
        public override string Folder { get; } = "Folder";
        public override string FileName { get; } = "File name";
        public override string FolderNameAndFileName { get; } = "Folder name and file name";
        public override string Image { get; } = "Image";

        // -- EditPythonScriptWindow --
        public override string Content { get; } = "Content";

        // -- ListPythonScriptWindow --
        public override string NewPythonScript { get; } = "New Python script";

        // -- SearchWindow --
        public override string SearchTargetFolder { get; } = "Search target folder";
        public override string Exclude { get; } = "Exclude";
        public override string CopySourceAppName { get; } = "Copy source app name";
        public override string StartDate { get; } = "Start date";
        public override string EndDate { get; } = "End date";
        public override string IncludeSubfolders { get; } = "Include subfolders";
        public override string Clear { get; } = "Clear";

        // -- TagSearchWindow
        public override string TagSearch { get; } = "Tag search";

        // -- VectorSearchResultWindow
        public override string VectorSearchResult { get; } = "Vector search result";

        // -- ImageChatWindow
        public override string SettingItem { get; } = "Setting item";
        public override string SettingValue { get; } = "Setting value";
        public override string CheckType { get; } = "Check type";
        public override string Paste { get; } = "Paste";

        // -- ImageCheck.MainWindow --
        public override string SelectImageFile { get; } = "Select image file";
        public override string EditImageEvidenceCheckItem { get; } = "Edit image evidence check item";
        public override string Open { get; } = "Open";
        public override string TheAnswerWillBeDisplayedHere { get; } = "The answer will be displayed here";
        public override string EnterYourQuestionHere { get; } = "Enter your question here";
        public override string Save { get; } = "Save";
        public override string Send { get; } = "Send";

        // -- ListVectorDBWindow --
        public override string DisplayVectorsForTheSystem { get; } = "Display vectors for the system";

        public override string DisplayPromptsForTheSystem { get; } = "Display prompts for the system";

        public override string VectorDBLocation { get; } = "Vector DB location";
        public override string VectorDBType { get; } = "Vector DB type";
        public override string NewVectorDBSetting { get; } = "New vector DB setting";
        public override string EditVectorDBSetting { get; } = "Edit vector DB setting";

        // -- QAChatControl --
        public override string ExperimentalFunction1 { get; } = "Experimental function 1";
        public override string VectorDBFolder { get; } = "Vector DB (folder)";

        // ここをクリックしてベクトルDB(フォルダ)を追加
        public override string ClickHereToAddVectorDBFolder { get; } = "Click here to add Vector DB (Folder)";

        // ベクトルDB選択
        public override string SelectVectorDB { get; } = "Select Vector DB";

        public override string ExcludeFromList { get; } = "Exclude from list";
        public override string VectorDB { get; } = "Vector DB";
        public override string VectorDBExternal { get; } = "Vector DB (external)";
        public override string ClickHereToAddVectorDB { get; } = "Click here to add Vector DB";
        public override string AdditionalItem { get; } = "Additional Item";
        public override string ClickHereToPasteTheSelectedItem { get; } = "Click here to paste the selected item";
        public override string ImageFile { get; } = "Image file";
        public override string ClickHereToAddImageFile { get; } = "Click here to add image file";
        public override string Chat { get; } = "Chat";
        public override string PromptTemplate { get; } = "Prompt template";
        public override string Preview { get; } = "Preview";
        public override string PreviewJSON { get; } = "Preview (JSON)";
        public override string Copy { get; } = "Copy";

        // --- ClipboardFolderViewModel ---
        public override string AutoProcessingIsSet { get; } = "Auto processing is set";
        public override string SearchCondition { get; } = "Search condition";
        public override string FolderEdited { get; } = "Folder edited";
        public override string Reloaded { get; } = "Reloaded";
        public override string SelectFilePlease { get; } = "Select file, please";
        public override string SelectFolderPlease { get; } = "Select folder, please";
        public override string FolderExported { get; } = "Folder exported";
        public override string FolderImported { get; } = "Folder imported";
        public override string RootFolderCannotBeDeleted { get; } = "Root folder cannot be deleted";
        public override string Confirm { get; } = "Confirm";
        public override string ConfirmDeleteFolder { get; } = "Confirm delete folder?";
        public override string FolderDeleted { get; } = "Folder deleted";
        public override string ConfirmDeleteItems { get; } = "Confirm delete items?";
        public override string DeletedItems { get; } = "Deleted items";
        public override string Added { get; } = "Added";
        public override string Edited { get; } = "Edited";
        public override string Pasted { get; } = "Pasted";
        public override string SelectTwoItemsToMerge { get; } = "Select two items to merge";
        public override string MergeTargetNotSelected { get; } = "Merge target not selected";
        public override string MergeSourceNotSelected { get; } = "Merge source not selected";
        public override string Merged { get; } = "Merged";
        public override string ErrorOccurredAndMessage { get; } = "Error occurred.\nMessage";
        public override string StackTrace { get; } = "Stack trace";

        // --- ClipboardItemViewModel ---
        public override string CannotOpenFolderForNonFileContent { get; } = "Cannot open folder for non-file content";
        public override string CannotExtractTextForNonFileContent { get; } = "Cannot extract text for non-file content";
        public override string MainWindowViewModelIsNull { get; } = "MainWindowViewModel is null";
        public override string GenerateTitleInformation { get; } = "Generate title information";
        public override string GeneratedTitleInformation { get; } = "Generated title information";
        public override string GenerateBackgroundInformation { get; } = "Generate background information";
        public override string GeneratedBackgroundInformation { get; } = "Generated background information";
        public override string GenerateSummary2 { get; } = "Generate summary";
        public override string GeneratedSummary { get; } = "Generated summary";

        // Other prompts
        public override string OtherPrompts { get; } = "Other Prompts";
        public override string GenerateVector2 { get; } = "Generate vector";
        public override string GeneratedVector { get; } = "Generated vector";
        public override string CannotExtractTextForNonImageContent { get; } = "Cannot extract text for non-image content";
        public override string EnterANumber { get; } = "Enter a number";
        public override string FolderNotSelected { get; } = "Folder not selected";
        public override string EnterRuleName { get; } = "Enter rule name";
        public override string SelectAction { get; } = "Select action";
        public override string RuleNotFound { get; } = "Rule not found";
        public override string SelectCopyOrMoveTargetFolder { get; } = "Select copy or move target folder";
        public override string CannotCopyOrMoveToTheSameFolder { get; } = "Cannot copy or move to the same folder";
        public override string DetectedAnInfiniteLoopInCopyMoveProcessing { get; } = "Detected an infinite loop in copy/move processing";
        public override string SelectPromptTemplate { get; } = "Select prompt template";
        public override string SelectPythonScript { get; } = "Select Python script";
        public override string CannotCopyOrMoveToNonStandardFolders { get; } = "Cannot copy or move to non-standard folders";
        public override string RootFolderViewModelIsNull { get; } = "RootFolderViewModel is null";

        // --- EditPythonScriptWindowViewModel ---
        public override string EnterDescription { get; } = "Enter description";

        // --- FolderEditWindowViewModel ---
        public override string FolderNotSpecified { get; } = "Folder not specified";
        public override string EnterFolderName { get; } = "Enter folder name";

        // --- FolderSelectWindowViewModel ---
        public override string FolderSelectWindowViewModelInstanceNotFound { get; } = "Error occurred. FolderSelectWindowViewModel instance not found";
        public override string SelectedFolderNotFound { get; } = "Error occurred. Selected folder not found";

        // --- ListAutoProcessRuleWindowViewModel ---
        public override string AutoProcessRuleNotSelected { get; } = "Auto process rule not selected";
        public override string ConfirmDelete { get; } = "Confirm delete";
        public override string SavedSystemCommonSettings { get; } = "Saved system common settings";
        public override string NoChangesToSystemCommonSettings { get; } = "No changes to system common settings";

        // --- ListPythonScriptWindowViewModel ---
        public override string Execute { get; } = "Execute";
        public override string SelectScript { get; } = "Select script";

        // --- SearchWindowViewModel ---
        public override string SearchFolder { get; } = "Search folder";
        public override string Standard { get; } = "Standard";
        public override string SearchConditionRuleIsNull { get; } = "Search condition rule is null";
        public override string NoSearchConditions { get; } = "No search conditions";

        // --- TagSearchWindowViewModel ---
        public override string TagIsEmpty { get; } = "Tag is empty";
        public override string TagAlreadyExists { get; } = "Tag already exists";

        // --- ClipboardApp.MainWindowViewModel ---
        public override string DisplayModeWillChangeWhenYouRestartTheApplication { get; } = "Display mode will change when you restart the application";
        public override string Information { get; } = "Information";
        public override string ConfirmExit { get; } = "Confirm exit";
        public override string NoItemSelected { get; } = "No item selected";
        public override string Cut { get; } = "Cut";
        public override string Copied { get; } = "Copied";
        public override string NoPasteFolder { get; } = "No paste folder";
        public override string NoCopyFolder { get; } = "No copy folder";
        public override string ConfirmDeleteSelectedItems { get; } = "Confirm delete selected items";
        public override string Deleted { get; } = "Deleted";

        // --- ImageCHat ---
        public override string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "Confirm if the following sentences are correct or not";
        public override string NoImageFileSelected { get; } = "No image file selected";
        public override string SendPrompt { get; } = "Send prompt";
        public override string ImageFileName { get; } = "Image file name";
        public override string ErrorOccurred { get; } = "Error occurred";
        public override string SelectImageFilePlease { get; } = "Select image file";
        public override string AllFiles { get; } = "All files";
        public override string FileDoesNotExist { get; } = "File does not exist";

        // -- EditPromptItemWindowViewModel --
        public override string EditPrompt { get; } = "Edit prompt";

        // -- ListPromptTemplateWindow -- 
        public override string NewPromptTemplate { get; } = "New prompt template";
        public override string RAG { get; } = "RAG";

        // -- DevFeatures.cs
        public override string CannotMaskNonTextContent { get; } = "Cannot mask non-text content";
        public override string MaskedData { get; } = "Masked data";
        public override string RestoreMaskingData { get; } = "Restore masking data";
        public override string CannotGetImage { get; } = "Cannot get image";

        // -- ScriptAutoProcessItem.cs --
        public override string ExecutedPythonScript { get; } = "Executed Python script";

        // -- SystemAutoProcessItem.cs --
        public override string AutoProcessItemNotFound { get; } = "Auto process item not found";

        // -- EnumDescription.cs --
        public override string NotEnumType { get; } = "Not enum type";

        // --- WindowsNotificationController.cs ---
        public override string AccessDenied { get; } = "Access denied";

        // --- EditPromptItemWindowViewModel ---
        public override string EnterName { get; } = "Enter name";

        // --- EditRAGSourceWindowViewModel ---
        public override string EditRAGSource { get; } = "Edit RAG source";
        public override string ItemViewModelIsNull { get; } = "ItemViewModel is null";

        // EditVectorDBWindowViewModel
        public override string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Only Chroma (in-memory) vector DB type is currently supported";
        public override string PromptTemplateList { get; } = "Prompt template list";
        public override string NoPromptTemplateSelected { get; } = "No prompt template selected";

        // ListVectorDBWindowViewModel
        public override string SelectVectorDBToEdit { get; } = "Select vector DB to edit";
        public override string SelectVectorDBToDelete { get; } = "Select vector DB to delete";
        public override string ConfirmDeleteSelectedVectorDB { get; } = "Confirm delete selected vector DB";
        public override string SelectVectorDBPlease { get; } = "Select vector DB";

        // RAGManagementWindowViewModel
        public override string SelectRAGSourceToEdit { get; } = "Select RAG source to edit";
        public override string SelectRAGSourceToDelete { get; } = "Select RAG source to delete";
        public override string ConfirmDeleteSelectedRAGSource { get; } = "Confirm delete selected RAG source";
        public override string SelectCommitPlease { get; } = "Select commit please";
        public override string SelectTarget { get; } = "Select target";
        public override string ProcessedFileCount { get; } = "Processed file count";
        public override string CreatingIndex { get; } = "Creating index";
        public override string Completed { get; } = "Completed";
        public override string SkipUnsupportedFileType { get; } = "Skip unsupported file type";
        public override string Failed { get; } = "Failed";
        public override string IndexCreationCompleted { get; } = "Index creation completed";
        public override string IndexCreationInterrupted { get; } = "Index creation interrupted";
        public override string FailedToSendChat { get; } = "Failed to send chat";

        public override string PythonSettingCheck { get; } = "Check Python Settings";
        public override string PythonDLLPathNotSet { get; } = "Python DLL Path Not Set";
        public override string PythonDLLPathSet { get; } = "Python DLL Path Set";
        public override string PythonDLLFileDoesNotExist { get; } = "Python DLL File Does Not Exist";
        public override string PythonDLLFileExists { get; } = "Python DLL File Exists";
        public override string TestRunPythonScript { get; } = "Test Run Python Script";
        public override string OpenAISettingCheck { get; } = "Check OpenAI Settings";
        public override string OpenAIKeyNotSet { get; } = "OpenAI Key Not Set";
        public override string OpenAIKeySet { get; } = "OpenAI Key Set";
        public override string OpenAICompletionModelNotSet { get; } = "OpenAI Completion Model Not Set";
        public override string OpenAICompletionModelSet { get; } = "OpenAI Completion Model Set";
        public override string OpenAIEmbeddingModelNotSet { get; } = "OpenAI Embedding Model Not Set";
        public override string OpenAIEmbeddingModelSet { get; } = "OpenAI Embedding Model Set";
        public override string AzureOpenAISettingCheck { get; } = "Check Azure OpenAI Settings";
        public override string AzureOpenAIEndpointNotSet { get; } = "Azure OpenAI Endpoint Not Set";
        public override string SetAzureOpenAIEndpointOrBaseURL { get; } = "Set Azure OpenAI Endpoint or Base URL";
        public override string CannotSetBothAzureOpenAIEndpointAndBaseURL { get; } = "Cannot Set Both Azure OpenAI Endpoint and Base URL";
        public override string TestRunOpenAI { get; } = "Test Run OpenAI";
        public override string FailedToRunPython { get; } = "Failed to Run Python";
        public override string PythonRunIsPossible { get; } = "Python Run is Possible";
        public override string FailedToRunOpenAI { get; } = "Failed to Run OpenAI";
        public override string OpenAIRunIsPossible { get; } = "OpenAI Run is Possible";
        public override string FailedToRunLangChain { get; } = "Failed to Run LangChain";
        public override string LangChainRunIsPossible { get; } = "LangChain Run is Possible";
        public override string ConfirmRun { get; } = "Confirm Run";
        public override string CheckingSettings { get; } = "Checking Settings";
        public override string SettingsSaved { get; } = "Settings Saved";
        public override string Canceled { get; } = "Canceled";
        public override string Log { get; } = "Log";

        public override string Statistics { get; } = "統計";

        public override string NoMatch { get; } = "No Match";
        public override string NoActionSet { get; } = "No Action Set";
        public override string Condition { get; } = "Condition";
        public override string Action { get; } = "Action";
        public override string ActionNone { get; } = "Action None";
        public override string FolderNone { get; } = "Folder None";
        public override string DetectedAnInfiniteLoop { get; } = "Detected an Infinite Loop";
        public override string Clipboard { get; } = "Clipboard";
        public override string ChatHistory { get; } = "Chat History";
        public override string ItemsDeletedOrMovedByAutoProcessing { get; } = "Items Deleted or Moved by Auto Processing";
        public override string AddedItems { get; } = "Added Items";
        public override string ApplyAutoProcessing { get; } = "Apply Auto Processing";
        public override string ItemsDeletedByAutoProcessing { get; } = "Items Deleted by Auto Processing";
        public override string FailedToParseJSONString { get; } = "Failed to Parse JSON String";
        public override string CannotMergeToNonTextItems { get; } = "Cannot Merge to Non-Text Items";
        public override string CannotMergeItemsContainingNonTextItems { get; } = "Cannot Merge Items Containing Non-Text Items";
        public override string CreationDateTime { get; } = "Creation Date Time";
        public override string SourceAppName { get; } = "Source App Name";
        public override string Pinned { get; } = "Pinned";
        public override string CannotGetFolder { get; } = "Cannot Get Folder";
        public override string SaveToFileOnOS { get; } = "Save to File on OS";
        public override string CommittedToGit { get; } = "Committed to Git";
        public override string RepositoryNotFound { get; } = "Repository Not Found";
        public override string CommitIsEmpty { get; } = "Commit is Empty";
        public override string SavedToFileOnOS { get; } = "Saved to File on OS";
        public override string Summary { get; } = "Summary";
        public override string DeleteFileOnOS { get; } = "Delete File on OS";
        public override string DeletedFileOnOS { get; } = "Deleted File on OS";
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
        public override string FailedToRunFile { get; } = "Failed to Run File";
        public override string OpenAsTextFile { get; } = "Open as Text File";
        public override string ChatItem { get; } = "Chat Item";
        public override string ExportTheFollowingItems { get; } = "Export the Following Items";
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
        public override string WorkingDirectory { get; } = "Working Directory";
        public override string RepositoryURL { get; } = "Repository URL";
        public override string LastIndexedCommit { get; } = "Last Indexed Commit";
        public override string NewRAGSource { get; } = "New RAG Source";

        public override string ExecuteAutoProcessingOnImport { get; } = "Execute Auto Processing on Import";
        public override string ExampleGeneralVectorDB { get; } = "Example: General Vector DB for Searching Past Documents Based on User Questions";
        public override string DocumentChunkSize { get; } = "Document Chunk Size";
        public override string VectorSearchResultLimit { get; } = "Vector Search Result Limit";
        public override string UseMultiVectorRetriever { get; } = "Use MultiVectorRetriever";
        public override string SQLite3LocationForDocStore { get; } = "SQLite3 Location for DocStore";
        public override string ExampleSQLite3Location { get; } = "Example: sqlite:///C:\\Users\\Username\\sqlite3.db";
        public override string DocumentChunkSizeForMultiVectorRetriever { get; } = "Document Chunk Size for MultiVectorRetriever";
        public override string ExampleVectorDBLocationChroma { get; } = "Example：C:\\Users\\Username\\vector.db";
        public override string ExampleVectorDBLocationPostgres { get; } = "Example：postgresql+psycopg://langchain:langchain@localhost:5432/langchain";
        public override string ClearChatContents { get; } = "Clear Chat Contents";
        // Clear Content
        public override string ClearContent { get; } = "Clear Content";

        // Reload Content
        public override string ReloadContent { get; } = "Reload Content";

        public override string ExtractedText { get; } = "Extracted Text";

        // タブ削除
        public override string DeleteTab { get; } = "Delete Tab";
        // Export all chat contents
        public override string ExportAllChatContents { get; } = "Export All Chat Contents";

    }
}
