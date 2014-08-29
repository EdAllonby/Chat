using System;
using SharedClasses.Message;

namespace ChatClient.ViewModels.LoginWindowViewModel
{
    public class LoginErrorEventArgs : EventArgs
    {
        public string ErrorDescription { get; private set; }

        public LoginResult Result { get; private set; }

        public LoginErrorEventArgs(LoginResult result, string errorDescription)
        {
            Result = result;
            ErrorDescription = errorDescription;
        }
    }
}