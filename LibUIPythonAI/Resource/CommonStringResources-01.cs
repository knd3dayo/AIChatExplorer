namespace LibUIPythonAI.Resource {
    public partial class CommonStringResources {

        #region --- プロンプトメニュー --------------------------------------------------

        public virtual string PromptMenu { get; } = "プロンプトメニュー";
        // タイトルを生成
        public virtual string GenerateTitle { get; } = "タイトルを生成";

        // 背景情報を生成
        public virtual string GenerateBackgroundInfo { get; } = "背景情報を生成";

        // サマリーを生成
        public virtual string GenerateSummary { get; } = "サマリーを生成";

        // ベクトル生成
        public virtual string GenerateVector { get; } = "ベクトル生成";

        #endregion

        //ベクトル検索
        public virtual string VectorSearch { get; } = "ベクトル検索";

        // ベクトルDBのリフレッシュ
        public virtual string RefreshVectorDB { get; } = "ベクトルDBのリフレッシュ";


        // 自動処理ルール一覧
        public virtual string ListAutoProcessRule { get; } = "自動処理ルール一覧";
        // Pythonスクリプト一覧
        public virtual string ListPythonScript { get; } = "Pythonスクリプト一覧";

        // タグ一覧
        public virtual string ListTag { get; } = "タグ一覧";

        // 新規タグ
        public virtual string NewTag { get; } = "新規タグ";
        // タグ
        public virtual string Tag { get; } = "タグ";

        // ベクトルDB一覧
        public virtual string ListVectorDB { get; } = "ベクトルDB一覧";
        // ベクトルDB編集
        public virtual string EditVectorDB { get; } = "ベクトルDB編集";

        // --- ToolTip ---
        // 選択中のフォルダにアイテムを作成します。
        public virtual string CreateItemToolTip { get; } = "選択中のフォルダにアイテムを作成します。";

        // アプリケーションを終了します。
        public virtual string ExitToolTip { get; } = "アプリケーションを終了します。";
        // タグを編集します。
        public virtual string EditTagToolTip { get; } = "タグを編集します。";

        // 選択したタグを削除します。
        public virtual string DeleteSelectedTag { get; } = "選択したタグを削除";


        // --- 画面タイトル ---

        // 自動処理ルール一覧
        public virtual string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // 自動処理ルール編集
        public virtual string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }
        // Pythonスクリプト一覧
        public virtual string ListPythonScriptWindowTitle {
            get {
                return $"{AppName} - {ListPythonScript}";
            }
        }

        // RAGソース(git)編集
        public virtual string EditGitRagSourceWindowTitle {
            get {
                return $"{AppName} - {EditGitRagSource}";
            }
        }
        // RAGソース一覧
        public virtual string ListGitRagSourceWindowTitle {
            get {
                return $"{AppName} - RAGソース(git)一覧";
            }
        }
        // ベクトルDB一覧
        public virtual string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }
        // ベクトルDB編集
        public virtual string EditVectorDBWindowTitle {
            get {
                return $"{AppName} - {EditVectorDB}";
            }
        }
        // コミット選択
        public virtual string SelectCommitWindowTitle {
            get {
                return $"{AppName} - コミット選択";
            }
        }
        // QAチャット
        public virtual string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // タグ一覧
        public virtual string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // ログ表示
        public virtual string LogWindowTitle {
            get {
                return $"{AppName} - ログ表示";
            }
        }
        // スクリーンショットチェック用プロンプト生成
        public virtual string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - スクリーンショットチェック用プロンプト生成";
            }
        }

        // --- namespace WpfAppCommon.PythonIF ---

        // --- DefaultClipboardController.cs ---
        // クリップボードの内容が変更されました
        // --- EmptyPythonFunctions.cs ---
        // Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。
        public virtual string PythonNotEnabledMessage { get; } = "Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。";

        // --- PythonExecutor.cs ---
        // カスタムPythonスクリプトの、templateファイル
        public virtual string TemplateScript { get; } = "python/script_template.py";

        // クリップボードアプリ用のPythonスクリプト
        public virtual string WpfAppCommonUtilsScript { get; } = "python/ai_app.py";

        // テンプレートファイルが見つかりません
        public virtual string TemplateScriptNotFound { get; } = "テンプレートファイルが見つかりません";

        // --- PythonNetFunctions.cs ---
        // "PythonDLLが見つかりません。PythonDLLのパスを確認してください:"
        public virtual string PythonDLLNotFound { get; } = "PythonDLLが見つかりません。PythonDLLのパスを確認してください:";
        //  "Pythonの初期化に失敗しました。"
        public virtual string PythonInitFailed { get; } = "Pythonの初期化に失敗しました。";

        // "Pythonスクリプトファイルに、{function_name}関数が見つかりません"
        public virtual string FunctionNotFound(string function_name) {
            return $"Pythonスクリプトファイルに、{function_name}関数が見つかりません";
        }
        // "Pythonスクリプトの実行中にエラーが発生しました
        public virtual string PythonExecuteError { get; } = "Pythonスクリプトの実行中にエラーが発生しました";

        // "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。
        public virtual string ModuleNotFound { get; } = "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。";

        // $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        public virtual string PythonExecuteErrorDetail(Exception e) {
            return $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        }
        // "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください"
        public virtual string SpacyModelNameNotSet { get; } = "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください";

        // "マスキング結果がありません"
        public virtual string MaskingResultNotFound { get; } = "マスキング結果がありません";

        // "マスキングした文字列取得に失敗しました"
        public virtual string MaskingResultFailed { get; } = "マスキングした文字列取得に失敗しました";

        // "マスキング解除結果がありません"
        public virtual string UnmaskingResultNotFound { get; } = "マスキング解除結果がありません";
        // "マスキング解除した文字列取得に失敗しました"
        public virtual string UnmaskingResultFailed { get; } = "マスキング解除した文字列取得に失敗しました";

        // "画像のバイト列に変換できません"
        public virtual string ImageByteFailed { get; } = "画像のバイト列に変換できません";

        // "VectorDBItemsが空です"
        public virtual string VectorDBItemsEmpty { get; } = "VectorDBItemsが空です";

        // "OpenAIの応答がありません"
        public virtual string OpenAIResponseEmpty { get; } = "OpenAIの応答がありません";

        // ファイルが存在しません
        public virtual string FileNotFound { get; } = "ファイルが存在しません";

        // -- ClipboardApp.MainWindowDataGrid1 --
        // 更新日
        public virtual string UpdateDate { get; } = "更新日";

        // 作成日
        public virtual string CreateDate { get; } = "作成日";

        // ベクトル化日時
        public virtual string VectorizedDate { get; } = "ベクトル化日時";
        // タイトル
        public virtual string Title { get; } = "タイトル";

        // ソースタイトル
        public virtual string SourceTitle { get; } = "ソースタイトル";
        // ピン留め
        public virtual string Pin { get; } = "ピン留め";

        // 種別
        public virtual string Type { get; } = "種別";

        // -- AutoProcessRule --
        // 自動処理ルール

        // ルール名
        public virtual string RuleName { get; } = "ルール名";

        // 有効
        public virtual string Enable { get; } = "有効";

        // 適用対象フォルダ
        public virtual string TargetFolder { get; } = "適用対象フォルダ";

        // すべてのアイテムに適用
        public virtual string ApplyAllItems { get; } = "すべてのアイテムに適用";

        // 次の条件に合致するアイテムに適用
        public virtual string ApplyMatchedItems { get; } = "次の条件に合致するアイテムに適用";

        // アイテムの種類
        public virtual string ItemType { get; } = "アイテムの種類";
        // アイテムのタイプがテキストの場合
        public virtual string ItemTypeText { get; } = "アイテムのタイプがテキストの場合";
        // 行以上
        public virtual string LineOrMore { get; } = "行以上";

        // 行以下のテキスト
        public virtual string LineOrLess { get; } = "行以下のテキスト";

        // アイテムのタイプがファイルの場合
        public virtual string ItemTypeFile { get; } = "アイテムのタイプがファイルの場合";

        // アイテムのタイプがイメージの場合
        public virtual string ItemTypeImage { get; } = "アイテムのタイプがイメージの場合";

        // タイトルに次の文字が含まれる場合
        public virtual string TitleContains { get; } = "タイトルに次の文字が含まれる場合";

        // 本文に次の文字列が含まれる場合
        public virtual string BodyContains { get; } = "本文に次の文字列が含まれる場合";

        // ソースアプリの名前に次の文字列が含まれる場合
        public virtual string SourceAppContains { get; } = "ソースアプリの名前に次の文字列が含まれる場合";

        // 実行する処理
        public virtual string ExecuteProcess { get; } = "実行する処理";

        // 次の処理を実行する
        public virtual string ExecuteNextProcess { get; } = "次の処理を実行する";

        // コピー/移動/マージ先
        public virtual string CopyMoveMergeTarget { get; } = "コピー/移動/マージ先";

        // Pythonスクリプトを実行する
        public virtual string ExecutePythonScript { get; } = "Pythonスクリプトを実行する";

        // OpenAIのプロンプトを実行する
        public virtual string ExecuteOpenAI { get; } = "OpenAIのプロンプトを実行する";

        // OpenAIの実行モード
        public virtual string OpenAIMode { get; } = "OpenAIの実行モード";

        // ベクトルDBに格納する
        public virtual string StoreVectorDB { get; } = "ベクトルDBに格納する";

        // 適用対象フォルダ(パス)
        public virtual string TargetFolderFullPath { get; } = "適用対象フォルダ(パス)";

        // フォルダ単位
        public virtual string FolderUnit { get; } = "フォルダ単位";

        // 上へ
        public virtual string Up { get; } = "上へ";
        // 下へ
        public virtual string Down { get; } = "下へ";

        // しない
        public virtual string DoNot { get; } = "しない";


        // -- FolderEditWindow --
        // クリップボードフォルダ編集
        public virtual string EditClipboardFolder { get; } = "クリップボードフォルダ編集";

        // 自動処理時の設定
        public virtual string AutoProcessSetting { get; } = "自動処理時の設定";

        // 別のプロンプトの結果を入力にする
        public virtual string PromptResultToInput { get; } = "別のプロンプト結果を入力にする";

        // 入力となるプロンプトテンプレート
        public virtual string PromptInputName { get; } = "入力となるプロンプトテンプレート";
        // ベクトルDBを使用する
        public virtual string UseVectorDB { get; } = "ベクトルDBを使用する";

        // 出力形式
        public virtual string OutputType { get; } = "出力形式";
        // 新規タブ
        public virtual string NewTab { get; } = "新規タブ";
        // 本文を上書き
        public virtual string OverwriteContent { get; } = "本文を上書き";
        // タイトルを上書き
        public virtual string OverwriteTitle { get; } = "タイトルを上書き";

        // フォルダ選択
        public virtual string SelectFolder { get; } = "フォルダ選択";

        // ファイル選択
        public virtual string SelectFile { get; } = "ファイル選択";

        // SelectClipboardFolder
        public virtual string SelectClipboardFolder { get; } = "クリップボードフォルダ選択";

        // SelectedClipboardFolder
        public virtual string SelectedClipboardFolder { get; } = "選択したクリップボードフォルダ";

        // -- EditItemWindow --
        // テキストをファイルとして開く
        public virtual string OpenTextAsFile { get; } = "テキストをファイルとして開く";

        // ファイルを開く
        public virtual string OpenFile { get; } = "ファイルを開く";

        // 新規ファイルとして開く
        public virtual string OpenAsNewFile { get; } = "新規ファイルとして開く";

        // フォルダを開く
        public virtual string OpenFolder { get; } = "フォルダを開く";

        // テキストを抽出
        public virtual string ExtractText { get; } = "テキストを抽出";

        // 画像を開く
        public virtual string OpenImage { get; } = "画像を開く";

        // 画像からテキストを抽出
        public virtual string ExtractTextFromImage { get; } = "画像からテキストを抽出";

        // データをマスキング
        public virtual string MaskData { get; } = "データをマスキング";

        // ここをクリックするとタグ編集画面が開きます
        public virtual string ClickHereToOpenTheTagEditScreen { get; } = "ここをクリックするとタグ編集画面が開きます";

        // Properties
        public virtual string Properties { get; } = "プロパティ";

        // テキスト
        public virtual string Text { get; } = "テキスト";


        // ファイルパス
        public virtual string FilePath { get; } = "ファイルパス";

        // フォルダ
        public virtual string Folder { get; } = "フォルダ";

        // ファイル名
        public virtual string FileName { get; } = "ファイル名";

        // フォルダ名とファイル名
        public virtual string FolderNameAndFileName { get; } = "フォルダ名とファイル名";

        // イメージ
        public virtual string Image { get; } = "イメージ";

        // -- EditPythonScriptWindow --
        // 内容
        public virtual string Content { get; } = "内容";

        // -- ListPythonScriptWindow --
        // 新規Pythonスクリプト
        public virtual string NewPythonScript { get; } = "新規Pythonスクリプト";

        // -- SearchWindow --
        // 検索対象
        public virtual string SearchTarget { get; } = "検索対象";

        // 検索条件
        public virtual string SearchCondition { get; } = "検索条件";

        // 全フォルダ
        public virtual string AllFolders { get; } = "全フォルダ";

        // 検索対象フォルダ
        public virtual string SearchTargetFolder { get; } = "検索対象フォルダ";

        // 除外
        public virtual string Exclude { get; } = "除外";

        // コピー元アプリ名
        public virtual string CopySourceAppName { get; } = "コピー元アプリ名";

        // 開始日
        public virtual string StartDate { get; } = "開始日";

        // 終了日
        public virtual string EndDate { get; } = "終了日";

        // 適用対象配下のフォルダも対象にする
        public virtual string IncludeSubfolders { get; } = "適用対象配下のフォルダも対象にする";

        // -- TagSearchWindow
        // タグ検索
        public virtual string TagSearch { get; } = "タグ検索";

        // -- VectorSearchResultWindow
        // ベクトル検索結果
        public virtual string VectorSearchResult { get; } = "ベクトル検索結果";

        // -- ImageChatWindow
        // 設定項目
        public virtual string SettingItem { get; } = "設定項目";

        // 設定値
        public virtual string SettingValue { get; } = "設定値";

        // チェックタイプ
        public virtual string CheckType { get; } = "チェックタイプ";


        // -- ImageCheck.MainWindow --
        // 画像ファイル選択
        public virtual string SelectImageFile { get; } = "画像ファイル選択";

        // 画像エビデンスチェック項目編集
        public virtual string EditImageEvidenceCheckItem { get; } = "画像エビデンスチェック項目編集";

        // ここに回答が表示されます
        public virtual string TheAnswerWillBeDisplayedHere { get; } = "ここに回答が表示されます";

        // ここに質問を入力
        public virtual string EnterYourQuestionHere { get; } = "ここに質問を入力";

        // -- ListVectorDBWindow --
        // システム用のベクトルを表示
        public virtual string DisplayVectorsForTheSystem { get; } = "システム用のベクトルを表示";
        // システム用のプロンプトを表示
        public virtual string DisplayPromptsForTheSystem { get; } = "システム用のプロンプトを表示";

        // ベクトルDBの場所
        public virtual string VectorDBLocation { get; } = "ベクトルDBの場所";

        // ベクトルDBのタイプ
        public virtual string VectorDBType { get; } = "ベクトルDBのタイプ";

        // 新規ベクトルDB設定
        public virtual string NewVectorDBSetting { get; } = "新規ベクトルDB設定";

        // ベクトルDB設定編集
        public virtual string EditVectorDBSetting { get; } = "ベクトルDB設定編集";

        // -- QAChatControl --
        // 実験的機能1(文章解析+辞書生成+RAG)"
        public virtual string ExperimentalFunction1 { get; } = "実験的機能1(文章解析+辞書生成+RAG)";

        // ベクトルDB(フォルダ)
        public virtual string VectorDBFolder { get; } = "ベクトルDB(フォルダ)";

        // ここをクリックしてベクトルDB(フォルダ)を追加
        public virtual string ClickHereToAddVectorDBFolder { get; } = "ここをクリックしてベクトルDB(フォルダ)を追加";

        // ベクトルDB選択
        public virtual string SelectVectorDB { get; } = "ベクトルDB選択";

        // VectorDBSearchAgent ベクトルDB検索用のエージェント
        public virtual string VectorDBSearchAgent { get; } = "ベクトルDB検索用のエージェント";


        // リストから除外
        public virtual string ExcludeFromList { get; } = "リストから除外";

        // ベクトルDB(外部)
        public virtual string VectorDB { get; } = "ベクトルDB";

        // ベクトルDB(外部)
        public virtual string VectorDBExternal { get; } = "ベクトルDB(外部)";

        // CollectionName
        public virtual string CollectionName { get; } = "コレクション名";

        // ここをクリックしてベクトルDBを追加
        public virtual string ClickHereToAddVectorDB { get; } = "ここをクリックしてベクトルDBを追加";

        // ベクトルDB検索結果の最大値
        public virtual string VectorDBSearchResultMax { get; } = "ベクトルDB検索結果の最大値";

        // 画像アイテム
        public virtual string AdditionalItem { get; } = "追加アイテム";

        // ここをクリックして選択中のアイテムを貼り付け
        public virtual string ClickHereToPasteTheSelectedItem { get; } = "ここをクリックして選択中のアイテムを貼り付け";

        // 画像ファイル
        public virtual string ImageFile { get; } = "画像ファイル";

        // ここをクリックして画像ファイルを追加
        public virtual string ClickHereToAddImageFile { get; } = "ここをクリックして画像ファイルを追加";

        // チャット
        public virtual string Chat { get; } = "チャット";

        // プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public virtual string PromptTemplate { get; } = "プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。";


        // リクエストパラメーター(JSON)
        public virtual string RequestParameterJson { get; } = "リクエストパラメーター(JSON)";

        // --- ClipboardFolderViewModel ---
        // 自動処理が設定されています
        public virtual string AutoProcessingIsSet { get; } = "自動処理が設定されています";



        // フォルダを編集しました
        public virtual string FolderEdited { get; } = "フォルダを編集しました";

        // リロードしました
        public virtual string Reloaded { get; } = "リロードしました";

        // ファイルを選択してください
        public virtual string SelectFilePlease { get; } = "ファイルを選択してください";

        // フォルダを選択してください
        public virtual string SelectFolderPlease { get; } = "フォルダを選択してください";

        // フォルダをエクスポートしました
        public virtual string FolderExported { get; } = "フォルダをエクスポートしました";

        // フォルダをインポートしました
        public virtual string FolderImported { get; } = "フォルダをインポートしました";

        // ルートフォルダは削除できません
        public virtual string RootFolderCannotBeDeleted { get; } = "ルートフォルダは削除できません";

        // "フォルダを削除しますか？"
        public virtual string ConfirmDeleteFolder { get; } = "フォルダを削除しますか？";

        // "フォルダを削除しました"
        public virtual string FolderDeleted { get; } = "フォルダを削除しました";

        // "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?"
        public virtual string ConfirmDeleteItems { get; } = "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?";

        // アイテムを削除しました
        public virtual string DeletedItems { get; } = "アイテムを削除しました";

        // "追加しました"
        public virtual string Added { get; } = "追加しました";

        // "編集しました"
        public virtual string Edited { get; } = "編集しました";

        // 貼り付けました
        public virtual string Pasted { get; } = "貼り付けました";

        // エラーが発生しました。\nメッセージ
        public virtual string ErrorOccurredAndMessage { get; } = "エラーが発生しました。\nメッセージ";

        // スタックトレース
        public virtual string StackTrace { get; } = "スタックトレース";

        // チャット結果を保存しました
        public virtual string SavedChatResult { get; } = "チャット結果を保存しました";

        // --- ClipboardItemViewModelCommands ---



        // "MainWindowViewModelがNullです"
        public virtual string MainWindowViewModelIsNull { get; } = "MainWindowViewModelがNullです";


        // 背景情報
        public virtual string BackgroundInformation { get; } = "背景情報";

        // 背景情報を生成します
        public virtual string GenerateBackgroundInformation { get; } = "背景情報を生成します";

        // "背景情報を生成しました"
        public virtual string GeneratedBackgroundInformation { get; } = "背景情報を生成しました";

        // "サマリーを生成します"
        public virtual string GenerateSummary2 { get; } = "サマリーを生成します";

        // "サマリーを生成しました"
        public virtual string GeneratedSummary { get; } = "サマリーを生成しました";

        // 課題リスト
        public virtual string TasksList { get; } = "課題リスト";

        // "課題リストを生成します"
        public virtual string GenerateTasks { get; } = "課題リストを生成します";

        // "課題リストを生成しました"
        public virtual string GeneratedTasks { get; } = "課題リストを生成しました";

        // 文書信頼度をチェックします
        public virtual string CheckDocumentReliability { get; } = "文書信頼度をチェックします";

        // "文書信頼度をチェックしました"
        public virtual string CheckedDocumentReliability { get; } = "文書信頼度をチェックしました";


        // その他のプロンプト
        public virtual string OtherPrompts { get; } = "その他のプロンプト";


        // 画像以外のコンテンツはテキストを抽出できません
        public virtual string CannotExtractTextForNonImageContent { get; } = "画像以外のコンテンツはテキストを抽出できません";

        // "数値を入力してください。"
        public virtual string EnterANumber { get; } = "数値を入力してください。";

        // フォルダが選択されていません。
        public virtual string FolderNotSelected { get; } = "フォルダが選択されていません。";

        // ルール名を入力してください。
        public virtual string EnterRuleName { get; } = "ルール名を入力してください。";

        // "アクションを選択してください。"
        public virtual string SelectAction { get; } = "アクションを選択してください。";

        // "編集対象のルールが見つかりません。"
        public virtual string RuleNotFound { get; } = "編集対象のルールが見つかりません。";

        // コピーまたは移動先のフォルダを選択してください。
        public virtual string SelectCopyOrMoveTargetFolder { get; } = "コピーまたは移動先のフォルダを選択してください。";

        // "同じフォルダにはコピーまたは移動できません。"
        public virtual string CannotCopyOrMoveToTheSameFolder { get; } = "同じフォルダにはコピーまたは移動できません。";

        // "コピー/移動処理の無限ループを検出しました。"
        public virtual string DetectedAnInfiniteLoopInCopyMoveProcessing { get; } = "コピー/移動処理の無限ループを検出しました。";

        // "PromptTemplateを選択してください。"
        public virtual string SelectPromptTemplate { get; } = "PromptTemplateを選択してください。";

        // PythonScriptを選択してください。
        public virtual string SelectPythonScript { get; } = "PythonScriptを選択してください。";

        
        // 標準フォルダ以外にはコピーまたは移動できません。
        public virtual string CannotCopyOrMoveToNonStandardFolders { get; } = "標準フォルダ以外にはコピーまたは移動できません。";

        // RootFolderViewModelがNullです。
        public virtual string RootFolderViewModelIsNull { get; } = "RootFolderViewModelがNullです。";


        // --- EditPythonScriptWindowViewModel ---
        // 説明を入力してください
        public virtual string EnterDescription { get; } = "説明を入力してください";

        // --- FolderEditWindowViewModel ---
        // フォルダが指定されていません
        public virtual string FolderNotSpecified { get; } = "フォルダが指定されていません";

        // フォルダ名を入力してください
        public virtual string EnterFolderName { get; } = "フォルダ名を入力してください";

        // --- FolderSelectWindowViewModel ---
        // "エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない
        public virtual string FolderSelectWindowViewModelInstanceNotFound { get; } = "エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない";

        // エラーが発生しました。選択中のフォルダがない
        public virtual string SelectedFolderNotFound { get; } = "エラーが発生しました。選択中のフォルダがない";

        // --- ListAutoProcessRuleWindowViewModel ---
        // 自動処理ルールが選択されていません。
        public virtual string AutoProcessRuleNotSelected { get; } = "自動処理ルールが選択されていません。";

        // を削除しますか？
        public virtual string ConfirmDelete { get; } = "を削除しますか？";

        // "システム共通設定を保存しました。"
        public virtual string SavedSystemCommonSettings { get; } = "システム共通設定を保存しました。";

        // "システム共通設定の変更はありません。"
        public virtual string NoChangesToSystemCommonSettings { get; } = "システム共通設定の変更はありません。";

        // --- ListPythonScriptWindowViewModel ---

        // スクリプトを選択してください
        public virtual string SelectScript { get; } = "スクリプトを選択してください";

        // --- SearchWindowViewModel ---
        // 検索フォルダ
        public virtual string SearchFolder { get; } = "検索フォルダ";

        // 検索フォルダ 英語名
        public virtual string SearchFolderEnglish { get; } = "SearchFolder";



        // 標準
        public virtual string Standard { get; } = "標準";

        // SearchConditionRuleがNullです
        public virtual string SearchConditionRuleIsNull { get; } = "SearchConditionRuleがNullです";

        // 検索条件がありません
        public virtual string NoSearchConditions { get; } = "検索条件がありません";

        // --- TagSearchWindowViewModel ---
        // "タグが空です"
        public virtual string TagIsEmpty { get; } = "タグが空です";

        // "タグが既に存在します"
        public virtual string TagAlreadyExists { get; } = "タグが既に存在します";

        // バージョン情報
        public virtual string VersionInformation { get; } = "バージョン情報";


        #region  ClipboardItemViewModelCommands
        // **********************************************************************************
        // -- ExtractTextCommand


        // マージチャット 事前処理実行中
        public virtual string MergeChatPreprocessingInProgress { get; } = "マージチャット 事前処理実行中";
        // テキスト抽出処理が完了しました
        public virtual string TextExtractionCompleted { get; } = "テキスト抽出処理が完了しました";


        // タイトル生成処理実行中
        public virtual string TitleGenerationInProgress { get; } = "タイトル生成処理実行中";

        // "タイトルを生成しました"
        public virtual string TitleGenerationCompleted { get; } = "タイトルを生成しました";

        // ベクトルを生成します
        public virtual string GenerateVectorInProgress { get; } = "ベクトル生成処理実行中";





        // **********************************************************************************
        #endregion

        // "アプリケーションを再起動すると、表示モードが変更されます。"
        public virtual string DisplayModeWillChangeWhenYouRestartTheApplication { get; } = "アプリケーションを再起動すると、表示モードが変更されます。";
        // 情報
        public virtual string Information { get; } = "情報";

        // "終了しますか?"
        public virtual string ConfirmExit { get; } = "終了しますか?";



        // 切り取りました"
        public virtual string Cut { get; } = "切り取りました";

        // コピーしました"
        public virtual string Copied { get; } = "コピーしました";

        // 貼り付け先のフォルダがない
        public virtual string NoPasteFolder { get; } = "貼り付け先のフォルダがない";

        // "コピー元のフォルダがない"
        public virtual string NoCopyFolder { get; } = "コピー元のフォルダがない";

        // "選択中のアイテムを削除しますか?"
        public virtual string ConfirmDeleteSelectedItems { get; } = "選択中のアイテムを削除しますか?";

        // --- ImageCHat ---
        // 画像を確認して以下の各文が正しいか否かを教えてください\n\n
        public virtual string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "画像を確認して以下の各文が正しいか否かを教えてください\n\n";

        // 画像ファイルが選択されていません。
        public virtual string NoImageFileSelected { get; } = "画像ファイルが選択されていません。";

        // プロンプトを送信します
        public virtual string SendPrompt { get; } = "プロンプトを送信します";
        // 画像ファイル名
        public virtual string ImageFileName { get; } = "画像ファイル名";

        // エラーが発生しました。
        public virtual string ErrorOccurred { get; } = "エラーが発生しました。";

        // 画像ファイルを選択してください
        public virtual string SelectImageFilePlease { get; } = "画像ファイルを選択してください";

        // すべてのファイル
        public virtual string AllFiles { get; } = "すべてのファイル";

        // "ファイルが存在しません。"
        public virtual string FileDoesNotExist { get; } = "ファイルが存在しません。";

        // -- EditPromptItemWindowViewModel --
        // プロンプト編集
        public virtual string EditPrompt { get; } = "プロンプト編集";

        // -- ListPromptTemplateWindow -- 
        // 新規プロンプトテンプレート
        public virtual string NewPromptTemplate { get; } = "新規プロンプトテンプレート";

        // RAG
        public virtual string RAG { get; } = "RAG";

        // -- DevFeatures.cs
        // テキスト以外のコンテンツはマスキングできません
        public virtual string CannotMaskNonTextContent { get; } = "テキスト以外のコンテンツはマスキングできません";

        // データをマスキングしました
        public virtual string MaskedData { get; } = "データをマスキングしました";

        // マスキングデータをもとに戻します
        public virtual string RestoreMaskingData { get; } = "マスキングデータをもとに戻します";

        // 画像が取得できません
        public virtual string CannotGetImage { get; } = "画像が取得できません";


        // -- ScriptAutoProcessItem.cs --
        // Pythonスクリプトを実行しました
        public virtual string ExecutedPythonScript { get; } = "Pythonスクリプトを実行しました";

        // -- SystemAutoProcessItem.cs --
        // AutoProcessItemが見つかりません
        public virtual string AutoProcessItemNotFound { get; } = "AutoProcessItemが見つかりません";

        // -- EnumDescription.cs --

        // Enum型ではありません
        public virtual string NotEnumType { get; } = "Enum型ではありません";

        // --- WindowsNotificationController.cs ---
        // アクセス拒否
        public virtual string AccessDenied { get; } = "アクセス拒否";

        // --- EditPromptItemWindowViewModel ---
        // 名前を入力してください
        public virtual string EnterName { get; } = "名前を入力してください";

        // --- EditRAGSourceWindowViewModel ---
        // RAGソース編集
        public virtual string EditRAGSource { get; } = "RAGソース編集";

        // ItemViewModelがnullです"
        public virtual string ItemViewModelIsNull { get; } = "ItemViewModelがnullです";

        // EditVectorDBWindowViewModel
        // Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません
        public virtual string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません";

        // プロンプトテンプレート一覧
        public virtual string PromptTemplateList { get; } = "プロンプトテンプレート一覧";

        // プロンプトテンプレートが選択されていません
        public virtual string NoPromptTemplateSelected { get; } = "プロンプトテンプレートが選択されていません";

        // ListVectorDBWindowViewModel
        // 編集するベクトルDBを選択してください
        public virtual string SelectVectorDBToEdit { get; } = "編集するベクトルDBを選択してください";

        // 削除するベクトルDBを選択してください
        public virtual string SelectVectorDBToDelete { get; } = "削除するベクトルDBを選択してください";

        // 選択中のベクトルDBを削除しますか？
        public virtual string ConfirmDeleteSelectedVectorDB { get; } = "選択中のベクトルDBを削除しますか？";

        // ベクトルDBを選択してください
        public virtual string SelectVectorDBPlease { get; } = "ベクトルDBを選択してください";

        // RAGManagementWindowViewModel
        // 編集するRAG Sourceを選択してください
        public virtual string SelectRAGSourceToEdit { get; } = "編集するRAG Sourceを選択してください";

        // 削除するRAG Sourceを選択してください
        public virtual string SelectRAGSourceToDelete { get; } = "削除するRAG Sourceを選択してください";

        // 選択中のRAG Sourceを削除しますか？
        public virtual string ConfirmDeleteSelectedRAGSource { get; } = "選択中のRAG Sourceを削除しますか？";

        // コミットを選択してください
        public virtual string SelectCommitPlease { get; } = "コミットを選択してください";

        // --- UpdateRAGIndexWindowViewModel
        // インデックス化対象ファイル
        public virtual string IndexingTargetFile { get; } = "インデックス化対象ファイル";

        // ファイル追加
        public virtual string AddFile { get; } = "ファイル追加";
        // ファイル削除
        public virtual string DeleteFile { get; } = "ファイル削除";

        // ファイル更新
        public virtual string UpdateFile { get; } = "ファイル更新";

        // 対象ファイル取得
        public virtual string GetTargetFile { get; } = "対象ファイル取得";

        // インデックス作成
        public virtual string CreateIndex { get; } = "インデックス作成";

        //  "戻る"
        public virtual string Back { get; } = "戻る";

        // RAGSourceItemViewModelが設定されていません
        public virtual string RAGSourceItemViewModelNotSet { get; } = "RAGSourceItemViewModelが設定されていません";

        // "開始コミットを指定してください"
        public virtual string SpecifyStartCommit { get; } = "開始コミットを指定してください";

        // "対象を選択してください"
        public virtual string SelectTarget { get; } = "対象を選択してください";

        // 処理ファイル数
        public virtual string ProcessedFileCount { get; } = "処理ファイル数";

        // インデックス作成中
        public virtual string CreatingIndex { get; } = "インデックス作成中";

        // 完了
        public virtual string Completed { get; } = "完了";

        // 未対応ファイルタイプのためスキップ
        public virtual string SkipUnsupportedFileType { get; } = "未対応ファイルタイプのためスキップ";

        // 失敗
        public virtual string Failed { get; } = "失敗";

        // "インデックス作成が完了しました"
        public virtual string IndexCreationCompleted { get; } = "インデックス作成が完了しました";

        // インデックス作成処理を中断しました
        public virtual string IndexCreationInterrupted { get; } = "インデックス作成処理を中断しました";

        // チャットの送信に失敗しました。
        public virtual string FailedToSendChat { get; } = "チャットの送信に失敗しました。";


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

        // 統計
        public virtual string Statistics { get; } = "統計";

        // --- ClipboardAppVectorDBItem

        // --- ClipboardFolder.cs ---
        // クリップボード
        public virtual string Clipboard { get; } = "クリップボード";

        // クリップボード 英語名
        public virtual string ClipboardEnglish { get; } = "Clipboard";


        // チャット履歴
        public virtual string ChatHistory { get; } = "チャット履歴";

        // チャット履歴 英語名
        public virtual string ChatHistoryEnglish { get; } = "ChatHistory";



        // 自動処理でアイテムが削除または移動されました
        public virtual string ItemsDeletedOrMovedByAutoProcessing { get; } = "自動処理でアイテムが削除または移動されました";

        
        // 自動処理を適用します
        public virtual string ApplyAutoProcessing { get; } = "自動処理を適用します";

        // 自動処理を適用しました
        public virtual string AutoProcessingApplied { get; } = "自動処理を適用しました";

        // 自動処理でアイテムが削除されました
        public virtual string ItemsDeletedByAutoProcessing { get; } = "自動処理でアイテムが削除されました";


        // 作成日時
        public virtual string CreationDateTime { get; } = "作成日時";

        // ソースアプリ名
        public virtual string SourceAppName { get; } = "ソースアプリ名";

        // ピン留めしてます
        public virtual string Pinned { get; } = "ピン留めしてます";

        // フォルダを取得できません
        public virtual string CannotGetFolder { get; } = "フォルダを取得できません";

        // サマリー
        public virtual string Summary { get; } = "サマリー";


        // --- TextSelector.cs ---
        // ファイルを実行できませんでした
        public virtual string FailedToRunFile { get; } = "ファイルを実行できませんでした";

        // テキストファイルとして開きます。
        public virtual string OpenAsTextFile { get; } = "テキストファイルとして開きます。";

        // --- EditChatItem ---
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


        // 例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。
        public virtual string ExampleGeneralVectorDB { get; } = "例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";


        // ドキュメントのチャンクサイズ
        public virtual string DocumentChunkSize { get; } = "ドキュメントのチャンクサイズ";

        // ベクトル検索結果の上限値
        public virtual string DefaultSearchResultLimit { get; } = "ベクトル検索結果のデフォルト上限値";

        // MultiVectorRetrieverを使用
        public virtual string UseMultiVectorRetriever { get; } = "MultiVectorRetrieverを使用";

        // DocStore用のSQLite3の場所
        public virtual string SQLite3LocationForDocStore { get; } = "DocStore用のSQLite3の場所";

        // 例：sqlite:///C:\Users\Username\sqlite3.db
        public virtual string ExampleSQLite3Location { get; } = "例：sqlite:///C:\\Users\\Username\\sqlite3.db";

        // マルチベクターリトリーバのドキュメントのチャンクサイズ
        public virtual string DocumentChunkSizeForMultiVectorRetriever { get; } = "マルチベクターリトリーバのドキュメントのチャンクサイズ";

        // 例：C:\Users\Username\vector.db
        public virtual string ExampleVectorDBLocationChroma { get; } = "例：C:\\Users\\Username\\vector.db";

        // 例：postgresql+psycopg://langchain:langchain@localhost:5432/langchain
        public virtual string ExampleVectorDBLocationPostgres { get; } = "例：postgresql+psycopg://langchain:langchain@localhost:5432/langchain";

        // チャット履歴をクリア
        public virtual string ClearChatContents { get; } = "チャット内容をクリア";

        // 本文をクリア
        public virtual string ClearContent { get; } = "本文をクリア";

        // 本文を再読み込み
        public virtual string ReloadContent { get; } = "本文を再読み込み";

        // 抽出したテキスト
        public virtual string ExtractedText { get; } = "抽出したテキスト";

        // タブ削除
        public virtual string DeleteTab { get; } = "タブ削除";

        // デバッグ用のコマンド生成
        public virtual string GenerateDebugCommand { get; } = "デバッグ用のコマンド生成";

        // ExecuteDebugCommand
        public virtual string ExecuteDebugCommand { get; } = "デバッグ用のコマンドを実行";

        // VectorDBSettings
        public virtual string VectorDBSettings { get; } = "ベクトルDB設定";

        // ファイルの内容を表示するには「テキストを抽出」を実行してください
        public virtual string ExecuteExtractTextToViewFileContent { get; } = "ファイルの内容を表示するには「テキストを抽出」を実行してください";

        // Webページの内容を表示するには「Webページをダウンロード」を実行してください。
        public virtual string ExecuteDownloadWebPageToViewContent { get; } = "Webページの内容を表示するには「Webページをダウンロード」を実行してください。";

        // チャット：
        public virtual string ChatHeader { get; } = "チャット：";
        // タイトルなし
        public virtual string NoTitle { get; } = "タイトルなし";
    }
}
