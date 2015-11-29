using System;

namespace SharpIMServer.Events
{
    public class GroupMessageEventArgs : EventArgs
    {
        private readonly string from;
        private readonly string group;
        private readonly string message;

        public GroupMessageEventArgs(string from, string group, string message)
        {
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

        public string Group
        {
            get { return group; }
        }
    }
}
