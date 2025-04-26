using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using LibUIImageChat.View;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.PromptTemplate;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Image;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Image;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Python;

namespace LibUIImageChat.ViewModel {
    public class ImageChatWindowViewModel : ChatViewModelBase {
        // コンストラクタ
        public ImageChatWindowViewModel(ContentItemWrapper clipboardItem, Action afterUpdate) {
            AfterUpdate = afterUpdate;
            ClipboardItem = clipboardItem;
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(InputText));
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(ImageFiles));
            ChatRequest = new();
        }
        public ScreenShotCheckItem ScreenShotCheckItem { get; set; } = new();

        // データ保存用のClipboardItem
        public ContentItemWrapper ClipboardItem { get; set; }

        // Chat
        public ChatRequest ChatRequest { get; set; }

        // 更新後の処理
        public Action AfterUpdate { get; set; } = () => { };


        // プロンプトの入力テキスト
        public string InputText {
            get {
                return ScreenShotCheckItem.InputText;
            }
            set {
                ScreenShotCheckItem.InputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }
        // 結果のテキスト
        public string ResultText {
            get {
                return ScreenShotCheckItem.ResultText;
            }
            set {
                ScreenShotCheckItem.ResultText = value;
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
        public string PromptText {
            get {
                return ScreenShotCheckItem.PromptText;
            }
            set {
                ScreenShotCheckItem.PromptText = value;
                OnPropertyChanged(nameof(PromptText));
            }
        }


        // 画像ファイル
        public ObservableCollection<ScreenShotImageViewModel> ImageFiles {
            get {
                ObservableCollection<ScreenShotImageViewModel> result = new();
                foreach (ScreenShotImage image in ScreenShotCheckItem.ScreenShotImages) {
                    result.Add(new(this, image));
                }
                return result;
            }
            set {
                ScreenShotCheckItem.ScreenShotImages = new(value.Select(image => image.ScreenShotImage));
                OnPropertyChanged(nameof(ImageFiles));
            }
        }

        // 最後に選択された画像ファイルがあるフォルダ
        private string lastSelectedImageFolder = ".";

        // ScreenShotCheckPromptWindowを開くコマンド
        public SimpleDelegateCommand<object> ScreenShotCheckPromptCommand => new((parameter) => {
            // ScreenShotCheckPromptWindowを生成してWindowを表示する。
            ScreenShotCheckPromptWindow.OpenScreenShotCheckPromptWindow(ScreenShotCheckItem.ScreenShotCheckIConditions, (Conditions) => {
                // ContentItem.ScreenShotCheckItem.ScreenShotCheckIConditionsにConditionsをコピー
                ScreenShotCheckItem.ScreenShotCheckIConditions = [.. Conditions];

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
                ChatMode = OpenAIExecutionModeEnum.Normal,
                PromptTemplateText = PromptText,

            };

            // 画像イメージファイル名がない場合はエラー
            if (ImageFiles.Count == 0) {
                LogWrapper.Error(StringResources.NoImageFileSelected);
                return;
            }
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // プログレスバーを表示
                UpdateIndeterminate( true);

                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                // モードがNormalの場合はOpenAIChatでチャットを送信
                ChatResult? result = null;
                await Task.Run(() => {

                    // ScreenShotImageのリストからファイル名のリストを取得
                    List<string> imageFileNames = ImageFiles.Select(image => image.ScreenShotImage.
                    ImagePath).ToList();
                    // Base64に変換
                    List<string> imageBase64Strings = imageFileNames.Select(imageFileName => ChatUtil.CreateImageURLFromFilePath(imageFileName)).ToList();

                    ChatRequest.ImageURLs = imageBase64Strings;
                    ChatRequest.ContentText = InputText;
                    // ChatRequestを送信してChatResultを受信
                    result = ChatUtil.ExecuteChat(ChatRequest, chatRequestContext, (message) => { });

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
                UpdateIndeterminate(false);
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
                    ScreenShotCheckItem.ScreenShotImages.Add(image);
                }
                OnPropertyChanged(nameof(ImageFiles));
            }
            window.Activate();

        });

        // クリアコマンド
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) => {
            InputText = "";
            ImageFiles = [];
            ChatRequest = new();

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
            ScreenShotCheckItem.ScreenShotImages.Remove(image.ScreenShotImage);
            OnPropertyChanged(nameof(ImageFiles));
        });

        public SimpleDelegateCommand<object> SelectPromptTemplateCommand => new((parameter) => {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        });



    }
}
