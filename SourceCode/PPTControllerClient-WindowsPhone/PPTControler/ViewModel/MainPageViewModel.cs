using Microsoft.Devices;
using Microsoft.Practices.Prism.Commands;
using PPTController.Infrastructure;
using PPTController.Sensor;
using PPTController.Tasks;
using System;
using System.Windows.Input;

namespace PPTController.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService, new Uri("/View/MainPage.xaml", UriKind.Relative))
        {
        }

        public ICommand WifiCommand
        {
            get { return wifiCommand ?? (wifiCommand = new DelegateCommand<string>(WifiCommandExecuted)); }
        }

        public ICommand BlueToothCommand
        {
            get { return blueToothCommand ?? (blueToothCommand = new DelegateCommand<string>(BlueToothExecuted)); }
        }

        public ICommand HotSpotCommand
        {
            get { return hotSpotCommand ?? (hotSpotCommand = new DelegateCommand<string>(HotSpotExecuted)); }
        }

        public ICommand SetupCommand
        {
            get { return setupCommand ?? (setupCommand = new DelegateCommand<string>(SetupExecuted)); }
        }

        private ICommand wifiCommand;
        private ICommand blueToothCommand;
        private ICommand hotSpotCommand;
        private ICommand setupCommand;

        private void WifiCommandExecuted(string command)
        {
            this.NavigationService.Navigate(new Uri("/View/WifiConnnectionView.xaml", UriKind.Relative));
        }

        private void BlueToothExecuted(string uriString)
        {
            this.NavigationService.Navigate(new Uri("/View/BlueToothConnnectionView.xaml", UriKind.Relative));
        }

        private void HotSpotExecuted(string uriString)
        {
            this.NavigationService.Navigate(new Uri("/View/WifiIPDirectView.xaml?HotSpot=true", UriKind.Relative));
        }

        private void SetupExecuted(string uriString)
        {
            this.NavigationService.Navigate(new Uri("/View/Setup.xaml", UriKind.Relative));
        }

        public override void OnPageResumeFromTombstoning()
        {

        }
    }
}

