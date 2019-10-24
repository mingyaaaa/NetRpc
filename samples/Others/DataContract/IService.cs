﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataContract
{
    public interface IService
    {
        Task<Ret> Call(InParam p, Stream stream, Action<int> progs, CancellationToken token);

        Task<string> Call2(string s);
    }

    public class InParam
    {
        public string P1 { get; set; }
    }

    public class Ret
    {
        public string P1 { get; set; }

        public Stream Stream { get; set; }
    }
}