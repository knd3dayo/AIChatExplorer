using System.Data;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Prompt;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    internal class PromptResultViewModel : ObservableObject {

        public PromptResultViewModel(PromptChatResult promptChatResult, string promptName) {

            this.PromptName = promptName;
            this.PromptChatResult = promptChatResult;
            this.ComplexContent = ListToDataTable(PromptChatResult.GetComplexContent(promptName));
        }

        public string PromptName { get; set; }
        public PromptChatResult PromptChatResult { get; set; }

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;
        public string TextContent {
            get {
                return PromptChatResult.GetTextContent(PromptName);
            }
            set {
                PromptChatResult.SetTextContent(PromptName, value);
            }
        }
        // BackgroundInfoが空の場合はCollapsed,それ以外はVisible
        public Visibility TextContentVisibility {
            get {
                if (string.IsNullOrEmpty(TextContent)) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
        public DataTable ComplexContent { get; set; }

        // List<Dictionary<string, object>>からDataTableに変換
        private DataTable ListToDataTable(List<Dictionary<string, object>> complexContent) {
            DataTable dataTable = new();
            if (complexContent.Count == 0) {
                return dataTable;
            }
            // complexContentの1番目の要素からキーを取得して、DataTableのカラムを作成
            var firstRow = complexContent[0];
            foreach (var key in firstRow.Keys) {
                dataTable.Columns.Add(key, typeof(string));
            }
            // complexContentの各要素をDataTableに追加
            foreach (var row in complexContent) {
                DataRow dataRow = dataTable.NewRow();
                foreach (var key in row.Keys) {
                    dataRow[key] = row[key];
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }


        // DataTableからList<Dictionary<string, object>>に変換
        private List<Dictionary<string, object>> DataTableToList(DataTable dataTable) {
            List<Dictionary<string, object>> complexContent = new();
            foreach (DataRow row in dataTable.Rows) {
                Dictionary<string, object> newRow = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns) {
                    ((IDictionary<string, object>)newRow)[column.ColumnName] = row[column];
                }
                complexContent.Add(newRow);
            }
            return complexContent;
        }
        // ComplexContentを更新する
        private void UpdateComplexContent() {
            PromptChatResult.SetComplexContent(PromptName, DataTableToList(ComplexContent));
        }


        public Visibility ComplexContentVisibility {
            get {
                if (ComplexContent.Rows.Count == 0) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }

        // SelectedTaskItem
        public dynamic? SelectedItem { get; set; }

        // Tasksの削除
        public SimpleDelegateCommand<object> DeleteSelectedItemCommand => new((parameter) => {
            if (SelectedItem == null) {
                return;
            }
            // DataTableから削除
            DataRowView rowView = (DataRowView)SelectedItem;
            DataRow row = rowView.Row;
            ComplexContent.Rows.Remove(row);
            UpdateComplexContent();
            OnPropertyChanged(nameof(ComplexContent));
        });

        // TasksのSelectionChangedイベント発生時の処理
        public SimpleDelegateCommand<RoutedEventArgs> ItemSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                dynamic item = dataGrid.SelectedItem;
                SelectedItem = item;
            }

        });


    }
}
