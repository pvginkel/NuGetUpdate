using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Expressions;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class ScriptRunner : IDisposable
    {
        public ScriptEnvironment Environment { get; private set; }
        private readonly Script _script;
        private readonly ScriptRunnerVisitor _visitor;
        private readonly ScriptContext _context;
        private Thread _thread;
        private Continuation _currentContinuation;
        private bool _aborted;
        private bool _disposed;

        public ScriptRunnerMode Mode { get; private set; }

        public VariableCollection Variables
        {
            get { return _context.Variables; }
        }

        public event EventHandler Completed;

        protected virtual void OnCompleted(EventArgs e)
        {
            var ev = Completed;
            if (ev != null)
                ev(this, e);
        }

        public event ScriptExceptionEventHandler UnhandledException;

        protected virtual void OnUnhandledException(ScriptExceptionEventArgs e)
        {
            var ev = UnhandledException;
            if (ev != null)
                ev(this, e);
        }

        public ScriptRunner(ScriptEnvironment environment, ScriptRunnerVisitor visitor, ScriptRunnerMode mode, string fileName)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            _visitor = visitor;
            Mode = mode;
            _script = ScriptLoader.Load(fileName);
            _context = new ScriptContext(environment);

            Environment = environment;
        }

        public void Execute()
        {
            _thread = new Thread(ThreadProc)
            {
                IsBackground = true
            };

            _thread.Start();
        }

        private void ThreadProc()
        {
            try
            {
                _script.Setup.Visit(_visitor);

                ContainerType container;

                switch (Mode)
                {
                    case ScriptRunnerMode.Install: container = _script.Install; break;
                    case ScriptRunnerMode.Uninstall: container = _script.Uninstall; break;
                    case ScriptRunnerMode.Update: container = _script.Update; break;

                    default:
                        Debug.Fail("Unexpected mode");
                        return;
                }

                container.Visit(_visitor);
            }
            catch (AbortedException)
            {
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ScriptExceptionEventArgs(ex));
            }

            OnCompleted(EventArgs.Empty);
        }

        public string ParseTemplate(string text)
        {
            var template = new TemplateParser(text);

            foreach (var expression in template.Expressions)
            {
                if (!String.IsNullOrEmpty(expression.Expression))
                {
                    var replacement = InvokeExpression(expression.Expression);

                    if (replacement != null)
                        expression.Replacement = replacement.ToString();
                }
            }

            return template.CreateTarget();
        }

        public object InvokeExpression(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new DynamicExpression(
                expression, ExpressionLanguage.Csharp
            ).Invoke(
                _context
            );
        }

        public T InvokeExpression<T>(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new DynamicExpression<T>(
                expression, ExpressionLanguage.Csharp
            ).Invoke(
                _context
            );
        }

        public IScriptContinuation GetContinuation()
        {
            if (_currentContinuation != null)
                throw new ScriptException(UILabels.ContinuationAlreadyRunning);

            _currentContinuation = new Continuation(this);

            return _currentContinuation;
        }

        public void ExecuteChildren(ContainerType action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (action.Items != null)
            {
                foreach (IScriptAction item in action.Items)
                {
                    item.Visit(_visitor);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _aborted = true;

                if (_currentContinuation != null)
                    _currentContinuation.Resume();

                _thread.Join(TimeSpan.FromSeconds(3));

                _disposed = true;
            }
        }

        private class Continuation : IScriptContinuation
        {
            private ManualResetEvent _event = new ManualResetEvent(false);
            private bool _disposed;
            private readonly ScriptRunner _runner;

            public Continuation(ScriptRunner runner)
            {
                if (runner == null)
                    throw new ArgumentNullException("runner");

                _runner = runner;
            }

            public void Resume()
            {
                _event.Set();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    using (_event)
                    {
                        _event.WaitOne();
                    }

                    _event = null;

                    Debug.Assert(_runner._currentContinuation == this);

                    _runner._currentContinuation = null;

                    if (_runner._aborted)
                        throw new AbortedException();

                    _disposed = true;
                }
            }
        }
    }
}
