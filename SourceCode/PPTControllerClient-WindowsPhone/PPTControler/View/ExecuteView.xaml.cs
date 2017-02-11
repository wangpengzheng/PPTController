using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Devices;
using PPTController.Infrastructure;
using Microsoft.Practices.Prism.Events;
using PPTController.Events;
using Microsoft.Phone.Shell;
using PPTController.ViewModel;
using PPTController.Resources;

namespace PPTController
{
    public partial class ExecuteView : PhoneApplicationPage
    {
        // Constructor
        public ExecuteView()
        {
            InitializeComponent();
            ContainerManager c = new ContainerManager();
            eventAggregator = c.Container.Resolve<IEventAggregator>();

            BuildLocalizedApplicationBar();
        }

        private IEventAggregator eventAggregator;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.eventAggregator.GetEvent<ExecutionPageQuitEvent>().Publish("");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                if (e.HorizontalVelocity > 0)
                {
                    eventAggregator.GetEvent<ExecutionEvent>().Publish("END");
                }
                else
                {
                    eventAggregator.GetEvent<ExecutionEvent>().Publish("HOME");
                }
            }
            else
            {
                if (e.VerticalVelocity > 0)
                {
                    eventAggregator.GetEvent<ExecutionEvent>().Publish("DOWN");
                }
                else
                {
                    eventAggregator.GetEvent<ExecutionEvent>().Publish("UP");
                }
            }
        }

        private void StartPPT_Click_1(object sender, EventArgs e)
        {
            var vm = DataContext as ExecuteViewModel;
            if (vm != null)
            {
                vm.ImplementPPTCommand.Execute("START");
            }
        }

        private void FullScreen_Click_1(object sender, EventArgs e)
        {
            var vm = DataContext as ExecuteViewModel;
            if (vm != null)
            {
                vm.ImplementPPTCommand.Execute("FULL");
            }
        }

        private void Exit_Click_1(object sender, EventArgs e)
        {
            var vm = DataContext as ExecuteViewModel;
            if (vm != null)
            {
                vm.ImplementPPTCommand.Execute("ESC");
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem StartPPT = new ApplicationBarMenuItem(AppResource.StartPPT);
            StartPPT.Click += StartPPT_Click_1;

            ApplicationBarMenuItem FullScreen = new ApplicationBarMenuItem(AppResource.FullScreen);
            FullScreen.Click += FullScreen_Click_1;

            ApplicationBarMenuItem Exit = new ApplicationBarMenuItem(AppResource.NormalView);
            Exit.Click += Exit_Click_1;

            ApplicationBar.MenuItems.Add(StartPPT);
            ApplicationBar.MenuItems.Add(FullScreen);
            ApplicationBar.MenuItems.Add(Exit);
        }
    }
}