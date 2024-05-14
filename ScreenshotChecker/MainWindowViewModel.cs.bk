using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace ScreenshotChecker {
    public class MainWindowViewModel : MyWindowViewModel {
        // コンストラクタ
        public MainWindowViewModel() {
            // DataGridの初期化。 1行追加
            ScreenShotCheckItems.Add(new ScreenShotCheckItem());
        }
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

        // 内部から起動されたか否か
        private bool isStartFromInternalApp = true;
        public bool IsStartFromInternalApp {
            get {
                return isStartFromInternalApp;
            }
            set {
                isStartFromInternalApp = value;
                OnPropertyChanged(nameof(IsStartFromInternalApp));
            }
        }

        public StringBuilder Log = new();

        // プロンプトの入力テキスト
        private string inputText = """
            {setting_item:設定項目, setting_value:設定値, result_string:"", actual_value:""}のフォーマットのリストを入力データとして渡します。
            - 入力データのリストの要素毎に次の処理を行います。
              1.setting_itemの値が画像に含まれているかチェックします。
              - setting_itemの値が画像に含まれていない場合は、result_string:Unknownとactual_value: ""を設定してください。
              - setting_itemの値が画像に存在する場合は、2の処理を実行します。
              2.setting_itemの値の付近にsetting_valueの値が存在するかチェックします。　
              - 存在しない場合は、result_string:NG, actual_value: 画像内の文字列 を設定します。
              - 存在する場合は、result_string:OK, actual_value: 画像内の文字列 を設定します。

            - 結果をJSONで返してください。
            - 結果例:
            {
            "results":[
                {"setting_item": "設定項目1", "setting_value": "設定値1", result_string: "OK", "actual_value":  "設定値1"},
                {"setting_item": "設定項目2", "setting_value": "設定値2", result_string: "NG"} "actual_value": "設定値5",
                {"setting_item": "設定項目3", "setting_value": "設定値3", result_string: "Unknown", "actual_value": ""},
            ]
            }
            --- 以下は入力データ--
            
            """;
        public string InputText {
            get {
                return inputText;
            }
            set {
                inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        // 設定項目、設定値を保持するScreenShotCheckItem DataGridのItemsSource 
        public ObservableCollection<ScreenShotCheckItem> ScreenShotCheckItems { get; set; } = [];

        // 画像ファイル名一覧
        public ObservableCollection<string> ImageFileNames { get; set; } = [];

        // 最後に選択された画像ファイルがあるフォルダ
        private string lastSelectedImageFolder = ".";


        // チャットを送信するコマンド
        public SimpleDelegateCommand SendChatCommand => new(async (parameter) => {
            // 画像イメージファイル名がない場合はエラー
            if (ImageFileNames.Count == 0) {
                Tools.Error("画像ファイルが選択されていません。");
                return;
            }
            // DataGridが空の場合はエラー
            if (ScreenShotCheckItems.Count == 0) {
                Tools.Error("設定項目が入力されていません。");
                return;
            }
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // ScreenShotCheckItemsのResultとActualValueを初期化
                foreach (var item in ScreenShotCheckItems) {
                    item.Result = ScreenShotCheckItem.ResultEnum.Empty;
                    item.ActualValue = "";
                }
                // Tools.ActiveWindowでWindowを取得してDataGridを取得
                if (Tools.ActiveWindow?.FindName("datagrid1") is DataGrid dataGrid) {
                    dataGrid.Items.Refresh();
                }

                List<ScreenShotCheckItem> resultScreenShotCheckItems = [];
                // resultScreenShotCheckItemsにScreenShotCheckItemsの内容をコピー
                foreach (var item in ScreenShotCheckItems) {
                    resultScreenShotCheckItems.Add(item);
                }
                List<ScreenShotCheckItem> tempScreenShotCheckItems = [];
                // DataGridの内容をコピー
                foreach (var item in ScreenShotCheckItems) {
                    tempScreenShotCheckItems.Add(item);
                }


                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                // モードがNormalの場合はOpenAIChatでチャットを送信
                await Task.Run(() => {
                    foreach (var imageFileName in ImageFileNames) {
                        string prompt = inputText;
                        // DataGridの内容をJSON文字列に変換
                        string json = ScreenShotCheckItem.ToJson(tempScreenShotCheckItems);
                        // promptにJSON文字列を追加
                        prompt += json;

                        ChatResult? result = null;

                        // 画像を一枚ずつ処理
                        Tools.Info($"プロンプト：{prompt}を送信します");
                        Tools.Info("画像ファイルを処理中です。" + imageFileName);
                        result = PythonExecutor.PythonFunctions?.OpenAIChatWithVision(prompt, [imageFileName]);
                        // result.ResponseをScreenShotCheckItems.FromJsonでScreenShotCheckItemsに変換
                        if (result != null && string.IsNullOrEmpty(result.Response) == false) {
                            Tools.Info("処理結果を受信しました。" + result.Response);
                            resultScreenShotCheckItems = ScreenShotCheckResults.FromJson(result.Response);

                        } else {
                            Tools.Error("処理結果の受信に失敗しました。");
                            break;
                        }
                        // ScreenShotCheckItemsに結果を上書き
                        foreach (var item in resultScreenShotCheckItems) {
                            // 同じ設定項目の結果がある場合は上書き
                            var targetItem = ScreenShotCheckItems.FirstOrDefault(x => x.SettingItem == item.SettingItem);
                        }
                        // resultScreenShotCheckItemsからOKとNSのものを削除 次のループでUnknownのものをチェック
                        tempScreenShotCheckItems = resultScreenShotCheckItems.Where(
                            x => x.Result is not ScreenShotCheckItem.ResultEnum.OK and not ScreenShotCheckItem.ResultEnum.NG).ToList();

                        // verboseがある場合はログに追加
                        if (!string.IsNullOrEmpty(result.Verbose)) {
                            Log.AppendLine(result.Verbose);
                        }
                    }
                });
                // resultScreenShotCheckItemsの内容をDataGridに反映
                ScreenShotCheckItems.Clear();
                foreach (var item in resultScreenShotCheckItems) {
                    ScreenShotCheckItems.Add(item);
                }


            } catch (Exception e) {
                Tools.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });
        // DataGridにデータを貼り付けるコマンド
        public SimpleDelegateCommand PasteDataGridCommand => new((parameter) => {
            // クリップボードからデータを取得
            IDataObject clipboardData = Clipboard.GetDataObject();
            if (clipboardData == null) {
                return;
            }
            if (clipboardData.GetData(DataFormats.Text) is not string clipboardText) {
                return;
            }
            // データを行に分割
            string[] lines = clipboardText.Split(separator, StringSplitOptions.None);
            if (lines.Length == 0) {
                return;
            }
            // DataGridのItemsSourceをクリア
            ScreenShotCheckItems.Clear();

            // 行を列に分割
            foreach (string line in lines) {
                string[] items = line.Split('\t');
                if (items.Length >= 2) {
                    ScreenShotCheckItem item = new() {
                        SettingItem = items[0],
                        SettingValue = items[1],
                    };
                    ScreenShotCheckItems.Add(item);
                }
            }
        });

        // 画像選択コマンド SelectImageFileCommand
        public SimpleDelegateCommand SelectImageFileCommand => new((parameter) => {

            //ファイルダイアログを表示
            // 画像ファイルを選択して画像ファイル名一覧に追加
            CommonOpenFileDialog dialog = new() {
                Title = "画像ファイルを選択してください",
                InitialDirectory = lastSelectedImageFolder,
                Multiselect = true,
                Filters = {
                    new CommonFileDialogFilter("画像ファイル", "*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.tif"),
                    new CommonFileDialogFilter("すべてのファイル", "*.*"),
                }
            };
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            } else {
                string filePath = dialog.FileName;
                // filePathをフォルダ名とファイル名に分割してフォルダ名を取得
                string? folderPath = Path.GetDirectoryName(filePath);
                if (folderPath != null) {
                    lastSelectedImageFolder = folderPath;
                }
                // 画像ファイル名一覧に画像ファイル名を追加
                ImageFileNames.Add(filePath);


            }
        });



        // クリアコマンド
        public SimpleDelegateCommand ClearChatCommand => new((parameter) => {
            // InputText = "";
            ScreenShotCheckItems.Clear();
            ImageFileNames.Clear();

        });

        // Closeコマンド
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            if (parameter is Window window) {
                window.Close();
            }
        });

        // 設定画面を開くコマンド
        public SimpleDelegateCommand SettingCommand => new((parameter) => {
            // SettingUserControlを生成してWindowを表示する。
            SettingsUserControl settingsControl = new();
            Window window = new() {
                Title = StringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();
        }

        );

        private static readonly string[] separator = ["\r\n", "\r", "\n"];
    }
}
