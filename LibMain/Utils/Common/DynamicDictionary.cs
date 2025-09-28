using System.Dynamic;

namespace LibMain.Utils.Common {
    // The class derived from DynamicObject.
    public class DynamicDictionary : DynamicObject {
        // The inner dictionary.
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object? result) {
            string name = binder.Name;
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value) {
            if (value == null) {
                dictionary.Remove(binder.Name);
            } else {
                dictionary[binder.Name] = value;
            }
            return true;
        }
    }
}
