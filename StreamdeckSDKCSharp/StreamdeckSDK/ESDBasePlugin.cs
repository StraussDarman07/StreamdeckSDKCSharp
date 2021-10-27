using System;

namespace Elgato.StreamdeckSDK
{
    public abstract class ESDBasePlugin : IDisposable
    {
        protected ESDConnectionManager ESDConnectionManager { get; }

        protected ESDBasePlugin(ESDConnectionManager connectionManager)
        {
            ESDConnectionManager = connectionManager;
            SubscribeToStreamdeck();
        }

        private void SubscribeToStreamdeck()
        {

        }

        private void UnSubscribeFromStreamdeck()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnSubscribeFromStreamdeck();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
