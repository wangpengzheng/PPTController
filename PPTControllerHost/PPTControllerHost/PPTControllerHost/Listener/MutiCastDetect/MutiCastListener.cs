using PPTController;
using PPTControllerHost.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PPTController;
using System.Timers;

namespace PPTControllerHost.Listener
{
    public class MutiCastListener : AysncReporter, IPPTControllerListener
    {
        private UdpClient Client { get; set; }
        private string computerName;
        private Timer _timer;

        public MutiCastListener()
        {
            computerName = Utils.getCurComputerName();
        }

        public void StartListening()
        {
            this.Client = new UdpClient(SocketCommands.MutiCastPort, AddressFamily.InterNetwork);
            this.Client.JoinMulticastGroup(IPAddress.Parse(SocketCommands.GROUP_ADDRESS));
            this.Send(SocketCommands.JoinFormat, computerName);
        }

        public void StopListening()
        {
            if (_timer != null)
                _timer.Stop();

            this.Client.DropMulticastGroup(IPAddress.Parse(SocketCommands.GROUP_ADDRESS));
        }

        public void Send(string format, params object[] args)
        {
            try
            {
                if (_timer == null)
                {
                    _timer = new Timer(1000);
                    _timer.Elapsed += delegate(object o, ElapsedEventArgs arg)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(string.Format(format, args));
                        IPEndPoint ClientOriginatordest = new IPEndPoint(IPAddress.Parse(SocketCommands.GROUP_ADDRESS), SocketCommands.MutiCastPort);
                        this.Client.Send(data, data.Length, ClientOriginatordest);
                    };
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                this.ReportMessageAsync(ex.Message);
                this.ReportMessageAsync("Computer Auto detection was disabled.", InfoType.Error);
            }
        }
    }
}
