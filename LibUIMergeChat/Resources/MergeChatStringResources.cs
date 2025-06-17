namespace LibUIMergeChat.Resources {
    public class MergeChatStringResources  {

        private static MergeChatStringResources? _instance;
        public static MergeChatStringResources Instance {
            get {
                if (_instance == null || _LangChanged) {
                    _LangChanged = false;
                    switch (Lang) {
                        case "ja-JP":
                            _instance = new MergeChatStringResourcesJa();
                            break;
                        default:
                            _instance = new MergeChatStringResources();
                            break;
                    }
                }
                return _instance;
            }
        }
        private static bool _LangChanged = false;
        private static string _Lang = "ja-JP";
        public static string Lang {
            get { return _Lang; }
            set {
                if (_Lang != value) {
                    _LangChanged = true;
                }
                _Lang = value;
            }
        }



        // マージ対象
        public virtual string MergeTarget { get; set; } = "Merge Target";

        // マージ対象アイテム
        public virtual string MergeTargetItemSelection { get; set; } = "Merge Target Item";

        // マージ対象データ
        public virtual string MergeTargetDataSelection { get; set; } = "Merge Target Data";

        // 事前処理用プロンプト マージ前に各アイテムに対して事前処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public virtual string PreProcessingPromptHint { get; set; } = " Pre-processing prompt for performing pre-processing on each item before merging. Double-click to open the prompt template selection screen.";

        // 事後処理用プロンプト マージ後のアイテムに対して事後処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public virtual string PostProcessingPromptHint { get; set; } = "Post-processing prompt for performing post-processing on items after merging. Double-click to open the prompt template selection screen.";

        // OutputFolderSelection
        public virtual string OutputFolderSelection { get; set; } = "Output Folder Selection";
    }
}
