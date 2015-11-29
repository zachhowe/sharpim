using System;

namespace SharpIMServer.Events
{
    public class UserStatusEventArgs : EventArgs
    {
        private readonly BuddyStatus status;
        private readonly string username;

        public UserStatusEventArgs(string username, BuddyStatus status)
        {
            this.username = username;
            this.status = status;
        }

        public string Username
        {
            get { return username; }
        }

        public BuddyStatus Status
        {
            get { return status; }
        }
    }
}
