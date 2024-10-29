using System.IO;
using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using QAChat.Abstract;
using WpfAppCommon.Utils;

namespace ClipboardApp {
    public class ClipboardAppPythonAILibConfigParams : IPythonAILibConfigParams, IQAChatConfigParams {

        public string GetLang() {
            return ClipboardAppConfig.Instance.ActualLang;
        }
        public string GetPythonDllPath() {
            return ClipboardAppConfig.Instance.PythonDllPath;
        }
        public string GetPathToVirtualEnv() {
            return ClipboardAppConfig.Instance.PythonVenvPath;
        }
        public string GetAppDataPath() {
            return ClipboardAppConfig.Instance.AppDataFolder;
        }

        public IDataFactory GetDataFactory() {
            return ClipboardAppFactory.Instance.GetClipboardDBController();
        }
        public OpenAIProperties GetOpenAIProperties() {
            return ClipboardAppConfig.Instance.CreateOpenAIProperties();
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
            return ClipboardAppConfig.Instance.TextWrapping;
        }

        public string GetDBPath() {
            /// Get AppData folder path
            string appDataPath = ClipboardAppConfig.Instance.AppDataFolder;
            // Create database file path
            string dbPath = Path.Combine(appDataPath, "clipboard.db");

            return dbPath;

        }
        public string GetSystemVectorDBPath() {
            string vectorDBPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "clipboard_vector_db");
            return vectorDBPath;
        }

        public string GetSystemDocDBPath() {
            string docDBPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "clipboard_doc_store.db");
            return docDBPath;
        }
    }
}
