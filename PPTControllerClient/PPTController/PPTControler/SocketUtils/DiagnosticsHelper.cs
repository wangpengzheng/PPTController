using System;
using System.Windows;

namespace PPTController
{
    /// <summary>
    /// This helper is used to make sure we always instantiate a MessageBox on the UI thread. 
    /// This is useful when you are dealing with alot of asynchronous operations and their callbacks. 
    /// These occur on a backgroudn thread and interacting with the UI from another thread is not allowed and will throw errors.
    /// </summary>
    public class DiagnosticsHelper
    {
        public static MessageBoxResult SafeShow(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageBoxResult result = MessageBoxResult.None;

            // Am I already on the UI thread?
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                result =  MessageBox.Show(messageBoxText, caption, button);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    result = MessageBox.Show(messageBoxText, caption, button);
                });
            }

            return result;
        }

        public static MessageBoxResult SafeShow(string messageBoxText)
        {
            return SafeShow(messageBoxText, String.Empty, MessageBoxButton.OK);
        }
    }
}
