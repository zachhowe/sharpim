using System;

namespace SharpIMServer.Events
{
    public class UserMessageEventArgs : EventArgs
    {
        private readonly string from;
        private readonly string message;
        private readonly string to;

        public UserMessageEventArgs(string from, string to, string message)
        {
            this.from = from;
            this.to = to;
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }

        public string From
        {
            get { return from; }
        }

        public string To
        {
            get { return to; }
        }
    }
}
