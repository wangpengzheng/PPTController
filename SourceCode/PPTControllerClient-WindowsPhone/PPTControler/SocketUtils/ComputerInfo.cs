using System;
using System.Net;
using System.Windows;

namespace PPTController
{
    /// <summary>
    /// The information about each Computer. 
    /// </summary>
    public class ComputerInfo
    {
        public ComputerInfo(string computerName, IPEndPoint endPoint)
        {
            ComputerEndPoint = endPoint;
            ComputerName = computerName;
        }

        public ComputerInfo(string computerName)
        {
            ComputerEndPoint = null;
            ComputerName = computerName;
        }

        public IPEndPoint ComputerEndPoint { get; set; }
        public string ComputerName { get; set; }
    }
}
