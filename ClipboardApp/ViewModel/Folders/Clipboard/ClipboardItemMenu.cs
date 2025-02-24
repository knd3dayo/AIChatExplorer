using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Main;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Prompt;
using PythonAILib.Resources;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Folders.Clipboard {
    public class ClipboardItemMenu : AppViewModelBase {

        public ClipboardItemViewModel ClipboardItemViewModel { get; private set; }

        protected AppItemViewModelCommands AppCommands { get; set; }

        public ClipboardItemMenu(ClipboardItemViewModel clipboardItemViewModel) {
            ClipboardItemViewModel = clipboardItemViewModel;
            ContentItemViewModelCommands contentCommands = clipboardItemViewModel.Commands;

            if (contentCommands is not AppItemViewModelCommands commands) {
                throw new Exception("commands is not AppItemViewModelCommands");
            }
            AppCommands = commands;
        }

        // Itemのコンテキストメニュー
        public virtual ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                return CreateBasicItemContextMenuItems(ClipboardItemViewModel);
            }
        }

        public MenuItem CreatePromptMenuItems(ClipboardItemViewModel itemViewModel) {


            // プロンプトメニュー
            MenuItem promptMenuItem = new() {
                Header = StringResources.PromptMenu,
            };
            // タイトルを生成
            MenuItem generateTitleMenuItem = new() {
                Header = StringResources.GenerateTitle,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateTitleCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };

            promptMenuItem.Items.Add(generateTitleMenuItem);

            // 背景情報生成
            MenuItem generateBackgroundInfoMenuItem = new() {
                Header = StringResources.GenerateBackgroundInfo,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateBackgroundInfoCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateBackgroundInfoMenuItem);

            // サマリーを生成
            MenuItem generateSummaryMenuItem = new() {
                Header = StringResources.GenerateSummary,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateSummaryCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateSummaryMenuItem);

            // 課題リストを生成
            MenuItem generateTasksMenuItem = new() {
                Header = StringResources.GenerateTasks,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateTasksCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateTasksMenuItem);

            // 文書信頼度をチェック
            MenuItem checkDocumentTrustMenuItem = new() {
                Header = StringResources.CheckDocumentReliability,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.CheckDocumentReliabilityCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(checkDocumentTrustMenuItem);

            // その他のプロンプト(プロンプトテンプレート一覧画面を開く)
            MenuItem otherPromptMenuItem = new() {
                Header = StringResources.OtherPrompts,
            };
            // DBからプロンプトテンプレートを取得し、選択させる
            List<PromptItem> promptItems = PromptItem.GetPromptItems().Where(x => x.PromptTemplateType == PromptTemplateTypeEnum.UserDefined).ToList();
            var itemViewModels = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems;
            foreach (var promptItem in promptItems) {
                MenuItem promptItemMenuItem = new() {
                    Header = promptItem.Description,
                    Command = AppCommands.ExecutePromptTemplateCommand,
                    CommandParameter = new ValueTuple<ObservableCollection<ContentItemViewModel>, PromptItem>([.. itemViewModels], promptItem)
                };
                otherPromptMenuItem.Items.Add(promptItemMenuItem);
            }

            promptMenuItem.Items.Add(otherPromptMenuItem);

            return promptMenuItem;
        }

        public virtual ObservableCollection<MenuItem> CreateBasicItemContextMenuItems(ClipboardItemViewModel itemViewModel) {

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
            MenuItem promptMenuItem = CreatePromptMenuItems(itemViewModel);
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
                    Header = StringResources.Copy,
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
                    Header = StringResources.Open,
                    Command = AppCommands.OpenItemCommand,
                    CommandParameter = ClipboardItemViewModel,
                    InputGestureText = "Ctrl+O"
                };
                return openMenuItem;
            }
        }
        // テキストをファイルとして開く
        public MenuItem OpenContentAsFileMenuItem {
            get {
                MenuItem openContentAsFileMenuItem = new() {
                    Header = StringResources.OpenTextAsFile,
                    Command = AppCommands.OpenContentAsFileCommand,
                    CommandParameter = ClipboardItemViewModel,
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
                    CommandParameter = ClipboardItemViewModel
                };
                return pinnedStateChangeMenuItem;

            }
        }
        // 削除
        public MenuItem DeleteMenuItem {
            get {
                // 削除
                MenuItem deleteMnuItem = new() {
                    Header = StringResources.Delete,
                    // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                    Command = AppCommands.DeleteItemsCommand,
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
                    Header = StringResources.GenerateVector,
                    // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                    Command = AppCommands.GenerateVectorCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems,
                };
                return generateVectorMenuItem;
            }
        }

        // ベクトル検索
        public MenuItem VectorSearchMenuItem {
            get {
                MenuItem vectorSearchMenuItem = new() {
                    Header = StringResources.VectorSearch,
                    // 将来、複数のアイテムの処理を行う可能性があるため、MainWindowViewModelのコマンドを使用
                    Command = AppCommands.VectorSearchCommand,
                    CommandParameter = ClipboardItemViewModel
                };
                return vectorSearchMenuItem;
            }
        }
        //  テキストを抽出
        public MenuItem ExtractTextMenuItem {
            get {
                MenuItem extractTextMenuItem = new() {
                    Header = StringResources.ExtractText,
                    Command = AppCommands.ExtractTextCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel?.SelectedItems
                };
                return extractTextMenuItem;
            }
        }
        // マージチャット
        public MenuItem MergeChatMenuItem {
            get {
                MenuItem mergeChatMenuItem = new() {
                    Header = StringResources.MergeChat,
                    Command = MainWindowViewModel.Instance.OpenSelectedItemsMergeChatWindow,
                    CommandParameter = ClipboardItemViewModel
                };
                return mergeChatMenuItem;
            }
        }
    }
}
