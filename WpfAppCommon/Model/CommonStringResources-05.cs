namespace WpfAppCommon.Model {
    public partial class CommonStringResources {

        // --- SettingUserControlViewModel
        // Pythonの設定チェック
        public virtual string PythonSettingCheck { get; } = "Pythonの設定チェック";

        // PythonDLLのパスが設定されていません
        public virtual string PythonDLLPathNotSet { get; } = "PythonDLLのパスが設定されていません";

        // PythonDLLのパスが設定されています
        public virtual string PythonDLLPathSet { get; } = "PythonDLLのパスが設定されています";

        // PythonDLLのファイルが存在しません
        public virtual string PythonDLLFileDoesNotExist { get; } = "PythonDLLのファイルが存在しません";

        // PythonDLLのファイルが存在します
        public virtual string PythonDLLFileExists { get; } = "PythonDLLのファイルが存在します";

        // Pythonスクリプトをテスト実行
        public virtual string TestRunPythonScript { get; } = "Pythonスクリプトをテスト実行";

        // OpenAIの設定チェック
        public virtual string OpenAISettingCheck { get; } = "OpenAIの設定チェック";

        // OpenAIのAPIキーが設定されていません
        public virtual string OpenAIKeyNotSet { get; } = "OpenAIのAPIキーが設定されていません";
        // OpenAIのAPIキーが設定されています
        public virtual string OpenAIKeySet { get; } = "OpenAIのAPIキーが設定されています";

        // OpenAIのCompletionModelが設定されていません
        public virtual string OpenAICompletionModelNotSet { get; } = "OpenAIのCompletionModelが設定されていません";

        // OpenAIのCompletionModelが設定されています
        public virtual string OpenAICompletionModelSet { get; } = "OpenAIのCompletionModelが設定されています";

        // OpenAIのEmbeddingModelが設定されていません
        public virtual string OpenAIEmbeddingModelNotSet { get; } = "OpenAIのEmbeddingModelが設定されていません";

        // OpenAIのEmbeddingModelが設定されています
        public virtual string OpenAIEmbeddingModelSet { get; } = "OpenAIのEmbeddingModelが設定されています";

        // Azure OpenAIの設定チェック
        public virtual string AzureOpenAISettingCheck { get; } = "Azure OpenAIの設定チェック";

        // Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック
        public virtual string AzureOpenAIEndpointNotSet { get; } = "Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック";

        // Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください
        public virtual string SetAzureOpenAIEndpointOrBaseURL { get; } = "Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください";

        // Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません
        public virtual string CannotSetBothAzureOpenAIEndpointAndBaseURL { get; } = "Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません";

        // OpenAIのテスト実行
        public virtual string TestRunOpenAI { get; } = "OpenAIのテスト実行";

        // Pythonの実行に失敗しました
        public virtual string FailedToRunPython { get; } = "Pythonの実行に失敗しました";

        // Pythonの実行が可能です
        public virtual string PythonRunIsPossible { get; } = "Pythonの実行が可能です";

        // OpenAIの実行に失敗しました
        public virtual string FailedToRunOpenAI { get; } = "OpenAIの実行に失敗しました";

        // OpenAIの実行が可能です。
        public virtual string OpenAIRunIsPossible { get; } = "OpenAIの実行が可能です。";

        // LangChainの実行に失敗しました
        public virtual string FailedToRunLangChain { get; } = "LangChainの実行に失敗しました";

        // LangChainの実行が可能です。
        public virtual string LangChainRunIsPossible { get; } = "LangChainの実行が可能です。";
        // 実行しますか？
        public virtual string ConfirmRun { get; } = "実行しますか？";

        // 設定チェック中
        public virtual string CheckingSettings { get; } = "設定チェック中";

        // 設定を保存しました。
        public virtual string SettingsSaved { get; } = "設定を保存しました。";

        // "キャンセルしました"
        public virtual string Canceled { get; } = "キャンセルしました";

        // MyStatusBarViewModel
        // ログ
        public virtual string Log { get; } = "ログ";

        // --- AutoProcessRule.cs ---
        // RuleName + "は無効です"
        public virtual string RuleNameIsInvalid(string RuleName) {
            return RuleName + "は無効です";
        }
        // 条件にマッチしませんでした
        public virtual string NoMatch { get; } = "条件にマッチしませんでした";

        // アクションが設定されていません
        public virtual string NoActionSet { get; } = "アクションが設定されていません";

        // 条件
        public virtual string Condition { get; } = "条件";

        // アクション
        public virtual string Action { get; } = "アクション";

        // アクション:なし
        public virtual string ActionNone { get; } = "アクション:なし";

        // フォルダ:なし
        public virtual string FolderNone { get; } = "フォルダ:なし";

        // 無限ループを検出しました
        public virtual string DetectedAnInfiniteLoop { get; } = "無限ループを検出しました";

        // "Descriptionが" + condition.Keyword + "を含む
        public virtual string DescriptionContains(string Keyword) {
            return "Descriptionが" + Keyword + "を含む";
        }
        // "Contentが" + condition.Keyword + "を含む 
        public virtual string ContentContains(string Keyword) {
            return "Contentが" + Keyword + "を含む";
        }
        // "SourceApplicationNameが" + condition.Keyword + "を含む \n";
        public virtual string SourceApplicationNameContains(string Keyword) {
            return "SourceApplicationNameが" + Keyword + "を含む \n";
        }
        // "SourceApplicationTitleが" + condition.Keyword + "を含む
        public virtual string SourceApplicationTitleContains(string Keyword) {
            return "SourceApplicationTitleが" + Keyword + "を含む";
        }
        // "SourceApplicationPathが" + condition.Keyword + "を含む
        public virtual string SourceApplicationPathContains(string Keyword) {
            return "SourceApplicationPathが" + Keyword + "を含む";
        }
        // --- ClipboardAppVectorDBItem
        //  "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        public virtual string GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions { get; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";

        // --- ClipboardFolder.cs ---
        // クリップボード
        public virtual string Clipboard { get; } = "クリップボード";

        // チャット履歴
        public virtual string ChatHistory { get; } = "チャット履歴";

        // 自動処理でアイテムが削除または移動されました
        public virtual string ItemsDeletedOrMovedByAutoProcessing { get; } = "自動処理でアイテムが削除または移動されました";

        // "アイテムを追加しました"
        public virtual string AddedItems { get; } = "アイテムを追加しました";

        // 自動処理を適用します
        public virtual string ApplyAutoProcessing { get; } = "自動処理を適用します";

        // 自動処理でアイテムが削除されました
        public virtual string ItemsDeletedByAutoProcessing { get; } = "自動処理でアイテムが削除されました";

        // JSON文字列をパースできませんでした
        public virtual string FailedToParseJSONString { get; } = "JSON文字列をパースできませんでした";

        // ClipboardItem
        // Text以外のアイテムへのマージはできません
        public virtual string CannotMergeToNonTextItems { get; } = "Text以外のアイテムへのマージはできません";

        // "Text以外のアイテムが含まれているアイテムはマージできません"
        public virtual string CannotMergeItemsContainingNonTextItems { get; } = "Text以外のアイテムが含まれているアイテムはマージできません";

        // 作成日時
        public virtual string CreationDateTime { get; } = "作成日時";

        // ソースアプリ名
        public virtual string SourceAppName { get; } = "ソースアプリ名";

        // ピン留めしてます
        public virtual string Pinned { get; } = "ピン留めしてます";

        // フォルダを取得できません
        public virtual string CannotGetFolder { get; } = "フォルダを取得できません";

        // OS上のファイルに保存します
        public virtual string SaveToFileOnOS { get; } = "OS上のファイルに保存します";

        // Gitコミットしました
        public virtual string CommittedToGit { get; } = "Gitコミットしました";

        // リポジトリが見つかりませんでした
        public virtual string RepositoryNotFound { get; } = "リポジトリが見つかりませんでした";

        // コミットが空です
        public virtual string CommitIsEmpty { get; } = "コミットが空です";

        // OS上のファイルに保存しました
        public virtual string SavedToFileOnOS { get; } = "OS上のファイルに保存しました";

        // 背景情報
        public virtual string BackgroundInformation { get; } = "背景情報";

        // サマリー
        public virtual string Summary { get; } = "サマリー";

        // "Embeddingを保存します
        public virtual string SaveEmbedding { get; } = "Embeddingを保存します";


        // Embeddingを保存しました
        public virtual string SavedEmbedding { get; } = "Embeddingを保存しました";

        // Embeddingを削除します
        public virtual string DeleteEmbedding { get; } = "Embeddingを削除します";

        // Embeddingを削除しました
        public virtual string DeletedEmbedding { get; } = "Embeddingを削除しました";

        // 画像から抽出したテキストのEmbeddingを保存します
        public virtual string SaveTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存します";
        // 画像から抽出したテキストのEmbeddingを保存しました
        public virtual string SavedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存しました";

        // 画像から抽出したテキストのEmbeddingを削除します
        public virtual string DeleteTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除します";
        // 画像から抽出したテキストのEmbeddingを削除しました
        public virtual string DeletedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除しました";


        // "OS上のファイルを削除します"
        public virtual string DeleteFileOnOS { get; } = "OS上のファイルを削除します";
        // OS上のファイルを削除しました
        public virtual string DeletedFileOnOS { get; } = "OS上のファイルを削除しました";

        // サポートされていないファイル形式です
        public virtual string UnsupportedFileType { get; } = "サポートされていないファイル形式です";

        // テキストを抽出しました
        public virtual string ExtractedText { get; } = "テキストを抽出しました";

        // --- ScreenShotCheckCondition.cs ---
        //  $"{SettingItem}の値は{SettingValue}である";
        public virtual string SettingValueIs(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}である";
        }

        // $"{SettingItem}の値は{SettingValue}でない";
        public virtual string SettingValueIsNot(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}でない";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれている";
        public virtual string SettingValueContains(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれている";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれていない";
        public virtual string SettingValueNotContain(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれていない";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっている";
        public virtual string SettingValueStartsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっている";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっていない";
        public virtual string SettingValueNotStartWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっていない";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっている";
        public virtual string SettingValueEndsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっている";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっていない";
        public virtual string SettingValueNotEndWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっていない";
        }

        //  $"{SettingItem}の値が空である";
        public virtual string SettingValueIsEmpty(string SettingItem) {
            return $"{SettingItem}の値が空である";
        }
        // $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        public virtual string SettingValueIsChecked(string SettingItem, string SettingValue) {
            return $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        }

        // --- SystemAutoProcessItem.cs ---
        // 無視
        public virtual string Ignore { get; } = "無視";
        // "何もしません"
        public virtual string DoNothing { get; } = "何もしません";

        // フォルダにコピー
        public virtual string CopyToFolder { get; } = "フォルダにコピー";
        // クリップボードの内容を指定されたフォルダにコピーします
        public virtual string CopyClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダにコピーします";

        // フォルダに移動"
        public virtual string MoveToFolder { get; } = "フォルダに移動";
        // "クリップボードの内容を指定されたフォルダに移動します"
        public virtual string MoveClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダに移動します";

        // "クリップボードのテキストを抽出します"
        public virtual string ExtractClipboardText { get; } = "クリップボードのテキストを抽出します";

        // "データマスキング",
        public virtual string DataMasking { get; } = "データマスキング";
        // "クリップボードのテキストをマスキングします"
        public virtual string MaskClipboardText { get; } = "クリップボードのテキストをマスキングします";

        //  "フォルダ内のアイテムをマージ", 
        public virtual string MergeItemsInFolder { get; } = "フォルダ内のアイテムをマージ";

        // "フォルダ内のアイテムをマージします"
        public virtual string MergeItemsInFolderDescription { get; } = "フォルダ内のアイテムをマージします";

        // "同じSourceApplicationTitleを持つアイテムをマージ",
        public virtual string MergeItemsWithTheSameSourceApplicationTitle { get; } = "同じSourceApplicationTitleを持つアイテムをマージ";
        // "同じSourceApplicationTitleを持つアイテムをマージします"
        public virtual string MergeItemsWithTheSameSourceApplicationTitleDescription { get; } = "同じSourceApplicationTitleを持つアイテムをマージします";

        // フォルダが選択されていません
        public virtual string NoFolderSelected { get; } = "フォルダが選択されていません";

        // フォルダにコピーします
        public virtual string CopyToFolderDescription { get; } = "フォルダにコピーします";

        // ディレクトリは新規ファイルとして開けません
        public virtual string CannotOpenDirectoryAsNewFile { get; } = "ディレクトリは新規ファイルとして開けません";

        // --- TextSelector.cs ---
        // ファイルを実行できませんでした
        public virtual string FailedToRunFile { get; } = "ファイルを実行できませんでした";

        // テキストファイルとして開きます。
        public virtual string OpenAsTextFile { get; } = "テキストファイルとして開きます。";

        // --- EditChatItemWindow ---
        // チャットアイテム
        public virtual string ChatItem { get; } = "チャットアイテム";

        // --- ExportImportWindow ---
        // 以下の項目をエクスポートします
        public virtual string ExportTheFollowingItems { get; } = "以下の項目をエクスポートします";

        // 作業ディレクトリ
        public virtual string WorkingDirectory { get; } = "作業ディレクトリ";
        // リポジトリURL
        public virtual string RepositoryURL { get; } = "リポジトリURL";
        // 最後にインデックス化したコミット
        public virtual string LastIndexedCommit { get; } = "最後にインデックス化したコミット";
        // 新規RAGソース
        public virtual string NewRAGSource { get; } = "新規RAGソース";

        // インポート時に自動処理を実行します
        public virtual string ExecuteAutoProcessingOnImport { get; } = "インポート時に自動処理を実行します";

        // ベクトルDB編集
        public virtual string EditVectorDB { get; } = "ベクトルDB編集";

        // 例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。
        public virtual string ExampleGeneralVectorDB { get; } = "例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";

        // ベクトルDBの場所
        public virtual string VectorDBLocation { get; } = "ベクトルDBの場所";

        // ベクトルDBのタイプ
        public virtual string VectorDBType { get; } = "ベクトルDBのタイプ";

        // ドキュメントのチャンクサイズ
        public virtual string DocumentChunkSize { get; } = "ドキュメントのチャンクサイズ";

        // ベクトル検索結果の上限値
        public virtual string VectorSearchResultLimit { get; } = "ベクトル検索結果の上限値";

        // MultiVectorRetrieverを使用
        public virtual string UseMultiVectorRetriever { get; } = "MultiVectorRetrieverを使用";

        // DocStore用のSQLite3の場所
        public virtual string SQLite3LocationForDocStore { get; } = "DocStore用のSQLite3の場所";

        // 例：sqlite:///C:\Users\Username\sqlite3.db
        public virtual string ExampleSQLite3Location { get; } = "例：sqlite:///C:\\Users\\Username\\sqlite3.db";

        // マルチベクターリトリーバのドキュメントのチャンクサイズ
        public virtual string DocumentChunkSizeForMultiVectorRetriever { get; } = "マルチベクターリトリーバのドキュメントのチャンクサイズ";

    }
}
