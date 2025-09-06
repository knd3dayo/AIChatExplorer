using LibPythonAI.Common;

namespace LibPythonAI.Resources {
    public class PythonAILibStringResources {

        private static PythonAILibStringResources? _Instance;
        public static PythonAILibStringResources Instance {
            get {
                if (_Instance == null || _LangChanged) {
                    _LangChanged = false;
                    if ( Lang.ToLower().StartsWith("ja")) {
                        _Instance = new PythonAILibStringResourcesJa();
                        // PromptStringResourceもここで設定
                        PromptStringResource.Instance = new PromptStringResourceJa();

                    }else {
                        _Instance = new PythonAILibStringResources();
                        // PromptStringResourceもここで設定
                        PromptStringResource.Instance = new PromptStringResource();
                    }
                }
                return _Instance;
            }
        }

        private static bool _LangChanged = false;
        private static string _Lang = "ja-JP";
        public static string Lang {
            get { return _Lang; }
            set {
                if (_Lang != value) {
                    _LangChanged = true;
                }
                _Lang = value;
            }
        }


        #region LibUIMainから移動
        public virtual string Application { get; } = "Application";
        public virtual string ChatHistory { get; } = "Chat History";
        public virtual string SearchFolder { get; } = "Search folder";
        // BitmapImage Chat
        public virtual string ImageChat { get; } = "Image Chat";
        // Local FileSystem
        public virtual string FileSystem { get; } = "Local FileSystem";

        // Shortcut
        public virtual string Shortcut { get; } = "Shortcut";

        // Outlook
        public virtual string Outlook { get; } = "Outlook";

        // EdgeBrowseHistory
        public virtual string EdgeBrowseHistory { get; } = "Edge Browse History";

        // RecentFiles
        public virtual string RecentFiles { get; } = "Recent Files";

        // ClipboardHistory
        public virtual string ClipboardHistory { get; } = "Clipboard History";

        // ScreenShotHistory
        public virtual string ScreenShotHistory { get; } = "Screen Shot History";

        // IntegratedMonitorHistory
        public virtual string IntegratedMonitorHistory { get; } = "Integrated Monitor History";

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

        public virtual string AutoProcessingIsSet { get; } = "Auto processing is set";

        // 検索条件
        public virtual string SearchCondition { get; } = "Search condition";
        #endregion



        // PythonNotFound
        public virtual string PythonNotFound { get; } = "Python not found. Please check the Python path:";

        // UvNotFound
        public virtual string UvNotFound { get; } = "uv package not found. Please install it with pip install uv.";


        // 自動タグ設定処理を実行します
        public virtual string AutoSetTag { get; } = "Executing automatic tag setting process";

        // モードが不正です
        public virtual string InvalidMode { get; } = "Invalid mode";

        // UpdateVectorDBIndex実行
        public virtual string UpdateVectorDBIndex { get; } = "Execute UpdateVectorDBIndex";

        // VectorSearch実行
        public virtual string VectorSearchExecute { get; } = "Execute VectorSearch";

        // LangChain実行
        public virtual string LangChainExecute { get; } = "Execute LangChain";
        // プロンプト
        public virtual string Prompt { get; } = "Prompt";

        // Pythonを手動でインストールしてください
        public virtual string PythonVenvMaualCreateMessage(IPythonAILibConfigParams configParams) {
            string message = $"""
            Please create a Python Venv environment with the following commands.
            cd {configParams.GetAppDataPath()}
            curl -L https://github.com/knd3dayo/ai_chat_lib/archive/refs/heads/main.zip -o ai_chat_lib.zip
            call powershell -command "Expand-Archive  ai_chat_lib.zip"
            python -m venv {configParams.GetPathToVirtualEnv()}
            call {configParams.GetPathToVirtualEnv()}\Scripts\activate
            pip install ai_chat_lib\ai_chat_lib-main
            """;
            return message;
        }

        // PythonAILibManagerIsNotInitialized
        public virtual string PythonAILibManagerIsNotInitialized { get; } = "PythonAILibManager is not initialized";

        // PythonAILibManagerInitializationTimeout
        public virtual string PythonAILibManagerInitializationTimeout { get; } = "PythonAILibManager initialization timed out.";

        // PythonAILibManagerInitializationFailed
        public virtual string PythonAILibManagerInitializationFailed { get; } = "PythonAILibManager initialization failed. Please check the Python path and environment settings.";

        // Running automatic title setting process
        public virtual string AutoSetTitle { get; } = "Running automatic title setting process";

        // Python venv environment not found. Please check the path to Python venv:
        public virtual string PythonVenvPathNotFound { get; } = "Python venv environment not found. Please check the path to Python venv:";

        // Python venv environment not found. Please create a Python venv:
        public virtual string PythonVenvNotCreated { get; } = "Python venv environment not found. Please create a Python venv.";

        // Python venv environment not found. Would you like to create a Python venv?
        public virtual string ConfirmPythonVenvCreate { get; } = "Python venv environment not found. Would you like to create a Python venv?";

        public virtual string PythonVenvCreationFailed { get; } = "Failed to create Python venv. Please check the Python path.";
        // PythonVenvCreationSuccess
        public virtual string PythonVenvCreationSuccess { get; } = "Successfully created Python venv.";

        // PythonLibsInstallationFailed
        public virtual string PythonLibsInstallationFailed { get; } = "Failed to install Python libraries.";

        // PythonLibsInstallationSuccess
        public virtual string PythonLibsInstallationSuccess { get; } = "Successfully installed Python libraries.";



        // OpenAIKeyNotSet
        public virtual string OpenAIKeyNotSet { get; } = "OpenAI key not set. Please set OpenAIKey from the settings screen.";

        // AppDataPathNotSet
        public virtual string AppDataPathNotSet { get; } = "AppData path not set. Please set AppDataPath from the settings screen.";

        // // Python venv path not set. Please set PythonVenvPath from the settings screen.
        public virtual string PythonVenvPathNotSet { get; } = "Python venv path not set. Please set PythonVenvPath from the settings screen.";

        // // Python関連の初期化処理が行われていません
        public virtual string PythonNotInitialized { get; } = "Python related initialization process has not been performed";

        // No response from OpenAI
        public virtual string OpenAIResponseEmpty { get; } = "No response from OpenAI";

        // File not found
        public virtual string FileNotFound { get; } = "File not found";

        // --- ChatItem.cs ---
        // Reference Information
        public virtual string ReferenceInformation { get; } = "Reference Information";

        public virtual string ReferenceDocument { get; } = "Reference Document";

        // Property information
        public virtual string RequestInfo { get; } = "Request information";

        public virtual string VectorDBItems { get; } = "Vector DB Items";

        // ベクトル検索リクエスト
        public virtual string VectorSearchRequest { get; } = "Vector Search Request";

        // UpdateAutoGenAgentExecute
        public virtual string UpdateAutoGenAgentExecute { get; } = "Update AutoGen Agent Execute";

        // UpdateAutogenLLMConfigExecute
        public virtual string UpdateAutogenLLMConfigExecute { get; } = "Update Autogen LLM Config Execute";

        // DeleteAutoGenAgentExecute
        public virtual string DeleteAutoGenAgentExecute { get; } = "Delete AutoGen Agent Execute";
        // DeleteAutogenLLMConfigExecute
        public virtual string DeleteAutogenLLMConfigExecute { get; } = "Delete Autogen LLM Config Execute";

        // DeleteAutoGenAgentExecute
        public virtual string DeleteAutoGenGroupChatExecute { get; } = "Delete AutoGen Group Chat Execute";

        // UpdateAutoGenGroupChatExecute
        public virtual string UpdateAutoGenGroupChatExecute { get; } = "Update AutoGen Group Chat Execute";
        // UpdateAutoGenToolExecute
        public virtual string UpdateAutoGenToolExecute { get; } = "Update AutoGen Tool Execute";

        // DeleteAutoGenToolExecute
        public virtual string DeleteAutoGenToolExecute { get; } = "Delete AutoGen Tool Execute";

        // Excelへのエクスポートを実行します
        public virtual string ExportToExcelExecute { get; } = "Export to Excel Execute";
        // Excelへのエクスポートが失敗しました
        public virtual string ExportToExcelFailed { get; } = "Export to Excel Failed";
        // Excelへのエクスポートが成功しました
        public virtual string ExportToExcelSuccess { get; } = "Export to Excel Success";

        // ファイルパス
        public virtual string FilePath { get; } = "ファイルパス";
        // データ
        public virtual string Data { get; } = "データ";


        // Output
        public virtual string Response { get; } = "Response";

        // Execute OpenAI
        public virtual string OpenAIExecute { get; } = "Execute OpenAI";

        // GetTokenCountExecute
        public virtual string GetTokenCountExecute { get; } = "Execute GetTokenCount";


        // Chat history (English)
        public virtual string ChatHistoryEnglish { get; } = "Chat history (English)";

        // GetContentItemsExecute
        public virtual string GetContentItemsExecute { get; } = "Execute GetContentItems";

        // GetContentItemsByFolderExecute
        public virtual string GetContentItemsByFolderExecute { get; } = "Execute GetContentItemsByFolder";
        // GetContentItemExecute
        public virtual string GetContentItemExecute { get; } = "Execute GetContentItem";

        // UpdateContentItemsExecute
        public virtual string UpdateContentItemsExecute { get; } = "Execute UpdateContentItems";

        // DeleteContentItemsExecute
        public virtual string DeleteContentItemsExecute { get; } = "Execute DeleteContentItems";

        // SearchContentItemsExecute
        public virtual string SearchContentItemsExecute { get; } = "Execute SearchContentItems";

        // GetRootContentFoldersExecute
        public virtual string GetRootContentFoldersExecute { get; } = "Execute GetRootContentFolders";

        // GetContentFoldersExecute
        public virtual string GetContentFoldersExecute { get; } = "Execute GetContentFolders";

        // GetContentFolderByIdExecute
        public virtual string GetContentFolderByIdExecute { get; } = "Execute GetContentFolderById";

        // GetContentFolderByPathExecute
        public virtual string GetContentFolderByPathExecute { get; } = "Execute GetContentFolderByPath";

        // GetParentContentFolderExecute
        public virtual string GetParentContentFolderExecute { get; } = "Execute GetParentContentFolder";

        // GetChildContentFoldersExecute
        public virtual string GetChildContentFoldersExecute { get; } = "Execute GetChildContentFolders";

        // UpdateContentFoldersExecute
        public virtual string UpdateContentFoldersExecute { get; } = "Execute UpdateContentFolders";

        // DeleteContentFoldersExecute
        public virtual string DeleteContentFoldersExecute { get; } = "Execute DeleteContentFolders";

        // GetAutoProcessRulesExecute
        public virtual string GetAutoProcessRulesExecute { get; } = "Execute GetAutoProcessRules";
        // UpdateAutoProcessRulesExecute
        public virtual string UpdateAutoProcessRulesExecute { get; } = "Execute UpdateAutoProcessRules";

        public virtual string DeleteAutoProcessRulesExecute { get; } = "Execute UpdateContentFolders";

        // GetAutoProcessItemsExecute
        public virtual string GetAutoProcessItemsExecute { get; } = "Execute UpdateContentFolders";

        // UpdateAutoProcessItemsExecute
        public virtual string UpdateAutoProcessItemsExecute { get; } = "Execute UpdateContentFolders";

        // DeleteAutoProcessItemsExecute
        public virtual string DeleteAutoProcessItemsExecute { get; } = "Execute UpdateContentFolders";

        // GetSearchRulesExecute
        public virtual string GetSearchRulesExecute { get; } = "Execute GetSearchRules";
        // UpdateSearchRulesExecute
        public virtual string UpdateSearchRulesExecute { get; } = "Execute UpdateSearchRules";

        // DeleteSearchRulesExecute
        public virtual string DeleteSearchRulesExecute { get; } = "Execute DeleteSearchRules";



        // GetPromptItemsExecute
        public virtual string GetPromptItemsExecute { get; } = "Execute GetPromptItems";
        // UpdatePromptItemsExecute
        public virtual string UpdatePromptItemsExecute { get; } = "Execute UpdatePromptItems";
        // DeletePromptItemsExecute
        public virtual string DeletePromptItemsExecute { get; } = "Execute DeletePromptItems";

        // GetTagItemsExecute
        public virtual string GetTagItemsExecute { get; } = "Execute GetTagItems";

        // UpdateTagItemsExecute
        public virtual string UpdateTagItemsExecute { get; } = "Execute UpdateTagItems";

        // DeleteTagItemsExecute
        public virtual string DeleteTagItemsExecute { get; } = "Execute DeleteTagItems";

        // UpdateVectorDBIndex実行
        public virtual string UpdateEmbeddingExecute { get; } = "Execute UpdateEmbedding";

        // DeleteEmbeddingsByFolderExecute
        public virtual string DeleteEmbeddingsByFolderExecute { get; } = "Execute DeleteEmbeddingsByFolder";

        // DeleteVectorDBIndex実行
        public virtual string DeleteEmbeddingExecute { get; } = "Execute DeleteEmbedding";

        // ベクトルDBアイテムを更新
        public virtual string UpdateVectorDBItemExecute { get; } = "Execute UpdateVectorDBItem";
        // ベクトルDBアイテムを削除
        public virtual string DeleteVectorDBItemExecute { get; } = "Execute DeleteVectorDBItem";

        // GetVectorDBItemsExecute
        public virtual string GetVectorDBItemsExecute { get; } = "Execute GetVectorDBItems";

        // GetVectorDBItemByIdExecute
        public virtual string GetVectorDBItemByIdExecute { get; } = "Execute GetVectorDBItemById";
        // GetVectorDBItemByNameExecute
        public virtual string GetVectorDBItemByNameExecute { get; } = "Execute GetVectorDBItemByName";

        // ベクトルDBのコレクション削除を実行
        public virtual string DeleteVectorDBCollectionExecute { get; } = "Execute DeleteVectorDBCollection";

        // ベクトルDBのコレクション更新を実行
        public virtual string UpdateVectorDBCollectionExecute { get; } = "Execute UpdateVectorDBCollection";
        // ベクトルDBの説明を取得
        public virtual string GetVectorDBDescription { get; } = "Get VectorDB Description";
        // UpdateVectorDBDescription
        public virtual string UpdateVectorDBDescription { get; } = "Update VectorDB Description";


        public virtual string TextExtracted { get; } = "Extracted Text";

        public virtual string UpdateDate { get; } = "Update Date";

        // VectorizedDate
        public virtual string VectorizedDate { get; } = "Vectorized Date";

        public virtual string Title { get; } = "Title";

        public virtual string SourceTitle { get; } = "Source Title";

        // Path
        public virtual string SourcePath { get; } = "Source Path";

        public virtual string Pin { get; } = "Pin";

        // Document reliability
        public virtual string DocumentReliability { get; } = "Document reliability";

        // Document category summary
        public virtual string DocumentCategorySummary { get; } = "Document category summary";

        public virtual string Type { get; } = "Type";

        public virtual string CreationDateTime { get; } = "Creation Date Time";
        public virtual string SourceAppName { get; } = "Source App Name";
        public virtual string Pinned { get; } = "Pinned";

        // Tag
        public virtual string Tag { get; } = "Tag";
        public virtual string BackgroundInformation { get; } = "Background Information";

        // -- ScreenShotCheckCondition.cs --
        public virtual string CheckTypeEqual { get; } = "Equal";
        public virtual string CheckTypeNotEqual { get; } = "Not equal";
        public virtual string CheckTypeInclude { get; } = "Include";
        public virtual string CheckTypeNotInclude { get; } = "Not include";
        public virtual string CheckTypeStartWith { get; } = "Start with";
        public virtual string CheckTypeNotStartWith { get; } = "Not start with";
        public virtual string CheckTypeEndWith { get; } = "End with";
        public virtual string CheckTypeNotEndWith { get; } = "Not end with";
        public virtual string CheckTypeEmpty { get; } = "Empty";
        public virtual string CheckTypeCheckBox { get; } = "Check box";


        public virtual string SettingValueIs(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " is " + SettingValue;
        }
        public virtual string SettingValueIsNot(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " is Not " + SettingValue;
        }
        public virtual string SettingValueContains(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Contains " + SettingValue;
        }
        public virtual string SettingValueNotContain(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Does Not Contain " + SettingValue;
        }
        public virtual string SettingValueStartsWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Starts With " + SettingValue;
        }
        public virtual string SettingValueNotStartWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Does Not Start With " + SettingValue;
        }
        public virtual string SettingValueEndsWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Ends With " + SettingValue;
        }
        public virtual string SettingValueNotEndWith(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " Does Not End With " + SettingValue;
        }
        public virtual string SettingValueIsEmpty(string SettingItem) {
            return "Setting Value of " + SettingItem + " is Empty";
        }
        public virtual string SettingValueIsChecked(string SettingItem, string SettingValue) {
            return "Setting Value of " + SettingItem + " is Checked " + SettingValue;
        }
        public virtual string NoRemoteRepositorySet { get; } = "No remote repository set";
        public virtual string NoWorkingDirectorySpecified { get; } = "No working directory specified";
        public virtual string SpecifiedDirectoryDoesNotExist { get; } = "Specified directory does not exist";
        public virtual string SpecifiedDirectoryIsNotAGitRepository { get; } = "Specified directory is not a Git repository";
        public virtual string NoVectorDBSet { get; } = "No vector DB set";

        // FolderNotFound
        public virtual string FolderNotFound { get; } = "Folder not found";

        public virtual string UnsupportedFileType { get; } = "Unsupported File Type";
        public virtual string SaveEmbedding { get; } = "Save Embedding";
        public virtual string SavedEmbedding { get; } = "Saved Embedding";
        public virtual string DeleteEmbedding { get; } = "Delete Embedding";
        public virtual string DeletedEmbedding { get; } = "Deleted Embedding";
        public virtual string SaveTextEmbeddingFromImage { get; } = "Save Text Embedding from Image";
        public virtual string SavedTextEmbeddingFromImage { get; } = "Saved Text Embedding from Image";
        public virtual string DeleteTextEmbeddingFromImage { get; } = "Delete Text Embedding from Image";
        public virtual string DeletedTextEmbeddingFromImage { get; } = "Deleted Text Embedding from Image";
        public virtual string GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions { get; } = "General Vector DB for Searching Past Documents Based on User Questions";

        // InputContentNotFound
        public virtual string InputContentNotFound { get; } = "Input content not found";

        public virtual string AddedItems { get; } = "Added Items";

        // Auto processing rule
        public virtual string ExtractText { get; } = "Extract text";
        public virtual string Ignore { get; } = "Ignore";
        public virtual string DoNothing { get; } = "Do Nothing";
        public virtual string CopyToFolder { get; } = "Copy to Folder";
        public virtual string CopyClipboardContentToSpecifiedFolder { get; } = "Copy Clipboard Content to Specified Folder";
        public virtual string MoveToFolder { get; } = "Move to Folder";
        public virtual string MoveClipboardContentToSpecifiedFolder { get; } = "Move Clipboard Content to Specified Folder";
        public virtual string ExtractClipboardText { get; } = "Extract Clipboard Text";
        public virtual string DataMasking { get; } = "Data Masking";
        public virtual string MaskClipboardText { get; } = "Mask Clipboard Text";
        public virtual string NoFolderSelected { get; } = "No Folder Selected";
        public virtual string CopyToFolderDescription { get; } = "Copy to Folder Description";
        public virtual string CannotOpenDirectoryAsNewFile { get; } = "Cannot Open Directory as New File";

        // Auto Process Rule
        public virtual string NoMatch { get; } = "No Match";
        public virtual string NoActionSet { get; } = "No Action Set";
        public virtual string Condition { get; } = "Condition";
        public virtual string Action { get; } = "Action";
        public virtual string ActionNone { get; } = "Action None";
        public virtual string FolderNone { get; } = "Folder None";
        public virtual string DetectedAnInfiniteLoop { get; } = "Detected an Infinite Loop";
        public virtual string RuleNameIsInvalid(string RuleName) {
            return RuleName + " is Invalid";
        }
        public virtual string DescriptionContains(string Keyword) {
            return "Description Contains " + Keyword;
        }
        public virtual string ContentContains(string Keyword) {
            return "Content Contains " + Keyword;
        }
        public virtual string SourceApplicationNameContains(string Keyword) {
            return "Source Application Name Contains " + Keyword;
        }
        public virtual string SourceApplicationTitleContains(string Keyword) {
            return "Source Application Title Contains " + Keyword;
        }
        public virtual string SourceApplicationPathContains(string Keyword) {
            return "Source Application Path Contains " + Keyword;
        }

        public virtual string AutoExtractImageText { get; } = "Executing auto image text extraction process";

        // ファイル
        public virtual string File { get; } = "File";

        // フォルダ
        public virtual string Folder { get; } = "Folder";

        public virtual string AutoSetBackgroundInfo { get; } = "Executing auto background information addition process";
        public virtual string AddBackgroundInfoFailed { get; } = "Failed to add background information";

        public virtual string AutoCreateSummary { get; } = "Executing auto summary creation process";

        // Execute automatic document reliability check process
        public virtual string AutoCheckDocumentReliability { get; } = "Executing auto document reliability check process";
        // Document reliability check process failed
        public virtual string CheckDocumentReliabilityFailed { get; } = "Document reliability check process failed";

        public virtual string CreateSummaryFailed { get; } = "Failed to create summary";

        // 自動課題リスト作成処理を実行します
        public virtual string AutoCreateTaskList { get; } = "Execute auto Task list creation process";
        // 課題リスト作成処理が失敗しました
        public virtual string CreateTaskListFailed { get; } = "Failed to create Task list";

        public virtual string ApplyAutoProcessing { get; } = "Apply Auto Processing";

        public virtual string ItemsDeletedByAutoProcessing { get; } = "Items Deleted by Auto Processing";

        // JSON文字列をパースできませんでした
        public virtual string FailedToParseJSONString { get; } = "Failed to parse JSON string";

        // FileSystemFolderPathDisplayName
        public virtual string FileSystemFolderPathDisplayName { get; } = "File System Folder Path";

        public virtual string NoItemSelected { get; } = "No item selected";

        // TextExtractionInProgress
        public virtual string TextExtractionInProgress { get; } = "Text extraction in progress";

        public virtual string CannotExtractTextForNonFileContent { get; } = "Cannot extract text for non-file content";

        // OpenFolderInExplorer
        public virtual string CannotOpenFolderForNonFileContent { get; } = "Cannot open folder for non-file content";

        // ExecuteOpenFolder
        public virtual string ExecuteOpenFolder { get; } = "Execute open folder";

        // ExecuteOpenedFolderSuccess
        public virtual string ExecuteOpenFolderSuccess { get; } = "Execute opened folder success";

        // ExecuteOpenFolderFailed
        public virtual string ExecuteOpenFolderFailed { get; } = "Execute open folder failed";

        // Execute the prompt template [promptName].
        public virtual string PromptTemplateExecute(string promptName) => $"Execute the prompt template [{promptName}].";

        // PromptItemsNotLoaded
        public virtual string PromptItemsNotLoaded { get; } = "Prompt items not loaded. Please load the prompt items first.";

        // プロンプトテンプレート[promptName]を実行中
        public virtual string PromptTemplateInProgress(string promptName) => $"Executing the prompt template [{promptName}].";

        // "The prompt template [promptName] has been executed."
        public virtual string PromptTemplateExecuted(string promptName) => $"The prompt template [{promptName}] has been executed.";

        public virtual string GenerateVectorCompleted { get; } = "Generated vector";

        // PropertyNotSet
        public virtual string PropertyNotSet(string propertyName) {
            return $"Property '{propertyName}' is not set";
        }

        // VectorDBNotFound
        public virtual string VectorDBNotFound(string vectorDBName) {
            return $"Vector DB '{vectorDBName}' not found";
        }

        // Properties
        public virtual string Properties { get; } = "Properties";

        // Text
        public virtual string Text { get; } = "Text";

        // Image
        public virtual string Image { get; } = "Image";

        #region Statistics and Logging

        // Daily token count
        public virtual string DailyTokenCount { get; } = "Daily Token Count";
        // Total token count
        public virtual string TotalTokenFormat(long tokens) {
            return $"Total Token Count: {tokens} tokens";
        }
        // Token count
        public virtual string TokenCount { get; } = "Token Count";

        public virtual string DailyTokenFormat(string date, long totalTokens) {
            return $"Token count for {date}: {totalTokens} tokens";
        }
        #endregion


    }
}
