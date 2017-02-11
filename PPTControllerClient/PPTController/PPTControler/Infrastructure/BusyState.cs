using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPTController.Infrastructure
{
    /// <summary>
    /// ViewModel-level concept for keeping track of when a ViewModel has outstanding data operations
    /// </summary>
    public class BusyState
    {
        List<string> ongoingOperations = new List<string>();
        Subject<bool> isDownloading = new Subject<bool>();

        public BusyState() { }

        public IObservable<bool> IsDownloading
        {
            get
            {
                return this.isDownloading;
            }
        }

        public class BusyToken
        {
            bool alreadyEnded;
            string operationName;
            BusyState parent;
            public BusyToken(BusyState parent, string operationName)
            {
                this.operationName = operationName;
                this.parent = parent;
            }
            public void Done()
            {
                if (!alreadyEnded)
                {
                    alreadyEnded = true;
                    parent.EndOperation(operationName);
                }
            }
        }

        // OperationName is for debugging purposes only. All we really care about is that all the operations which start must end
        // before we remove the busy indicator.
        public BusyToken StartOperation(string operationName)
        {
            if (!this.ongoingOperations.Any())
            {
                this.isDownloading.OnNext(true);
            }
            ongoingOperations.Add(operationName);
            return new BusyToken(this, operationName);
        }

        public void EndOperation(string operationName)
        {
            EndOperation(operationName, false);
        }

        public bool TryEndOperation(string operationName)
        {
            return EndOperation(operationName, true);
        }

        private bool EndOperation(string operationName, bool tryEndOperation)
        {
            if (!this.ongoingOperations.Contains(operationName))
            {
                if (tryEndOperation)
                {
                    return false;
                }

                throw new InvalidOperationException(string.Format("Cannot end operation '{0}' because no operation '{0}' is currently executing. This is a bug.", operationName));
            }

            this.ongoingOperations.Remove(operationName);

            if (!this.ongoingOperations.Any())
            {
                this.isDownloading.OnNext(false);
            }

            return true;
        }
    }
}
