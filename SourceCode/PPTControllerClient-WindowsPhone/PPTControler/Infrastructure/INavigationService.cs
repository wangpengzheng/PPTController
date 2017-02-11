using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PPTController.Infrastructure
{
    public interface INavigationService
    {
        bool CanGoBack { get; }
        bool Navigate(Uri source);
        void GoBack();
        event EventHandler<ObscuredEventArgs> Obscured;
        bool RecoveredFromTombstoning { get; set; }
        bool ApplicationIsLoading { get; set; }
        void UpdateTombstonedPageTracking(Uri pageUri);
        bool DoesPageNeedtoRecoverFromTombstoning(Uri pageUri);

        void RegisterViewModel(ViewModelBase viewModel);

        void DisposeViewModel(ViewModelBase viewModel);
    }
}
