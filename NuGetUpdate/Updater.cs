using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace NuGetUpdate
{
    public class Updater
    {
        public string PackageCode { get; set; }
        public string[] RestartArguments { get; set; }

        public event EventHandler NoUpdateAvailable;
        public event CancelEventHandler UpdateAvailable;
        public event EventHandler UpdateStarted;
        public event ExceptionEventHandler Exception;

        public void Start()
        {
            var context = SynchronizationContext.Current;
            if (context == null)
                Check();
            else
                CheckAsync(context);
        }

        private void CheckAsync(SynchronizationContext context)
        {
            ThreadPool.QueueUserWorkItem(p =>
            {
                bool updateAvailable = false;
                Exception exception = null;

                try
                {
                    updateAvailable = Update.IsUpdateAvailable(PackageCode);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                context.Post(
                    p1 =>
                    {
                        if (exception != null)
                            OnException(new ExceptionEventArgs(exception));
                        else if (updateAvailable)
                            DoUpdate();
                        else
                            OnNoUpdateAvailable();
                    },
                    null
                );
            });
        }

        private void Check()
        {
            try
            {
                if (Update.IsUpdateAvailable(PackageCode))
                    DoUpdate();
                else
                    OnNoUpdateAvailable();
            }
            catch (Exception ex)
            {
                OnException(new ExceptionEventArgs(ex));
            }
        }

        private void DoUpdate()
        {
            var e = new CancelEventArgs();
            OnUpdateAvailable(e);
            if (e.Cancel)
                return;

            try
            {
                Update.StartUpdate(PackageCode, RestartArguments);

                OnUpdateStarted();
            }
            catch (Exception ex)
            {
                OnException(new ExceptionEventArgs(ex));
            }
        }

        protected virtual void OnNoUpdateAvailable()
        {
            var ev = NoUpdateAvailable;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        protected virtual void OnUpdateAvailable(CancelEventArgs e)
        {
            var ev = UpdateAvailable;
            if (ev != null)
                ev(this, e);
        }

        protected virtual void OnUpdateStarted()
        {
            var ev = UpdateStarted;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        protected virtual void OnException(ExceptionEventArgs e)
        {
            var ev = Exception;
            if (ev != null)
                ev(this, e);
        }
    }
}
