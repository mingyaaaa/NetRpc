﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NetRpc.Http
{
    internal static class Helper
    {
        public const string ConnectionIdName = "_connectionId";
        public const string CallIdName = "_callId";

        public static object ToObject(this string str, Type t)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            return JsonConvert.DeserializeObject(str, t);
        }

        public static string ToJson<T>(this T obj)
        {
            if (obj == null)
                return null;
            return JsonConvert.SerializeObject(obj);
        }

        public static string FormatPath(string path)
        {
            path = path.Replace('\\', '/');
            path = path.Replace("//", "/");
            path = path.TrimStart('/');
            return path;
        }

        public static bool HasStream(this Type t)
        {
            if (t == typeof(Stream))
                return true;

            var propertyInfos = t.GetProperties();
            return propertyInfos.Any(i => i.PropertyType == typeof(Stream));
        }

        public static Type GetTypeFromReturnTypeDefinition(Type returnTypeDefinition)
        {
            if (returnTypeDefinition.IsGenericType && returnTypeDefinition.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var at = returnTypeDefinition.GetGenericArguments()[0];
                return at;
            }

            return returnTypeDefinition;
        }

        public static Type GetArgType(MethodInfo m, out string streamName, out TypeName action, out TypeName cancelToken)
        {
            streamName = null;
            action = null;
            cancelToken = null;

            var typeName = $"{m.Name}Param";
            var t = ClassHelper.BuildType(typeName);
            var cis = new List<ClassHelper.CustomsPropertyInfo>();

            foreach (var p in m.GetParameters())
            {
                if (p.ParameterType == typeof(Stream))
                {
                    streamName = p.Name;
                    continue;
                }

                if (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(Action<>))
                {
                    action = new TypeName
                    {
                        Type = p.ParameterType,
                        Name = p.Name
                    };

                    //connectionId callId
                    cis.Add(new ClassHelper.CustomsPropertyInfo(typeof(string), ConnectionIdName));
                    cis.Add(new ClassHelper.CustomsPropertyInfo(typeof(string), CallIdName));
                    continue;
                }

                if (p.ParameterType == typeof(CancellationToken?) || p.ParameterType == typeof(CancellationToken))
                {
                    cancelToken = new TypeName
                    {
                        Type = p.ParameterType,
                        Name = p.Name
                    };
                    continue;
                }

                cis.Add(new ClassHelper.CustomsPropertyInfo(p.ParameterType, p.Name));
            }

            t = ClassHelper.AddProperty(t, cis);
            if (cis.Count == 0)
                return null;
            return t;
        }

        public static object[] GetArgsFromDataObj(Type dataObjType, object dataObj)
        {
            List<object> ret = new List<object>();
            foreach (var p in dataObjType.GetProperties())
                ret.Add(ClassHelper.GetPropertyValue(dataObj, p.Name));
            return ret.ToArray();
        }

        public static object ToObjectForHttp(string str, Type t)
        {
            object dataObj;
            try
            {
                dataObj = str.ToObject(t);
            }
            catch (Exception e)
            {
                throw new HttpFailedException($"{e.Message}, str:\r\n{str}");
            }

            return dataObj;
        }

        public static object[] GetArgsFromQuery(IQueryCollection query, Type dataObjType)
        {
            if (dataObjType == null)
                return new object[0];

            var ret = new List<object>();
            foreach (var p in dataObjType.GetProperties())
            {
                if (query.TryGetValue(p.Name, out var values))
                {
                    try
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            ret.Add(values[0]);
                            continue;
                        }

                        var v = p.PropertyType.GetMethod("Parse", new[] { typeof(string) }).Invoke(null, new object[] { values[0] });
                        ret.Add(v);
                    }
                    catch (Exception ex)
                    {
                        throw new HttpNotMatchedException($"'{p.Name}' is not valid value, {ex.Message}");
                    }
                }
                else
                {
                    ret.Add(ClassHelper.GetDefault(p.PropertyType));
                }
            }
            return ret.ToArray();
        }
    }
}