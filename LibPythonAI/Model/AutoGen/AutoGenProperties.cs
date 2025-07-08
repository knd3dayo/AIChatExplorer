using System.IO;
using LibPythonAI.Common;
using LibPythonAI.Model.Folders.Base;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.Model.AutoGen {
    public class AutoGenProperties {

        public const string CHAT_TYPE_GROUP = "group";
        public const string CHAT_TYPE_NORMAL = "normal";

        // autogen_db_path
        public string AutoGenDBPath { get; set; } = PythonAILibManager.Instance.ConfigParams.GetMainDBPath();
        // venv_path
        public string VenvPath { get; set; } = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();
        // work_dir
        public string WorkDir { get; set; } = PythonAILibManager.Instance.ConfigParams.GetAutoGenWorkDir();

        // tool_dir
        public string ToolDir { get; set; } = PythonAILibManager.Instance.ConfigParams.GetAutoGenToolDir();

        // chat_type
        public string ChatType { get; set; } = CHAT_TYPE_GROUP;

        // chat_name
        public string ChatName { get; set; } = "default";


        // terminate_msg
        public string TerminateMsg { get; set; } = "TERMINATE";

        // max_msg
        public int MaxMsg { get; set; } = 15;

        // timeout
        public int Timeout { get; set; } = 120;

        // SessionToken
        public string SessionToken { get; set; } = Guid.NewGuid().ToString();


        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "autogen_db_path", AutoGenDBPath },
                { "work_dir", WorkDir },
                { "tool_dir", ToolDir },
                { "venv_path", VenvPath },
                { "chat_type", ChatType },
                { "chat_name", ChatName },
                { "terminate_msg", TerminateMsg },
                { "max_msg", MaxMsg },
                { "timeout", Timeout },
                { "main_vector_db_id" , VectorDBItem.GetDefaultVectorDB().Id },
                { "chat_history_folder_id" , FolderManagerBase.ChatRootFolder.Id },
                { "session_token", SessionToken },
            };
            return dict;
        }
    }
}
