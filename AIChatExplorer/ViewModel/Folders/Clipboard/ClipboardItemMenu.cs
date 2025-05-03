using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Main;
using LibPythonAI.Model.Prompt;
using LibUIPythonAI.Resource;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Prompt;
using PythonAILib.Resources;

namespace AIChatExplorer.ViewModel.Folders.Clipboard {
    public class ClipboardItemMenu : CommonViewModelBase {

        public ContentItemViewModel ClipboardItemViewModel { get; private set; }

        protected AppViewModelCommands AppCommands { get; set; }

        public ClipboardItemMenu(ContentItemViewModel clipboardItemViewModel) {
            ClipboardItemViewModel = clipboardItemViewModel;
            ContentItemViewModelCommands contentCommands = clipboardItemViewModel.Commands;

            if (contentCommands is not AppViewModelCommands commands) {
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

        public MenuItem CreatePromptMenuItems(ContentItemViewModel itemViewModel) {


            // プロンプトメニュー
            MenuItem promptMenuItem = new() {
                Header = CommonStringResources.Instance.PromptMenu,
            };
            // タイトルを生成
            MenuItem generateTitleMenuItem = new() {
                Header = CommonStringResources.Instance.GenerateTitle,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateTitleCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateTitleMenuItem);

            // タグを生成
            MenuItem generateTagMenuItem = new() {
                Header = CommonStringResources.Instance.GenerateTag,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateTagsCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };

            promptMenuItem.Items.Add(generateTagMenuItem);

            // 背景情報生成
            MenuItem generateBackgroundInfoMenuItem = new() {
                Header = CommonStringResources.Instance.GenerateBackgroundInfo,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateBackgroundInfoCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateBackgroundInfoMenuItem);

            // サマリーを生成
            MenuItem generateSummaryMenuItem = new() {
                Header = CommonStringResources.Instance.GenerateSummary,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateSummaryCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateSummaryMenuItem);

            // 課題リストを生成
            MenuItem generateTasksMenuItem = new() {
                Header = CommonStringResources.Instance.GenerateTasks,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.GenerateTasksCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(generateTasksMenuItem);

            // 文書信頼度をチェック
            MenuItem checkDocumentTrustMenuItem = new() {
                Header = CommonStringResources.Instance.CheckDocumentReliability,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = AppCommands.CheckDocumentReliabilityCommand,
                CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems
            };
            promptMenuItem.Items.Add(checkDocumentTrustMenuItem);

            // その他のプロンプト(プロンプトテンプレート一覧画面を開く)
            MenuItem otherPromptMenuItem = new() {
                Header = CommonStringResources.Instance.OtherPrompts,
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

        public virtual ObservableCollection<MenuItem> CreateBasicItemContextMenuItems(ContentItemViewModel itemViewModel) {

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
                    Header = CommonStringResources.Instance.OpenTextAsFile,
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
                    Header = CommonStringResources.Instance.Delete,
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
                    Header = CommonStringResources.Instance.GenerateVector,
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
                    Header = CommonStringResources.Instance.VectorSearch,
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
                    Header = CommonStringResources.Instance.ExtractText,
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
                    Header = CommonStringResources.Instance.MergeChat,
                    Command = MainWindowViewModel.Instance.OpenSelectedItemsMergeChatWindow,
                    CommandParameter = ClipboardItemViewModel
                };
                return mergeChatMenuItem;
            }
        }
    }
}
