using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wahtherewahhere.GoodLockNet
{
    public partial class GoodSemaphoreSlim : IDisposable
    {
        private readonly SemaphoreSlim _sem;
        private readonly AsyncLocal<Container> _localSemaphoreSlim;

        public GoodSemaphoreSlim()
        {
            _sem = new SemaphoreSlim(1);
            _localSemaphoreSlim = new AsyncLocal<Container>();

        }

        public IDisposable UseWait(CancellationToken cancellationToken = default)
        {
            if (_localSemaphoreSlim.Value == null)
            {
                _localSemaphoreSlim.Value = new Container();
            }
            if (_localSemaphoreSlim.Value.sem == null)
            {
                //Debug.Assert(Interlocked.CompareExchange(ref _localSemaphoreSlim.Value.counter, 1, 0) == 0);
                _localSemaphoreSlim.Value.counter++;
                _sem.Wait(cancellationToken);
                _localSemaphoreSlim.Value.sem = _sem;
                _localSemaphoreSlim.Value.cancellationToken = cancellationToken;
            }
            else
            {
                Interlocked.Increment(ref _localSemaphoreSlim.Value.counter);
            }
            return this;
        }

        public void Dispose()
        {
            if (_localSemaphoreSlim.Value.sem == null)
            {
                return;
            }
            else
            {
                int result = --_localSemaphoreSlim.Value.counter;  //Interlocked.Decrement(ref _localSemaphoreSlim.Value.counter);
                if (result < 0)
                {
                    throw new Exception("GG");
                }
                if (result == 0)
                {
                    _localSemaphoreSlim.Value.sem.Release();
                    _localSemaphoreSlim.Value.sem = null;
                }
            }
        }
        

        private class Container
        {
            public object objlock = new object();
            public SemaphoreSlim sem;
            public /*volatile*/ int counter = 0;
            public CancellationToken cancellationToken;
            public Container()
            {
            }
        }
    }
}
