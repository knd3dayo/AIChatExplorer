using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.Image;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel;

namespace LibUIImageChat.ViewModel {

    public class ScreenShotCheckPromptWindowViewModel : CommonViewModelBase {

        // 設定項目、設定値を保持するScreenShotCheckItem DataGridのItemsSource 
        public ObservableCollection<ScreenShotCheckCondition> ScreenShotCheckItems { get; set; } = [];
        Action<List<ScreenShotCheckCondition>> Action { get; set; } = (parameter) => { };

        // Initialize
        public ScreenShotCheckPromptWindowViewModel(List<ScreenShotCheckCondition> conditions, Action<List<ScreenShotCheckCondition>> action) {
            ScreenShotCheckItems = [.. conditions];
            OnPropertyChanged(nameof(ScreenShotCheckItems));
            Action = action;
        }

        // OKCommand
        public SimpleDelegateCommand<Window> OKCommand => new((window) => {

            // DataGridを取得
            DataGrid dataGrid = (DataGrid)window.FindName("ScreenShotCheckDataGrid");
            // DataGridのItemsSourceを取得
            ScreenShotCheckItems = (ObservableCollection<ScreenShotCheckCondition>)dataGrid.ItemsSource;
            // Actionを実行
            Action([.. ScreenShotCheckItems]);
            // Windowを閉じる
            window.Close();

        });
        // クリアコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            // InputText = "";
            ScreenShotCheckItems.Clear();

        });

        // DataGridにデータを貼り付けるコマンド
        public SimpleDelegateCommand<object> PasteDataGridCommand => new((parameter) => {
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
            ScreenShotCheckItems = [];

            // 行を列に分割
            foreach (string line in lines) {
                string[] items = line.Split('\t');
                if (items.Length >= 2) {
                    ScreenShotCheckCondition item = new() {
                        SettingItem = items[0],
                        SettingValue = items[1],
                        CheckTypeString = ScreenShotCheckCondition.CheckTypeEqual
                    };
                    ScreenShotCheckItems.Add(item);
                }
            }
            // DataGridのItemsSourceを更新
            OnPropertyChanged(nameof(ScreenShotCheckItems));
        });
        private static readonly string[] separator = ["\r\n", "\r", "\n"];

    }
}
