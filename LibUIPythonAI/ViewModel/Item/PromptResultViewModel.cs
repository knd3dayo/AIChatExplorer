using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Utils;
using PythonAILib.Common;
using PythonAILib.Model.Prompt;

namespace LibUIPythonAI.ViewModel.Item {
    public class PromptResultViewModel : ObservableObject {

        public PromptResultViewModel(PromptChatResult promptChatResult, string promptName) {

            PromptName = promptName;
            PromptChatResult = promptChatResult;
            TableContent = PromptChatResult.GetTableContent(promptName);
        }

        public string PromptName { get; set; }
        public PromptChatResult PromptChatResult { get; set; }

        public string TextContent {
            get {
                return PromptChatResult.GetTextContent(PromptName);
            }
            set {
                PromptChatResult.SetTextContent(PromptName, value);
            }
        }

        public FlowDocument? MarkdownTextContent => MarkdownView ? LibPythonAI.Utils.Common.Tools.CreateFlowDocument(TextContent) : null;

        public DataTable TableContent { get; set; }

        // SelectedTaskItem
        public dynamic? SelectedItem { get; set; }

        
        public bool MarkdownView {
            get {
                return PythonAILibManager.Instance.ConfigParams.IsMarkdownView();
            }
        }

        // Tasksの削除
        public SimpleDelegateCommand<object> DeleteSelectedItemCommand => new((parameter) => {
            if (SelectedItem == null) {
                return;
            }
            // DataTableから削除
            DataRowView rowView = (DataRowView)SelectedItem;
            DataRow row = rowView.Row;
            TableContent.Rows.Remove(row);
            // PromptChatResultから削除
            PromptChatResult.SetTableContent(PromptName, TableContent);
            OnPropertyChanged(nameof(TableContent));
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


        // TextContentが空の場合はCollapsed,それ以外はVisible
        public Visibility TextContentVisibility => Tools.BoolToVisibility(string.IsNullOrEmpty(TextContent) == false && MarkdownView == false);

        // MarkdownViewがTrueの場合はCollapsed,それ以外はVisible
        public Visibility MarkdownViewVisibility => Tools.BoolToVisibility(string.IsNullOrEmpty(TextContent) == false && MarkdownView == true);
        // TableContentが空の場合はCollapsed,それ以外はVisible
        public Visibility TableContentVisibility => Tools.BoolToVisibility(TableContent.Rows.Count != 0);


    }
}
