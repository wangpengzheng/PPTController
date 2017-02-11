using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PPTControllerHost.ViewModel
{
    public class PPTInformationViewModel : NotifyPropertyChanged
    {
        #region Properties
        private string _pptLocation;
        public string PPTLocation
        {
            get
            {
                if (String.IsNullOrEmpty(_pptLocation))
                    return "";
                else
                    return _pptLocation;
            }
            set
            {
                if (_pptLocation != value)
                {
                    if ((Path.GetExtension(value) != ".pptx") && (Path.GetExtension(value) != ".ppt"))
                        return;

                    _pptLocation = value;

                    PPTName = Path.GetFileNameWithoutExtension(_pptLocation);

                    Properties.Settings.Default.PPTLocation = value;
                    OnPropertyChanged("PPTLocation");
                }
            }
        }
        
        private string _pptName;
        public string PPTName
        {
            get
            {
                if (String.IsNullOrEmpty(_pptName))
                    return "";
                else
                    return _pptName;
            }
            set
            {
                if (_pptName != value)
                {
                    _pptName = value;
                    OnPropertyChanged("PPTName");
                }
            }
        }


        private string _computerName;
        public string ComputerName
        {
            get
            {
                if (String.IsNullOrEmpty(_computerName))
                    return "";
                else
                    return _computerName;
            }
            set
            {
                if (_computerName != value)
                {
                    _computerName = value;
                    OnPropertyChanged("ComputerName");
                }
            }
        }

        private string _ipAddress;
        public string IPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_ipAddress))
                    return "";

                return _ipAddress;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _ipAddress != value)
                {
                    _ipAddress = value;
                    OnPropertyChanged("IPAddress");
                }
            }
        }

        private bool _pptNameVisible = false;
        public bool PPTNameVisible
        {
            get
            {
                return _pptNameVisible;
            }
            set
            {
                _pptNameVisible = value;
                OnPropertyChanged("PPTNameVisible");
            }
        }
        #endregion

        public PPTInformationViewModel()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            string name = host.HostName;
            StringBuilder sb = new StringBuilder();

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    sb.Append(ip.ToString());
                    sb.Append("  ");
                }
            }

            ComputerName = name;
            IPAddress = sb.ToString();

            if (String.IsNullOrEmpty(Properties.Settings.Default.PPTLocation) ||
                File.Exists(Properties.Settings.Default.PPTLocation))
            {
                PPTLocation = Properties.Settings.Default.PPTLocation;
                PPTNameVisible = true;
            }
            else
                PPTNameVisible = false;
        }
    }
}
