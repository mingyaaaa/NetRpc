﻿using System.Threading;

namespace NetRpc
{
    public static class GlobalActionExecutingContext
    {
        private static readonly AsyncLocal<ActionExecutingContext> Local = new AsyncLocal<ActionExecutingContext>();

        public static ActionExecutingContext Context
        {
            get => Local.Value;
            set => Local.Value = value;
        }
    }
}