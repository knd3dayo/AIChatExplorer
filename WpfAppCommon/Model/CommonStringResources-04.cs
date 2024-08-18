namespace WpfAppCommon.Model {
    public partial class CommonStringResources {

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
        // 実行
        public virtual string Execute { get; } = "実行";

        // スクリプトを選択してください
        public virtual string SelectScript { get; } = "スクリプトを選択してください";

        // --- SearchWindowViewModel ---
        // 検索フォルダ
        public virtual string SearchFolder { get; } = "検索フォルダ";

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

        // -- ClipboardApp.MainWindowViewModel --
        // "アプリケーションを再起動すると、表示モードが変更されます。"
        public virtual string DisplayModeWillChangeWhenYouRestartTheApplication { get; } = "アプリケーションを再起動すると、表示モードが変更されます。";
        // 情報
        public virtual string Information { get; } = "情報";

        // "終了しますか?"
        public virtual string ConfirmExit { get; } = "終了しますか?";

        // 選択中のアイテムがない"
        public virtual string NoItemSelected { get; } = "選択中のアイテムがない";

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

        // "削除しました"
        public virtual string Deleted { get; } = "削除しました";

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

        // リモートリポジトリが設定されていません
        public virtual string NoRemoteRepositorySet { get; } = "リモートリポジトリが設定されていません";

        // 作業ディレクトリが指定されていません
        public virtual string NoWorkingDirectorySpecified { get; } = "作業ディレクトリが指定されていません";

        // "指定されたディレクトリが存在しません"
        public virtual string SpecifiedDirectoryDoesNotExist { get; } = "指定されたディレクトリが存在しません";

        // "指定されたディレクトリはGitリポジトリではありません"
        public virtual string SpecifiedDirectoryIsNotAGitRepository { get; } = "指定されたディレクトリはGitリポジトリではありません";

        // "ベクトルDBが設定されていません"
        public virtual string NoVectorDBSet { get; } = "ベクトルDBが設定されていません";

        // -- ScreenShotCheckCondition.cs --

        public virtual string CheckTypeEqual { get; } = "等しい";
        public virtual string CheckTypeNotEqual { get; } = "等しくない";
        public virtual string CheckTypeInclude { get; } = "含む";
        public virtual string CheckTypeNotInclude { get; } = "含まない";
        public virtual string CheckTypeStartWith { get; } = "開始している";
        public virtual string CheckTypeNotStartWith { get; } = "開始していない";
        public virtual string CheckTypeEndWith { get; } = "終わっている";
        public virtual string CheckTypeNotEndWith { get; } = "終わっていない";
        public virtual string CheckTypeEmpty { get; } = "空である";
        public virtual string CheckTypeCheckBox { get; } = "チェックボックス";

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

    }
}
