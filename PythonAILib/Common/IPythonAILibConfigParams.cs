namespace PythonAILib.Common {
    public interface IPythonAILibConfigParams {

        public string GetLang();
        public string GetPythonDllPath();
        public string GetDBPath();

        public string GetSystemVectorDBPath();

        public string GetSystemDocDBPath();

        public string GetPathToVirtualEnv();

        public string GetAppDataPath();

        public IDataFactory GetDataFactory();

        public OpenAIProperties GetOpenAIProperties();

        public Action<string> GetInfoAction();

        public Action<string> GetWarnAction();

        public Action<string> GetErrorAction();

        public string GetHttpProxy();

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

        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle();

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

    }
}
