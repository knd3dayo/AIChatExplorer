using System.Windows;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Common {
    public interface IPythonAILibConfigParams {

        public string GetLang();

        #region 削除候補
        public string GetPythonLibPath();

        public string GetPathToVirtualEnv();

        #endregion

        #region APIサーバー関連
        public string GetAPIServerURL();

        public bool IsUseInternalAPI();

        public bool IsUseExternalAPI();
        #endregion

        public string GetMainDBPath();

        public string GetAppDataPath();

        public string GetContentOutputPath();

        public OpenAIProperties GetOpenAIProperties();

        public ILogWrapperAction GetLogWrapperAction();

        public string GetHttpsProxy();

        public string GetNoProxy();

        public string GetAutoGenWorkDir();

        public string GetAutoGenToolDir();


        public bool IsAutoTagEnabled();

        // IsAutoTitleEnabled
        public bool IsAutoTitleEnabled();
        // IsAutoTitleWithOpenAIEnabled
        public bool IsAutoTitleWithOpenAIEnabled();

        // IsAutoBackgroundInfoEnabled
        public bool IsAutoBackgroundInfoEnabled();

        // IsAutoSummaryEnabled
        public bool IsAutoSummaryEnabled();

        // IsAutoGenerateTasksEnabled
        public bool IsAutoGenerateTasksEnabled();

        // IsAutoDocumentReliabilityCheckEnabled
        public bool IsAutoDocumentReliabilityCheckEnabled();

        // IsAutoFileExtractEnabled
        public bool IsAutoFileExtractEnabled();
        
        // IsAutoExtractImageWithOpenAIEnabled
        public bool IsAutoExtractImageWithOpenAIEnabled();

        // IsAutoPredictUserIntentEnabled
        public bool IsAutoPredictUserIntentEnabled();

        // GetIgnoreLineCount
        public int GetIgnoreLineCount();

        // IsDevFeaturesEnabled
        public bool IsDevFeaturesEnabled();

        public void UpdateDevFeaturesEnabled(bool value);

        // GUI
        public TextWrapping GetTextWrapping();

        // MarkdownView
        public bool IsMarkdownView();

        public void UpdateMarkdownView(bool value);

        // TextWrapping
        public bool IsTextWrapping();
        public void UpdateTextWrapping(TextWrapping value);

        // AutoTextWrapping
        public bool IsAutoTextWrapping();

        public void UpdateAutoTextWrapping(bool value);

        // ShowProperties
        public bool IsShowProperties();

        public void UpdateShowProperties(bool value);

        // ClipboardMonitoring
        public string GetMonitorTargetAppNames();

        public int GetEditorFontSize();

        public bool MaterialDesignDarkTheme();

        public void UpdateMaterialDesignDarkTheme(bool value);


    }
}
