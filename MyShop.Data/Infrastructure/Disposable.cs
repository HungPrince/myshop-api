using System;

namespace MyShop.Data.Infrastructure
{
    public class Disposable : IDisposable
    {
        private bool isDisposable;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposable && disposing)
            {
                DisposeCore();
            }
            isDisposable = true;
        }

        protected virtual void DisposeCore()
        {
        }
    }
}