using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibUIPythonAI.Resource {
    public partial class CommonStringResources  {

        #region QAChatControl
        // Export all chat contents
        public virtual string ExportAllChatContents { get; } = "Export All Chat Contents";
        // Copy selected chat contents
        public virtual string CopySelectedChatContents { get; } = "Copy Selected Chat Contents";
        // Copy all chat contents
        public virtual string CopyAllChatContents { get; } = "Copy All Chat Contents";

        #endregion

        #region AutoGen
        // AutoGenSettingList
        public virtual string AutoGenSettingList { get; } = "AutoGenSettingList";
        // EditAutoGenAgentWindowTitle
        public virtual string EditAutoGenAgentWindowTitle { get; } = "EditAutoGenAgentWindowTitle";

        // EditAutoGenToolWindowTitle
        public virtual string EditAutoGenToolWindowTitle { get; } = "EditAutoGenToolWindowTitle";

        // EditAutoGenLLMConfigWindowTitle
        public virtual string EditAutoGenLLMConfigWindowTitle { get; } = "EditAutoGenLLMConfigWindowTitle";

        // Tools
        public virtual string Tools { get; } = "ToolsForLLM";
        // ToolsForExecution
        public virtual string ToolsForExecution { get; } = "ToolsForExecution";
        // SystemMessage
        public virtual string SystemMessage { get; } = "SystemMessage";
        // GroupChat
        public virtual string GroupChat { get; } = "Group Chat";
        // NormalChat
        public virtual string NormalChat { get; } = "Normal Chat";
        // NestedChat
        public virtual string NestedChat { get; } = "Nested Chat";
        // Agent
        public virtual string Agent { get; } = "Agent";
        // Path
        public virtual string SourcePath { get; } = "Source Path";
        // InitialAgent
        public virtual string InitialAgent { get; } = "Initial Agent";
        // AgentType
        public virtual string AgentType { get; } = "Agent Type";
        // UserProxy
        public virtual string UserProxy { get; } = "User Proxy";
        // Assistant
        public virtual string Assistant { get; } = "Assistant";
        // HumanInputMode
        public virtual string HumanInputMode { get; } = "Human Input Mode";
        // NEVER
        public virtual string Never { get; } = "NEVER";
        // ALWAYS
        public virtual string Always { get; } = "ALWAYS";
        // TERMINATE
        public virtual string Terminate { get; } = "TERMINIATE";
        // TERMINATEMsg
        public virtual string TerminateMsg { get; } = "TerminateMsg";
        // CodeExecution
        public virtual string CodeExecution { get; } = "Code Execution";
        // LLMConfig
        public virtual string LLMConfig { get; } = "LLM Config";

        // LLM
        public virtual string LLM { get; } = "LLM";
        // AddVectorDB
        public virtual string AddVectorDB { get; } = "Add Vector DB";

        // ApiType
        public virtual string ApiType { get; } = "Api Type";

        // Model
        public virtual string Model { get; } = "Model";

        // BaseURL
        public virtual string BaseURL { get; } = "Base URL";

        // MaxRounds
        public virtual string MaxRounds { get; } = "Max Rounds";

        #endregion

        // Enabled
        public virtual string Enabled { get; } = "Enabled";

        #region folder
        // ExportTarget
        public virtual string ExportTarget { get; } = "Export Target";

        // ImportTarget
        public virtual string ImportTarget { get; } = "Import Target";

        // SaveVectorDB
        public virtual string SaveVectorDB { get; } = "Save Vector DB";

        // ReferenceVectorDB
        public virtual string ReferenceVectorDB { get; } = "Reference Vector DB";

        #endregion

        #region RAG
        // CommitDateTime
        public virtual string CommitDateTime { get; } = "Commit Date Time";

        // CommitHash
        public virtual string CommitHash { get; } = "Commit Hash";

        // Message
        public virtual string Message { get; } = "Message";

        // UpdateRAGIndexWindow
        public virtual string UpdateRAGIndexWindow { get; } = "Update RAG Index Window";

        // GitRepositoryURL
        public virtual string GitRepositoryURL { get; } = "Git Repository URL";

        // UpdateEmbeddingsAsync
        public virtual string UpdateIndex { get; } = "Update Index";

        // IndexAllFilesFromFirstCommitToLatestCommit
        public virtual string IndexAllFilesFromFirstCommitToLatestCommit { get; } = "Index All Files From First Commit To Latest Commit";

        // IndexFilesFromLastIndexedCommitToLatestCommit
        public virtual string IndexFilesFromLastIndexedCommitToLatestCommit { get; } = "Index Files From Last Indexed Commit To Latest Commit";

        // LastIndexedCommitHash
        public virtual string LastIndexedCommitHash { get; } = "Last Indexed Commit Hash";

        // SpecifyTheRangeOfCommitsToIndex
        public virtual string SpecifyTheRangeOfCommitsToIndex { get; } = "Specify The Range Of Commits To Index";

        // StartCommitForIndexing
        public virtual string StartCommitForIndexing { get; } = "Start Commit For Indexing";

        // IndexingSummary
        public virtual string IndexingSummary { get; } = "Indexing Summary";

        // IndexingDetail
        public virtual string IndexingDetail { get; } = "Indexing Detail";

        #endregion
        #region VectorDB

        // VectorDBList
        public virtual string VectorDBList { get; } = "Vector DB List";

        // MultiVectorRetrieverFinalSearchResult
        public virtual string MultiVectorRetrieverFinalSearchResult { get; } = "Multi Vector Retriever Final Search Result";

        #endregion
        // PromptTemplateNotFound
        public virtual string PromptTemplateNotFound { get; } = "Prompt Template Not Found";

        // 新規アイテムとしてエクスポート
        public virtual string ExportAsNewItem { get; } = "Export As New Item";

    }
}
