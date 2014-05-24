using System;

namespace ChatClient.ViewModels.LoginWindowViewModel
{
    internal class UserAlreadyConnectedException : Exception
    {
        public UserAlreadyConnectedException()
        {
        }

        public UserAlreadyConnectedException(string message)
            : base(message)
        {
        }

        public UserAlreadyConnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}