using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonAILib.Model.Abstract
{
    public class IssueItemBase
    {

        // タイトル
        public string Title { get; set; } = "";
        // 内容
        public string Content { get; set; } = "";

        // アクション
        public string Action { get; set; } = "";

    }
}
