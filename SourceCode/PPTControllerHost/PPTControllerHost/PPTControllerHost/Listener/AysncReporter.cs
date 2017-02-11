using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPTControllerHost.ViewModel;

namespace PPTControllerHost.Listener
{
    public class AysncReporter
    {
        #region Async Reporting.
        protected ConnectionViewModel attachedViewModel;

        public void ParmConnectionViewModel(ConnectionViewModel VM)
        {
            attachedViewModel = VM;
        }

        protected void ReportMessageAsync(String info, InfoType curType= InfoType.Normal)
        {
            if (attachedViewModel == null)
                throw new Exception("Wrong implementation, attched ViewModel Can't be Null!");

            attachedViewModel.SendInfoMessage(info, curType);
        }
        #endregion
    }
}
