using System.Collections.ObjectModel;
using System.Windows.Controls;
using PythonAILib.Model.Prompt;
using PythonAILib.Resource;

namespace ClipboardApp.ViewModel.ClipboardItemView {
    public class ClipboardItemMenu : ClipboardAppViewModelBase {

        public ClipboardItemViewModel ClipboardItemViewModel { get; private set; }

        public ClipboardItemMenu(ClipboardItemViewModel clipboardItemViewModel) {
            ClipboardItemViewModel = clipboardItemViewModel;
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
                Command = MainWindowViewModel.ActiveInstance.GenerateTitleCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateTitleMenuItem);

            // 背景情報生成
            MenuItem generateBackgroundInfoMenuItem = new() {
                Header = StringResources.GenerateBackgroundInfo,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateBackgroundInfoCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateBackgroundInfoMenuItem);

            // サマリーを生成
            MenuItem generateSummaryMenuItem = new() {
                Header = StringResources.GenerateSummary,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateSummaryCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateSummaryMenuItem);

            // 課題リストを生成
            MenuItem generateTasksMenuItem = new() {
                Header = StringResources.GenerateTasks,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateTasksCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateTasksMenuItem);

            // 文書信頼度をチェック
            MenuItem checkDocumentTrustMenuItem = new() {
                Header = StringResources.CheckDocumentReliability,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.CheckDocumentReliabilityCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(checkDocumentTrustMenuItem);

            // その他のプロンプト(プロンプトテンプレート一覧画面を開く)
            MenuItem otherPromptMenuItem = new() {
                Header = StringResources.OtherPrompts,
            };
            // DBからプロンプトテンプレートを取得し、選択させる
            List<PromptItem> promptItems = PromptItem.GetPromptItems().Where(x => x.PromptTemplateType == PromptTemplateTypeEnum.UserDefined).ToList();
            foreach (var promptItem in promptItems) {
                MenuItem promptItemMenuItem = new() {
                    Header = promptItem.Description,
                    Command = MainWindowViewModel.ActiveInstance.ExecutePromptTemplateCommand,
                    CommandParameter = new Tuple<ClipboardItemViewModel, PromptItem>(itemViewModel, promptItem)
                };
                otherPromptMenuItem.Items.Add(promptItemMenuItem);
            }

            promptMenuItem.Items.Add(otherPromptMenuItem);

            return promptMenuItem;
        }

        public ObservableCollection<MenuItem> CreateBasicItemContextMenuItems(ClipboardItemViewModel itemViewModel) {
            // MenuItemのリストを作成
            ObservableCollection<MenuItem> menuItems = [];
            // 開く
            MenuItem createMenuItem = new() {
                Header = StringResources.Open,
                Command = itemViewModel.FolderViewModel.OpenItemCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Ctrl+O"
            };
            menuItems.Add(createMenuItem);

            // テキストをファイルとして開く
            MenuItem openContentAsFileMenuItem = new() {
                Header = StringResources.OpenTextAsFile,
                Command = itemViewModel.OpenContentAsFileCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Ctrl+Shit+O"
            };
            menuItems.Add(openContentAsFileMenuItem);
            // ピン留め
            MenuItem pinnedStateChangeMenuItem = new() {
                Header = PythonAILibStringResources.Instance.Pin,
                Command = itemViewModel.ChangePinCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(pinnedStateChangeMenuItem);

            // コピー
            MenuItem copyMenuItem = new() {
                Header = StringResources.Copy,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.CopyItemCommand,
                CommandParameter = this,
                InputGestureText = "Ctrl+C"
            };
            menuItems.Add(copyMenuItem);

            // 削除
            MenuItem deleteMnuItem = new() {
                Header = StringResources.Delete,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.DeleteItemCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Delete"
            };
            menuItems.Add(deleteMnuItem);

            // プロンプトメニュー
            MenuItem promptMenuItem = CreatePromptMenuItems(itemViewModel);
            menuItems.Add(promptMenuItem);

            // ベクトル生成
            MenuItem generateVectorMenuItem = new() {
                Header = StringResources.GenerateVector,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateVectorCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(generateVectorMenuItem);

            // ベクトル検索
            MenuItem vectorSearchMenuItem = new() {
                Header = StringResources.VectorSearch,
                // 将来、複数のアイテムの処理を行う可能性があるため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.VectorSearchCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(vectorSearchMenuItem);

            //  テキストを抽出
            MenuItem extractTextMenuItem = new() {
                Header = StringResources.ExtractText,
                Command = itemViewModel.ExtractTextCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(extractTextMenuItem);
            // 文章の信頼度判定
            MenuItem checkDocumentTrustMenuItem = new() {
                Header = StringResources.CheckDocumentReliability,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.CheckDocumentReliabilityCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(checkDocumentTrustMenuItem);
            return menuItems;
        }
 
    }
}
