using System;

namespace SharpIMServer.Events
{
    public class GroupSendMessageEventArgs : EventArgs
    {
        private readonly string from;
        private readonly string group;
        private readonly string message;
        private readonly string to;

        public GroupSendMessageEventArgs(string group, string from, string to, string message)
        {
            this.to = to;
            this.from = from;
            this.group = group;
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

        public string Group
        {
            get { return group; }
        }
    }
}
