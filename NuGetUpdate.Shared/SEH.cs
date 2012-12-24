using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NuGetUpdate.Shared
{
    public static class SEH
    {
        [DebuggerStepThrough]
        public static bool SinkExceptions(Action action)
        {
            try
            {
                action();

                return true;
            }
            catch
            {
                return false;
            }
        }

        [DebuggerStepThrough]
        public static bool SinkExceptions<TParam>(Action<TParam> action, TParam param)
        {
            try
            {
                action(param);

                return true;
            }
            catch
            {
                return false;
            }
        }

        [DebuggerStepThrough]
        public static TResult SinkExceptions<TResult>(Func<TResult> action, TResult defaultResult = default(TResult))
        {
            try
            {
                return action();
            }
            catch
            {
                return defaultResult;
            }
        }

        [DebuggerStepThrough]
        public static TResult SinkExceptions<TParam, TResult>(Func<TParam, TResult> action, TParam param, TResult defaultResult = default(TResult))
        {
            try
            {
                return action(param);
            }
            catch
            {
                return defaultResult;
            }
        }
    }
}
