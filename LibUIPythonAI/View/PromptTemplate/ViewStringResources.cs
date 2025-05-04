using LibUIPythonAI.Resource;

namespace LibUIPythonAI.View.PromptTemplate {
    public class ViewStringResources {

        public static CommonStringResources CommonStringResources { get; set; } = CommonStringResources.Instance;

        // Clear
        public static string Clear { get; set; } = CommonStringResources.Instance.Clear;

        // Close

        // Edit
        public static string Edit { get; set; } = CommonStringResources.Instance.Edit;

        // Delete
        public static string Delete { get; set; } = CommonStringResources.Instance.Delete;

        public static string Close { get; set; } = CommonStringResources.Instance.Close;

        // SelectFolder
        public static string SelectFolder { get; set; } = CommonStringResources.Instance.SelectFolder;

        // Select
        public static string Select { get; set; } = CommonStringResources.Instance.Select;

        // Cancel
        public static string Cancel { get; set; } = CommonStringResources.Instance.Cancel;

        // 文字列リソース
        public static string Title { get; set; } = CommonStringResources.Instance.EditPrompt;
        public static string Name { get; set; } = CommonStringResources.Instance.Name;
        public static string Description { get; set; } = CommonStringResources.Instance.Description;
        // 自動処理時の設定
        public static string AutoProcessSetting { get; set; } = CommonStringResources.Instance.AutoProcessSetting;

        // プロンプト結果を入力にする
        public static string PromptResultToInput { get; set; } = CommonStringResources.Instance.PromptResultToInput;
        // 入力となるプロンプトテンプレート
        public static string PromptInputName { get; set; } = CommonStringResources.Instance.PromptInputName;

        // Chatモード
        public static string ChatMode { get; set; } = CommonStringResources.Instance.ChatMode;

        // ChatMode_OpenAI
        public static string ChatMode_OpenAI => CommonStringResources.Instance.ChatMode_OpenAI;

        // ChatMode_OpenAI_RAG
        public static string ChatMode_OpenAI_RAG => CommonStringResources.Instance.ChatMode_OpenAI_RAG;

        // ChatMode_AutoGen_GroupChat
        public static string ChatMode_AutoGen_GroupChat => CommonStringResources.Instance.ChatMode_AutoGen_GroupChat;

        // 分割モード
        public static string SplitMode { get; set; } = CommonStringResources.Instance.SplitMode;

        // SplitMode_None
        public static string SplitMode_None => CommonStringResources.Instance.SplitMode_None;

        // SplitMode_SplitIfExceedSpecifiedToken
        public static string SplitMode_SplitIfExceedMaxToken => CommonStringResources.Instance.SplitMode_SplitIfExceedSpecifiedToken;

        // SplitMode_SplitAndSummarizeIfExceedSpecifiedToken
        public static string SplitMode_SplitAndSummarizeIfExceedMaxToken => CommonStringResources.Instance.SplitMode_SplitAndSummarizeIfExceedSpecifiedToken;


        // UseVectorDB
        public static string UseVectorDB { get; set; } = CommonStringResources.Instance.UseVectorDB;

        // 出力形式
        public static string OutputType { get; set; } = CommonStringResources.Instance.OutputType;

        // 文字列
        public static string StringType { get; set; } = CommonStringResources.Instance.StringType;
        // リスト
        public static string ListType { get; set; } = CommonStringResources.Instance.ListType;
        // テーブル
        public static string TableType { get; set; } = CommonStringResources.Instance.TableType;

        // Dictionary
        public static string DictionaryType { get; set; } = CommonStringResources.Instance.DictionaryType;

        // 出力先
        public static string OutputDestination { get; set; } = CommonStringResources.Instance.OutputDestination;
        // 新規タブ
        public static string NewTab { get; set; } = CommonStringResources.Instance.NewTab;
        // 本文を上書き
        public static string OverwriteContent { get; set; } = CommonStringResources.Instance.OverwriteContent;
        // タイトルを上書き
        public static string OverwriteTitle { get; set; } = CommonStringResources.Instance.OverwriteTitle;

        // タグ追加
        public static string AppendTags { get; set; } = CommonStringResources.Instance.AppendTags;
        // OK
        public static string OK { get; set; } = CommonStringResources.Instance.OK;
        // ExperimentalFunction1
        public static string ExperimentalFunction1 { get; set; } = CommonStringResources.Instance.ExperimentalFunction1;
        // PromptTemplateList
        public static string PromptTemplateList { get; set; } = CommonStringResources.Instance.PromptTemplateList;
        // DisplayPromptsForTheSystem
        public static string DisplayPromptsForTheSystem { get; set; } = CommonStringResources.Instance.DisplayPromptsForTheSystem;
        // NewPromptTemplate
        public static string NewPromptTemplate { get; set; } = CommonStringResources.Instance.NewPromptTemplate;

        // EditPromptTemplate
        public static string EditPromptTemplate { get; set; } = CommonStringResources.Instance.EditPromptTemplate;

        // File Menu
        public static string FileMenu { get; set; } = CommonStringResources.Instance.File;

        // Export
        public static string ExportToExcel { get; set; } = CommonStringResources.Instance.ExportToExcel;

        // Import
        public static string ImportFromExcel { get; set; } = CommonStringResources.Instance.ImportFromExcel;

    }
}
