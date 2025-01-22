using System.IO;
using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Settings;
using PythonAILib.Common;
using QAChat.Abstract;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class ClipboardAppPythonAILibConfigParams : IPythonAILibConfigParams, IQAChatConfigParams {

        public string GetHttpsProxy() {
            return ClipboardAppConfig.Instance.ProxyURL;
        }
        public string GetNoProxy() {
            return ClipboardAppConfig.Instance.NoProxyList;
        }

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
        public string GetPythonLibPath() {
            /// Get AppData folder path
            string appDataPath = ClipboardAppConfig.Instance.AppDataFolder;
            // Create database file path
            string path = Path.Combine(appDataPath, "python_ai_lib");
            return path;
        
        }

        public string GetSystemVectorDBPath() {
            string vectorDBPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "clipboard_vector_db");
            return vectorDBPath;
        }

        public string GetSystemDocDBPath() {
            string docDBPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "clipboard_doc_store.db");
            return docDBPath;
        }

        public string GetAutoGenDBPath() {
            string dbPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "autogen.db");
            return dbPath;
        }

        // AutoGenWorkDir
        public string GetAutoGenWorkDir() {
            string workDir = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "autogen");
            return workDir;
        }
        // GetCatalogDBURL
        public string GetCatalogDBURL() {
            string dbUrl = string.Concat("sqlite:///", Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "catalog.db"));
            return dbUrl;
        }

        public bool AutoTag() {
            return ClipboardAppConfig.Instance.AutoTag;
        }

        // AutoTitle
        public bool AutoTitle() {
            return ClipboardAppConfig.Instance.AutoDescription;
        }
        // AutoTitleWithOpenAI
        public bool AutoTitleWithOpenAI() {
            return ClipboardAppConfig.Instance.AutoDescriptionWithOpenAI;
        }

        // AutoBackgroundInfo
        public bool AutoBackgroundInfo() {
            return ClipboardAppConfig.Instance.AutoBackgroundInfo;
        }

        // AutoSummary
        public bool AutoSummary() {
            return ClipboardAppConfig.Instance.AutoSummary;
        }

        // AutoGenerateTasks
        public bool AutoGenerateTasks() {
            return ClipboardAppConfig.Instance.AutoGenerateTasks;
        }

        // AutoDocumentReliabilityCheck
        public bool AutoDocumentReliabilityCheck() {
            return ClipboardAppConfig.Instance.AutoDocumentReliabilityCheck;
        }
        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle() {
            return ClipboardAppConfig.Instance.AutoMergeItemsBySourceApplicationTitle;
        }

        // AutoFileExtract
        public bool AutoFileExtract() {
            return ClipboardAppConfig.Instance.AutoFileExtract;
        }

        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR() {
            return ClipboardAppConfig.Instance.AutoExtractImageWithPyOCR;
        }

        // AutoExtractImageWithOpenAI
        public bool AutoExtractImageWithOpenAI() {
            return ClipboardAppConfig.Instance.AutoExtractImageWithOpenAI;
        }
        // IgnoreLineCount
        public int IgnoreLineCount() {
            return ClipboardAppConfig.Instance.IgnoreLineCount;
        }
        // TesseractExePath
        public string TesseractExePath() {
            return ClipboardAppConfig.Instance.TesseractExePath;
        }
        // public bool DevFeaturesEnabled();
        public bool DevFeaturesEnabled() {
            return ClipboardAppConfig.Instance.EnableDevFeatures;
        }
    }
}
