﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NetRpc
{
#if NETSTANDARD2_1 || NETCOREAPP3_1
    public interface IServiceOnceApiConvert : IDisposable, IAsyncDisposable
#else
    public interface IServiceOnceApiConvert : IDisposable
#endif
    {
        Task SendBufferAsync(byte[] buffer);

        Task SendBufferEndAsync();

        Task SendBufferCancelAsync();

        Task SendBufferFaultAsync();

        Task StartAsync(CancellationTokenSource cts);

        Task<ServiceOnceCallParam> GetServiceOnceCallParamAsync();

        /// <returns>True need send stream next, otherwise false.</returns>
        Task<bool> SendResultAsync(CustomResult result, Stream stream, string streamName, ActionExecutingContext context);

        Task SendFaultAsync(Exception body, ActionExecutingContext context);

        Task SendCallbackAsync(object callbackObj);
    }
}