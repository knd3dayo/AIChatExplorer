using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using Python.Runtime;
using PythonAILib.Common;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;
namespace PythonAILib.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        // Python script for OpenAI
        public static string OpenAIScript {
            get {
                string path = Path.Combine(PythonAILibPath, "ai_app_wrapper.py");
                return path;
            }
        }

        // Python script for Misc
        public static string MiscScript {
            get {
                string devPath = Path.Combine(PythonAILibPath, "dev");
                string path = Path.Combine(devPath, "misc_app.py");
                return path;
            }
        }

        private static IPythonAILibConfigParams? ConfigPrams;

        public static string? PythonPath { get; set; }

        private static string PathToVirtualEnv { get; set; } = "";

        private static string PythonAILibPath { get; set; } = DefaultPythonAILibDir;

        private const string DefaultPythonAILibDir = "python_ai_lib";

        private static IPythonAIFunctions? _pythonAIFunctions;
        public static IPythonAIFunctions PythonAIFunctions {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                if (string.IsNullOrEmpty(ConfigPrams?.GetAppDataPath())) {
                    throw new Exception(StringResources.PythonDLLNotFound);
                }
                _pythonAIFunctions ??= new PythonNetFunctions();
                return _pythonAIFunctions;
            }
        }

        private static IPythonMiscFunctions? _pythonMiscFunctions;
        public static IPythonMiscFunctions PythonMiscFunctions {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                if (string.IsNullOrEmpty(PythonPath)) {
                    throw new Exception(StringResources.PythonDLLNotFound);

                }
                if (_pythonMiscFunctions == null) {
                    _pythonMiscFunctions = new PythonMiscFunctions();

                }
                return _pythonMiscFunctions;
            }
        }
        // Initialize Python functions
        public static void Init(IPythonAILibConfigParams configPrams) {
            ConfigPrams = configPrams;

            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            string appDataDir = configPrams.GetAppDataPath();

            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                PathToVirtualEnv = pathToVirtualEnv;
            }
            if (!string.IsNullOrEmpty(appDataDir)) {

                // ★TODO Pythonスクリプトをアプリケーション用ディレクトリにコピーする処理
                // バージョンアップ時には、アプリケーション用ディレクトリにコピーする処理が必要となるが、
                // 未実装のため、一旦コメントアウトしておく
                PythonAILibPath = Path.Combine(appDataDir, DefaultPythonAILibDir);

                // Check if the PythonAILibPath exists
                if (!Directory.Exists(PythonAILibPath)) {
                    // ./pythonディレクトリをPythonAILibPathRootへコピーする
                    Tools.CopyDirectory(DefaultPythonAILibDir, PythonAILibPath, true, true);
                }

                InitPythonNet(configPrams);
            }
        }

        private static void InitAutogenScripts() {
            if (ConfigPrams == null) {
                throw new Exception("ConfigPrams is not initialized.");
            }

            // search_wikipedia_ja
            string toolName = "search_wikipedia_ja";
            string toolDescription = "This function searches Wikipedia with the specified keywords and returns related articles.";
            string toolPath = Path.Combine(ConfigPrams.GetPythonLibPath(), "ai_app_autogen", "default_tools.py");
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // list_files_in_directory
            toolName = "list_files_in_directory";
            toolDescription = "This function lists the files in the specified directory.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // extract_file
            toolName = "extract_file";
            toolDescription = "This function extracts the specified file.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // check_file
            toolName = "check_file";
            toolDescription = "This function checks if the specified file exists.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // extract_webpage
            toolName = "extract_webpage";
            toolDescription = "This function extracts text and links from the specified URL of a web page.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // search_duckduckgo
            toolName = "search_duckduckgo";
            toolDescription = "This function searches DuckDuckGo with the specified keywords and returns related articles.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // save_text_file
            toolName = "save_text_file";
            toolDescription = "This function saves the specified text to a file.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // save_tools
            toolName = "save_tools";
            toolDescription = "This function saves the specified tools to a file.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);

            // get_current_time
            toolName = "get_current_time";
            toolDescription = "This function returns the current time.";
            UpdateAutoGenTool(toolPath, toolName, toolDescription, false);


        }
        public static void UpdateAutoGenTool(string toolPath, string toolName, string toolDescription, bool overwrite) {
            if (ConfigPrams == null) {
                throw new Exception("ConfigPrams is not initialized.");
            }
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            SQLiteConnection.CreateFile(autogenDBURL);
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // DBにテーブルを作成
            // テーブルが存在しない場合のみ作成
            // toolsテーブル： ツールの情報を格納
            // name: ツール名
            // path: ツールのパス
            // description: ツールの説明

            using var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS tools (name TEXT, path TEXT, description TEXT)", sqlConn);
            cmd.ExecuteNonQuery();
            // ツールの情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM tools WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", "search_wikipedia_ja");
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {

                using var insertCmd = new SQLiteCommand("INSERT INTO tools (name, path, description) VALUES (@name, @path, @description)", sqlConn);
                insertCmd.Parameters.AddWithValue("@name", toolName);
                insertCmd.Parameters.AddWithValue("@path", toolPath);
                insertCmd.Parameters.AddWithValue("@description", toolDescription);
                insertCmd.ExecuteNonQuery();
            } else if (overwrite) {
                using var insertCmd = new SQLiteCommand("UPDATE tools SET path = @path, description = @description WHERE name = @name", sqlConn);
                insertCmd.Parameters.AddWithValue("@name", toolName);
                insertCmd.Parameters.AddWithValue("@path", toolPath);
                insertCmd.Parameters.AddWithValue("@description", toolDescription);
                insertCmd.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();

        }

        public static void UpdateAutoGenChat(string scriptPath, string functionName, string description, bool overwrite) {
            if (ConfigPrams == null) {
                throw new Exception("ConfigPrams is not initialized.");
            }
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            SQLiteConnection.CreateFile(autogenDBURL);
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // DBにテーブルを作成
            // テーブルが存在しない場合のみ作成
            // chatsテーブル： ツールの情報を格納
            // name: ツール名
            // path: ツールのパス
            // description: ツールの説明
            using var insertCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS chats (name TEXT, path TEXT, description TEXT)", sqlConn);
            insertCmd.ExecuteNonQuery();
            // チャット定義の情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM chats WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", functionName);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {
                using var insertCmd2 = new SQLiteCommand("INSERT INTO chats (name, path, description) VALUES (@name, @path, @description)", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", functionName);
                insertCmd2.Parameters.AddWithValue("@path", scriptPath);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.ExecuteNonQuery();
            } else if (overwrite) {
                using var insertCmd2 = new SQLiteCommand("UPDATE chats SET path = @path, description = @description WHERE name = @name", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", functionName);
                insertCmd2.Parameters.AddWithValue("@path", scriptPath);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();

        }

        private static void InitPythonNet(IPythonAILibConfigParams configPrams) {
            // Pythonスクリプトを実行するための準備

            // 既に初期化されている場合は初期化しない
            if (PythonEngine.IsInitialized) {
                return;
            }

            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            string appDataDir = configPrams.GetAppDataPath();
            string pythonDLLPath = configPrams.GetPythonDllPath();
            string pythonAILibPath = PythonAILibPath;
            string httpsProxy = configPrams.GetHttpsProxy();
            string noProxy = configPrams.GetNoProxy();

            // PythonDLLのパスを設定
            Runtime.PythonDLL = pythonDLLPath;

            // Runtime.PythonDLLのファイルが存在するかチェック
            if (!File.Exists(Runtime.PythonDLL)) {
                string message = StringResources.PythonDLLNotFound;
                throw new Exception(message + Runtime.PythonDLL);

            }
            // Venv環境が存在するかチェック
            if (!string.IsNullOrEmpty(pathToVirtualEnv) && !Directory.Exists(pathToVirtualEnv)) {
                string message = StringResources.PythonVenvNotFound;
                throw new Exception(message + pathToVirtualEnv);
            }

            try {
                // venvを使用する場合の設定
                // 公式ドキュメントの設定ではPythonEngine.Initialize()時にクラッシュするため、
                // 以下を参考にして設定を行う
                // https://github.com/pythonnet/pythonnet/issues/1478#issuecomment-897933730

                // PythonEngineにアクセスするためのダミー処理
                string version = PythonEngine.Version;

                if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                    LogWrapper.Info($"Python Version: {version}");
                    // 実行中の Python のユーザー site-packages へのパスを無効にする
                    PythonEngine.SetNoSiteFlag();
                }

                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();


                // sys.prefix、sys.exec_prefixを venvのパスに変更

                using (Py.GIL()) {
                    // fix the prefixes to point to our venv
                    // (This is for Windows, there may be some difference with sys.exec_prefix on other platforms)
                    dynamic sys = Py.Import("sys");
                    dynamic site = Py.Import("site");
                    dynamic os = Py.Import("os");
                    if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                        sys.prefix = pathToVirtualEnv;
                        sys.exec_prefix = pathToVirtualEnv;

                        // This has to be overwritten because site module may already have 
                        // been loaded by the interpreter (but not run yet)
                        site.PREFIXES = new List<PyObject> { sys.prefix, sys.exec_prefix };
                    }
                    // set the path to pythonAILib
                    site.addsitedir(pythonAILibPath);

                    // set the proxy settings
                    if (!string.IsNullOrEmpty(httpsProxy)) {
                        os.environ["HTTPS_PROXY"] = new PyString(httpsProxy);
                        os.environ["NO_PROXY"] = new PyString(noProxy);
                    } else {
                        // NO_PROXY="*"
                        os.environ["NO_PROXY"] = new PyString("*");
                    }

                    // Run site path modification with tweaked prefixes
                    site.main();
                }

            } catch (TypeInitializationException e) {
                string message = StringResources.PythonInitFailed + e.Message;
                LogWrapper.Error(message);
            }
        }

        // Load Python script
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            // Load the file
            string script = File.ReadAllText(file.FullName);

            return script;
        }
    }

}
