﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NetRpc.Http
{
    public class HttpNetRpcMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;

        public HttpNetRpcMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IOptions<ContractOptions> contractOptions, IHubContext<CallbackHub, ICallback> hub,
            IOptions<HttpServiceOptions> httpOptions, RequestHandler requestHandler, IServiceProvider serviceProvider)
        {
            //if grpc channel message go to next.
            if (httpContext.Request.Path.Value.EndsWith("DuplexStreamingServerMethod"))
            {
                await _next(httpContext);
                return;
            }

            bool notMatched;
#if NETCOREAPP3_1
            await
#endif
            using (var convert = new HttpServiceOnceApiConvert(contractOptions.Value.Contracts, httpContext,
                httpOptions.Value.ApiRootPath, httpOptions.Value.IgnoreWhenNotMatched, hub, serviceProvider))
            {
                await requestHandler.HandleAsync(convert);
                notMatched = convert.NotMatched;
            }

            if (httpOptions.Value.IgnoreWhenNotMatched && notMatched)
                await _next(httpContext);
        }
    }
}