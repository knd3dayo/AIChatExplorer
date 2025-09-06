namespace LibUIMain.Resource {
    public partial class CommonStringResourcesJa : CommonStringResources {


        #region --- 共通 ------------------------------------------------------ 
        // ファイル
        public override string File { get; } = "ファイル";

        // ファイル/画像
        public override string FileOrImage { get; } = "ファイル/画像";

        // チャット内容
        public override string ChatContent { get; } = "チャット内容";
        // 作成
        public override string Create { get; } = "作成";
        // アイテム作成
        public override string CreateItem { get; } = "アイテム作成";

        // フォルダ作成
        public override string CreateFolder { get; } = "フォルダ作成";

        // 終了
        public override string Exit { get; } = "終了";
        // 編集
        public override string Edit { get; } = "編集";

        // 開始
        public override string Start { get; } = "開始";
        // 停止
        public override string Stop { get; } = "停止";
        // 選択
        public override string Select { get; } = "選択";
        // ヘルプ
        public override string Help { get; } = "ヘルプ";
        // バージョン情報
        public override string VersionInfo { get; } = "バージョン情報";

        // プロパティ表示
        public override string ShowProperties { get; } = "プロパティ表示";

        // ItemSelection
        public override string ItemSelection { get; } = "アイテム選択";

        // TargetDataSelection
        public override string TargetDataSelection { get; } = "対象データ選択";

        // ReferenceItemList
        public override string ReferenceItemList { get; } = "参照アイテム一覧";

        // MarkdownView
        public override string MarkdownView { get; } = "マークダウン表示";


        // 表示
        public override string View { get; } = "表示";

        // 検索
        public override string Search { get; } = "検索";
        // 設定
        public override string Setting { get; } = "設定";
        // 削除
        public override string Delete { get; } = "削除";
        // "削除しました"
        public override string Deleted { get; } = "削除しました";

        // 追加
        public override string Add { get; } = "追加";
        // OK
        public override string OK { get; } = "OK";
        // キャンセル
        public override string Cancel { get; } = "キャンセル";
        // 閉じる
        public override string Close { get; } = "閉じる";

        // ショートカット登録
        public override string CreateShortCut { get; } = "ショートカット登録";

        // ロード
        public override string Load { get; } = "ロード";

        // 同期
        public override string Sync { get; } = "同期";

        // DownloadWebPage
        public override string DownloadWebPage { get; } = "Webページをダウンロード";

        // ExportImport
        public override string ExportImport { get; } = "エクスポート/インポート";

        // Export
        public override string ExportToExcel { get; } = "Excelへエクスポート";
        // Import
        public override string ImportFromExcel { get; } = "Excelからインポート";

        // Standard
        public override string Standard { get; } = "標準";

        // RAG
        public override string RAG { get; } = "RAG";

        // ImportFromRULList
        public override string ImportFromRULList { get; } = "URLリストからインポート";

        // モード
        public override string ChatMode { get; } = "チャットモード";

        // OpenAI
        public override string ChatMode_OpenAI { get; } = "OpenAI";

        // OpenAI_RAG
        public override string ChatMode_OpenAI_RAG { get; } = "OpenAI_RAG";

        // AutoGen_GroupChat
        public override string ChatMode_AutoGen_GroupChat { get; } = "AutoGen_GroupChat";

        // AutoGen_NormalChat
        public override string ChatMode_AutoGen_NormalChat { get; } = "AutoGen_NormalChat";

        // AutoGent_NestedChat
        public override string ChatMode_AutoGen_NestedChat { get; } = "AutoGen_NestedChat";

        // GroupChatName
        public override string GroupChatName { get; } = "グループチャット名";

        // Timeout
        public override string Timeout { get; } = "タイムアウト";

        // TerminateMessage
        public override string TerminateMessage { get; } = "終了メッセージ";

        // MaxMsg
        public override string MaxMsg { get; } = "最大メッセージ数";

        // 分割モード
        public override string SplitMode { get; } = "リクエスト分割モード";

        // 分割モード なし
        public override string SplitMode_None { get; } = "なし";

        // 分割モード 指定したトークン数を超える場合はリクエストを分割
        public override string SplitMode_SplitIfExceedSpecifiedToken { get; } = "指定したトークン数を超える場合はリクエストを分割";

        // 指定したトークン数を超える場合はリクエストを分割して要約n
        public override string SplitMode_SplitAndSummarizeIfExceedSpecifiedToken { get; } = "指定したトークン数を超える場合はリクエストを分割して要約";

        // PromptTextIsNeededWhenSplitModeIsEnabled
        public override string PromptTextIsNeededWhenSplitModeIsEnabled { get; } = "分割モードが有効な場合はプロンプトテキストが必要です";

        // RAGMode_None
        public override string RAGMode_None { get; } = "ベクトル検索情報は行わない";

        // RAGMode_NormalSearch
        public override string RAGMode_NormalSearch { get; } = "ベクトル検索情報を使用する";

        // RAGMode_PromptSearch
        public override string RAGMode_PromptSearch { get; } = "プロンプトを指定してベクトル検索を行う";

        // SendRelatedItemsOnlyFirstRequest_True
        public override string SendRelatedItemsOnlyFirstRequest_True { get; } = "関連アイテムを最初のリクエスト時のみ送信する";

        // SendRelatedItemsOnlyFirstRequest_False
        public override string SendRelatedItemsOnlyFirstRequest_False { get; } = "関連アイテムをすべてのリクエストで送信する";

        // リクエストを分割するトークン数
        public override string NumberOfTokensToSplitRequest { get; } = "リクエストを分割するトークン数";

        // DataGridIsNotFound
        public override string DataGridIsNotFound { get; } = "DataGridが見つかりません";

        // 貼り付け
        public override string Paste { get; } = "貼り付け";
        // 開く
        public override string Open { get; } = "開く";
        // 保存
        public override string Save { get; } = "保存";

        // 送信
        public override string Send { get; } = "送信";
        // Copy
        public override string Copy { get; } = "Copy";

        // すべて選択します。
        public override string SelectAll { get; } = "すべて選択";
        // すべて選択解除します。
        public override string UnselectAll { get; } = "すべて選択解除";

        // 名前
        public override string Name { get; } = "名前";

        // 説明
        public override string Description { get; } = "説明";
        // 文字列
        public override string StringType { get; } = "文字列";
        // リスト
        public override string ListType { get; } = "リスト";
        // テーブル
        public override string TableType { get; } = "テーブル";

        // Dictionary
        public override string DictionaryType { get; } = "ディクショナリ";
        // 出力先
        public override string OutputDestination { get; } = "出力先";

        // クリア
        public override string Clear { get; } = "クリア";

        // 確認
        public override string Confirm { get; } = "確認";
        // 実行
        public override string Execute { get; } = "実行";



        #endregion

        public override string PromptMenu { get; } = "プロンプトメニュー";
        
        // ベクトル生成
        public override string GenerateVector { get; } = "ベクトル生成";


        //ベクトル検索
        public override string VectorSearch { get; } = "ベクトル検索";

        // ベクトルDBのリフレッシュ
        public override string RefreshVectorDB { get; } = "ベクトルDBのリフレッシュ";


        // 自動処理ルール一覧
        public override string ListAutoProcessRule { get; } = "自動処理ルール一覧";
        // Pythonスクリプト一覧
        public override string ListPythonScript { get; } = "Pythonスクリプト一覧";

        // タグ一覧
        public override string ListTag { get; } = "タグ一覧";

        // 新規タグ
        public override string NewTag { get; } = "新規タグ";
        // タグ
        public override string Tag { get; } = "タグ";

        // ベクトルDB一覧
        public override string ListVectorDB { get; } = "ベクトルDB一覧";
        // ベクトルDB編集
        public override string EditVectorDB { get; } = "ベクトルDB編集";

        // --- ToolTip ---
        // 選択中のフォルダにアイテムを作成します。
        public override string CreateItemToolTip { get; } = "選択中のフォルダにアイテムを作成します。";

        // アプリケーションを終了します。
        public override string ExitToolTip { get; } = "アプリケーションを終了します。";
        // タグを編集します。
        public override string EditTagToolTip { get; } = "タグを編集します。";

        // 選択したタグを削除します。
        public override string DeleteSelectedTag { get; } = "選択したタグを削除";


        // --- 画面タイトル ---

        // 自動処理ルール一覧
        public override string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // 自動処理ルール編集
        public override string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }

        // ベクトルDB一覧
        public override string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }

        // QAチャット
        public override string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // タグ一覧
        public override string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // スクリーンショットチェック用プロンプト生成
        public override string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - スクリーンショットチェック用プロンプト生成";
            }
        }
        
        // --- PythonNetFunctions.cs ---
        // "PythonDLLが見つかりません。PythonDLLのパスを確認してください:"
        public override string PythonDLLNotFound { get; } = "PythonDLLが見つかりません。PythonDLLのパスを確認してください:";

        // 更新日
        public override string UpdateDate { get; } = "更新日";

        // 作成日
        public override string CreateDate { get; } = "作成日";

        // ベクトル化日時
        public override string VectorizedDate { get; } = "ベクトル化日時";
        // タイトル
        public override string Title { get; } = "タイトル";

        // ソースタイトル
        public override string SourceTitle { get; } = "ソースタイトル";
        // ピン留め
        public override string Pin { get; } = "ピン留め";

        // 種別
        public override string Type { get; } = "種別";

        // -- AutoProcessRule --
        // 自動処理ルール

        // ルール名
        public override string RuleName { get; } = "ルール名";

        // 有効
        public override string Enable { get; } = "有効";

        // 適用対象フォルダ
        public override string TargetFolder { get; } = "適用対象フォルダ";

        // すべてのアイテムに適用
        public override string ApplyAllItems { get; } = "すべてのアイテムに適用";

        // 次の条件に合致するアイテムに適用
        public override string ApplyMatchedItems { get; } = "次の条件に合致するアイテムに適用";

        // アイテムの種類
        public override string ItemType { get; } = "アイテムの種類";
        // アイテムのタイプがテキストの場合
        public override string ItemTypeText { get; } = "アイテムのタイプがテキストの場合";
        // 行以上
        public override string LineOrMore { get; } = "行以上";

        // 行以下のテキスト
        public override string LineOrLess { get; } = "行以下のテキスト";

        // アイテムのタイプがファイルの場合
        public override string ItemTypeFile { get; } = "アイテムのタイプがファイルの場合";

        // アイテムのタイプがイメージの場合
        public override string ItemTypeImage { get; } = "アイテムのタイプがイメージの場合";

        // タイトルに次の文字が含まれる場合
        public override string TitleContains { get; } = "タイトルに次の文字が含まれる場合";

        // 本文に次の文字列が含まれる場合
        public override string BodyContains { get; } = "本文に次の文字列が含まれる場合";

        // ソースアプリの名前に次の文字列が含まれる場合
        public override string SourceAppContains { get; } = "ソースアプリの名前に次の文字列が含まれる場合";

        // 実行する処理
        public override string ExecuteProcess { get; } = "実行する処理";

        // 次の処理を実行する
        public override string ExecuteNextProcess { get; } = "次の処理を実行する";

        // コピー/移動/マージ先
        public override string CopyMoveMergeTarget { get; } = "コピー/移動/マージ先";

        // Pythonスクリプトを実行する
        public override string ExecutePythonScript { get; } = "Pythonスクリプトを実行する";

        // OpenAIのプロンプトを実行する
        public override string ExecuteOpenAI { get; } = "OpenAIのプロンプトを実行する";

        // OpenAIの実行モード
        public override string OpenAIMode { get; } = "OpenAIの実行モード";


        // 適用対象フォルダ(パス)
        public override string TargetFolderFullPath { get; } = "適用対象フォルダ(パス)";

        // フォルダ単位
        public override string FolderUnit { get; } = "フォルダ単位";

        // 上へ
        public override string Up { get; } = "上へ";
        // 下へ
        public override string Down { get; } = "下へ";

        // しない
        public override string DoNot { get; } = "しない";


        // -- FolderEditWindow --
        // フォルダ編集
        public override string EditApplicationFolder { get; } = "フォルダ編集";

        // 自動処理時の設定
        public override string AutoProcessSetting { get; } = "自動処理時の設定";

        // 別のプロンプトの結果を入力にする
        public override string PromptResultToInput { get; } = "別のプロンプト結果を入力にする";

        // 入力となるプロンプトテンプレート
        public override string PromptInputName { get; } = "入力となるプロンプトテンプレート";
        // ベクトルDBを使用する
        public override string UseVectorDB { get; } = "ベクトルDBを使用する";

        // UseVectorDBSettingInFolder
        public override string UseVectorDBSettingInFolder { get; } = "フォルダ設定を使用";

        // 出力形式
        public override string OutputType { get; } = "出力形式";
        // 新規タブ
        public override string NewTab { get; } = "新規タブ";
        // 本文を上書き
        public override string OverwriteContent { get; } = "本文を上書き";
        // タイトルを上書き
        public override string OverwriteTitle { get; } = "タイトルを上書き";

        // タグ追加
        public override string AppendTags { get; } = "タグ追加";

        // フォルダ選択
        public override string SelectFolder { get; } = "フォルダ選択";

        // ファイル選択
        public override string SelectFile { get; } = "ファイル選択";

        // SelectApplicationFolder
        public override string SelectApplicationFolder { get; } = "フォルダ選択";

        // SelectedApplicationFolder
        public override string SelectedApplicationFolder { get; } = "選択したフォルダ";

        // -- EditItemWindow --
        // テキストをファイルとして開く
        public override string OpenTextAsFile { get; } = "テキストをファイルとして開く";

        // ファイルを開く
        public override string OpenFile { get; } = "ファイルを開く";

        // 新規ファイルとして開く
        public override string OpenAsNewFile { get; } = "新規ファイルとして開く";

        // フォルダを開く
        public override string OpenFolder { get; } = "フォルダを開く";

        // テキストを抽出
        public override string ExtractText { get; } = "テキストを抽出";

        // データをマスキング
        public override string MaskData { get; } = "データをマスキング";

        // ここをクリックするとタグ編集画面が開きます
        public override string ClickHereToOpenTheTagEditScreen { get; } = "ここをクリックするとタグ編集画面が開きます";

        // Properties
        public override string Properties { get; } = "プロパティ";

        // テキスト
        public override string Text { get; } = "テキスト";


        // ファイルパス
        public override string FilePath { get; } = "ファイルパス";

        // フォルダ
        public override string Folder { get; } = "フォルダ";

        // ファイル名
        public override string FileName { get; } = "ファイル名";

        // フォルダ名とファイル名
        public override string FolderNameAndFileName { get; } = "フォルダ名とファイル名";

        // イメージ
        public override string Image { get; } = "イメージ";

        // -- EditPythonScriptWindow --
        // 内容
        public override string Content { get; } = "内容";

        // -- ListPythonScriptWindow --
        // 新規Pythonスクリプト
        public override string NewPythonScript { get; } = "新規Pythonスクリプト";

        // -- SearchWindow --
        // 検索対象
        public override string SearchTarget { get; } = "検索対象";



        // 全フォルダ
        public override string AllFolders { get; } = "全フォルダ";

        // 検索対象フォルダ
        public override string SearchTargetFolder { get; } = "検索対象フォルダ";

        // 除外
        public override string Exclude { get; } = "除外";

        // コピー元アプリ名
        public override string CopySourceAppName { get; } = "コピー元アプリ名";

        // 開始日
        public override string StartDate { get; } = "開始日";

        // 終了日
        public override string EndDate { get; } = "終了日";

        // 適用対象配下のフォルダも対象にする
        public override string IncludeSubfolders { get; } = "適用対象配下のフォルダも対象にする";

        // -- TagSearchWindow
        // タグ検索
        public override string TagSearch { get; } = "タグ検索";

        // -- VectorSearchResultWindow
        // ベクトル検索結果
        public override string VectorSearchResult { get; } = "ベクトル検索結果";

        // VectorDBSearchScoreThreashold
        public override string VectorDBSearchScoreThreashold { get; } = "VectorDB検索スコア閾値";
        // -- ImageChatWindow
        // 設定項目
        public override string SettingItem { get; } = "設定項目";

        // 設定値
        public override string SettingValue { get; } = "設定値";

        // チェックタイプ
        public override string CheckType { get; } = "チェックタイプ";


        // -- ImageCheck.MainWindow --
        // SplitRequestByImage
        public override string SplitRequestByImage { get; } = "画像ごとにリクエストを分割";
        // 画像ファイル選択
        public override string SelectImageFile { get; } = "画像ファイル選択";

        // 画像エビデンスチェック項目編集
        public override string EditImageEvidenceCheckItem { get; } = "画像エビデンスチェック項目編集";

        // ここに回答が表示されます
        public override string TheAnswerWillBeDisplayedHere { get; } = "ここに回答が表示されます";

        // ここに質問を入力
        public override string EnterYourQuestionHere { get; } = "ここに質問を入力";

        // -- ListVectorDBWindow --
        // システム用のベクトルを表示
        public override string DisplayVectorsForTheSystem { get; } = "システム用のベクトルを表示";
        // システム用のプロンプトを表示
        public override string DisplayPromptsForTheSystem { get; } = "システム用のプロンプトを表示";

        // ベクトルDBの場所
        public override string VectorDBLocation { get; } = "ベクトルDBの場所";

        // ベクトルDBのタイプ
        public override string VectorDBType { get; } = "ベクトルDBのタイプ";

        // 新規ベクトルDB設定
        public override string NewVectorDBSetting { get; } = "新規ベクトルDB設定";

        // ベクトルDB設定編集
        public override string EditVectorDBSetting { get; } = "ベクトルDB設定編集";

        // -- QAChatControl --
        // 実験的機能1(文章解析+辞書生成+RAG)"
        public override string ExperimentalFunction1 { get; } = "実験的機能1(文章解析+辞書生成+RAG)";

        // ベクトルDB(フォルダ)
        public override string VectorDBFolder { get; } = "ベクトルDB(フォルダ)";

        // ベクトルDB選択
        public override string SelectVectorDB { get; } = "ベクトルDB選択";

        // VectorDBSearchAgent ベクトルDB検索用のエージェント
        public override string VectorDBSearchAgent { get; } = "ベクトルDB検索用のエージェント";


        // リストから除外
        public override string ExcludeFromList { get; } = "リストから除外";

        // ベクトルDB(外部)
        public override string VectorDB { get; } = "ベクトルDB";

        // CollectionName
        public override string CollectionName { get; } = "コレクション名";

        // ここをクリックしてベクトルDBを追加
        public override string ClickHereToAddVectorDB { get; } = "ここをクリックしてベクトルDBを追加";

        // ベクトルDB検索結果の最大値
        public override string VectorDBSearchResultMax { get; } = "ベクトルDB検索結果の最大値";

        // 画像ファイル
        public override string ImageFile { get; } = "画像ファイル";

        // ここをクリックして画像ファイルを追加
        public override string ClickHereToAddImageFile { get; } = "ここをクリックして画像ファイルを追加";

        // チャット
        public override string Chat { get; } = "チャット";

        // プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public override string PromptTemplate { get; } = "プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。";


        // リクエストパラメーター(JSON)
        public override string RequestParameterJson { get; } = "リクエストパラメーター(JSON)";

        // --- ApplicationFolderViewModel ---

        // フォルダを編集しました
        public override string FolderEdited { get; } = "フォルダを編集しました";

        // リロードしました
        public override string Reloaded { get; } = "リロードしました";

        // ファイルを選択してください
        public override string SelectFilePlease { get; } = "ファイルを選択してください";

        // "フォルダを削除しますか？"
        public override string ConfirmDeleteFolder { get; } = "フォルダを削除しますか？";

        // "フォルダを削除しました"
        public override string FolderDeleted { get; } = "フォルダを削除しました";

        // "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?"
        public override string ConfirmDeleteItems { get; } = "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?";

        // アイテムを削除しました
        public override string DeletedItems { get; } = "アイテムを削除しました";

        // "追加しました"
        public override string Added { get; } = "追加しました";

        // "編集しました"
        public override string Edited { get; } = "編集しました";

        // 貼り付けました
        public override string Pasted { get; } = "貼り付けました";

        // エラーが発生しました。\nメッセージ
        public override string ErrorOccurredAndMessage { get; } = "エラーが発生しました。\nメッセージ";

        // スタックトレース
        public override string StackTrace { get; } = "スタックトレース";

        // チャット結果を保存しました
        public override string SavedChatResult { get; } = "チャット結果を保存しました";

        // --- ApplicationItemViewModelCommands ---


        // 背景情報
        public override string BackgroundInformation { get; } = "背景情報";

        // 背景情報を生成します
        public override string GenerateBackgroundInformation { get; } = "背景情報を生成します";


        // その他のプロンプト
        public override string OtherPrompts { get; } = "その他のプロンプト";


        // "数値を入力してください。"
        public override string EnterANumber { get; } = "数値を入力してください。";

        // フォルダが選択されていません。
        public override string FolderNotSelected { get; } = "フォルダが選択されていません。";

        // ルール名を入力してください。
        public override string EnterRuleName { get; } = "ルール名を入力してください。";

        // "アクションを選択してください。"
        public override string SelectAction { get; } = "アクションを選択してください。";

        // "編集対象のルールが見つかりません。"
        public override string RuleNotFound { get; } = "編集対象のルールが見つかりません。";

        // コピーまたは移動先のフォルダを選択してください。
        public override string SelectCopyOrMoveTargetFolder { get; } = "コピーまたは移動先のフォルダを選択してください。";

        // "同じフォルダにはコピーまたは移動できません。"
        public override string CannotCopyOrMoveToTheSameFolder { get; } = "同じフォルダにはコピーまたは移動できません。";

        // "コピー/移動処理の無限ループを検出しました。"
        public override string DetectedAnInfiniteLoopInCopyMoveProcessing { get; } = "コピー/移動処理の無限ループを検出しました。";

        // "PromptTemplateを選択してください。"
        public override string SelectPromptTemplate { get; } = "PromptTemplateを選択してください。";

        // --- EditPythonScriptWindowViewModel ---
        // 説明を入力してください
        public override string EnterDescription { get; } = "説明を入力してください";

        // --- FolderEditWindowViewModel ---
        // フォルダ名を入力してください
        public override string EnterFolderName { get; } = "フォルダ名を入力してください";

        // --- FolderSelectWindowViewModel ---
        // エラーが発生しました。選択中のフォルダがない
        public override string SelectedFolderNotFound { get; } = "エラーが発生しました。選択中のフォルダがない";

        // --- ListAutoProcessRuleWindowViewModel ---
        // 自動処理ルールが選択されていません。
        public override string AutoProcessRuleNotSelected { get; } = "自動処理ルールが選択されていません。";

        // を削除しますか？
        public override string ConfirmDelete { get; } = "を削除しますか？";

        // --- SearchWindowViewModel ---



        // SearchConditionRuleがNullです
        public override string SearchConditionRuleIsNull { get; } = "SearchConditionRuleがNullです";

        // 検索条件がありません
        public override string NoSearchConditions { get; } = "検索条件がありません";

        // --- TagSearchWindowViewModel ---
        // "タグが空です"
        public override string TagIsEmpty { get; } = "タグが空です";

        // "タグが既に存在します"
        public override string TagAlreadyExists { get; } = "タグが既に存在します";

        // ExportCompleted
        public override string ExportCompleted { get; } = "エクスポートが完了しました";

        // バージョン情報
        public override string VersionInformation { get; } = "バージョン情報";


        #region  ApplicationItemViewModelCommands
        // **********************************************************************************
        // -- ExtractTextCommand


        // マージチャット 事前処理実行中
        public override string MergeChatPreprocessingInProgress { get; } = "マージチャット 事前処理実行中";
        // テキスト抽出処理が完了しました
        public override string TextExtractionCompleted { get; } = "テキスト抽出処理が完了しました";

        // **********************************************************************************
        #endregion

        // 情報
        public override string Information { get; } = "情報";

        // Error
        public override string Error { get; } = "エラー";

        // "終了しますか?"
        public override string ConfirmExit { get; } = "終了しますか?";



        // 切り取りました"
        public override string Cut { get; } = "切り取りました";

        // コピーしました"
        public override string Copied { get; } = "コピーしました";

        // 貼り付け先のフォルダがない
        public override string NoPasteFolder { get; } = "貼り付け先のフォルダがない";

        // "コピー元のフォルダがない"
        public override string NoCopyFolder { get; } = "コピー元のフォルダがない";

        // "選択中のアイテムを削除しますか?"
        public override string ConfirmDeleteSelectedItems { get; } = "選択中のアイテムを削除しますか?";

        // --- ImageCHat ---
        // 画像を確認して以下の各文が正しいか否かを教えてください\n\n
        public override string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "画像を確認して以下の各文が正しいか否かを教えてください\n\n";

        // 画像ファイルが選択されていません。
        public override string NoImageFileSelected { get; } = "画像ファイルが選択されていません。";

        // エラーが発生しました。
        public override string ErrorOccurred { get; } = "エラーが発生しました。";

        // 画像ファイルを選択してください
        public override string SelectImageFilePlease { get; } = "画像ファイルを選択してください";

        // すべてのファイル
        public override string AllFiles { get; } = "すべてのファイル";

        // "ファイルが存在しません。"
        public override string FileDoesNotExist { get; } = "ファイルが存在しません。";

        // -- EditPromptItemWindowViewModel --
        // プロンプト編集
        public override string EditPrompt { get; } = "プロンプト編集";

        // -- ListPromptTemplateWindow -- 
        // 新規プロンプトテンプレート
        public override string NewPromptTemplate { get; } = "新規プロンプトテンプレート";

        // -- SystemAutoProcessItem.cs --
        // --- EditPromptItemWindowViewModel ---
        // 名前を入力してください
        public override string EnterName { get; } = "名前を入力してください";

        // --- EditRAGSourceWindowViewModel ---
        // EditVectorDBWindowViewModel
        // Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません
        public override string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません";

        // プロンプトテンプレート一覧
        public override string PromptTemplateList { get; } = "プロンプトテンプレート一覧";

        // プロンプトテンプレートが選択されていません
        public override string NoPromptTemplateSelected { get; } = "プロンプトテンプレートが選択されていません";

        // ListVectorDBWindowViewModel
        // 編集するベクトルDBを選択してください
        public override string SelectVectorDBToEdit { get; } = "編集するベクトルDBを選択してください";

        // 削除するベクトルDBを選択してください
        public override string SelectVectorDBToDelete { get; } = "削除するベクトルDBを選択してください";

        // 選択中のベクトルDBを削除しますか？
        public override string ConfirmDeleteSelectedVectorDB { get; } = "選択中のベクトルDBを削除しますか？";

        // ベクトルDBを選択してください
        public override string SelectVectorDBPlease { get; } = "ベクトルDBを選択してください";

        // RAGManagementWindowViewModel

        // チャットの送信に失敗しました。
        public override string FailedToSendChat { get; } = "チャットの送信に失敗しました。";


        // 実行しますか？
        public override string ConfirmRun { get; } = "実行しますか？";

        // 設定チェック中
        public override string CheckingSettings { get; } = "設定チェック中";

        // 設定を保存しました。
        public override string SettingsSaved { get; } = "設定を保存しました。";

        // "キャンセルしました"
        public override string Canceled { get; } = "キャンセルしました";

        // MyStatusBarViewModel
        // ログ
        public override string Log { get; } = "ログ";

        // 統計
        public override string Statistics { get; } = "統計";

        // --- ApplicationFolder.cs ---

        // --- EditChatItem ---
        // チャットアイテム
        public override string ChatItem { get; } = "チャットアイテム";

        // --- ExportImportWindow ---
        // 以下の項目をエクスポートします
        public override string ExportTheFollowingItems { get; } = "以下の項目をエクスポートします";

        // インポート時に自動処理を実行します
        public override string ExecuteAutoProcessingOnImport { get; } = "インポート時に自動処理を実行します";


        // 例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。
        public override string ExampleGeneralVectorDB { get; } = "例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";


        // ドキュメントのチャンクサイズ
        public override string DocumentChunkSize { get; } = "ドキュメントのチャンクサイズ";

        // ベクトル検索結果の上限値
        public override string DefaultSearchResultLimit { get; } = "ベクトル検索結果のデフォルト上限値";

        // ベクトルのスコア(コサイン類似度)の閾値
        public override string DefaultScoreThreshold { get; } = "ベクトルのスコア(コサイン類似度)の閾値";

        // MultiVectorRetrieverを使用
        public override string UseMultiVectorRetriever { get; } = "MultiVectorRetrieverを使用";

        // DocStore用のSQLite3の場所
        public override string SQLite3LocationForDocStore { get; } = "DocStore用のSQLite3の場所";

        // 例：sqlite:///C:\Users\Username\sqlite3.db
        public override string ExampleSQLite3Location { get; } = "例：sqlite:///C:\\Users\\Username\\sqlite3.db";

        // 例：C:\Users\Username\vector.db
        public override string ExampleVectorDBLocationChroma { get; } = "例：C:\\Users\\Username\\vector.db";

        // 例：postgresql+psycopg://langchain:langchain@localhost:5432/langchain
        public override string ExampleVectorDBLocationPostgres { get; } = "例：postgresql+psycopg://langchain:langchain@localhost:5432/langchain";

        // チャット履歴をクリア
        public override string ClearChatContents { get; } = "チャット内容をクリア";

        // 本文をクリア
        public override string ClearContent { get; } = "本文をクリア";

        // 本文を再読み込み
        public override string ReloadContent { get; } = "本文を再読み込み";

        // 抽出したテキスト
        public override string ExtractedText { get; } = "抽出したテキスト";

        // デバッグ用のコマンド生成
        public override string GenerateDebugCommand { get; } = "デバッグ用のコマンド生成";

        // ExecuteDebugCommand
        public override string ExecuteDebugCommand { get; } = "デバッグ用のコマンドを実行";

        // VectorDBSettings
        public override string VectorDBSettings { get; } = "ベクトルDB設定";

        // ファイルの内容を表示するには「テキストを抽出」を実行してください
        public override string ExecuteExtractTextToViewFileContent { get; } = "ファイルの内容を表示するには「テキストを抽出」を実行してください";

        // Webページの内容を表示するには「Webページをダウンロード」を実行してください。
        public override string ExecuteDownloadWebPageToViewContent { get; } = "Webページの内容を表示するには「Webページをダウンロード」を実行してください。";

        // チャット：
        public override string ChatHeader { get; } = "チャット：";
        // タイトルなし
        public override string NoTitle { get; } = "タイトルなし";

        // サマリー
        public override string Summary { get; } = "サマリー";


    }
}
