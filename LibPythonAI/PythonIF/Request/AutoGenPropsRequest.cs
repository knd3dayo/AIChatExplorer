using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Folder;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    public class AutoGenPropsRequest {


        public AutoGenPropsRequest(AutoGenProperties autoGenProperties) {
            AutoGenDBPath = autoGenProperties.AutoGenDBPath;
            VenvPath = autoGenProperties.VenvPath;
            WorkDir = autoGenProperties.WorkDir;
            ToolDir = autoGenProperties.ToolDir;
            ChatType = autoGenProperties.ChatType;
            ChatName = autoGenProperties.ChatName;
            TerminateMsg = autoGenProperties.TerminateMsg;
            MaxMsg = autoGenProperties.MaxMsg;
            Timeout = autoGenProperties.Timeout;
            SessionToken = autoGenProperties.SessionToken;
        }

        // autogen_db_path
        public string AutoGenDBPath { get; set; }
        // venv_path
        public string VenvPath { get; set; }
        // work_dir
        public string WorkDir { get; set; }

        // tool_dir
        public string ToolDir { get; set; }

        // chat_type
        public string ChatType { get; set; }

        // chat_name
        public string ChatName { get; set; }


        // terminate_msg
        public string TerminateMsg { get; set; }

        // max_msg
        public int MaxMsg { get; set; }

        // timeout
        public int Timeout { get; set; }

        // SessionToken
        public string SessionToken { get; set; }


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
                { "chat_history_folder_id" , FolderManager.ChatRootFolder.Id },
                { "session_token", SessionToken },
            };
            return dict;
        }

    }
}
