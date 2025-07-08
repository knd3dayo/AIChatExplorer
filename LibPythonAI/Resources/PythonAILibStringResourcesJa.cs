using LibPythonAI.Common;

namespace LibPythonAI.Resources {
    public class PythonAILibStringResourcesJa: PythonAILibStringResources {

        #region LibUIPythonAIからコピー
        // クリップボード
        public override string Application { get; } = "アプリケーション";

        // チャット履歴
        public override string ChatHistory { get; } = "チャット履歴";

        // 検索フォルダ
        public override string SearchFolder { get; } = "検索フォルダ";

        // 検索フォルダ 英語名
        public override string SearchFolderEnglish { get; } = "SearchFolder";

        // イメージチャット
        public override string ImageChat { get; } = "イメージチャット";

        // イメージチャット 英語名
        public override string ImageChatEnglish { get; } = "ImageChat";

        // ローカルファイルシステム
        public override string FileSystem { get; } = "ファイルシステム";

        // ローカルファイルシステム 英語名
        public override string FileSystemEnglish { get; } = "FileSystem";

        // Shortcut
        public override string Shortcut { get; } = "ショートカット";

        // Shortcut 英語名
        public override string ShortcutEnglish { get; } = "Shortcut";

        // Outlook
        public override string Outlook { get; } = "Outlook";

        // Outlook 英語名
        public override string OutlookEnglish { get; } = "Outlook";

        // Edgeのブラウズ履歴
        public override string EdgeBrowseHistory { get; } = "Edgeの閲覧履歴";

        // Edgeのブラウズ履歴 英語名
        public override string EdgeBrowseHistoryEnglish { get; } = "EdgeBrowseHistory";

        // RecentFiles
        public override string RecentFiles { get; } = "最近のファイル";

        // ClipboardHistory
        public override string ClipboardHistory { get; } = "クリップボード履歴";

        // ScreenShotHistory
        public override string ScreenShotHistory { get; } = "スクリーンショット履歴";

        // IntegratedMonitorHistory
        public override string IntegratedMonitorHistory { get; } = "統合モニター履歴";

        // 自動処理が設定されています
        public override string AutoProcessingIsSet { get; } = "自動処理が設定されています";
        // 検索条件
        public override string SearchCondition { get; } = "検索条件";
        #endregion

        // PythonAILibManagerIsNotInitialized
        public override string PythonAILibManagerIsNotInitialized { get; } = "PythonAILibManagerが初期化されていません";

        // PythonAILibManagerInitializationFailed
        public override string PythonAILibManagerInitializationFailed { get; } = "PythonAILibManagerの初期化に失敗しました。Pythonのパスを確認してください。";

        // 自動タイトル設定処理を実行します
        public override string AutoSetTitle { get; } = "自動タイトル設定処理を実行します";

        // 自動タグ設定処理を実行します
        public override string AutoSetTag { get; } = "自動タグ設定処理を実行します";


        // --- PythonNetFunctions.cs ---
        
        // Python venv環境が見つかりません。Python venvのパスを確認してください:
        public override string PythonVenvPathNotFound { get; } = "Python venv環境が見つかりません。設定画面からPython venvのパスを設定してください。";

        // Python venv環境が見つかりません。Python venvを作成してください:
        public override string PythonVenvNotCreated { get; } = "Python venv環境が見つかりません。Python venvを作成してください。";

        // Python venv環境が見つかりません。Python venvを作成しますか？
        public override string ConfirmPythonVenvCreate { get; } = "Python venv環境が見つかりません。Python venvを作成しますか？";

        // Pythonを手動でインストールしてください
        public override string PythonVenvMaualCreateMessage(IPythonAILibConfigParams configParams) {
            string message = $"""
            以下のコマンドでPython Venv環境を作成してください。
            cd {configParams.GetAppDataPath()}
            curl -L https://github.com/knd3dayo/ai_chat_lib/archive/refs/heads/main.zip -o ai_chat_lib.zip
            call powershell -command "Expand-Archive  ai_chat_lib.zip"
            python -m venv {configParams.GetPathToVirtualEnv()}
            call {configParams.GetPathToVirtualEnv()}\Scripts\activate
            pip install ai_chat_lib\ai_chat_lib-main
            """;
            return message;
        }

        // PythonVenvCreationFailed
        public override string PythonVenvCreationFailed { get; } = "Python venvの作成に失敗しました。Pythonのパスを確認してください。";
        // PythonVenvCreationSuccess
        public override string PythonVenvCreationSuccess { get; } = "Python venvの作成に成功しました。";

        // PythonLibsInstallationFailed
        public override string PythonLibsInstallationFailed { get; } = "Pythonライブラリのインストールに失敗しました。";

        // PythonLibsInstallationSuccess
        public override string PythonLibsInstallationSuccess { get; } = "Pythonライブラリのインストールに成功しました。";


        // OpenAIKeyNotSet
        public override string OpenAIKeyNotSet { get; } = "OpenAIのAPIキーが設定されていません。設定画面からOpenAI_API_KEYを設定してください。";

        // Python関連の初期化処理が行われていません
        public override string PythonNotInitialized { get; } = "Python関連の初期化処理が行われていません";

        // PythonNotFound
        public override string PythonNotFound { get; } = "Pythonが見つかりません。Pythonのパスを確認してください:";

        // UvNotFound
        public override string UvNotFound { get; } = "uvパッケージが見つかりません。pip install uvでインストールしてください。";

        // "OpenAIの応答がありません"
        public override string OpenAIResponseEmpty { get; } = "OpenAIの応答がありません";

        // ファイルが存在しません
        public override string FileNotFound { get; } = "ファイルが存在しません";

        // --- ChatItem.cs ---
        // Reference Information
        public override string ReferenceInformation { get; } = "参照情報";
        // <参照元ドキュメントルート>
        public override string ReferenceDocument { get; } = "<参考ドキュメント>";

        // --- PythonNetFunctions.cs ---
        // リクエスト情報
        public override string RequestInfo { get; } = "リクエスト情報";

        // レスポンス
        public override string Response { get; } = "レスポンス";

        // OpenAI実行
        public override string OpenAIExecute { get; } = "OpenAI実行";

        // チャット履歴(英語)
        public override string ChatHistoryEnglish { get; } = "ChatHistory";

        // UpdateContentFoldersExecute
        public override string UpdateContentFoldersExecute { get; } = "ベクトル検索用フォルダ情報を更新します";

        // DeleteContentFoldersExecute
        public override string DeleteContentFoldersExecute { get; } = "ベクトル検索用フォルダ情報を削除します";


        // GetAutoProcessRulesExecute
        public override string GetAutoProcessRulesExecute { get; } = "自動処理ルールを取得します";
        // UpdateAutoProcessRulesExecute
        public override string UpdateAutoProcessRulesExecute { get; } = "自動処理ルールを更新します";
        // DeleteAutoProcessRulesExecute
        public override string DeleteAutoProcessRulesExecute { get; } = "自動処理ルールを削除します";

        // GetAutoProcessItemsExecute
        public override string GetAutoProcessItemsExecute { get; } = "自動処理アイテムを取得します";

        // UpdateAutoProcessItemsExecute
        public override string UpdateAutoProcessItemsExecute { get; } = "自動処理アイテムを更新します";

        // DeleteAutoProcessItemsExecute
        public override string DeleteAutoProcessItemsExecute { get; } = "自動処理アイテムを削除します";

        // GetSearchRulesExecute
        public override string GetSearchRulesExecute { get; } = "検索ルールを取得します";

        // UpdateSearchRulesExecute
        public override string UpdateSearchRulesExecute { get; } = "検索ルールを更新します";

        // DeleteSearchRulesExecute
        public override string DeleteSearchRulesExecute { get; } = "検索ルールを削除します";



        // GetPromptItemsExecute
        public override string GetPromptItemsExecute { get; } = "プロンプトアイテムを取得します";

        // UpdatePromptItemsExecute
        public override string UpdatePromptItemsExecute { get; } = "プロンプトアイテムを更新します";
        // DeletePromptItemsExecute
        public override string DeletePromptItemsExecute { get; } = "プロンプトアイテムを削除します";


        // GetTagItemsExecute
        public override string GetTagItemsExecute { get; } = "タグアイテムを取得します";

        // UpdateTagItemsExecute
        public override string UpdateTagItemsExecute { get; } = "タグアイテムを更新します";
        // DeleteTagItemsExecute
        public override string DeleteTagItemsExecute { get; } = "タグアイテムを削除します";

        // GetTokenCountExecute
        public override string GetTokenCountExecute { get; } = "GetTokenCount実行";

        // UpdateVectorDBIndex実行
        public override string UpdateEmbeddingExecute { get; } = "Embeddingを更新します";

        // DeleteEmbeddingsByFolderExecute
        public override string DeleteEmbeddingsByFolderExecute { get; } = "フォルダ内のEmbeddingを削除します";

        // Embeddingを削除します
        public override string DeleteEmbeddingExecute { get; } = "Embeddingを削除します";

        // ベクトルDBアイテムを更新します
        public override string UpdateVectorDBItemExecute { get; } = "ベクトルDBアイテムを更新します";
        // ベクトルDBアイテムを削除します
        public override string DeleteVectorDBItemExecute { get; } = "ベクトルDBアイテムを削除します";

        // GetVectorDBItemsExecute
        public override string GetVectorDBItemsExecute { get; } = "ベクトルDBアイテムを取得します";

        // GetVectorDBItemByIdExecute
        public override string GetVectorDBItemByIdExecute { get; } = "ベクトルDBアイテムをIDで取得します";

        // GetVectorDBItemByNameExecute
        public override string GetVectorDBItemByNameExecute { get; } = "ベクトルDBアイテムを名前で取得します";

        // DeleteAsync
        // ベクトルDBのコレクション削除を実行
        public override string DeleteVectorDBCollectionExecute { get; } = "ベクトルDBのコレクション削除を実行";
        // ベクトルDBのコレクション更新を実行
        public override string UpdateVectorDBCollectionExecute { get; } = "ベクトルDBのコレクション更新を実行";
        // GetVectorDBDescription
        public override string GetVectorDBDescription { get; } = "ベクトルDBの説明を取得";
        // UpdateVectorDBDescription
        public override string UpdateVectorDBDescription { get; } = "ベクトルDBの説明を更新";

        // モードが不正です
        public override string InvalidMode { get; } = "モードが不正です";

        // UpdateVectorDBIndex実行
        public override string UpdateVectorDBIndex { get; } = "UpdateVectorDBIndex実行";

        // LangChain実行
        public override string LangChainExecute { get; } = "LangChain実行";
        // プロンプト
        public override string Prompt { get; } = "プロンプト";


        // VectorSearch実行
        public override string VectorSearchExecute { get; } = "VectorSearch実行";

        // ベクトルDB
        public override string VectorDBItems { get; } = "ベクトルDBアイテム";

        // ベクトル検索リクエスト
        public override string VectorSearchRequest { get; } = "ベクトル検索リクエスト";

        // UpdateAutoGenAgentExecute
        public override string UpdateAutoGenAgentExecute { get; } = "UpdateAutoGenAgent実行";

        // UpdateAutogenLLMConfigExecute
        public override string UpdateAutogenLLMConfigExecute { get; } = "UpdateAutogenLLMConfig実行";

        // DeleteAutoGenAgentExecute
        public override string DeleteAutoGenAgentExecute { get; } = "DeleteAutoGenAgent実行";

        // DeleteAutogenLLMConfigExecute
        public override string DeleteAutogenLLMConfigExecute { get; } = "DeleteAutogenLLMConfig実行";

        // DeleteAutoGenAgentExecute
        public override string DeleteAutoGenGroupChatExecute { get; } = "DeleteAutoGenGroupChat実行";

        // UpdateAutoGenGroupChatExecute
        public override string UpdateAutoGenGroupChatExecute { get; } = "UpdateAutoGenGroupChat実行";

        // UpdateAutoGenToolExecute
        public override string UpdateAutoGenToolExecute { get; } = "UpdateAutoGenTool実行";

        // DeleteAutoGenToolExecute
        public override string DeleteAutoGenToolExecute { get; } = "DeleteAutoGenTool実行";

        // Excelへのエクスポートを実行します
        public override string ExportToExcelExecute { get; } = "Excelへのエクスポートを実行します";
        // Excelへのエクスポートが失敗しました
        public override string ExportToExcelFailed { get; } = "Excelへのエクスポートが失敗しました";
        // Excelへのエクスポートが成功しました
        public override string ExportToExcelSuccess { get; } = "Excelへのエクスポートが成功しました";

        // ファイルパス
        public override string FilePath { get; } = "ファイルパス";
        // データ
        public override string Data { get; } = "データ";

        // テキストを抽出しました
        public override string TextExtracted { get; } = "テキストを抽出しました";

        // 更新日
        public override string UpdateDate { get; } = "更新日";

        // ベクトル化日時
        public override string VectorizedDate { get; } = "ベクトル化日時";
        // タイトル
        public override string Title { get; } = "タイトル";

        // ソースタイトル
        public override string SourceTitle { get; } = "ソースタイトル";

        // Path
        public override string SourcePath { get; } = "ソースパス";

        // ピン留め
        public override string Pin { get; } = "ピン留め";

        // 文章の信頼度
        public override string DocumentReliability { get; } = "文章の信頼度";

        // 文章カテゴリ概要
        public override string DocumentCategorySummary { get; } = "文章カテゴリ";

        // 種別
        public override string Type { get; } = "種別";

        // 作成日時
        public override string CreationDateTime { get; } = "作成日時";

        // ソースアプリ名
        public override string SourceAppName { get; } = "ソースアプリ名";

        // ピン留めしてます
        public override string Pinned { get; } = "ピン留めしてます";
        // タグ
        public override string Tag { get; } = "タグ";

        // 背景情報
        public override string BackgroundInformation { get; } = "背景情報";

        // --- ScreenShotCheckCondition.cs ---

        public override string CheckTypeEqual { get; } = "等しい";
        public override string CheckTypeNotEqual { get; } = "等しくない";
        public override string CheckTypeInclude { get; } = "含む";
        public override string CheckTypeNotInclude { get; } = "含まない";
        public override string CheckTypeStartWith { get; } = "開始している";
        public override string CheckTypeNotStartWith { get; } = "開始していない";
        public override string CheckTypeEndWith { get; } = "終わっている";
        public override string CheckTypeNotEndWith { get; } = "終わっていない";
        public override string CheckTypeEmpty { get; } = "空である";
        public override string CheckTypeCheckBox { get; } = "チェックボックス";


        //  $"{SettingItem}の値は{SettingValue}である";
        public override string SettingValueIs(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}である";
        }

        // $"{SettingItem}の値は{SettingValue}でない";
        public override string SettingValueIsNot(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}でない";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれている";
        public override string SettingValueContains(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれている";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれていない";
        public override string SettingValueNotContain(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれていない";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっている";
        public override string SettingValueStartsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっている";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっていない";
        public override string SettingValueNotStartWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっていない";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっている";
        public override string SettingValueEndsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっている";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっていない";
        public override string SettingValueNotEndWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっていない";
        }

        //  $"{SettingItem}の値が空である";
        public override string SettingValueIsEmpty(string SettingItem) {
            return $"{SettingItem}の値が空である";
        }
        // $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        public override string SettingValueIsChecked(string SettingItem, string SettingValue) {
            return $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        }

        // リモートリポジトリが設定されていません
        public override string NoRemoteRepositorySet { get; } = "リモートリポジトリが設定されていません";

        // 作業ディレクトリが指定されていません
        public override string NoWorkingDirectorySpecified { get; } = "作業ディレクトリが指定されていません";

        // "指定されたディレクトリが存在しません"
        public override string SpecifiedDirectoryDoesNotExist { get; } = "指定されたディレクトリが存在しません";

        // "指定されたディレクトリはGitリポジトリではありません"
        public override string SpecifiedDirectoryIsNotAGitRepository { get; } = "指定されたディレクトリはGitリポジトリではありません";

        // "ベクトルDBが設定されていません"
        public override string NoVectorDBSet { get; } = "ベクトルDBが設定されていません";
        // サポートされていないファイル形式です
        public override string UnsupportedFileType { get; } = "サポートされていないファイル形式です";

        // "Embeddingを保存します
        public override string SaveEmbedding { get; } = "Embeddingを保存します";
        // Embeddingを保存しました
        public override string SavedEmbedding { get; } = "Embeddingを保存しました";
        // Embeddingを削除します
        public override string DeleteEmbedding { get; } = "Embeddingを削除します";

        // Embeddingを削除しました
        public override string DeletedEmbedding { get; } = "Embeddingを削除しました";


        // 画像から抽出したテキストのEmbeddingを保存します
        public override string SaveTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存します";
        // 画像から抽出したテキストのEmbeddingを保存しました
        public override string SavedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存しました";

        // 画像から抽出したテキストのEmbeddingを削除します
        public override string DeleteTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除します";
        // 画像から抽出したテキストのEmbeddingを削除しました
        public override string DeletedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除しました";

        //  "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        public override string GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions { get; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";



        // InputContentNotFound
        public override string InputContentNotFound { get; } = "入力内容が見つかりません";


        // "アイテムを追加しました"
        public override string AddedItems { get; } = "アイテムを追加しました";

        // --- SystemAutoProcessItem.cs ---
        // 無視
        public override string Ignore { get; } = "無視";
        // "何もしません"
        public override string DoNothing { get; } = "何もしません";

        // フォルダにコピー
        public override string CopyToFolder { get; } = "フォルダにコピー";
        // クリップボードの内容を指定されたフォルダにコピーします
        public override string CopyClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダにコピーします";
        // テキストを抽出
        public override string ExtractText { get; } = "テキストを抽出";

        // フォルダに移動"
        public override string MoveToFolder { get; } = "フォルダに移動";
        // "クリップボードの内容を指定されたフォルダに移動します"
        public override string MoveClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダに移動します";

        // "クリップボードのテキストを抽出します"
        public override string ExtractClipboardText { get; } = "クリップボードのテキストを抽出します";

        // "データマスキング",
        public override string DataMasking { get; } = "データマスキング";
        // "クリップボードのテキストをマスキングします"
        public override string MaskClipboardText { get; } = "クリップボードのテキストをマスキングします";

        // フォルダが選択されていません
        public override string NoFolderSelected { get; } = "フォルダが選択されていません";

        // フォルダにコピーします
        public override string CopyToFolderDescription { get; } = "フォルダにコピーします";

        // ディレクトリは新規ファイルとして開けません
        public override string CannotOpenDirectoryAsNewFile { get; } = "ディレクトリは新規ファイルとして開けません";

        // --- AutoProcessRule.cs ---
        // RuleName + "は無効です"
        public override string RuleNameIsInvalid(string RuleName) {
            return RuleName + "は無効です";
        }
        // 条件にマッチしませんでした
        public override string NoMatch { get; } = "条件にマッチしませんでした";

        // アクションが設定されていません
        public override string NoActionSet { get; } = "アクションが設定されていません";

        // 条件
        public override string Condition { get; } = "条件";

        // アクション
        public override string Action { get; } = "アクション";

        // アクション:なし
        public override string ActionNone { get; } = "アクション:なし";

        // フォルダ:なし
        public override string FolderNone { get; } = "フォルダ:なし";

        // 無限ループを検出しました
        public override string DetectedAnInfiniteLoop { get; } = "無限ループを検出しました";

        // "Descriptionが" + condition.Keyword + "を含む
        public override string DescriptionContains(string Keyword) {
            return "Descriptionが" + Keyword + "を含む";
        }
        // "Contentが" + condition.Keyword + "を含む 
        public override string ContentContains(string Keyword) {
            return "Contentが" + Keyword + "を含む";
        }
        // "SourceApplicationNameが" + condition.Keyword + "を含む \n";
        public override string SourceApplicationNameContains(string Keyword) {
            return "SourceApplicationNameが" + Keyword + "を含む \n";
        }
        // "SourceApplicationTitleが" + condition.Keyword + "を含む
        public override string SourceApplicationTitleContains(string Keyword) {
            return "SourceApplicationTitleが" + Keyword + "を含む";
        }
        // "SourceApplicationPathが" + condition.Keyword + "を含む
        public override string SourceApplicationPathContains(string Keyword) {
            return "SourceApplicationPathが" + Keyword + "を含む";
        }
        // 自動イメージテキスト抽出処理を実行します
        public override string AutoExtractImageText { get; } = "自動イメージテキスト抽出処理を実行します";

        // File
        public override string File { get; } = "ファイル";
        // Folder
        public override string Folder { get; } = "フォルダ";


        // 自動背景情報追加処理を実行します
        public override string AutoSetBackgroundInfo { get; } = "自動背景情報追加処理を実行します";
        // 背景情報追加処理が失敗しました
        public override string AddBackgroundInfoFailed { get; } = "背景情報追加処理が失敗しました";

        // 自動サマリー作成処理を実行します
        public override string AutoCreateSummary { get; } = "自動サマリー作成処理を実行します";
        // サマリー作成処理が失敗しました

        // 自動文書信頼度チェック処理を実行します
        public override string AutoCheckDocumentReliability { get; } = "自動文書信頼度チェック処理を実行します";
        // 文書信頼度チェック処理が失敗しました
        public override string CheckDocumentReliabilityFailed { get; } = "文書信頼度チェック処理が失敗しました";

        public override string CreateSummaryFailed { get; } = "サマリー作成処理が失敗しました";

        // 自動課題リスト作成処理を実行します
        public override string AutoCreateTaskList { get; } = "自動課題リスト作成処理を実行します";
        // 課題リスト作成処理が失敗しました
        public override string CreateTaskListFailed { get; } = "課題リスト作成処理が失敗しました";


        // 自動処理を適用します
        public override string ApplyAutoProcessing { get; } = "自動処理を適用します";

        // 自動処理でアイテムが削除されました
        public override string ItemsDeletedByAutoProcessing { get; } = "自動処理でアイテムが削除されました";

        // JSON文字列をパースできませんでした
        public override string FailedToParseJSONString { get; } = "JSON文字列をパースできませんでした";

        // 拡張プロパティ
        // FileSystemFolderPathDisplayName
        public override string FileSystemFolderPathDisplayName { get; } = "ファイルシステムフォルダパス";

        // 選択中のアイテムがない"
        public override string NoItemSelected { get; } = "選択中のアイテムがない";

        // テキスト抽出処理実行中
        public override string TextExtractionInProgress { get; } = "テキスト抽出処理実行中";
        // ファイル以外のコンテンツはテキストを抽出できません
        public override string CannotExtractTextForNonFileContent { get; } = "ファイル以外のコンテンツはテキストを抽出できません";

        // OpenFolderInExplorer

        // ファイル以外のコンテンツはフォルダを開けません
        public override string CannotOpenFolderForNonFileContent { get; } = "ファイル以外のコンテンツはフォルダを開けません";

        // フォルダを開きます
        public override string ExecuteOpenFolder { get; } = "フォルダを開きます";

        // フォルダを開きました
        public override string ExecuteOpenFolderSuccess { get; } = "フォルダを開きました";

        // プロンプトテンプレート[promptName]を実行します.
        public override string PromptTemplateExecute(string promptName) => $"プロンプトテンプレート[{promptName}]を実行します.";

        // PromptItemsNotLoaded
        public override string PromptItemsNotLoaded { get; } = "プロンプトアイテムが読み込まれていません";

        // プロンプトテンプレート[promptName]を実行中
        public override string PromptTemplateInProgress(string promptName) => $"プロンプトテンプレート[{promptName}]を実行中";

        // "プロンプトテンプレート[promptName]を実行しました."
        public override string PromptTemplateExecuted(string promptName) => $"プロンプトテンプレート[{promptName}]を実行しました.";

        // "ベクトルを生成しました"
        public override string GenerateVectorCompleted { get; } = "ベクトルを生成しました";

        // プロパティが設定されていません
        public override string PropertyNotSet(string propertyName) {
            return $"{propertyName}が設定されていません";
        }
        // VectorDBNotFound
        public override string VectorDBNotFound(string name) {
            return $"ベクトルDB[{name}]が見つかりません";
        }

        // Properties
        public override string Properties { get; } = "プロパティ";
        // Text
        public override string Text { get; } = "テキスト";

        // Image
        public override string Image { get; } = "画像";


        // 日次トークン数
        public override string DailyTokenCount { get; } = "日次トークン数";
        // 総トークン数
        public override string TotalTokenFormat(long tokens) {
            return $"総トークン数: {tokens} トークン";
        }
        // トークン数
        public override string TokenCount { get; } = "トークン数";

        public override string DailyTokenFormat(string date, long totalTokens) {
            return $"{date}のトークン数: {totalTokens} トークン";
        }
    }
}
