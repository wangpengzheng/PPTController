using Microsoft.Devices;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using PPTController.Infrastructure;
using PPTController.Sensor;
using PPTController.Tasks;
using System;
using System.Windows.Input;
using Microsoft.Phone.Tasks;

namespace PPTController.ViewModel
{
    public class SetupViewModel : ViewModelBase
    {
        public SetupViewModel(INavigationService navigationService)
            : base(navigationService, new Uri("/View/Setup.xaml", UriKind.Relative))
        {
            ShakeByAction = Convert.ToBoolean(App.Settings[App.settingsKey.ShakeByAction.ToString()]);
            HotSpotIP = App.Settings[App.settingsKey.HotSpotIP.ToString()].ToString();
        }

        public bool ShakeByAction
        {
            get { return this.shakeByAction; }
            set
            {
                this.shakeByAction = value;
                App.Settings[App.settingsKey.ShakeByAction.ToString()] = value;
                App.Settings.Save();
                RaisePropertyChanged(() => ShakeByAction);
            }
        }

        public string HotSpotIP
        {
            get { return this.hotSpotIP; }
            set { 
                this.hotSpotIP = value;
                App.Settings[App.settingsKey.HotSpotIP.ToString()] = value;
                App.Settings.Save();
                RaisePropertyChanged(() => HotSpotIP); 
            }
        }

        public ICommand VoteCommand
        {
            get { return voteCommand ?? (voteCommand = new DelegateCommand<string>(voteCommandExecuted)); }
        }

        public ICommand VisitCommand
        {
            get { return visitCommand ?? (visitCommand = new DelegateCommand<string>(VisitCommandExecuted)); }
        }

        public ICommand ResetCommand
        {
            get { return resetCommand ?? (resetCommand = new DelegateCommand<string>(ResetCommandExecuted)); }
        }

        private ICommand voteCommand;
        private ICommand visitCommand;
        private ICommand resetCommand;
        private bool shakeByAction;
        private string hotSpotIP;

        private void voteCommandExecuted(string command)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void VisitCommandExecuted(string uriString)
        {
            new WebBrowserTask { Uri = new Uri("http://www.wicresoft.com", UriKind.Absolute) }.Show();
        }

        private void ResetCommandExecuted(string uriString)
        {
            ShakeByAction = true;
            HotSpotIP = "192.168.173.1";
        }

        public override void OnPageResumeFromTombstoning()
        {

        }
    }
}
