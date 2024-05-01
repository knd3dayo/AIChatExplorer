using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.View.RAGWindow {
    internal class UpdateRAGIndexWindowViewModel : MyWindowViewModel {

        private RAGSourceItemViewModel? itemViewModel;
        private Action<RAGSourceItemViewModel> afterUpdate = (parameter) => { };

        // Taskキャンセル用のトークン
        private System.Threading.CancellationTokenSource tokenSource = new ();

        // LastProcessedCommit
        public string LastIndexCommitHash {
            get => itemViewModel?.Item.LastIndexCommitHash ?? "";
        }
        // IsAllCommit
        private bool isAllCommit;
        public bool IsAllCommit {
            get => isAllCommit;
            set {
                isAllCommit = value;
                OnPropertyChanged(nameof(IsAllCommit));
                // Modeを0に戻す
                SetMode(0);
            }
        }
        // IsAfterLastIndexedCommit
        private bool isAfterLastIndexedCommit = true;
        public bool IsAfterLastIndexedCommit {
            get => isAfterLastIndexedCommit;
            set {
                isAfterLastIndexedCommit = value;
                OnPropertyChanged(nameof(IsAfterLastIndexedCommit));
                // Modeを0に戻す
                SetMode(0);
            }
        }
        // IsRange
        private bool isRange;
        public bool IsRange {
            get => isRange;
            set {
                isRange = value;
                OnPropertyChanged(nameof(IsRange));
                // Modeを0に戻す
                SetMode(0);
            }
        }
        // RangeStart
        private string rangeStart = "";
        public string RangeStart {
            get => rangeStart;
            set {
                rangeStart = value;
                OnPropertyChanged(nameof(RangeStart));
            }
        }
        private string indexingStatusSummaryText = "";
        public string IndexingStatusSummaryText {
            get => indexingStatusSummaryText;
            set {
                indexingStatusSummaryText = value;
                OnPropertyChanged(nameof(IndexingStatusSummaryText));
            }
        }
        private string indexingStatusText = "";
        public string IndexingStatusText {
            get => indexingStatusText;
            set {
                indexingStatusText = value;
                OnPropertyChanged(nameof(IndexingStatusText));
            }
        }
        // TargetFilesInfo
        private int addedFilesCount = 0;
        private int modifiedFilesCount = 0;
        private int deletedFilesCount = 0;

        public string TargetFilesInfo {
            get {
                return $"インデックス化対象ファイル:{addedFilesCount}ファイル追加 {modifiedFilesCount}ファイル更新 {deletedFilesCount}ファイル削除";
            }
        }
        // インデックス化対象ファイルのリスト
        public ObservableCollection<FileStatus> TargetFiles { get; set; } = [];


        // OKボタンのテキスト表示のフラグ 0は対象ファイル取得モード、1はインデックス作成準備モード、2はインデックス作成中、3はインデックス作成完了
        private int Mode = 0;
        // OKボタンのテキスト 初期表示は"対象ファイル取得" 、インデックス作成準備モードの場合は"インデックス作成"
        // インデックス作成中および完了時の場合はボタン自体を非表示にする
        public string OkButtonText {
            get {
                return Mode == 0 ? "対象ファイル取得" : "インデックス作成";
            }
        }
        // キャンセルボタンのテキスト　初期表示は"閉じる"、インデックス作成準備モードの場合は戻る、インデックス作成中の場合は"停止"
        // インデックス作成管理時は初期状態に戻す
        public string CancelButtonText {
            get {
                return Mode == 0 ? "閉じる" : Mode == 1 ? "戻る" : Mode == 2 ? "停止" : "閉じる";
            }
        }
        // 対象ファイル一覧のVisibility
        public Visibility TargetFilesVisibility {
            get {
                return Mode == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // インデックス化処理の状態のVisibility
        public Visibility IndexingStatusVisibility {
            get {
                return Mode != 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // OKボタンのVisibility インデックス作成中および完了時の場合は非表示
        public Visibility OkButtonVisibility {
            get {
                return Mode == 2 || Mode == 3 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        // プログレスインジケーターを表示するかどうか
        private bool isIndeterminate = false;
        public bool IsIndeterminate {
            get => isIndeterminate;
            set {
                isIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }


        public void Initialize(RAGSourceItemViewModel itemViewModel, Action<RAGSourceItemViewModel> action) {

            this.itemViewModel = itemViewModel;
            this.afterUpdate = action;
            OnPropertyChanged(nameof(LastIndexCommitHash));

        }
        // SelectRangeStartCommand
        public SimpleDelegateCommand SelectRangeStartCommand => new(SelectRangeStartCommandExecute);
        private void SelectRangeStartCommandExecute(object parameter) {
            if (itemViewModel == null) {
                Tools.Error("RAGSourceItemViewModelが設定されていません");
                return;
            }
            var selectWindow = new SelectCommitWindow();
            SelectCommitWindowViewModel viewModel = (SelectCommitWindowViewModel)selectWindow.DataContext;
            viewModel.Initialize(itemViewModel, (hash) => {
                RangeStart = hash;
            });
            selectWindow.ShowDialog();
        }

        private void ClearIndexingStatus() {
            IndexingStatusText = "";
            IndexingStatusSummaryText = "";
        }
        private void ClearTargetFiles() {
            TargetFiles.Clear();
            addedFilesCount = 0;
            modifiedFilesCount = 0;
            deletedFilesCount = 0;
            OnPropertyChanged(nameof(TargetFilesInfo));
        }
        private void UpdateTargetFiles() {
            if (itemViewModel == null) {
                return;
            }
            // カウント初期化
            ClearTargetFiles();

            // 更新対象のファイルを取得
            List<FileStatus> files = [];
            if (IsAllCommit) {
                files = itemViewModel.Item.GetFileStatusList();
            } else if (IsAfterLastIndexedCommit) {
                files = itemViewModel.Item.GetAfterIndexedCommitFileStatusList();
            } else if (IsRange) {
                files = itemViewModel.Item.GetFileStatusList(RangeStart);
            }

            foreach (var file in files) {
                if (file.Status == FileStatusEnum.Added) {
                    // カウント更新
                    addedFilesCount++;
                    TargetFiles.Add(file);
                } else if (file.Status == FileStatusEnum.Modified) {
                    // カウント更新
                    modifiedFilesCount++;
                    TargetFiles.Add(file);
                } else if (file.Status == FileStatusEnum.Deleted) {
                    // カウント更新
                    deletedFilesCount++;
                    TargetFiles.Add(file);
                }
            }
            // 通知処理
            OnPropertyChanged(nameof(TargetFilesInfo));
        }
        private void SetMode(int mode) {
            Mode = mode;
            if (mode == 0) {
                // モードが0の場合はTargetFilesとIndexingStatusをクリア
                ClearTargetFiles();
                ClearIndexingStatus();
            }
            if (mode == 1) {
                // モードが1の場合はTargetFilesを更新 
                UpdateTargetFiles();
            }
            if (mode == 2) {
                // モードが2の場合はインデックス作成処理を開始
                ClearIndexingStatus();
                // Taskのキャンセル用のトークンを初期化
                tokenSource = new System.Threading.CancellationTokenSource();
            }
            if (mode == 3) {
                // モードが3の場合はTargetFilesをクリア
                ClearTargetFiles();
            }
            UpdateVisibility();
        }
        private void UpdateVisibility() {
            OnPropertyChanged(nameof(OkButtonText));
            OnPropertyChanged(nameof(CancelButtonText));
            OnPropertyChanged(nameof(TargetFilesVisibility));
            OnPropertyChanged(nameof(IndexingStatusVisibility));
            OnPropertyChanged(nameof(OkButtonVisibility));
        }

        // OKボタンのコマンド
        public SimpleDelegateCommand OkButtonCommand => new(async (parameter) => {
            if (itemViewModel == null) {
                Tools.Error("RAGSourceItemViewModelが設定されていません");
                return;
            }
            if (IsRange) {
                if (string.IsNullOrEmpty(RangeStart)) {
                    Tools.Error("開始コミットを指定してください");
                    return;
                }
            } else if (IsAllCommit == false && IsAfterLastIndexedCommit == false) {
                Tools.Error("対象を選択してください");
                return;
            }
            if (Mode == 0) {
                SetMode(1);
                // 通知処理
                OnPropertyChanged(nameof(TargetFilesInfo));
                return;
            }
            // 更新処理
            if (Mode == 1) {
                SetMode(2);
                try {
                    IndexingStatusText = "";
                    int fileCount = TargetFiles.Count;
                    IsIndeterminate = true;
                    int totalTokenCount = 0;
                    for (int i = 0; i < fileCount; i++) {
                        var file = TargetFiles[i];
                        // LangChainでは現在、Embeddingのトークンが取得できない.(https://github.com/langchain-ai/langchain/issues/20799)
                        // IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}] トークン数:[{totalTokenCount}]";
                        IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}]";

                        // 更新処理を開始
                        UpdateIndexResult? result;
                        IndexingStatusText += $"[{i + 1}/{fileCount}] {file.Path} インデックス作成中...";
                        Task task = new (() => {
                            // キャンセル用タスクの実行
                            Task.Run(() => {
                                while (IsIndeterminate) {

                                    if (tokenSource.Token.IsCancellationRequested) {
                                        tokenSource.Token.ThrowIfCancellationRequested();
                                    }
                                    System.Threading.Thread.Sleep(1000);
                                }
                            });
                            result = itemViewModel.Item.UpdateIndex(file);
                            totalTokenCount += result?.TokenCount ?? 0;
                        });
                        task.Start();
                        await task.WaitAsync(tokenSource.Token);

                        IndexingStatusText += "完了\n";
                        // IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}] トークン数:[{totalTokenCount}]";
                        IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}]";

                    }
                    IsIndeterminate = false;
                    SetMode(3);
                    // LastIndexCommitHashを更新
                    itemViewModel.Item.SetLastIndexCommitHash();
                    OnPropertyChanged(nameof(LastIndexCommitHash));
                    // 保存
                    itemViewModel.Item.Save();

                    // 完了通知のメッセージボックス
                    MessageBox.Show("インデックス作成が完了しました", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                    afterUpdate(itemViewModel);
                    if (parameter is not Window window) {
                        return;
                    }
                } catch (System.OperationCanceledException) {
                    Tools.Info("インデックス作成処理を中断しました");
                    SetMode(1);
                } catch (Exception e) {
                    Tools.Error($"エラーが発生しました\n[メッセージ]\n{ e.Message}\n[スタックトレース]\n{e.StackTrace}" );
                    SetMode(1);

                } finally {
                    IsIndeterminate = false;
                }
            }
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new(CancelButtonCommandExecute);
        private void CancelButtonCommandExecute(object parameter) {
            // Modeが0の場合はウィンドウを閉じる
            if (Mode == 0) {
                if (parameter is not Window window) {
                    return;
                }
                window.Close();
                return;
            }
            // Modeが1の場合はModeを0に戻す TargetFilesをクリア
            if (Mode == 1) {
                SetMode(0);
            }
            // Modeが2の場合はキャンセル処理を実行
            if (Mode == 2) {
                // 更新処理を停止
                // Taskのキャンセル
                tokenSource?.Cancel();
                return;
            }
            // Modeが3の場合はWindowを閉じる
            if (Mode == 3) {
                if (parameter is not Window window) {
                    return;
                }
                window.Close();
                return;
            }
        }
    }
}
