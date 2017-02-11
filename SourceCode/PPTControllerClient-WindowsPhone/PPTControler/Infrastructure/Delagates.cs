using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPTController.Infrastructure
{
    public class Delagates
    {
        public delegate void ResponseReceivedEventHandler(object sender, ResponseReceivedEventArgs e);
    }
}
