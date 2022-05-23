using System;
using System.Net;
using System.Threading.Tasks;

// https://www.ryadel.com/en/webclient-request-timeout-class-c-sharp-dot-net/

namespace WaxRentals.Waxp
{
    internal class QuickTimeoutWebClient : WebClient
    {

        public string DownloadString(string address, TimeSpan timeout)
        {
            return timeout.TotalSeconds >= 100 // Default timeout is 100 seconds.
                ? DownloadString(address)
                : DownloadStringTaskAsync(address, timeout).GetAwaiter().GetResult();
        }

        public async Task<string> DownloadStringTaskAsync(string address, TimeSpan timeout)
        {
            var task = DownloadStringTaskAsync(address);
            if (await Task.WhenAny(task, Task.Delay((int)timeout.TotalMilliseconds)) != task)
                CancelAsync();
            return await task;
        }

    }
}
