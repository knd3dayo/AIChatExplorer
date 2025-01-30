namespace PythonAILib.Model.Image {
    public class ScreenShotCheckItem {

        // チェック条件のリスト
        public List<ScreenShotCheckCondition> ScreenShotCheckIConditions { get; set; } = [];

        // InputText
        public string InputText { get; set; } = "";

        // ResultText
        public string ResultText { get; set; } = "";

        // ImageFilePaths
        public List<ScreenShotImage> ScreenShotImages { get; set; } = [];

    }
}
