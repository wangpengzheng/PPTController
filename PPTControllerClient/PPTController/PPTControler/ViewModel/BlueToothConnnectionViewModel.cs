using Microsoft.Practices.Prism.Commands;
using PPTController.Infrastructure;
using PPTController.Tasks;
using System;
using System.Windows.Input;
using Windows.Networking.Proximity;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using Windows.Networking.Sockets;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using ShakeGestures;

namespace PPTController.ViewModel
{
    public class BlueToothConnnectionViewModel : ViewModelBase
    {
        public BlueToothConnnectionViewModel(INavigationService navigationService, IMessageBox messageBox, IEventAggregator eventAggregator)
            : base(navigationService, new Uri("/View/BlueToothConnnectionView.xaml", UriKind.Relative))
        {
            this.messageBox = messageBox;
            this.PeerApps = new ObservableCollection<PairedDeviceInfo>();
            this.eventAggregator = eventAggregator;
            
            // register shake event
            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);

            // optional, set parameters
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 5;

            // start shake detection
            ShakeGesturesHelper.Instance.Active = true;
        }

        private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => this.RefreshPairedDevicesList());

            System.Diagnostics.Debug.WriteLine(DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond + "  " + e.ShakeType);
        }

        public ICommand RefreshPeerCommand
        {
            get { return refreshPeerCommand ?? (refreshPeerCommand = new DelegateCommand(RefreshPeerCommandExecuted)); }
        }

        public ICommand ConnectCommand
        {
            get { return connectCommand ?? (connectCommand = new DelegateCommand(ConnectCommandExecuted, () => { return !this.IsDownloading; })); }
        }

        public ICommand BluetoothMgeCommand
        {
            get { return bluetoothMgeCommand ?? (bluetoothMgeCommand = new DelegateCommand(showBluetoothControlPanel)); }
        }

        public string Message
        {
            get { return this.message; }
            set { this.message = value; RaisePropertyChanged(() => Message); }
        }

        public PairedDeviceInfo DeviceInfo
        {
            get { return this.deviceInfo; }
            set { this.deviceInfo = value; RaisePropertyChanged(() => DeviceInfo); }
        }

        public void HandleConnectedToBlueToothEvent(string result)
        {
            this.eventAggregator.GetEvent<ConnectedToBlueToothEvent>().Unsubscribe(this.HandleConnectedToBlueToothEvent);  
            this.busyState.EndOperation("Connecting to bluetooth.");
            ((DelegateCommand)ConnectCommand).RaiseCanExecuteChanged();
            if (result.Equals("Pass"))
            {
                this.NavigationService.Navigate(new Uri("/View/ExecuteView.xaml", UriKind.Relative));
            }
            else
            {
                this.messageBox.Show("There is no bluetooth connection avaliable, please double check and retry again.");
            }
        }

        private ICommand refreshPeerCommand;
        private ICommand connectCommand;
        private ICommand bluetoothMgeCommand;

        private void RefreshPeerCommandExecuted()
        {
            this.RefreshPairedDevicesList();
        }

        private async void RefreshPairedDevicesList()
        {
            try
            {
                this.busyState.StartOperation("finding peers");
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();

                // By clearing the backing data, we are effectively clearing the ListBox
                PeerApps.Clear();

                if (peers.Count == 0)
                {
                    this.Message = "No peers found. Tap 'refresh peers' or simple Shake your phone to update.";
                }
                else
                {
                    this.Message = String.Format("Peers found: {0}. Tap 'refresh peers' to update.", peers.Count);
                    // Add peers to list
                    foreach (var peer in peers)
                    {
                        PeerApps.Add(new PairedDeviceInfo(peer));
                    }

                    // If there is only one peer, go ahead and select it
                    if (PeerApps.Count == 1)
                    {
                        this.DeviceInfo = PeerApps[0];
                    }
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    var result = this.messageBox.Show("Bluetooth is turned off. To see the current Bluetooth settings tap 'ok'.",
                        "Bluetooth Off",
                        MessageBoxButton.OKCancel);
                    if (result == true)
                    {
                        showBluetoothControlPanel();
                    }
                }
                else if ((uint)ex.HResult == ERR_MISSING_CAPS)
                {
                    this.messageBox.Show("To run this app, you must have ID_CAP_PROXIMITY enabled in WMAppManifest.xaml");
                }
                else if ((uint)ex.HResult == ERR_NOT_ADVERTISING)
                {
                    this.messageBox.Show("You are currently not advertising yourself, i.e., a call to PeerFinder.Start() must proceed FindAllPeersAsync()");
                }
                else
                {
                    this.messageBox.Show(ex.Message);
                }
            }
            finally
            {
                this.busyState.EndOperation("finding peers");
            }
        }

        private void showBluetoothControlPanel()
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
            connectionSettingsTask.Show();
        }

        private void ConnectCommandExecuted()
        {
            if (this.DeviceInfo == null)
            {
                this.messageBox.Show("You must select a peer from the list.", "Can't connect", MessageBoxButton.OK);
                return;
            }

            // Connect to the selected peer.
            PeerInformation peer = this.DeviceInfo.PeerInfo;
            this.busyState.StartOperation("Connecting to bluetooth.");
            this.eventAggregator.GetEvent<ConnectedToBlueToothEvent>().Subscribe(this.HandleConnectedToBlueToothEvent); 
            ((DelegateCommand)ConnectCommand).RaiseCanExecuteChanged();
            this.task = new BlueToothSendCommandTask(this.eventAggregator);
            task.Connect(peer);

            ExecuteViewModel.SendCommandTask = task;
            // We can preserve battery by not advertising our presence.
            PeerFinder.Stop();            
        }       

        public override void OnPageResumeFromTombstoning()
        {

        }

        public ObservableCollection<PairedDeviceInfo> PeerApps { get; private set; }
        private string message;
        private IMessageBox messageBox;
        const uint ERR_BLUETOOTH_OFF = 0x8007048F;      // The Bluetooth radio is off
        const uint ERR_MISSING_CAPS = 0x80070005;       // A capability is missing from your WMAppManifest.xml
        const uint ERR_NOT_ADVERTISING = 0x8000000E;    // You are currently not advertising your presence using PeerFinder.Start()
        private PairedDeviceInfo deviceInfo;
        public StreamSocket Socket = new StreamSocket();
        private IEventAggregator eventAggregator;
        private BlueToothSendCommandTask task;
    }

    /// <summary>
    ///  Class to hold all paired device information
    /// </summary>
    public class PairedDeviceInfo
    {
        internal PairedDeviceInfo(PeerInformation peerInformation)
        {
            this.PeerInfo = peerInformation;
            this.DisplayName = this.PeerInfo.DisplayName;
            this.HostName = this.PeerInfo.HostName.DisplayName;
            this.ServiceName = this.PeerInfo.ServiceName;
        }

        public string DisplayName { get; private set; }
        public string HostName { get; private set; }
        public string ServiceName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }
}
