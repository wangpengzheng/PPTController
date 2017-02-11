using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PPTControllerHost.Listener.BlueTooth;
using PPTControllerHost.Listener.Wifi;
using PPTControllerHost;
using System.ComponentModel;
using PPTControllerHost.ViewModel;

namespace PPTControllerHost.Listener
{
    public class PPTControllerListener
    {
        IPPTControllerListener Listener;

        public PPTControllerListener(ListenerMode curMode, ConnectionViewModel VM)
        {
            switch (curMode)
            {
                case ListenerMode.Wifi: Listener = new WifiListener(); break;
                case ListenerMode.Bluetooth: Listener = new BlueToothListener(); break;
                case ListenerMode.ComputerDetect: Listener = new MutiCastListener(); break;
            }

            Listener.ParmConnectionViewModel(VM);
        }

        public void StartListening()
        {
            Listener.StartListening();
        }

        public void StopListening()
        {
            Listener.StopListening();
        }
    }
}
