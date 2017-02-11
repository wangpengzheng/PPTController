using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPTControllerHost
{
    public interface IAsyncTask
    {
        void StartTask();
        //void CancelTask();
        void FinishTask();
    }

    public interface IRevertable
    {
        void RevertTask();
    }
}
