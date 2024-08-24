namespace WpfAppCommon.Model {
    public partial class CommonStringResourcesEn : CommonStringResources {

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
        // Create
        public override string Create { get; } = "Create";
        // Create Item
        public override string CreateItem { get; } = "Create Item";
        // Exit
        public override string Exit { get; } = "Exit";
        // Edit
        public override string Edit { get; } = "Edit";

        // Generate Title
        public override string GenerateTitle { get; } = "Generate Title";

        // Generate Background Info
        public override string GenerateBackgroundInfo { get; } = "Generate Background Info";

        // Generate Summary
        public override string GenerateSummary { get; } = "Generate Summary";

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
        // Image Chat
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
    }
}
