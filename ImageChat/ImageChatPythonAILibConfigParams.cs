using System.IO;
using System.Windows;
using ImageChat.Model;
using PythonAILib.Common;
using QAChat.Abstract;
using WpfAppCommon.Utils;

namespace ImageChat {
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
            throw new System.NotImplementedException();
        }

        // AutoTitle
        public bool AutoTitle() {
            throw new System.NotImplementedException();
        }
        // AutoTitleWithOpenAI
        public bool AutoTitleWithOpenAI() {
            throw new System.NotImplementedException();
        }

        // AutoBackgroundInfo
        public bool AutoBackgroundInfo() {
            throw new System.NotImplementedException();
        }

        // AutoSummary
        public bool AutoSummary() {
            throw new System.NotImplementedException();
        }

        // AutoGenerateTasks
        public bool AutoGenerateTasks() {
            throw new System.NotImplementedException();
        }

        // AutoDocumentReliabilityCheck
        public bool AutoDocumentReliabilityCheck() {
            throw new System.NotImplementedException();
        }
        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle() {
            throw new System.NotImplementedException();
        }

        // AutoFileExtract
        public bool AutoFileExtract() {
            throw new System.NotImplementedException();
        }

        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR() {
            throw new System.NotImplementedException();
        }

        // AutoExtractImageWithOpenAI
        public bool AutoExtractImageWithOpenAI() {
            throw new System.NotImplementedException();
        }
        // IgnoreLineCount
        public int IgnoreLineCount() {
            throw new System.NotImplementedException();
        }
        // TesseractExePath
        public string TesseractExePath() {
            throw new System.NotImplementedException();
        }
        // public bool DevFeaturesEnabled();
        public bool DevFeaturesEnabled() {
            throw new System.NotImplementedException();
        }
    }
}
