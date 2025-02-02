namespace QAChat.Resource {
    public partial class CommonStringResources {

        private static CommonStringResources? _instance;
        public static CommonStringResources Instance {
            get {
                if (_instance == null || _LangChanged) {
                    _LangChanged = false;
                    switch (Lang) {
                        case "ja-JP":
                            _instance = new CommonStringResources();
                            break;
                        default:
                            _instance = new CommonStringResourcesEn();
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

        #region --- 共通 ------------------------------------------------------ 
        // ファイル
        public virtual string File { get; } = "ファイル";

        // ファイル/画像
        public virtual string FileOrImage { get; } = "ファイル/画像";

        // チャット内容
        public virtual string ChatContent { get; } = "チャット内容";
        // 作成
        public virtual string Create { get; } = "作成";
        // アイテム作成
        public virtual string CreateItem { get; } = "アイテム作成";
        // 終了
        public virtual string Exit { get; } = "終了";
        // 編集
        public virtual string Edit { get; } = "編集";

        // 開始
        public virtual string Start { get; } = "開始";
        // 停止
        public virtual string Stop { get; } = "停止";
        // 選択
        public virtual string Select { get; } = "選択";
        // ヘルプ
        public virtual string Help { get; } = "ヘルプ";
        // バージョン情報
        public virtual string VersionInfo { get; } = "バージョン情報";

        // プロパティ表示
        public virtual string ShowProperties { get; } = "プロパティ表示";

        // 表示
        public virtual string View { get; } = "表示";

        // 検索
        public virtual string Search { get; } = "検索";
        // 設定
        public virtual string Setting { get; } = "設定";
        // 削除
        public virtual string Delete { get; } = "削除";
        // "削除しました"
        public virtual string Deleted { get; } = "削除しました";

        // 追加
        public virtual string Add { get; } = "追加";
        // OK
        public virtual string OK { get; } = "OK";
        // キャンセル
        public virtual string Cancel { get; } = "キャンセル";
        // 閉じる
        public virtual string Close { get; } = "閉じる";

        // ショートカット登録
        public virtual string CreateShortCut { get; } = "ショートカット登録";
        
        // ロード
        public virtual string Load { get; } = "ロード";

        // 同期
        public virtual string Sync { get; } = "同期";

        // ExportImport
        public virtual string ExportImport { get; } = "エクスポート/インポート";

        // Export
        public virtual string ExportToExcel { get; } = "Excelへエクスポート";
        // Import
        public virtual string ImportFromExcel { get; } = "Excelからインポート";

        // ImportFromRULList
        public virtual string ImportFromRULList { get; } = "URLリストからインポート";

        // モード
        public virtual string ChatMode { get; } = "チャットモード";

        // OpenAI
        public virtual string ChatMode_OpenAI { get; } = "OpenAI";

        // OpenAI_RAG
        public virtual string ChatMode_OpenAI_RAG { get; } = "OpenAI_RAG";

        // AutoGen_GroupChat
        public virtual string ChatMode_AutoGen_GroupChat { get; } = "AutoGen_GroupChat";

        // AutoGen_NormalChat
        public virtual string ChatMode_AutoGen_NormalChat { get; } = "AutoGen_NormalChat";

        // AutoGent_NestedChat
        public virtual string ChatMode_AutoGen_NestedChat { get; } = "AutoGen_NestedChat";

        // GroupChatName
        public virtual string GroupChatName { get; } = "グループチャット名";

        // Timeout
        public virtual string Timeout { get; } = "タイムアウト";

        // TerminateMessage
        public virtual string TerminateMessage { get; } = "終了メッセージ";

        // MaxMsg
        public virtual string MaxMsg { get; } = "最大メッセージ数";

        // 分割モード
        public virtual string SplitMode { get; } = "リクエスト分割モード";

        // 分割モード なし
        public virtual string SplitMode_None { get; } = "なし";

        // 分割モード 指定したトークン数を超える場合はリクエストを分割
        public virtual string SplitMode_SplitIfExceedSpecifiedToken { get; } = "指定したトークン数を超える場合はリクエストを分割";

        // 指定したトークン数を超える場合はリクエストを分割して要約n
        public virtual string SplitMode_SplitAndSummarizeIfExceedSpecifiedToken { get; } = "指定したトークン数を超える場合はリクエストを分割して要約";

        // PromptTextIsNeededWhenSplitModeIsEnabled
        public virtual string PromptTextIsNeededWhenSplitModeIsEnabled { get; } = "分割モードが有効な場合はプロンプトテキストが必要です";

        // リクエストを分割するトークン数
        public virtual string NumberOfTokensToSplitRequest { get; } = "リクエストを分割するトークン数";

        // DataGridIsNotFound
        public virtual string DataGridIsNotFound { get; } = "DataGridが見つかりません";

        // 貼り付け
        public virtual string Paste { get; } = "貼り付け";
        // 開く
        public virtual string Open { get; } = "開く";
        // 保存
        public virtual string Save { get; } = "保存";

        // 送信
        public virtual string Send { get; } = "送信";
        // Copy
        public virtual string Copy { get; } = "Copy";

        // すべて選択します。
        public virtual string SelectAll { get; } = "すべて選択";
        // すべて選択解除します。
        public virtual string UnselectAll { get; } = "すべて選択解除";

        // 名前
        public virtual string Name { get; } = "名前";

        // 説明
        public virtual string Description { get; } = "説明";
        // 文字列
        public virtual string StringType { get; } = "文字列";
        // リスト
        public virtual string ListType { get; } = "リスト";
        // テーブル
        public virtual string TableType { get; } = "テーブル";
        // 出力先
        public virtual string OutputDestination { get; } = "出力先";

        // クリア
        public virtual string Clear { get; } = "クリア";

        // 確認
        public virtual string Confirm { get; } = "確認";
        // 実行
        public virtual string Execute { get; } = "実行";



        #endregion


    }
}
