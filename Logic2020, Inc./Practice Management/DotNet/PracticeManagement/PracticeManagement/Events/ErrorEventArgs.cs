using System;

namespace PraticeManagement.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }
    }
}

