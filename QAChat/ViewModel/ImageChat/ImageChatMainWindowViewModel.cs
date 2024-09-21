using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Image;
using PythonAILib.PythonIF;
using QAChat.View.ImageChat;
using QAChat.View.PromptTemplateWindow;
using QAChat.ViewModel.PromptTemplateWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.ImageChat {
    public class ImageChatMainWindowViewModel : MyWindowViewModel {
        // コンストラクタ
        public ImageChatMainWindowViewModel(ContentItemBase clipboardItem, Action afterUpdate) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            AfterUpdate = afterUpdate;
            ClipboardItem = clipboardItem;
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(InputText));
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(ImageFiles));
            ChatController = new(ClipboardAppConfig.CreateOpenAIProperties());
        }
        // データ保存用のClipboardItem
        public ContentItemBase ClipboardItem { get; set; }

        // Chat
        public Chat ChatController { get; set; }

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

        // SaveCommand
        public SimpleDelegateCommand<object> SaveCommand => new((parameter) => {
            // ClipboardItemを保存
            ClipboardItem.Save();
        });

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            // 画像イメージファイル名がない場合はエラー
            if (ImageFiles.Count == 0) {
                LogWrapper.Error(StringResources.NoImageFileSelected);
                return;
            }
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath, ClipboardAppConfig.PythonVenvPath);

                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                // モードがNormalの場合はOpenAIChatでチャットを送信
                ChatResult? result = null;
                await Task.Run(() => {

                    // ScreenShotImageのリストからファイル名のリストを取得
                    List<string> imageFileNames = ImageFiles.Select(image => image.ScreenShotImage.
                    ImagePath).ToList();
                    // Base64に変換
                    List<string> imageBase64Strings = imageFileNames.Select(imageFileName => Chat.CreateImageURLFromFilePath(imageFileName)).ToList();

                    ChatController.ChatMode = OpenAIExecutionModeEnum.Normal;
                    ChatController.ImageURLs = imageBase64Strings;
                    ChatController.ContentText = InputText;
                    ChatController.PromptTemplateText = PromptText;
                    // ChatRequestを送信してChatResultを受信
                    result = ChatController.ExecuteChat();

                });
                // 結果を表示
                if (result == null) {
                    LogWrapper.Error(StringResources.ErrorOccurred);
                    return;
                }
                ResultText = result.Response;


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
            ChatController = new(ClipboardAppConfig.CreateOpenAIProperties());

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
