using ClipboardApp.Model;
using System.Windows;
using PythonAILib.Model.Prompt;
using WpfAppCommon.Utils;
using PythonAILib.Model.Content;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;

namespace ClipboardApp.ViewModel {
    internal class PromptResultViewModel(PromptChatResult promptChatResult, string promptName): ObservableObject {
        public PromptChatResult PromptChatResult { get; set; } = promptChatResult;

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;
        public string TextContent {
            get {
                return PromptChatResult.GetTextContent(promptName);
            }
            set {
                PromptChatResult.SetTextContent(promptName, value);
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
        public ObservableCollection<object> ComplexContent {
            get {
                dynamic? content = PromptChatResult.GetComplexContent(promptName);
                if (content == null) {
                    return new ObservableCollection<object>();
                }
                return new ObservableCollection<object>(content);

            }
            set {
                PromptChatResult.SetComplexContent(promptName, value);
            }
        }
        public Visibility ComplexContentVisibility {
            get {
                if (ComplexContent.Count == 0) {
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
            PromptChatResult.GetComplexContent(promptName)?.Remove(SelectedItem);
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
