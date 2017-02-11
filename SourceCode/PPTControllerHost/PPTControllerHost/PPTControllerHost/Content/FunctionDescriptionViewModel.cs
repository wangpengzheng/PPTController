using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PPTControllerHost.Model;

namespace PPTControllerHost.Content
{
    public class FunctionDescriptionViewModel : BasicNotify
    {
        private ObservableCollection<DescriptionModel> _allDescriptions;

        public ObservableCollection<DescriptionModel> AllDescriptions
        {
            get
            {
                if (_allDescriptions == null)
                    _allDescriptions = new ObservableCollection<DescriptionModel>();

                return _allDescriptions;
            }
            set
            {
                if (value != _allDescriptions)
                {
                    _allDescriptions = value;
                    this.RaisePropertyChanged("AllDescriptions");
                }
            }
        }

        public FunctionDescriptionViewModel()
        {
            #region Add menuitem information.
            try
            {
                DescriptionModel Item0 = new DescriptionModel()
                {
                    Name = "Wifi Connection",
                    Description = "When your computer and phone are under same wifi environment.\r\n" +
                                  "You phone will able to detect all the computers which installed the PPTControllerHost.\r\n" +
                                  "Click the Connect button in your PPTController to establish a connection between your computer and Phone.\r\n" +
                                  "Then you will able to control your ppt through your Windows phone. Enjoy!"
                };

                DescriptionModel Item1 = new DescriptionModel()
                {
                    Name = "Bluetooth Connection",
                    Description = "When your computer support Bluetooth feature or installed a bluetooth plug-in. \r\n" +
                                  "You need to create a Receive COM port for receive the commands send from your phone.\r\n" +
                                  "Enable the bluetooth auto detection both on your computer and phone.\r\n" +
                                  "After paired them in your PPTController, start to enjoy!"
                };

                DescriptionModel Item2 = new DescriptionModel()
                {
                    Name = "Hotspot",
                    Description = "HotSpot is for the case which didn't have a Wifi connection nor Bluetooth.\r\n" +
                                  "It will help you build your own wifi base on your computer.\r\n" +
                                  "This feature is supported by most of laptops and windows OS version >= win7"
                };

                DescriptionModel Item3 = new DescriptionModel()
                {
                    Name = "Control your PPT By Phone",
                    Description = "When a connection was built between your phone and computer. You will able to Open your ppt, set full screen or exist full screen. Control your presentation process through the action 'next page', 'previous page', 'first page' and 'last page'. \r\n\r\n" +
                    "Make sure you had specify your PPT location to PPTControllerHost, nor you need to open your ppt manually and start your presentation."
                };

                AllDescriptions.Add(Item0);
                AllDescriptions.Add(Item1);
                AllDescriptions.Add(Item2);
                AllDescriptions.Add(Item3);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Exception : " + e.Message);
            }
            #endregion
        }
    }
}
