/* SharpIM - The Better .NET Instant Messenger
 * Copyright (C) 2007  Zachary Howe
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System.Collections;
using SharpIM.Server.Collections;
using SharpIM.Server.Events;

namespace SharpIM.Server.Core
{
    public class Group
    {
        private string groupname;
        private MembersCollection members;

        public Group()
        {
            members = new MembersCollection();
            GroupProperties = new ArrayList();
        }

        public ArrayList GroupProperties { get; set; }

        public MembersCollection Members
        {
            get { return members; }
            set { members = value; }
        }

        public byte[] Hash { get; set; }

        public string GroupName
        {
            get { return groupname; }
            set { groupname = value; }
        }

        public event GroupSendMessage OnGroupSendMessage;
        public event GroupSendUserJoined OnGroupSendUserJoined;
        public event GroupSendUserParted OnGroupSendUserParted;
        public event GroupSendUserKicked OnGroupSendUserKicked;
        public event GroupSendUserBanned OnGroupSendUserBanned;

        public void AddAdminUser(string user)
        {
            if (!members.Admins.Contains(user))
            {
                members.Admins.Add(user);
            }
        }

        public void AddNormalUser(string user)
        {
            if (!members.Normal.Contains(user))
            {
                members.Normal.Add(user);
            }
        }

        public void PromoteUser(string from, string user)
        {
            RemoveNormalUser(user);
            AddAdminUser(user);
        }

        public void DemoteUser(string from, string user)
        {
            RemoveAdminUser(user);
            AddNormalUser(user);
        }

        public void RemoveAdminUser(string user)
        {
            if (members.Admins.Contains(user))
            {
                members.Admins.Remove(user);
            }
        }

        public void RemoveNormalUser(string user)
        {
            if (members.Normal.Contains(user))
            {
                members.Normal.Remove(user);
            }
        }

        public void SendUserMessage(string user, string message)
        {
            if (Members.Admins.Contains(user) || Members.Normal.Contains(user))
            {
                foreach (string usr in members.Admins)
                {
                    if (user != usr)
                    {
                        if (OnGroupSendMessage != null)
                            OnGroupSendMessage(this, new GroupSendMessageEventArgs(groupname, user, usr, message));
                    }
                }

                foreach (string usr in members.Normal)
                {
                    if (user != usr)
                    {
                        if (OnGroupSendMessage != null)
                            OnGroupSendMessage(this, new GroupSendMessageEventArgs(groupname, user, usr, message));
                    }
                }
            }
        }

        public void SendUserJoined(string user)
        {
            foreach (string usr in members.Admins)
            {
                if (OnGroupSendUserJoined != null) OnGroupSendUserJoined(usr, user, groupname);
            }

            foreach (string usr in members.Normal)
            {
                if (OnGroupSendUserJoined != null) OnGroupSendUserJoined(usr, user, groupname);
            }
        }

        public void SendUserParted(string user)
        {
            foreach (string usr in members.Admins)
            {
                if (OnGroupSendUserParted != null) OnGroupSendUserParted(usr, user, groupname);
            }

            foreach (string usr in members.Normal)
            {
                if (OnGroupSendUserParted != null) OnGroupSendUserParted(usr, user, groupname);
            }
        }

        public void SendUserKicked(string from, string to, string why)
        {
            foreach (string usr in members.Admins)
            {
                if (OnGroupSendUserKicked != null) OnGroupSendUserKicked(usr, to, from, groupname, why);
            }

            foreach (string usr in members.Normal)
            {
                if (OnGroupSendUserKicked != null) OnGroupSendUserKicked(usr, to, from, groupname, why);
            }
        }

        public void SendUserBanned(string from, string to, string why)
        {
            foreach (string usr in members.Admins)
            {
                if (OnGroupSendUserBanned != null) OnGroupSendUserBanned(usr, to, from, groupname, why);
            }

            foreach (string usr in members.Normal)
            {
                if (OnGroupSendUserBanned != null) OnGroupSendUserBanned(usr, to, from, groupname, why);
            }
        }

        public static Group CreateGroup(string groupname)
        {
            var group = new Group();

            group.GroupName = groupname;

            return group;
        }
    }
}