using System;

namespace _NueCore.Common.Utility
{
    public class NCounter : IDisposable
    {
        public NCounter(int target)
        {
            Target = target;
        }

        public NCounter(int target, Action onFinished)
        {
            Target = target;
            OnFinished = onFinished;
        }

        public Action OnFinished;
        public Action OnCountDown;
        public int Target;
        private bool _isDisposed;

        public void CountDown()
        {
            if (_isDisposed)
            {
                return;
            }

            Target--;
            OnCountDown?.Invoke();
            if (Target <= 0)
            {
                OnFinished?.Invoke();
                Dispose();
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}