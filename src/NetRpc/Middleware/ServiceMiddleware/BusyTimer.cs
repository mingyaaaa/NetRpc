﻿using System;
using System.Threading.Tasks;
using System.Timers;

namespace NetRpc
{
    public sealed class BusyTimer : IDisposable
    {
        private readonly Timer T = new Timer();
        private DateTime _lastTime;
        public double Interval => T.Interval;

        public BusyTimer(double interval)
        {
            T.Elapsed += T_Elapsed;
            T.Interval = interval;
        }

        public void Dispose()
        {
            T.Dispose();
        }

        public void Start()
        {
            T.Start();
        }

        public void Stop()
        {
            T.Stop();
        }

        public Func<ElapsedEventArgs, Task> ElapsedAsync;

        private async Task OnElapsedAsync(ElapsedEventArgs e)
        {
            if (ElapsedAsync == null)
                return;
            await ElapsedAsync(e);
        }

        private async void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _lastTime).TotalMilliseconds < Interval)
                return;

            await OnElapsedAsync(e);
            _lastTime = DateTime.Now;
        }
    }
}