using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Management;
using PPTControllerHost.ViewModel;
using System.Drawing;

namespace PPTControllerHost.Listener.BlueTooth
{
    /// <summary>
    /// Manager for serial port data
    /// </summary>
    public class BlueToothListener : AysncReporter, IDisposable, IPPTControllerListener
    {
        private Boolean firstLaunch = true;
        public BlueToothListener()
        {
            // Finding installed serial ports on hardware
            _currentSerialSettings.PortNameCollection = SerialPort.GetPortNames();
            _currentSerialSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_currentSerialSettings_PropertyChanged);
            this.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(ProcessRequest);
        }

        #region Fields
        private SerialPort _serialPort;
        private SerialSettings _currentSerialSettings = new SerialSettings();
        private string _latestRecieved = String.Empty;
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current serial port settings
        /// </summary>
        public SerialSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set { _currentSerialSettings = value; }
        }

        #endregion

        #region Event handlers

        void _currentSerialSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // if serial port is changed, a new baud query is issued
            if (e.PropertyName.Equals("PortName"))
                UpdateBaudRateCollection();
        }


        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int dataLength = _serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = _serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;

            // Send data to whom ever interested
            if (NewSerialDataRecieved != null)
                NewSerialDataRecieved(this, new SerialDataEventArgs(data));
        }

        #endregion

        #region Methods
        private void ProcessRequest(object sender, SerialDataEventArgs e)
        {
            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string cmd = Encoding.ASCII.GetString(e.Data);
            if (cmd.Contains("GetScreen"))
            {
                Thread.Sleep(500);
                byte[] bytes = Utils.GetCopyPrimaryScreenAsBytes();
                _serialPort.Write(bytes, 0, bytes.Length);
                _serialPort.BaseStream.Flush();
            }
            else
            {
                KeyboardSimulator.SimulateOperation(cmd, attachedViewModel);
            }

            if (firstLaunch)
            {
                this.ReportMessageAsync("A device had successfully connected!", InfoType.Success);
                firstLaunch = false;
            }
        }


        /// <summary>
        /// Connects to a serial port defined through the current settings
        /// </summary>
        public void StartListening()
        {
            // If serial ports is found, we select the first found
            if (_currentSerialSettings.PortNameCollection.Length > 0)
            {
                // Closing serial port if it is open
                if (_serialPort != null && _serialPort.IsOpen)
                    _serialPort.Close();

                try
                {
                    string caption = "";
                    bool isFound = false;

                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_PnPEntity");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        var captionObj = queryObj["Caption"];
                        caption = (captionObj != null) ? captionObj.ToString() : null;

                        if (caption != null && (caption.Contains("SPP") || caption.Contains("Bluetooth")))
                        {
                            foreach (string port in _currentSerialSettings.PortNameCollection)
                            {
                                if (caption.Contains(port))
                                {
                                    _currentSerialSettings.PortName = port;
                                    isFound = true;
                                    break;
                                }
                            }
                        }

                        if (isFound == true)
                        { break; }
                    }

                    if (isFound == true)
                    {
                        this.ReportMessageAsync(string.Format("{0} is listening bluetooth connection...", _currentSerialSettings.PortName), InfoType.Success);
                    }
                    else
                    {
                        this.ReportMessageAsync("There is no serial port avaliable for listening bluetooth connection, it may caused by the OS delay, please wait a few seconds and try it again.", InfoType.Error);
                        return;
                    }
                }
                catch (ManagementException e)
                {
                    this.ReportMessageAsync("Please make sure you are running the app with administrator privilege and try it again.", InfoType.Error);
                    return;
                }
            }
            else
            {
                this.ReportMessageAsync("There is no serial port avaliable for listening bluetooth connection, please double check and try it again.", InfoType.Error);
                return;
            }

            // Setting serial port settings
            _serialPort = new SerialPort(
                _currentSerialSettings.PortName,
                _currentSerialSettings.BaudRate,
                _currentSerialSettings.Parity,
                _currentSerialSettings.DataBits,
                _currentSerialSettings.StopBits);

            // Subscribe to event and open serial port for data
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            _serialPort.Open();
        }

        /// <summary>
        /// Closes the serial port
        /// </summary>
        public void StopListening()
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                _serialPort.Close();
            }
        }

        /// <summary>
        /// Retrieves the current selected device's COMMPROP structure, and extracts the dwSettableBaud property
        /// </summary>
        private void UpdateBaudRateCollection()
        {
            try
            {
                _serialPort = new SerialPort(_currentSerialSettings.PortName);
                _serialPort.Open();
                object p = _serialPort.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_serialPort.BaseStream);
                Int32 dwSettableBaud = (Int32)p.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);

                _serialPort.Close();
                _currentSerialSettings.UpdateBaudRateCollection(dwSettableBaud);
            }
            catch (Exception ex)
            {
                this.ReportMessageAsync("Details Information: " + ex.Message, InfoType.Error);
                this.ReportMessageAsync("An error found: Please make use your Bluetooth is not in used by other process." + ex.Message, InfoType.Error);
            }
        }

        // Call to release serial port
        public void Dispose()
        {
            Dispose(true);
        }

        // Part of basic design pattern for implementing Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            }
            // Releasing serial port (and other unmanaged objects)
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort.Dispose();
            }
        }
        #endregion
    }

    /// <summary>
    /// EventArgs used to send bytes recieved on serial port
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }

        /// <summary>
        /// Byte array containing data from serial port
        /// </summary>
        public byte[] Data;
    }
}
