using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Main;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Resources;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Application {
    public class ApplicationItemMenu : CommonViewModelBase {

        public ContentItemViewModel ApplicationItemViewModel { get; private set; }

        protected AppViewModelCommandExecutes AppCommands { get; set; }

        public ApplicationItemMenu(ContentItemViewModel applicationItemViewModel) {
            ApplicationItemViewModel = applicationItemViewModel;
            CommonViewModelCommandExecutes contentCommands = applicationItemViewModel.ItemCommands.CommandExecutes;

            if (contentCommands is not AppViewModelCommandExecutes commands) {
                throw new Exception("commands is not AppItemViewModelCommands");
            }
            AppCommands = commands;

        }


        // Itemのコンテキストメニュー
        public MenuItem CreatePromptMenuItems() {


            // プロンプトメニュー
            MenuItem promptMenuItem = new() {
                Header = CommonStringResources.Instance.PromptMenu,
            };
            // システムプロンプト
            List<PromptItem> systemPromptItems = PromptItem.GetSystemPromptItems();
            foreach (var promptItem in systemPromptItems) {
                MenuItem promptItemMenuItem = new() {
                    Header = promptItem.Description,
                    Command = ApplicationItemViewModel.ItemCommands.ExecutePromptTemplateCommand,
                    CommandParameter = new ValueTuple<ObservableCollection<ContentItemViewModel>, PromptItem>([.. MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems], promptItem)
                };
                promptMenuItem.Items.Add(promptItemMenuItem);
            }

            // その他のプロンプト(プロンプトテンプレート一覧画面を開く)
            MenuItem otherPromptMenuItem = new() {
                Header = CommonStringResources.Instance.OtherPrompts,
            };

            // DBからプロンプトテンプレートを取得し、選択させる
            List<PromptItem> promptItems =  PromptItem.GetUserDefinedPromptItems();
            var itemViewModels = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems;
            foreach (var promptItem in promptItems) {
                MenuItem promptItemMenuItem = new() {
                    Header = promptItem.Description,
                    Command = ApplicationItemViewModel.ItemCommands.ExecutePromptTemplateCommand,
                    CommandParameter = new ValueTuple<ObservableCollection<ContentItemViewModel>, PromptItem>([.. itemViewModels], promptItem)
                };
                otherPromptMenuItem.Items.Add(promptItemMenuItem);
            }

            promptMenuItem.Items.Add(otherPromptMenuItem);

            return promptMenuItem;
        }

        public  virtual ObservableCollection<MenuItem> CreateBasicItemContextMenuItems() {

            ObservableCollection<MenuItem> menuItems =
            [
                // 新規
                CreateMenuItem,
                // 開く
                OpenMenuItem,
                // テキストをファイルとして開く
                OpenContentAsFileMenuItem,
                // ピン留め
                PinnedMenuItem,
                // 削除
                DeleteMenuItem,
            ];

            // プロンプトメニュー
            MenuItem promptMenuItem = CreatePromptMenuItems();
            menuItems.Add(promptMenuItem);

            // ベクトル生成
            menuItems.Add(GenerateVectorMenuItem);

            // ベクトル検索
            menuItems.Add(VectorSearchMenuItem);

            //  テキストを抽出
            menuItems.Add(ExtractTextMenuItem);

            // マージチャット
            menuItems.Add(MergeChatMenuItem);

            return menuItems;
        }

        public MenuItem CreateMenuItem {
            get {
                MenuItem createMenuItem = new() {
                    Header = CommonStringResources.Instance.Copy,
                    // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                    Command = AppCommands.CopyItemCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems,
                    InputGestureText = "Ctrl+C"
                };
                return createMenuItem;
            }
        }

        // 開く
        public MenuItem OpenMenuItem {
            get {
                MenuItem openMenuItem = new() {
                    Header = CommonStringResources.Instance.Open,
                    Command = OpenItemCommand,
                    CommandParameter = ApplicationItemViewModel,
                    InputGestureText = "Ctrl+O"
                };
                return openMenuItem;
            }
        }
        // テキストをファイルとして開く
        public MenuItem OpenContentAsFileMenuItem {
            get {
                MenuItem openContentAsFileMenuItem = new() {
                    Header = CommonStringResources.Instance.OpenTextAsFile,
                    Command = AppCommands.OpenContentAsFileCommand,
                    CommandParameter = ApplicationItemViewModel,
                    InputGestureText = "Ctrl+Shit+O"
                };
                return openContentAsFileMenuItem;
            }
        }

        // ピン留め
        public MenuItem PinnedMenuItem {
            get {
                MenuItem pinnedStateChangeMenuItem = new() {
                    Header = PythonAILibStringResources.Instance.Pin,
                    Command = AppCommands.ChangePinCommand,
                    CommandParameter = ApplicationItemViewModel
                };
                return pinnedStateChangeMenuItem;

            }
        }
        // 削除
        public MenuItem DeleteMenuItem {
            get {
                // 削除
                MenuItem deleteMnuItem = new() {
                    Header = CommonStringResources.Instance.Delete,
                    // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                    Command = ApplicationItemViewModel.ItemCommands.DeleteItemsCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems,
                    InputGestureText = "Delete"
                };
                return deleteMnuItem;
            }
        }
        // ベクトル生成
        public MenuItem GenerateVectorMenuItem {
            get {
                MenuItem generateVectorMenuItem = new() {
                    Header = CommonStringResources.Instance.GenerateVector,
                    // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                    Command = ApplicationItemViewModel.ItemCommands.GenerateVectorCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems,
                };
                return generateVectorMenuItem;
            }
        }

        // ベクトル検索
        public MenuItem VectorSearchMenuItem {
            get {
                MenuItem vectorSearchMenuItem = new() {
                    Header = CommonStringResources.Instance.VectorSearch,
                    // 将来、複数のアイテムの処理を行う可能性があるため、MainWindowViewModelのコマンドを使用
                    Command = ApplicationItemViewModel.ItemCommands.VectorSearchCommand,
                    CommandParameter = ApplicationItemViewModel
                };
                return vectorSearchMenuItem;
            }
        }
        //  テキストを抽出
        public MenuItem ExtractTextMenuItem {
            get {
                MenuItem extractTextMenuItem = new() {
                    Header = CommonStringResources.Instance.ExtractText,
                    Command = ApplicationItemViewModel.ItemCommands.ExtractTextCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel?.SelectedItems
                };
                return extractTextMenuItem;
            }
        }
        // マージチャット
        public MenuItem MergeChatMenuItem {
            get {
                MenuItem mergeChatMenuItem = new() {
                    Header = CommonStringResources.Instance.MergeChat,
                    Command = MainWindowViewModel.Instance.OpenSelectedItemsMergeChatWindow,
                    CommandParameter = ApplicationItemViewModel
                };
                return mergeChatMenuItem;
            }
        }

        // OpenContentItemCommand
        public SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand => new((itemViewModel) => {
           AppViewModelCommandExecutes.OpenItemCommandExecute(itemViewModel);
        });
    }
}
