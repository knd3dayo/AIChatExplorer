using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Libraries.SharpClipboardNS;

namespace WpfApp1
{
    public  class PythonCommands
    {


        public static void CreatePythonScriptCommandExecute(object obj)
        {
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return;
            }
            EditScriptWindow editScriptWindow = new EditScriptWindow();
            EditScriptWindowViewModel editScriptWindowViewModel = ((EditScriptWindowViewModel)editScriptWindow.DataContext);
            editScriptWindowViewModel.ScriptItem = new ScriptItem("", PythonExecutor.LoadPythonScript(PythonExecutor.TemplateScript), ScriptType.Python);
            editScriptWindow.ShowDialog();
        }

        public static void EditPythonScriptCommandExecute(object obj)
        {
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return;
            }
            SelectScriptWindow SelectScriptWindow = new SelectScriptWindow();
            SelectScriptWindow.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Pythonスクリプトを実行するコマンド
        //--------------------------------------------------------------------------------
        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem AutoExtractTextCommandExecute(ClipboardItem clipboardItem)
        {
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return clipboardItem;
            }
            if (MainWindowViewModel.Instance == null)
            {
                return clipboardItem;
            }
            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Files)
            {
                Tools.ShowMessage("ファイル以外のコンテンツはテキストを抽出できません");
                return clipboardItem;
            }
            try
            {
                string text = PythonExecutor.ExtractText(clipboardItem.Content);
                ClipboardItem newClipboardItem = clipboardItem.Copy();
                newClipboardItem.Content = text;
                newClipboardItem.ContentType = SharpClipboard.ContentTypes.Text;
                MainWindowViewModel.Instance?.UpdateStatusText(clipboardItem.Content + "のテキストを抽出しました");

                return newClipboardItem;
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
                return clipboardItem;
            }
        }
        // コンテキストメニューの「テキストを抽出」をクリックしたときの処理
        public static void ExtractTextCommandExecute(object obj)
        {
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return;
            }
            if (MainWindowViewModel.Instance == null)
            {
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                return;
            }
            try
            {
                string text = PythonExecutor.ExtractText(MainWindowViewModel.Instance.SelectedItem.Content);
                System.Windows.Clipboard.SetText(text);
                MainWindowViewModel.Instance?.UpdateStatusText(MainWindowViewModel.Instance.SelectedItem.Content + "から抽出したテキストをクリップボードに貼り付けました");
                Tools.ShowMessage(MainWindowViewModel.StatusText.Text);
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
            }
        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public static ClipboardItem AutoMaskDataCommandExecute(ClipboardItem clipboardItem)
        {
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return clipboardItem;
            }
            if (MainWindowViewModel.Instance == null)
            {
                return clipboardItem;
            }
            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Text)
            {
                Tools.ShowMessage("テキスト以外のコンテンツはマスキングできません");
                return clipboardItem;
            }

            try
            {
                string maskedText = PythonExecutor.MaskData(clipboardItem.Content);
                ClipboardItem newClipboardItem = clipboardItem.Copy();
                newClipboardItem.Content = maskedText;

                MainWindowViewModel.Instance?.UpdateStatusText("データをマスキングしました");
                return newClipboardItem;
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
                return clipboardItem;
            }
        }

        // コンテキストメニューの「データをマスキング」をクリックしたときの処理
        public static void MaskDataCommandExecute(object obj)
        {
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return;
            }
            if (MainWindowViewModel.Instance == null)
            {
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                return;
            }
            try
            {
                string maskedText = PythonExecutor.MaskData(MainWindowViewModel.Instance.SelectedItem.Content);
                System.Windows.Clipboard.SetText(maskedText);
                MainWindowViewModel.Instance?.UpdateStatusText("マスキングしたデータをクリップボードに貼り付けました");
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
            }
        }

        // 自動実行でPythonスクリプトを実行するコマンド
        public static ClipboardItem AutoRunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            StatusText? StatusText = MainWindowViewModel.StatusText;

            if (Instance == null)
            {
                return clipboardItem;
            }
            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return clipboardItem;
            }
            try
            {
                ClipboardItem? resultItem = PythonExecutor.RunScript(scriptItem, clipboardItem);
                MainWindowViewModel.Instance?.UpdateStatusText("Pythonスクリプトを実行しました");
                if (resultItem != null)
                {
                    // 実行結果からClipboardItemを作成
                    return resultItem;
                }
                else
                {
                    Tools.ShowMessage("スクリプトの実行結果がありません");
                    return clipboardItem;
                }
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
                return clipboardItem;
            }
        }
        // メニューの「Pythonスクリプトを実行」をクリックしたときの処理
        public static void RunPythonScriptCommandExecute(object obj)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            StatusText? StatusText = MainWindowViewModel.StatusText;

            if (PythonExecutor.IsPythonEnabled == false)
            {
                Tools.ShowMessage("Pythonが有効になっていません。設定画面でUSE_PYTHON=Trueに設定してアプリケーションを再起動してください");
                return;
            }
            if (Instance == null)
            {
                return;
            }
            if (Instance.SelectedItem == null)
            {
                return;
            }

            if (obj is not ScriptItem)
            {
                return;
            }
            ScriptItem scriptItem = (ScriptItem)obj;
            try
            {
                ClipboardItem? resultItem = PythonExecutor.RunScript(scriptItem, Instance.SelectedItem);
                MainWindowViewModel.Instance?.UpdateStatusText("Pythonスクリプトを実行しました");
                if (resultItem != null)
                {
                    // 実行結果をクリップボードにコピー
                    System.Windows.Clipboard.SetText(resultItem.Content);

                }
                else
                {
                    Tools.ShowMessage("スクリプトの実行結果がありません");
                }
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
            }

        }
        public static void DeleteScriptCommandExecute(object obj)
        {
            if (obj is ScriptItem)
            {
                ScriptItem scriptItem = (ScriptItem)obj;
                PythonExecutor.DeleteScriptItem(scriptItem);
            }
        }
        // OpenAI Chatを開くコマンド
        public static void OpenAIChatCommandExecute(object obj)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;

            // PropertiesのUSE_OPENAIを確認
            if (Properties.Settings.Default.USE_OPENAI == false)
            {
                Tools.ShowMessage("OpenAIが有効になっていません。設定画面でUSE_OPENAI=Trueに設定してください");
                return;
            }
            // PropertiesのOPENAI_API_KEYを確認
            if (Properties.Settings.Default.OPENAI_API_KEY == "")
            {
                Tools.ShowMessage("OpenAI APIキーが設定されていません。設定画面でAPIキーを設定してください");
                return;
            }
            // PropertiesのCHAT_MODEL_NAMEを確認
            if (Properties.Settings.Default.CHAT_MODEL_NAME == "")
            {
                Tools.ShowMessage("OpenAI Chatモデルが設定されていません。設定画面でChatモデルを設定してください");
                return;
            }
            OpenAIChatWindow openAIChatWindow = new OpenAIChatWindow();
            OpenAIChatWindowViewModel openAIChatWindowViewModel = ((OpenAIChatWindowViewModel)openAIChatWindow.DataContext);
            if (Instance == null)
            {
                return;
            }
            if (Instance.SelectedItem != null)
            {
                openAIChatWindowViewModel.InpupText = Instance.SelectedItem.Content;
            }
            openAIChatWindowViewModel.ChatSessionCollectionName = ClipboardController.CreateChatSessionCollectionName();
            openAIChatWindow.Show();
        }


    }
}
