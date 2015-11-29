using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SharpIMClient.Collections;

namespace SharpIMClient
{
    public class SharpClient
    {
        private readonly BuddyCollection _buddyList;
        private bool _isConnected;
        private NetworkStream _ns;
        private string _password;
        private StreamReader _sr;
        private SslStream _ssl;
        private StreamWriter _sw;
        private TcpClient _tcp;
        private string _username;

        public SharpClient()
        {
            _buddyList = new BuddyCollection();
        }

        public string ScreenName
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public BuddyCollection Buddies
        {
            get { return _buddyList; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public event UserMessageEventHandler OnUserMessage;
        public event ServerMessageEventHandler OnServerMessage;
        public event UserStatusEventHandler OnUserStatus;
        public event BuddyListReceivedEventHandler OnBuddyListReceived;
        public event GroupMessageEventHandler OnGroupMessage;
        public event UserPartedGroupEventHandler OnUserPartedGroup;
        public event UserJoinedGroupEventHandler OnUserJoinedGroup;
        public event UserKickedUserEventHandler OnUserKickedUser;
        public event UserBannedUserEventHandler OnUserBannedUser;

        public void SendUserMessage(string user, string message)
        {
            _sw.WriteLine("MSG {0} {1}", user, message);
            _sw.Flush();
        }

        public void SendGroupMessage(string group, string message)
        {
            _sw.WriteLine("MSG #{0} {1}", group, message);
            _sw.Flush();
        }

        public void JoinGroup(string group)
        {
            _sw.WriteLine("JOIN {0}", group);
            _sw.Flush();
        }

        public void PartGroup(string group)
        {
            _sw.WriteLine("PART {0}", group);
            _sw.Flush();
        }

        public void GetBuddy(string user)
        {
            while (!_buddyList.Contains(user))
            {
                GetBuddySignOnTime(user);
                GetBuddyStatus(user);
            }
        }

        private void GetBuddyStatus(string user)
        {
            _sw.WriteLine("GET STATUS {0}", user);
            _sw.Flush();
        }

        private void GetBuddySignOnTime(string user)
        {
            _sw.WriteLine("GET SIGNONTIME {0}", user);
            _sw.Flush();
        }

        public void SetEmail(string email)
        {
            _sw.WriteLine("SET EMAIL {0}", email);
            _sw.Flush();
        }

        public void GetBuddyList()
        {
            _sw.WriteLine("GET BUDDYLIST");
            _sw.Flush();
        }

        public void AddBuddy(string buddy)
        {
            _sw.WriteLine("ADD BUDDY {0}", buddy);
            _sw.Flush();

            _buddyList.Add(new BuddyInfo(buddy));
            GetBuddy(buddy);
        }

        public void RemoveBuddy(string buddy)
        {
            _sw.WriteLine("REMOVE BUDDY {0}", buddy);
            _sw.Flush();

            Buddies.Remove(_buddyList.GetBuddy(buddy));
        }

        public void GlobalMessage(string message)
        {
            _sw.WriteLine("GLOBALMSG {0}", message);
            _sw.Flush();
        }

        private void UpdateBuddyStatus(string name, BuddyStatus status)
        {
            BuddyInfo budd = _buddyList.GetBuddy(name);
            budd.Status = status;
        }

        private void UpdateBuddySignOnTime(string name, DateTime time)
        {
            BuddyInfo budd = _buddyList.GetBuddy(name);
            budd.SignOnTime = time;
        }

        private void ReaderThread()
        {
            while (true)
            {
                string msg = _sr.ReadLine();
                string[] msgarray = msg.Split(' ');

                if (msgarray[0] == "MSG" && msgarray.Length >= 4)
                {
                    if (msgarray[1].ToUpper() == "USER")
                    {
                        if (OnUserMessage != null)
                            OnUserMessage(this, msgarray[2], Utility.GetRestOfMessage(msgarray, 3));
                    }
                    else if (msgarray[1].ToUpper() == "GROUP")
                    {
                        if (OnGroupMessage != null)
                            OnGroupMessage(this, msgarray[2], msgarray[3], Utility.GetRestOfMessage(msgarray, 4));
                    }
                }
                else if (msgarray[0] == "JOINED")
                {
                    if (OnUserJoinedGroup != null) OnUserJoinedGroup(this, msgarray[1], msgarray[2]);
                }
                else if (msgarray[0] == "PARTED")
                {
                    if (OnUserPartedGroup != null) OnUserPartedGroup(this, msgarray[1], msgarray[2]);
                }
                else if (msgarray[0] == "KICKED")
                {
                    if (OnUserKickedUser != null)
                        OnUserKickedUser(this, msgarray[2], msgarray[3], msgarray[1],
                                         Utility.GetRestOfMessage(msgarray, 4));
                }
                else if (msgarray[0] == "BANNED")
                {
                    if (OnUserBannedUser != null)
                        OnUserBannedUser(this, msgarray[2], msgarray[3], msgarray[1],
                                         Utility.GetRestOfMessage(msgarray, 4));
                }
                else if (msgarray[0] == "TIME")
                {
                    if (msgarray[1].ToUpper() == "USER")
                    {
                        DateTime time = DateTime.FromBinary(long.Parse(msgarray[3]));
                        UpdateBuddySignOnTime(msgarray[2], time);
                    }
                }
                else if (msgarray[0] == "STATUS")
                {
                    if (msgarray[2].ToUpper() == "ONLINE")
                    {
                        UpdateBuddyStatus(msgarray[1], BuddyStatus.Online);
                        if (OnUserStatus != null) OnUserStatus(this, msgarray[1], BuddyStatus.Online);
                    }
                    else if (msgarray[2].ToUpper() == "AWAY")
                    {
                        UpdateBuddyStatus(msgarray[1], BuddyStatus.Away);
                        if (OnUserStatus != null) OnUserStatus(this, msgarray[1], BuddyStatus.Away);
                    }
                    else if (msgarray[2].ToUpper() == "OFFLINE")
                    {
                        UpdateBuddyStatus(msgarray[1], BuddyStatus.Offline);
                        if (OnUserStatus != null) OnUserStatus(this, msgarray[1], BuddyStatus.Offline);
                    }
                }
                else if (msgarray[0].ToUpper() == "BUDDYLIST")
                {
                    string[] buddys = msgarray[1].Split(';');

                    foreach (string x in buddys)
                    {
                        if (x != string.Empty)
                        {
                            _buddyList.Add(new BuddyInfo(x));
                            GetBuddy(x);
                        }
                    }

                    Thread.Sleep(50*buddys.Length);
                    if (OnBuddyListReceived != null) OnBuddyListReceived(this);
                }
                else
                {
                    if (OnServerMessage != null) OnServerMessage(this, msg);
                }
            }
        }

        public void RawMessage(string message)
        {
            _sw.WriteLine(message);
            _sw.Flush();
        }

        public bool Register(string user, string pass)
        {
            _sw.WriteLine("REGISTER {0} {1}", user, pass);
            _sw.Flush();

            string resp = _sr.ReadLine();

            if (resp.EndsWith("FAIL"))
            {
                return false;
            }
            else
            {
                _username = user;
                _password = pass;

                new Thread(ReaderThread).Start();

                return true;
            }
        }

        public bool Register(string user, byte[] passhash)
        {
            string pass = Encoding.ASCII.GetString(passhash);

            _sw.WriteLine("REGISTER {0} {1}", user, pass);
            _sw.Flush();

            string resp = _sr.ReadLine();

            if (resp.EndsWith("FAIL"))
            {
                return false;
            }
            else
            {
                _username = user;
                _password = pass;

                new Thread(ReaderThread).Start();

                return true;
            }
        }

        public void Connect(string host, int port)
        {
            _tcp = new TcpClient(host, port);

            _ns = _tcp.GetStream();
            _ssl = new SslStream(_ns);

            _ssl.AuthenticateAsClient(host);

            if (_ssl.IsAuthenticated)
            {
                _sw = new StreamWriter(_ssl);
                _sr = new StreamReader(_ssl);
                _isConnected = true;
            }
        }

        public bool Login(string user, byte[] passhash)
        {
            string pass = Encoding.ASCII.GetString(passhash);

            _sw.WriteLine("LOGIN {0} {1}", user, pass);
            _sw.Flush();

            string resp = _sr.ReadLine();

            if (!resp.StartsWith("405"))
            {
                _username = user;
                _password = pass;

                new Thread(ReaderThread).Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Login(string user, string pass)
        {
            _sw.WriteLine("LOGIN {0} {1}", user, pass);
            _sw.Flush();

            string resp = _sr.ReadLine();

            if (!resp.StartsWith("405"))
            {
                _username = user;
                _password = pass;

                new Thread(ReaderThread).Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveData()
        {
            _sw.WriteLine("SAVE");
            _sw.Flush();
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _sw.WriteLine("QUIT");
                _sw.Flush();

                _sr.Close();
                _sw.Close();
                _ns.Close();
                _tcp.Close();

                _isConnected = false;
            }
        }
    }
}
