using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PPTControllerHost.Listener.BlueTooth;
using PPTControllerHost;
using System.ComponentModel;
using PPTControllerHost.ViewModel;
using System.Drawing;
using System.IO;

namespace PPTControllerHost.Listener.Wifi
{
    public class WifiListener : AysncReporter, IPPTControllerListener
    {
        private byte[] bytes = new Byte[StateObject.BufferSize];

        private Socket Listener
        {
            get
            {
                if (this.listener == null)
                {
                    this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }

                return this.listener;
            }
        }

        private Socket listener = null;

        private bool isListening = false;

        public void StartListening()
        {
            // Establish the local endpoint for the socket.
            // Note: remember to keep the portnumber updated if you change
            // it on here, or on the client
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 13001);

            this.ReportMessageAsync("Waiting for a connection...", InfoType.Normal);
            try
            {
                Listener.Bind(localEndPoint);
                Listener.Listen(10);

                Listener.BeginAccept(
                    new AsyncCallback(AcceptMessage),
                    Listener);

                this.isListening = true;
            }
            catch (SocketException portWasUseEx)
            {
                this.StopListening();

                this.ReportMessageAsync("An error found! Please Verify there no other PPTControllerHost currently Opened.", InfoType.Error);
                this.ReportMessageAsync("Details : " + portWasUseEx.Message, InfoType.Error);
            }
            catch (Exception ex)
            {
                this.StopListening();

                this.ReportMessageAsync(ex.Message, InfoType.Error);
                this.ReportMessageAsync("An error occured, below is details information.", InfoType.Error);
            }
        }

        public void StopListening()
        {
            this.isListening = false;
            this.Listener.Close();
        }

        private void ProcessRequest(Socket handler, string cmd)
        {
            byte[] byteData = Encoding.UTF8.GetBytes("Implement Complete.");

            KeyboardSimulator.SimulateOperation(cmd, attachedViewModel);

            if (cmd.Contains("Connect"))
            {
                // Convert the string data to byte data using ASCII encoding.
                byteData = Encoding.UTF8.GetBytes("Connect_Successfully");
                this.ReportMessageAsync("A device had successfully connected!", InfoType.Success);
            }
            else if (cmd.Contains("GetScreen"))
            {
                Thread.Sleep(300);
                byteData = Utils.GetCopyPrimaryScreenAsBytes();
            }


            // Begin sending the data to the remote device.
            try
            {
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(ResponseMessage), handler);
            }
            catch (SocketException ex)
            {
                this.ReportMessageAsync(ex.Message, InfoType.Error);
                this.ReportMessageAsync("The device is missing connection. Please reconnect again!", InfoType.Error);
            }
        }

        private void ResponseMessage(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                this.ReportMessageAsync(e.Message, InfoType.Error);
            }
        }

        private void AcceptMessage(IAsyncResult ar)
        {
            if (this.isListening == true)
            {
                try
                {
                    Listener.BeginAccept(
                            new AsyncCallback(AcceptMessage),
                            Listener);

                    // Get the socket that handles the client request.
                    Socket listener = (Socket)ar.AsyncState;
                    Socket handler = listener.EndAccept(ar);

                    // Create the state object.
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadIncomeMessage), state);
                }
                catch (Exception e)
                {
                    this.ReportMessageAsync(e.Message, InfoType.Error);
                }
            }
        }

        private void ReadIncomeMessage(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // Respond to the client
                    ProcessRequest(handler, content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadIncomeMessage), state);
                }
            }
        }
    }
}
