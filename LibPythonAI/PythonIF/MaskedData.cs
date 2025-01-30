namespace PythonAILib.PythonIF {
    public class MaskedData {
        public HashSet<MaskedEntity> Entities { get; set; } = [];
        public List<string> BeforeTextList { get; set; } = [];
        public List<string> AfterTextList { get; set; } = [];

        public MaskedData(List<string> beforeList) {

            foreach (var before in BeforeTextList) {
                BeforeTextList.Add(before);
            }
        }
    }
}
