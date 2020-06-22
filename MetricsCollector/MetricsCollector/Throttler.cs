using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetricsCollector
{
    public class Throttler
    {
        private readonly SemaphoreSlim semaphore;

        public Throttler(int maxConcurrency)
        {
            if (maxConcurrency < 1)
            {
                throw new ArgumentException($"'{nameof(maxConcurrency)}' must be greater than 0.");
            }

            this.semaphore = new SemaphoreSlim(maxConcurrency);
        }

        public async Task<Task> Throttle(Func<Task> action)
        {
            await this.semaphore.WaitAsync();
            return Task.Run(async () =>
            {
                try
                {
                    await action();
                }
                finally
                {
                    this.semaphore.Release();
                }
            });
        }
    }
}