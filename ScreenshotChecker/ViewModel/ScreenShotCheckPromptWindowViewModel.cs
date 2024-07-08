using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ImageChat.ViewModel
{

    public class ScreenShotCheckPromptWindowViewModel : MyWindowViewModel
    {

        // 設定項目、設定値を保持するScreenShotCheckItem DataGridのItemsSource 
        public ObservableCollection<ScreenShotCheckICondition> ScreenShotCheckItems { get; set; } = [];

        // CheckTypeList
        public ObservableCollection<CheckTypes> CheckTypeList { get; set; } = [.. CheckTypes.CheckTypeList];



        Action<List<ScreenShotCheckICondition>> Action { get; set; } = (parameter) => { };

        // Initialize
        public void Initialize(List<ScreenShotCheckICondition> conditions, Action<List<ScreenShotCheckICondition>> action)
        {
            ScreenShotCheckItems = [.. conditions];
            OnPropertyChanged(nameof(ScreenShotCheckItems));
            Action = action;
        }

        // OKCommand
        public SimpleDelegateCommand<Window> OKCommand => new((window) =>
        {

            // DataGridを取得
            DataGrid dataGrid = (DataGrid)window.FindName("ScreenShotCheckDataGrid");
            // DataGridのItemsSourceを取得
            ScreenShotCheckItems = (ObservableCollection<ScreenShotCheckICondition>)dataGrid.ItemsSource;
            // Actionを実行
            Action([.. ScreenShotCheckItems]);
            // Windowを閉じる
            window.Close();

        });

        // CancelCommand
        public SimpleDelegateCommand<Window> CloseCommand => new((window) =>
        {
            // Windowを閉じる
            window.Close();
        });
        // クリアコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) =>
        {
            // InputText = "";
            ScreenShotCheckItems.Clear();

        });

        // DataGridにデータを貼り付けるコマンド
        public SimpleDelegateCommand<object> PasteDataGridCommand => new((parameter) =>
        {
            // クリップボードからデータを取得
            IDataObject clipboardData = Clipboard.GetDataObject();
            if (clipboardData == null)
            {
                return;
            }
            if (clipboardData.GetData(DataFormats.Text) is not string clipboardText)
            {
                return;
            }
            // データを行に分割
            string[] lines = clipboardText.Split(separator, StringSplitOptions.None);
            if (lines.Length == 0)
            {
                return;
            }
            // DataGridのItemsSourceをクリア
            ScreenShotCheckItems.Clear();

            // 行を列に分割
            foreach (string line in lines)
            {
                string[] items = line.Split('\t');
                if (items.Length >= 2)
                {
                    ScreenShotCheckICondition item = new()
                    {
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
