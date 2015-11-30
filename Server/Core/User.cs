using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SharpIMServer.Events;
using SharpIMServer.Data;

namespace SharpIMServer.Core
{
    public class User
    {
        #region User Information

        public ArrayList Buddies
        {
            get;
            set;
        }

        public DateTime SignOnTime
        {
            get;
            set;
        }

        public BuddyStatus Status
        {
            get;
            set;
        }
        
        public string Username
        {
            get;
            set;
        }
        
        public bool IsAdmin
        {
            get;
            set;
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
        public event AdminKickedUser OnUserKickedUser;
        public event AdminBannedUser OnUserBannedUser;
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

            this.IsAdmin = false;

            // Initiate buddy list array
            this.Buddies = new ArrayList();

            endPoint = socket.RemoteEndPoint;

            // Initiate streams
            ns = new NetworkStream(socket);
            sw = new StreamWriter(ns);
            sr = new StreamReader(ns);
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

        private bool AuthenticateUser()
        {
            try
            {
                string[] loginarray = sr.ReadLine().Split(' ');

                if (loginarray.Length > 1)
                {
                    if (loginarray[0] == "LOGIN")
                    {
                        string user = loginarray[1];

                        if (Authentication.CheckUser(user))
                        {
                            sw.WriteLine("LOGIN SUCCESS");
                            sw.Flush();

                            Console.WriteLine("{0} has just logged on!", user);

                            this.Username = user;
                            
                            return true;
                        }
                        else
                        {
                            sw.WriteLine("LOGIN FAIL");
                            sw.Flush();

                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
            }

            return false;
        }

        public void Talk()
        {
            if (!ns.DataAvailable) return;

            try
            {
                string[] msgarray = sr.ReadLine().Split(' ');

                if (msgarray[0].ToUpper() == "QUIT")
                {
                    OnUserStatus(this, new UserStatusEventArgs(this.Username, BuddyStatus.Offline));
                    OnUserQuit(this);
                }
                else if (msgarray[0].ToUpper() == "SAVE")
                {
                    Save();
                }
                else if (msgarray[0].ToUpper() == "ADD")
                {
                    if (msgarray[1].ToUpper() == "BUDDY")
                    {
                        this.Buddies.Add(msgarray[2]);
                    }
                }
                else if (msgarray[0].ToUpper() == "REMOVE")
                {
                    if (msgarray[1].ToUpper() == "BUDDY")
                    {
                        this.Buddies.Remove(msgarray[2]);
                    }
                }
                else if (msgarray[0].ToUpper() == "KILL" && msgarray.Length > 2)
                {
                    if (this.IsAdmin)
                    {
                        sw.WriteLine("ERROR KILL Function working yet.");
                        sw.Flush();
                    }
                    else
                    {
                        sw.WriteLine("ERROR KILL Function not permitted.");
                        sw.Flush();
                    }
                }
                else if (msgarray[0].ToUpper() == "MSG")
                {
                    if (!msgarray[1].ToUpper().StartsWith("#"))
                    {
                        OnUserMessage(this,
                                      new UserMessageEventArgs(this.Username, msgarray[1],
                                                               Utility.GetRestOfMessage(msgarray, 2)));
                    }
                    else
                    {
                        OnUserGroupMessage(this,
                                           new GroupMessageEventArgs(this.Username, msgarray[1],
                                                                     Utility.GetRestOfMessage(msgarray, 2)));
                    }
                }
                else if (msgarray[0].ToUpper() == "JOIN")
                {
                    OnUserJoinedGroup(msgarray[1], this.Username);
                }
                else if (msgarray[0].ToUpper() == "PART")
                {
                    OnUserPartedGroup(msgarray[1], this.Username);
                }
                else if (msgarray[1].ToUpper() == "KICK")
                {
                    OnUserKickedUser(msgarray[3], this.Username, msgarray[2]);
                }
                else if (msgarray[1].ToUpper() == "BAN")
                {
                    OnUserBannedUser(msgarray[3], this.Username, msgarray[2]);
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

                        foreach (string x in this.Buddies)
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
                            this.Status = BuddyStatus.Away;
                            OnUserStatus(this, new UserStatusEventArgs(this.Username, BuddyStatus.Away));
                        }
                        else if (msgarray[2].ToUpper() == "BACK")
                        {
                            this.Status = BuddyStatus.Online;
                            OnUserStatus(this, new UserStatusEventArgs(this.Username, BuddyStatus.Online));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Authenticate()
        {
            sw.WriteLine("LOGIN {0:s}", "READY");
            sw.Flush();

            if (AuthenticateUser())
            {
                if (OnAuthenticated != null) OnAuthenticated(this);

                Load();

                if (OnUserStatus != null) OnUserStatus(this, new UserStatusEventArgs(this.Username, BuddyStatus.Online));

                this.SignOnTime = DateTime.Now;
            }
            else
            {
                socket.Shutdown(SocketShutdown.Both);
                sw = null;
                sr = null;
            }
        }

        public void Save()
        {
            DataAccessor.UpdateUser(this);
        }

        public void Load()
        {
        }
    }
}
