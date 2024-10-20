using System.ComponentModel;
using System.Reflection;

namespace WpfAppCommon.Utils {
    public class EnumDescription {

        public static string GetEnumDescription<T>(object value) {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new();

            string description = string.Empty;
            if (value == null) {
                return description;
            }
            string? strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue)) {
                return description;
            }
            description = strValue;

            FieldInfo? fieldInfo = type.GetField(strValue);
            if (fieldInfo == null) {
                return description;
            }

            Attribute? attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
            if (attribute == null) {
                return description;
            }
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)attribute;
            description = descriptionAttribute.Description;

            return description;
        }

        public static T? GetEnumValue<T>(string description) {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Not EnumType");
            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields) {
                if (field == null) {
                    continue;
                }
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null && attribute.Description == description) {
                    object? o = field.GetValue(null);
                    if (o != null) {
                        return (T)o;
                    }
                }
            }
            return default;
        }

    }
}
