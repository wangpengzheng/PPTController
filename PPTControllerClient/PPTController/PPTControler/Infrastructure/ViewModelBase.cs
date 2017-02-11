//===============================================================================
// Microsoft patterns & practices
// Windows Phone 7 Developer Guide
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://wp7guide.codeplex.com/license)
//===============================================================================


using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Phone.Reactive;

namespace PPTController.Infrastructure
{
    /// <summary>
    /// This base ViewModel class provides tombstoning support to derived view models. 
    /// The OnPageDeactivation virtual method is called when the user navigates away from a page. 
    /// The OnPageResumeFromTombstoning is called when a page is recovered from tombstoning.
    /// </summary>
    public abstract class ViewModelBase : NotificationObject, IDisposable
    {
        private readonly INavigationService navigationService;
        private bool disposed;
        private readonly Uri pageUri;
        private DelegateCommand<string> _navigateCommand;
        private static string unparsedQueryString;
        private static IDictionary<string, string> queryString = new Dictionary<string, string>();
        private bool isDownloading;
        protected BusyState busyState;

        public static IDictionary<string, string> QueryString
        {
            get { return queryString; }
        }

        public Uri PageUri { get { return this.pageUri; } }

        public virtual ICommand NavigateCommand
        {
            get { return _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(NavigateCommandExecuted, x => CanNavigate)); }
        }

        public bool IsDownloading
        {
            get { return this.isDownloading; }
            private set { this.isDownloading = value; this.RaisePropertyChanged(() => this.IsDownloading); }
        }

        public bool CanNavigate { get; set; }

        public NavigationMode NavigationMode { get; set; }

        private void NavigateCommandExecuted(string destinationUri)
        {
            NavigationService.Navigate(new Uri(destinationUri, UriKind.Relative));
        }

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="navigationService">This parameter provides the ability to navigate between
        /// pages as well as determine if the application has recovered from tombstoning.</param>
        /// <param name="phoneApplicationServiceFacade">This parameter provides a facade over PhoneApplicationService.Current.State.</param>
        /// <param name="pageUri">This parameter identifies the URI of the page that is associated with this ViewModel instance.</param>
        protected ViewModelBase(INavigationService navigationService, Uri pageUri)
        {
            this.pageUri = pageUri;
            this.navigationService = navigationService;

            this.navigationService.RegisterViewModel(this);
            
            this.busyState = new BusyState();
            this.busyState.IsDownloading.Subscribe(x => this.IsDownloading = x);

            CanNavigate = true;
        }

        /// <summary>
        /// When a navigating event is raised, this event handler identifies the view model of the current page, 
        /// and then calls the OnPageDeactivation method only on the view model of the current page. 
        /// A boolean value is passed into the OnPageDeactivation method so that derived view models can handle two cases:
        /// 1) Navigation was due to page to page navigation (intentional navigation).
        /// 2) Navigation was due to deactivation (unintentional navigation).
        /// </summary>
        internal void OnNavigationService_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            Navigating(sender, e);
        }

        public static void SetQueryStringFrom(Uri uri)
        {
            if (unparsedQueryString == uri.ToString())
            {
                // already cached
                return;
            }
            else
            {
                unparsedQueryString = uri.ToString();
                QueryString.Clear();
                int queryStringStartIndex = unparsedQueryString.IndexOf("?");

                if (queryStringStartIndex != -1)
                {
                    string[] queryString = unparsedQueryString.Substring(queryStringStartIndex + 1).Split('&');

                    foreach (string str in queryString)
                    {
                        string[] query = str.Split('=');

                        if (query.Length > 1)
                        {
                            QueryString.Add(query[0], query[1]);
                        }
                    }
                }
            }
        }
        
        // Allows child classes to handle the navigationservice navigating event
        protected virtual void Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            
        }

        /// <summary>
        /// The base ViewModel class uses the navigation service to determine if this view model instance  
        /// needs to recover from tombstoning. If this page needs to recover from tombstoning 
        /// but has not done so, the OnPageResumeFromTombstoning method will be called. 
        /// The navigation service is then notified that this page has completed
        /// tombstone recovery. This ensures that only pages that were tombstoned are resumed via 
        /// the OnPageResumeFromTombstoning method.
        /// </summary>
        internal void OnNavigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this.NavigationMode = e.NavigationMode;

            if (IsResumingFromTombstoning)
            {
                OnPageResumeFromTombstoning();
                navigationService.UpdateTombstonedPageTracking(pageUri);   
            }

            // Might not have set query string on Navigating if we just entered the app
            SetQueryStringFrom(e.Uri);
            Navigated(sender, e);

            if (numberOfPagesToGoBack > 0)
            {
                numberOfPagesToGoBack--;
                NavigationService.GoBack();
            }
        }

        // Allows the child classes to handle the Navigated event.
        protected virtual void Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        ~ViewModelBase()
        {
            this.Dispose();
        }

        protected bool IsResumingFromTombstoning
        {
            get
            {
                return navigationService.DoesPageNeedtoRecoverFromTombstoning(pageUri);
            }
        }

        public INavigationService NavigationService
        {
            get { return this.navigationService; }
        }

        public virtual void OnPageDeactivation(bool isIntentionalNavigation)
        {
        }

        public abstract void OnPageResumeFromTombstoning();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                navigationService.DisposeViewModel(this);
            }

            this.disposed = true;
        }

        /// <summary>
        /// Simple wrapper over PhoneApplicationFrame.GoBack.
        /// Number Of Pages specifies the number of pages to go back.
        /// </summary>
        /// <param name="numberOfPages"></param>
        public void GoBack(int numberOfPages)
        {
            if (numberOfPages < 0)
            {
                throw new AppException("NumberOfPages must be greater then 0");
            }

            numberOfPagesToGoBack = --numberOfPages;
            NavigationService.GoBack();
        }

        public BusyState.BusyToken SetIsDownloading(string operationName)
        {
            return this.busyState.StartOperation(operationName);
        }

        private static int numberOfPagesToGoBack;
    }
}

