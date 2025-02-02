namespace MergeChat.Resources {
    public class MergeChatStringResources {


        private static MergeChatStringResources? _instance;
        public static MergeChatStringResources Instance {
            get {
                if (_instance == null || _LangChanged) {
                    _LangChanged = false;
                    switch (Lang) {
                        case "ja-JP":
                            _instance = new MergeChatStringResources();
                            break;
                        default:
                            _instance = new MergeChatStringResourcesEn();
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


        // MergeTarget
        public virtual string MergeTarget { get; set; } = "マージ対象";

        // MergeTargetItemSelection
        public virtual string MergeTargetItemSelection { get; set; } = "マージ対象アイテム選択";

        // MergeTargetDataSelection
        public virtual string MergeTargetDataSelection { get; set; } = "マージ対象データ";

        // 事前処理用プロンプト マージ前に各アイテムに対して事前処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public virtual string PreProcessingPromptHint { get; set; } = """
            事前処理用プロンプト マージ前に各アイテムに対して事前処理を行うためのプロンプト。
            ダブルクリックするとプロンプトテンプレート選択画面が開きます。
            """;

        // 事後処理用プロンプト マージ後のアイテムに対して事後処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public virtual string PostProcessingPromptHint { get; set; } = """
            事後処理用プロンプト マージ後のアイテムに対して事後処理を行うためのプロンプト
            ダブルクリックするとプロンプトテンプレート選択画面が開きます。
            """;

        // OutputFolderSelection
        public virtual string OutputFolderSelection { get; set; } = "出力フォルダ選択";

    }
}
