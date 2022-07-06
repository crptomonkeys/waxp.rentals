using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using WaxRentalsWeb.Extensions;

namespace WaxRentalsWeb.Files
{
    public abstract class FileMonitor : IDisposable
    {

        public string Contents { get { return SafeContents.Value; } }
        protected LockedString SafeContents { get; } = new();

        public event EventHandler Updated;

        #region " Watcher "

        private readonly string _file;
        public FileMonitor(string file)
        {
            _file = file;
        }

        // FileSystemWatcher doesn't work in containers.
        private PhysicalFileProvider _watcher;
        private readonly object _deadbolt = new();

        public void Initialize()
        {
            if (_watcher == null)
            {
                lock (_deadbolt)
                {
                    if (_watcher == null)
                    {
                        var directory = Path.GetDirectoryName(_file);
                        var filename = Path.GetFileName(_file);
                        _watcher = new PhysicalFileProvider(directory)
                        {
                            UsePollingFileWatcher = true,
                            UseActivePolling = true
                        };
                        Activated(filename); // Activate on startup.
                    }
                }
            }
        }

        private void Activated(object f)
        {
            var filename = (string)f;
            _watcher.Watch(filename)
                    .RegisterChangeCallback(Activated, filename);
            SafeContents.Value =
                File.ReadAllText(
                    Path.Combine(_watcher.Root, filename)
                );
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _watcher.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region " LockedString "

        protected class LockedString
        {

            private string _value = null;
            private readonly ReaderWriterLockSlim _deadbolt = new();

            public string Value
            {
                get
                {
                    var @this = this;
                    return _deadbolt.SafeRead(() => @this._value);
                }
                set
                {
                    var @this = this;
                    _deadbolt.SafeWrite(() => @this._value = value);
                }
            }

        }

        #endregion

    }
}
