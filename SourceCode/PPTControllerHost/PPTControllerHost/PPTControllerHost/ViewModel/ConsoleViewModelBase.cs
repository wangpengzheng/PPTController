using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstFloor.ModernUI.Presentation;

namespace PPTControllerHost.ViewModel
{
    public enum InfoType
    {
        Normal,
        Success,
        Warning,
        Error
    }

    public enum HotspotStatus
    {
        Stopped,
        Running,
        Pending,
        Error
    }

    public abstract class ConsoleViewModelBase : NotifyPropertyChanged
    {
        protected string _infoText;
        public string InfoText
        {
            get
            {
                if (String.IsNullOrEmpty(_infoText))
                    return "";
                else
                    return _infoText;
            }
            set
            {
                if (_infoText != value)
                {
                    _infoText = value;
                    OnPropertyChanged("InfoText");
                }
            }
        }

        /// <summary>
        /// Construct BBCode for InfoLog Text. Add new message to the header of line.
        /// For BBCode tag Reference please visit http://mui.codeplex.com/wikipage?title=BBCode%20tag%20reference&referringTitle=Documentation
        /// </summary>
        /// <param name="info">Output text</param>
        /// <param name="type">Output type</param>
        public virtual void SendInfoMessage(string info, InfoType type = InfoType.Normal)
        {
            StringBuilder sbForInfo = new StringBuilder(_infoText);
            StringBuilder sbCurMsg = new StringBuilder();

            switch (type)
            {
                case InfoType.Warning: sbCurMsg.Append("[color=#A020F0]"); break;
                case InfoType.Error: sbCurMsg.Append("[color=#ff4500]"); break;
                case InfoType.Success: sbCurMsg.Append("[color=#0000FF]"); break;
            }

            sbCurMsg.Append(info);

            if (type != InfoType.Normal)
                sbCurMsg.Append("[/color]");

            if (!string.IsNullOrEmpty(_infoText))
                sbCurMsg.Append("\r\n\r\n");

            sbForInfo.Insert(0, sbCurMsg.ToString());

            InfoText = sbForInfo.ToString();
        }
    }
}
