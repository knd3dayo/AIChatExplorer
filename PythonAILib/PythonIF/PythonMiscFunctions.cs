using Python.Runtime;
using PythonAILib.Resource;
using PythonAILib.Utils;
using System.Drawing;

namespace PythonAILib.PythonIF
{
    public enum SpacyEntityNames {

        PERSON,
        ORG,
        GPE,
        LOC,
        PRODUCT,
        EVENT,
        WORK_OF_ART,
        LAW,
        LANGUAGE,
        DATE,
        TIME,
        PERCENT,
        MONEY,
        QUANTITY,
        ORDINAL,
        CARDINAL
    }

    public class PythonMiscFunctions : PythonNetFunctions , IPythonMiscFunctions{

        
        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        // IPythonFunctionsのメソッドを実装
        // データをマスキングする
        public string GetMaskedString(string SpacyModel, string text) {
            List<string> beforeTextList = [text];
            MaskedData maskedData = GetMaskedData(SpacyModel, beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        // IPythonFunctionsのメソッドを実装
        // マスキングされたデータを元に戻す
        public string GetUnmaskedString(string SpacyModel, string maskedText) {
            List<string> beforeTextList = [maskedText];
            MaskedData maskedData = GetMaskedData(SpacyModel, beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        public MaskedData GetMaskedData(string SpacyModel, List<string> beforeTextList) {

            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SpacyModel)) {
                throw new Exception(StringResources.SpacyModelNameNotSet);
            }
            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };

            MaskedData actionResult = new(beforeTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonMiscScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                string function_name = "mask_data";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = function_object(beforeTextList, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new Exception(StringResources.MaskingResultNotFound);
                }
                PyObject? textDictObject = resultDict.GetItem("TEXT") ?? throw new Exception(StringResources.MaskingResultFailed);

                PyDict textDict = textDictObject.As<PyDict>();
                PyList? afterList = textDict.GetItem("AFTER").As<PyList>();
                foreach (PyObject item in afterList) {
                    string? text = item.ToString();
                    if (text == null) {
                        continue;
                    }
                    actionResult.AfterTextList.Add(text);
                }
                // SpacyEntitiesNames毎に処理
                foreach (SpacyEntityNames entityName in Enum.GetValues(typeof(SpacyEntityNames))) {
                    string entityNameString = entityName.ToString();
                    PyObject? entities;
                    try {
                        entities = resultDict.GetItem(entityNameString);
                    } catch (PythonException) {
                        entities = null;
                        return;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

            });
            return actionResult;
        }

        // GetUnMaskedDataの実装
        public MaskedData GetUnMaskedData(string SpacyModel, List<string> maskedTextList) {

            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
            MaskedData actionResult = new(maskedTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonMiscScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "unmask_data";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = function_object(actionResult, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new Exception(StringResources.UnmaskingResultNotFound);
                }

                PyObject? textListObject = resultDict.GetItem("TEXT") ?? throw new Exception(StringResources.UnmaskingResultFailed);
                PyList textList = textListObject.As<PyList>();
                foreach (PyObject item in textList) {
                    PyObject afterTextObject = item.GetItem("AFTER");
                    string? text = afterTextObject.ToString();
                    if (text == null) {
                        continue;
                    }
                    actionResult.AfterTextList.Add(text);
                }
                // SpacyEntitiesNames毎に処理
                foreach (SpacyEntityNames entityName in Enum.GetValues(typeof(SpacyEntityNames))) {
                    string entityNameString = entityName.ToString();
                    PyObject? entities = resultDict.GetItem(entityNameString);
                    if (entities == null) {
                        continue;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

            });
            return actionResult;
        }
        public string ExtractTextFromImage(Image image, string tesseractExePath) {
            // Pythonスクリプトを実行する
            string result = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonMiscScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text_from_image";
                dynamic function_object = GetPythonFunction(ps, function_name);
                ImageConverter imageConverter = new();
                object? bytesObject = imageConverter.ConvertTo(image, typeof(byte[]))
                ?? throw new Exception(StringResources.ImageByteFailed);
                byte[] bytes = (byte[])bytesObject;
                // extract_text_from_image関数を呼び出す。戻り値は{ "text": "抽出したテキスト" , "log": "ログ" }の形式
                Dictionary<string, string> dict = new();
                PyDict? pyDict = function_object(bytes, tesseractExePath);
                if (pyDict == null) {
                    throw new Exception("pyDict is null");
                }
                // textを取得
                PyObject? textObject = pyDict.GetItem("text");
                if (textObject == null) {
                    throw new Exception("textObject is null");
                }
                result = textObject.ToString() ?? "";
                // logを取得
                PyObject? logObject = pyDict.GetItem("log");
                if (logObject != null) {
                    string log = logObject.ToString() ?? "";
                    LogWrapper.Info($"log:{log}");
                }

            });
            return result;
        }

        private List<MaskedEntity> GetMaskedEntities(string label, PyDict pyDict) {

            List<MaskedEntity> result = [];
            foreach (var key in pyDict.Keys()) {
                PyObject? entity = pyDict.GetItem(key);
                if (entity == null) {
                    continue;
                }
                string? keyString = key.ToString();
                if (keyString == null) {
                    continue;
                }
                string? entityString = entity.ToString();
                if (entityString == null) {
                    continue;
                }
                MaskedEntity maskedEntity = new() {
                    Label = label,
                    Before = keyString,
                    After = entityString
                };
                result.Add(maskedEntity);
            }
            return result;
        }


        // IPythonFunctionsのメソッドを実装
        // スクリプトの内容とJSON文字列を引数に取り、結果となるJSON文字列を返す
        public string RunScript(string script, string input) {
            string resultString = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonMiscScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                string function_name = "run_script";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // run_script関数を呼び出す
                resultString = function_object(script, input);
            });
            return resultString;

        }

        // IPythonFunctionsのメソッドを実装
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {

            HashSet<string> actionResult = [];
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonMiscScript, (ps) => {

                // SPACY_MODEL_NAMEが空の場合は例外をスロー
                if (string.IsNullOrEmpty(SpacyModel)) {
                    throw new Exception(StringResources.SpacyModelNameNotSet);
                }

                Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
                // 結果用のDictionaryを作成
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_entity";
                dynamic function_object = GetPythonFunction(ps, function_name);
                PyIterable pyIterable = function_object(text, dict);
                // PythonのリストをC#のHashSetに変換
                foreach (PyObject item in pyIterable) {
                    string? entity = item.ToString();
                    if (entity != null) {
                        actionResult.Add(entity);
                    }
                }
            });
            return actionResult;
        }

    }
}
