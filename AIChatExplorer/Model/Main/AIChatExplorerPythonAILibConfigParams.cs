using System.IO;
using System.Windows;
using AIChatExplorer.ViewModel.Settings;
using LibPythonAI.Common;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;

namespace AIChatExplorer.Model.Main {
    public class AIChatExplorerPythonAILibConfigParams : IPythonAILibConfigParams {

        public string GetHttpsProxy() {
            return AIChatExplorerConfig.Instance.ProxyURL;
        }
        public string GetNoProxy() {
            return AIChatExplorerConfig.Instance.NoProxyList;
        }

        public string GetLang() {
            return AIChatExplorerConfig.Instance.ActualLang;
        }


        public string GetPathToVirtualEnv() {
            return AIChatExplorerConfig.Instance.PythonVenvPath;
        }
        public string GetAppDataPath() {
            return AIChatExplorerConfig.Instance.AppDataPath;
        }
        public string GetContentOutputPath() {
            return Path.Combine(AIChatExplorerConfig.Instance.AppDataPath, "content_output");
        }

        public OpenAIProperties GetOpenAIProperties() {
            return AIChatExplorerConfig.Instance.CreateOpenAIProperties();
        }


        public ILogWrapperAction GetLogWrapperAction() {
            return new LogWrapperAction();
        }

        public TextWrapping GetTextWrapping() {
            return AIChatExplorerConfig.Instance.TextWrapping;
        }


        public string GetMainDBPath() {
            /// Get AppData folder path
            string appDataPath = AIChatExplorerConfig.Instance.AppDataPath;
            // Create database file path
            string dbPath = Path.Combine(appDataPath, "client", "main_db");
            if (!Directory.Exists(dbPath)) {
                Directory.CreateDirectory(dbPath);
            }
            dbPath = Path.Combine(dbPath, "client_main.db");
            return dbPath;
        }
        public string GetPythonLibPath() {
            /// Get AppData folder path
            string appDataPath = AIChatExplorerConfig.Instance.AppDataPath;
            // Create database file path
            string path = Path.Combine(appDataPath, "python_lib");
            return path;

        }


        // AutoGenWorkDir
        public string GetAutoGenWorkDir() {
            string workDir = Path.Combine(AIChatExplorerConfig.Instance.AppDataPath, "autogen", "work");
            // Create directory if it does not exist
            if (!Directory.Exists(workDir)) {
                Directory.CreateDirectory(workDir);
            }
            return workDir;
        }
        // AutoGenToolDir
        public string GetAutoGenToolDir() {
            string toolDir = Path.Combine(AIChatExplorerConfig.Instance.AppDataPath, "autogen", "tools");
            // Create directory if it does not exist
            if (!Directory.Exists(toolDir)) {
                Directory.CreateDirectory(toolDir);
            }
            return toolDir;
        }

        public bool AutoTag() {
            return AIChatExplorerConfig.Instance.AutoTag;
        }

        // AutoTitle
        public bool AutoTitle() {
            return AIChatExplorerConfig.Instance.AutoDescription;
        }
        // AutoTitleWithOpenAI
        public bool AutoTitleWithOpenAI() {
            return AIChatExplorerConfig.Instance.AutoDescriptionWithOpenAI;
        }

        // AutoBackgroundInfo
        public bool AutoBackgroundInfo() {
            return AIChatExplorerConfig.Instance.AutoBackgroundInfo;
        }

        // AutoSummary
        public bool AutoSummary() {
            return AIChatExplorerConfig.Instance.AutoSummary;
        }

        // AutoGenerateTasks
        public bool AutoGenerateTasks() {
            return AIChatExplorerConfig.Instance.AutoGenerateTasks;
        }

        // AutoDocumentReliabilityCheck
        public bool AutoDocumentReliabilityCheck() {
            return AIChatExplorerConfig.Instance.AutoDocumentReliabilityCheck;
        }

        // AutoFileExtract
        public bool AutoFileExtract() {
            return AIChatExplorerConfig.Instance.AutoFileExtract;
        }

        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR() {
            return AIChatExplorerConfig.Instance.AutoExtractImageWithPyOCR;
        }

        // AutoExtractImageWithOpenAI
        public bool AutoExtractImageWithOpenAI() {
            return AIChatExplorerConfig.Instance.AutoExtractImageWithOpenAI;
        }
        // IgnoreLineCount
        public int IgnoreLineCount() {
            return AIChatExplorerConfig.Instance.IgnoreLineCount;
        }

        // public bool DevFeaturesEnabled();
        public bool DevFeaturesEnabled() {
            return AIChatExplorerConfig.Instance.EnableDevFeatures;
        }

        public void UpdateDevFeaturesEnabled(bool value) {
            AIChatExplorerConfig.Instance.EnableDevFeatures = value;
            AIChatExplorerConfig.Instance.Save();
        }

        // APIServerURL
        public string GetAPIServerURL() {
            return AIChatExplorerConfig.Instance.APIServerURL;
        }

        // UseInternalAPI
        public bool UseInternalAPI() {
            return AIChatExplorerConfig.Instance.UseInternalAPI;
        }
        // UseAPI
        public bool UseExternalAPI() {
            return AIChatExplorerConfig.Instance.UseExternalAPI;
        }
        // MarkdownView
        public bool IsMarkdownView() {
                return AIChatExplorerConfig.Instance.MarkdownView;
        }

        public void UpdateMarkdownView(bool value) {
            AIChatExplorerConfig.Instance.MarkdownView = value;
            AIChatExplorerConfig.Instance.Save();
        }

        // TextWrapping
        public bool IsTextWrapping() {
            return AIChatExplorerConfig.Instance.TextWrapping == TextWrapping.Wrap;
        }
        public void UpdateTextWrapping(TextWrapping value) {
            AIChatExplorerConfig.Instance.TextWrapping = value;
            AIChatExplorerConfig.Instance.Save();
        }

        // AutoTextWrapping
        public bool IsAutoTextWrapping() {
            return AIChatExplorerConfig.Instance.AutoTextWrapping;
        }

        public void UpdateAutoTextWrapping(bool value) {
            AIChatExplorerConfig.Instance.AutoTextWrapping = value;
            AIChatExplorerConfig.Instance.Save();
        }
        // AutoPredictUserIntent
        public bool AutoPredictUserIntent() {
            return AIChatExplorerConfig.Instance.AutoPredictUserIntent;
        }

        // ShowProperties
        public bool IsShowProperties() {
            return AIChatExplorerConfig.Instance.ShowProperties;
        }
        public void UpdateShowProperties(bool value) {
            AIChatExplorerConfig.Instance.ShowProperties = value;
            AIChatExplorerConfig.Instance.Save();
        }
    }
}
