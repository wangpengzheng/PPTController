using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using PPTControllerHost.Listener;
using PPTControllerHost.ViewModel;

namespace PPTControllerHost
{
    public enum ListenerMode
    {
        Stopped,
        Wifi,
        Bluetooth,
        ComputerDetect
    }

    public class AsyncPPTControllerListener : AsyncTaskBase, IAsyncTask, IRevertable
    {
        private PPTControllerListener pptCommandListener;

        public AsyncPPTControllerListener(ListenerMode listenMode, ConnectionViewModel VMb) : base(VMb)          
        {
            pptCommandListener = new PPTControllerListener(listenMode, VMb);

            _curTask = this as IAsyncTask;
        }

        public void StartTask()
        {
            pptCommandListener.StartListening();
        }

        public void FinishTask() 
        {
            // Do nothing here.
        }

        public void RevertTask()
        {
            pptCommandListener.StopListening();
        }
    }
}
