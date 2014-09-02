using System;
using SharedClasses.Message;

namespace ChatClient.ViewModels.LoginWindowViewModel
{
    public class LoginErrorEventArgs : EventArgs
    {
        public LoginErrorEventArgs(LoginResult result, string errorDescription)
        {
            Result = result;
            ErrorDescription = errorDescription;
        }

        public string ErrorDescription { get; private set; }

        public LoginResult Result { get; private set; }
    }
}