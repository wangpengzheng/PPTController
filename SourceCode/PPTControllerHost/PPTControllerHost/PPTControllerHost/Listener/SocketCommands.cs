using System;
using System.Net;
using System.Windows;

namespace PPTController
{
    /// <summary>
    /// This class holds all the commands a that can be passed between clients of the multicast. 
    /// </summary>
    public class SocketCommands
    {
        public const string CommandDelimeter = "|";
        public const string Find = "F";
        public const int MutiCastPort = 13002;
        public const int CommunicatePort = 13001;
        public const string GROUP_ADDRESS = "224.0.0.2";

        public const string JoinFormat = Find + CommandDelimeter + "{0}";
    }
}
