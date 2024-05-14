using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ImageChat.View {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ScreenShotCheckPromptWindowViewModel : MyWindowViewModel {

        // 設定項目、設定値を保持するScreenShotCheckItem DataGridのItemsSource 
        public ObservableCollection<ScreenShotCheckItem> ScreenShotCheckItems { get; set; } = [];

        // CheckTypeList
        public ObservableCollection<CheckTypes> CheckTypeList { get; set; } = [.. CheckTypes.CheckTypeList];

        // 選択されたチェックタイプ
        private CheckTypes _selectedCheckType = CheckTypes.CheckTypeList[0];
        public CheckTypes SelectedCheckType {
            get {
                return _selectedCheckType;
            }
            set {
                _selectedCheckType = value;
                OnPropertyChanged(nameof(SelectedCheckType));
            }
        }

        Action<string> Action { get; set; } = (parameter) => { };

        // Initialize
        public void Initialize(Action<string> action) {
            Action = action;
        }

        // OKCommand
        public SimpleDelegateCommand OKCommand => new((parameter) => {
            // ScreenShotCheckItemsを文字列に変換
            string result = "画像を確認して以下の各文が正しいか否かを教えてください\n\n";
            foreach (ScreenShotCheckItem item in ScreenShotCheckItems) {
                result += "- " + item.ToPromptString() + "\n";
            }
            // Actionを実行
            Action(result);

            // Windowを閉じる
            if (parameter is Window window) {
                window.Close();
            }
        });
        
        // CancelCommand
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            // Windowを閉じる
            if (parameter is Window window) {
                window.Close();
            }
        });
        // クリアコマンド
        public SimpleDelegateCommand ClearCommand => new((parameter) => {
            // InputText = "";
            ScreenShotCheckItems.Clear();

        });

        // DataGridにデータを貼り付けるコマンド
        public SimpleDelegateCommand PasteDataGridCommand => new((parameter) => {
            // クリップボードからデータを取得
            IDataObject clipboardData = Clipboard.GetDataObject();
            if (clipboardData == null) {
                return;
            }
            if (clipboardData.GetData(DataFormats.Text) is not string clipboardText) {
                return;
            }
            // データを行に分割
            string[] lines = clipboardText.Split(separator, StringSplitOptions.None);
            if (lines.Length == 0) {
                return;
            }
            // DataGridのItemsSourceをクリア
            ScreenShotCheckItems.Clear();

            // 行を列に分割
            foreach (string line in lines) {
                string[] items = line.Split('\t');
                if (items.Length >= 2) {
                    ScreenShotCheckItem item = new() {
                        SettingItem = items[0],
                        SettingValue = items[1],
                    };
                    ScreenShotCheckItems.Add(item);
                }
            }
        });

        private static readonly string[] separator = ["\r\n", "\r", "\n"];

    }
}
