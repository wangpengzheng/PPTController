using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using PPTController.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPTController.Tasks
{
    public class SocketSendCommandTask : ISendCommandTask
    {
        public SocketSendCommandTask(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<ExecutionPageQuitEvent>().Subscribe(this.HandleExecutionPageQuitEvent);
            this.ResponseReceived += new Delagates.ResponseReceivedEventHandler(this.HandelResponse);
        }
        
        public string ServerIP { get; set; }
        public int Port { get; set; }
        // Notify the callee or user of this class through a custom event
        public event Delagates.ResponseReceivedEventHandler ResponseReceived;
        private IEventAggregator eventAggregator;
        static string dataIn = String.Empty;
        private bool hasResponse = false;

        public void Connect(object obj)
        {
            SocketSendCommandInfo info = obj as SocketSendCommandInfo;

            this.ServerIP = info.IPAddress;
            this.Port = int.Parse(info.Port);
            this.Send("Connect");
        }

        public void Send(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data");
            }

            dataIn = data;

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

            DnsEndPoint hostEntry = new DnsEndPoint(this.ServerIP, this.Port);

            // Create a socket and connect to the server 

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventArg_Completed);
            socketEventArg.RemoteEndPoint = hostEntry;

            socketEventArg.UserToken = sock;

            try
            {
                sock.ConnectAsync(socketEventArg);

                if (data == "Connect")
                {
                    new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(3000);

                        if (this.hasResponse == false)
                        {
                            this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Publish("Fail");
                        }
                        
                    })).Start();
                }    
            }
            catch (SocketException ex)
            {
                throw new SocketException((int)ex.ErrorCode);
            }
        }


        // A single callback is used for all socket operations. 
        // This method forwards execution on to the correct handler 
        // based on the type of completed operation 
        void SocketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new Exception("Invalid operation completed");
            } 
        }

        // Called when a ConnectAsync operation completes 
        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // Successfully connected to the server 
                // Send data to the server 
                byte[] buffer = Encoding.UTF8.GetBytes(dataIn + "<EOF>");
                e.SetBuffer(buffer, 0, buffer.Length);
                Socket sock = e.UserToken as Socket;
                sock.SendAsync(e);
            }
            else
            {
                ResponseReceivedEventArgs args = new ResponseReceivedEventArgs();
                args.response = e.SocketError.ToString();
                args.isError = true;
                OnResponseReceived(args);

            } 
        }

        // Called when a ReceiveAsync operation completes  
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // Received data from server 
                string dataFromServer = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);

                Socket sock = e.UserToken as Socket;
                sock.Shutdown(SocketShutdown.Send);
                sock.Close();

                // Respond to the client in the UI thread to tell him that data was received
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ResponseReceivedEventArgs args = new ResponseReceivedEventArgs();
                    args.response = dataFromServer;
                    OnResponseReceived(args);
                });

            }
            else
            {
                this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Publish("Fail");
            }
        }

        // Called when a SendAsync operation completes 
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Read data sent from the server 
                Socket sock = e.UserToken as Socket;

                sock.ReceiveAsync(e);
            }
            else
            {
                ResponseReceivedEventArgs args = new ResponseReceivedEventArgs();
                args.response = e.SocketError.ToString();
                args.isError = true;
                OnResponseReceived(args);
            }
        }

        // Invoke the ResponseReceived event
        protected void OnResponseReceived(ResponseReceivedEventArgs e)
        {
            if (ResponseReceived != null)
                ResponseReceived(this, e);
        }

        public void HandleExecutionPageQuitEvent(string str)
        {
            
        }

        void HandelResponse(object sender, ResponseReceivedEventArgs e)
        {
            this.hasResponse = true;
            
            if (e.isError)
            {
                this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Publish(e.response);
            }
            else if (e.response == "Connect_Succ")
            {
                this.eventAggregator.GetEvent<ConnectedToWifiEvent>().Publish("Pass");
            }
        }
    }
}
