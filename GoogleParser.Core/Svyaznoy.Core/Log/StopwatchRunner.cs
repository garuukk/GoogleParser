using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;

namespace Svyaznoy.Core.Log
{
    public class StopwatchRunner
    {
        private readonly Stopwatch _stopwatch;

        public DateTime StarTime { get; private set; }

        public StopwatchRunner(bool autoStart = true)
        {
            _stopwatch = new Stopwatch();
            if (autoStart)
            {
                Start();
            }
        }

        public void Start()
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
                StarTime = DateTime.Now;
            }
        }

        public void Stop()
        {
            if(_stopwatch.IsRunning)
                _stopwatch.Stop();
        }

        public long ElapsedMilliseconds
        {
            get { return _stopwatch.ElapsedMilliseconds; }
        }
    }
}
