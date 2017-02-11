using System;

namespace PPTController.Infrastructure
{
    /// <summary>
    /// A custom exception class for use in application-specific exceptions
    /// </summary>
    public class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message)
            : base(message)
        {
        }

        public AppException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public AppException(string message, bool userFriendly, bool shouldExit)
            : base(message)
        {
            UserFriendly = userFriendly;
            ShouldExit = shouldExit;
        }

        public AppException(string message, Exception inner, bool userFriendly, bool shouldExit)
            : base(message, inner)
        {
            UserFriendly = userFriendly;
            ShouldExit = shouldExit;
        }

        public bool UserFriendly { get; private set; }

        public bool ShouldExit { get; private set; }
    }
}

