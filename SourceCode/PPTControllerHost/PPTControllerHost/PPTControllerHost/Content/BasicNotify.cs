using System.ComponentModel;

namespace PPTControllerHost
{
    public class BasicNotify : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string _propertyName)
        {
            PropertyChangedEventHandler hander = this.PropertyChanged;

            if (hander != null)
            {
                hander(this, new PropertyChangedEventArgs(_propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
