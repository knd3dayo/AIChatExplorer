using System.IO;
using System.Windows.Media.Imaging;

namespace LibPythonAI.Model.Image {
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
        public System.Drawing.Image? Image {
            get {
                if (string.IsNullOrEmpty(ImagePath)) {
                    return null;
                }
                if (!System.IO.File.Exists(ImagePath)) {
                    return null;
                }
                return System.Drawing.Image.FromFile(ImagePath);
            }
        }

        // 画像のBitmapImage
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
        public System.Drawing.Image? Thumbnail {
            get {
                return Image?.GetThumbnailImage(100, 100, () => false, nint.Zero);
            }
        }
        // 画像のサムネイルのBitmapImage
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
