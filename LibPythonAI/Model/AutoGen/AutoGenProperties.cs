using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Folder;
using LibPythonAI.Model.VectorDB;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenProperties {

        static readonly JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        public static readonly string CHAT_TYPE_GROUP = "group";
        public static readonly string CHAT_TYPE_NORMAL = "normal";

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
                { "chat_history_folder_id" , FolderManager.ChatRootFolder.Id },
                { "session_token", SessionToken },
            };
            return dict;
        }

        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

        private static bool Initialized { get; set; } = false;
        // デフォルトの設定を取得
        public static async Task Init() {
            if (Initialized) {
                return;
            }

            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // llm_configの初期設定  
            AutoGenLLMConfig config = new() {
                Name = "default",
                ApiType = ConfigPrams.GetOpenAIProperties().AzureOpenAI ? "azure" : "",
                ApiVersion = ConfigPrams.GetOpenAIProperties().AzureOpenAIAPIVersion,
                Model = ConfigPrams.GetOpenAIProperties().OpenAICompletionModel,
                ApiKey = ConfigPrams.GetOpenAIProperties().OpenAIKey,
            };
            if (config.ApiType == "azure") {
                config.BaseURL = ConfigPrams.GetOpenAIProperties().AzureOpenAIEndpoint;
            } else {
                config.BaseURL = ConfigPrams.GetOpenAIProperties().OpenAIBaseURL;
            }

            await config.Save();

            string work_dir = ConfigPrams.GetAutoGenWorkDir();

            // search_wikipedia_ja
            string toolName = "search_wikipedia_ja";
            string toolDescription = "This function searches Wikipedia with the specified keywords and returns related articles.";
            string toolPath = Path.Combine(ConfigPrams.GetPythonLibPath(), "ai_chat_explorer", "autogen_modules", "default_tools.py");
            AutoGenTool autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // list_files_in_directory
            toolName = "list_files_in_directory";
            toolDescription = "This function lists the files in the specified directory.";

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // extract_file
            toolName = "extract_file";
            toolDescription = "This function extracts the specified file.";

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // check_file
            toolName = "check_file";
            toolDescription = "This function checks if the specified file exists.";

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // extract_webpage
            toolName = "extract_webpage";
            toolDescription = "This function extracts text and links from the specified URL of a web page.";

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // search_duckduckgo
            toolName = "search_duckduckgo";
            toolDescription = "This function searches DuckDuckGo with the specified keywords and returns related articles.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // save_text_file
            toolName = "save_text_file";
            toolDescription = "This function saves the specified text to a file.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // arxiv_search
            toolName = "arxiv_search";
            toolDescription = "This function searches arXiv with the specified keywords and returns related articles.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // list_agents
            toolName = "list_agents";
            toolDescription = "This function retrieves a list of registered agents from the database.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // execute_agent
            toolName = "execute_agent";
            toolDescription = "This function executes the specified agent.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // list_agents
            toolName = "list_agents";
            toolDescription = "This function retrieves a list of registered agents from the database.";

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // get_current_time
            toolName = "get_current_time";
            toolDescription = "This function returns the current time.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // vector_search
            toolName = "vector_search";
            toolDescription = "This function searches for the specified vector in the database.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // global_vector_search
            toolName = "global_vector_search";
            toolDescription = "This function searches for the specified vector in the global database.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // past_chat_history_vector_search
            toolName = "past_chat_history_vector_search";
            toolDescription = "This function searches for the specified vector in the past chat history.";
            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // list_tool_agent
            toolName = "list_tool_agents";
            toolDescription = "This function retrieves a list of registered tool agents from the database.";

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // execute_tool_agent
            toolName = "execute_tool_agent";
            toolDescription = """
                This function executes the specified agent with the input text and returns the output text.
                First argument: agent name, second argument: input text.
                - Agent name: Specify the name of the agent as the Python function name.
                - Input text: The text data to be processed by the agent.
                """;

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // register_tool_agent
            toolName = "register_tool_agent";
            toolDescription = """
                This function creates a FunctionTool object with the specified function name, documentation, and Python code.
                引数で与えられたPythonコードから関数を作成し、FunctionToolオブジェクトを作成します。
                作成したFunctionToolを実行するためのエージェントを作成し、tool_agentsに追加します。
            """;

            autoGenTool = new() {
                Name = toolName,
                Description = toolDescription,
                Path = toolPath,
            };
            await autoGenTool.Save();

            // agentの初期化
            var agentName = "planner";
            var agentDescription = "ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します";
            var agentSystemMessage = """
                ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します
                - ユーザーの要求を達成するための計画を作成してタスク一覧を作成します。
                - タスクの割り当てに問題ないか？もっと効率的な計画およびタスク割り当てがないか？については対象エージェントに確認します。
                - 計画に基づき、対象のエージェントにタスクを割り当てます。
                - 計画作成が完了したら[計画作成完了]と返信してください
                その後、計画に基づきタスクを実行します。全てのタスクが完了したら、[TERMINATE]と返信してください。
                """;
            var agentCodeExecution = false;
            var agentLLMConfig = "default";
            var agentTools = new List<string> { };

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "tool_generative_planner";
            agentDescription = "ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します.必要に応じてPython関数を作成してツールとして登録します";
            agentSystemMessage = """
                ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します
                - ユーザーの要求を達成するための計画を作成してタスク一覧を作成します。
                A. タスクの割り当てに問題ないか？もっと効率的な計画およびタスク割り当てがないか？については対象エージェントに確認します。
                B. ユーザーの要求を達成するためのエージェントがいない場合はPython関数作成エージェントにPython関数を依頼します。  
                   Python関数作成エージェントにPython関数を作成する前には必ず同じような関数が存在しないことを確認してから依頼してください。
                C. その後、計画とタスク一覧を再度作成します。タスク割り当て先のエージェントの準備が整うまでA～Cを繰り返します。
                - 計画に基づき、対象のエージェントにタスクを割り当てます。
                - 計画作成が完了したら[計画作成完了]と返信してください
                  その後、計画に基づきタスクを実行します。全てのタスクが完了したら、[TERMINATE]と返信してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = new List<string> { };

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "code_writer";
            agentDescription = "コードを書くためのエージェントです。";
            agentSystemMessage = """
                あなたはスクリプト開発者です。ユーザーの指示に従ってコードを書きます。 
                コードの実行結果は、コードを投稿した後に自動的に表示されます。 
                ただし、次の条件を厳守する必要があります。 
                ルール:
                - コードはPythonで実装して、出力は、Markdown形式(例:```python ...```)であること。
                - スクリプトの実行結果がエラーの場合、エラーメッセージに基づいて対策を検討し、再度修正したコードを作成すること。
                - スクリプトの実行から得られる情報が不十分な場合、現在得られている情報に基づいて再度修正したコードを作成すること。

                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = [];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "code_executor";
            agentDescription = "コードを実行するためのエージェントです。";
            agentSystemMessage = """
                あなたはスクリプト実行者です。 ユーザーの指示に従ってコードを実行します。 コードの実行結果は、コードを投稿した後に自動的に表示されます。 
                ただし、次の条件を厳守する必要があります。 
                ルール:
                - 入力はMarkdown形式(例:```python ...```)であること。入力がMarkdown形式でない場合はMarkdown形式の入力を要求すること。
                """;
            agentCodeExecution = true;
            agentLLMConfig = "default";
            agentTools = [];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "web_searcher";
            agentDescription = "Web検索を行うエージェントです。";
            agentSystemMessage = """
                あなたはウェブサーチャーです。ユーザーの指示に従って、以下の機能を利用してウェブ上のドキュメントを検索します。
                それ以外の指示には返信しないでください。
                - 情報を検索するために提供された search_duckduckgo 関数を使用してください。
                - 必要なドキュメントがリンク先にない場合は、さらにリンクされた情報を検索してください。
                - 必要なドキュメントが見つかった場合は、extract_webpage でドキュメントを取得し、ユーザーにテキストを提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["search_duckduckgo", "extract_webpage"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "file_operator";
            agentDescription = "ファイル操作を行うエージェントです。";
            agentSystemMessage = $"""
                あなたはファイルオペレーターです。ユーザーの指示に従って、以下の機能を利用してファイル操作を行います。
                - list_files_in_directory 関数で指定されたディレクトリ内のファイルをリストします。
                - extract_file 関数を使用して指定されたファイルからテキスト抽出します。
                - check_file 関数を使用して指定されたファイルが存在するかどうかを確認します。
                - save_text_file 関数を使用してテキストをファイルに保存します。 デフォルトのディレクトリは、{work_dir}です。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["list_files_in_directory", "extract_file", "check_file", "save_text_file"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "time_checker";
            agentDescription = "時間を確認するエージェントです。";
            agentSystemMessage = """
                あなたはタイムチェッカーです。ユーザーの指示に従って、以下の機能を利用して時間を確認します。
                それ以外の指示には返信しないでください。
                - 現在の時間を取得するために提供された get_current_time 関数を使用してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["get_current_time"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            // arxiv_searcher
            agentName = "arxiv_searcher";
            agentDescription = "arXivから論文を検索するエージェントです。";
            agentSystemMessage = $"""
                あなたはarXiv検索エージェントです。
                ユーザーからキーワードを受け取り、arXivから論文を検索します。
                検索結果をユーザーに提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["arxiv_search"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "tool_agent_creator";
            agentDescription = "Python関数を作成するためのエージェント";
            agentSystemMessage = """
                * ツール(Python関数)実行エージェント一覧を取得して、ユーザーの要求にマッチするPython関数を実行します.
                * ユーザーの要求にマッチするエージェントがなかった場合は、Python関数を作成して新しいツール実行エージェントを作成します。
                  * ツールを作成する場合は以下の仕様で作成します。
                  * register_tool_agentの引数：name, doc, code
                  * name: 関数名
                  * doc: 関数の説明 関数の目的、引数、戻り値について記述してください。
                  * code: 関数のPythonコード 関数名はnameと同じにしてください。
                  引数と戻り値はAnnotatedを使用して型を指定してください。忘れずにImport文を追加してください。
                  docstringには関数の説明を記述してください。
                  codeの例：
                  import uuid
                  from typing import Annotated
                  def sample_tool(input_text: Annoteted[str, "入力文字列"]) -> Annotated[str, "出力文字列"]:
                      \"\"\"
                      This is a sample tool function.
                      \"\"\"
                      return f"Input text: {input_text}"

                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["list_tool_agents", "execute_tool_agent", "register_tool_agent"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            // ユーザーからの指示にマッチするエージェントを検索してエージェントを実行する
            agentName = "agent_selector";
            agentDescription = "エージェント一覧からユーザーの要求にマッチするエージェントを取得して実行するエージェントです。";
            agentSystemMessage = $"""
                あなたはエージェント実行エージェントです。
                list_agents
                エージェント一覧からユーザーの要求にマッチするエージェントを取得して実行します。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["execute_agent", "list_agents"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            // ベクトル検索エージェント
            agentName = "vector_searcher";
            agentDescription = "ユーザーが指定したフォルダID内のベクトルを検索するエージェントです。";
            agentSystemMessage = $"""
                あなたはユーザーが指定したフォルダID内の情報を検索するベクトル検索エージェントです。
                ユーザーからの依頼に基づきベクトルを検索します。
                検索結果をユーザーに提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["vector_search"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            // global_vector_searcher
            agentName = "global_vector_searcher";
            agentDescription = "ユーザーが過去に収集した全データを検索するベクトル検索エージェントです。";
            agentSystemMessage = $"""
                あなたはユーザーが過去に収集した全データを検索するベクトル検索エージェントです。
                ユーザーからの依頼に基づき、ユーザーが過去に収集したデータを検索します。
                検索結果をユーザーに提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["global_vector_search"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            // past_chat_history_vector_searcher
            agentName = "past_chat_history_vector_searcher";
            agentDescription = "ユーザーが過去のチャット履歴を検索するベクトル検索エージェントです。";
            agentSystemMessage = $"""
                あなたはユーザーが過去のチャット履歴を検索するベクトル検索エージェントです。
                ユーザーからの依頼に基づき、過去のチャット履歴を検索します。
                検索結果をユーザーに提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["past_chat_history_vector_search"];


            // group_chatの初期化

            // エージェント一覧から適切なエージェントを取得して実行するグループチャット
            var groupName = "default";
            var groupDescription = "エージェント一覧から適切なエージェントを取得して実行するグループチャットです。";
            var groupLLMConfig = "default";
            var groupAgentNames = new List<string> { "planner", "code_writer", "code_executor", "web_searcher", "file_operator", "time_checker", "tool_agent_selector" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);

            // 全エージェント名のリスト
            groupName = "agent_selector_chat";
            groupDescription = "計画に基づき適切なエージェントを選択しながらタスクを実行するグループチャットです。";
            groupAgentNames = new List<string> { "planner", "agent_selector" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);

            groupName = "code_execution";
            groupDescription = "コードを実行するためのグループチャットです。";
            groupLLMConfig = "default";
            groupAgentNames = new List<string> { "planner", "code_writer", "code_executor" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);

            groupName = "agent_generative_chat";
            groupDescription = "ツールエージェントの実行と登録するためのグループチャットです。";
            groupLLMConfig = "default";
            groupAgentNames = new List<string> { "tool_generative_planner", "tool_agent_creator", "agent_selector" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);



            Initialized = true;

        }


    }
}
