using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPTControllerHost
{
    public enum AppExceptionType
    { 
        Warning,
        Error
    }

    public class AppException : Exception
    {
        public AppExceptionType CurAppExceptionType
        {
            get;
            private set;
        }

        public AppException() : base()
        {
 
        }

        public AppException(String message)
            : base(message)
        {

        }

        public AppException(String message, AppExceptionType exceptionType)
            : base(message)
        {
            CurAppExceptionType = exceptionType;
        }
    }
}
