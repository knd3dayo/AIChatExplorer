
using System.IO;
using PythonAILib.Model.Chat;

namespace ImageChat.Model
{
    public class ImageChatConfig {

        private static ImageChatConfig? _instance;
        public static ImageChatConfig Instance {
            get {
                if (_instance == null) {
                    _instance = new ImageChatConfig();
                }
                return _instance;
            }
        }

        // このアプリケーションのデータ用のフォルダを取得
        public string AppDataFolder {
            get {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appDataFolder = Path.Combine(appDataPath, "ImageChat");
                if (!Directory.Exists(appDataFolder)) {
                    Directory.CreateDirectory(appDataFolder);
                }
                return appDataFolder;
            }
        }


        // 言語
        private string? _lang;
        public string Lang {
            get {
                if (_lang == null) {
                    _lang = Properties.Settings.Default.Lang;
                }
                return _lang;
            }
            set {
                _lang = value;
                Properties.Settings.Default.Lang = value;
            }
        }
        public string ActualLang {
            get {
                if (string.IsNullOrEmpty(Lang)) {
                    return System.Globalization.CultureInfo.CurrentUICulture.Name;
                }
                return Lang;
            }
        }

        // PythonDllPath
        private string? _pythonDllPath;
        public string PythonDllPath {
            get {
                if (_pythonDllPath == null) {
                    _pythonDllPath = Properties.Settings.Default.PythonDllPath;
                }
                return _pythonDllPath;
            }
            set {
                _pythonDllPath = value;
                Properties.Settings.Default.PythonDllPath = value;
            }
        }
        // PythonVenvPath
        private string? _pythonVenvPath;
        public string PythonVenvPath {
            get {
                if (_pythonVenvPath == null) {
                    _pythonVenvPath = Properties.Settings.Default.PythonVenvPath;
                }
                return _pythonVenvPath;
            }
            set {
                _pythonVenvPath = value;
                Properties.Settings.Default.PythonVenvPath = value;
            }
        }

        // AzureOpenAI
        private bool? _azureOpenAI;
        public bool AzureOpenAI {
            get {
                if (_azureOpenAI == null) {
                    _azureOpenAI = Properties.Settings.Default.AzureOpenAI;
                }
                return _azureOpenAI.Value;
            }
            set {
                _azureOpenAI = value;
                Properties.Settings.Default.AzureOpenAI = value;
            }
        }
        // AzureOpenAIEndpoint
        private string? _azureOpenAIEndpoint;
        public string AzureOpenAIEndpoint {
            get {
                if (_azureOpenAIEndpoint == null) {
                    _azureOpenAIEndpoint = Properties.Settings.Default.AzureOpenAIEndpoint;
                }
                return _azureOpenAIEndpoint;
            }
            set {
                _azureOpenAIEndpoint = value;
                Properties.Settings.Default.AzureOpenAIEndpoint = value;
            }
        }

        // OpenAIKey
        private string? _openAIKey;
        public string OpenAIKey {
            get {
                if (_openAIKey == null) {
                    _openAIKey = Properties.Settings.Default.OpenAIKey;
                }
                return _openAIKey;
            }
            set {
                _openAIKey = value;
                Properties.Settings.Default.OpenAIKey = value;
            }
        }
        // OpenAICompletionModel
        private string? _openAICompletionModel;
        public string OpenAICompletionModel {
            get {
                if (_openAICompletionModel == null) {
                    _openAICompletionModel = Properties.Settings.Default.OpenAICompletionModel;
                }
                return _openAICompletionModel;
            }
            set {
                _openAICompletionModel = value;
                Properties.Settings.Default.OpenAICompletionModel = value;
            }
        }
        // OpenAIEmbeddingModel
        private string? _openAIEmbeddingModel;
        public string OpenAIEmbeddingModel {
            get {
                if (_openAIEmbeddingModel == null) {
                    _openAIEmbeddingModel = Properties.Settings.Default.OpenAIEmbeddingModel;
                }
                return _openAIEmbeddingModel;
            }
            set {
                _openAIEmbeddingModel = value;
                Properties.Settings.Default.OpenAIEmbeddingModel = value;
            }
        }
        // OpenAICompletionBaseURL
        private string? _openAICompletionBaseURL;
        public string OpenAICompletionBaseURL {
            get {
                if (_openAICompletionBaseURL == null) {
                    _openAICompletionBaseURL = Properties.Settings.Default.OpenAICompletionBaseURL;
                }
                return _openAICompletionBaseURL;
            }
            set {
                _openAICompletionBaseURL = value;
                Properties.Settings.Default.OpenAICompletionBaseURL = value;
            }
        }
        // OpenAIEmbeddingBaseURL
        private string? _openAIEmbeddingBaseURL;
        public string OpenAIEmbeddingBaseURL {
            get {
                if (_openAIEmbeddingBaseURL == null) {
                    _openAIEmbeddingBaseURL = Properties.Settings.Default.OpenAIEmbeddingBaseURL;
                }
                return _openAIEmbeddingBaseURL;
            }
            set {
                _openAIEmbeddingBaseURL = value;
                Properties.Settings.Default.OpenAIEmbeddingBaseURL = value;
            }
        }

        // TextWrapping
        private bool _textWrapping = false;
        public System.Windows.TextWrapping TextWrapping {
            get {
                _textWrapping = Properties.Settings.Default.TextWrapping;
                return _textWrapping ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap;
            }
            set {
                _textWrapping = value == System.Windows.TextWrapping.Wrap;
                Properties.Settings.Default.TextWrapping = _textWrapping;
            }
        }


        public void Save() {
            Properties.Settings.Default.Save();

        }
        public void Reload() {
            Properties.Settings.Default.Reload();
        }
        public OpenAIProperties CreateOpenAIProperties() {
            OpenAIProperties openAIProperties = new() {
                OpenAIKey = OpenAIKey,
                OpenAICompletionModel = OpenAICompletionModel,
                OpenAIEmbeddingModel = OpenAIEmbeddingModel,
                AzureOpenAI = AzureOpenAI,
            };

            if (string.IsNullOrEmpty(Properties.Settings.Default.AzureOpenAIEndpoint) == false) {
                openAIProperties.AzureOpenAIEndpoint = Properties.Settings.Default.AzureOpenAIEndpoint;
            }

            if (Properties.Settings.Default.OpenAICompletionBaseURL != "") {
                openAIProperties.OpenAICompletionBaseURL = Properties.Settings.Default.OpenAICompletionBaseURL;
            }
            if (Properties.Settings.Default.OpenAIEmbeddingBaseURL != "") {
                openAIProperties.OpenAIEmbeddingBaseURL = Properties.Settings.Default.OpenAIEmbeddingBaseURL;
            }
            return openAIProperties;
        }

    }
}
