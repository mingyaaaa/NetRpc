﻿using System;
using System.Threading.Tasks;

namespace NetRpc
{
    public delegate Task<ActionExecutedContext> ActionExecutionDelegate();


    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public virtual void OnActionExecuting(ServiceContext context)
        {
        }

        public virtual async Task OnActionExecutionAsync(ServiceContext context, ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            OnActionExecuting(context);
            if (context.Result == null)
            {
                OnActionExecuted(await next());
            }
        }
    }
}