using PPTController.Infrastructure;
using System;

namespace PPTController.ViewModel
{
    public class ViewModelLocator : IDisposable
    {
        public ViewModelLocator()
        {
            containerManager = new ContainerManager();
        }

        #region ViewModels

        public MainPageViewModel MainPageViewModel
        {
            get { return containerManager.Container.Resolve<MainPageViewModel>(); }    
        }

        public BlueToothConnnectionViewModel BlueToothConnnectionViewModel
        {
            get { return containerManager.Container.Resolve<BlueToothConnnectionViewModel>(); }   
        }

        public ExecuteViewModel ExecuteViewModel
        {
            get { return containerManager.Container.Resolve<ExecuteViewModel>(); }
        }

        public WifiIPDirectViewModel WifiIPDirectViewModel
        {
            get { return containerManager.Container.Resolve<WifiIPDirectViewModel>(); }
        }

        public WifiConnectionViewModel WifiConnnectionViewModel
        {
            get { return containerManager.Container.Resolve<WifiConnectionViewModel>(); }
        }

        public SetupViewModel SetupViewModel
        {
            get { return containerManager.Container.Resolve<SetupViewModel>(); }
        }
        #endregion

        #region Infrastructure

        public INavigationService NavigationService
        {
            get { return containerManager.Container.Resolve<INavigationService>(); }
        }

        #endregion

        #region IDisposable

        /*
        ~ViewModelLocator()
        {
            Dispose(false);
        }
         */

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                // dispose managed resources
                if (disposeManagedResources)
                {
                    containerManager.Dispose();
                }

                // dispose unmanaged resources
                {
                    
                }

                _isDisposed = true;
            }
        }

        #endregion

        #region Private Members
        
        private readonly ContainerManager containerManager;
        private bool _isDisposed;

        #endregion
    }
}
