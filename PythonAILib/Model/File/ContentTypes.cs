using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace PythonAILib.Model.File
{
    public class ContentTypes
    {

        public enum ContentItemTypes
        {
            Text,
            Files,
            Image,
            ImageFile,
            Unknown
        }

        public enum ImageType { png, jpg, gif, webp, unknown }

        public static bool IsImageFile(string filePath)
        {
            ImageType imageType = GetImageTypeFromFilePath(filePath);
            return imageType != ImageType.unknown;
        }
        public static bool IsImageData(string base64String)
        {
            ImageType imageType = GetImageTypeFromBase64(base64String);
            return imageType != ImageType.unknown;
        }
        public static bool IsImageData(byte[] byteData)
        {
            string base64String = Convert.ToBase64String(byteData);
            ImageType imageType = GetImageTypeFromBase64(base64String);
            return imageType != ImageType.unknown;
        }

        public static ImageType GetImageTypeFromFilePath(string filePath)
        {

            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            string base64string = Convert.ToBase64String(imageBytes);
            return GetImageTypeFromBase64(base64string);
        }

        public static ImageType GetImageTypeFromBase64(string base64String)
        {
            // 先頭の文字列からイメージのフォーマットを判別
            // PNG  iVBOR
            // gif  R0lGO
            // jpeg  /9j/4
            // webp RIFF____WEBP
            // となる
            if (base64String.Length < 5)
            {
                return ImageType.unknown;
            }
            string base64Header = base64String.Substring(0, 5);
            ImageType imageType = ImageType.unknown;
            if (base64Header == "iVBOR")
            {
                imageType = ImageType.png;
            }
            else if (base64Header == "R0lGO")
            {
                imageType = ImageType.gif;
            }
            else if (base64Header == "/9j/4")
            {
                imageType = ImageType.jpg;
            }
            else if (base64Header == "RIFF")
            {
                // webpの場合は、拡張子をwebpにする
                string riffHeader = base64String.Substring(8, 4);
                if (riffHeader == "WEBP")
                {
                    imageType = ImageType.webp;
                }
            }
            return imageType;
        }
        public static System.Drawing.Image GetImageFromBase64(string base64String)
        {
            byte[] binaryData = Convert.FromBase64String(base64String);
            MemoryStream ms = new(binaryData, 0, binaryData.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        public static BitmapImage GetBitmapImage(byte[] binaryData)
        {
            MemoryStream ms = new(binaryData, 0, binaryData.Length);
            BitmapImage bi = new();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }


        public static string GetBase64StringFromImage(System.Drawing.Image image)
        {
            MemoryStream ms = new();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteData = ms.ToArray();
            return Convert.ToBase64String(byteData);
        }


    }
}
