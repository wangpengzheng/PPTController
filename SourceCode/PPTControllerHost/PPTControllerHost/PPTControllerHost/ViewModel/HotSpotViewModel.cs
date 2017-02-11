using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Drawing;
using System.ComponentModel;

namespace PPTControllerHost.ViewModel
{
    public class HotSpotViewModel : ConsoleViewModelBase, IDataErrorInfo
    {
        #region Properties
        private string _mainText;
        public string MainText
        {
            get
            {
                if (String.IsNullOrEmpty(_mainText))
                    return "";
                else
                    return _mainText;
            }
            set
            {
                if (_mainText != value)
                {
                    _mainText = value;
                    OnPropertyChanged("MainText");
                }
            }
        }

        private string _virtualNetWorkName;
        public string VirtualNetWorkName
        {
            get
            {
                if (string.IsNullOrEmpty(_virtualNetWorkName))
                    _virtualNetWorkName = Properties.Settings.Default.SSID;

                return _virtualNetWorkName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _virtualNetWorkName != value)
                {
                    _virtualNetWorkName = value;
                    Properties.Settings.Default.SSID = value;
                    OnPropertyChanged("VirtualNetWorkName");
                }
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_password))
                    _password = Properties.Settings.Default.SSIDPassword;

                return _password;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _password != value)
                {
                    _password = value;
                    Properties.Settings.Default.SSIDPassword = value;
                    OnPropertyChanged("Password");
                }
            }
        }
        #endregion

        public string Error
        {
            get { return ""; }
        }

        public string this[string columnName]
        {
            get
            {
                string ret = "";
                switch (columnName)
                {
                    case "VirtualNetWorkName": ret = String.IsNullOrEmpty(this._virtualNetWorkName) ? "Virtual Net work name is madantory!" : ""; break;
                    case "Password": ret = String.IsNullOrEmpty(this._password) ? "A password is required" : ""; break;
                }

                return ret;
            }
        }

        public HotSpotViewModel()
        {
            CurHotSpotStatus = HotspotStatus.Stopped;
            MainText = "Start your Hotspot";

            StartHotSpotCommand = new RelayCommand(
                    o => StartHotSpotAsync(), 
                    o => _curHotSpotStatus == HotspotStatus.Stopped 
                        || _curHotSpotStatus == HotspotStatus.Running
                        || _curHotSpotStatus != HotspotStatus.Pending);

            _curAsyncHotSpotController = new AsyncHotSpotController(this);
        }

        public override void SendInfoMessage(string info, InfoType type = InfoType.Normal)
        {
            base.SendInfoMessage(info, type);

            // Disable hotspot if found errors
            if (type == InfoType.Error)
                CurHotSpotStatus = HotspotStatus.Error;
        }

        private AsyncHotSpotController _curAsyncHotSpotController;

        public void StartHotSpotAsync()
        {           
            if (_curHotSpotStatus == HotspotStatus.Running)
            {
                _curHotSpotStatus = HotspotStatus.Pending;
                _curAsyncHotSpotController.RevertTask();
            }
            else
            {
                _curHotSpotStatus = HotspotStatus.Pending;
                _curAsyncHotSpotController.StartTaskAsync();
            }
        }

        private HotspotStatus _curHotSpotStatus;

        public HotspotStatus CurHotSpotStatus
        {
            set
            {
                if (value != _curHotSpotStatus)
                {
                    switch (value)
                    {
                        case HotspotStatus.Stopped: MainText = "Start your Hotspot"; break;
                        case HotspotStatus.Running: MainText = "Stop your Hotspot"; break;
                        case HotspotStatus.Pending: MainText = "Starting your Hotspot"; break;
                        case HotspotStatus.Error: MainText = "Hotspot Cannnot Start!"; break;
                    }
                    _curHotSpotStatus = value;
                }
            }
            get
            {
                return _curHotSpotStatus;
            }
        }
        
        public ICommand StartHotSpotCommand
        {
            get;
            private set;
        }
    }
}
