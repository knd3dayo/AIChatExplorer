using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel
{
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class SelectCommitWindowViewModel : MyWindowViewModel
    {

        public ObservableCollection<CommitInfo> CommitInfoList { get; set; } = [];

        // RagSourceItemViewModel
        private RAGSourceItemViewModel? itemViewModel;

        // 選択中のコミットハッシュ
        private CommitInfo? selectedCommitInfo;
        public CommitInfo? SelectedCommitInfo
        {
            get
            {
                return selectedCommitInfo;
            }
            set
            {
                selectedCommitInfo = value;
                OnPropertyChanged(nameof(SelectedCommitInfo));
            }
        }
        private Action<string> afterUpdate = (hash) => { };

        public void Initialize(RAGSourceItemViewModel itemViewModel, Action<string> action)
        {
            this.itemViewModel = itemViewModel;
            // コミット情報を取得
            CommitInfoList.Clear();
            foreach (var commit in itemViewModel.Item.GetCommitList())
            {
                CommitInfoList.Add(commit);
            }
            afterUpdate = action;
        }

        // OKボタン
        public SimpleDelegateCommand<Window> OkCommand => new((window) =>
        {
            // 選択中のコミットハッシュが空の場合はエラー
            if (SelectedCommitInfo == null)
            {
                LogWrapper.Error("コミットを選択してください");
                return;
            }
            // 選択中のコミットハッシュを設定
            afterUpdate(SelectedCommitInfo.Hash);

            // Windowを閉じる
            window.Close();
        });

        public SimpleDelegateCommand<Window> CloseCommand => new((window) =>
        {
            WpfAppCommon.Properties.Settings.Default.Reload();
            LogWrapper.Info("設定をキャンセルしました");
            // Windowを閉じる
            window.Close();
        });


    }
}
