using PPTControllerHost.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPTControllerHost
{
    public class AsyncTaskBase 
    {
        protected BackgroundWorker listerWorker;
        protected IAsyncTask _curTask;
        protected ConsoleViewModelBase _curViewModel;

        public AsyncTaskBase(ConsoleViewModelBase VM)
        {
            listerWorker = new BackgroundWorker();
            _curViewModel = VM;
        }

        public virtual void StartTaskAsync()
        {
            listerWorker.WorkerReportsProgress = false;
            listerWorker.DoWork += listerWorker_DoWork;
            listerWorker.RunWorkerCompleted += listerWorker_RunWorkerCompleted;

            if (!listerWorker.IsBusy)
            {
                listerWorker.RunWorkerAsync();
            }
        }
        
        private void listerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _curTask.StartTask();
        }

        private void listerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _curTask.FinishTask();
        }
    }
}
