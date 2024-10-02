using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resource;
using QAChat.Control;
using QAChat.View.EditChatItemWindow;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.QAChatMain {
    public partial class QAChatControlViewModel {

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            PythonAILibManager? libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                await Task.Run(() => {

                    // LangChainChat用。VectorDBItemsを設定
                    List<VectorDBItem> items = [.. VectorDBItems];
                    ChatController.VectorDBItems = items;

                    // OpenAIChat or LangChainChatを実行
                    result = ChatController.ExecuteChat(libManager.ConfigParams.GetOpenAIProperties());
                });

                if (result == null) {
                    LogWrapper.Error(StringResources.FailedToSendChat);
                    return;
                }
                // ClipboardItemがある場合はClipboardItemのChatItemsを更新
                QAChatStartupProps.ContentItem.ChatItems = [.. ChatHistory];
                // inputTextをクリア
                InputText = "";
                OnPropertyChanged(nameof(ChatHistory));


            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // チャット履歴をクリアコマンド
        public SimpleDelegateCommand<object> ClearChatHistoryCommand => new((parameter) => {
            ChatHistory = [];
            // ClipboardItemがある場合は、ChatItemsをクリア
            QAChatStartupProps.ContentItem.ChatItems = [];
            OnPropertyChanged(nameof(ChatHistory));
        });
        // 本文を再読み込みコマンド
        public SimpleDelegateCommand<object> ReloadInputTextCommand => new((parameter) => {
            InputText = QAChatStartupProps.ContentItem?.Content ?? "";
            OnPropertyChanged(nameof(InputText));
        });
        // 本文をクリアコマンド
        public SimpleDelegateCommand<object> ClearInputTextCommand => new((parameter) => {
            InputText = "";
            OnPropertyChanged(nameof(InputText));

            PromptText = "";
            OnPropertyChanged(nameof(PromptText));

        });

        // モードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatController.ChatMode = (OpenAIExecutionModeEnum)index;
            // ModeがNormal以外の場合は、VectorDBItemを取得
            VectorDBItems = [];
            if (ChatController.ChatMode != OpenAIExecutionModeEnum.Normal) {
                VectorDBItem? item = QAChatStartupProps.ContentItem.GetVectorDBItem();
                if (item != null) {
                    VectorDBItems.Add(item);
                }
            }
            // VectorDBItemVisibilityを更新
            OnPropertyChanged(nameof(VectorDBItemVisibility));

        });

        // Tabが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
                // タブが変更されたときの処理
                if (tabControl.SelectedIndex == 1) {
                    // プレビュータブが選択された場合、プレビューテキストを更新
                    OnPropertyChanged(nameof(PreviewText));
                }
                if (tabControl.SelectedIndex == 2) {
                    // プレビュー(JSON)タブが選択された場合、プレビューJSONを更新
                    OnPropertyChanged(nameof(PreviewJson));
                }
            }
        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PromptTemplateCommand => new((parameter) => {

            PromptTemplateCommandExecute(parameter);

        });

        // チャットアイテムを編集するコマンド
        public SimpleDelegateCommand<ChatHistoryItem> OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // ベクトルDBをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem != null) {
                VectorDBItems.Remove(SelectedVectorDBItem);
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });
        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            QAChatStartupProps?.SelectVectorDBItemAction(VectorDBItems);
            OnPropertyChanged(nameof(VectorDBItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                VectorDBItems.Add(selectedItem);
            });
        });

        // クリップボードまたは他のクリップボードアイテムをペーストしたときの処理
        public SimpleDelegateCommand<object> PasteCommand => new((parameter) => {

            // ペースト処理を実行
            QAChatStartupProps?.AddContentItemCommandAction((values) => {
                MainUITask.Run(() => {
                    // ペーストしたアイテムを追加する
                    foreach (var item in values) {
                        ChatController.AdditionalItems.Add(item);
                    }
                    OnPropertyChanged(nameof(AdditionalItems));
                });
            });
        });
        // 選択したアイテムを開くコマンド
        public SimpleDelegateCommand<AdditionalItemViewModel> OpenSelectedItemCommand => new((item) => {
            QAChatStartupProps?.OpenSelectedItemCommand(item.ContentItem);
        });

        // Windowを閉じるコマンド
        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            window.Close();
        });


    }

}
