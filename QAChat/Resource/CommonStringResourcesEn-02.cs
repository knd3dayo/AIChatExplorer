using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAChat.Resource {
    public partial class CommonStringResourcesEn : CommonStringResources  {


        #region ClipboardItem関連
        // 新規アイテム
        public override string NewItem { get; } = "New Item";

        #endregion

        #region FolderView related
        // Add my folder to target vector DB
        public override string AddMyFolderToTargetVectorDB { get; } = "Add my folder to target vector DB";


        // Input description of this folder here
        public override string InputDescriptionOfThisFolder { get; } = "Input description of this folder here";

        #endregion

        #region ClipboardApp MainWindow

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

        // Automatically disable wrapping for large text
        public override string AutoTextWrapping { get; } = "Automatically disable wrapping for large text";

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

        #endregion
    }
}
