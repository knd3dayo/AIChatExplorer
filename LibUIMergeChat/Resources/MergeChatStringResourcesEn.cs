namespace MergeChat.Resources {
    public class MergeChatStringResourcesEn: MergeChatStringResources {

        // マージ対象
        public override string MergeTarget { get; set; } = "Merge Target";

        // マージ対象アイテム
        public override string MergeTargetItemSelection { get; set; } = "Merge Target Item";

        // マージ対象データ
        public override string MergeTargetDataSelection { get; set; } = "Merge Target Data";

        // 事前処理用プロンプト マージ前に各アイテムに対して事前処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public override string PreProcessingPromptHint { get; set; } = " Pre-processing prompt for performing pre-processing on each item before merging. Double-click to open the prompt template selection screen.";

        // 事後処理用プロンプト マージ後のアイテムに対して事後処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public override string PostProcessingPromptHint { get; set; } = "Post-processing prompt for performing post-processing on items after merging. Double-click to open the prompt template selection screen.";

        // OutputFolderSelection
        public override string OutputFolderSelection { get; set; } = "Output Folder Selection";
    }
}
