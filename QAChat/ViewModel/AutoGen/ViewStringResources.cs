using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QAChat.Resource;

namespace QAChat.ViewModel.AutoGen {
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
        public static string Agent { get; set; } = "エージェント";
        public static string Tool { get; set; } = "ツール";
        public static string SourcePath { get; set; } = "ソースパス";
        public static string Save { get; set; } = CommonStringResources.Save;

        // Close
        public static string Close { get; set; } = CommonStringResources.Close;


    }
}
