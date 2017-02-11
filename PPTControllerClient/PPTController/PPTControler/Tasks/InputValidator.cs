using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PPTController.Tasks
{
    public class InputValidator
    {
        private Regex regexIPAddress = new Regex(@"^\d+\.\d+\.\d+\.\d+$");
        private Regex regexPort = new Regex(@"^\d+$");

        public bool ValidateIpAddress(string ipAddress)
        {
            return regexIPAddress.IsMatch(ipAddress);
        }

        public bool ValidatePort(string port)
        {
            return regexPort.IsMatch(port);
        }
    }
}
