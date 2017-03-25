using System;
using System.Reflection;

namespace Tarantool.Client.Helpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static string GetStringValue(Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetRuntimeField(enumVal.ToString());
            var attribute = memInfo.GetCustomAttribute<StringValueAttribute>();
            return attribute.Value;
        }
    }
}