using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;

namespace WpfAppCommon.Model {
    public class ScreenShotImage {

        // 画像のパス
        public string ImagePath { get; set; } = "";
        // 画像のファイル名
        public string FileName {
            get {
                return Path.GetFileName(ImagePath);
            }
        }

        // 画像
        [BsonIgnore]
        public Image? Image {
            get {
                if (string.IsNullOrEmpty(ImagePath)) {
                    return null;
                }
                if (!File.Exists(ImagePath)) {
                    return null;
                }
                return Image.FromFile(ImagePath);
            }
        }

        // 画像のBitmapImage
        [BsonIgnore]
        public BitmapImage? BitmapImage {
            get {
                if (Image == null) {
                    return null;
                }
                MemoryStream ms = new();
                Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bi = new();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }

        // 画像のサムネイル
        [BsonIgnore]
        public Image? Thumbnail {
            get {
                return Image?.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
            }
        }
        // 画像のサムネイルのBitmapImage
        [BsonIgnore]
        public BitmapImage? ThumbnailBitmapImage {
            get {
                if (Thumbnail == null) {
                    return null;
                }
                MemoryStream ms = new();
                Thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bi = new();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }


    }
}
