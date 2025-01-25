using System.IO;
using System.Windows;
using PythonAILib.Common;
using QAChat.Abstract;
using WpfAppCommon.Utils;

namespace ImageChat.Common {
    public class ImageChatPythonAILibConfigParams : IPythonAILibConfigParams, IQAChatConfigParams {

        public string GetHttpsProxy() {
            return ImageChatConfig.Instance.ProxyURL;
        }
        public string GetNoProxy() {
            return ImageChatConfig.Instance.NoProxyList;
        }
        public string GetLang() {
            return ImageChatConfig.Instance.ActualLang;
        }
        public string GetPythonDllPath() {
            return ImageChatConfig.Instance.PythonDllPath;
        }
        public string GetPathToVirtualEnv() {
            return ImageChatConfig.Instance.PythonVenvPath;
        }
        public string GetAppDataPath() {
            return ImageChatConfig.Instance.AppDataFolder;
        }

        public IDataFactory GetDataFactory() {
            return ImageChatDataFactory.Instance;
        }
        public OpenAIProperties GetOpenAIProperties() {
            return ImageChatConfig.Instance.CreateOpenAIProperties();
        }

        public Action<string> GetInfoAction() {
            return LogWrapper.Info;
        }
        public Action<string> GetWarnAction() {
            return LogWrapper.Warn;
        }
        public Action<string> GetErrorAction() {
            return LogWrapper.Error;
        }

        public TextWrapping GetTextWrapping() {
            return ImageChatConfig.Instance.TextWrapping;
        }
        public string GetDBPath() {

            /// AppDataフォルダーパスを取得
            string appDataPath = ImageChatConfig.Instance.AppDataFolder;
            // データベースファイルのパスを作成
            string dbPath = Path.Combine(appDataPath, "imagechat.db");

            return dbPath;

        }
        public string GetPythonLibPath() {
            /// Get AppData folder path
            string appDataPath = ImageChatConfig.Instance.AppDataFolder;
            // Create database file path
            string path = Path.Combine(appDataPath, "python_ai_lib");
            return path;

        }

        public string GetSystemVectorDBPath() {
            string vectorDBPath = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "imagechat_vector_db");
            return vectorDBPath;
        }

        public string GetSystemDocDBPath() {
            string docDBPath = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "imagechat_doc_store.db");
            return docDBPath;

        }

        public string GetAutoGenDBPath() {
            string dbPath = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "autogen.db");
            return dbPath;
        }

        public string GetAutoGenWorkDir() {
            string workDir = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "autogen");
            return workDir;
        }
        public string GetCatalogDBURL() {
            string dbUrl = string.Concat("sqlite:///", Path.Combine(ImageChatConfig.Instance.AppDataFolder, "catalog.db"));
            return dbUrl;
        }
        public bool AutoTag() {
            throw new NotImplementedException();
        }

        // AutoTitle
        public bool AutoTitle() {
            throw new NotImplementedException();
        }
        // AutoTitleWithOpenAI
        public bool AutoTitleWithOpenAI() {
            throw new NotImplementedException();
        }

        // AutoBackgroundInfo
        public bool AutoBackgroundInfo() {
            throw new NotImplementedException();
        }

        // AutoSummary
        public bool AutoSummary() {
            throw new NotImplementedException();
        }

        // AutoGenerateTasks
        public bool AutoGenerateTasks() {
            throw new NotImplementedException();
        }

        // AutoDocumentReliabilityCheck
        public bool AutoDocumentReliabilityCheck() {
            throw new NotImplementedException();
        }
        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle() {
            throw new NotImplementedException();
        }

        // AutoFileExtract
        public bool AutoFileExtract() {
            throw new NotImplementedException();
        }

        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR() {
            throw new NotImplementedException();
        }

        // AutoExtractImageWithOpenAI
        public bool AutoExtractImageWithOpenAI() {
            throw new NotImplementedException();
        }
        // IgnoreLineCount
        public int IgnoreLineCount() {
            throw new NotImplementedException();
        }
        // TesseractExePath
        public string TesseractExePath() {
            throw new NotImplementedException();
        }
        // public bool DevFeaturesEnabled();
        public bool DevFeaturesEnabled() {
            throw new NotImplementedException();
        }
    }
}
