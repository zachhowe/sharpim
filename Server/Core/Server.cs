using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SharpIMServer.Collections;
using SharpIMServer.Events;

namespace SharpIMServer.Core
{
    public class Server
    {
        #region Fields

        private readonly GroupCollection groups;
        private readonly TcpListener listener;
        private readonly Thread poolThread;
        private readonly UserPool users;
        private Task listenerTask;
        private bool isRunning;

        #endregion

        #region Properties

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public UserCollection Users
        {
            get { return users; }
        }

        public GroupCollection Groups
        {
            get { return groups; }
        }

        #endregion

        public Server(int port)
        {
            users = new UserPool();
            groups = new GroupCollection();
            listener = new TcpListener(IPAddress.Any, port);

            poolThread = new Thread(new ThreadStart(users.SocketPollingThread));
            poolThread.Start();
        }

        #region User Event Handlers

        protected void User_OnUserJoinedGroup(string group, string user)
        {
            Group grp = GetGroup(group);

            if (grp == null)
            {
                grp = Group.CreateGroup(group);
                grp.AddAdminUser(user);
                grp.OnGroupSendUserJoined += Group_OnGroupSendUserJoined;
                grp.OnGroupSendUserParted += Group_OnGroupSendUserParted;
                grp.OnGroupSendMessage += Group_OnGroupSendMessage;
                AddGroup(grp);
            }
            else
            {
                grp.AddNormalUser(user);
            }
        }

        protected void User_OnUserPartedGroup(string group, string user)
        {
            Group grp = GetGroup(group);

            if (grp != null)
            {
                grp.RemoveAdminUser(user);
                grp.RemoveNormalUser(user);

                if (grp.Members.Normal.Count == 0 && grp.Members.Admins.Count == 0)
                {
                    RemoveGroup(grp);
                }
            }
        }

        protected void User_OnUserStatusRequest(object sender, string user)
        {
            var usr = (User) sender;

            if (users.GetBuddyStatus(user) == BuddyStatus.Online)
            {
                usr.SendStatus(user, BuddyStatus.Online);
            }
            else if (users.GetBuddyStatus(user) == BuddyStatus.Away)
            {
                usr.SendStatus(user, BuddyStatus.Away);
            }
            else
            {
                usr.SendStatus(user, BuddyStatus.Away);
            }
        }

        protected void User_OnAuthenticated(object sender)
        {
            var user = (User) sender;

            if (GetUser(user.Username) == null)
            {
                user.OnUserGroupMessage += User_OnUserGroupMessage;
                user.OnUserQuit += User_OnUserQuit;
                user.OnUserMessage += User_OnUserMessage;
                user.OnUserStatus += User_OnUserStatus;
                user.OnUserStatusRequest += User_OnUserStatusRequest;
                user.OnUserJoinedGroup += User_OnUserJoinedGroup;
                user.OnUserPartedGroup += User_OnUserPartedGroup;
                user.OnUserSignOnTimeRequest += User_OnUserSignOnTimeRequest;

                AddUser(user);
            }
            else
            {
            }
        }

        protected void User_OnUserMessage(object sender, UserMessageEventArgs e)
        {
            User user = GetUser(e.To);

            if (user != null)
            {
                user.SendUserMessage(e.From, e.Message);
            }
            else
            {
                User from = GetUser(e.From);

                if (from != null)
                {
                    from.SendStatus(e.To, BuddyStatus.Offline);
                }
            }
        }

        protected void User_OnUserQuit(object sender)
        {
            var usr = (User) sender;

            Console.WriteLine("{0} has just signed off!", usr.Username);

            RemoveUser(usr);
        }

        protected void User_OnUserSignOnTimeRequest(object sender, string user)
        {
            var usrSender = (User) sender;
            User usrTarget = GetUser(user);

            if (usrTarget != null)
            {
                usrSender.SendUserSignOnTime(user, usrTarget.SignOnTime);
            }
        }

        protected void User_OnUserStatus(object sender, UserStatusEventArgs e)
        {
            var usr = (User) sender;

            foreach (string budd in usr.Buddies)
            {
                User buddy = GetUser(budd);

                if (buddy != null)
                {
                    if (buddy.Buddies.Contains(usr.Username))
                    {
                        buddy.SendStatus(e.Username, e.Status);
                    }
                }
            }
        }

        protected void User_OnUserGroupMessage(object sender, GroupMessageEventArgs e)
        {
            Group group = GetGroup(e.Group);

            if (group != null)
            {
                // Check if user exists in group.
                var user = (User) sender;

                if (group.Members.Admins.Contains(user.Username))
                {
                    group.SendUserMessage(e.From, e.Message);
                }
                else if (group.Members.Normal.Contains(user.Username))
                {
                    group.SendUserMessage(e.From, e.Message);
                }
            }
        }

        #endregion

        #region Group Event Handlers

        protected void Group_OnGroupSendMessage(object sender, GroupSendMessageEventArgs e)
        {
            User user = GetUser(e.To);

            if (user != null)
            {
                user.SendGroupMessage(e.Group, e.From, e.Message);
            }
        }

        protected void Group_OnGroupSendUserJoined(string to, string user, string group)
        {
            User usr = GetUser(to);

            if (usr != null)
            {
                usr.SendUserJoined(group, user);
            }
        }

        protected void Group_OnGroupSendUserParted(string to, string user, string group)
        {
            User usr = GetUser(to);

            if (usr != null)
            {
                usr.SendUserParted(group, user);
            }
        }

        protected void Group_OnGroupSendUserKicked(string to, string user, string group)
        {
            User usr = GetUser(to);

            if (usr != null)
            {
                usr.SendUserParted(group, user);
            }
        }

        protected void Group_OnGroupSendUserBanned(string to, string user, string group)
        {
            User usr = GetUser(to);

            if (usr != null)
            {
                usr.SendUserParted(group, user);
            }
        }

        #endregion

        private async void ListenLoop()
        {
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                if (socket == null)
                {
                    break;
                }
                
                var user = new User(socket);
                user.OnAuthenticated += User_OnAuthenticated;
                user.Authenticate();
            }
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public void RemoveUser(User user)
        {
            users.Remove(user);
        }

        public void AddGroup(Group group)
        {
            groups.Add(group);
        }

        public void RemoveGroup(Group group)
        {
            groups.Remove(group);
        }

        private User GetUser(string username)
        {
            return users.GetUser(username);
        }

        private Group GetGroup(string groupname)
        {
            foreach (Group group in groups)
            {
                if (group.GroupName == groupname)
                {
                    return group;
                }
            }

            return null;
        }

        public void ListUsers()
        {
            Console.WriteLine("Users connected ({0:d} Total):", users.Count);
            Console.WriteLine();

            foreach (User user in users)
            {
                Console.WriteLine(user.Username);
            }
        }

        public void StartServer()
        {
            if (!isRunning)
            {
                isRunning = true;
                listener.Start();
                listenerTask = Task.Factory.StartNew(() => ListenLoop());
            }
        }

        public void StopServer()
        {
            if (isRunning)
            {
                isRunning = false;
                listener.Stop();
            }
        }
    }
}
