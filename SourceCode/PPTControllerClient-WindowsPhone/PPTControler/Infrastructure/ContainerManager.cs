using Microsoft.Practices.Prism.Events;
using PPTController.Sensor;
using PPTController.Tasks;
using PPTController.ViewModel;
using System;
using System.Windows;

namespace PPTController.Infrastructure
{
    public class ContainerManager : IDisposable
    {
        static SimpleContainer configuredContainer = ConfigureContainer();

        public ContainerManager()
        {
            Container = configuredContainer;
        }

        #region Properties

        public SimpleContainer Container { get; private set; }

        #endregion

        #region Private Methods

        private static SimpleContainer ConfigureContainer()
        {
            var Container = new SimpleContainer();
            // Register Services
            EventAggregator eventAggregator = null;
            Container.Register<IEventAggregator>(c => eventAggregator ?? (eventAggregator = new EventAggregator()));
            INavigationService navigationService = null;
            Container.Register<INavigationService>(c => navigationService ?? (navigationService = new ApplicationFrameNavigationService(((App)Application.Current).RootFrame)));
            Container.Register<IHookTask>(c => new CameraButtonHookTask());
            Container.Register(c => new InputValidator());
            Container.Register<IMessageBox>(c => new MessageBox());
            Container.Register<BlueToothSendCommandTask>(c => new BlueToothSendCommandTask(c.Resolve<IEventAggregator>()));
            Container.Register<ISensor>(c => new AccelerometerSensor());
            Container.Register(c => new ExecuteViewModel(c.Resolve<IEventAggregator>(), c.Resolve<INavigationService>()));
            Container.Register(c => new BlueToothConnnectionViewModel(c.Resolve<INavigationService>(), c.Resolve<IMessageBox>(), c.Resolve<IEventAggregator>()));
            Container.Register(c => new WifiIPDirectViewModel(c.Resolve<INavigationService>(), c.Resolve<InputValidator>(), c.Resolve<IMessageBox>(), c.Resolve<IEventAggregator>()));
            Container.Register(c => new WifiConnectionViewModel(c.Resolve<INavigationService>(), c.Resolve<InputValidator>(), c.Resolve<IMessageBox>(), c.Resolve<IEventAggregator>()));
            Container.Register(c => new MainPageViewModel(c.Resolve<INavigationService>()));
            Container.Register(c => new SetupViewModel(c.Resolve<INavigationService>()));
            return Container;
        }

        #endregion

        #region IDisposable

        /*
        ~ContainerManager()
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
                    Container.Dispose();
                }

                // dispose unmanaged resources
                {

                }

                _isDisposed = true;
            }
        }

        #endregion

        #region Private Members

        private bool _isDisposed;

        #endregion
    }
}
