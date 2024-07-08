using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using ImageChat.View;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ImageChat.ViewModel
{
    public class MainWindowViewModel : MyWindowViewModel
    {
        // コンストラクタ
        public MainWindowViewModel()
        {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);

        }
        // データ保存用のClipboardItem
        public ClipboardItem ClipboardItem { get; set; } = new(ClipboardFolder.ImageCheckRootFolder.Id);


        // Progress Indicatorの表示状態
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate
        {
            get
            {
                return _IsIndeterminate;
            }
            set
            {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        // 内部から起動されたか否か
        private bool isStartFromInternalApp = true;
        public bool IsStartFromInternalApp
        {
            get
            {
                return isStartFromInternalApp;
            }
            set
            {
                isStartFromInternalApp = value;
                OnPropertyChanged(nameof(IsStartFromInternalApp));
            }
        }
        // 更新後の処理
        public Action AfterUpdate { get; set; } = () => { };

        public void Initialize(ClipboardItem? clipboardItem, bool isStartFromInternalApp, Action afterUpdate)
        {
            IsStartFromInternalApp = isStartFromInternalApp;
            AfterUpdate = afterUpdate;
            if (clipboardItem != null)
            {
                ClipboardItem = clipboardItem;
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(InputText));
                OnPropertyChanged(nameof(ResultText));
                OnPropertyChanged(nameof(ImageFiles));

            }
        }

        public StringBuilder Log = new();

        // プロンプトの入力テキスト
        public string InputText
        {
            get
            {
                return ClipboardItem.ScreenShotCheckItem.InputText;
            }
            set
            {
                ClipboardItem.ScreenShotCheckItem.InputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }
        // 結果のテキスト
        public string ResultText
        {
            get
            {
                return ClipboardItem.ScreenShotCheckItem.ResultText;
            }
            set
            {
                ClipboardItem.ScreenShotCheckItem.ResultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }
        // ClipboardItemの説明
        public string Description
        {
            get
            {
                return ClipboardItem.Description;
            }
            set
            {
                ClipboardItem.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }


        // 画像ファイル
        public ObservableCollection<ScreenShotImage> ImageFiles
        {
            get
            {
                return [.. ClipboardItem.ScreenShotCheckItem.ScreenShotImages];
            }
            set
            {
                ClipboardItem.ScreenShotCheckItem.ScreenShotImages = [.. value];
                OnPropertyChanged(nameof(ImageFiles));
            }
        }

        // 最後に選択された画像ファイルがあるフォルダ
        private string lastSelectedImageFolder = ".";

        // ScreenShotCheckPromptWindowを開くコマンド
        public SimpleDelegateCommand<object> ScreenShotCheckPromptCommand => new((parameter) =>
        {
            // ScreenShotCheckPromptWindowを生成してWindowを表示する。
            ScreenShotCheckPromptWindow.OpenScreenShotCheckPromptWindow(ClipboardItem.ScreenShotCheckItem.ScreenShotCheckIConditions, (Conditions) =>
            {
                // ClipboardItem.ScreenShotCheckItem.ScreenShotCheckIConditionsにConditionsをコピー
                ClipboardItem.ScreenShotCheckItem.ScreenShotCheckIConditions = [.. Conditions];

                // ScreenShotCheckItemsを文字列に変換
                string result = "画像を確認して以下の各文が正しいか否かを教えてください\n\n";
                foreach (ScreenShotCheckICondition item in Conditions)
                {
                    result += "- " + item.ToPromptString() + "\n";
                }

                InputText = result;

            });
        });

        // SaveCommand
        public SimpleDelegateCommand<object> SaveCommand => new((parameter) =>
        {
            // ClipboardItemを保存
            ClipboardItem.Save();
        });

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) =>
        {
            // 画像イメージファイル名がない場合はエラー
            if (ImageFiles.Count == 0)
            {
                LogWrapper.Error("画像ファイルが選択されていません。");
                return;
            }
            // OpenAIにチャットを送信してレスポンスを受け取る
            try
            {
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                // モードがNormalの場合はOpenAIChatでチャットを送信
                ChatResult? result = null;
                await Task.Run(() =>
                {
                    string prompt = InputText;

                    // ScreenShotImageのリストからファイル名のリストを取得
                    List<string> imageFileNames = ImageFiles.Select(image => image.ImagePath).ToList();
                    // Base64に変換
                    List<string> imageBase64Strings = imageFileNames.Select(imageFileName => ChatRequest.CreateImageURLFromFilePath(imageFileName)).ToList();
                    // ChatRequestを生成
                    ChatRequest chatRequest = new(ClipboardAppConfig.CreateOpenAIProperties())
                    {
                        ChatMode = OpenAIExecutionModeEnum.Normal,
                        ImageURLs = imageBase64Strings,
                        ContentText = prompt
                    };
                    // ログ
                    LogWrapper.Info($"プロンプト：{prompt}を送信します");
                    // imageFileNamesをログに追加
                    LogWrapper.Info($"画像ファイル名：{string.Join(",", imageFileNames)}");
                    // ChatRequestを送信してChatResultを受信
                    result = chatRequest.ExecuteChat();

                    // verboseがある場合はログに追加
                    if (!string.IsNullOrEmpty(result?.Verbose))
                    {
                        Log.AppendLine(result.Verbose);
                    }
                });
                // 結果を表示
                if (result == null)
                {
                    LogWrapper.Error("エラーが発生しました。");
                    return;
                }
                ResultText = result.Response;


            }
            catch (Exception e)
            {
                LogWrapper.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            }
            finally
            {
                IsIndeterminate = false;
            }

        });

        // 画像選択コマンド SelectImageFileCommand
        public SimpleDelegateCommand<Window> SelectImageFileCommand => new((window) =>
        {

            //ファイルダイアログを表示
            // 画像ファイルを選択して画像ファイル名一覧に追加
            CommonOpenFileDialog dialog = new()
            {
                Title = "画像ファイルを選択してください",
                InitialDirectory = lastSelectedImageFolder,
                Multiselect = true,
                Filters = {
                    new CommonFileDialogFilter("画像ファイル", "*.png;*.jpg;*.jpeg;*.bmp;*.gif"),
                    new CommonFileDialogFilter("すべてのファイル", "*.*"),
                }
            };
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }
            else
            {
                foreach (string filePath in dialog.FileNames)
                {
                    // filePathをフォルダ名とファイル名に分割してフォルダ名を取得
                    string? folderPath = Path.GetDirectoryName(filePath);
                    if (folderPath != null)
                    {
                        lastSelectedImageFolder = folderPath;
                    }
                    // ScreenShotImageを生成してImageFilesに追加
                    ScreenShotImage image = new()
                    {
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
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) =>
        {
            InputText = "";
            ImageFiles = [];

        });

        // Closeコマンド
        public SimpleDelegateCommand<Window> CloseCommand => new((window) =>
        {
            AfterUpdate();
            window.Close();
        });

        // 設定画面を開くコマンド
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) =>
        {
            // SettingUserControlを生成してWindowを表示する。
            SettingsUserControl settingsControl = new();
            Window window = new()
            {
                SizeToContent = SizeToContent.Height,
                Title = CommonStringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();
        }

        );

        // OpenSelectedImageFileCommand  選択した画像ファイルを開くコマンド
        public SimpleDelegateCommand<ScreenShotImage> OpenSelectedImageFileCommand => new((image) =>
        {
            if (File.Exists(image.ImagePath))
            {
                ProcessStartInfo psi = new()
                {
                    FileName = image.ImagePath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            else
            {
                LogWrapper.Error("ファイルが存在しません。");
            }
        });

        // RemoveSelectedImageFileCommand  選択した画像ファイルをScreenShotImageのリストから削除するコマンド
        public SimpleDelegateCommand<ScreenShotImage> RemoveSelectedImageFileCommand => new((image) =>
        {
            ClipboardItem.ScreenShotCheckItem.ScreenShotImages.Remove(image);
            OnPropertyChanged(nameof(ImageFiles));
        });


    }
}
