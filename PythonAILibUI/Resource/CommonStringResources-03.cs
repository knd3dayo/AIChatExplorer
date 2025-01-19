namespace QAChat.Resource {
    public partial class CommonStringResources {

        #region QAChatControl

        // 全チャット内容をエクスポート
        public virtual string ExportAllChatContents { get; } = "全チャット内容をエクスポート";
        // 選択したチャット内容をコピー
        public virtual string CopySelectedChatContents { get; } = "選択したチャット内容をコピー";
        // 全のチャット内容をコピー
        public virtual string CopyAllChatContents { get; } = "全のチャット内容をコピー";

        #endregion

        #region AutoGen
        // AutoGen設定一覧
        public virtual string AutoGenSettingList { get; } = "AutoGen設定一覧";

        // AutoGenエージェント編集
        public virtual string EditAutoGenAgentWindowTitle { get; } = "AutoGenエージェント編集";

        // ToolsForLLM
        public virtual string ToolsForLLM { get; } = "ToolsForLLM";

        // ToolsForExecution
        public virtual string ToolsForExecution { get; } = "ToolsForExecution";

        // SystemMessage
        public virtual string SystemMessage { get; } = "SystemMessage";

        // グループチャット
        public virtual string GroupChat { get; } = "グループチャット";
        // 通常チャット
        public virtual string NormalChat { get; } = "通常チャット";

        // ネストチャット
        public virtual string NestedChat { get; } = "ネストチャット";

        // エージェント
        public virtual string Agent { get; } = "エージェント";

        // ソースパス
        public virtual string SourcePath { get; } = "ソースパス";

        // 初期エージェント
        public virtual string InitialAgent { get; } = "初期エージェント";

        // エージェントタイプ
        public virtual string AgentType { get; } = "エージェントタイプ";

        // userproxy
        public virtual string UserProxy { get; } = "userproxy";

        // assistant
        public virtual string Assistant { get; } = "assistant";

        // HumanInputMode
        public virtual string HumanInputMode { get; } = "HumanInputMode";

        // NEVER
        public virtual string Never { get; } = "NEVER";

        // ALWAYS
        public virtual string Always { get; } = "ALWAYS";

        // TERMINATE
        public virtual string Terminate { get; } = "TERMINATE";

        // TERMINATEMsg
        public virtual string TerminateMsg { get; } = "TerminateMsg";

        // CodeExecution
        public virtual string CodeExecution { get; } = "CodeExecution";

        // LLM
        public virtual string LLM { get; } = "LLM";

        // AddVectorDB
        public virtual string AddVectorDB { get; } = "ベクトルDB追加";
        #endregion

        // 有効
        public virtual string Enabled { get; } = "有効";

        #region folder
        // エクスポート対象
        public virtual string ExportTarget { get; } = "エクスポート対象";

        // インポート対象
        public virtual string ImportTarget { get; } = "インポート対象";

        // 保存用ベクトルDB
        public virtual string SaveVectorDB { get; } = "保存用ベクトルDB";

        // 参照用ベクトルDB
        public virtual string ReferenceVectorDB { get; } = "参照用ベクトルDB";

        #endregion

        #region RAG
        // コミット日時
        public virtual string CommitDateTime { get; } = "コミット日時";

        // コミットハッシュ
        public virtual string CommitHash { get; } = "コミットハッシュ";

        // メッセージ
        public virtual string Message { get; } = "メッセージ";

        // UpdateRAGIndexWindow
        public virtual string UpdateRAGIndexWindow { get; } = "RAG用ベクトルDB更新";

        // GitRepositoryURL
        public virtual string GitRepositoryURL { get; } = "GitリポジトリURL";

        // UpdateIndex
        public virtual string UpdateIndex { get; } = "インデックス更新";

        // 最初のコミットから最新のコミットまでの全ファイルをインデックス化
        public virtual string IndexAllFilesFromFirstCommitToLatestCommit { get; } = "最初のコミットから最新のコミットまでの全ファイルをインデックス化";

        // 最後にインデックス化したコミットから最新のコミットまでのファイルをインデックス化
        public virtual string IndexFilesFromLastIndexedCommitToLatestCommit { get; } = "最後にインデックス化したコミットから最新のコミットまでのファイルをインデックス化";

        // 最後にインデックス化したコミットのハッシュ値
        public virtual string LastIndexedCommitHash { get; } = "最後にインデックス化したコミットのハッシュ値";

        // インデックス化するコミットの範囲を指定
        public virtual string SpecifyTheRangeOfCommitsToIndex { get; } = "インデックス化するコミットの範囲を指定";

        // インデックス化を開始するコミット
        public virtual string StartCommitForIndexing { get; } = "インデックス化を開始するコミット";

        // インデックス化処理の状況サマリー
        public virtual string IndexingSummary { get; } = "インデックス化処理の状況サマリー";

        // インデックス化処理の状況詳細
        public virtual string IndexingDetail { get; } = "インデックス化処理の状況詳細";

        #endregion

        #region VectorDB

        // ベクトルDB一覧
        public virtual string VectorDBList { get; } = "ベクトルDB一覧";

        // マルチベクターリトリーバーの最終的な検索結果
        public virtual string MultiVectorRetrieverFinalSearchResult { get; } = "マルチベクターリトリーバーの最終的な検索結果";

        #endregion

        // プロンプトテンプレートが存在しません。
        public virtual string PromptTemplateNotFound { get; } = "プロンプトテンプレートが存在しません。";


    }
}
