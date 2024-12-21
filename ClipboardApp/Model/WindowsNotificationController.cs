using ClipboardApp.Model.Folder;
using PythonAILib.PythonIF;
using QAChat.Resource;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model
{
    public class WindowsNotificationController {
        private static bool _Enabled = false;
        public static void Start(ClipboardFolder clipboardFolder, Action<ClipboardItem> afterUpdate) {

            _Enabled = true;
            Task.Run(() => StartTask(clipboardFolder, afterUpdate));
        }
        public static void Stop() {

            _Enabled = false;
        }
        private static async void StartTask(ClipboardFolder folder, Action<ClipboardItem> afterUpdate) {

            if (!ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener")) {
                LogWrapper.Error("UserNotificationListener is not available");
                return;
            }
            UserNotificationListener listener = UserNotificationListener.Current;
            Console.Write("listener: ");
            Console.WriteLine(listener);

            UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();
            if (accessStatus != UserNotificationListenerAccessStatus.Allowed) {
                LogWrapper.Error(CommonStringResources.Instance.AccessDenied);
                return;
            }

            while (true) {
                // _EnabledがFalseの場合は終了
                if (!_Enabled) {
                    break;
                }
                DateTime lastCheckedTime = MiscConfig.WindowsNotificationLastCheckedTime;

                IReadOnlyList<UserNotification> notifiedItems = await listener.GetNotificationsAsync(NotificationKinds.Toast);

                foreach (var n in notifiedItems) {
                    // 通知の時間が最終チェック時間よりも前の場合はスキップ
                    if (n.CreationTime < lastCheckedTime) {
                        continue;
                    }
                    NotificationBinding toastBinding = n.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                    if (toastBinding != null) {
                        IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();

                        // string? titleText = textElements.FirstOrDefault()?.Text;

                        // string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                        string displayName = n.AppInfo.DisplayInfo.DisplayName;
                        string text = string.Join("\n", textElements.Select(t => t.Text));
                        // ClipboardItemの作成
                        ClipboardItem clipboardItem = new(folder.Id) {
                            SourceApplicationTitle = displayName,
                            Content = text
                        };
                        FolderManager.ProcessClipboardItem(clipboardItem, (item) => {
                            afterUpdate(item);
                        });

                    }
                    Thread.Sleep(500);

                }
                // 最終チェック時間を更新
                MiscConfig.WindowsNotificationLastCheckedTime = DateTime.Now;
                MiscConfig.Save();

                Thread.Sleep(1000 * 5);

            }

        }

    }
}
