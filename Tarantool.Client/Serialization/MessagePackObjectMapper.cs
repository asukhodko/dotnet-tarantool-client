﻿using System;
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

        public static object Map(Type targetType, MessagePackObject source, PropertyInfo property = null)
        {
            if (targetType == typeof(string)) return source.AsString();
            if (targetType == typeof(int)) return source.AsInt32();
            if (targetType == typeof(uint)) return source.AsUInt32();
            if (targetType == typeof(long)) return source.AsInt64();
            if (targetType == typeof(ulong)) return source.AsUInt64();
            if (targetType == typeof(float)) return source.AsSingle();
            if (targetType == typeof(double)) return source.AsDouble();
            if (targetType == typeof(bool)) return source.AsBoolean();
            if (targetType == typeof(byte[])) return source.AsBinary();
            if (targetType == typeof(byte)) return source.AsByte();
            if (targetType == typeof(sbyte)) return source.AsSByte();
            if (targetType == typeof(char[])) return source.AsCharArray();
            if (targetType == typeof(short)) return source.AsInt16();
            if (targetType == typeof(ushort)) return source.AsUInt16();
            if (targetType == typeof(IList<MessagePackObject>)) return source.AsList();
            if (targetType == typeof(IEnumerable<MessagePackObject>)) return source.AsEnumerable();
            if (targetType == typeof(DateTime)) return MapDateTime(property, source);
            if (targetType == typeof(DateTimeOffset)) return MapDateTimeOffset(property, source);

            if (targetType == typeof(MessagePackObject)) return source;

            Type nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
            if (nullableUnderlyingType != null)
            {
                if (source.IsNil)
                    return null;
                else
                    return Map(nullableUnderlyingType, source, property);
            }

            var ti = targetType.GetTypeInfo();

            if (ti.IsGenericType && (targetType.GetGenericTypeDefinition() == typeof(List<>)
                                     || targetType.GetGenericTypeDefinition() == typeof(IList<>)))
                return MapList(targetType, source.AsList());

            if (ti.IsClass && (source.IsList || source.IsNil)) return MapClass(targetType, source);

            if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Dictionary<,>) && source.IsMap) return MapDictionary(targetType, source.AsDictionary());

            if (ti.IsClass && source.IsMap) return MapDictionaryToObject(targetType, source.AsDictionary());

            if (ti.IsEnum) return MapEnum(targetType, source);

            throw new MessagePackMapperException(
                $"Cannot find MsgPackObject converter for type {targetType.FullName}.");
        }

        private static object MapClass(Type targetType, MessagePackObject source)
        {
            if (source.IsNil) return null;
            var sourceFields = source.AsList();
            var target = Activator.CreateInstance(targetType);
            foreach (var property in targetType.GetRuntimeProperties())
            {
                var attr = property.GetCustomAttribute<MessagePackMemberAttribute>();
                if (attr != null && attr.Id < sourceFields.Count)
                    try
                    {
                        var msgPackValue = sourceFields[attr.Id];
                        var targetValue = Map(property.PropertyType, msgPackValue, property);
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

        private static readonly DateTime _unixEpocUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        private static object MapDateTime(PropertyInfo property, MessagePackObject source)
        {
            var conversionMethod = property?.GetCustomAttribute<MessagePackDateTimeMemberAttribute>()
                ?.DateTimeConversionMethod;
            switch (conversionMethod)
            {
                case DateTimeMemberConversionMethod.UnixEpoc:
                    return _unixEpocUtc.AddMilliseconds(source.AsInt64());
                default:
                    return DateTime.FromBinary(source.AsInt64());
            }
        }

        private static object MapDateTimeOffset(PropertyInfo property, MessagePackObject source)
        {
            var conversionMethod = property?.GetCustomAttribute<MessagePackDateTimeMemberAttribute>()
                ?.DateTimeConversionMethod;
            try
            {
                switch (conversionMethod)
                {
                    case DateTimeMemberConversionMethod.UnixEpoc:
                        return new DateTimeOffset(_unixEpocUtc.AddMilliseconds(source.AsInt64()));
                    default:
                        if (source.IsArray)
                        {
                            var arr = source.AsList();
                            return new DateTimeOffset(arr[0].AsInt64(), TimeSpan.FromMinutes(arr[1].AsInt32()));
                        }
                        else
                        {
                            return new DateTimeOffset(source.AsInt64(), TimeSpan.Zero);
                        }
                }
            }
            catch (Exception ex)
            {
                throw new MessagePackMapperException(
                            $"Cannot map field [{property.Name}] from [{source}]. See inner exception for details.",
                            ex);
            }
        }

        private static object MapDictionary(Type targetType, MessagePackObjectDictionary source)
        {
            var target = Activator.CreateInstance(targetType);
            Type keyType = targetType.GetGenericArguments()[0];
            Type valueType = targetType.GetGenericArguments()[1];
            MethodInfo addMI = targetType.GetMethod("Add", new[] { keyType, valueType });
            foreach (var pair in source)
            {
                var targetKey = Map(keyType, pair.Key);
                var targetValue = Map(valueType, pair.Value);
                addMI.Invoke(target, new[] { targetKey, targetValue });
            }

            return target;
        }

        private static object MapDictionaryToObject(Type targetType, MessagePackObjectDictionary source)
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

        private static object MapEnum(Type targetType, MessagePackObject source)
        {
            if (source.IsTypeOf<int>() ?? false) return source.AsInt32();
            if (source.IsTypeOf<string>() ?? false)
            {
                var strVal = source.AsString();
                return Enum.Parse(targetType, strVal, true);
            }

            throw new MessagePackMapperException($"Cannot map value to enum {targetType.FullName}.");
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
    }
}