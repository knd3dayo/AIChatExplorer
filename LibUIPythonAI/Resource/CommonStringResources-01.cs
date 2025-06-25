using WpfAppCommon.Model;

namespace LibUIPythonAI.Resource {
    public  partial class CommonStringResources  {

        // Refresh Vector DB
        public virtual string RefreshVectorDB { get; } = "Refresh Vector DB";

        // Other Prompts
        public virtual string OtherPrompts { get; } = "Other Prompts";

        // Search Folder (English name)
        public virtual string SearchFolderEnglish { get; } = "SearchFolder";

        // Edge Browse History (English name)
        public virtual string EdgeBrowseHistoryEnglish { get; } = "EdgeBrowseHistory";


        // RecentFiles (English name)
        public virtual string RecentFilesEnglish { get; } = "RecentFiles";

        // Clipboard History (English name)
        public virtual string ClipboardHistoryEnglish { get; } = "ClipboardHistory";

        // ScreenShot History (English name)
        public virtual string ScreenShotHistoryEnglish { get; } = "ScreenShotHistory";

        // Integrated Monitor (English name)
        public virtual string IntegratedMonitorHistoryEnglish { get; } = "IntegratedMonitorHistory";

        // Application (English name)
        public virtual string ApplicationEnglish { get; } = "Application";

        // Image Chat (English name)
        public virtual string ImageChatEnglish { get; } = "ImageChat";

        // Local File System (English name)
        public virtual string FileSystemEnglish { get; } = "FileSystem";

        // Shortcut (English name)
        public virtual string ShortcutEnglish { get; } = "Shortcut";

        // Outlook (English name)
        public virtual string OutlookEnglish { get; } = "Outlook";


        // Version Information
        public virtual string VersionInformation { get; } = "Version Information";



        // -- SettingsUserControl.xaml --
        // Restart the application to apply the changes.
        public virtual string RestartAppToApplyChanges { get; } = "Restart the application to apply the changes.";

        // Basic Settings
        public virtual string BasicSettings { get; } = "Basic Settings";

        // 詳細設定
        public virtual string DetailSettings { get; } = "Detail Settings";


        // Specify the python3**.dll location for Python installation
        public virtual string SpecifyPython3Dll { get; } = "Specify the python3**.dll location for Python installation";

        // Python DLL Path
        public virtual string PythonDLLPath { get; } = "Python DLL Path";

        // Application Data Path
        public virtual string AppDataPath { get; } = "Application Data Path";

        // Application Data Path Hint
        public virtual string AppDataPathHint { get; } = "Application Data Path Hint";

        // Python virtual environment location
        public virtual string PythonVenvPath { get; } = "Python virtual environment location";

        // Specify the venv location if using Python venv
        public virtual string PythonVenvPathHint { get; } = "Specify the venv location if using Python venv";

        // Main DB backup generations
        public virtual string ApplicationDBBackupGenerations { get; } = "Main DB backup generations";

        // Backup generations of clipbord.db and clipboard-log.db
        public virtual string ApplicationDBBackupGenerationsDescription { get; } = "Backup generations of clipbord.db and clipboard-log.db";

        // OpenAI Settings
        public virtual string OpenAISettings { get; } = "OpenAI Settings";

        // OpenAI API Key
        public virtual string OpenAIKey { get; } = "OpenAI API Key";

        // Set OpenAI Key for OpenAI or Azure OpenAI
        public virtual string SetOpenAIKey { get; } = "Set OpenAI Key for OpenAI or Azure OpenAI";

        // Use Azure OpenAI
        public virtual string UseAzureOpenAI { get; } = "Use Azure OpenAI";

        // AzureOpenAIAPIVersion
        public virtual string AzureOpenAIAPIVersion { get; } = "AzureOpenAIAPIVersion";

        // Use Azure OpenAI instead of OpenAI
        public virtual string UseAzureOpenAIInsteadOfOpenAI { get; } = "Use Azure OpenAI instead of OpenAI";

        // Azure OpenAI Endpoint
        public virtual string AzureOpenAIEndpoint { get; } = "Azure OpenAI Endpoint";

        // Set Azure OpenAI Endpoint if using Azure OpenAI
        public virtual string SetAzureOpenAIEndpoint { get; } = "Set Azure OpenAI Endpoint if using Azure OpenAI";

        // Model for OpenAI Chat
        public virtual string OpenAIModel { get; } = "Model for OpenAI Chat";

        // Set OpenAI or Azure OpenAI chat model. Example: gpt-4-turbo, gpt-4-1106-preview, etc.
        public virtual string SetOpenAIModel { get; } = "Set OpenAI or Azure OpenAI chat model. Example: gpt-4-turbo, gpt-4-1106-preview, etc.";

        // Model for OpenAI Embedding
        public virtual string OpenAIEmbeddingModel { get; } = "Model for OpenAI Embedding";

        // Set OpenAI or Azure OpenAI embedding model. Example: text-embedding-ada-002, text-embedding-3-small, etc.
        public virtual string SetOpenAIEmbeddingModel { get; } = "Set OpenAI or Azure OpenAI embedding model. Example: text-embedding-ada-002, text-embedding-3-small, etc.";

        // Base URL for OpenAI Chat model
        public virtual string OpenAIBaseURL { get; } = "Base URL for OpenAI Chat model";

        // Set different endpoint for OpenAI Chat model than the default endpoint or Azure OpenAI endpoint
        public virtual string SetOpenAIBaseURL { get; } = "Set different endpoint for OpenAI Chat model than the default endpoint or Azure OpenAI endpoint";

        // Python Spacy Settings
        public virtual string PythonSpacySettings { get; } = "Python Spacy Settings";

        // Spacy Model Name
        public virtual string SpacyModelName { get; } = "Spacy Model Name";

        // Set installed Spacy model name. Example: ja_core_news_sm, ja_core_news_lg, etc.
        public virtual string SetSpacyModelName { get; } = "Set installed Spacy model name. Example: ja_core_news_sm, ja_core_news_lg, etc.";

        // Python OCR Settings
        public virtual string PythonOCRSettings { get; } = "Python OCR Settings";

        // Tesseract Path
        public virtual string TesseractPath { get; } = "Tesseract Path";

        // Other
        public virtual string Other { get; } = "Other";

        // Enable development features
        public virtual string EnableDevelopmentFeatures { get; } = "Enable development features";

        // Check settings
        public virtual string CheckSettings { get; } = "Check settings";

        // PythonSettings
        public virtual string PythonSettings { get; } = "Python Settings";

        // UseExternalAPIServer
        public virtual string UseExternalAPIServer { get; } = "Use External API Server";

        // UseInternalAPIServer
        public virtual string UseInternalAPIServer { get; } = "Use Internal API Server";

        // UsePythonNet
        public virtual string UsePythonNet { get; } = "Use PythonNet";

        // API Server URL
        public virtual string APIServerURL { get; } = "API Server URL";
        // SpecifiedAPIServerURL
        public virtual string SpecifyAPIServerURL { get; } = "Specified API Server URL";

        // InternalAPIServerSettings
        public virtual string InternalAPIServerSettings { get; } = "Internal API Server Settings";

        // ScreenMonitoringInterval
        public virtual string ScreenMonitoringInterval { get; } = "Screen Monitoring Interval";

        // IsAutoPredictUserIntentEnabled
        public virtual string AutoPredictUserIntent { get; } = "Auto Predict User Intent";

        public virtual string AppName { get; } = "AIChatExplorer";
        // File
        public virtual string File { get; } = "File";

        // File / Image
        public virtual string FileOrImage { get; } = "File/Image";

        // チャット内容
        public virtual string ChatContent { get; } = "Chat Content";

        // Create
        public virtual string Create { get; } = "Create";
        // Create Item
        public virtual string CreateItem { get; } = "Create Item";

        // Create Folder
        public virtual string CreateFolder { get; } = "Create Folder";

        // Exit
        public virtual string Exit { get; } = "Exit";
        // Edit
        public virtual string Edit { get; } = "Edit";

        #region Prompt Menu
        public virtual string PromptMenu { get; } = "Prompt Menu";


        // Background Information
        public virtual string BackgroundInformation { get; } = "Background Information";

        #endregion

        // Generate Vector
        public virtual string GenerateVector { get; } = "Generate Vector";

        // Vector Search
        public virtual string VectorSearch { get; } = "Vector Search";

        // Start
        public virtual string Start { get; } = "Start";
        // Stop
        public virtual string Stop { get; } = "Stop";
        // Select
        public virtual string Select { get; } = "Select";
        // Help
        public virtual string Help { get; } = "Help";
        // Version Info
        public virtual string VersionInfo { get; } = "Version Info";

        // ShowProperties
        public virtual string ShowProperties { get; } = "Show Properties";

        // MarkdownView
        public virtual string MarkdownView { get; } = "Markdown View";

        // View
        public virtual string View { get; } = "View";

        // Search
        public virtual string Search { get; } = "Search";
        // Setting
        public virtual string Setting { get; } = "Setting";
        // DeleteAsync
        public virtual string Delete { get; } = "Delete";
        // Add
        public virtual string Add { get; } = "Add";
        // OK
        public virtual string OK { get; } = "OK";
        // Cancel
        public virtual string Cancel { get; } = "Cancel";
        // Close
        public virtual string Close { get; } = "Close";

        // ショートカット登録
        public virtual string CreateShortCut { get; } = "Create ShortCut";

        // Load
        public virtual string Load { get; } = "Load";

        // 同期
        public virtual string Sync { get; } = "Sync";

        // DownloadWebPage
        public virtual string DownloadWebPage { get; } = "Download Web Page";

        // Export/Import
        public virtual string ExportImport { get; } = "Export/Import";

        // Export
        public virtual string ExportToExcel { get; } = "Export to Excel";
        // Import
        public virtual string ImportFromExcel { get; } = "Import from Excel";

        // Standard
        public virtual string Standard { get; } = "Standard";

        // RAG
        public virtual string RAG { get; } = "RAG";

        // ImportFromRULList
        public virtual string ImportFromRULList { get; } = "Import from URL List";

        // ChatMode
        public virtual string ChatMode { get; } = "Chat Mode";

        // OpenAI
        public virtual string ChatMode_OpenAI { get; } = "OpenAI";

        // OpenAI+RAG
        public virtual string ChatMode_OpenAI_RAG { get; } = "OpenAI+RAG";

        // AutoGen GroupChat
        public virtual string ChatMode_AutoGen_GroupChat { get; } = "AutoGen GroupChat";

        // AutoGen NormalChat
        public virtual string ChatMode_AutoGen_NormalChat { get; } = "AutoGen NormalChat";

        // AutoGen NestedChat
        public virtual string ChatMode_AutoGen_NestedChat { get; } = "AutoGen NestedChat";

        // GroupChatName
        public virtual string GroupChatName { get; } = "Group Chat Name";

        // Timeout
        public virtual string Timeout { get; } = "Timeout";

        // TerminateMessage
        public virtual string TerminateMessage { get; } = "Terminate Message";

        // MaxMsg
        public virtual string MaxMsg { get; } = "Max Msg";

        // 分割モード
        public virtual string SplitMode { get; } = "Request Split Mode";

        // 分割モード なし
        public virtual string SplitMode_None { get; } = "None";

        // 分割モード 指定したトークン数を超える場合はリクエストを分割
        public virtual string SplitMode_SplitIfExceedSpecifiedToken { get; } = "Split request if it seems to exceed the specified token count";

        // 指定したトークン数を超える場合はリクエストを分割して要約
        public virtual string SplitMode_SplitAndSummarizeIfExceedSpecifiedToken { get; } = "Split and summarize request if it seems to exceed the specified token count";

        // PromptTextIsNeededWhenSplitModeIsEnabled
        public virtual string PromptTextIsNeededWhenSplitModeIsEnabled { get; } = "Prompt text is needed when split mode is enabled";

        // RAGMode_None ベクトル検索情報は行わない
        public virtual string RAGMode_None { get; } = "No vector search information will be performed";
        // RAGMode_NormalSearch ベクトル検索情報を使用する
        public virtual string RAGMode_NormalSearch { get; } = "Use vector search information";

        // RAGMode_PromptSearch プロンプトを指定してベクトル検索を行う
        public virtual string RAGMode_PromptSearch { get; } = "Perform vector search with specified prompt";

        // NumberOfTokensToSplitRequest
        public virtual string NumberOfTokensToSplitRequest { get; } = "Number of tokens to split request";

        // DataGridIsNotFound
        public virtual string DataGridIsNotFound { get; } = "DataGrid is not found";

        // List of Auto Process Rules
        public virtual string ListAutoProcessRule { get; } = "List of Auto Process Rules";
        // List of Python Scripts
        public virtual string ListPythonScript { get; } = "List of Python Scripts";

        // List of Tags
        public virtual string ListTag { get; } = "List of Tags";

        // New Tag
        public virtual string NewTag { get; } = "New Tag";
        // Tag
        public virtual string Tag { get; } = "Tag";

        // List of Vector DBs
        public virtual string ListVectorDB { get; } = "List of Vector DBs";
        // Edit Vector DB
        public virtual string EditVectorDB { get; } = "Edit Vector DB";

        // --- ToolTip ---

        // Create items in the selected folder.
        public virtual string CreateItemToolTip { get; } = "Create items in the selected folder.";

        // Exit the application.
        public virtual string ExitToolTip { get; } = "Exit the application.";
        // Edit tags.
        public virtual string EditTagToolTip { get; } = "Edit tags.";

        // DeleteAsync selected tags.
        public virtual string DeleteSelectedTag { get; } = "Delete selected tags";
        // Select all.
        public virtual string SelectAll { get; } = "Select all";
        // Unselect all.
        public virtual string UnselectAll { get; } = "Unselect all";

        // --- Window Titles ---

        // List of Auto Process Rules
        public virtual string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // Edit Auto Process Rule
        public virtual string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }

        // Setting
        public virtual string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // Setting Check Result
        public virtual string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - Setting Check Result";
            }
        }

        // List of Vector DBs
        public virtual string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }

        // QA Chat
        public virtual string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // List of Tags
        public virtual string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // Prompt Generation for Screenshot Check
        public virtual string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - Prompt Generation for Screenshot Check";
            }
        }

        // --- DefaultClipboardController.cs ---
        public virtual string AutoProcessing { get; } = "Auto processing in progress";
        public virtual string AddItemFailed { get; } = "Failed to add clipboard item";

        // Execute automatic document reliability check process
        public virtual string AutoCheckDocumentReliability { get; } = "Executing auto document reliability check process";

        // --- PythonNetFunctions.cs ---
        public virtual string PythonDLLNotFound { get; } = "Python DLL not found. Please check the path of Python DLL:";

        // -- MainWindowDataGrid1 --
        public virtual string UpdateDate { get; } = "Update Date";

        // Create Date
        public virtual string CreateDate { get; } = "Create Date";
        // VectorizedDate
        public virtual string VectorizedDate { get; } = "Vectorized Date";

        public virtual string Title { get; } = "Title";

        public virtual string SourceTitle { get; } = "Source Title";
        public virtual string Pin { get; } = "Pin";

        public virtual string Type { get; } = "Type";

        // -- AutoProcessRule --
        public virtual string RuleName { get; } = "Rule Name";

        public virtual string Enable { get; } = "Enable";

        public virtual string TargetFolder { get; } = "Target Folder";

        public virtual string ApplyAllItems { get; } = "Apply to all items";

        public virtual string ApplyMatchedItems { get; } = "Apply to matched items";

        public virtual string ItemType { get; } = "Item Type";
        public virtual string ItemTypeText { get; } = "Item Type is Text";
        public virtual string LineOrMore { get; } = "Lines or more";

        public virtual string LineOrLess { get; } = "Lines or less";

        public virtual string ItemTypeFile { get; } = "Item Type is File";

        public virtual string ItemTypeImage { get; } = "Item Type is Image";

        public virtual string TitleContains { get; } = "Title contains";

        public virtual string BodyContains { get; } = "Body contains";

        public virtual string SourceAppContains { get; } = "Source App contains";

        public virtual string ExecuteProcess { get; } = "Execute process";

        public virtual string ExecuteNextProcess { get; } = "Execute next process";

        public virtual string CopyMoveMergeTarget { get; } = "Copy/Move/Merge target";

        public virtual string ExecutePythonScript { get; } = "Execute Python script";

        public virtual string ExecuteOpenAI { get; } = "Execute OpenAI prompt";

        public virtual string OpenAIMode { get; } = "OpenAI execution mode";

        public virtual string TargetFolderFullPath { get; } = "Target Folder (Full Path)";

        public virtual string FolderUnit { get; } = "Folder Unit";

        public virtual string Up { get; } = "Up";
        public virtual string Down { get; } = "Down";

        public virtual string SourceApp { get; } = "Source App for Clipboard Monitoring";

        public virtual string SourceAppExample { get; } = "Enter the names of the apps to monitor, separated by commas. Example: notepad.exe,Teams.exe";

        public virtual string IgnoreTextLessOrEqualToSpecifiedLines { get; } = "Ignore text items with specified lines or less";

        public virtual string AutoTitleGeneration { get; } = "Auto Title Generation";

        public virtual string DoNot { get; } = "Do not";

        public virtual string AutomaticallyGenerateTitleUsingOpenAI { get; } = "Automatically generate title using OpenAI";

        public virtual string AutomaticallyGenerateTags { get; } = "Automatically generate tags";

        public virtual string AutomaticallyGenerateTagsFromContent { get; } = "Automatically generate tags from content";

        public virtual string AutomaticallyExtractTextFromFile { get; } = "Automatically extract text from file";

        public virtual string AutomaticallyExtractTextFromFileIfApplicationItemIsFile { get; } = "Automatically extract text from file if clipboard item is file";

        public virtual string AutomaticallyExtractTextFromImage { get; } = "Automatically extract text from image";

        public virtual string ExtractTextUsingPyOCR { get; } = "Extract text using PyOCR";

        public virtual string ExtractTextUsingOpenAI { get; } = "Extract text using OpenAI";

        public virtual string EmbeddingWhenExtractingTextFromImage { get; } = "Embedding when extracting text from image";
        public virtual string EmbeddingWhenExtractingTextFromImageDescription { get; } = "Embedding when extracting text from image";

        public virtual string AutomaticallyAddBackgroundInformation { get; } = "Automatically add background information";

        public virtual string AutomaticallyGenerateSummary { get; } = "Automatically generate summary";

        public virtual string GenerateSummaryTextFromContent { get; } = "Generate summary text from content";

        // Automatically generate Task list
        public virtual string AutomaticallyGenerateTaskList { get; } = "Automatically generate Task list";

        // Generate Task list from content
        public virtual string GenerateTaskListFromContent { get; } = "Generate Task list from content";

        // Checks the document reliability of the content.
        public virtual string CheckDocumentReliabilityOfContent { get; } = "Checks the document reliability of the content.";

        public virtual string EntityExtractionDataMasking { get; } = "Entity Extraction / Data Masking";

        public virtual string ExtractEntitiesAndMaskDataUsingSpacyFromClipboardContent { get; } = "Extract entities and mask data using Spacy from clipboard content";

        public virtual string MaskPersonalInformationInDataSentToOpenAI { get; } = "Mask personal information in data sent to OpenAI";

        public virtual string NewAutoProcessRule { get; } = "New Auto Process Rule";

        public virtual string SaveSystemCommonSettings { get; } = "Save system common settings";

        // -- FolderEditWindow --
        public virtual string EditApplicationFolder { get; } = "Edit Application Folder";

        public virtual string Name { get; } = "Name";

        public virtual string Description { get; } = "Description";

        // 別のプロンプト結果を入力にする
        public virtual string PromptResultToInput { get; } = "Use another prompt result as input";

        // 自動処理時の設定
        public virtual string AutoProcessSetting { get; } = "Auto Process Setting";

        // 入力となるプロンプトテンプレート
        public virtual string PromptInputName { get; } = "Prompt Input Name";

        // ベクトルDBを使用する
        public virtual string UseVectorDB { get; } = "Use Vector DB";

        // フォルダ設定を使用
        public virtual string UseVectorDBSettingInFolder { get; } = "Use Vector DB setting in folder";
        // 出力形式
        public virtual string OutputType { get; } = "Output Type";
        // 文字列
        public virtual string StringType { get; } = "String";
        // リスト
        public virtual string ListType { get; } = "List";
        // テーブル
        public virtual string TableType { get; } = "Table";

        // Dictionary
        public virtual string DictionaryType { get; } = "Dictionary";
        // 出力先
        public virtual string OutputDestination { get; } = "Output Destination";
        // 新規タブ
        public virtual string NewTab { get; } = "New Tab";
        // 本文を上書き
        public virtual string OverwriteContent { get; } = "Overwrite Content";
        // タイトルを上書き
        public virtual string OverwriteTitle { get; } = "Overwrite Title";

        // タグ追加
        public virtual string AppendTags { get; } = "Add Tag";
        public virtual string SelectFolder { get; } = "Select Folder";

        public virtual string SelectFile { get; } = "Select File";

        // SelectApplicationFolder
        public virtual string SelectApplicationFolder { get; } = "Select Application Folder";

        // SelectedApplicationFolder
        public virtual string SelectedApplicationFolder { get; } = "Selected Application Folder";

        // -- EditItemWindow --
        public virtual string OpenTextAsFile { get; } = "Open text as file";
        public virtual string OpenFile { get; } = "Open file";
        public virtual string OpenAsNewFile { get; } = "Open as new file";
        public virtual string OpenFolder { get; } = "Open folder";
        public virtual string ExtractText { get; } = "Extract text";
        public virtual string MaskData { get; } = "Mask data";
        public virtual string ClickHereToOpenTheTagEditScreen { get; } = "Click here to open the tag edit screen";

        // Properties
        public virtual string Properties { get; } = "Properties";
        public virtual string Text { get; } = "Text";
        public virtual string FilePath { get; } = "File path";
        public virtual string Folder { get; } = "Folder";
        public virtual string FileName { get; } = "File name";
        public virtual string FolderNameAndFileName { get; } = "Folder name and file name";
        public virtual string Image { get; } = "Image";

        // -- EditPythonScriptWindow --
        public virtual string Content { get; } = "Content";

        // -- ListPythonScriptWindow --
        public virtual string NewPythonScript { get; } = "New Python script";

        // -- SearchWindow --
        // 検索対象
        public virtual string SearchTarget { get; } = "Search target";

        // 全フォルダ
        public virtual string AllFolders { get; } = "All folders";

        // 検索条件
        public virtual string SearchCondition { get; } = "Search condition";

        public virtual string SearchTargetFolder { get; } = "Search target folder";
        public virtual string Exclude { get; } = "Exclude";
        public virtual string CopySourceAppName { get; } = "Copy source app name";
        public virtual string StartDate { get; } = "Start date";
        public virtual string EndDate { get; } = "End date";
        public virtual string IncludeSubfolders { get; } = "Include subfolders";
        public virtual string Clear { get; } = "Clear";

        // -- TagSearchWindow
        public virtual string TagSearch { get; } = "Tag search";

        // -- VectorSearchResultWindow
        public virtual string VectorSearchResult { get; } = "Vector search result";

        // VectorDBSearchScoreThreashold
        public virtual string VectorDBSearchScoreThreashold { get; } = "Vector DB search score threshold";
        // -- ImageChatWindow
        public virtual string SettingItem { get; } = "Setting item";
        public virtual string SettingValue { get; } = "Setting value";
        public virtual string CheckType { get; } = "Check type";
        public virtual string Paste { get; } = "Paste";

        // -- ImageCheck.MainWindow --
        public virtual string SelectImageFile { get; } = "Select image file";
        public virtual string EditImageEvidenceCheckItem { get; } = "Edit image evidence check item";
        public virtual string Open { get; } = "Open";
        public virtual string TheAnswerWillBeDisplayedHere { get; } = "The answer will be displayed here";
        public virtual string EnterYourQuestionHere { get; } = "Enter your question here";
        public virtual string Save { get; } = "Save";
        public virtual string Send { get; } = "Send";

        // -- ListVectorDBWindow --
        public virtual string DisplayVectorsForTheSystem { get; } = "Display vectors for the system";

        public virtual string DisplayPromptsForTheSystem { get; } = "Display prompts for the system";

        public virtual string VectorDBLocation { get; } = "Vector DB location";
        public virtual string VectorDBType { get; } = "Vector DB type";
        public virtual string NewVectorDBSetting { get; } = "New vector DB setting";
        public virtual string EditVectorDBSetting { get; } = "Edit vector DB setting";

        // -- QAChatControl --
        public virtual string ExperimentalFunction1 { get; } = "Experimental function 1";
        public virtual string VectorDBFolder { get; } = "Vector DB (folder)";

        // ベクトルDB選択
        public virtual string SelectVectorDB { get; } = "Select Vector DB";

        // VectorDBSearchAgent
        public virtual string VectorDBSearchAgent { get; } = "Vector DB Search Agent";

        public virtual string ExcludeFromList { get; } = "Exclude from list";
        public virtual string VectorDB { get; } = "Vector DB";

        // CollectionName
        public virtual string CollectionName { get; } = "Collection Name";

        public virtual string ClickHereToAddVectorDB { get; } = "Click here to add Vector DB";

        // ベクトルDB検索結果の最大値
        public virtual string VectorDBSearchResultMax { get; } = "Maximum value of Vector DB search results";
        public virtual string ImageFile { get; } = "Image file";
        public virtual string ClickHereToAddImageFile { get; } = "Click here to add image file";
        public virtual string Chat { get; } = "Chat";
        public virtual string PromptTemplate { get; } = "Prompt template";
        public virtual string RequestParameterJson { get; } = "Request Parameters (JSON)";
        public virtual string Copy { get; } = "Copy";

        // --- ApplicationFolderViewModel ---
        public virtual string AutoProcessingIsSet { get; } = "Auto processing is set";
        public virtual string FolderEdited { get; } = "Folder edited";
        public virtual string Reloaded { get; } = "Reloaded";
        public virtual string SelectFilePlease { get; } = "Select file, please";
        public virtual string Confirm { get; } = "Confirm";
        public virtual string ConfirmDeleteFolder { get; } = "Confirm delete folder?";
        public virtual string FolderDeleted { get; } = "Folder deleted";
        public virtual string ConfirmDeleteItems { get; } = "Confirm delete items?";
        public virtual string DeletedItems { get; } = "Deleted items";
        public virtual string Added { get; } = "Added";
        public virtual string Edited { get; } = "Edited";
        public virtual string Pasted { get; } = "Pasted";
        public virtual string ErrorOccurredAndMessage { get; } = "Error occurred.\nMessage";
        public virtual string StackTrace { get; } = "Stack trace";

        // チャット結果を保存しました
        public virtual string SavedChatResult { get; } = "Chat result saved";

        // --- ApplicationItemViewModel ---
        public virtual string GenerateBackgroundInformation { get; } = "Generate background information";

        // Other prompts
        public virtual string EnterANumber { get; } = "Enter a number";
        public virtual string FolderNotSelected { get; } = "Folder not selected";
        public virtual string EnterRuleName { get; } = "Enter rule name";
        public virtual string SelectAction { get; } = "Select action";
        public virtual string RuleNotFound { get; } = "Rule not found";
        public virtual string SelectCopyOrMoveTargetFolder { get; } = "Select copy or move target folder";
        public virtual string CannotCopyOrMoveToTheSameFolder { get; } = "Cannot copy or move to the same folder";
        public virtual string DetectedAnInfiniteLoopInCopyMoveProcessing { get; } = "Detected an infinite loop in copy/move processing";
        public virtual string SelectPromptTemplate { get; } = "Select prompt template";

        // --- EditPythonScriptWindowViewModel ---
        public virtual string EnterDescription { get; } = "Enter description";

        // --- FolderEditWindowViewModel ---
        public virtual string EnterFolderName { get; } = "Enter folder name";

        // --- FolderSelectWindowViewModel ---
        public virtual string SelectedFolderNotFound { get; } = "Error occurred. Selected folder not found";

        // --- ListAutoProcessRuleWindowViewModel ---
        public virtual string AutoProcessRuleNotSelected { get; } = "Auto process rule not selected";
        public virtual string ConfirmDelete { get; } = "Confirm delete";

        // --- ListPythonScriptWindowViewModel ---
        public virtual string Execute { get; } = "Execute";
        // --- SearchWindowViewModel ---
        public virtual string SearchFolder { get; } = "Search folder";
        public virtual string SearchConditionRuleIsNull { get; } = "Search condition rule is null";
        public virtual string NoSearchConditions { get; } = "No search conditions";

        // --- TagSearchWindowViewModel ---
        public virtual string TagIsEmpty { get; } = "Tag is empty";
        public virtual string TagAlreadyExists { get; } = "Tag already exists";

        // **********************************************************************************
        public virtual string Information { get; } = "Information";
        public virtual string ConfirmExit { get; } = "Confirm exit";


        // // マージチャット 事前処理実行中
        public virtual string MergeChatPreprocessingInProgress { get; } = "Preprocessing merge chat";

        // TextExtractionCompleted
        public virtual string TextExtractionCompleted { get; } = "Text extraction completed";

        public virtual string Cut { get; } = "Cut";
        public virtual string Copied { get; } = "Copied";
        public virtual string NoPasteFolder { get; } = "No paste folder";
        public virtual string NoCopyFolder { get; } = "No copy folder";
        public virtual string ConfirmDeleteSelectedItems { get; } = "Confirm delete selected items";
        public virtual string Deleted { get; } = "Deleted";

        // --- ImageCHat ---
        public virtual string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "Confirm if the following sentences are correct or not";
        public virtual string NoImageFileSelected { get; } = "No image file selected";
        public virtual string ErrorOccurred { get; } = "Error occurred";
        public virtual string SelectImageFilePlease { get; } = "Select image file";
        public virtual string AllFiles { get; } = "All files";
        public virtual string FileDoesNotExist { get; } = "File does not exist";

        // -- EditPromptItemWindowViewModel --
        public virtual string EditPrompt { get; } = "Edit prompt";

        // -- ListPromptTemplateWindow -- 
        public virtual string NewPromptTemplate { get; } = "New prompt template";

        // --- EditPromptItemWindowViewModel ---
        public virtual string EnterName { get; } = "Enter name";


        // EditVectorDBWindowViewModel
        public virtual string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Only Chroma (in-memory) vector DB type is currently supported";
        public virtual string PromptTemplateList { get; } = "Prompt template list";
        public virtual string NoPromptTemplateSelected { get; } = "No prompt template selected";

        // ListVectorDBWindowViewModel
        public virtual string SelectVectorDBToEdit { get; } = "Select vector DB to edit";
        public virtual string SelectVectorDBToDelete { get; } = "Select vector DB to delete";
        public virtual string ConfirmDeleteSelectedVectorDB { get; } = "Confirm delete selected vector DB";
        public virtual string SelectVectorDBPlease { get; } = "Select vector DB";

        public virtual string FailedToSendChat { get; } = "Failed to send chat";

        public virtual string PythonSettingCheck { get; } = "Check Python Settings";

        // PythonVenvPathNotSet
        public virtual string PythonVenvPathNotSet { get; } = "Python virtual environment path is not set";

        // PythonVenvPathSet
        public virtual string PythonVenvPathSet { get; } = "Python virtual environment path is set";

        // PythonVenvNotCreated
        public virtual string PythonVenvNotCreated { get; } = "Python virtual environment is not created";
        // PythonVenvCreated
        public virtual string PythonVenvCreated { get; } = "Python virtual environment is created";

        // APIServerURLCheck
        public virtual string APIServerURLCheck { get; } = "Check API server URL";
        // APIServerURLNotSet
        public virtual string APIServerURLNotSet { get; } = "API server URL is not set";

        // APIServerURLSet
        public virtual string APIServerURLSet { get; } = "API server URL is set";


        public virtual string PythonDLLPathNotSet { get; } = "Python DLL path is not set";
        public virtual string PythonDLLPathSet { get; } = "Python DLL Path Set";
        public virtual string PythonDLLFileExists { get; } = "Python DLL File Exists";
        public virtual string TestRunPythonScript { get; } = "Test Run Python Script";
        public virtual string OpenAISettingCheck { get; } = "Check OpenAI Settings";
        public virtual string OpenAIKeyNotSet { get; } = "OpenAI Key Not Set";
        public virtual string OpenAIKeySet { get; } = "OpenAI Key Set";
        public virtual string OpenAICompletionModelNotSet { get; } = "OpenAI Completion Model Not Set";
        public virtual string OpenAICompletionModelSet { get; } = "OpenAI Completion Model Set";
        public virtual string OpenAIEmbeddingModelNotSet { get; } = "OpenAI Embedding Model Not Set";
        public virtual string OpenAIEmbeddingModelSet { get; } = "OpenAI Embedding Model Set";
        public virtual string AzureOpenAISettingCheck { get; } = "Check Azure OpenAI Settings";
        public virtual string AzureOpenAIEndpointNotSet { get; } = "Azure OpenAI Endpoint Not Set";
        public virtual string CannotSetBothAzureOpenAIEndpointAndBaseURL { get; } = "Cannot Set Both Azure OpenAI Endpoint and Base URL";
        public virtual string TestRunOpenAI { get; } = "Test Run OpenAI";
        public virtual string FailedToRunPython { get; } = "Failed to Run Python";
        public virtual string PythonRunIsPossible { get; } = "Python Run is Possible";
        public virtual string FailedToRunOpenAI { get; } = "Failed to Run OpenAI";
        public virtual string OpenAIRunIsPossible { get; } = "OpenAI Run is Possible";
        public virtual string ConfirmRun { get; } = "Confirm Run";
        public virtual string CheckingSettings { get; } = "Checking Settings";
        public virtual string SettingsSaved { get; } = "Settings Saved";
        public virtual string Canceled { get; } = "Canceled";
        public virtual string Log { get; } = "Log";

        public virtual string Statistics { get; } = "Statistics";

        public virtual string Application { get; } = "Application";
        public virtual string ChatHistory { get; } = "Chat History";
        public virtual string ChatItem { get; } = "Chat Item";
        public virtual string ExportTheFollowingItems { get; } = "Export the Following Items";
        public virtual string ExecuteAutoProcessingOnImport { get; } = "Execute Auto Processing on Import";
        public virtual string ExampleGeneralVectorDB { get; } = "Example: General Vector DB for Searching Past Documents Based on User Questions";
        public virtual string DocumentChunkSize { get; } = "Document Chunk Size";

        // ベクトル検索結果のデフォルト上限値
        public virtual string DefaultSearchResultLimit { get; } = "Default Search Result Limit";

        // ベクトルのスコア(コサイン類似度)の閾値
        public virtual string DefaultScoreThreshold { get; } = "Default Score Threshold";
        public virtual string UseMultiVectorRetriever { get; } = "Use MultiVectorRetriever";
        public virtual string SQLite3LocationForDocStore { get; } = "SQLite3 Location for DocStore";
        public virtual string ExampleSQLite3Location { get; } = "Example: sqlite:///C:\\Users\\Username\\sqlite3.db";
        public virtual string ExampleVectorDBLocationChroma { get; } = "Example：C:\\Users\\Username\\vector.db";
        public virtual string ExampleVectorDBLocationPostgres { get; } = "Example：postgresql+psycopg://langchain:langchain@localhost:5432/langchain";
        public virtual string ClearChatContents { get; } = "Clear Chat Contents";
        // Clear Path
        public virtual string ClearContent { get; } = "Clear Content";

        // Reload Path
        public virtual string ReloadContent { get; } = "Reload Content";

        public virtual string ExtractedText { get; } = "Extracted Text";

        // デバッグ用のコマンド生成
        public virtual string GenerateDebugCommand { get; } = "Generate Debug Command";

        // ExecuteDebugCommand
        public virtual string ExecuteDebugCommand { get; } = "Execute Debug Command";

        // VectorDBSettings
        public virtual string VectorDBSettings { get; } = "Vector DB Settings";

        // // ファイルの内容を表示するには「テキストを抽出」を実行してください
        public virtual string ExecuteExtractTextToViewFileContent { get; } = "To view the contents of the file, execute \"Extract Text\"";

        // Webページの内容を表示するには「Webページをダウンロード」を実行してください。
        public virtual string ExecuteDownloadWebPageToViewContent { get; } = "To view the contents of the web page, execute \"Download Web Page\"";

        // "Chat: "
        public virtual string ChatHeader { get; } = "Chat: ";

        // タイトルなし
        public virtual string NoTitle { get; } = "No Title";

        // サマリー
        public virtual string Summary { get; } = "Summary";

        
    }
}
