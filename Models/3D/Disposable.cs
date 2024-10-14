using System;

namespace Avalonia_3D_STL.Models._3D
{
    public abstract class Disposable : IDisposable
    {
        private bool disposedValue;

        ~Disposable()
        {
            Dispose(disposing: false);
        }

        protected abstract void Destroy(bool disposing = false);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Destroy(disposing);

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
