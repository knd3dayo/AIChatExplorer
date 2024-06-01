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

namespace QAChat.View.VectorDBWindow {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListVectorDBWindowViewModel : MyWindowViewModel {

        // VectorDBItemのリスト
        public ObservableCollection<VectorDBItemViewModel> VectorDBItems { get; set; } = [];

        // 選択中のVectorDBItem
        private VectorDBItemViewModel? selectedVectorDBItem;
        public VectorDBItemViewModel? SelectedVectorDBItem {
            get {
                return selectedVectorDBItem;
            }
            set {
                selectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }

        public void Initialize() {
            // VectorDBItemのリストを初期化
            VectorDBItems.Clear();
            foreach (var item in VectorDBItem.GetItems()) {
                VectorDBItems.Add(new VectorDBItemViewModel(item));
            }
            OnPropertyChanged(nameof(VectorDBItems));
        }

        // VectorDB Sourceの追加
        public SimpleDelegateCommand<object> AddVectorDBCommand => new((parameter) => {
            // SelectVectorDBItemを設定
            SelectedVectorDBItem = new VectorDBItemViewModel(new VectorDBItem());
            // ベクトルDBの編集Windowを開く
            EditVectorDBWindow.OpenEditVectorDBWindow(SelectedVectorDBItem, (afterUpdate) => {
                // リストを更新
                VectorDBItems.Clear();
                foreach (var item in VectorDBItem.GetItems()) {
                    VectorDBItems.Add(new VectorDBItemViewModel(item));
                }
                OnPropertyChanged(nameof(VectorDBItems));
            });

        });
        // Vector DB編集
        public SimpleDelegateCommand<object> EditVectorDBCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                Tools.Error("編集するベクトルDBを選択してください");
                return;
            }
            // ベクトルDBの編集Windowを開く
            EditVectorDBWindow.OpenEditVectorDBWindow(SelectedVectorDBItem, (afterUpdate) => {

                // リストを更新
                VectorDBItems.Clear();
                foreach (var item in VectorDBItem.GetItems()) {
                    VectorDBItems.Add(new VectorDBItemViewModel(item));
                }
                OnPropertyChanged(nameof(VectorDBItems));
            });

        });
        // DeleteVectorDBCommand
        public SimpleDelegateCommand<object> DeleteVectorDBCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                Tools.Error("削除するベクトルDBを選択してください");
                return;
            }
            // 確認ダイアログを表示
            MessageBoxResult result = MessageBox.Show("選択中のベクトルDBを削除しますか？", "確認", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) { 

                // 削除
                SelectedVectorDBItem.Delete();
                // リストを更新
                VectorDBItems.Clear();
                foreach (var item in VectorDBItem.GetItems()) {
                    VectorDBItems.Add(new VectorDBItemViewModel(item));
                }
            }
        });
        // CancelCommand
        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            // Windowを閉じる
            window.Close();
        });


    }
}
