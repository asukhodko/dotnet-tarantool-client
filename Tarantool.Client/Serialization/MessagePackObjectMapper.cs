using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Client.Serialization
{
    public class MessagePackObjectMapper
    {
        public static T Map<T>(MessagePackObject source)
        {
            return (T)Map(typeof(T), source);
        }

        public static object Map(Type targetType, MessagePackObject source)
        {
            if (targetType == typeof(string))
                return source.AsString();
            if (targetType == typeof(int))
                return source.AsInt32();
            if (targetType == typeof(uint))
                return source.AsUInt32();
            if (targetType == typeof(long))
                return source.AsInt64();
            if (targetType == typeof(ulong))
                return source.AsUInt64();
            if (targetType == typeof(float))
                return source.AsSingle();
            if (targetType == typeof(double))
                return source.AsDouble();
            if (targetType == typeof(bool))
                return source.AsBoolean();
            if (targetType == typeof(byte[]))
                return source.AsBinary();
            if (targetType == typeof(byte))
                return source.AsByte();
            if (targetType == typeof(sbyte))
                return source.AsSByte();
            if (targetType == typeof(char[]))
                return source.AsCharArray();
            if (targetType == typeof(short))
                return source.AsInt16();
            if (targetType == typeof(ushort))
                return source.AsUInt16();
            if (targetType == typeof(DateTime))
                return DateTime.FromBinary(source.AsInt64());
            if (targetType == typeof(IList<MessagePackObject>))
                return source.AsList();
            if (targetType == typeof(IEnumerable<MessagePackObject>))
                return source.AsEnumerable();

            var ti = targetType.GetTypeInfo();

            if (targetType == typeof(MessagePackObject))
                return source;

            if (ti.IsGenericType && (targetType.GetGenericTypeDefinition() == typeof(List<>) ||
                                     targetType.GetGenericTypeDefinition() == typeof(IList<>)))
                return MapList(targetType, source.AsList());

            if (ti.IsClass && source.IsList)
                return MapClass(targetType, source);

            if (ti.IsClass && source.IsMap)
                return MapDictionary(targetType, source.AsDictionary());

            if (ti.IsEnum)
                return MapEnum(targetType, source);

            throw new MessagePackMapperException(
                $"Cannot find MsgPackObject converter for type {targetType.FullName}.");
        }

        private static object MapList(Type targetType, IList<MessagePackObject> source)
        {
            var entryType = targetType.GenericTypeArguments[0];
            var addMethod = targetType.GetRuntimeMethod("Add", new[] { entryType });
            var target = Activator.CreateInstance(targetType);
            foreach (var o in source)
            {
                var value = Map(entryType, o);
                addMethod.Invoke(target, new[] { value });
            }
            return target;
        }

        private static object MapDictionary(Type targetType, MessagePackObjectDictionary source)
        {
            var target = Activator.CreateInstance(targetType);
            var props = targetType.GetRuntimeProperties().ToList();
            foreach (var pair in source)
            {
                var property = props.FirstOrDefault(x => x.Name.ToLower() == pair.Key.AsString().ToLower());
                if (property != null)
                {
                    var targetValue = Map(property.PropertyType, pair.Value);
                    property.SetValue(target, targetValue);
                }
            }
            return target;
        }

        private static object MapClass(Type targetType, MessagePackObject source)
        {
            var sourceFields = source.AsList();
            var target = Activator.CreateInstance(targetType);
            foreach (var property in targetType.GetRuntimeProperties())
            {
                var attr = property.GetCustomAttribute<MessagePackMemberAttribute>();
                if (attr != null && attr.Id < sourceFields.Count)
                    try
                    {
                        var msgPackValue = sourceFields[attr.Id];
                        var targetValue = Map(property.PropertyType, msgPackValue);
                        property.SetValue(target, targetValue);
                    }
                    catch (Exception ex)
                    {
                        throw new MessagePackMapperException(
                            $"Cannot map field [{property.Name}] from position [{attr.Id}]. See inner exception for details.",
                            ex);
                    }
            }
            return target;
        }

        private static object MapEnum(Type targetType, MessagePackObject source)
        {
            if (source.IsTypeOf<int>() ?? false)
                return source.AsInt32();
            if (source.IsTypeOf<string>() ?? false)
            {
                var strVal = source.AsString();
                return Enum.Parse(targetType, strVal, true);
            }
            throw new MessagePackMapperException($"Cannot map value to enum {targetType.FullName}.");
        }
    }
}