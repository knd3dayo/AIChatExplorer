using System.Windows;
using LibPythonAI.Utils.Common;

namespace PythonAILib.Common {
    public interface IPythonAILibConfigParams {

        public string GetLang();

        #region 削除候補
        public string GetPythonLibPath();

        public string GetPathToVirtualEnv();

        #endregion

        #region APIサーバー関連
        public string GetAPIServerURL();

        public bool UseInternalAPI();

        public bool UseExternalAPI();
        #endregion

        public string GetDBPath();

        public string GetMainDBPath();

        public string GetSystemVectorDBPath();

        public string GetSystemDocDBPath();

        public string GetAppDataPath();

        public string GetContentOutputPath();

        public OpenAIProperties GetOpenAIProperties();

        public ILogWrapperAction GetLogWrapperAction();

        public string GetHttpsProxy();

        public string GetNoProxy();

        public string GetAutoGenWorkDir();

        public string GetAutoGenToolDir();


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

        // MarkdownView
        public bool IsMarkdownView();

        public void UpdateMarkdownView(bool value);
    }
}
