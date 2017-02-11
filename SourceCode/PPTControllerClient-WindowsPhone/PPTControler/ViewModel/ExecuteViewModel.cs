using Microsoft.Devices;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using PPTController.Infrastructure;
using PPTController.Sensor;
using PPTController.Tasks;
using System;
using System.Windows.Input;

namespace PPTController.ViewModel
{
    public class ExecuteViewModel : ViewModelBase
    {
        public ExecuteViewModel(IEventAggregator eventAggregator,INavigationService navigationService)
            : base(navigationService, new Uri("/View/ExecuteView.xaml", UriKind.Relative))
        {
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<ExecutionEvent>().Subscribe(ImplementPPTCommandExecuted);

            needVibrate = Convert.ToBoolean(App.Settings[App.settingsKey.ShakeByAction.ToString()]);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.eventAggregator.GetEvent<ExecutionEvent>().Unsubscribe(ImplementPPTCommandExecuted);
        }

        public ICommand ImplementPPTCommand
        {
            get { return implementPPTCommand ?? (implementPPTCommand = new DelegateCommand<string>(ImplementPPTCommandExecuted)); }
        }

        private bool needVibrate;
        private bool PptInited { get; set; }
        private bool InputValidate { get; set; }
        private ICommand implementPPTCommand;
        public static ISendCommandTask SendCommandTask { get; set; }
        private IEventAggregator eventAggregator;

        private void ImplementPPTCommandExecuted(string command)
        {
            SendCommandTask.Send(command); // Test the ability of connection.

            if (needVibrate)
            {
                VibrateController.Default.Start(TimeSpan.FromMilliseconds(200));
            }
        }

        public override void OnPageResumeFromTombstoning()
        {

        }

        public override void OnPageDeactivation(bool isIntentionalNavigation)
        {
            base.OnPageDeactivation(isIntentionalNavigation);

            if (isIntentionalNavigation)
            {
                Dispose();
                return;
            }
        }
    }
}
