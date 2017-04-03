using System;
using System.Collections.Generic;
using System.Reflection;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Client.Serialization
{
    public class MessagePackObjectMapper<T>
    {
        public void Map(MessagePackObject source, T target)
        {
            var sourceFields = source.AsList();
            foreach (var property in typeof(T).GetRuntimeProperties())
            {
                var attr = property.GetCustomAttribute<MessagePackMemberAttribute>();
                if (attr != null)
                {
                    try
                    {
                        var val = sourceFields[attr.Id];
                        SetPropertyValue(target, property, val);
                    }
                    catch (Exception ex)
                    {
                        throw new MessagePackMapperException($"Cannot map field [{property.Name}] from position [{attr.Id}]. See inner exception.", ex);
                    }
                }
            }
        }

        private static void SetPropertyValue(T target, PropertyInfo property, MessagePackObject val)
        {
            var t = property.PropertyType;
            if (t == typeof(string))
                property.SetValue(target, val.AsString());
            else if (t == typeof(int))
                property.SetValue(target, val.AsInt32());
            else if (t == typeof(uint))
                property.SetValue(target, val.AsUInt32());
            else if (t == typeof(long))
                property.SetValue(target, val.AsInt64());
            else if (t == typeof(ulong))
                property.SetValue(target, val.AsUInt64());
            else if (t == typeof(float))
                property.SetValue(target, val.AsSingle());
            else if (t == typeof(double))
                property.SetValue(target, val.AsDouble());
            else if (t == typeof(bool))
                property.SetValue(target, val.AsBoolean());
            else if (t == typeof(byte[]))
                property.SetValue(target, val.AsBinary());
            else if (t == typeof(byte))
                property.SetValue(target, val.AsByte());
            else if (t == typeof(sbyte))
                property.SetValue(target, val.AsSByte());
            else if (t == typeof(char[]))
                property.SetValue(target, val.AsCharArray());
            else if (t == typeof(short))
                property.SetValue(target, val.AsInt16());
            else if (t == typeof(ushort))
                property.SetValue(target, val.AsUInt16());
            else if (t == typeof(IList<MessagePackObject>))
                property.SetValue(target, val.AsList());
            else if (t == typeof(IEnumerable<MessagePackObject>))
                property.SetValue(target, val.AsEnumerable());
        }
    }
}
