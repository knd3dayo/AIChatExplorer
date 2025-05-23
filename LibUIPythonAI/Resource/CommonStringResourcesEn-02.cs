using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibUIPythonAI.Resource {
    public partial class CommonStringResourcesEn : CommonStringResources  {


        #region FolderView related
        // Add my folder to target vector DB

        // AddVectorDBForSaveToReferenceVectorDB
        public override string AddVectorDBForSaveToReferenceVectorDB { get; } = "Add VectorDB for save to ReferenceVectorDB";

        // Input description of this folder here
        public override string InputDescriptionOfThisFolder { get; } = "Input description of this folder here";

        #endregion

        #region ToolTip
        // Start: Start Clipboard Watch. Stop: Stop Clipboard Watch.
        public override string ToggleClipboardWatchToolTop { get; } = "Start: Start Clipboard Watch. Stop: Stop Clipboard Watch.";

        #endregion
        #region  MainWindow

        // Start Clipboard Watch
        public override string StartClipboardWatch { get; } = "Start Clipboard Watch";
        // Stop Clipboard Watch
        public override string StopClipboardWatch { get; } = "Stop Clipboard Watch";

        // Started Clipboard Watch
        public override string StartClipboardWatchMessage { get; } = "Started Clipboard Watch";
        // Stopped Clipboard Watch
        public override string StopClipboardWatchMessage { get; } = "Stopped Clipboard Watch";


        // Edit Tag
        public override string EditTag { get; } = "Edit Tag";
        // Edit Auto Process Rule
        public override string EditAutoProcessRule { get; } = "Edit Auto Process Rule";
        // Edit Python Script
        public override string EditPromptTemplate { get; } = "Edit Prompt Template";
        // Edit Git RAG Source
        public override string EditGitRagSource { get; } = "Edit Git RAG Source";

        // AutoGen定義編集
        public override string EditAutoGenDefinition { get; } = "Edit AutoGen Definition";

        // -- View Menu --
        // Wrap text at the right edge
        public override string TextWrapping { get; } = "Wrap text at the right edge";

        // Automatically disable wrapping for large text
        public override string AutoTextWrapping { get; } = "Automatically disable wrapping for large text";


        // Tool
        public override string Tool { get; } = "Tool";
        // OpenAI Chat
        public override string OpenAIChat { get; } = "OpenAI Chat";
        // BitmapImage Chat
        public override string ImageChat { get; } = "Image Chat";

        // マージチャット
        public override string MergeChat { get; } = "Merge Chat";

        // AutoGenChat
        public override string AutoGenChat { get; } = "AutoGen Chat";

        // Monitor
        public override string Monitor { get; } = "Monitor";

        // Local FileSystem
        public override string FileSystem { get; } = "Local FileSystem";

        // Shortcut
        public override string Shortcut { get; } = "Shortcut";

        // Outlook
        public override string Outlook { get; } = "Outlook";

        // EdgeBrowseHistory
        public override string EdgeBrowseHistory { get; } = "Edge Browse History";

        // RecentFiles
        public override string RecentFiles { get; } = "Recent Files";

        // ProxyURL
        public override string ProxyURL { get; } = "Proxy Server URL";
        // NoProxyList
        public override string NoProxyList { get; } = "Proxy Exclusion List";
        #endregion

    }
}
