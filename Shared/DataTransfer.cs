using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public class DataTransfer<T> : IDisposable
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private DataOwner? _currentValue;

        public class DataOwner : IDisposable
        {
            public T Value { get; }
            private readonly DataTransfer<T> _parent;

            internal DataOwner(T value, DataTransfer<T> parent)
            {
                Value = value;
                _parent = parent;
            }

            public void Dispose()
            {
                _parent._currentValue = null;
            }
        }

        public void Dispose()
        {
            _currentValue?.Dispose();
            _lock.Dispose();
        }

        public bool CanWrite => _currentValue == null;

        public bool TryWrite(T value)
        {
            if (_currentValue == null)
            {
                _currentValue = new DataOwner(value, this);
                _lock.Release();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<DataOwner> GetNext(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _lock.WaitAsync(stoppingToken);
                var value = _currentValue;
                if (value != null)
                {
                    return value;
                }
            }
            throw new OperationCanceledException(stoppingToken);
        }
    }
}
