using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PPTController.Infrastructure
{
    /// <summary>
    /// This class provides both a wrapper over navigation related methods provided by the 
    /// PhoneApplicationFrame, and tombstoning support. The RecoveredFromTombstoning property is expected to be 
    /// set based on the value of e.IsApplicationInstancePreserved in the Application_Activated event handler (App.xaml.cs).
    /// However, since this value is scoped to the application, this class also determines which pages
    /// were tombstoned so that they can properly recover from tombstoning.
    /// </summary>
    public class ApplicationFrameNavigationService : INavigationService
    {
        private readonly PhoneApplicationFrame frame;
        private Dictionary<string, bool> tombstonedPages;

        public ApplicationFrameNavigationService(PhoneApplicationFrame frame)
        {
            this.frame = frame;
            this.frame.Navigated += frame_Navigated;
            this.frame.Navigating += frame_Navigating;
            this.frame.Obscured += frame_Obscured;
            this.RecoveredFromTombstoning = false;
        }

        /// <summary>
        /// Simple wrapper over PhoneApplicationFrame.CanGoBack.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.frame.CanGoBack; }
        }

        /// <summary>
        /// This value is expected to be set based on the value of e.IsApplicationInstancePreserved 
        /// in the Application_Activated event handler (App.xaml.cs).
        /// </summary>
        public bool RecoveredFromTombstoning { get; set; }

        /// <summary>
        /// This value is expected to be set to true when the Application_Launching event handler executes
        /// </summary>
        public bool ApplicationIsLoading { get; set; }

        /// <summary>
        /// This method determines if a given page needs to recover from tombstoning. 
        /// This is used in the base ViewModel class in order to determine whether or not to call the 
        /// ViewModel.OnPageResumeFromTombstoning method.
        /// This method fails fast if the application did not resume from tombstoning 
        /// (see the RecoveredFromTombstoning property).
        /// 
        /// It is assumed that we can determine the list of pages that were tombstoned by taking the
        /// first page/view model that was instantiated due to a tombstoning recovery, and then examining
        /// the backstack. This method populates a dictionary with the pages that were tombstoned and 
        /// uses this dictionary to track whether the base ViewModel class called the OnPageResumedFromTombstoning
        /// method for each tombstoned page.
        /// </summary>
        public bool DoesPageNeedtoRecoverFromTombstoning(Uri pageUri)
        {
            if (!RecoveredFromTombstoning) return false;

            if (tombstonedPages == null)
            {
                tombstonedPages = new Dictionary<string, bool>();
                tombstonedPages.Add(pageUri.ToString(), true);
                foreach (var journalEntry in frame.BackStack)
                {
                    // This is a fix for navigating forward to the same view twice. 
                    // In this case there would be two sources that are equal and adding to tombstonedPages will fail.
                    if (!tombstonedPages.ContainsKey(journalEntry.Source.ToString()))
                    {
                        tombstonedPages.Add(journalEntry.Source.ToString(), true);
                    }                    
                }
                return true;
            }

            if (tombstonedPages.ContainsKey(pageUri.ToString()))
            {
                return tombstonedPages[pageUri.ToString()];
            }
            return false;
        }

        public void UpdateTombstonedPageTracking(Uri pageUri)
        {
            tombstonedPages[pageUri.ToString()] = false;
        }

        /// <summary>
        /// Simple wrapper over PhoneApplicationFrame.Navigate.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool Navigate(Uri source)
        {
            return this.frame.Navigate(source);
        }

        /// <summary>
        /// Simple wrapper over PhoneApplicationFrame.GoBack.
        /// </summary>
        public void GoBack()
        {
            this.frame.GoBack();
        }

        /// <summary>
        /// Pass the event to the first matching ViewModel for that Uri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back && frame.BackStack != null)
            {
                // (If we just backed INTO this application from tombstoning, we might not need to remove any.)
                var eUri = e.Uri.ToString();
                if (activeStack.Any(v => eUri.StartsWith(v.PageUri.ToString())))
                {
                    while (activeStack.Any() && !eUri.StartsWith(activeStack.First().PageUri.ToString()))
                        activeStack.First().Dispose();
                }
            }
            var navigated = this.activeStack.FirstOrDefault(vm => e.Uri.ToString().StartsWith(vm.PageUri.ToString()));
            if (navigated != null)
            {
                navigated.OnNavigationService_Navigated(sender, e);
                this.activeViewModel = navigated;
            }
        }

        /// <summary>
        /// Pass the event to the first matching ViewModel for that Uri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (this.activeViewModel != null)
            {
                this.activeViewModel.OnPageDeactivation(e.IsNavigationInitiator);
            }
            var uri = e.Uri;
            ViewModelBase.SetQueryStringFrom(uri);
            var navigating = this.activeStack.FirstOrDefault(vm => e.Uri.ToString().StartsWith(vm.PageUri.ToString()));
            if (navigating != null)
            {
                navigating.OnNavigationService_Navigating(sender, e);
            }
        }

        public event EventHandler<ObscuredEventArgs> Obscured;

        void frame_Obscured(object sender, ObscuredEventArgs e)
        {
            var handler = this.Obscured;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        ViewModelBase activeViewModel = null;
        Stack<ViewModelBase> activeStack = new Stack<ViewModelBase>();

        /// <summary>
        /// Put the ViewModel at the head of the "active" view stack
        /// </summary>
        public void RegisterViewModel(ViewModelBase viewModel)
        {
            this.activeStack.Push(viewModel);
        }

        /// <summary>
        /// Mark the ViewModel as no longer active
        /// </summary>
        /// <param name="viewModel"></param>
        public void DisposeViewModel(ViewModelBase viewModel)
        {
            System.Diagnostics.Debug.Assert(this.activeStack.Contains(viewModel));
            var popped = this.activeStack.Pop();
            System.Diagnostics.Debug.Assert(viewModel == popped);
        }
    }
}
