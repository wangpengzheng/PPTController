using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using PPTController.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace PPTController.Tasks
{
    public class BlueToothSendCommandTask : ISendCommandTask
    {        
        public BlueToothSendCommandTask(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<ExecutionPageQuitEvent>().Subscribe(this.HandleExecutionPageQuitEvent);
        }

        public StreamSocket Socket
        {
            get
            {
                if (this.socket == null)
                {
                    this.socket = new StreamSocket();
                }

                return this.socket;
            }
            set
            {
                this.socket = value;
            }
        }

        private StreamSocket socket;

        // Notify the callee or user of this class through a custom event
        public event Delagates.ResponseReceivedEventHandler ResponseReceived;
        private DataWriter _dataWriter;
        private IEventAggregator eventAggregator;
        // A delegate type for hooking up change notifications.
        
        static string dataIn = String.Empty;

        public async void Connect(object obj)
        {
            try
            {
                PeerInformation peer = obj as PeerInformation;

                await this.Socket.ConnectAsync(peer.HostName, "{00001101-0000-1000-8000-00805F9B34FB}");
                this.eventAggregator.GetEvent<ConnectedToBlueToothEvent>().Publish("Pass");
            }
            catch (Exception ex)
            {
                CloseConnection(false);
                this.eventAggregator.GetEvent<ConnectedToBlueToothEvent>().Publish("Fail");
            }  
        }

        public async void Send(string message)
        {
            if (message.Trim().Length == 0)
            {
                System.Windows.MessageBox.Show("Please enter a message to send.", "Can't send", MessageBoxButton.OK);
                return;
            }

            if (Socket == null)
            {
                System.Windows.MessageBox.Show("You are not connected to a Peer.", "Can't send", MessageBoxButton.OK);
                return;
            }

            if (_dataWriter == null)
                _dataWriter = new DataWriter(Socket.OutputStream);

            // Each message is sent in two blocks.
            // The first is the size of the message.
            // The second if the message itself.
            _dataWriter.WriteInt32(message.Length);
            await _dataWriter.StoreAsync();

            _dataWriter.WriteString(message);
            await _dataWriter.StoreAsync();
        }

        public void CloseConnection(bool continueAdvertise)
        {
            if (Socket != null)
            {
                Socket.Dispose();
                Socket = null;
            }

            if (this._dataWriter != null)
            {
                this._dataWriter.Dispose();
                this._dataWriter = null;
            }

            if (continueAdvertise)
            {
                // Since there is no connection, let's advertise ourselves again, so that peers can find us.
                PeerFinder.Start();
            }
            else
            {
                PeerFinder.Stop();
            }
        }

        public void HandleExecutionPageQuitEvent(string str)
        {
            this.CloseConnection(false);
        }

        // Invoke the ResponseReceived event
        protected void OnResponseReceived(ResponseReceivedEventArgs e)
        {
            if (ResponseReceived != null)
                ResponseReceived(this, e);
        }       
    }
}
