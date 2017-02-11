using System;
using System.Collections.Generic;

namespace PPTController.Infrastructure
{
    /// <summary>
    /// Dependency Injector
    /// </summary>
    public class SimpleContainer : IDisposable
    {
        private bool _disposed;
        private readonly Dictionary<Type, Func<SimpleContainer, object>> _delegates
            = new Dictionary<Type, Func<SimpleContainer, object>>();

        public T Resolve<T>() where T : class
        {
            return _delegates[typeof(T)](this) as T;
        }

        public void Register<T>(Func<SimpleContainer, T> func) where T : class
        {
            _delegates.Add(typeof(T), c => func(c));
        }

        public void UnRegister<T>()
        {
            _delegates.Remove(typeof(T));
        }
        
        #region IDisposable
        
        /*
        ~SimpleContainer()
        {
            Dispose(false);
        }
         * */

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!_disposed)
            {
                // dispose all managed resources
                if (disposeManagedResources)
                {
                    
                }

                // clean up unmanaged resourses here
                {
                    
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
