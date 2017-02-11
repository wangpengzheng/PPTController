using System;

namespace PPTController.Infrastructure
{
    public class ResponseReceivedEventArgs : EventArgs
    {
        // True if an error occured, False otherwise
        public bool isError { get; set; }

        // If there was an erro, this will contain the error message, data otherwise
        public string response { get; set; }
    }
}
