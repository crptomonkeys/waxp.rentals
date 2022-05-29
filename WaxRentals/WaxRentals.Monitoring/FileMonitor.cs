using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace WaxRentals.Monitoring
{
    public abstract class FileMonitor : IDisposable
    {

        #region " Event "

        public event EventHandler<string> Updated;

        protected void RaiseEvent(string contents)
        {
            Updated?.Invoke(this, contents);
        }

        #endregion

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
            RaiseEvent(
                File.ReadAllText(
                    Path.Combine(_watcher.Root, filename)
                )
            );
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        #endregion

    }
}
