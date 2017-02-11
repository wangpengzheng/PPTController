using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPTController.Sensor
{
    public class AccelerometerSensor : ISensor
    {
        Accelerometer accelerometer;
        Vector3 acceleration;
        bool isDataValid;
        DevicePosition position = DevicePosition.DeviceUpward;
        public event Action ImplementNextPage;
        public event Action ImplementPreviousPage;
        private float XValue { get; set; }
        private float ZValue { get; set; }
        private bool resetRequired = false;
        private DevicePosition Position
        {
            get
            {
                return this.position;
            }
            set
            {
                //if (value != this.position)
                //{
                    if (this.movement == DevideMovement.DeviceClockwiseMovement)
                    {
                        this.OnClockwiseMovement();
                    }
                    else
                    {
                        this.OnAntiClockwiseMovement();
                    }
                //}

                this.position = value;
            }
        }
        private DevideMovement movement { get; set; }

        protected virtual void OnClockwiseMovement()//声明事件触发的方法
        {
            if (ImplementNextPage != null)
                ImplementNextPage();
        }

        protected virtual void OnAntiClockwiseMovement()//声明事件触发的方法
        {
            if (ImplementPreviousPage != null)
                ImplementPreviousPage();
        }

        public AccelerometerSensor()
        {
        }

        public void InitializeSensor()
        {
            if (accelerometer != null && accelerometer.IsDataValid)
            {
                // Stop data acquisition from the accelerometer.
                accelerometer.Stop();
            }
            else
            {
                if (accelerometer == null)
                {
                    // Instantiate the accelerometer.
                    accelerometer = new Accelerometer();


                    // Specify the desired time between updates. The sensor accepts
                    // intervals in multiples of 20 ms.
                    accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);


                    accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                }

                try
                {
                    accelerometer.Start();
                }
                catch (InvalidOperationException)
                {

                }

            }
        }

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Note that this event handler is called from a background thread
            // and therefore does not have access to the UI thread. To update 
            // the UI from this handler, use Dispatcher.BeginInvoke() as shown.
            // Dispatcher.BeginInvoke(() => { statusTextBlock.Text = "in CurrentValueChanged"; });


            isDataValid = accelerometer.IsDataValid;
            acceleration = e.SensorReading.Acceleration;

            this.DetectDevicePosition(acceleration.X);
            this.DetectMovement(acceleration.X, acceleration.Z);
        }

        void DetectDevicePosition(float xValue)
        {
            if (xValue > 0.5 && this.resetRequired == false)
            {
                this.resetRequired = true;
                this.OnClockwiseMovement();
            }
            else if (xValue < -0.5 && this.resetRequired == false)
            {
                this.resetRequired = true;
                this.OnAntiClockwiseMovement();
            }
            else if (xValue > -0.1 && xValue < 0.1)
            {
                this.resetRequired = false;
            }
        }

        void DetectMovement(float xValue, float zValue)
        {
            if (xValue > 0 && zValue < 0 && xValue > this.XValue && zValue > this.ZValue) 
                //(xValue > 0 && zValue > 0 && xValue < this.XValue && zValue > this.ZValue) ||
                //(xValue < 0 && zValue < 0 && xValue > this.XValue && zValue < this.ZValue) ||
                //(xValue < 0 && zValue < 0 && xValue > this.XValue && zValue < this.ZValue))
            {
                this.movement = DevideMovement.DeviceClockwiseMovement;
            }
            else if (xValue < 0 && zValue < 0 && xValue < this.XValue && zValue > this.ZValue)
            {
                this.movement = DevideMovement.DeviceAntiClockwiseMovement;
            }
            //if ((xValue > this.XValue && zValue < this.ZValue) ||
            //    (xValue < this.XValue && zValue > this.ZValue))
            //{
            //    this.movement = DevideMovement.DeviceAntiClockwiseMovement;
            //}

            this.XValue = xValue;
            this.ZValue = zValue;
        }
    }

    public enum DevicePosition
    {
        DeviceUpward,
        DeviceDownward
    }

    public enum DevideMovement
    {
        DeviceClockwiseMovement,
        DeviceAntiClockwiseMovement
    }
}
