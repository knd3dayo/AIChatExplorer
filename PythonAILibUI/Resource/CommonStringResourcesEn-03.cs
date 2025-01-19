using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAChat.Resource {
    public partial class CommonStringResourcesEn : CommonStringResources  {

        #region QAChatControl
        // Export all chat contents
        public override string ExportAllChatContents { get; } = "Export All Chat Contents";
        // Copy selected chat contents
        public override string CopySelectedChatContents { get; } = "Copy Selected Chat Contents";
        // Copy all chat contents
        public override string CopyAllChatContents { get; } = "Copy All Chat Contents";

        #endregion

        #region AutoGen
        // AutoGenSettingList
        public override string AutoGenSettingList { get; } = "AutoGenSettingList";
        // EditAutoGenAgentWindowTitle
        public override string EditAutoGenAgentWindowTitle { get; } = "EditAutoGenAgentWindowTitle";
        // ToolsForLLM
        public override string ToolsForLLM { get; } = "ToolsForLLM";
        // ToolsForExecution
        public override string ToolsForExecution { get; } = "ToolsForExecution";
        // SystemMessage
        public override string SystemMessage { get; } = "SystemMessage";
        // GroupChat
        public override string GroupChat { get; } = "Group Chat";
        // NormalChat
        public override string NormalChat { get; } = "Normal Chat";
        // NestedChat
        public override string NestedChat { get; } = "Nested Chat";
        // Agent
        public override string Agent { get; } = "Agent";
        // SourcePath
        public override string SourcePath { get; } = "Source Path";
        // InitialAgent
        public override string InitialAgent { get; } = "Initial Agent";
        // AgentType
        public override string AgentType { get; } = "Agent Type";
        // UserProxy
        public override string UserProxy { get; } = "User Proxy";
        // Assistant
        public override string Assistant { get; } = "Assistant";
        // HumanInputMode
        public override string HumanInputMode { get; } = "Human Input Mode";
        // NEVER
        public override string Never { get; } = "NEVER";
        // ALWAYS
        public override string Always { get; } = "ALWAYS";
        // TERMINATE
        public override string Terminate { get; } = "TERMINIATE";
        // TERMINATEMsg
        public override string TerminateMsg { get; } = "TerminateMsg";
        // CodeExecution
        public override string CodeExecution { get; } = "Code Execution";
        // LLM
        public override string LLM { get; } = "LLM";
        // AddVectorDB
        public override string AddVectorDB { get; } = "Add Vector DB";
        #endregion

        // Enabled
        public override string Enabled { get; } = "Enabled";

        #region folder
        // ExportTarget
        public override string ExportTarget { get; } = "Export Target";

        // ImportTarget
        public override string ImportTarget { get; } = "Import Target";

        // SaveVectorDB
        public override string SaveVectorDB { get; } = "Save Vector DB";

        // ReferenceVectorDB
        public override string ReferenceVectorDB { get; } = "Reference Vector DB";

        #endregion

        #region RAG
        // CommitDateTime
        public override string CommitDateTime { get; } = "Commit Date Time";

        // CommitHash
        public override string CommitHash { get; } = "Commit Hash";

        // Message
        public override string Message { get; } = "Message";

        // UpdateRAGIndexWindow
        public override string UpdateRAGIndexWindow { get; } = "Update RAG Index Window";

        // GitRepositoryURL
        public override string GitRepositoryURL { get; } = "Git Repository URL";

        // UpdateIndex
        public override string UpdateIndex { get; } = "Update Index";

        // IndexAllFilesFromFirstCommitToLatestCommit
        public override string IndexAllFilesFromFirstCommitToLatestCommit { get; } = "Index All Files From First Commit To Latest Commit";

        // IndexFilesFromLastIndexedCommitToLatestCommit
        public override string IndexFilesFromLastIndexedCommitToLatestCommit { get; } = "Index Files From Last Indexed Commit To Latest Commit";

        // LastIndexedCommitHash
        public override string LastIndexedCommitHash { get; } = "Last Indexed Commit Hash";

        // SpecifyTheRangeOfCommitsToIndex
        public override string SpecifyTheRangeOfCommitsToIndex { get; } = "Specify The Range Of Commits To Index";

        // StartCommitForIndexing
        public override string StartCommitForIndexing { get; } = "Start Commit For Indexing";

        // IndexingSummary
        public override string IndexingSummary { get; } = "Indexing Summary";

        // IndexingDetail
        public override string IndexingDetail { get; } = "Indexing Detail";

        #endregion
        #region VectorDB

        // VectorDBList
        public override string VectorDBList { get; } = "Vector DB List";

        // MultiVectorRetrieverFinalSearchResult
        public override string MultiVectorRetrieverFinalSearchResult { get; } = "Multi Vector Retriever Final Search Result";

        #endregion
        // PromptTemplateNotFound
        public override string PromptTemplateNotFound { get; } = "Prompt Template Not Found";
    }
}
