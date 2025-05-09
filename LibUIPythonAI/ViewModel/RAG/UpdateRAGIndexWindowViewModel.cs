using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.View.RAG;
using LibUIPythonAI.Utils;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Model.File;

namespace LibUIPythonAI.ViewModel.RAG {
    internal class UpdateRAGIndexWindowViewModel : CommonViewModelBase {
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
                return $"{CommonStringResources.Instance.IndexingTargetFile}:{addedFilesCount} {CommonStringResources.Instance.AddFile} {modifiedFilesCount} {CommonStringResources.Instance.UpdateFile} {deletedFilesCount} {CommonStringResources.Instance.DeleteFile}";
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
                return Mode == 0 ? CommonStringResources.Instance.GetTargetFile : CommonStringResources.Instance.CreateIndex;
            }
        }
        // キャンセルボタンのテキスト　初期表示は"閉じる"、インデックス作成準備モードの場合は戻る、インデックス作成中の場合は"停止"
        // インデックス作成管理時は初期状態に戻す
        public string CancelButtonText {
            get {
                return Mode == 0 ? CommonStringResources.Instance.Close : Mode == 1 ? CommonStringResources.Instance.Back : Mode == 2 ? CommonStringResources.Instance.Stop : CommonStringResources.Instance.Close;
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


        // SelectRangeStartCommand
        public SimpleDelegateCommand<object> SelectRangeStartCommand => new((parameter) => {
            if (itemViewModel == null) {
                LogWrapper.Error(CommonStringResources.Instance.RAGSourceItemViewModelNotSet);
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
                LogWrapper.Error(CommonStringResources.Instance.RAGSourceItemViewModelNotSet);
                return;
            }
            if (IsRange) {
                if (string.IsNullOrEmpty(RangeStart)) {
                    LogWrapper.Error(CommonStringResources.Instance.SpecifyStartCommit);
                    return;
                }
            } else if (IsAllCommit == false && IsAfterLastIndexedCommit == false) {
                LogWrapper.Error(CommonStringResources.Instance.SelectTarget);
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
                    CommonViewModelProperties.UpdateIndeterminate(true);
                    int totalTokenCount = 0;
                    for (int i = 0; i < fileCount; i++) {
                        var file = TargetFiles[i];
                        // LangChainでは現在、Embeddingのトークンが取得できない.(https://github.com/langchain-ai/langchain/issues/20799)
                        // IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}] トークン数:[{totalTokenCount}]";
                        IndexingStatusSummaryText = $"{CommonStringResources.Instance.ProcessedFileCount}:[{i + 1}/{fileCount}]";

                        // 更新処理を開始
                        UpdateIndexResult result = new();
                        IndexingStatusText += $"[{i + 1}/{fileCount}] {file.Path} {CommonStringResources.Instance.CreatingIndex}...";
                        Task task = new(() => {
                            // キャンセル用タスクの実行
                            Task.Run(() => {
                                while (CommonViewModelProperties.IsIndeterminate) {

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
                            IndexingStatusText += $"{CommonStringResources.Instance.Completed}\n";
                        } else if (result.Result == UpdateIndexResult.UpdateIndexResultEnum.Failed_InvalidFileType) {
                            IndexingStatusText += CommonStringResources.Instance.SkipUnsupportedFileType;
                        } else {
                            IndexingStatusText += $"{CommonStringResources.Instance.Failed}:{result.Message}\n";
                        }

                        // IndexingStatusSummaryText = $"処理ファイル数:[{i + 1}/{fileCount}] トークン数:[{totalTokenCount}]";
                        IndexingStatusSummaryText = $"{CommonStringResources.Instance.ProcessedFileCount}:[{i + 1}/{fileCount}]";

                    }
                    CommonViewModelProperties.UpdateIndeterminate(false);
                    SetMode(3);
                    // LastIndexCommitHashを更新
                    itemViewModel.Item.SetLastIndexCommitHash();
                    OnPropertyChanged(nameof(LastIndexCommitHash));
                    // 保存
                    itemViewModel.Item.Save();

                    // 完了通知のメッセージボックス
                    MessageBox.Show(CommonStringResources.Instance.IndexCreationCompleted, CommonStringResources.Instance.Completed, MessageBoxButton.OK, MessageBoxImage.Information);
                    afterUpdate(itemViewModel);
                    if (parameter is not Window window) {
                        return;
                    }
                } catch (OperationCanceledException) {
                    LogWrapper.Info(CommonStringResources.Instance.IndexCreationInterrupted);
                    SetMode(1);
                } catch (Exception e) {
                    LogWrapper.Error($"{CommonStringResources.Instance.ErrorOccurredAndMessage} {e.Message}\n[{CommonStringResources.Instance.StackTrace}]\n{e.StackTrace}");
                    SetMode(1);

                } finally {
                    CommonViewModelProperties.UpdateIndeterminate(false);
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
