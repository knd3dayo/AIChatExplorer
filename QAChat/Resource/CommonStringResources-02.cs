namespace QAChat.Resource {
    public partial class CommonStringResources {

        #region EditItemWindow関連
        // 新規アイテム
        public virtual string NewItem { get; } = "新規アイテム";
        #endregion

        #region FolderView関連
        // 自フォルダを参照先ベクトルDBに追加
        public virtual string AddMyFolderToTargetVectorDB { get; } = "自フォルダを参照先ベクトルDBに追加";

        // こにフォルダの説明を入力
        public virtual string InputDescriptionOfThisFolder { get; } = "こにフォルダの説明を入力";

        #endregion

        #region ClipboardApp MainWindow
        // クリップボード監視開始
        public virtual string StartClipboardWatch { get; } = "クリップボード監視開始";
        // クリップボード監視停止
        public virtual string StopClipboardWatch { get; } = "クリップボード監視停止";
        // Windows通知監視開始
        public virtual string StartNotificationWatch { get; } = "Windows通知監視開始";
        // Windows通知監視停止
        public virtual string StopNotificationWatch { get; } = "Windows通知監視停止";

        // クリップボード監視を開始しました
        public virtual string StartClipboardWatchMessage { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public virtual string StopClipboardWatchMessage { get; } = "クリップボード監視を停止しました";
        // Windows通知監視を開始しました
        public virtual string StartNotificationWatchMessage { get; } = "Windows通知監視を開始しました";
        // Windows通知監視を停止しました
        public virtual string StopNotificationWatchMessage { get; } = "Windows通知監視を停止しました";

        // タグ編集
        public virtual string EditTag { get; } = "タグ編集";
        // 自動処理ルール編集
        public virtual string EditAutoProcessRule { get; } = "自動処理ルール編集";
        // Pythonスクリプト編集
        public virtual string EditPythonScript { get; } = "Pythonスクリプト編集";
        // プロンプトテンプレート編集
        public virtual string EditPromptTemplate { get; } = "プロンプトテンプレート編集";
        // RAGソース編集
        public virtual string EditGitRagSource { get; } = "RAGソース(git)編集";

        // -- 表示メニュー
        // テキストを右端で折り返す
        public virtual string TextWrapping { get; } = "テキストを右端で折り返す";
        // サイズの大きなテキストの場合は自動的に折り返しを解除する
        public virtual string AutoTextWrapping { get; } = "サイズの大きなテキストの場合は自動的に折り返しを解除する";

        // プレビューモード
        public virtual string PreviewMode { get; } = "プレビューを有効にする";
        // コンパクト表示モード
        public virtual string CompactMode { get; } = "コンパクト表示にする";

        // ツール
        public virtual string Tool { get; } = "ツール";
        // OpenAIチャット
        public virtual string OpenAIChat { get; } = "OpenAIチャット";
        // 画像エビデンスチェッカー
        public virtual string ImageChat { get; } = "イメージチャット";

        #endregion

        #region SettingWindow
        // ProxyURL
        public virtual string ProxyURL { get; } = "ProxyサーバーのURL";
        // NoProxyList
        public virtual string NoProxyList { get; } = "Proxy除外リスト";
        #endregion
    }
}
