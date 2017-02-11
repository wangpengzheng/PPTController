using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstFloor.ModernUI.Presentation;
using PPTControllerHost;

namespace PPTControllerHost.ViewModel
{
    public class ConnectionViewModel : ConsoleViewModelBase
    {
        #region Properties
        private Boolean _checkForWifi;
        public Boolean CheckForWifi
        {
            get
            {
                return _checkForWifi;
            }
            set
            {
                if (_checkForWifi != value)
                {
                    _checkForWifi = value;
                    _checkForBluetooth = !_checkForWifi;

                    OnPropertyChanged("CheckForWifi");

                }
            }
        }

        private Boolean _checkForBluetooth;
        public Boolean CheckForBluetooth
        {
            get
            {
                return _checkForBluetooth;
            }
            set
            {
                if (_checkForBluetooth != value)
                {
                    _checkForBluetooth = value;
                    _checkForWifi = !_checkForBluetooth;

                    OnPropertyChanged("CheckForBluetooth");
                }
            }
        }

        private ListenerMode _curListenerMode;
        public ListenerMode CurListenerMode
        {
            get
            {
                return _curListenerMode;
            }
            set
            {
                if (_curListenerMode != value)
                {
                    _curListenerMode = value;
                    OnPropertyChanged("CurListenerMode");

                    this.StartListenerAsync();
                }
            }
        }
        #endregion

        private AsyncPPTControllerListener _curListener;
        private AsyncPPTControllerListener _computerDetectListener;
        public ConnectionViewModel()
        {
            CheckForWifi = true;
            CheckForBluetooth = false;
        }
        
        public void StartListenerAsync()
        {
            if (_checkForWifi == _checkForBluetooth)
                return;

            // Use the port 13002 to send current IP to muticast group for auto detection of wifi mode.
            if (_computerDetectListener == null)
            {
                _computerDetectListener = new AsyncPPTControllerListener(ListenerMode.ComputerDetect, this);
                _computerDetectListener.StartTaskAsync();
            }

            // Push cancel action first.
            if (_curListener != null)
            {
                this.SendInfoMessage("Stopping the listener of mode : [b]" + (_checkForBluetooth ? "Wifi[/b]" : "Bluetooth[/b]"), InfoType.Warning);
                _curListener.RevertTask();
            }

            this.SendInfoMessage("Starting the listener of mode : [b]" + _curListenerMode.ToString() + "[/b]", InfoType.Normal);

            // Start Listener
            try
            {   
                // Start to user Wifi to listen the connection from port 13001 or Bluetooth mode.
                _curListener = new AsyncPPTControllerListener(_curListenerMode, this);
                _curListener.StartTaskAsync();
            }
            catch (AppException ex)
            {
                this.SendInfoMessage("Error Found : " + ex.Message, ex.CurAppExceptionType == AppExceptionType.Warning ? InfoType.Warning : InfoType.Error);
            }            
        }
    }
}
