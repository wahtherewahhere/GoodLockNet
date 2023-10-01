#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wahtherewahhere.GoodLockNet
{
    public partial class GoodSemaphoreSlim : IAsyncDisposable
    {
        public IAsyncDisposable UseWaitAsync(CancellationToken cancellationToken = default)
        {
            if (_localSemaphoreSlim.Value == null)
            {
                _localSemaphoreSlim.Value = new Container();
            }
            if (_localSemaphoreSlim.Value.sem == null)
            {
                //Debug.Assert(Interlocked.CompareExchange(ref _localSemaphoreSlim.Value.counter, 1, 0) == 0);
                _localSemaphoreSlim.Value.counter++;
                _sem.WaitAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                _localSemaphoreSlim.Value.sem = _sem;
                _localSemaphoreSlim.Value.cancellationToken = cancellationToken;
            }
            else
            {
                Interlocked.Increment(ref _localSemaphoreSlim.Value.counter);
            }
            return this;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }
    }
}
#endif