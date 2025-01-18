using QAChat.Resource;

namespace QAChat.View.PromptTemplate {
    public class ViewStringResources {

        public static CommonStringResources CommonStringResources { get; set; } = CommonStringResources.Instance;

        // Clear
        public static string Clear { get; set; } = CommonStringResources.Instance.Clear;

        // Close
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

        // Chatタイプ
        public static string ChatType { get; set; } = CommonStringResources.Instance.ChatType;
        // 出力形式
        public static string OutputType { get; set; } = CommonStringResources.Instance.OutputType;

        // 文字列
        public static string StringType { get; set; } = CommonStringResources.Instance.StringType;
        // リスト
        public static string ListType { get; set; } = CommonStringResources.Instance.ListType;
        // テーブル
        public static string TableType { get; set; } = CommonStringResources.Instance.TableType;
        // 出力先
        public static string OutputDestination { get; set; } = CommonStringResources.Instance.OutputDestination;
        // 新規タブ
        public static string NewTab { get; set; } = CommonStringResources.Instance.NewTab;
        // 本文を上書き
        public static string OverwriteContent { get; set; } = CommonStringResources.Instance.OverwriteContent;
        // タイトルを上書き
        public static string OverwriteTitle { get; set; } = CommonStringResources.Instance.OverwriteTitle;
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


    }
}
