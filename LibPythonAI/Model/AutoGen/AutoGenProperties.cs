using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Model.AutoGen;
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
        public string WorkDir { get; set; } = "";

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

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "autogen_db_path", AutoGenDBPath },
                { "work_dir", WorkDir },
                { "venv_path", VenvPath },
                { "chat_type", ChatType },
                { "chat_name", ChatName },
                { "terminate_msg", TerminateMsg },
                { "max_msg", MaxMsg },
                { "timeout", Timeout },
                { "main_vector_db_id" , VectorDBItem.GetDefaultVectorDB().Id }
            };
            return dict;
        }

        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

        private static bool Initialized { get; set; } = false;
        // デフォルトの設定を取得
        public static void Init() {
            if (Initialized) {
                return;
            }

            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // llm_configの初期設定  
            string name = "default";
            string api_type = ConfigPrams.GetOpenAIProperties().AzureOpenAI ? "azure" : "";
            string api_version = ConfigPrams.GetOpenAIProperties().AzureOpenAIAPIVersion;
            string model = ConfigPrams.GetOpenAIProperties().OpenAICompletionModel;
            string api_key = ConfigPrams.GetOpenAIProperties().OpenAIKey;
            string base_url = ConfigPrams.GetOpenAIProperties().OpenAIBaseURL;
            if (api_type == "azure") {
                base_url = ConfigPrams.GetOpenAIProperties().AzureOpenAIEndpoint;
            }
            string work_dir = ConfigPrams.GetAutoGenWorkDir();
            AutoGenLLMConfig.UpdateAutoGenLLMConfig(name, api_type, api_version, model, api_key, base_url);

            // search_wikipedia_ja
            string toolName = "search_wikipedia_ja";
            string toolDescription = "This function searches Wikipedia with the specified keywords and returns related articles.";
            string toolPath = Path.Combine(ConfigPrams.GetPythonLibPath(), "ai_app_autogen", "default_tools.py");
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // list_files_in_directory
            toolName = "list_files_in_directory";
            toolDescription = "This function lists the files in the specified directory.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // extract_file
            toolName = "extract_file";
            toolDescription = "This function extracts the specified file.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // check_file
            toolName = "check_file";
            toolDescription = "This function checks if the specified file exists.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // extract_webpage
            toolName = "extract_webpage";
            toolDescription = "This function extracts text and links from the specified URL of a web page.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // search_duckduckgo
            toolName = "search_duckduckgo";
            toolDescription = "This function searches DuckDuckGo with the specified keywords and returns related articles.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // save_text_file
            toolName = "save_text_file";
            toolDescription = "This function saves the specified text to a file.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // arxiv_search
            toolName = "arxiv_search";
            toolDescription = "This function searches arXiv with the specified keywords and returns related articles.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // list_agents
            toolName = "list_agents";
            toolDescription = "This function retrieves a list of registered agents from the database.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // execute_agent
            toolName = "execute_agent";
            toolDescription = "This function executes the specified agent.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // register_agent
            toolName = "register_tool_agent";
            toolDescription = """
            This function saves the agent that use the specified tool to a sqlite3 database.
            First argument: tool name.
            - Tool name: Specify the name of the tool as the Python function name.
            If the tool not registered, return False.
            If the tool registration is successful, register the agent.
            The agent name is [tool_name]_agent, and the agent description is the same as the tool description.
            If the agent with the same name already exists, return True.
            If the agent registration is successful, return True.
            If the agent registration failed, return False.
            """;
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // list_agents
            toolName = "list_agents";
            toolDescription = "This function retrieves a list of registered agents from the database.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // get_current_time
            toolName = "get_current_time";
            toolDescription = "This function returns the current time.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // vector_search
            toolName = "vector_search";
            toolDescription = "This function searches for the specified vector in the database.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // global_vector_search
            toolName = "global_vector_search";
            toolDescription = "This function searches for the specified vector in the global database.";
            AutoGenTool.UpdateAutoGenTool(toolPath, toolName, toolDescription, true);

            // agentの初期化
            var agentName = "planner";
            var agentDescription = "ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します";
            var agentSystemMessage = """
                ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します
                - ユーザーの要求を達成するための計画を作成してタスク一覧を作成します。
                - タスクの割り当てに問題ないか？もっと効率的な計画およびタスク割り当てがないか？については対象エージェントに確認します。
                - 情報収集が必要な場合は、ユーザーが専門的な情報やドメイン固有の情報を優先するためベクトル検索、ファイル検索を優先してください。
                  一般的な情報を得るためのWeb検索はなるべく最後に行ってください。
                - 計画に基づき、対象のエージェントにタスクを割り当てます。
                - 計画作成が完了したら[計画作成完了]と返信してください
                  その後、計画に基づきタスクを実行します。全てのタスクが完了したら、[TERMINATE]と返信してください。
                """;
            var agentCodeExecution = false;
            var agentLLMConfig = "default";
            var agentTools = new List<string> { };

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            agentName = "auto_generative_planner";
            agentDescription = "ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します.必要に応じてPython関数を作成してツールとして登録します";
            agentSystemMessage = """
                ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します
                - ユーザーの要求を達成するための計画を作成してタスク一覧を作成します。
                - タスクの割り当てに問題ないか？もっと効率的な計画およびタスク割り当てがないか？については対象エージェントに確認します。
                - 情報収集が必要な場合は、ユーザーが専門的な情報やドメイン固有の情報を優先するためベクトル検索、ファイル検索を優先してください。
                  一般的な情報を得るためのWeb検索はなるべく最後に行ってください。
                - 適切なエージェントが存在せず、coder_writer,code_executerエージェントがいる場合は、彼らにタスク遂行が可能か確認します。 
                  coder_writerがタスクを処理するための適切なPythonコードを生成した場合は、tool_resister_agentにツール登録を依頼します。
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

            // tool_agent_register
            var toolsDir = Path.Combine(ConfigPrams.GetPythonLibPath(), "generated_tools");
            if (!Directory.Exists(toolsDir)) {
                Directory.CreateDirectory(toolsDir);
            }

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

            // global_vector_searcher
            agentName = "global_vector_searcher";
            agentDescription = "ユーザーが収集したデータを検索するベクトル検索エージェントです。";
            agentSystemMessage = $"""
                あなたはグローバルベクトル検索エージェントです。
                ユーザーからの依頼に基づき、ユーザーが過去に収集したデータを検索します。
                検索結果をユーザーに提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["global_vector_search"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);


            agentName = "tool_agent_register";
            agentDescription = "ツール実行エージェントを登録するエージェントです。";
            agentSystemMessage = $"""
                あなたはツール実行エージェントの登録者です。code_writerエージェントが作成したpythonコードをPython関数(ツール)に加工します。
                Python関数(ツール)をregister_tool_agentツールを用いてDBに保存します。
                - Python関数(ツール)は. 引数と戻り値はAnnotatedを使用して型を指定する必要があります。
                - Python関数(ツール)はexecにより動的に呼び出されるため、import文は原則、関数内に記述してください。
                - Python関数(ツール)は汎用的なものであるべきです。code_writerエージェントが作成したpythonコードが具体的なものである場合は汎用化可能か検討してください。
                - 例： xxxという文字列をファイルに書き込む -> 引数で指定された文字列をファイルに書き込む
                - register_tool_agentに渡すfunction_nameは作成したPython関数(ツール)と同じである必要があります。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["register_tool_agent"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);

            // ユーザーからの指示にマッチするエージェントを検索してエージェントを実行する
            agentName = "execute_agent";
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
            agentDescription = "ベクトルを検索するエージェントです。";
            agentSystemMessage = $"""
                あなたはベクトル検索エージェントです。
                ユーザーからの依頼に基づきベクトルを検索します。
                検索結果をユーザーに提供してください。
                """;
            agentCodeExecution = false;
            agentLLMConfig = "default";
            agentTools = ["vector_search"];

            AutoGenAgent.UpdateAutoGenAgent(agentName, agentDescription, agentSystemMessage, agentCodeExecution, agentLLMConfig, agentTools, [], true);


            // group_chatの初期化
            var groupName = "default";
            var groupDescription = "デフォルトのグループチャットです。";
            var groupLLMConfig = "default";
            // 全エージェント名のリスト
            var groupAgentNames = new List<string> { "planner", "code_writer", "code_executor", "web_searcher", "file_operator", "time_checker", "tool_agent_register" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);

            groupName = "code_execution";
            groupDescription = "コードを実行するためのグループチャットです。";
            groupLLMConfig = "default";
            groupAgentNames = new List<string> { "planner", "code_writer", "code_executor" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);

            groupName = "agent_registration";
            groupDescription = "ツールとエージェントを登録するためのグループチャットです。";
            groupLLMConfig = "default";
            groupAgentNames = new List<string> { "planner", "code_writer", "code_executor", "tool_agent_register" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);

            // エージェント一覧から適切なエージェントを取得して実行するグループチャット
            groupName = "execute_agent";
            groupDescription = "エージェント一覧から適切なエージェントを取得して実行するグループチャットです。";
            groupLLMConfig = "default";
            groupAgentNames = new List<string> { "auto_generative_planner", "execute_agent" };
            AutoGenGroupChat.UpdateAutoGenGroupChat(groupName, groupDescription, groupLLMConfig, groupAgentNames, true);


            Initialized = true;

        }


    }
}
