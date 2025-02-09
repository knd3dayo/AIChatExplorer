using System.Windows;

namespace PythonAILib.Common {
    public interface IPythonAILibConfigParams {

        public string GetLang();

        #region 削除候補
        public string GetPythonDllPath();
        
        public string GetPythonLibPath();

        public string GetPathToVirtualEnv();

        #endregion

        #region APIサーバー関連
        public string GetAPIServerURL();

        public bool UseInternalAPI();

        public bool UseExternalAPI();

        public bool UsePythonNet();
        #endregion

        public string GetDBPath();

        public string GetSystemVectorDBPath();

        public string GetSystemDocDBPath();

        public string GetAutoGenDBPath();

        public string GetAppDataPath();

        public IDataFactory GetDataFactory();

        public OpenAIProperties GetOpenAIProperties();

        public Action<string> GetDebugAction();
        public Action<string> GetInfoAction();

        public Action<string> GetWarnAction();

        public Action<string> GetErrorAction();

        public string GetHttpsProxy();

        public string GetNoProxy();

        public string GetAutoGenWorkDir();

        public string GetCatalogDBURL();

        public bool AutoTag();

        // AutoTitle
        public bool AutoTitle();
        // AutoTitleWithOpenAI
        public bool AutoTitleWithOpenAI();

        // AutoBackgroundInfo
        public bool AutoBackgroundInfo();

        // AutoSummary
        public bool AutoSummary();

        // AutoGenerateTasks
        public bool AutoGenerateTasks();

        // AutoDocumentReliabilityCheck
        public bool AutoDocumentReliabilityCheck();

        // AutoFileExtract
        public bool AutoFileExtract();
        
        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR();

        // AutoExtractImageWithOpenAI
        public bool AutoExtractImageWithOpenAI();

        // IgnoreLineCount
        public int IgnoreLineCount();

        // TesseractExePath
        public string TesseractExePath();

        // DevFeaturesEnabled
        public bool DevFeaturesEnabled();

        // GUI
        public TextWrapping GetTextWrapping();
    }
}
