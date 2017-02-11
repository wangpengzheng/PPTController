using System;
using System.Net;
using System.Windows;
using System.Text;

namespace PPTController
{
    public class UdpPacketReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public IPEndPoint Source { get; set; }

        public UdpPacketReceivedEventArgs(byte[] data, IPEndPoint source)
        {
            this.Message = Encoding.UTF8.GetString(data, 0, data.Length);
            this.Source = source;
        }
    }
}
