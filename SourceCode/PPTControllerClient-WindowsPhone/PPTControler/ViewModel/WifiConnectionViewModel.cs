using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using PPTController.Infrastructure;
using PPTController.Tasks;
using System;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using ShakeGestures;
using System.Windows;
using PPTController.Events;

namespace PPTController.ViewModel
{
    public class WifiConnectionViewModel : ViewModelBase
    {
        private ComputerDetection computerDetection;
        public WifiConnectionViewModel(INavigationService navigationService, InputValidator inputValidator, IMessageBox messageBox, IEventAggregator eventAggregator)
            : base(navigationService, new Uri("/View/WifiConnnectionView.xaml", UriKind.Relative))
        {
            this.messageBox = messageBox;
            this.eventAggregator = eventAggregator;

            // Init channel and regist events. Transfer the Computers instance need to be found.
            computerDetection = new ComputerDetection(this);
            this.startWifiDetection();

            // register shake event
            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);

            // optional, set parameters
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 5;

            // start shake detection
            ShakeGesturesHelper.Instance.Active = true;
        }

        public ICommand WifiMgeCommand
        {
            get { return wifiMgeCommand ?? (wifiMgeCommand = new DelegateCommand(showWifiControlPanel)); }
        }

        public ICommand WifiAddIPControl
        {
            get { return wifiAddIPControl ?? (wifiAddIPControl = new DelegateCommand(navigateToAddIPPage)); }
        }

        public ICommand RefreshPeerCommand
        {
            get { return refreshPeerCommand ?? (refreshPeerCommand = new DelegateCommand(refreshPeers)); }
        }

        public ICommand ConnectCommand
        {
            get { return connectCommand ?? (connectCommand = new DelegateCommand(ConnectCommandExecuted)); }
        }

        public string Message
        {
            get { return this.message; }
            set { this.message = value; RaisePropertyChanged(() => Message); }
        }

        public ObservableCollection<ComputerInfo> ComputersDetected
        {
            get { return this.computersDetected; }
            set { this.computersDetected = value; RaisePropertyChanged(() => computersDetected); }
        }

        public ComputerInfo CurComputer
        {
            get {return this.curComputer;}
            set { this.curComputer = value; RaisePropertyChanged(() => curComputer); }
        }

        private ObservableCollection<ComputerInfo> computersDetected = new ObservableCollection<ComputerInfo>();
        private ComputerInfo curComputer;
        private ICommand connectCommand;
        private ICommand wifiMgeCommand;
        private ICommand wifiAddIPControl;
        private ICommand refreshPeerCommand;
        private string message;
        private IEventAggregator eventAggregator;
        private IMessageBox messageBox;
        private SocketSendCommandTask task;

        /// <summary>
        /// This method will call ComputerDetection to start recevice the data from muticast group which send by host.
        /// </summary>
        private void startWifiDetection()
        {
            computerDetection.Join();
        }

        private void showWifiControlPanel()
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            connectionSettingsTask.Show();
        }

        private void navigateToAddIPPage()
        {
            this.NavigationService.Navigate(new Uri("/View/WifiIPDirectView.xaml", UriKind.Relative));
        }

        private void refreshPeers()
        {
            this.startWifiDetection();
        }

        private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => this.startWifiDetection());
        }

        private void ConnectCommandExecuted()
        {
            if (this.CurComputer == null)
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.messageBox.Show("Please choose a found device and then click connect. If no computers was found, try to connect your PC through IP directly.");
                });


            // Connect to the selected peer.
            this.busyState.StartOperation("Connecting to wifi.");
            this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Subscribe(this.HandleConnectedToWifiEvent);
            ((DelegateCommand)ConnectCommand).RaiseCanExecuteChanged();
            this.task = new SocketSendCommandTask(this.eventAggregator);
            task.Connect(new SocketSendCommandInfo() { IPAddress = curComputer.ComputerEndPoint.Address.ToString(), Port = SocketCommands.CommunicatePort });

            ExecuteViewModel.SendCommandTask = task;
            // We can preserve battery by not advertising our presence.
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
