using LibMain.Resources;
using LibUIMain.Resource;

namespace LibUIImageChat.View {
    public class ViewStringResources {
        public static CommonStringResources CommonStringResources { get; set; } = CommonStringResources.Instance;

        // Cancel
        public static string Cancel => CommonStringResources.Cancel;

        // Open
        public static string Open => CommonStringResources.Open;
        // DeleteAsync
        public static string Delete => CommonStringResources.Delete;

        // TheAnswerWillBeDisplayedHere
        public static string TheAnswerWillBeDisplayedHere => CommonStringResources.TheAnswerWillBeDisplayedHere;

        // PromptTemplate
        public static string PromptTemplate => CommonStringResources.PromptTemplate;

        // EnterYourQuestionHere
        public static string EnterYourQuestionHere => CommonStringResources.EnterYourQuestionHere;

        // Send
        public static string Send => CommonStringResources.Send;

        // Clear
        public static string Clear => CommonStringResources.Clear;

        // Close
        public static string Close => CommonStringResources.Close;

        // ScreenShotCheckPromptWindowTitle
        public static string ScreenShotCheckPromptWindowTitle => CommonStringResources.ScreenShotCheckPromptWindowTitle;

        // ImageChat
        public static string ImageChat => PythonAILibStringResources.Instance.ImageChat;

        // SettingItem
        public static string SettingItem => CommonStringResources.SettingItem;

        // SettingValue
        public static string SettingValue => CommonStringResources.SettingValue;

        // CheckType
        public static string CheckType => CommonStringResources.CheckType;

        // OK
        public static string OK => CommonStringResources.OK;

        // File
        public static string File => CommonStringResources.File;

        // SelectImageFile
        public static string SelectImageFile => CommonStringResources.SelectImageFile;

        // Edit
        public static string Edit => CommonStringResources.Edit;

        // EditImageEvidenceCheckItem
        public static string EditImageEvidenceCheckItem => CommonStringResources.EditImageEvidenceCheckItem;

        // Title
        public static string Title => CommonStringResources.Title;
        // SplitRequestByImage
        public static string SplitRequestByImage => CommonStringResources.SplitRequestByImage;
    }
}
