using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
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
