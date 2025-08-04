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

        // Started Application Watch Message
        public virtual string StartClipboardWatchMessage { get; } = "Clipboard monitoring has started.";
        // Stopped Application Watch Message
        public virtual string StopClipboardWatchMessage { get; } = "Clipboard monitoring has stopped ";

        // Start Screen Watch
        public virtual string StartScreenWatch { get; } = "Start Screen monitoring";
        // Stop Screen Watch
        public virtual string StopScreenWatch { get; } = "Stop Screen monitoring";

        // Start Screen Watch Message
        public virtual string StartScreenWatchMessage { get; } = "Screen monitoring has started.";
        // Stopped Screen Watch Message
        public virtual string StopScreenWatchMessage { get; } = "Screen monitoring has stopped ";

        // Start Integrated Monitor Watch
        public virtual string StartIntegratedMonitorWatch { get; } = "Start Integrated monitoring (Clipboard and Screen)";

        // Stop Integrated Monitor Watch
        public virtual string StopIntegratedMonitorWatch { get; } = "Stop Integrated monitoring";
        // Start Integrated Monitor Message
        public virtual string StartIntegratedMonitorMessage { get; } = "Integrated monitoring has started";
        // StopIntegratedMonitor Message
        public virtual string StopIntegratedMonitorMessage { get; } = " Integrated monitoring has stopped";

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
        // マージチャット
        public virtual string MergeChat { get; } = "Merge Chat";

        // AutoGenChat
        public virtual string AutoGenChat { get; } = "AutoGen Chat";

        // AutoGenChat (In Development)
        public virtual string AutoGenChatInDevelopment { get; } = "AutoGen Chat (In Development)";

        // Normal Chat (In Development)
        public virtual string NormalChatInDevelopment { get; } = "Normal Chat (In Development)";

        // Monitor
        public virtual string Monitor { get; } = "Monitor";

        // ProxyURL
        public virtual string ProxyURL { get; } = "Proxy Server URL";
        // NoProxyList
        public virtual string NoProxyList { get; } = "Proxy Exclusion List";

        // RootFolderNotFound
        public virtual string RootFolderNotFound { get; } = "Root folder not found. Please check the configuration.";

        #endregion

    }
}
