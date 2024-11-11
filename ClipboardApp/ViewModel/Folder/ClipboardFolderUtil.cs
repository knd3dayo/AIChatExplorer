using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using PythonAILib.Model.Prompt;
using PythonAILib.PythonIF;
using QAChat.Resource;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.ViewModel.Folder {
    public class ClipboardFolderUtil {

        #region static methods
        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(CommonStringResources toRes) {
            // ClipboardRootFolder
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
            ClipboardFolder? clipboardRootFolder = collection.FindAll().Where(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Normal).FirstOrDefault();
            if (clipboardRootFolder != null) {
                clipboardRootFolder.FolderName = toRes.Clipboard;
                clipboardRootFolder.Save();
            }
            // SearchRootFolder
            ClipboardFolder? searchRootFolder = collection.FindAll().Where(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Search).FirstOrDefault();
            if (searchRootFolder != null) {
                searchRootFolder.FolderName = toRes.SearchFolder;
                searchRootFolder.Save();
            }
            // ChatRootFolder
            ClipboardFolder? chatRootFolder = collection.FindAll().Where(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Chat).FirstOrDefault();
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
                chatRootFolder.Save();
            }
            // ImageCheckRootFolder
            ClipboardFolder? imageCheckRootFolder = collection.FindAll().Where(x => x.ParentId == null && x.FolderType == FolderTypeEnum.ImageCheck).FirstOrDefault();
            if (imageCheckRootFolder != null) {
                imageCheckRootFolder.FolderName = toRes.ImageChat;
                imageCheckRootFolder.Save();
            }
        }

        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new();

        //--------------------------------------------------------------------------------
        public static ClipboardFolder RootFolder {
            get {
                var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
                ClipboardFolder? clipboardRootFolder = collection.FindAll().Where(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Normal).FirstOrDefault();
                if (clipboardRootFolder == null) {
                    clipboardRootFolder = new() {
                        FolderName = ClipboardFolder.CLIPBOARD_ROOT_FOLDER_NAME,
                        IsRootFolder = true,
                        IsAutoProcessEnabled = true,
                        FolderType = FolderTypeEnum.Normal
                    };
                    clipboardRootFolder.Save();
                }
                // 既にRootFolder作成済みの環境のための措置
                clipboardRootFolder.IsRootFolder = true;
                return clipboardRootFolder;
            }
        }

        public static ClipboardFolder SearchRootFolder {
            get {
                var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
                ClipboardFolder? searchRootFolder = collection.FindAll().Where(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Search).FirstOrDefault();
                if (searchRootFolder == null) {
                    searchRootFolder = new ClipboardFolder {
                        FolderName = ClipboardFolder.SEARCH_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.Search,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false
                    };
                    searchRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                searchRootFolder.IsRootFolder = true;
                searchRootFolder.FolderType = FolderTypeEnum.Search;
                return searchRootFolder;
            }
        }

        public static ClipboardFolder ChatRootFolder {
            get {
                var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
                ClipboardFolder? chatRootFolder = collection.FindAll().Where(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Chat).FirstOrDefault();
                if (chatRootFolder == null) {
                    chatRootFolder = new ClipboardFolder {
                        FolderName = ClipboardFolder.CHAT_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.Chat,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false
                    };
                    chatRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                chatRootFolder.IsRootFolder = true;
                return chatRootFolder;
            }
        }
        #endregion


        /// Process clipboard item 
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_afterClipboardChanged"></param>
        public static void ProcessClipboardItem(ClipboardItem item, Action<ClipboardItem> _afterClipboardChanged) {

            // Execute in a separate thread
            Task.Run(() => {
                StatusText statusText = Tools.StatusText;
                MainUITask.Run(() => {
                    statusText.InProgressText = CommonStringResources.Instance.AutoProcessing;
                    statusText.IsInProgress = true;
                });
                try {
                    // Apply automatic processing
                    Task<ClipboardItem?> updatedItemTask = ApplyAutoAction(item);
                    if (updatedItemTask.Result == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItemTask.Result);

                } catch (Exception ex) {
                    LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");

                } finally {
                    MainUITask.Run(() => {
                        statusText.IsInProgress = false;
                    });
                }
            });
        }


        /// Create ContentItem
        public static List<ClipboardItem> CreateClipboardItem(
            ClipboardFolder clipboardFolder, ClipboardChangedEventArgs e) {

            List<ClipboardItem> result = [];

            PythonAILib.Model.File.ContentTypes.ContentItemTypes contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text;
            if (e.ContentType == ContentTypes.Text) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text;
            } else if (e.ContentType == ContentTypes.Files) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files;
            } else if (e.ContentType == ContentTypes.Image) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image;
            } else if (e.ContentType == ContentTypes.Other) {
                return result;
            } else {
                return result;
            }

            // If ContentType is Text, set text data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                ClipboardItem item = new(clipboardFolder.Id) {
                    ContentType = contentTypes
                };
                SetApplicationInfo(item, e);
                item.Content = (string)e.Content;
                result.Add(item);
                return result;
            }

            // If ContentType is BitmapImage, set image data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image) {
                ClipboardItem item = new(clipboardFolder.Id) {
                    ContentType = contentTypes
                };
                SetApplicationInfo(item, e);
                System.Drawing.Image image = (System.Drawing.Image)e.Content;
                // byte
                item.CachedBase64String = PythonAILib.Model.File.ContentTypes.GetBase64StringFromImage(image);
                result.Add(item);
                return result;
            }

            // If ContentType is Files, set file data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ClipboardItem item = new(clipboardFolder.Id) {
                        ContentType = contentTypes
                    };
                    SetApplicationInfo(item, e);
                    item.FilePath = files[i];
                    item.LastModified = new System.IO.FileInfo(item.FilePath).LastWriteTime.Ticks;
                    result.Add(item);
                }
                return result;
            }
            return result;
        }

        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        public static void SetApplicationInfo(ClipboardItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        private static async Task<ClipboardItem?> ApplyAutoAction(ClipboardItem item) {
            // ★TODO Implement processing based on automatic processing rules.
            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text && lineCount <= ClipboardAppConfig.Instance.IgnoreLineCount) {
                return null;
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            if (ClipboardAppConfig.Instance.AutoMergeItemsBySourceApplicationTitle) {
                LogWrapper.Info(CommonStringResources.Instance.AutoMerge);
                RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
            }
            // If AutoFileExtract is set, extract files
            if (ClipboardAppConfig.Instance.AutoFileExtract && item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(item.FilePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (ClipboardAppConfig.Instance.AutoExtractImageWithPyOCR) {
                    string extractImageText = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(item.Image, ClipboardAppConfig.Instance.TesseractExePath);
                    item.Content += "\n" + extractImageText;
                    LogWrapper.Info(CommonStringResources.Instance.OCR);

                } else if (ClipboardAppConfig.Instance.AutoExtractImageWithOpenAI) {

                    LogWrapper.Info(CommonStringResources.Instance.AutoExtractImageText);
                    item.ExtractImageWithOpenAI();
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            var task1 = Task.Run(() => {
                // If AUTO_TAG is set, automatically set the tags
                if (ClipboardAppConfig.Instance.AutoTag) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTag);
                    ClipboardItem.CreateAutoTags(item);
                }
            });
            var task2 = Task.Run(() => {
                // If AUTO_DESCRIPTION is set, automatically set the Description
                if (ClipboardAppConfig.Instance.AutoDescription) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                    ClipboardItem.CreateAutoTitle(item);

                } else if (ClipboardAppConfig.Instance.AutoDescriptionWithOpenAI) {

                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                    item.CreateAutoTitleWithOpenAI();
                }
            });
            var task3 = Task.Run(() => {
                // 背景情報
                if (ClipboardAppConfig.Instance.AutoBackgroundInfo) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetBackgroundInfo);
                    item.CreateAutoBackgroundInfo();
                }
            });
            var task4 = Task.Run(() => {
                // サマリー
                if (ClipboardAppConfig.Instance.AutoSummary) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCreateSummary);
                    item.CreateChatResult(SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(() => {
                // Tasks
                if (ClipboardAppConfig.Instance.AutoGenerateTasks) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCreateTaskList);
                    item.CreateChatResult(SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(() => {
                // Tasks
                if (ClipboardAppConfig.Instance.AutoDocumentReliabilityCheck) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCheckDocumentReliability);
                    item.CheckDocumentReliability();
                }
            });

            await Task.WhenAll(task1, task2, task3, task4, task5, task6);

            return item;
        }



    }
}
