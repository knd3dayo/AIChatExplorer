using QAChat.Resource;

namespace QAChat.View.AutoGen {
    public class ViewStringResources {

        public static CommonStringResources CommonStringResources { get; set; } = CommonStringResources.Instance;

        // EditAutoGenAgentWindowTitle
        public static string EditAutoGenAgentWindowTitle { get; set; } = "EditAutoGenAgentWindowTitle";

        // Name
        public static string Name { get; set; } = CommonStringResources.Name;
        // Description
        public static string Description { get; set; } = CommonStringResources.Description;

        // ToolsForLLM
        public static string ToolsForLLM { get; set; } = "ToolsForLLM";
        // ToolsForExecution
        public static string ToolsForExecution { get; set; } = "ToolsForExecution";

        // Add
        public static string Add { get; set; } = CommonStringResources.Add;

        // OK
        public static string OK { get; set; } = CommonStringResources.OK;

        // Cancel
        public static string Cancel { get; set; } = CommonStringResources.Cancel;

        // Enabled
        public static string Enabled { get; set; } = "有効";

        // Delete
        public static string Delete { get; set; } = CommonStringResources.Delete;

        // BasicSettings
        public static string BasicSettings { get; set; } = CommonStringResources.BasicSettings;

        // DetailSettings
        public static string DetailSettings { get; set; } = CommonStringResources.DetailSettings;

        // SystemMessage
        public static string SystemMessage { get; set; } = "SystemMessage";
        public static string GroupChat { get; set; } = "グループチャット";

        // NormalChat
        public static string NormalChat { get; set; } = "通常チャット";

        // NestedChat
        public static string NestedChat { get; set; } = "ネストチャット";
        public static string Agent { get; set; } = "エージェント";
        public static string Tool { get; set; } = "ツール";
        public static string SourcePath { get; set; } = "ソースパス";
        public static string Save { get; set; } = CommonStringResources.Save;

        // Close
        public static string Close { get; set; } = CommonStringResources.Close;

        // Select
        public static string Select { get; set; } = CommonStringResources.Select;
        // InitAgent
        public static string InitAgent { get; set; } = "初期エージェント";
        // AgentType
        public static string AgentType { get; set; } = "エージェントタイプ";

        // UserProxy
        public static string UserProxy { get; set; } = "userproxy";

        // Assistant
        public static string Assistant { get; set; } = "assistant";

        // HumanInputMode
        public static string HumanInputMode { get; set; } = "HumanInputMode";
        // Never
        public static string Never { get; set; } = "NEVER";
        // Always
        public static string Always { get; set; } = "ALWAYS";

        // Terminate
        public static string Terminate { get; set; } = "TERMINATE";

        // TerminateMsg
        public static string TerminateMsg { get; set; } = "TerminateMsg";

        // CodeExecution
        public static string CodeExecution { get; set; } = "CodeExecution";

        // LLM
        public static string LLM { get; set; } = "LLM";

        // VectorDB
        public static string VectorDB { get; set; } =  CommonStringResources.VectorDB;

        // AddVectorDB
        public static string AddVectorDB { get; set; } = "AddVectorDB";


    }
}
