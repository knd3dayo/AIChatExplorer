using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using LibPythonAI.Common;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Image;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using LibUIImageChat.View;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel.PromptTemplate;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LibUIImageChat.ViewModel {
    public class ImageChatWindowViewModel : CommonViewModelBase {
        // コンストラクタ
        public ImageChatWindowViewModel(ContentItemWrapper applicationItem, Action afterUpdate) {
            AfterUpdate = afterUpdate;
            ApplicationItem = applicationItem;
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(InputText));
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(ImageFiles));
        }
        public ScreenShotCheckItem ScreenShotCheckItem { get; set; } = new();

        // データ保存用のApplicationItem
        public ContentItemWrapper ApplicationItem { get; set; }

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
        // ApplicationItemの説明
        public string Description {
            get {
                return ApplicationItem.Description;
            }
            set {
                ApplicationItem.Description = value;
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

        // リクエストを画像ごとに分割するかどうか
        private bool splitRequestByImage = false;
        public bool SplitRequestByImage {
            get => splitRequestByImage;
            set {
                splitRequestByImage = value;
                OnPropertyChanged(nameof(SplitRequestByImage));
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
                string result = CommonStringResources.Instance.ConfirmTheFollowingSentencesAreCorrectOrNot;
                foreach (ScreenShotCheckCondition item in Conditions) {
                    result += "- " + item.ToPromptString() + "\n";
                }

                InputText = result;

            });
        });

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            try {
                CommonViewModelProperties.UpdateIndeterminate(true);
                if (SplitRequestByImage) {
                    List<string> results = [];
                    foreach (ScreenShotImageViewModel image in ImageFiles) {
                        // 画像ごとにリクエストを送信
                        string result = await SendChatByImagesAsync(InputText, PromptText, new ObservableCollection<ScreenShotImageViewModel> { image });
                        if (!string.IsNullOrEmpty(result)) {
                            results.Add(result);
                        }
                    }
                    // 結果を結合してResultTextに設定
                    ResultText = string.Join("\n", results);

                } else {
                    ResultText = await SendChatByImagesAsync(InputText, PromptText, ImageFiles);
                }
            } catch (Exception e) {
                LogWrapper.Error($"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}");
            } finally {
                CommonViewModelProperties.UpdateIndeterminate(false);
            }
        });
        // リクエストを送信するメソッド
        private static async Task<string> SendChatByImagesAsync(string InputText, string promptText, ObservableCollection<ScreenShotImageViewModel> images) {
            PythonAILibManager? libManager = PythonAILibManager.Instance;
            ChatSettings chatSettings = new() {
                PromptTemplateText = promptText,
            };
            ChatRequestContext chatRequestContext = new(chatSettings);
            if (images.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoImageFileSelected);
                return "";
            }
            try {
                // 画像ファイル名リスト取得
                List<string> imageFileNames = images.Select(image => image.ScreenShotImage.ImagePath).ToList();
                // Base64変換（I/Oが重い場合はTask.Runでオフロード）
                List<string> imageBase64Strings = await Task.Run(() =>
                    imageFileNames.Select(imageFileName => ChatUtil.CreateImageURLFromFilePath(imageFileName)).ToList()
                );
                ChatRequest chatRequest = new();
                chatRequest.ImageURLs = imageBase64Strings;
                chatRequest.ContentText = InputText;

                // ChatRequest送信
                ChatResponse? result = await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, chatRequest, chatRequestContext, (message) => { });
                if (result == null) {
                    LogWrapper.Error(CommonStringResources.Instance.ErrorOccurred);
                    return "";
                }
                return result.Output;

            } catch (Exception e) {
                LogWrapper.Error($"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}");
            } 
            return "";
        }

        // 画像選択コマンド SelectImageFileCommand
        public SimpleDelegateCommand<Window> SelectImageFileCommand => new((window) => {

            //ファイルダイアログを表示
            // 画像ファイルを選択して画像ファイル名一覧に追加
            CommonOpenFileDialog dialog = new() {
                Title = CommonStringResources.Instance.SelectImageFilePlease,
                InitialDirectory = lastSelectedImageFolder,
                Multiselect = true,
                Filters = {
                    new CommonFileDialogFilter(CommonStringResources.Instance.ImageFile, "*.png;*.jpg;*.jpeg;*.bmp;*.gif"),
                    new CommonFileDialogFilter(CommonStringResources.Instance.AllFiles, "*.*"),
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
            ResultText = "";
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
                LogWrapper.Error(CommonStringResources.Instance.FileDoesNotExist);
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
