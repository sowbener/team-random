using System;
using System.Diagnostics;
using Waldo.Interfaces.Settings;
using Waldo.Helpers;

namespace Waldo.Helpers
{
    // CREDIT TO unifiedtrinity
    [Flags]
    internal enum LogCategory
    {
        None = 0,
        Performance
    }

    [DebuggerStepThrough]
    public class PerformanceLogger : IDisposable
    {
        private readonly string _BlockName;
        private readonly Stopwatch _Stopwatch;
        private bool _IsDisposed;

        public PerformanceLogger(string blockName)
        {
            _BlockName = blockName;
            if (WaSettings.Instance.General.PerformanceLogging.HasFlag(LogCategory.Performance))
            {
                _Stopwatch = new Stopwatch();
                _Stopwatch.Start();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!_IsDisposed)
            {
                _IsDisposed = true;
                if (WaSettings.Instance.General.PerformanceLogging.HasFlag(LogCategory.Performance))
                {
                    _Stopwatch.Stop();
                    if (_Stopwatch.Elapsed.TotalMilliseconds > 1)
                    {
                        WaLogger.Performance("[Performance] Execution of the block {0} took {1:00.00}ms.", _BlockName,
                                      _Stopwatch.Elapsed.TotalMilliseconds);
                    }
                }

                GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable Members

        ~PerformanceLogger()
        {
            Dispose();
        }
    }
}