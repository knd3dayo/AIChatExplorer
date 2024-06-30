using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfAppCommon.View.QAChat;

namespace WpfAppCommon.Control.QAChat {
    public partial class QAChatControlViewModel {
      
        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

                await Task.Run(() => {
                    //追加テキストと追加画像を設定
                    UpdateChatRequestAdditionalItems();
                    // LangChainChat用。VectorDBItemの有効なアイテムを設定。
                    ChatController.VectorDBItems = VectorDBItems;
                    // OpenAIChat or LangChainChatを実行
                    result = ChatController.ExecuteChat();
                });

                if (result == null) {
                    LogWrapper.Error("チャットの送信に失敗しました。");
                    return;
                }
                // ClipboardItemがある場合はClipboardItemのChatItemsを更新
                if (ClipboardItem != null) {
                    ClipboardItem.ChatItems = [.. ChatHistory];

                }
                // inputTextをクリア
                InputText = "";
                OnPropertyChanged(nameof(ChatHistory));


            } catch (Exception e) {
                LogWrapper.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // Saveコマンド
        public SimpleDelegateCommand<object> SaveCommand => new((parameter) => {
            // ChatHistoryをClipboardItemに設定
            if (ClipboardItem == null) {
                return;
            }
            ClipboardItem.ChatItems = [.. ChatHistory];
            // ClipboardItemを保存
            ClipboardItem.Save();

            //ChatHistoryItemがある場合は保存
            if (ChatHistoryItem != null) {
                ClipboardItem.CopyTo(ChatHistoryItem);
                ChatHistoryItem.Save();
            }

        });
        // クリアコマンド
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) => {
            ChatHistory = [];
            InputText = "";
            // ClipboardItemがある場合は、ChatItemsをクリア
            if (ClipboardItem != null) {
                ClipboardItem.ChatItems = [];
            }
            OnPropertyChanged(nameof(ChatHistory));
        });

        // モードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatController.ChatMode = (OpenAIExecutionModeEnum)index;
            // ModeがRAGの場合は、VectorDBItemを取得
            if (ChatController.ChatMode == OpenAIExecutionModeEnum.RAG) {
                VectorDBItems = [.. ClipboardFolder?.GetVectorDBItems()];
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

        // 追加テキストのコンテキストメニュー
        // クリア処理
        public SimpleDelegateCommand<object> AdditionalTextClearCommand => new((parameter) => {
            ChatController.AdditionalTextItems.Clear();
            OnPropertyChanged(nameof(AdditionalTextItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 選択したフォルダのアイテムを追加

        public SimpleDelegateCommand<object> AdditionalTextAddFromFolderCommand => new((parameter) => {
            // ClipboardItemを選択
            SetContentTextFromClipboardItemsAction((List<ClipboardItem> selectedItems) => {
                AdditionalTextItems = [.. selectedItems];
            });
            OnPropertyChanged(nameof(AdditionalTextItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 検索結果のアイテムを追加
        public SimpleDelegateCommand<object> AdditionalTextAddFromSearchCommand => new((parameter) => {
            // SearchWindowを表示
            ShowSearchWindowAction((List<ClipboardItem> selectedItems) => {
                AdditionalTextItems = [.. selectedItems];
            });
            OnPropertyChanged(nameof(AdditionalTextItems));
            OnPropertyChanged(nameof(PreviewJson));
        });


        // 追加テキストのコンテキストメニュー
        // クリア処理
        public SimpleDelegateCommand<object> AdditionalImageClearCommand => new((parameter) => {
            ChatController.AdditionalImageURLs.Clear();
            OnPropertyChanged(nameof(AdditionalImageItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 選択したフォルダのアイテムを追加

        public SimpleDelegateCommand<object> AdditionalImageAddFromFolderCommand => new((parameter) => {
            // ClipboardItemを選択
            SetContentTextFromClipboardItemsAction((List<ClipboardItem> selectedItems) => {
                // SelectedItemsのうち、ClipboardItemImagesがあるものを追加
                foreach (var item in selectedItems) {
                    AdditionalTextItems.Add(item);
                }
            });
            OnPropertyChanged(nameof(AdditionalImageItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 検索結果のアイテムを追加
        public SimpleDelegateCommand<object> AdditionalImageAddFromSearchCommand => new((parameter) => {
            // SearchWindowを表示
            ShowSearchWindowAction((List<ClipboardItem> selectedItems) => {
                // AdditionalImageItemsに追加
                foreach (var item in selectedItems) {
                    AdditionalImageItems.Add(item);
                }
            });
            OnPropertyChanged(nameof(AdditionalImageItems));
            OnPropertyChanged(nameof(PreviewJson));
        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PromptTemplateCommand => new((parameter) => { 

            PromptTemplateCommandExecute(parameter);
        });

        // Closeコマンド
        public SimpleDelegateCommand<Window?> CloseCommand => new((window) => {

            window?.Close();
        });

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });

        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);
        });

        // チャットアイテムを編集するコマンド
        public SimpleDelegateCommand<ChatItem>  OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // 選択したクリップボードアイテムを開くコマンド

        public SimpleDelegateCommand<object> OpenClipboardItemCommand => new((parameter) => {
            if (SelectedContextItem != null) {
                OpenClipboardItemAction(SelectedContextItem);
            }
        });

        // 選択したクリップボードアイテムをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveClipboardItemCommand => new((parameter) => {
            if (SelectedContextItem != null) {
                AdditionalTextItems.Remove(SelectedContextItem);
            }
            OnPropertyChanged(nameof(AdditionalTextItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem != null) {
                OpenVectorDBItemAction(SelectedVectorDBItem);
            }
        });

        // 選択したVectorDBItemをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem != null) {
                VectorDBItems.Remove(SelectedVectorDBItem);
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });

    }

}
