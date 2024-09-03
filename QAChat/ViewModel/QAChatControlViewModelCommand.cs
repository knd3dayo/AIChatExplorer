using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using QAChat.Control;
using QAChat.View.EditChatItemWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public partial class QAChatControlViewModel {

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath, ClipboardAppConfig.PythonVenvPath);

                await Task.Run(() => {

                    // LangChainChat用。VectorDBItemsを設定
                    List<VectorDBItemBase> items = [.. SystemVectorDBItems, .. ExternalVectorDBItems];
                    ChatController.VectorDBItems = items;

                    // ImageFilesとImageItemsのImageをChatControllerに設定
                    ChatController.ImageURLs = [];
                    foreach (var item in ImageFiles) {
                        ChatController.ImageURLs.Add(Chat.CreateImageURLFromFilePath(item.ScreenShotImage.ImagePath));
                    }
                    foreach (var item in AdditionalItems) {
                        foreach (var file in item.ClipboardItem.ClipboardItemFiles) {
                            if (file.IsImage()) {
                                ChatController.ImageURLs.Add(Chat.CreateImageURL(file.Base64String));
                            }
                        }
                    }

                    // OpenAIChat or LangChainChatを実行
                    result = ChatController.ExecuteChat();
                });

                if (result == null) {
                    LogWrapper.Error(StringResources.FailedToSendChat);
                    return;
                }
                // ClipboardItemがある場合はClipboardItemのChatItemsを更新
                QAChatStartupProps.ClipboardItem.ChatItems = [.. ChatHistory];
                // inputTextをクリア
                InputText = "";
                OnPropertyChanged(nameof(ChatHistory));


            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // Saveコマンド
        public SimpleDelegateCommand<object> SaveCommand => new((parameter) => {
            // ChatHistoryをClipboardItemに設定
            if (QAChatStartupProps.ClipboardItem == null) {
                return;
            }
            QAChatStartupProps.ClipboardItem.ChatItems = [.. ChatHistory];
            // ClipboardItemを保存
            QAChatStartupProps.ClipboardItem.Save();

            //ChatHistoryItemがある場合は保存
            // チャット履歴用のItemの設定
            // チャット履歴を保存する。チャット履歴に同一階層のフォルダを作成して、Itemをコピーする。
            ClipboardFolder chatFolder = ClipboardFolder.GetAnotherTreeFolder(QAChatStartupProps.ClipboardItem.GetFolder(), ClipboardFolder.ChatRootFolder, true);
            ClipboardItem chatHistoryItem = new(chatFolder.Id);

            QAChatStartupProps.ClipboardItem.CopyTo(chatHistoryItem);
            chatHistoryItem.Save();

        });

        // チャット履歴をクリアコマンド
        public SimpleDelegateCommand<object> ClearChatHistoryCommand => new((parameter) => {
            ChatHistory = [];
            // ClipboardItemがある場合は、ChatItemsをクリア
            QAChatStartupProps.ClipboardItem.ChatItems = [];
            OnPropertyChanged(nameof(ChatHistory));
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
            ExternalVectorDBItems = [];
            if (ChatController.ChatMode != OpenAIExecutionModeEnum.Normal) {
                VectorDBItemBase? item = QAChatStartupProps.ClipboardItem.GetFolder().GetVectorDBItem();
                if (item != null) {
                    ExternalVectorDBItems.Add(item);
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
        public SimpleDelegateCommand<ChatIHistorytem> OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // ベクトルDB(フォルダ)をリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedSystemVectorDBItem != null) {
                SystemVectorDBItems.Remove(SelectedSystemVectorDBItem);
            }
            OnPropertyChanged(nameof(SystemVectorDBItems));
        });
        // ベクトルDB(フォルダ)を追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemFolderCommand => new((parameter) => {
            // フォルダを選択
            QAChatStartupProps?.SelectFolderAction(SystemVectorDBItems);
            OnPropertyChanged(nameof(SystemVectorDBItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenExternalVectorDBItemCommand => new((parameter) => {
            QAChatStartupProps?.SelectVectorDBItemsAction(ExternalVectorDBItems);
        });

        // 選択したVectorDBItemをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveExternalVectorDBItemCommand => new((parameter) => {
            if (SelectedExternalVectorDBItem != null) {
                ExternalVectorDBItems.Remove(SelectedExternalVectorDBItem);
            }
            OnPropertyChanged(nameof(ExternalVectorDBItems));
        });

        // クリップボードまたは他のクリップボードアイテムをペーストしたときの処理
        public SimpleDelegateCommand<object> PasteCommand => new((parameter) => {

            // ペースト処理を実行
            QAChatStartupProps?.PasteFromClipboardCommandAction((values) => {
                // ペーストしたアイテムを追加する
                foreach (var item in values) {
                    MainUITask.Run(() => {
                        AdditionalItemViewModel additionalItemViewModel = new(this, item);
                        AdditionalItems.Add(additionalItemViewModel);
                        OnPropertyChanged(nameof(AdditionalItems));
                    });
                }
            });
        });
        // 選択したアイテムを開くコマンド
        public SimpleDelegateCommand<AdditionalItemViewModel> OpenSelectedItemCommand => new((item) => {
            QAChatStartupProps?.OpenSelectedItemCommand(item.ClipboardItem);
        });

        // Windowを閉じるコマンド
        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            window.Close();
        });


    }

}
