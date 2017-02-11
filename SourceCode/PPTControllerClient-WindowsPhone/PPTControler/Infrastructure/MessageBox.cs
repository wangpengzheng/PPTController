using System.Windows;

namespace PPTController.Infrastructure
{
    public class MessageBox : IMessageBox
    {
        public void Show(string message)
        {
            System.Windows.MessageBox.Show(message);
        }

        public bool Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show(messageBoxText, caption, button);
            return  result == MessageBoxResult.OK || result == MessageBoxResult.Yes;
        }
    }
}
