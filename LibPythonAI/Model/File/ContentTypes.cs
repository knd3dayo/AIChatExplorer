using System.IO;
using System.Windows.Media.Imaging;

namespace PythonAILib.Model.File {
    public class ContentTypes {

        public enum ContentItemTypes {
            Text,
            Files,
            Image,
            ImageFile,
            Unknown
        }

        public enum ImageType { png, jpg, gif, webp, unknown }

        public static (bool,ImageType) IsImageData(byte[] buffer) {
            // pngかどうか
            if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4e && buffer[3] == 0x47) {
                return (true, ImageType.png);  
            }
            // jpegかどうか
            if (buffer[0] == 0xff && buffer[1] == 0xd8) {
                return (true, ImageType.jpg);
            }
            // gifかどうか
            if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38) {
                return (true, ImageType.gif);
            }
            // webpかどうか
            // 	webpStartBytes = []byte{0x52, 0x49, 0x46, 0x46} // 'R' 'I' 'F' 'F'
            //  webpFormatChar = []byte{ 0x57, 0x45, 0x42, 0x50} // 'W' 'E' 'B' 'P'
            if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46) {
                if (buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50) {
                    return (true, ImageType.webp);
                }
            }
            return (false, ImageType.unknown);

        }
        public static (bool, ImageType) IsImageFile(string path) {
            // ファイルの先頭8バイトを取得
            byte[] buffer = new byte[12];
            using (FileStream fs = new(path, FileMode.Open, FileAccess.Read)) {
                fs.Read(buffer, 0, 12);
            }
            return IsImageData(buffer);
        }

        public static (bool, ImageType) GetImageTypeFromBase64(string base64String) {
            // 先頭の文字列からイメージのフォーマットを判別
            // PNG  iVBOR
            // gif  R0lGO
            // jpeg  /9j/4
            // webp RIFF____WEBP
            // となる
            if (base64String.Length < 5) {
                return (false, ImageType.unknown);
            }
            string base64Header = base64String.Substring(0, 5);
            if (base64Header == "iVBOR") {
                return (true, ImageType.png);
            } else if (base64Header == "R0lGO") {
                return (true, ImageType.gif);
            } else if (base64Header == "/9j/4") {
                return (true, ImageType.jpg);
            } else if (base64Header == "RIFF") {
                // webpの場合は、拡張子をwebpにする
                string riffHeader = base64String.Substring(8, 4);
                if (riffHeader == "WEBP") {
                    return (true, ImageType.webp);
                }
            }
            return (false, ImageType.unknown);
        }
        public static System.Drawing.Image GetImageFromBase64(string base64String) {
            byte[] binaryData = Convert.FromBase64String(base64String);
            MemoryStream ms = new(binaryData, 0, binaryData.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        public static BitmapImage GetBitmapImage(byte[] binaryData) {
            MemoryStream ms = new(binaryData, 0, binaryData.Length);
            BitmapImage bi = new();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }


        public static string GetBase64StringFromImage(System.Drawing.Image image) {
            MemoryStream ms = new();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteData = ms.ToArray();
            return Convert.ToBase64String(byteData);
        }


    }
}
