using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibUIPythonAI.Resource {
    public partial class CommonStringResources  {


        #region FolderView related
        // Add my folder to target vector DB

        // AddVectorDBForSaveToReferenceVectorDB
        public virtual string AddVectorDBForSaveToReferenceVectorDB { get; } = "Add VectorDB for save to ReferenceVectorDB";

        // Input description of this folder here
        public virtual string InputDescriptionOfThisFolder { get; } = "Input description of this folder here";

        #endregion

        #region ToolTip
        // Start: Start Application Watch. Stop: Stop Application Watch.
        public virtual string ToggleClipboardWatchToolTop { get; } = "Start: Start Clipboard monitoring. Stop: Stop Clipboard monitoring.";

        // Start: Start Screen Watch. Stop: Stop Screen Watch.
        public virtual string ToggleScreenWatchToolTop { get; } = "Start: Start Screen monitoring. Stop: Stop Screen monitoring.";

        // Start: Start Integrated Monitor Watch. Stop: Stop Integrated Monitor Watch.
        public virtual string ToggleIntegratedMonitorToolTop { get; } = "Start: Start Integrated monitoring (Clipboard and Screen). Stop: Stop Integrated monitoring.";

        #endregion
        #region  MainWindow

        // Start Application Watch
        public virtual string StartClipboardWatch { get; } = "Start Clipboard monitoring";
        // Stop Application Watch
        public virtual string StopClipboardWatch { get; } = "Stop Clipboard monitoring";

        // Started Application Watch
        public virtual string StartClipboardWatchMessage { get; } = "Clipboard monitoring has started.";
        // Stopped Application Watch
        public virtual string StopClipboardWatchMessage { get; } = "Clipboard monitoring has stopped ";

        // Start Screen Watch
        public virtual string StartScreenWatch { get; } = "Start Screen monitoring";
        // Stop Screen Watch
        public virtual string StopScreenWatch { get; } = "Stop Screen monitoring";

        // Started Screen Watch
        public virtual string StartScreenWatchMessage { get; } = "Screen monitoring has started.";
        // Stopped Screen Watch
        public virtual string StopScreenWatchMessage { get; } = "Screen monitoring has stopped ";

        // Start Integrated Monitor
        public virtual string StartIntegratedMonitorMessage { get; } = "Start Integrated monitoring (Clipboard and Screen)";
        // StopIntegratedMonitor
        public virtual string StopIntegratedMonitorMessage { get; } = "Stop Integrated monitoring";

        // Edit Tag
        public virtual string EditTag { get; } = "Edit Tag";
        // Edit Auto Process Rule
        public virtual string EditAutoProcessRule { get; } = "Edit Auto Process Rule";
        // Edit Python Script
        public virtual string EditPromptTemplate { get; } = "Edit Prompt Template";
        // Edit Git RAG Source
        public virtual string EditGitRagSource { get; } = "Edit Git RAG Source";

        // AutoGen定義編集
        public virtual string EditAutoGenDefinition { get; } = "Edit AutoGen Definition";

        // -- View Menu --
        // Wrap text at the right edge
        public virtual string TextWrapping { get; } = "Wrap text at the right edge";

        // Automatically disable wrapping for large text
        public virtual string AutoTextWrapping { get; } = "Automatically disable wrapping for large text";


        // Tool
        public virtual string Tool { get; } = "Tool";
        // OpenAI Chat
        public virtual string OpenAIChat { get; } = "OpenAI Chat";
        // BitmapImage Chat
        public virtual string ImageChat { get; } = "Image Chat";

        // マージチャット
        public virtual string MergeChat { get; } = "Merge Chat";

        // AutoGenChat
        public virtual string AutoGenChat { get; } = "AutoGen Chat";

        // Monitor
        public virtual string Monitor { get; } = "Monitor";

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

        // ProxyURL
        public virtual string ProxyURL { get; } = "Proxy Server URL";
        // NoProxyList
        public virtual string NoProxyList { get; } = "Proxy Exclusion List";
        #endregion

    }
}
