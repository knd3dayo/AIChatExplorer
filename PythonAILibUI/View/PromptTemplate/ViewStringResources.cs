using QAChat.Resource;

namespace QAChat.View.PromptTemplate {
    public class ViewStringResources {

        public static CommonStringResources CommonStringResources { get; set; } = CommonStringResources.Instance;

        // Clear
        public static string Clear { get; set; } = CommonStringResources.Clear;

        // Close
        public static string Close { get; set; } = CommonStringResources.Close;

        // SelectFolder
        public static string SelectFolder { get; set; } = CommonStringResources.SelectFolder;

        // Select
        public static string Select { get; set; } = CommonStringResources.Select;

        // Cancel
        public static string Cancel { get; set; } = CommonStringResources.Cancel;

        // 文字列リソース
        public static string Title { get; set; } = CommonStringResources.EditPrompt;
        public static string Name { get; set; } = CommonStringResources.Name;
        public static string Description { get; set; } = CommonStringResources.Description;
        // 自動処理時の設定
        public static string AutoProcessSetting { get; set; } = CommonStringResources.AutoProcessSetting;
        // Chatタイプ
        public static string ChatType { get; set; } = CommonStringResources.ChatType;
        // 出力形式
        public static string OutputType { get; set; } = CommonStringResources.OutputType;

        // 文字列
        public static string StringType { get; set; } = CommonStringResources.StringType;
        // リスト
        public static string ListType { get; set; } = CommonStringResources.ListType;
        // テーブル
        public static string TableType { get; set; } = CommonStringResources.TableType;
        // 出力先
        public static string OutputDestination { get; set; } = CommonStringResources.OutputDestination;
        // 新規タブ
        public static string NewTab { get; set; } = CommonStringResources.NewTab;
        // 本文を上書き
        public static string OverwriteContent { get; set; } = CommonStringResources.OverwriteContent;
        // タイトルを上書き
        public static string OverwriteTitle { get; set; } = CommonStringResources.OverwriteTitle;
        // OK
        public static string OK { get; set; } = CommonStringResources.OK;
        // ExperimentalFunction1
        public static string ExperimentalFunction1 { get; set; } = CommonStringResources.ExperimentalFunction1;
        // PromptTemplateList
        public static string PromptTemplateList { get; set; } = CommonStringResources.PromptTemplateList;
        // DisplayPromptsForTheSystem
        public static string DisplayPromptsForTheSystem { get; set; } = CommonStringResources.DisplayPromptsForTheSystem;
        // NewPromptTemplate
        public static string NewPromptTemplate { get; set; } = CommonStringResources.NewPromptTemplate;

        // EditPromptTemplate
        public static string EditPromptTemplate { get; set; } = CommonStringResources.EditPromptTemplate;


    }
}
