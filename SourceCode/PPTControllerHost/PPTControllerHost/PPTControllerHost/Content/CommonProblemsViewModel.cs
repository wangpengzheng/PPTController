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
    public class CommonProblemsViewModel : BasicNotify
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

        public CommonProblemsViewModel()
        {
            #region Add menuitem information.
            try
            {
                DescriptionModel Item0 = new DescriptionModel()
                {
                    Name = "Connection can not created through 'Wifi Connection'",
                    Description = "1. Make sure your computer and phone are under same Wifi environment.\r\n" +
                                  "2. Make sure you had entered a correct IPv4 address.\r\n" +
                                  "3. Make sure there is no firewall or security software is blocking incoming connections.\r\n" +
                                  "4. If you have multi IP address, computer auto detect for Wifi may disabled.\r\n" +
                                  "   Try to manually fill the IP address displayed on tab 'Information' one by one."
                };

                DescriptionModel Item1 = new DescriptionModel()
                {
                    Name = "Connection can not created through 'Bluetooth Connection'",
                    Description = "1. Make sure your computer support bluetooth feature or had a bluetooth plug-in.\r\n" +
                                  "2. If you use the bluetooth plug-in, make sure system have corretly installed the drive." +
                                  "3. Make sure your bluetooth had setup a receive COM port.",
                };

                DescriptionModel Item2 = new DescriptionModel()
                {
                    Name = "HotSpot can not start",
                    Description = "1. Make sure your PC is a laptop and OS version >= win7" +
                                  "2. Try to reopen this app with admin authority.",
                };

                AllDescriptions.Add(Item0);
                AllDescriptions.Add(Item1);
                AllDescriptions.Add(Item2);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Exception : " + e.Message);
            }
            #endregion
        }
    }
}
