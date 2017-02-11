using Microsoft.Devices;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using PPTController.Infrastructure;
using PPTController.Sensor;
using PPTController.Tasks;
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using Microsoft.Phone.Tasks;
using PPTController;
using PPTController.Resources;

namespace PPTController.ViewModel
{
    public class WifiIPDirectViewModel : ViewModelBase
    {
        public WifiIPDirectViewModel(INavigationService navigationService, InputValidator inputValidator, IMessageBox messageBox, IEventAggregator eventAggregator)
            : base(navigationService, new Uri("/View/WifiIPDirectView.xaml", UriKind.Relative))
        {
            this.inputValidator = inputValidator;
            this.messageBox = messageBox;
            this.eventAggregator = eventAggregator;
            this.Port = "13001";

            if (QueryString.Keys.Contains("HotSpot"))
            {
                this.IpAddress = App.Settings[App.settingsKey.HotSpotIP.ToString()].ToString();
                CurMode = AppResource.HotSpot;
            }
            else
            {
                this.IpAddress = App.Settings[App.settingsKey.DefaultIP.ToString()].ToString();
                CurMode = AppResource.Wifi;
            }
        }

        public ICommand ConnectCommand
        {
            get { return connectCommand ?? (connectCommand = new DelegateCommand(ConnectCommandExecuted, () => { return !this.IsDownloading; })); }
        }

        public ICommand WifiControl
        {
            get { return wifiControl ?? (wifiControl = new DelegateCommand(showWifiControlPanel)); }
        }

        public string Message
        {
            get { return this.message; }
            set { this.message = value; RaisePropertyChanged(() => Message); }
        }

        public string IpAddress
        {
            get { return this.ipAddress; }
            set
            {
                this.ipAddress = value;
                if (CurMode == "Wifi")
                {
                    App.Settings[App.settingsKey.DefaultIP.ToString()] = value;
                }
                RaisePropertyChanged(() => IpAddress);
            }
        }

        public string CurMode
        {
            get { return this.curMode; }
            set { this.curMode = value; RaisePropertyChanged(() => CurMode); }
        }

        public string Port
        {
            get { return this.port; }
            set { this.port = value; RaisePropertyChanged(() => Port); }
        }

        private bool InputValidate { get; set; }
        private ICommand connectCommand;
        private ICommand wifiControl;
        private InputValidator inputValidator;
        private string message;
        private string ipAddress;
        private string curMode;
        private string port;
        private IEventAggregator eventAggregator;
        private IMessageBox messageBox;
        private SocketSendCommandTask task;

        private void ConnectCommandExecuted()
        {
            if (this.inputValidator.ValidateIpAddress(this.IpAddress) == false)
            {
                this.Message = "Please correct the Ip address to correct format and try again.";
                return;
            }
            
            // Connect to the selected peer.
            this.busyState.StartOperation("Connecting to wifi.");
            this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Subscribe(this.HandleConnectedToWifiEvent);
            ((DelegateCommand)ConnectCommand).RaiseCanExecuteChanged();
            this.task = new SocketSendCommandTask(this.eventAggregator);
            task.Connect(new SocketSendCommandInfo() { IPAddress = this.IpAddress, Port = Port });

            ExecuteViewModel.SendCommandTask = task;
            // We can preserve battery by not advertising our presence.
        }

        private void showWifiControlPanel()
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            connectionSettingsTask.Show();
        }

        public void HandleConnectedToWifiEvent(string result)
        {
            this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Unsubscribe(this.HandleConnectedToWifiEvent);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.busyState.EndOperation("Connecting to wifi.");

                ((DelegateCommand)ConnectCommand).RaiseCanExecuteChanged();
            });

            if (result.Equals("Pass"))
            {
                this.NavigationService.Navigate(new Uri("/View/ExecuteView.xaml", UriKind.Relative));
            }
            else if (result.Equals("Fail"))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.messageBox.Show("There is no wifi connection avaliable, please double check and retry again.");
                });   
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.messageBox.Show(result);
                });
            }  
        }

        public override void OnPageResumeFromTombstoning()
        {

        }
    }
}

