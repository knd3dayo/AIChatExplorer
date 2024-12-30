using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.Image;
using PythonAILib.Utils.Python;
using QAChat.Model;
using QAChat.View.QAChatMain;
using QAChat.View.PromptTemplate;
using QAChat.ViewModel.PromptTemplate;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.ImageChat
{
    public class ImageChatMainWindowViewModel : QAChatViewModelBase {
        // コンストラクタ
        public ImageChatMainWindowViewModel(ContentItem clipboardItem, Action afterUpdate) {
            AfterUpdate = afterUpdate;
            ClipboardItem = clipboardItem;
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(InputText));
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(ImageFiles));
            ChatController = new();
        }
        // データ保存用のClipboardItem
        public ContentItem ClipboardItem { get; set; }

        // Chat
        public ChatRequest ChatController { get; set; }

        // Progress Indicatorの表示状態
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        // 更新後の処理
        public Action AfterUpdate { get; set; } = () => { };


        // プロンプトの入力テキスト
        public string InputText {
            get {
                return ClipboardItem.ScreenShotCheckItem.InputText;
            }
            set {
                ClipboardItem.ScreenShotCheckItem.InputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }
        // 結果のテキスト
        public string ResultText {
            get {
                return ClipboardItem.ScreenShotCheckItem.ResultText;
            }
            set {
                ClipboardItem.ScreenShotCheckItem.ResultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }
        // ClipboardItemの説明
        public string Description {
            get {
                return ClipboardItem.Description;
            }
            set {
                ClipboardItem.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        // PromptText
        private string _PromptText = "";
        public string PromptText {
            get {
                return _PromptText;
            }
            set {
                _PromptText = value;
                OnPropertyChanged(nameof(PromptText));
            }
        }


        // 画像ファイル
        public ObservableCollection<ScreenShotImageViewModel> ImageFiles {
            get {
                ObservableCollection<ScreenShotImageViewModel> result = new();
                foreach (ScreenShotImage image in ClipboardItem.ScreenShotCheckItem.ScreenShotImages) {
                    result.Add(new(this, image));
                }
                return result;
            }
            set {
                ClipboardItem.ScreenShotCheckItem.ScreenShotImages = new(value.Select(image => image.ScreenShotImage));
                OnPropertyChanged(nameof(ImageFiles));
            }
        }

        // 最後に選択された画像ファイルがあるフォルダ
        private string lastSelectedImageFolder = ".";

        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }


        // ScreenShotCheckPromptWindowを開くコマンド
        public SimpleDelegateCommand<object> ScreenShotCheckPromptCommand => new((parameter) => {
            // ScreenShotCheckPromptWindowを生成してWindowを表示する。
            ScreenShotCheckPromptWindow.OpenScreenShotCheckPromptWindow(ClipboardItem.ScreenShotCheckItem.ScreenShotCheckIConditions, (Conditions) => {
                // ContentItem.ScreenShotCheckItem.ScreenShotCheckIConditionsにConditionsをコピー
                ClipboardItem.ScreenShotCheckItem.ScreenShotCheckIConditions = [.. Conditions];

                // ScreenShotCheckItemsを文字列に変換
                string result = StringResources.ConfirmTheFollowingSentencesAreCorrectOrNot;
                foreach (ScreenShotCheckCondition item in Conditions) {
                    result += "- " + item.ToPromptString() + "\n";
                }

                InputText = result;

            });
        });

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            PythonAILibManager? libManager = PythonAILibManager.Instance;
            // ChatRequestContextを生成
            ChatRequestContext chatRequestContext = new() {
                OpenAIProperties = libManager.ConfigParams.GetOpenAIProperties(),
            };

            // 画像イメージファイル名がない場合はエラー
            if (ImageFiles.Count == 0) {
                LogWrapper.Error(StringResources.NoImageFileSelected);
                return;
            }
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // プログレスバーを表示
                IsIndeterminate = true;

                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                // モードがNormalの場合はOpenAIChatでチャットを送信
                ChatResult? result = null;
                await Task.Run(() => {

                    // ScreenShotImageのリストからファイル名のリストを取得
                    List<string> imageFileNames = ImageFiles.Select(image => image.ScreenShotImage.
                    ImagePath).ToList();
                    // Base64に変換
                    List<string> imageBase64Strings = imageFileNames.Select(imageFileName => ChatUtil.CreateImageURLFromFilePath(imageFileName)).ToList();

                    ChatController.ChatMode = OpenAIExecutionModeEnum.Normal;
                    ChatController.ImageURLs = imageBase64Strings;
                    ChatController.ContentText = InputText;
                    ChatController.PromptTemplateText = PromptText;
                    // ChatRequestを送信してChatResultを受信
                    result = ChatController.ExecuteChat(chatRequestContext, (message) => { });

                });
                // 結果を表示
                if (result == null) {
                    LogWrapper.Error(StringResources.ErrorOccurred);
                    return;
                }
                ResultText = result.Output;


            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // 画像選択コマンド SelectImageFileCommand
        public SimpleDelegateCommand<Window> SelectImageFileCommand => new((window) => {

            //ファイルダイアログを表示
            // 画像ファイルを選択して画像ファイル名一覧に追加
            CommonOpenFileDialog dialog = new() {
                Title = StringResources.SelectImageFilePlease,
                InitialDirectory = lastSelectedImageFolder,
                Multiselect = true,
                Filters = {
                    new CommonFileDialogFilter(StringResources.ImageFile, "*.png;*.jpg;*.jpeg;*.bmp;*.gif"),
                    new CommonFileDialogFilter(StringResources.AllFiles, "*.*"),
                }
            };
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(currentWindow) != CommonFileDialogResult.Ok) {
                return;
            } else {
                foreach (string filePath in dialog.FileNames) {
                    // filePathをフォルダ名とファイル名に分割してフォルダ名を取得
                    string? folderPath = Path.GetDirectoryName(filePath);
                    if (folderPath != null) {
                        lastSelectedImageFolder = folderPath;
                    }
                    // ScreenShotImageを生成してImageFilesに追加
                    ScreenShotImage image = new() {
                        ImagePath = filePath
                    };
                    // 画像ファイル名一覧に画像ファイル名を追加
                    ClipboardItem.ScreenShotCheckItem.ScreenShotImages.Add(image);
                }
                OnPropertyChanged(nameof(ImageFiles));
            }
            window.Activate();

        });

        // クリアコマンド
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) => {
            InputText = "";
            ImageFiles = [];
            ChatController = new();

        });

        // OpenSelectedImageFileCommand  選択した画像ファイルを開くコマンド
        public SimpleDelegateCommand<ScreenShotImageViewModel> OpenSelectedImageFileCommand => new((image) => {
            if (File.Exists(image.ScreenShotImage.ImagePath)) {
                ProcessStartInfo psi = new() {
                    FileName = image.ScreenShotImage.ImagePath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            } else {
                LogWrapper.Error(StringResources.FileDoesNotExist);
            }
        });

        // RemoveSelectedImageFileCommand  選択した画像ファイルをScreenShotImageのリストから削除するコマンド
        public SimpleDelegateCommand<ScreenShotImageViewModel> RemoveSelectedImageFileCommand => new((image) => {
            ClipboardItem.ScreenShotCheckItem.ScreenShotImages.Remove(image.ScreenShotImage);
            OnPropertyChanged(nameof(ImageFiles));
        });

        public SimpleDelegateCommand<object> SelectPromptTemplateCommand => new((parameter) => {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        });



    }
}
