namespace LibUIMain.Resource {
    public partial class CommonStringResourcesJa {

        #region QAChatControl

        // 全チャット内容をエクスポート
        public override string ExportAllChatContents { get; } = "全チャット内容をエクスポート";
        // 選択したチャット内容をコピー
        public override string CopySelectedChatContents { get; } = "選択したチャット内容をコピー";
        // 全のチャット内容をコピー
        public override string CopyAllChatContents { get; } = "全のチャット内容をコピー";

        #endregion

        #region AutoGen
        // AutoGen設定一覧
        public override string AutoGenSettingList { get; } = "AutoGen設定一覧";

        // AutoGenツール編集
        public override string EditAutoGenToolWindowTitle { get; } = "AutoGenツール編集";

        // AutoGenエージェント編集
        public override string EditAutoGenAgentWindowTitle { get; } = "AutoGenエージェント編集";

        // AutoGenLLMConfig編集
        public override string EditAutoGenLLMConfigWindowTitle { get; } = "AutoGenLLMConfig編集";

        // Tools
        public override string Tools { get; } = "Tools";

        // ToolsForExecution
        public override string ToolsForExecution { get; } = "ToolsForExecution";

        // SystemMessage
        public override string SystemMessage { get; } = "SystemMessage";

        // グループチャット
        public override string GroupChat { get; } = "グループチャット";
        // 通常チャット
        public override string NormalChat { get; } = "通常チャット";

        // ネストチャット
        public override string NestedChat { get; } = "ネストチャット";

        // エージェント
        public override string Agent { get; } = "エージェント";

        // ソースパス
        public override string SourcePath { get; } = "ソースパス";

        // 初期エージェント
        public override string InitialAgent { get; } = "初期エージェント";

        // エージェントタイプ
        public override string AgentType { get; } = "エージェントタイプ";

        // userproxy
        public override string UserProxy { get; } = "userproxy";

        // assistant
        public override string Assistant { get; } = "assistant";

        // HumanInputMode
        public override string HumanInputMode { get; } = "HumanInputMode";

        // NEVER
        public override string Never { get; } = "NEVER";

        // ALWAYS
        public override string Always { get; } = "ALWAYS";

        // TERMINATE
        public override string Terminate { get; } = "TERMINATE";

        // TERMINATEMsg
        public override string TerminateMsg { get; } = "TerminateMsg";

        // LLMConfig
        public override string LLMConfig { get; } = "LLMConfig";

        // CodeExecution
        public override string CodeExecution { get; } = "CodeExecution";

        // LLM
        public override string LLM { get; } = "LLM";

        // AddVectorDB
        public override string AddVectorDB { get; } = "ベクトルDB追加";

        // ApiType
        public override string ApiType { get; } = "ApiType";

        // Model
        public override string Model { get; } = "Model";

        // BaseURL
        public override string BaseURL { get; } = "BaseURL";

        // MaxRounds
        public override string MaxRounds { get; } = "MaxRounds";


        #endregion


        // 有効
        public override string Enabled { get; } = "有効";

        #region folder
        // エクスポート対象
        public override string ExportTarget { get; } = "エクスポート対象";

        // インポート対象
        public override string ImportTarget { get; } = "インポート対象";

        // 保存用ベクトルDB
        public override string SaveVectorDB { get; } = "保存用ベクトルDB";

        // 参照用ベクトルDB
        public override string ReferenceVectorDB { get; } = "参照用ベクトルDB";

        #endregion

        #region RAG
        // コミット日時
        public override string CommitDateTime { get; } = "コミット日時";

        // コミットハッシュ
        public override string CommitHash { get; } = "コミットハッシュ";

        // メッセージ
        public override string Message { get; } = "メッセージ";

        // UpdateRAGIndexWindow
        public override string UpdateRAGIndexWindow { get; } = "RAG用ベクトルDB更新";

        // GitRepositoryURL
        public override string GitRepositoryURL { get; } = "GitリポジトリURL";

        // UpdateEmbeddingsAsync
        public override string UpdateIndex { get; } = "インデックス更新";

        // 最初のコミットから最新のコミットまでの全ファイルをインデックス化
        public override string IndexAllFilesFromFirstCommitToLatestCommit { get; } = "最初のコミットから最新のコミットまでの全ファイルをインデックス化";

        // 最後にインデックス化したコミットから最新のコミットまでのファイルをインデックス化
        public override string IndexFilesFromLastIndexedCommitToLatestCommit { get; } = "最後にインデックス化したコミットから最新のコミットまでのファイルをインデックス化";

        // 最後にインデックス化したコミットのハッシュ値
        public override string LastIndexedCommitHash { get; } = "最後にインデックス化したコミットのハッシュ値";

        // インデックス化するコミットの範囲を指定
        public override string SpecifyTheRangeOfCommitsToIndex { get; } = "インデックス化するコミットの範囲を指定";

        // インデックス化を開始するコミット
        public override string StartCommitForIndexing { get; } = "インデックス化を開始するコミット";

        // インデックス化処理の状況サマリー
        public override string IndexingSummary { get; } = "インデックス化処理の状況サマリー";

        // インデックス化処理の状況詳細
        public override string IndexingDetail { get; } = "インデックス化処理の状況詳細";

        #endregion

        #region VectorDB

        // ベクトルDB一覧
        public override string VectorDBList { get; } = "ベクトルDB一覧";

        // マルチベクターリトリーバーの最終的な検索結果
        public override string MultiVectorRetrieverFinalSearchResult { get; } = "マルチベクターリトリーバーの最終的な検索結果";

        #endregion

        // プロンプトテンプレートが存在しません。
        public override string PromptTemplateNotFound { get; } = "プロンプトテンプレートが存在しません。";

        // 新規アイテムとしてエクスポート
        public override string ExportAsNewItem { get; } = "新規アイテムとしてエクスポート";

    }
}
