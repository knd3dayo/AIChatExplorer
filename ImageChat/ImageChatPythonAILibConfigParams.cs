using System.IO;
using System.Windows;
using ImageChat.Model;
using PythonAILib.Common;
using QAChat.Abstract;
using WpfAppCommon.Utils;

namespace ImageChat {
    public class ImageChatPythonAILibConfigParams : IPythonAILibConfigParams, IQAChatConfigParams {

        public string GetHttpProxy() {
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
        public string GetSystemVectorDBPath() {
            string vectorDBPath = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "imagechat_vector_db");
            return vectorDBPath;
        }

        public string GetSystemDocDBPath() {
            string docDBPath = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "imagechat_doc_store.db");
            return docDBPath;

        }
        public string GetAutoGenWorkDir() {
            string workDir = Path.Combine(ImageChatConfig.Instance.AppDataFolder, "autogen");
            return workDir;
        }
        public string GetCatalogDBURL() {
            string dbUrl = string.Concat("sqlite:///", Path.Combine(ImageChatConfig.Instance.AppDataFolder, "catalog.db"));
            return dbUrl;
        }

    }
}
