﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataContract
{
    public interface IService
    {
        Task<Ret> Call(InParam p, int i, Stream stream, Action<int> progs, CancellationToken token);

        Task<string> Call2(string s);
    }

    [Serializable]
    public class InParam
    {
        public string P1 { get; set; }
    }

    [Serializable]
    public class Ret
    {
        public string P1 { get; set; }

        [field:NonSerialized]
        public Stream Stream { get; set; }
    }
}