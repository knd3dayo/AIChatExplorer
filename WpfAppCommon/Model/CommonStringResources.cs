namespace WpfAppCommon.Model {
    public class CommonStringResources {

        private static CommonStringResources? _instance;
        public static CommonStringResources Instance {
            get {
                if (_instance == null) {
                    _instance = new CommonStringResources();
                }
                return _instance;
            }
        }

        public string AppName { get; } = "コピペアプリ";
        // ファイル
        public string File { get; } = "ファイル";
        // 作成
        public string Create { get; } = "作成";
        // アイテム作成
        public string CreateItem { get; } = "アイテム作成";
        // 終了
        public string Exit { get; } = "終了";
        // 編集
        public string Edit { get; } = "編集";

        // 背景情報を生成
        public string GenerateBackgroundInfo { get; } = "背景情報を生成";

        // サマリーを生成
        public string GenerateSummary { get; } = "サマリーを生成";

        // ベクトル生成
        public string GenerateVector { get; } = "ベクトル生成";


        //ベクトル検索
        public string VectorSearch { get; } = "ベクトル検索";

        // 開始
        public string Start { get; } = "開始";
        // 停止
        public string Stop { get; } = "停止";
        // 選択
        public string Select { get; } = "選択";
        // ヘルプ
        public string Help { get; } = "ヘルプ";
        // バージョン情報
        public string VersionInfo { get; } = "バージョン情報";

        // 表示
        public string View { get; } = "表示";

        // クリップボード監視開始
        public string StartClipboardWatch { get; } = "クリップボード監視開始";
        // クリップボード監視停止
        public string StopClipboardWatch { get; } = "クリップボード監視停止";
        // Windows通知監視開始
        public string StartNotificationWatch { get; } = "Windows通知監視開始";
        // Windows通知監視停止
        public string StopNotificationWatch { get; } = "Windows通知監視停止";

        // クリップボード監視を開始しました
        public string StartClipboardWatchMessage { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public string StopClipboardWatchMessage { get; } = "クリップボード監視を停止しました";
        // Windows通知監視を開始しました
        public string StartNotificationWatchMessage { get; } = "Windows通知監視を開始しました";
        // Windows通知監視を停止しました
        public string StopNotificationWatchMessage { get; } = "Windows通知監視を停止しました";

        // タグ編集
        public string EditTag { get; } = "タグ編集";
        // 自動処理ルール編集
        public string EditAutoProcessRule { get; } = "自動処理ルール編集";
        // Pythonスクリプト編集
        public string EditPythonScript { get; } = "Pythonスクリプト編集";
        // プロンプトテンプレート編集
        public string EditPromptTemplate { get; } = "プロンプトテンプレート編集";
        // RAGソース編集
        public string EditGitRagSource { get; } = "RAGソース(git)編集";

        // -- 表示メニュー
        // テキストを右端で折り返す
        public string TextWrapping { get; } = "テキストを右端で折り返す";
        // プレビューモード
        public string PreviewMode { get; } = "プレビューを有効にする";
        // コンパクト表示モード
        public string CompactMode { get; } = "コンパクト表示にする";

        // ツール
        public string Tool { get; } = "ツール";
        // OpenAIチャット
        public string OpenAIChat { get; } = "OpenAIチャット";
        // 画像エビデンスチェッカー
        public string ImageChat { get; } = "イメージチャット";

        // 検索
        public string Search { get; } = "検索";
        // 設定
        public string Setting { get; } = "設定";
        // 削除
        public string Delete { get; } = "削除";
        // 追加
        public string Add { get; } = "追加";
        // OK
        public string OK { get; } = "OK";
        // キャンセル
        public string Cancel { get; } = "キャンセル";
        // 閉じる
        public string Close { get; } = "閉じる";
        // エクスポート
        public string Export { get; } = "エクスポート";
        // インポート
        public string Import { get; } = "インポート";
        // 自動処理ルール一覧
        public string ListAutoProcessRule { get; } = "自動処理ルール一覧";
        // Pythonスクリプト一覧
        public string ListPythonScript { get; } = "Pythonスクリプト一覧";

        // タグ一覧
        public string ListTag { get; } = "タグ一覧";

        // 新規タグ
        public string NewTag { get; } = "新規タグ";
        // タグ
        public string Tag { get; } = "タグ";

        // ベクトルDB一覧
        public string ListVectorDB { get; } = "ベクトルDB一覧";
        // ベクトルDB編集
        public string EditVectorDB { get; } = "ベクトルDB編集";

        // --- ToolTip ---
        // 開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。
        public string ToggleClipboardWatchToolTop { get; } = "開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。";

        // 開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.
        public string ToggleNotificationWatchToolTop { get; } = "開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.";

        // 選択中のフォルダにアイテムを作成します。
        public string CreateItemToolTip { get; } = "選択中のフォルダにアイテムを作成します。";

        // アプリケーションを終了します。
        public string ExitToolTip { get; } = "アプリケーションを終了します。";
        // タグを編集します。
        public string EditTagToolTip { get; } = "タグを編集します。";

        // 選択したタグを削除します。
        public string DeleteSelectedTag { get; } = "選択したタグを削除";
        // すべて選択します。
        public string SelectAll { get; } = "すべて選択";
        // すべて選択解除します。
        public string UnselectAll { get; } = "すべて選択解除";

        // --- 画面タイトル ---

        // 自動処理ルール一覧
        public string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // 自動処理ルール編集
        public string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }
        // Pythonスクリプト一覧
        public string ListPythonScriptWindowTitle {
            get {
                return $"{AppName} - {ListPythonScript}";
            }
        }
        // Pythonスクリプト編集
        public string EditPythonScriptWindowTitle {
            get {
                return $"{AppName} - {EditPythonScript}";
            }
        }
        // 設定
        public string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // 設定チェック結果
        public string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - 設定チェック結果";
            }
        }

        // RAGソース(git)編集
        public string EditGitRagSourceWindowTitle {
            get {
                return $"{AppName} - {EditGitRagSource}";
            }
        }
        // RAGソース一覧
        public string ListGitRagSourceWindowTitle {
            get {
                return $"{AppName} - RAGソース(git)一覧";
            }
        }
        // ベクトルDB一覧
        public string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }
        // ベクトルDB編集
        public string EditVectorDBWindowTitle {
            get {
                return $"{AppName} - {EditVectorDB}";
            }
        }
        // コミット選択
        public string SelectCommitWindowTitle {
            get {
                return $"{AppName} - コミット選択";
            }
        }
        // QAチャット
        public string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // タグ一覧
        public string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // ログ表示
        public string LogWindowTitle {
            get {
                return $"{AppName} - ログ表示";
            }
        }
        // スクリーンショットチェック用プロンプト生成
        public string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - スクリーンショットチェック用プロンプト生成";
            }
        }

        // --- namespace WpfAppCommon.PythonIF ---

        // --- DefaultClipboardController.cs ---
        // クリップボードの内容が変更されました
        public string ClipboardChangedMessage { get; } = "クリップボードの内容が変更されました";
        // クリップボードアイテムを処理
        public string ProcessClipboardItem { get; } = "クリップボードアイテムを処理";
        // 自動処理を実行中
        public string AutoProcessing { get; } = "自動処理を実行中";
        // クリップボードアイテムの追加処理が失敗しました。
        public string AddItemFailed { get; } = "クリップボードアイテムの追加処理が失敗しました。";

        // 自動タイトル設定処理を実行します
        public string AutoSetTitle { get; } = "自動タイトル設定処理を実行します";
        // タイトル設定処理が失敗しました
        public string SetTitleFailed { get; } = "タイトル設定処理が失敗しました";

        // 自動背景情報追加処理を実行します
        public string AutoSetBackgroundInfo { get; } = "自動背景情報追加処理を実行します";
        // 背景情報追加処理が失敗しました
        public string AddBackgroundInfoFailed { get; } = "背景情報追加処理が失敗しました";

        // 自動サマリー作成処理を実行します
        public string AutoCreateSummary { get; } = "自動サマリー作成処理を実行します";
        // サマリー作成処理が失敗しました
        public string CreateSummaryFailed { get; } = "サマリー作成処理が失敗しました";


        // 自動イメージテキスト抽出処理を実行します
        public string AutoExtractImageText { get; } = "自動イメージテキスト抽出処理を実行します";
        // イメージテキスト抽出処理が失敗しました
        public string ExtractImageTextFailed { get; } = "イメージテキスト抽出処理が失敗しました";

        // 自動タグ設定処理を実行します
        public string AutoSetTag { get; } = "自動タグ設定処理を実行します";
        // タグ設定処理が失敗しました
        public string SetTagFailed { get; } = "タグ設定処理が失敗しました";
        // 自動マージ処理を実行します
        public string AutoMerge { get; } = "自動マージ処理を実行します";
        // マージ処理が失敗しました
        public string MergeFailed { get; } = "マージ処理が失敗しました";
        // OCR処理を実行します
        public string OCR { get; } = "OCR処理を実行します";
        // OCR処理が失敗しました
        public string OCRFailed { get; } = "OCR処理が失敗しました";

        // 自動ファイル抽出処理を実行します
        public string ExecuteAutoFileExtract { get; } = "自動ファイル抽出処理を実行します";
        // 自動ファイル抽出処理が失敗しました
        public string AutoFileExtractFailed { get; } = "自動ファイル抽出処理が失敗しました";

        // --- EmptyPythonFunctions.cs ---
        // Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。
        public string PythonNotEnabledMessage { get; } = "Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。";

        // --- PythonExecutor.cs ---
        // カスタムPythonスクリプトの、templateファイル
        public string TemplateScript { get; } = "python/script_template.py";

        // クリップボードアプリ用のPythonスクリプト
        public string WpfAppCommonUtilsScript { get; } = "python/ai_app.py";

        // テンプレートファイルが見つかりません
        public string TemplateScriptNotFound { get; } = "テンプレートファイルが見つかりません";

        // --- PythonNetFunctions.cs ---
        // "PythonDLLが見つかりません。PythonDLLのパスを確認してください:"
        public string PythonDLLNotFound { get; } = "PythonDLLが見つかりません。PythonDLLのパスを確認してください:";
        //  "Pythonの初期化に失敗しました。"
        public string PythonInitFailed { get; } = "Pythonの初期化に失敗しました。";

        // "Pythonスクリプトファイルに、{function_name}関数が見つかりません"
        public string FunctionNotFound(string function_name) {
            return $"Pythonスクリプトファイルに、{function_name}関数が見つかりません";
        }
        // "Pythonスクリプトの実行中にエラーが発生しました
        public string PythonExecuteError { get; } = "Pythonスクリプトの実行中にエラーが発生しました";

        // "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。
        public string ModuleNotFound { get; } = "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。";

        // $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        public string PythonExecuteErrorDetail(Exception e) {
            return $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        }
        // "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください"
        public string SpacyModelNameNotSet { get; } = "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください";

        // "マスキング結果がありません"
        public string MaskingResultNotFound { get; } = "マスキング結果がありません";

        // "マスキングした文字列取得に失敗しました"
        public string MaskingResultFailed { get; } = "マスキングした文字列取得に失敗しました";

        // "マスキング解除結果がありません"
        public string UnmaskingResultNotFound { get; } = "マスキング解除結果がありません";
        // "マスキング解除した文字列取得に失敗しました"
        public string UnmaskingResultFailed { get; } = "マスキング解除した文字列取得に失敗しました";

        // "画像のバイト列に変換できません"
        public string ImageByteFailed { get; } = "画像のバイト列に変換できません";

        // "VectorDBItemsが空です"
        public string VectorDBItemsEmpty { get; } = "VectorDBItemsが空です";

        // "OpenAIの応答がありません"
        public string OpenAIResponseEmpty { get; } = "OpenAIの応答がありません";

        // ファイルが存在しません
        public string FileNotFound { get; } = "ファイルが存在しません";

        // -- ClipboardApp.MainWindowDataGrid1 --
        // 更新日
        public string UpdateDate { get; } = "更新日";
        // タイトル
        public string Title { get; } = "タイトル";

        // ソースタイトル
        public string SourceTitle { get; } = "ソースタイトル";
        // ピン留め
        public string Pin { get; } = "ピン留め";

        // 種別
        public string Type { get; } = "種別";

        // -- AutoProcessRule --
        // 自動処理ルール

        // ルール名
        public string RuleName { get; } = "ルール名";

        // 有効
        public string Enable { get; } = "有効";

        // 適用対象フォルダ
        public string TargetFolder { get; } = "適用対象フォルダ";

        // すべてのアイテムに適用
        public string ApplyAllItems { get; } = "すべてのアイテムに適用";

        // 次の条件に合致するアイテムに適用
        public string ApplyMatchedItems { get; } = "次の条件に合致するアイテムに適用";

        // アイテムの種類
        public string ItemType { get; } = "アイテムの種類";
        // アイテムのタイプがテキストの場合
        public string ItemTypeText { get; } = "アイテムのタイプがテキストの場合";
        // 行以上
        public string LineOrMore { get; } = "行以上";

        // 行以下のテキスト
        public string LineOrLess { get; } = "行以下のテキスト";

        // アイテムのタイプがファイルの場合
        public string ItemTypeFile { get; } = "アイテムのタイプがファイルの場合";

        // アイテムのタイプがイメージの場合
        public string ItemTypeImage { get; } = "アイテムのタイプがイメージの場合";

        // タイトルに次の文字が含まれる場合
        public string TitleContains { get; } = "タイトルに次の文字が含まれる場合";

        // 本文に次の文字列が含まれる場合
        public string BodyContains { get; } = "本文に次の文字列が含まれる場合";

        // ソースアプリの名前に次の文字列が含まれる場合
        public string SourceAppContains { get; } = "ソースアプリの名前に次の文字列が含まれる場合";

        // 実行する処理
        public string ExecuteProcess { get; } = "実行する処理";

        // 次の処理を実行する
        public string ExecuteNextProcess { get; } = "次の処理を実行する";

        // コピー/移動/マージ先
        public string CopyMoveMergeTarget { get; } = "コピー/移動/マージ先";

        // Pythonスクリプトを実行する
        public string ExecutePythonScript { get; } = "Pythonスクリプトを実行する";

        // OpenAIのプロンプトを実行する
        public string ExecuteOpenAI { get; } = "OpenAIのプロンプトを実行する";

        // OpenAIの実行モード
        public string OpenAIMode { get; } = "OpenAIの実行モード";

        // ベクトルDBに格納する
        public string StoreVectorDB { get; } = "ベクトルDBに格納する";

        // 適用対象フォルダ(パス)
        public string TargetFolderFullPath { get; } = "適用対象フォルダ(パス)";

        // フォルダ単位
        public string FolderUnit { get; } = "フォルダ単位";

        // 上へ
        public string Up { get; } = "上へ";
        // 下へ
        public string Down { get; } = "下へ";

        // クリップボード監視対象のソースアプリ名
        public string SourceApp { get; } = "クリップボード監視対象のソースアプリ名";

        // 監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe
        public string SourceAppExample { get; } = "監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe";

        // 指定した行数以下のテキストアイテムを無視
        public string IgnoreTextLessOrEqualToSpecifiedLines { get; } = "指定した行数以下のテキストアイテムを無視";

        // 自動タイトル生成
        public string AutoTitleGeneration { get; } = "自動タイトル生成";

        // しない
        public string DoNot { get; } = "しない";

        // OpenAIを使用して自動的にタイトルを生成する
        public string AutomaticallyGenerateTitleUsingOpenAI { get; } = "OpenAIを使用して自動的にタイトルを生成する";

        // 自動でタグ生成する
        public string AutomaticallyGenerateTags { get; } = "自動でタグ生成する";

        // クリップボードの内容から自動的にタグを生成します
        public string AutomaticallyGenerateTagsFromClipboardContent { get; } = "クリップボードの内容から自動的にタグを生成します";

        // 自動でマージ
        public string AutomaticallyMerge { get; } = "自動でマージ";

        // コピー元のアプリ名、タイトルが同じ場合にアイテムを自動的にマージします
        public string AutomaticallyMergeItemsIfSourceAppAndTitleAreTheSame { get; } = "コピー元のアプリ名、タイトルが同じ場合にアイテムを自動的にマージします";

        // 自動でEmbedding
        public string AutomaticallyEmbedding { get; } = "自動でEmbedding";

        // クリップボードアイテム保存時に自動でEmbeddingを行います
        public string AutomaticallyEmbeddingWhenSavingClipboardItems { get; } = "クリップボードアイテム保存時に自動でEmbeddingを行います";

        // ファイルから自動でテキスト抽出
        public string AutomaticallyExtractTextFromFile { get; } = "ファイルから自動でテキスト抽出";

        // クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います
        public string AutomaticallyExtractTextFromFileIfClipboardItemIsFile { get; } = "クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います";

        // 画像から自動でテキスト抽出
        public string AutomaticallyExtractTextFromImage { get; } = "画像から自動でテキスト抽出";

        // PyOCRを使用してテキスト抽出します
        public string ExtractTextUsingPyOCR { get; } = "PyOCRを使用してテキスト抽出します";

        // OpenAIを使用してテキスト抽出します
        public string ExtractTextUsingOpenAI { get; } = "OpenAIを使用してテキスト抽出します";

        // 画像からテキスト抽出時にEmbedding
        public string EmbeddingWhenExtractingTextFromImage { get; } = "画像からテキスト抽出時にEmbedding";
        // 画像からテキスト抽出時にEmbeddingを行います
        public string EmbeddingWhenExtractingTextFromImageDescription { get; } = "画像からテキスト抽出時にEmbeddingを行います";


        // 自動背景情報追加
        public string AutomaticallyAddBackgroundInformation { get; } = "自動背景情報追加";

        // 同じフォルダにあるアイテムから背景情報を生成します。
        public string GenerateBackgroundInformationFromItemsInTheSameFolder { get; } = "同じフォルダにあるアイテムから背景情報を生成します。";

        // Embeddingに背景情報を含める
        public string IncludeBackgroundInformationInEmbedding { get; } = "Embeddingに背景情報を含める";

        // Embedding対象テキストに背景情報を含めます。
        public string IncludeBackgroundInformationInEmbeddingTargetText { get; } = "Embedding対象テキストに背景情報を含めます。";

        // 自動サマリー生成
        public string AutomaticallyGenerateSummary { get; } = "自動サマリー生成";

        // コンテンツからサマリーテキストを生成します。
        public string GenerateSummaryTextFromContent { get; } = "コンテンツからサマリーテキストを生成します。";

        // クリップボードアイテムをOS上のフォルダと同期させる
        public string SynchronizeClipboardItemsWithFoldersOnTheOS { get; } = "クリップボードアイテムをOS上のフォルダと同期させる";

        // クリップボードアイテムをOS上のフォルダと同期させます。
        public string SynchronizeClipboardItemsWithFoldersOnTheOSDescription { get; } = "クリップボードアイテムをOS上のフォルダと同期させます。";

        // 同期先のフォルダ名
        public string SyncTargetFolderName { get; } = "同期先のフォルダ名";

        // クリップボードアイテムを同期するOS上のフォルダ名を指定。
        public string SpecifyTheFolderNameOnTheOSToSynchronizeTheClipboardItems { get; } = "クリップボードアイテムを同期するOS上のフォルダ名を指定。";

        // 同期先のフォルダがGitリポジトリの場合、ファイル更新時に自動的にコミットします。
        public string IfTheSyncTargetFolderIsAGitRepositoryItWillAutomaticallyCommitWhenTheFileIsUpdated { get; } = "同期先のフォルダがGitリポジトリの場合、ファイル更新時に自動的にコミットします。";

        // エンティティ抽出/データマスキング
        public string EntityExtractionDataMasking { get; } = "エンティティ抽出/データマスキング";

        // クリップボードの内容からSpacyを使用してエンティティ抽出、データマスキングを行います
        public string ExtractEntitiesAndMaskDataUsingSpacyFromClipboardContent { get; } = "クリップボードの内容からSpacyを使用してエンティティ抽出、データマスキングを行います";

        // OpenAIに送信するデータ内の個人情報などをマスキングします。
        public string MaskPersonalInformationInDataSentToOpenAI { get; } = "OpenAIに送信するデータ内の個人情報などをマスキングします。";

        // 新規自動処理ルール
        public string NewAutoProcessRule { get; } = "新規自動処理ルール";

        // システム共通設定を保存
        public string SaveSystemCommonSettings { get; } = "システム共通設定を保存";

        // -- FolderEditWindow --
        // クリップボードフォルダ編集
        public string EditClipboardFolder { get; } = "クリップボードフォルダ編集";

        // 名前
        public string Name { get; } = "名前";

        // 説明
        public string Description { get; } = "説明";

        // フォルダ選択
        public string SelectFolder { get; } = "フォルダ選択";

        // -- EditItemWindow --
        // テキストをファイルとして開く
        public string OpenTextAsFile { get; } = "テキストをファイルとして開く";

        // ファイルを開く
        public string OpenFile { get; } = "ファイルを開く";

        // 新規ファイルとして開く
        public string OpenAsNewFile { get; } = "新規ファイルとして開く";

        // フォルダを開く
        public string OpenFolder { get; } = "フォルダを開く";

        // テキストを抽出
        public string ExtractText { get; } = "テキストを抽出";

        // 画像を開く
        public string OpenImage { get; } = "画像を開く";

        // 画像からテキストを抽出
        public string ExtractTextFromImage { get; } = "画像からテキストを抽出";

        // データをマスキング
        public string MaskData { get; } = "データをマスキング";

        // ここをクリックするとタグ編集画面が開きます
        public string ClickHereToOpenTheTagEditScreen { get; } = "ここをクリックするとタグ編集画面が開きます";

        // テキスト
        public string Text { get; } = "テキスト";

        // ファイルパス
        public string FilePath { get; } = "ファイルパス";

        // フォルダ
        public string Folder { get; } = "フォルダ";

        // ファイル名
        public string FileName { get; } = "ファイル名";

        // フォルダ名とファイル名
        public string FolderNameAndFileName { get; } = "フォルダ名とファイル名";

        // イメージ
        public string Image { get; } = "イメージ";

        // -- EditPythonScriptWindow --
        // 内容
        public string Content { get; } = "内容";

        // -- ListPythonScriptWindow --
        // 新規Pythonスクリプト
        public string NewPythonScript { get; } = "新規Pythonスクリプト";

        // -- SearchWindow --
        // 検索対象フォルダ
        public string SearchTargetFolder { get; } = "検索対象フォルダ";

        // 除外
        public string Exclude { get; } = "除外";

        // コピー元アプリ名
        public string CopySourceAppName { get; } = "コピー元アプリ名";

        // 開始日
        public string StartDate { get; } = "開始日";

        // 終了日
        public string EndDate { get; } = "終了日";

        // 適用対象配下のフォルダも対象にする
        public string IncludeSubfolders { get; } = "適用対象配下のフォルダも対象にする";

        // クリア
        public string Clear { get; } = "クリア";

        // -- TagSearchWindow
        // タグ検索
        public string TagSearch { get; } = "タグ検索";

        // -- VectorSearchResultWindow
        // ベクトル検索結果
        public string VectorSearchResult { get; } = "ベクトル検索結果";

        // -- ImageChatWindow
        // 設定項目
        public string SettingItem { get; } = "設定項目";

        // 設定値
        public string SettingValue { get; } = "設定値";

        // チェックタイプ
        public string CheckType { get; } = "チェックタイプ";

        // 貼り付け
        public string Paste { get; } = "貼り付け";

        // -- ImageChat.MainWindow --
        // 画像ファイル選択
        public string SelectImageFile { get; } = "画像ファイル選択";

        // 画像エビデンスチェック項目編集
        public string EditImageEvidenceCheckItem { get; } = "画像エビデンスチェック項目編集";

        // 開く
        public string Open { get; } = "開く";

        // ここに回答が表示されます
        public string TheAnswerWillBeDisplayedHere { get; } = "ここに回答が表示されます";

        // ここに質問を入力
        public string EnterYourQuestionHere { get; } = "ここに質問を入力";

        // 保存
        public string Save { get; } = "保存";

        // 送信
        public string Send { get; } = "送信";

        // -- ListVectorDBWindow --
        // システム用のベクトルを表示
        public string DisplayVectorsForTheSystem { get; } = "システム用のベクトルを表示";

        // ベクトルDBの場所
        public string VectorDBLocation { get; } = "ベクトルDBの場所";

        // ベクトルDBのタイプ
        public string VectorDBType { get; } = "ベクトルDBのタイプ";

        // 新規ベクトルDB設定
        public string NewVectorDBSetting { get; } = "新規ベクトルDB設定";

        // ベクトルDB設定編集
        public string EditVectorDBSetting { get; } = "ベクトルDB設定編集";

        // -- QAChatControl --
        // 実験的機能1(文章解析+辞書生成+RAG)"
        public string ExperimentalFunction1 { get; } = "実験的機能1(文章解析+辞書生成+RAG)";

        // ベクトルDB(フォルダ)
        public string VectorDBFolder { get; } = "ベクトルDB(フォルダ)";

        // ここをクリックしてベクトルDB(フォルダ)を追加
        public string ClickHereToAddVectorDBFolder { get; } = "ここをクリックしてベクトルDB(フォルダ)を追加";

        // リストから除外
        public string ExcludeFromList { get; } = "リストから除外";

        // ベクトルDB(外部)
        public string VectorDBExternal { get; } = "ベクトルDB(外部)";

        // ここをクリックしてベクトルDB(外部)を追加
        public string ClickHereToAddVectorDBExternal { get; } = "ここをクリックしてベクトルDB(外部)を追加";

        // 画像アイテム
        public string ImageItem { get; } = "画像アイテム";

        // ここをクリックして選択中のアイテムを貼り付け
        public string ClickHereToPasteTheSelectedItem { get; } = "ここをクリックして選択中のアイテムを貼り付け";

        // 画像ファイル
        public string ImageFile { get; } = "画像ファイル";

        // ここをクリックして画像ファイルを追加
        public string ClickHereToAddImageFile { get; } = "ここをクリックして画像ファイルを追加";

        // チャット
        public string Chat { get; } = "チャット";

        // プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public string PromptTemplate { get; } = "プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。";

        // プレビュー
        public string Preview { get; } = "プレビュー";

        // プレビュー(JSON)
        public string PreviewJSON { get; } = "プレビュー(JSON)";

        // Copy
        public string Copy { get; } = "Copy";

        // --- ClipboardFolderViewModel ---
        // 自動処理が設定されています
        public string AutoProcessingIsSet { get; } = "自動処理が設定されています";

        // 検索条件
        public string SearchCondition { get; } = "検索条件";

        // フォルダを編集しました
        public string FolderEdited { get; } = "フォルダを編集しました";

        // リロードしました
        public string Reloaded { get; } = "リロードしました";

        // フォルダを選択してください
        public string SelectFolderPlease { get; } = "フォルダを選択してください";

        // フォルダをエクスポートしました
        public string FolderExported { get; } = "フォルダをエクスポートしました";

        // フォルダをインポートしました
        public string FolderImported { get; } = "フォルダをインポートしました";

        // ルートフォルダは削除できません
        public string RootFolderCannotBeDeleted { get; } = "ルートフォルダは削除できません";

        // 確認
        public string Confirm { get; } = "確認";

        // "フォルダを削除しますか？"
        public string ConfirmDeleteFolder { get; } = "フォルダを削除しますか？";

        // "フォルダを削除しました"
        public string FolderDeleted { get; } = "フォルダを削除しました";

        // "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?"
        public string ConfirmDeleteItems { get; } = "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?";

        // アイテムを削除しました
        public string DeletedItems { get; } = "アイテムを削除しました";

        // "追加しました"
        public string Added { get; } = "追加しました";

        // "編集しました"
        public string Edited { get; } = "編集しました";

        // 貼り付けました
        public string Pasted { get; } = "貼り付けました";

        // "マージするアイテムを2つ選択してください"
        public string SelectTwoItemsToMerge { get; } = "マージするアイテムを2つ選択してください";

        // マージ先のアイテムが選択されていません
        public string MergeTargetNotSelected { get; } = "マージ先のアイテムが選択されていません";

        // マージ元のアイテムが選択されていません
        public string MergeSourceNotSelected { get; } = "マージ元のアイテムが選択されていません";

        // "マージしました"
        public string Merged { get; } = "マージしました";

        // エラーが発生しました。\nメッセージ
        public string ErrorOccurredAndMessage { get; } = "エラーが発生しました。\nメッセージ";

        // スタックトレース
        public string StackTrace { get; } = "スタックトレース";

        // --- ClipboardItemViewModel ---
        // ファイル以外のコンテンツはフォルダを開けません
        public string CannotOpenFolderForNonFileContent { get; } = "ファイル以外のコンテンツはフォルダを開けません";

        // ファイル以外のコンテンツはテキストを抽出できません
        public string CannotExtractTextForNonFileContent { get; } = "ファイル以外のコンテンツはテキストを抽出できません";

        // "MainWindowViewModelがNullです"
        public string MainWindowViewModelIsNull { get; } = "MainWindowViewModelがNullです";

        // 背景情報を生成します
        public string GenerateBackgroundInformation { get; } = "背景情報を生成します";

        // "背景情報を生成しました"
        public string GeneratedBackgroundInformation { get; } = "背景情報を生成しました";

        // "サマリーを生成します"
        public string GenerateSummary2 { get; } = "サマリーを生成します";

        // "サマリーを生成しました"
        public string GeneratedSummary { get; } = "サマリーを生成しました";

        // ベクトルを生成します
        public string GenerateVector2 { get; } = "ベクトルを生成します";

        // "ベクトルを生成しました"
        public string GeneratedVector { get; } = "ベクトルを生成しました";

        // 画像以外のコンテンツはテキストを抽出できません
        public string CannotExtractTextForNonImageContent { get; } = "画像以外のコンテンツはテキストを抽出できません";

        // "数値を入力してください。"
        public string EnterANumber { get; } = "数値を入力してください。";

        // フォルダが選択されていません。
        public string FolderNotSelected { get; } = "フォルダが選択されていません。";

        // ルール名を入力してください。
        public string EnterRuleName { get; } = "ルール名を入力してください。";

        // "アクションを選択してください。"
        public string SelectAction { get; } = "アクションを選択してください。";

        // "編集対象のルールが見つかりません。"
        public string RuleNotFound { get; } = "編集対象のルールが見つかりません。";

        // コピーまたは移動先のフォルダを選択してください。
        public string SelectCopyOrMoveTargetFolder { get; } = "コピーまたは移動先のフォルダを選択してください。";

        // "同じフォルダにはコピーまたは移動できません。"
        public string CannotCopyOrMoveToTheSameFolder { get; } = "同じフォルダにはコピーまたは移動できません。";

        // "コピー/移動処理の無限ループを検出しました。"
        public string DetectedAnInfiniteLoopInCopyMoveProcessing { get; } = "コピー/移動処理の無限ループを検出しました。";

        // "PromptTemplateを選択してください。"
        public string SelectPromptTemplate { get; } = "PromptTemplateを選択してください。";

        // PythonScriptを選択してください。
        public string SelectPythonScript { get; } = "PythonScriptを選択してください。";

        // "VectorDBを選択してください。"
        public string SelectVectorDB { get; } = "VectorDBを選択してください。";

        // 標準フォルダ以外にはコピーまたは移動できません。
        public string CannotCopyOrMoveToNonStandardFolders { get; } = "標準フォルダ以外にはコピーまたは移動できません。";

        // RootFolderViewModelがNullです。
        public string RootFolderViewModelIsNull { get; } = "RootFolderViewModelがNullです。";

        // --- EditPythonScriptWindowViewModel ---
        // 説明を入力してください
        public string EnterDescription { get; } = "説明を入力してください";

        // --- FolderEditWindowViewModel ---
        // フォルダが指定されていません
        public string FolderNotSpecified { get; } = "フォルダが指定されていません";

        // フォルダ名を入力してください
        public string EnterFolderName { get; } = "フォルダ名を入力してください";

        // --- FolderSelectWindowViewModel ---
        // "エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない
        public string FolderSelectWindowViewModelInstanceNotFound { get; } = "エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない";

        // エラーが発生しました。選択中のフォルダがない
        public string SelectedFolderNotFound { get; } = "エラーが発生しました。選択中のフォルダがない";

        // --- ListAutoProcessRuleWindowViewModel ---
        // 自動処理ルールが選択されていません。
        public string AutoProcessRuleNotSelected { get; } = "自動処理ルールが選択されていません。";

        // を削除しますか？
        public string ConfirmDelete { get; } = "を削除しますか？";

        // "システム共通設定を保存しました。"
        public string SavedSystemCommonSettings { get; } = "システム共通設定を保存しました。";

        // "システム共通設定の変更はありません。"
        public string NoChangesToSystemCommonSettings { get; } = "システム共通設定の変更はありません。";

        // --- ListPythonScriptWindowViewModel ---
        // 実行
        public string Execute { get; } = "実行";

        // スクリプトを選択してください
        public string SelectScript { get; } = "スクリプトを選択してください";

        // --- SearchWindowViewModel ---
        // 検索フォルダ
        public string SearchFolder { get; } = "検索フォルダ";

        // 標準
        public string Standard { get; } = "標準";

        // SearchConditionRuleがNullです
        public string SearchConditionRuleIsNull { get; } = "SearchConditionRuleがNullです";

        // 検索条件がありません
        public string NoSearchConditions { get; } = "検索条件がありません";

        // --- TagSearchWindowViewModel ---
        // "タグが空です"
        public string TagIsEmpty { get; } = "タグが空です";

        // "タグが既に存在します"
        public string TagAlreadyExists { get; } = "タグが既に存在します";

        // バージョン情報
        public string VersionInformation { get; } = "バージョン情報";

        // -- ClipboardApp.MainWindowViewModel --
        // "アプリケーションを再起動すると、表示モードが変更されます。"
        public string DisplayModeWillChangeWhenYouRestartTheApplication { get; } = "アプリケーションを再起動すると、表示モードが変更されます。";
        // 情報
        public string Information { get; } = "情報";

        // "終了しますか?"
        public string ConfirmExit { get; } = "終了しますか?";

        // 選択中のアイテムがない"
        public string NoItemSelected { get; } = "選択中のアイテムがない";

        // 切り取りました"
        public string Cut { get; } = "切り取りました";

        // コピーしました"
        public string Copied { get; } = "コピーしました";

        // 貼り付け先のフォルダがない
        public string NoPasteFolder { get; } = "貼り付け先のフォルダがない";

        // "コピー元のフォルダがない"
        public string NoCopyFolder { get; } = "コピー元のフォルダがない";

        // "選択中のアイテムを削除しますか?"
        public string ConfirmDeleteSelectedItems { get; } = "選択中のアイテムを削除しますか?";

        // "削除しました"
        public string Deleted { get; } = "削除しました";

        // --- ImageCHat ---
        // 画像を確認して以下の各文が正しいか否かを教えてください\n\n
        public string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "画像を確認して以下の各文が正しいか否かを教えてください\n\n";

        // 画像ファイルが選択されていません。
        public string NoImageFileSelected { get; } = "画像ファイルが選択されていません。";

        // プロンプトを送信します
        public string SendPrompt { get; } = "プロンプトを送信します";
        // 画像ファイル名
        public string ImageFileName { get; } = "画像ファイル名";

        // エラーが発生しました。
        public string ErrorOccurred { get; } = "エラーが発生しました。";

        // 画像ファイルを選択してください
        public string SelectImageFilePlease { get; } = "画像ファイルを選択してください";

        // すべてのファイル
        public string AllFiles { get; } = "すべてのファイル";

        // "ファイルが存在しません。"
        public string FileDoesNotExist { get; } = "ファイルが存在しません。";

        // -- EditPromptItemWindowViewModel --
        // プロンプト編集
        public string EditPrompt { get; } = "プロンプト編集";

        // -- ListPromptTemplateWindow -- 
        // 新規プロンプトテンプレート
        public string NewPromptTemplate { get; } = "新規プロンプトテンプレート";

        // RAG
        public string RAG { get; } = "RAG";

        // -- DevFeatures.cs
        // テキスト以外のコンテンツはマスキングできません
        public string CannotMaskNonTextContent { get; } = "テキスト以外のコンテンツはマスキングできません";

        // データをマスキングしました
        public string MaskedData { get; } = "データをマスキングしました";

        // マスキングデータをもとに戻します
        public string RestoreMaskingData { get; } = "マスキングデータをもとに戻します";

        // 画像が取得できません
        public string CannotGetImage { get; } = "画像が取得できません";

        // リモートリポジトリが設定されていません
        public string NoRemoteRepositorySet { get; } = "リモートリポジトリが設定されていません";

        // 作業ディレクトリが指定されていません
        public string NoWorkingDirectorySpecified { get; } = "作業ディレクトリが指定されていません";

        // "指定されたディレクトリが存在しません"
        public string SpecifiedDirectoryDoesNotExist { get; } = "指定されたディレクトリが存在しません";

        // "指定されたディレクトリはGitリポジトリではありません"
        public string SpecifiedDirectoryIsNotAGitRepository { get; } = "指定されたディレクトリはGitリポジトリではありません";

        // "ベクトルDBが設定されていません"
        public string NoVectorDBSet { get; } = "ベクトルDBが設定されていません";

        // -- ScreenShotCheckCondition.cs --

        public string CheckTypeEqual { get; } = "等しい";
        public string CheckTypeNotEqual { get; } = "等しくない";
        public string CheckTypeInclude { get; } = "含む";
        public string CheckTypeNotInclude { get; } = "含まない";
        public string CheckTypeStartWith { get; } = "開始している";
        public string CheckTypeNotStartWith { get; } = "開始していない";
        public string CheckTypeEndWith { get; } = "終わっている";
        public string CheckTypeNotEndWith { get; } = "終わっていない";
        public string CheckTypeEmpty { get; } = "空である";
        public string CheckTypeCheckBox { get; } = "チェックボックス";

        // -- ScriptAutoProcessItem.cs --
        // Pythonスクリプトを実行しました
        public string ExecutedPythonScript { get; } = "Pythonスクリプトを実行しました";

        // -- SystemAutoProcessItem.cs --
        // AutoProcessItemが見つかりません
        public string AutoProcessItemNotFound { get; } = "AutoProcessItemが見つかりません";

        // -- EnumDescription.cs --

        // Enum型ではありません
        public string NotEnumType { get; } = "Enum型ではありません";

        // --- WindowsNotificationController.cs ---
        // アクセス拒否
        public string AccessDenied { get; } = "アクセス拒否";

        // --- EditPromptItemWindowViewModel ---
        // 名前を入力してください
        public string EnterName { get; } = "名前を入力してください";

        // --- EditRAGSourceWindowViewModel ---
        // RAGソース編集
        public string EditRAGSource { get; } = "RAGソース編集";

        // ItemViewModelがnullです"
        public string ItemViewModelIsNull { get; } = "ItemViewModelがnullです";

        // EditVectorDBWindowViewModel
        // Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません
        public string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません";

        // プロンプトテンプレート一覧
        public string PromptTemplateList { get; } = "プロンプトテンプレート一覧";

        // プロンプトテンプレートが選択されていません
        public string NoPromptTemplateSelected { get; } = "プロンプトテンプレートが選択されていません";

        // ListVectorDBWindowViewModel
        // 編集するベクトルDBを選択してください
        public string SelectVectorDBToEdit { get; } = "編集するベクトルDBを選択してください";

        // 削除するベクトルDBを選択してください
        public string SelectVectorDBToDelete { get; } = "削除するベクトルDBを選択してください";

        // 選択中のベクトルDBを削除しますか？
        public string ConfirmDeleteSelectedVectorDB { get; } = "選択中のベクトルDBを削除しますか？";

        // ベクトルDBを選択してください
        public string SelectVectorDBPlease { get; } = "ベクトルDBを選択してください";

        // RAGManagementWindowViewModel
        // 編集するRAG Sourceを選択してください
        public string SelectRAGSourceToEdit { get; } = "編集するRAG Sourceを選択してください";

        // 削除するRAG Sourceを選択してください
        public string SelectRAGSourceToDelete { get; } = "削除するRAG Sourceを選択してください";

        // 選択中のRAG Sourceを削除しますか？
        public string ConfirmDeleteSelectedRAGSource { get; } = "選択中のRAG Sourceを削除しますか？";

        // コミットを選択してください
        public string SelectCommitPlease { get; } = "コミットを選択してください";

        // --- UpdateRAGIndexWindowViewModel
        // インデックス化対象ファイル
        public string IndexingTargetFile { get; } = "インデックス化対象ファイル";

        // ファイル追加
        public string AddFile { get; } = "ファイル追加";
        // ファイル削除
        public string DeleteFile { get; } = "ファイル削除";

        // ファイル更新
        public string UpdateFile { get; } = "ファイル更新";

        // 対象ファイル取得
        public string GetTargetFile { get; } = "対象ファイル取得";

        // インデックス作成
        public string CreateIndex { get; } = "インデックス作成";

        //  "戻る"
        public string Back { get; } = "戻る";

        // RAGSourceItemViewModelが設定されていません
        public string RAGSourceItemViewModelNotSet { get; } = "RAGSourceItemViewModelが設定されていません";

        // "開始コミットを指定してください"
        public string SpecifyStartCommit { get; } = "開始コミットを指定してください";

        // "対象を選択してください"
        public string SelectTarget { get; } = "対象を選択してください";

        // 処理ファイル数
        public string ProcessedFileCount { get; } = "処理ファイル数";

        // インデックス作成中
        public string CreatingIndex { get; } = "インデックス作成中";

        // 完了
        public string Completed { get; } = "完了";

        // 未対応ファイルタイプのためスキップ
        public string SkipUnsupportedFileType { get; } = "未対応ファイルタイプのためスキップ";

        // 失敗
        public string Failed { get; } = "失敗";

        // "インデックス作成が完了しました"
        public string IndexCreationCompleted { get; } = "インデックス作成が完了しました";

        // インデックス作成処理を中断しました
        public string IndexCreationInterrupted { get; } = "インデックス作成処理を中断しました";

        // チャットの送信に失敗しました。
        public string FailedToSendChat { get; } = "チャットの送信に失敗しました。";

        // --- SettingUserControlViewModel
        // Pythonの設定チェック
        public string PythonSettingCheck { get; } = "Pythonの設定チェック";

        // PythonDLLのパスが設定されていません
        public string PythonDLLPathNotSet { get; } = "PythonDLLのパスが設定されていません";

        // PythonDLLのパスが設定されています
        public string PythonDLLPathSet { get; } = "PythonDLLのパスが設定されています";

        // PythonDLLのファイルが存在しません
        public string PythonDLLFileDoesNotExist { get; } = "PythonDLLのファイルが存在しません";

        // PythonDLLのファイルが存在します
        public string PythonDLLFileExists { get; } = "PythonDLLのファイルが存在します";

        // Pythonスクリプトをテスト実行
        public string TestRunPythonScript { get; } = "Pythonスクリプトをテスト実行";

        // OpenAIの設定チェック
        public string OpenAISettingCheck { get; } = "OpenAIの設定チェック";

        // OpenAIのAPIキーが設定されていません
        public string OpenAIKeyNotSet { get; } = "OpenAIのAPIキーが設定されていません";
        // OpenAIのAPIキーが設定されています
        public string OpenAIKeySet { get; } = "OpenAIのAPIキーが設定されています";

        // OpenAIのCompletionModelが設定されていません
        public string OpenAICompletionModelNotSet { get; } = "OpenAIのCompletionModelが設定されていません";

        // OpenAIのCompletionModelが設定されています
        public string OpenAICompletionModelSet { get; } = "OpenAIのCompletionModelが設定されています";

        // OpenAIのEmbeddingModelが設定されていません
        public string OpenAIEmbeddingModelNotSet { get; } = "OpenAIのEmbeddingModelが設定されていません";

        // OpenAIのEmbeddingModelが設定されています
        public string OpenAIEmbeddingModelSet { get; } = "OpenAIのEmbeddingModelが設定されています";

        // Azure OpenAIの設定チェック
        public string AzureOpenAISettingCheck { get; } = "Azure OpenAIの設定チェック";

        // Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック
        public string AzureOpenAIEndpointNotSet { get; } = "Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック";

        // Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください
        public string SetAzureOpenAIEndpointOrBaseURL { get; } = "Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください";

        // Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません
        public string CannotSetBothAzureOpenAIEndpointAndBaseURL { get; } = "Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません";

        // OpenAIのテスト実行
        public string TestRunOpenAI { get; } = "OpenAIのテスト実行";

        // Pythonの実行に失敗しました
        public string FailedToRunPython { get; } = "Pythonの実行に失敗しました";

        // Pythonの実行が可能です
        public string PythonRunIsPossible { get; } = "Pythonの実行が可能です";

        // OpenAIの実行に失敗しました
        public string FailedToRunOpenAI { get; } = "OpenAIの実行に失敗しました";

        // OpenAIの実行が可能です。
        public string OpenAIRunIsPossible { get; } = "OpenAIの実行が可能です。";

        // LangChainの実行に失敗しました
        public string FailedToRunLangChain { get; } = "LangChainの実行に失敗しました";

        // LangChainの実行が可能です。
        public string LangChainRunIsPossible { get; } = "LangChainの実行が可能です。";
        // 実行しますか？
        public string ConfirmRun { get; } = "実行しますか？";

        // 設定チェック中
        public string CheckingSettings { get; } = "設定チェック中";

        // 設定を保存しました。
        public string SettingsSaved { get; } = "設定を保存しました。";

        // "キャンセルしました"
        public string Canceled { get; } = "キャンセルしました";

        // MyStatusBarViewModel
        // ログ
        public string Log { get; } = "ログ";

        // --- AutoProcessRule.cs ---
        // RuleName + "は無効です"
        public string RuleNameIsInvalid(string RuleName) {
            return RuleName + "は無効です";
        }
        // 条件にマッチしませんでした
        public string NoMatch { get; } = "条件にマッチしませんでした";

        // アクションが設定されていません
        public string NoActionSet { get; } = "アクションが設定されていません";

        // 条件
        public string Condition { get; } = "条件";

        // アクション
        public string Action { get; } = "アクション";

        // アクション:なし
        public string ActionNone { get; } = "アクション:なし";

        // フォルダ:なし
        public string FolderNone { get; } = "フォルダ:なし";

        // 無限ループを検出しました
        public string DetectedAnInfiniteLoop { get; } = "無限ループを検出しました";

        // "Descriptionが" + condition.Keyword + "を含む
        public string DescriptionContains(string Keyword) {
            return "Descriptionが" + Keyword + "を含む";
        }
        // "Contentが" + condition.Keyword + "を含む 
        public string ContentContains(string Keyword) {
            return "Contentが" + Keyword + "を含む";
        }
        // "SourceApplicationNameが" + condition.Keyword + "を含む \n";
        public string SourceApplicationNameContains(string Keyword) {
            return "SourceApplicationNameが" + Keyword + "を含む \n";
        }
        // "SourceApplicationTitleが" + condition.Keyword + "を含む
        public string SourceApplicationTitleContains(string Keyword) {
            return "SourceApplicationTitleが" + Keyword + "を含む";
        }
        // "SourceApplicationPathが" + condition.Keyword + "を含む
        public string SourceApplicationPathContains(string Keyword) {
            return "SourceApplicationPathが" + Keyword + "を含む";
        }
        // --- ClipboardAppVectorDBItem
        //  "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        public string GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions { get; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";

        // --- ClipboardFolder.cs ---
        // クリップボード
        public string Clipboard { get; } = "クリップボード";

        // チャット履歴
        public string ChatHistory { get; } = "チャット履歴";

        // 自動処理でアイテムが削除または移動されました
        public string ItemsDeletedOrMovedByAutoProcessing { get; } = "自動処理でアイテムが削除または移動されました";

        // "アイテムを追加しました"
        public string AddedItems { get; } = "アイテムを追加しました";

        // 自動処理を適用します
        public string ApplyAutoProcessing { get; } = "自動処理を適用します";

        // 自動処理でアイテムが削除されました
        public string ItemsDeletedByAutoProcessing { get; } = "自動処理でアイテムが削除されました";

        // JSON文字列をパースできませんでした
        public string FailedToParseJSONString { get; } = "JSON文字列をパースできませんでした";

        // ClipboardItem
        // Text以外のアイテムへのマージはできません
        public string CannotMergeToNonTextItems { get; } = "Text以外のアイテムへのマージはできません";

        // "Text以外のアイテムが含まれているアイテムはマージできません"
        public string CannotMergeItemsContainingNonTextItems { get; } = "Text以外のアイテムが含まれているアイテムはマージできません";

        // 作成日時
        public string CreationDateTime { get; } = "作成日時";

        // ソースアプリ名
        public string SourceAppName { get; } = "ソースアプリ名";

        // ピン留めしてます
        public string Pinned { get; } = "ピン留めしてます";

        // フォルダを取得できません
        public string CannotGetFolder { get; } = "フォルダを取得できません";

        // OS上のファイルに保存します
        public string SaveToFileOnOS { get; } = "OS上のファイルに保存します";

        // Gitコミットしました
        public string CommittedToGit { get; } = "Gitコミットしました";

        // リポジトリが見つかりませんでした
        public string RepositoryNotFound { get; } = "リポジトリが見つかりませんでした";

        // コミットが空です
        public string CommitIsEmpty { get; } = "コミットが空です";

        // OS上のファイルに保存しました
        public string SavedToFileOnOS { get; } = "OS上のファイルに保存しました";

        // 背景情報
        public string BackgroundInformation { get; } = "背景情報";

        // "Embeddingを保存します
        public string SaveEmbedding { get; } = "Embeddingを保存します";


        // Embeddingを保存しました
        public string SavedEmbedding { get; } = "Embeddingを保存しました";

        // Embeddingを削除します
        public string DeleteEmbedding { get; } = "Embeddingを削除します";

        // Embeddingを削除しました
        public string DeletedEmbedding { get; } = "Embeddingを削除しました";

        // 画像から抽出したテキストのEmbeddingを保存します
        public string SaveTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存します";
        // 画像から抽出したテキストのEmbeddingを保存しました
        public string SavedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存しました";

        // 画像から抽出したテキストのEmbeddingを削除します
        public string DeleteTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除します";
        // 画像から抽出したテキストのEmbeddingを削除しました
        public string DeletedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除しました";


        // "OS上のファイルを削除します"
        public string DeleteFileOnOS { get; } = "OS上のファイルを削除します";
        // OS上のファイルを削除しました
        public string DeletedFileOnOS { get; } = "OS上のファイルを削除しました";

        // サポートされていないファイル形式です
        public string UnsupportedFileType { get; } = "サポートされていないファイル形式です";

        // テキストを抽出しました
        public string ExtractedText { get; } = "テキストを抽出しました";

        // --- ScreenShotCheckCondition.cs ---
        //  $"{SettingItem}の値は{SettingValue}である";
        public string SettingValueIs(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}である";
        }

        // $"{SettingItem}の値は{SettingValue}でない";
        public string SettingValueIsNot(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}でない";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれている";
        public string SettingValueContains(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれている";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれていない";
        public string SettingValueNotContain(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれていない";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっている";
        public string SettingValueStartsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっている";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっていない";
        public string SettingValueNotStartWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっていない";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっている";
        public string SettingValueEndsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっている";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっていない";
        public string SettingValueNotEndWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっていない";
        }

        //  $"{SettingItem}の値が空である";
        public string SettingValueIsEmpty(string SettingItem) {
            return $"{SettingItem}の値が空である";
        }
        // $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        public string SettingValueIsChecked(string SettingItem, string SettingValue) {
            return $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        }

        // --- SystemAutoProcessItem.cs ---
        // 無視
        public string Ignore { get; } = "無視";
        // "何もしません"
        public string DoNothing { get; } = "何もしません";

        // フォルダにコピー
        public string CopyToFolder { get; } = "フォルダにコピー";
        // クリップボードの内容を指定されたフォルダにコピーします
        public string CopyClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダにコピーします";

        // フォルダに移動"
        public string MoveToFolder { get; } = "フォルダに移動";
        // "クリップボードの内容を指定されたフォルダに移動します"
        public string MoveClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダに移動します";

        // "クリップボードのテキストを抽出します"
        public string ExtractClipboardText { get; } = "クリップボードのテキストを抽出します";

        // "データマスキング",
        public string DataMasking { get; } = "データマスキング";
        // "クリップボードのテキストをマスキングします"
        public string MaskClipboardText { get; } = "クリップボードのテキストをマスキングします";

        //  "フォルダ内のアイテムをマージ", 
        public string MergeItemsInFolder { get; } = "フォルダ内のアイテムをマージ";

        // "フォルダ内のアイテムをマージします"
        public string MergeItemsInFolderDescription { get; } = "フォルダ内のアイテムをマージします";

        // "同じSourceApplicationTitleを持つアイテムをマージ",
        public string MergeItemsWithTheSameSourceApplicationTitle { get; } = "同じSourceApplicationTitleを持つアイテムをマージ";
        // "同じSourceApplicationTitleを持つアイテムをマージします"
        public string MergeItemsWithTheSameSourceApplicationTitleDescription { get; } = "同じSourceApplicationTitleを持つアイテムをマージします";

        // フォルダが選択されていません
        public string NoFolderSelected { get; } = "フォルダが選択されていません";

        // フォルダにコピーします
        public string CopyToFolderDescription { get; } = "フォルダにコピーします";

        // ディレクトリは新規ファイルとして開けません
        public string CannotOpenDirectoryAsNewFile { get; } = "ディレクトリは新規ファイルとして開けません";

        // --- TextSelector.cs ---
        // ファイルを実行できませんでした
        public string FailedToRunFile { get; } = "ファイルを実行できませんでした";

        // テキストファイルとして開きます。
        public string OpenAsTextFile { get; } = "テキストファイルとして開きます。";

        // --- EditChatItemWindow ---
        // チャットアイテム
        public string ChatItem { get; } = "チャットアイテム";

    }
}
