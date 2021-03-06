﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using NetRpc.Http.Client;

namespace NetRpc.Http
{
    internal static class Helper
    {
        public static HttpDataObj ToHttpDataObj(string json, Type t)
        {
            var obj = ToObjectForHttp(json, t);
            if (obj == null)
                return null;

            var (connectionId, callId, streamLength) = GetAdditionData(obj);

            return new HttpDataObj
            {
                StreamLength = streamLength,
                Value = obj,
                CallId = callId,
                ConnectionId = connectionId,
                Type = t
            };
        }

        public static object ToObjectForHttp(string json, Type t)
        {
            object dataObj;
            try
            {
                dataObj = json.ToDtoObject(t);
            }
            catch (Exception e)
            {
                throw new HttpFailedException($"{e.Message}, str:\r\n{json}");
            }

            return dataObj;
        }

        public static string FormatPath(string path)
        {
            path = path.Replace('\\', '/');
            path = path.Replace("//", "/");
            path = path.TrimStart('/');
            return path;
        }

        public static object[] GetPureArgsFromDataObj(Type dataObjType, object dataObj)
        {
            var ret = new List<object>();
            if (dataObjType == null)
                return ret.ToArray();
            foreach (var p in dataObjType.GetProperties())
                ret.Add(GetPropertyValue(dataObj, p.Name));
            return ret.ToArray();
        }

        public static bool IsEqualsOrSubclassOf(this Type t, Type c)
        {
            return t == c || t.IsSubclassOf(c);
        }

        public static List<string> GetCommentsXmlPaths()
        {
            var ret = new List<string>();
            var root = AppContext.BaseDirectory;
            foreach (var file in Directory.GetFiles(root))
            {
                if (string.Equals(Path.GetExtension(file), ".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var dll = GetFullPathWithoutExtension(file) + ".dll";
                    var exe = GetFullPathWithoutExtension(file) + ".exe";
                    if (File.Exists(dll) || File.Exists(exe))
                        ret.Add(file);
                }
            }

            return ret;
        }

        public static string GetFullPathWithoutExtension(string s)
        {
            var dir = Path.GetDirectoryName(s);
            if (dir == null)
                dir = "";
            var name = Path.GetFileNameWithoutExtension(s);
            if (name == null)
                name = "";
            return Path.Combine(dir, name);
        }

        public static object GetPropertyValue(object classInstance, string propertyName)
        {
            return classInstance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty,
                null, classInstance, new object[] { });
        }

        private static (string connectionId, string callId, long streamLength) GetAdditionData(object dataObj)
        {
            if (dataObj == null)
                return (null, null, 0);

            var connectionId = (string) GetValue(dataObj, ClientConstValue.ConnectionIdName);
            var callId = (string) GetValue(dataObj, ClientConstValue.CallIdName);
            var streamLengthObj = GetValue(dataObj, ClientConstValue.StreamLength);
            var streamLength = (long?) streamLengthObj ?? 0;
            return (connectionId, callId, streamLength);
        }

        private static object GetValue(object obj, string propertyName)
        {
            var pi = obj.GetType().GetProperty(propertyName);
            if (pi == null)
                return null;
            return pi.GetValue(obj);
        }
    }
}