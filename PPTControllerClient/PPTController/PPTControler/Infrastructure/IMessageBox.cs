using System.Windows;

namespace PPTController.Infrastructure
{
    public interface IMessageBox
    {
        void Show(string message);
        bool Show(string messageBoxText, string caption, MessageBoxButton button);
    }
}
