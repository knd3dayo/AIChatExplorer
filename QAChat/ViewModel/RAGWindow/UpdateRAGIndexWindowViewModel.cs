using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using QAChat.View.RAGWindow;
using WpfAppCommon.Utils;
using QAChat.Model;

namespace QAChat.ViewModel.RAGWindow {
    internal class UpdateRAGIndexWindowViewModel : QAChatViewModelBase {
        public UpdateRAGIndexWindowViewModel(RAGSourceItemViewModel itemViewModel, Action<RAGSourceItemViewModel> action) {

            this.itemViewModel = itemViewModel;
            afterUpdate = action;
            OnPropertyChanged(nameof(LastIndexCommitHash));

        }

        private RAGSourceItemViewModel itemViewModel;
        private Action<RAGSourceItemViewModel> afterUpdate;

        // Taskキャンセル用のトークン
        private CancellationTokenSource tokenSource = new();
        // Description
        public string Description { get; set; } = "";
        // Reliability
        public int Reliability { get; set; } = 50;


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
                return $"{StringResources.IndexingTargetFile}:{addedFilesCount} {StringResources.AddFile} {modifiedFilesCount} {StringResources.UpdateFile} {deletedFilesCount} {StringResources.DeleteFile}";
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
                return Mode == 0 ? StringResources.GetTargetFile : StringResources.CreateIndex;
            }
        }
        // キャンセルボタンのテキスト　初期表示は"閉じる"、インデックス作成準備モードの場合は戻る、インデックス作成中の場合は"停止"
        // インデックス作成管理時は初期状態に戻す
        public string CancelButtonText {
            get {
                return Mode == 0 ? StringResources.Close : Mode == 1 ? StringResources.Back : Mode == 2 ? StringResources.Stop : StringResources.Close;
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

        // SelectRangeStartCommand
        public SimpleDelegateCommand<object> SelectRangeStartCommand => new((parameter) => {
            if (itemViewModel == null) {
                LogWrapper.Error(StringResources.RAGSourceItemViewModelNotSet);
                return;
            }
            // ラジオボタンの選択をIsRangeに変更
            IsRange = true;

            SelectCommitWindow.OpenSelectCommitWindow(itemViewModel, (hash) => {
                RangeStart = hash;
            });

        });

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
                tokenSource = new CancellationTokenSource();
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
        public SimpleDelegateCommand<object> OkButtonCommand => new(async (parameter) => {
            if (itemViewModel == null) {
                LogWrapper.Error(StringResources.RAGSourceItemViewModelNotSet);
                return;
            }
            if (IsRange) {
                if (string.IsNullOrEmpty(RangeStart)) {
                    LogWrapper.Error(StringResources.SpecifyStartCommit);
                    return;
                }
            } else if (IsAllCommit == false && IsAfterLastIndexedCommit == false) {
                LogWrapper.Error(StringResources.SelectTarget);
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
                        IndexingStatusSummaryText = $"{StringResources.ProcessedFileCount}:[{i + 1}/{fileCount}]";

                        // 更新処理を開始
                        UpdateIndexResult result = new();
                        IndexingStatusText += $"[{i + 1}/{fileCount}] {file.Path} {StringResources.CreatingIndex}...";
                        Task task = new(() => {
                            // キャンセル用タスクの実行
                            Task.Run(() => {
                                while (IsIndeterminate) {

                                    if (tokenSource.Token.IsCancellationRequested) {
                                        tokenSource.Token.ThrowIfCancellationRequested();
                                    }
                                    Thread.Sleep(1000);
                                }
                            });
                            itemViewModel.Item.UpdateIndex(file, result, Description, Reliability);
                            totalTokenCount += result?.TokenCount ?? 0;
                        });
                        task.Start();
                        await task.WaitAsync(tokenSource.Token);

                        // resultがSuccessの場合
                        if (result.Result == UpdateIndexResult.UpdateIndexResultEnum.Success) {
                            IndexingStatusText += $"{StringResources.Completed}\n";
                        } else if (result.Result == UpdateIndexResult.UpdateIndexResultEnum.Failed_InvalidFileType) {
                            IndexingStatusText += StringResources.SkipUnsupportedFileType;
                        } else {
                            IndexingStatusText += $"{StringResources.Failed}:{result.Message}\n";
                        }

                        // IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}] トークン数:[{totalTokenCount}]";
                        IndexingStatusSummaryText = $"{StringResources.ProcessedFileCount}:[{i + 1}/{fileCount}]";

                    }
                    IsIndeterminate = false;
                    SetMode(3);
                    // LastIndexCommitHashを更新
                    itemViewModel.Item.SetLastIndexCommitHash();
                    OnPropertyChanged(nameof(LastIndexCommitHash));
                    // 保存
                    itemViewModel.Item.Save();

                    // 完了通知のメッセージボックス
                    MessageBox.Show(StringResources.IndexCreationCompleted, StringResources.Completed, MessageBoxButton.OK, MessageBoxImage.Information);
                    afterUpdate(itemViewModel);
                    if (parameter is not Window window) {
                        return;
                    }
                } catch (OperationCanceledException) {
                    LogWrapper.Info(StringResources.IndexCreationInterrupted);
                    SetMode(1);
                } catch (Exception e) {
                    LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage} {e.Message}\n[{StringResources.StackTrace}]\n{e.StackTrace}");
                    SetMode(1);

                } finally {
                    IsIndeterminate = false;
                }
            }
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<object> CancelButtonCommand => new(CancelButtonCommandExecute);
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
