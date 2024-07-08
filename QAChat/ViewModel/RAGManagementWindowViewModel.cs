using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.View.RAGWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel
{
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class RAGManagementWindowViewModel : MyWindowViewModel
    {

        // RagSourceItemのリスト
        public ObservableCollection<RAGSourceItemViewModel> RagSourceItems { get; set; } = [];

        // 選択中のRagSourceItem
        private RAGSourceItemViewModel? selectedRagSourceItem;
        public RAGSourceItemViewModel? SelectedRagSourceItem
        {
            get
            {
                return selectedRagSourceItem;
            }
            set
            {
                selectedRagSourceItem = value;
                OnPropertyChanged(nameof(SelectedRagSourceItem));
            }
        }

        public void Initialize()
        {
            // RagSourceItemのリストを初期化
            RagSourceItems.Clear();
            foreach (var item in RAGSourceItem.GetItems())
            {
                RagSourceItems.Add(new RAGSourceItemViewModel(item));
            }
            OnPropertyChanged(nameof(RagSourceItems));
        }

        // RAG Sourceの追加
        public SimpleDelegateCommand<object> AddRagSourceCommand => new((parameter) =>
        {
            // SelectRAGSourceItemを設定
            SelectedRagSourceItem = new RAGSourceItemViewModel(new RAGSourceItem());

            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) =>
            {
                // リストを更新
                RagSourceItems.Clear();
                foreach (var item in RAGSourceItem.GetItems())
                {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
                OnPropertyChanged(nameof(RagSourceItems));
            });

        });
        // RAG Sourceの編集
        public SimpleDelegateCommand<object> EditRagSourceCommand => new((parameter) =>
        {
            if (SelectedRagSourceItem == null)
            {
                LogWrapper.Error("編集するRAG Sourceを選択してください");
                return;
            }
            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) =>
            {

                // リストを更新
                RagSourceItems.Clear();
                foreach (var item in RAGSourceItem.GetItems())
                {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
                OnPropertyChanged(nameof(RagSourceItems));
            });

        });
        // DeleteRAGSourceCommand
        public SimpleDelegateCommand<object> DeleteRAGSourceCommand => new((parameter) =>
        {
            if (SelectedRagSourceItem == null)
            {
                LogWrapper.Error("削除するRAG Sourceを選択してください");
                return;
            }
            // 確認ダイアログを表示
            MessageBoxResult result = MessageBox.Show("選択中のRAG Sourceを削除しますか？", "確認", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {

                // 削除
                SelectedRagSourceItem.Item.Delete();
                // リストを更新
                RagSourceItems.Clear();
                foreach (var item in RAGSourceItem.GetItems())
                {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
            }
        });
        // CancelCommand
        public SimpleDelegateCommand<Window> CloseCommand => new((window) =>
        {
            // Windowを閉じる
            window.Close();
        });


    }
}
