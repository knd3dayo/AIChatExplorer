using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel
{
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class SelectCommitWindowViewModel : MyWindowViewModel {
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
        private RAGSourceItemViewModel itemViewModel { get; set;}

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
                LogWrapper.Error(StringResources.SelectCommitPlease);
                return;
            }
            // 選択中のコミットハッシュを設定
            afterUpdate(SelectedCommitInfo.Hash);

            // Windowを閉じる
            window.Close();
        });
    }
}
