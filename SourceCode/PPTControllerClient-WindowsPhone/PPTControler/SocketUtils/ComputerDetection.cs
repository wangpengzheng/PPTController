using System;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using PPTController.ViewModel;
using System.Collections.ObjectModel;

namespace PPTController
{
    /// <summary>
    /// This class handles all game communication. Communication for the game is made up of
    /// a number of commands that we have defined in the GameCommand.cs class. These commands 
    /// are the grammar, or set of actions, that we can transmist and receive and interpret.
    /// </summary>
    public class ComputerDetection : IDisposable
    {
        /// <summary>
        /// All communication takes place using a UdpAnySourceMulticastChannel. 
        /// A UdpAnySourceMulticastChannel is a wrapper we create around the UdpAnySourceMulticastClient.
        /// </summary>
        /// <value>The channel.</value>
        private UdpAnySourceMulticastChannel Channel { get; set; }
        
        /// <summary>
        /// Collect all the computers detected by auto detection, collect them to ViewModel.
        /// </summary>
        private ObservableCollection<ComputerInfo> computersDetected = new ObservableCollection<ComputerInfo>();

        /// <summary>
        /// The IP address of the multicast group. 
        /// </summary>
        /// <remarks>
        /// A multicast group is defined by a multicast group address, which is an IP address 
        /// that must be in the range from 224.0.0.0 to 239.255.255.255. Multicast addresses in 
        /// the range from 224.0.0.0 to 224.0.0.255 inclusive are “well-known” reserved multicast 
        /// addresses. For example, 224.0.0.0 is the Base address, 224.0.0.1 is the multicast group 
        /// address that represents all systems on the same physical network, and 224.0.0.2 represents 
        /// all routers on the same physical network.The Internet Assigned Numbers Authority (IANA) is 
        /// responsible for this list of reserved addresses. For more information on the reserved 
        /// address assignments, please see the IANA website.
        /// http://go.microsoft.com/fwlink/?LinkId=221630
        /// </remarks>
        private const string GROUP_ADDRESS = "224.0.0.2";

        /// <summary>
        /// This defines the port number through which all communication with the multicast group will take place. 
        /// </summary>
        /// <remarks>
        /// The value in this example is arbitrary and you are free to choose your own.
        /// </remarks>
        private const int GROUP_PORT = 13002;

        public ComputerDetection(WifiConnectionViewModel vm)
        {
            this.Channel = new UdpAnySourceMulticastChannel(GROUP_ADDRESS, GROUP_PORT);

            this.computersDetected = vm.ComputersDetected;

            // Register for events on the multicast channel.
            RegisterEvents();
        }

        #region Properties
        /// <summary>
        /// The Computer, against whom, I am currently playing.
        /// </summary>
        private ComputerInfo _currentOpponent;
        public ComputerInfo CurrentOpponent
        {
            get
            {
                return _currentOpponent;
            }
        }
        #endregion

        #region Game Actions
        public void Join()
        {
            //Open the connection
            this.Channel.Open();
        }

        #endregion

        #region Multicast Communication
        /// <summary>
        /// Register for events on the multicast channel.
        /// </summary>
        private void RegisterEvents()
        {
            // Register for events from the multicast channel
            //this.Channel.Joined += new EventHandler(Channel_Joined);
            this.Channel.PacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
        }

        /// <summary>
        /// Unregister for events on the multicast channel
        /// </summary>
        private void UnregisterEvents()
        {
            if (this.Channel != null)
            {
                // Register for events from the multicast channel
                //this.Channel.Joined -= new EventHandler(Channel_Joined);
                this.Channel.PacketReceived -= new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
            }
        }

        /// <summary>
        /// Handles the Joined event of the Channel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        //void Channel_Joined(object sender, EventArgs e)
        //{
        //    this.IsJoined = true;
        //    this.Channel.Send(SocketCommands.JoinFormat, _computerName);
        //}

        /// <summary>
        /// Handles the PacketReceived event of the Channel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SilverlightPlayground.UDPMulticast.UdpPacketReceivedEventArgs"/> instance containing the event data.</param>
        void Channel_PacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            string message = e.Message.Trim('\0');
            string[] messageParts = message.Split(SocketCommands.CommandDelimeter.ToCharArray());

            if (messageParts.Length == 2)
            {
                switch (messageParts[0])
                {
                    case SocketCommands.Find:
                        OnComputerFind(new ComputerInfo(messageParts[1], e.Source));
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Command Handlers - methods to handle each command that we receive
        /// <summary>
        /// Handle a Computer joining the multicast group.
        /// </summary>
        /// <param name="computerInfo">The Computer.</param>
        private void OnComputerFind(ComputerInfo computerInfo)
        {
            bool add = true;
            int numberAdded = 0;

            foreach (ComputerInfo pi in computersDetected)
            {
                if (pi.ComputerName == computerInfo.ComputerName)
                {
                    pi.ComputerEndPoint = computerInfo.ComputerEndPoint;

                    add = false;
                    break;
                }
            }

            if (add)
            {
                numberAdded++;
                computersDetected.Add(computerInfo);
            }

            // If any new Computers have been added, send out our join message again
            // to make sure we are added to their Computer list.
            //if (numberAdded > 0)
            //{
            //    this.Channel.Send(SocketCommands.JoinFormat, _computerName);
            //}

#if DEBUG
            Debug.WriteLine(" =========   Computers =============");
            foreach (ComputerInfo pi in computersDetected)
            {
                Debug.WriteLine(string.Format("{1} [{0}]", pi.ComputerName, pi.ComputerEndPoint));
            }
#endif
        }

        #endregion


        #region IDisposable Implementation
        public void Dispose()
        {
            UnregisterEvents();
        }
        #endregion
    }
}
