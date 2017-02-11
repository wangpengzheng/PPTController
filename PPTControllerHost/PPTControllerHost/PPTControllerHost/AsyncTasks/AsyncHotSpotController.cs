using PPTControllerHost.Listener;
using PPTControllerHost.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PPTControllerHost
{
    public class AsyncHotSpotController : AsyncTaskBase, IAsyncTask, IRevertable
    {
        private HotSpotViewModel _curViewModelHS;

        private VirtualRouterHost.VirtualRouterHost virtualRouterHost;
        private ServiceHost serviceHost;

        public AsyncHotSpotController(HotSpotViewModel VMb) : base(VMb)
        {
            _curViewModelHS = VMb;

            _curTask = this as IAsyncTask;
        }

        public void RevertTask()
        {
            virtualRouterHost.Stop();
            serviceHost.Close();

            _curViewModelHS.CurHotSpotStatus = HotspotStatus.Stopped;
            _curViewModel.SendInfoMessage("Hotspot Services Stopped!", InfoType.Warning);
        }

        public void StartTask()
        {
            try
            {
                _curViewModel.SendInfoMessage("Starting Service...", InfoType.Normal);

                virtualRouterHost = new VirtualRouterHost.VirtualRouterHost();
                serviceHost = new ServiceHost(virtualRouterHost);

                virtualRouterHost.SetConnectionSettings(_curViewModelHS.VirtualNetWorkName, 100);
                virtualRouterHost.SetPassword(_curViewModelHS.Password);

                var conns = virtualRouterHost.GetSharableConnections();
                var connToShare = conns.FirstOrDefault();
                if (!virtualRouterHost.Start(connToShare))
                {
                    _curViewModel.SendInfoMessage("Virtual Router could not be started. Supported hardware may not have been found.", InfoType.Error);
                }

                if (serviceHost.State != CommunicationState.Opened)
                {
                    serviceHost.Open();
                    _curViewModelHS.CurHotSpotStatus = HotspotStatus.Running;                    
                }
            }
            catch (AddressAccessDeniedException notAdmin)
            {
                _curViewModel.SendInfoMessage("Errors in detail: \"" + notAdmin.Message + "\"", InfoType.Error);
                _curViewModel.SendInfoMessage("HotSpot Start Failed! Please run this app with Admin then try again.", InfoType.Error);
                _curViewModelHS.CurHotSpotStatus = HotspotStatus.Error;
            }
            catch (Exception ex)
            {
                _curViewModel.SendInfoMessage(ex.Message, InfoType.Error);
                _curViewModelHS.CurHotSpotStatus = HotspotStatus.Error;
                _curViewModel.SendInfoMessage("Your hotspot is failed to start. Please verify if your OS version > Win7 and your computer support Virtual Wireless feature.", InfoType.Error);
            }
        }

        public void FinishTask()
        {
            if (_curViewModelHS.CurHotSpotStatus != HotspotStatus.Error)
            {
                _curViewModel.SendInfoMessage("IP Address: 192.168.173.1", InfoType.Success);
                _curViewModel.SendInfoMessage("Service Started! Please use PPTController to connect the IP address below,", InfoType.Success);
            }
        }
    }
}
