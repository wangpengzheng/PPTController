using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPTController.Resources;

namespace PPTController
{
    public class LocalizedStrings
    {
        public LocalizedStrings() { }

        private static AppResource localizedResources = new AppResource();

        public AppResource LocalizedResources { get { return localizedResources; } }
    }
}
