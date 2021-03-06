﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NetRpc;

namespace DataContract
{
    [HttpRoute("Service", true)]
    [FaultExceptionDefine(typeof(CustomException), 400, 1, "errorCode1 error description")]
    [FaultExceptionDefine(typeof(CustomException2), 400, 2, "errorCode2 error description")]
    [HttpHeader("h1", "h1 des.")]
    [SecurityApiKeyDefine("tokenKey", "t1", "t1 des")]
    [SecurityApiKeyDefine("tokenKey2", "t2", "t2 des")]
    public interface IServiceAsync
    {
        [Example("s1", "s1value")]
        [Example("s2", "s2value")]
        Task<CustomObj> Call2Async(CObj obj, string s1, string s2);

        /// <summary>
        /// summary of Call
        /// </summary>
        /// <response code="201">Returns the newly created item</response>
        [HttpRoute("Service1/Call2")]
        [HttpHeader("h2", "h2 des.")]
        [SecurityApiKey("tokenKey")]
        Task<CustomObj> CallAsync(string p1, int p2);


        /// <summary>
        /// summary of Call
        /// </summary>
        [FaultException(typeof(CustomException))]
        [FaultException(typeof(CustomException2))]
        [Tag("A1")]
        [Tag("A2")]
        Task CallByCustomExceptionAsync();

        Task CallByDefaultExceptionAsync();

        [Tag("A1")]
        Task CallByCancelAsync(CancellationToken token);

        /// <response code="701">return the pain text.</response>
        [ResponseText(701)]
        Task CallByResponseTextExceptionAsync();

        Task<ComplexStream> ComplexCallAsync(CustomObj obj, string p1, Stream stream, Func<CustomCallbackObj, Task> cb, CancellationToken token);

        Task<int> UploadAsync(Stream stream, string streamName, string p1, Func<int, Task> cb, CancellationToken token);
    }
}