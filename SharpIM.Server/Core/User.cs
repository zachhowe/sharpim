using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SharpIM.Server.Events;

namespace SharpIM.Server.Core
{
    public class User
    {
        #region User Information

        private readonly ArrayList buddies;
        private string email;
        private bool isAdmin;
        private byte[] password;
        private DateTime signOnTime;
        private BuddyStatus status;
        private string username;

        public ArrayList Buddies
        {
            get { return buddies; }
        }

        public DateTime SignOnTime
        {
            get { return signOnTime; }
            set { signOnTime = value; }
        }

        public BuddyStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public byte[] Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }

        #endregion

        #region Events

        public event UserMessage OnUserMessage;
        public event UserStatus OnUserStatus;
        public event UserAuthenticated OnAuthenticated;
        public event UserQuit OnUserQuit;
        public event UserStatusRequest OnUserStatusRequest;
        public event UserGroupMessage OnUserGroupMessage;
        public event UserJoinedGroup OnUserJoinedGroup;
        public event UserPartedGroup OnUserPartedGroup;
        public event UserKickedUser OnUserKickedUser;
        public event UserBannedUser OnUserBannedUser;
        public event UserSignOnTimeRequest OnUserSignOnTimeRequest;

        #endregion

        #region Fields

        private readonly EndPoint endPoint;
        private readonly NetworkStream ns;
        private readonly Socket socket;
        private StreamReader sr;
        private StreamWriter sw;

        #endregion

        #region Properties

        public IPAddress RemoteIPAddress
        {
            get
            {
                string address = endPoint.ToString();
                return IPAddress.Parse(address.Split(':')[0]);
            }
        }

        public int RemotePort
        {
            get
            {
                string address = endPoint.ToString();
                return Int32.Parse(address.Split(':')[1]);
            }
        }

        public StreamReader StreamReader
        {
            get { return sr; }
            set { sr = value; }
        }


        public StreamWriter StreamWriter
        {
            get { return sw; }
            set { sw = value; }
        }

        #endregion

        public User(Socket sock)
        {
            socket = sock;

            isAdmin = false;

            // Initiate buddy list array
            buddies = new ArrayList();

            endPoint = socket.RemoteEndPoint;

            // Initiate streams
            ns = new NetworkStream(socket);
            sw = new StreamWriter(ns);
            sr = new StreamReader(ns);

            // Start talking thread
            new Thread(TalkerThread).Start();
        }

        public void SendUserSignOnTime(string user, DateTime time)
        {
            sw.WriteLine("TIME USER {0} {1:d}", user, time);
            sw.Flush();
        }

        public void ForceDisconnect(string from, string reason)
        {
            sw.WriteLine("DIE {0} {1}", from, reason);
            sw.Flush();

            OnUserStatus(this, new UserStatusEventArgs(Username, BuddyStatus.Offline));
            OnUserQuit(this);
            Thread.CurrentThread.Abort();
        }

        public void SendStatus(string user, BuddyStatus status)
        {
            sw.WriteLine("STATUS {0} {1}", user, status.ToString().ToUpper());
            sw.Flush();
        }

        public void SendUserMessage(string from, string message)
        {
            sw.WriteLine("MSG USER {0} {1}", from, message);
            sw.Flush();
        }

        public void SendGroupMessage(string group, string from, string message)
        {
            sw.WriteLine("MSG GROUP {0} {1} {2}", group, from, message);
            sw.Flush();
        }

        public void SendGroupUsers(string group, string users)
        {
            sw.WriteLine("NAMES {0} {1}", group, users);
            sw.Flush();
        }

        public void SendUserJoined(string group, string from)
        {
            sw.WriteLine("JOINED {0} {1}", group, from);
            sw.Flush();
        }

        public void SendUserKicked(string group, string from)
        {
            sw.WriteLine("KICKED {0} {1}", group, from);
            sw.Flush();
        }

        public void SendUserBanned(string group, string from)
        {
            sw.WriteLine("BANNED {0} {1}", group, from);
            sw.Flush();
        }

        public void SendUserParted(string group, string from)
        {
            sw.WriteLine("PARTED {0} {1}", group, from);
            sw.Flush();
        }

        private bool Authenticate()
        {
            string[] loginarray = sr.ReadLine().Split(' ');

            if (loginarray.Length > 2)
            {
                if (loginarray[0] == "LOGIN")
                {
                    string user = loginarray[1];
                    string pass = loginarray[2];

                    if (Authentication.CheckUser(user, pass))
                    {
                        sw.WriteLine("LOGIN SUCCESS");
                        sw.Flush();

                        Console.WriteLine("{0} has just logged on!", user);

                        username = user;

                        // TODO: this should not be saved in memory
                        password = Encoding.ASCII.GetBytes(pass);

                        return true;
                    }
                    else
                    {
                        sw.WriteLine("LOGIN FAIL");
                        sw.Flush();

                        Console.WriteLine("{0} has just failed to login!", user);

                        return false;
                    }
                }
                else if (loginarray[0] == "REGISTER")
                {
                    isAdmin = false;

                    string user = loginarray[1];
                    string pass = loginarray[2];

                    if (Authentication.RegisterUser(user, pass))
                    {
                        sw.WriteLine("REGISTER SUCCESS");
                        sw.Flush();

                        Console.WriteLine("{0} has just registered and logged on!", loginarray[1]);

                        username = user;
                        password = Encoding.ASCII.GetBytes(pass);

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("{0} has just failed to register!", loginarray[1]);

                        sw.WriteLine("REGISTER FAIL");
                        sw.Flush();

                        return false;
                    }
                }
            }

            return false;
        }

        private void TalkerThread()
        {
            sw.WriteLine("LOGIN {0:s}", "READY"); // switch READY to DISABLED on-demand
            sw.WriteLine("REGISTER {0:s}", "READY"); // switch READY to DISABLED on-demand
            sw.Flush();

            if (Authenticate())
            {
                if (OnAuthenticated != null) OnAuthenticated(this);

                Load();

                if (OnUserStatus != null) OnUserStatus(this, new UserStatusEventArgs(username, BuddyStatus.Online));

                signOnTime = DateTime.Now;

                while (true)
                {
                    string[] msgarray = sr.ReadLine().Split(' ');

                    if (msgarray[0].ToUpper() == "QUIT")
                    {
                        OnUserStatus(this, new UserStatusEventArgs(username, BuddyStatus.Offline));
                        OnUserQuit(this);
                        Thread.CurrentThread.Abort();
                    }
                    else if (msgarray[0].ToUpper() == "SAVE")
                    {
                        SaveData();
                    }
                    else if (msgarray[0].ToUpper() == "ADD")
                    {
                        if (msgarray[1].ToUpper() == "BUDDY")
                        {
                            buddies.Add(msgarray[2]);
                        }
                    }
                    else if (msgarray[0].ToUpper() == "REMOVE")
                    {
                        if (msgarray[1].ToUpper() == "BUDDY")
                        {
                            buddies.Remove(msgarray[2]);
                        }
                    }
                    else if (msgarray[0].ToUpper() == "KILL" && msgarray.Length > 2)
                    {
                        if (isAdmin)
                        {
                            sw.WriteLine("599 Function working yet.");
                            sw.Flush();
                        }
                        else
                        {
                            sw.WriteLine("990 Function not permitted.");
                            sw.Flush();
                        }
                    }
                    else if (msgarray[0].ToUpper() == "MSG")
                    {
                        if (!msgarray[1].ToUpper().StartsWith("#"))
                        {
                            OnUserMessage(this,
                                          new UserMessageEventArgs(username, msgarray[1],
                                                                   Utility.GetRestOfMessage(msgarray, 2)));
                        }
                        else
                        {
                            OnUserGroupMessage(this,
                                               new GroupMessageEventArgs(username, msgarray[1],
                                                                         Utility.GetRestOfMessage(msgarray, 2)));
                        }
                    }
                    else if (msgarray[0].ToUpper() == "JOIN")
                    {
                        OnUserJoinedGroup(msgarray[1], username);
                    }
                    else if (msgarray[0].ToUpper() == "PART")
                    {
                        OnUserPartedGroup(msgarray[1], username);
                    }
                    else if (msgarray[1].ToUpper() == "KICK")
                    {
                        OnUserKickedUser(msgarray[3], username, msgarray[2]);
                    }
                    else if (msgarray[1].ToUpper() == "BAN")
                    {
                        OnUserBannedUser(msgarray[3], username, msgarray[2]);
                    }
                    else if (msgarray[0].ToUpper() == "GET")
                    {
                        if (msgarray[1].ToUpper() == "STATUS")
                        {
                            OnUserStatusRequest(this, msgarray[2]);
                        }
                        else if (msgarray[1].ToUpper() == "SIGNONTIME")
                        {
                            OnUserSignOnTimeRequest(this, msgarray[2]);
                        }
                        else if (msgarray[1].ToUpper() == "GROUP")
                        {
                        }
                        else if (msgarray[1].ToUpper() == "BUDDYLIST")
                        {
                            string buddys = string.Empty;

                            foreach (string x in Buddies)
                            {
                                buddys += x + ";";
                            }

                            sw.WriteLine("BUDDYLIST {0}", buddys);
                            sw.Flush();
                        }
                    }
                    else if (msgarray[0].ToUpper() == "SET")
                    {
                        if (msgarray[1].ToUpper() == "STATUS")
                        {
                            if (msgarray[2].ToUpper() == "AWAY")
                            {
                                status = BuddyStatus.Away;
                                OnUserStatus(this, new UserStatusEventArgs(username, BuddyStatus.Away));
                            }
                            else if (msgarray[2].ToUpper() == "BACK")
                            {
                                status = BuddyStatus.Online;
                                OnUserStatus(this, new UserStatusEventArgs(username, BuddyStatus.Online));
                            }
                        }
                        else if (msgarray[1].ToUpper() == "EMAIL")
                        {
                            email = msgarray[2];
                        }
                        else if (msgarray[1].ToUpper() == "PASSWD")
                        {
                            password = Encoding.ASCII.GetBytes(msgarray[2]);
                        }
                    }
                }
            }
            else
            {
                sw.Close();
                sr.Close();
                ns.Close();
                socket.Close();
            }
        }

        public void SaveData()
        {
            /*
			string configpath = "Data\\Users\\" + this.username + ".xml";
			
			if (!File.Exists(configpath))
			{
				File.Create(configpath).Close();
				
				XmlConfigSource xml = new XmlConfigSource();
				
				IConfig configBuddies = xml.AddConfig("Buddies");
				foreach (string x in this.buddies)
				{
					configBuddies.Set("Buddy: " + x, x);
				}
				
				IConfig configSettings = xml.AddConfig("Settings");
				if (this.isOperator) configSettings.Set("IsOperator", "True");
                else configSettings.Set("IsOperator", "False");
				if (this.isAdmin)configSettings.Set("IsAdmin", "True");
                else configSettings.Set("IsAdmin", "False");
				if (this.email != null) configSettings.Set("Email", this.email);
                if (this.hash != null) configSettings.Set("Password", ASCIIEncoding.ASCII.GetString(this.hash));
				if (this.homepage != null) configSettings.Set("Homepage", this.hash);
				
				xml.Save(configpath);
			}
			else
			{
				XmlConfigSource xml = new XmlConfigSource(configpath);
				
				IConfig configBuddies = xml.Configs["Buddies"];
				
				foreach (string x in this.buddies)
				{
					configBuddies.Set("Buddy: " + x, x);
				}
				
				IConfig configSettings = xml.Configs["Settings"];
                if (this.isOperator) configSettings.Set("IsOperator", "True");
                else configSettings.Set("IsOperator", "False");
                if (this.isAdmin) configSettings.Set("IsAdmin", "True");
                else configSettings.Set("IsAdmin", "False");
				if (this.email != null) configSettings.Set("Email", this.email);
				if (this.hash != null) configSettings.Set("Password", ASCIIEncoding.ASCII.GetString(this.hash));
				if (this.homepage != null) configSettings.Set("Homepage", this.hash);
				
				xml.Save();
			} */
        }

        private void Load()
        {
/*
			string configpath = "Data\\Users\\" + this.username + ".xml";
			
			XmlConfigSource xml = new XmlConfigSource(configpath);
				
			IConfig configBuddies = xml.Configs["Buddies"];
			foreach (string x in configBuddies.GetValues())
			{
				this.buddies.Add(x);
			}
			
			IConfig configSettings = xml.Configs["Settings"];

            if (configSettings.Get("IsOperator") == "True") this.isOperator = true;
            else this.isOperator = false;
            if (configSettings.Get("IsAdmin") == "True") this.isOperator = true;
            else this.isAdmin = false;
			this.email = configSettings.Get("Email");
			this.hash = ASCIIEncoding.ASCII.GetBytes(configSettings.Get("Password"));
			this.homepage = configSettings.Get("Homepage");*/
        }
    }
}