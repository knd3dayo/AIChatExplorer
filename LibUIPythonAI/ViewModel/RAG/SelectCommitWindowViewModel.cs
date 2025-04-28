using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using PythonAILib.Model.File;

namespace LibUIPythonAI.ViewModel.RAG {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class SelectCommitWindowViewModel : CommonViewModelBase {
        public SelectCommitWindowViewModel(RAGSourceItemViewModel itemViewModel, Action<string> action) {
            this.itemViewModel = itemViewModel;
            // コミット情報を取得
            CommitInfoList.Clear();
            foreach (var commit in itemViewModel.Item.GetCommitList()) {
                CommitInfoList.Add(commit);
            }
            afterUpdate = action;
        }


        public ObservableCollection<CommitInfo> CommitInfoList { get; set; } = [];

        // RagSourceItemViewModel
        private RAGSourceItemViewModel itemViewModel { get; set; }

        // 選択中のコミットハッシュ
        private CommitInfo? selectedCommitInfo;
        public CommitInfo? SelectedCommitInfo {
            get {
                return selectedCommitInfo;
            }
            set {
                selectedCommitInfo = value;
                OnPropertyChanged(nameof(SelectedCommitInfo));
            }
        }
        private Action<string> afterUpdate = (hash) => { };

        // OKボタン
        public SimpleDelegateCommand<Window> OkCommand => new((window) => {
            // 選択中のコミットハッシュが空の場合はエラー
            if (SelectedCommitInfo == null) {
                LogWrapper.Error(CommonStringResources.Instance.SelectCommitPlease);
                return;
            }
            // 選択中のコミットハッシュを設定
            afterUpdate(SelectedCommitInfo.Hash);

            // Windowを閉じる
            window.Close();
        });
    }
}
