using System;
using System.Diagnostics;

namespace WIO.Core
{
    /// <summary>
    /// Provides a more complete base implementation of the IDisposable pattern
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        // might also consider events for Disposing and Disposed

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableObject()
        {
            //Debug.Assert( Disposed, "WARNING: Object finalized without being disposed!" );

            if (Disposed)
                Debug.WriteLine("WARNING: Object finalized without being disposed!");

            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                DisposeManagedResources();
            }

            DisposeUnmanagedResources();
            Disposed = true;
        }

        protected virtual void DisposeManagedResources() { }
        protected virtual void DisposeUnmanagedResources() { }

        protected virtual void SafeDispose(IDisposable subobject)
        {
            if (null != subobject)
                subobject.Dispose();
        }
    }
}
