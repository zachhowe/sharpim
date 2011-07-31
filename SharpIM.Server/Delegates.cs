/* SharpIM - The Better .NET Instant Messenger
 * Copyright (C) 2007-2010  Zachary Howe
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

using SharpIM.Server.Core;
using SharpIM.Server.Events;

namespace SharpIM.Server
{
    // User delegates
    public delegate void UserGroupMessage(object sender, GroupMessageEventArgs e);

    public delegate void UserAuthenticated(object sender);

    public delegate void UserStatusRequest(object sender, string user);

    public delegate void UserSignOnTimeRequest(object sender, string from);

    public delegate void UserQuit(object sender);

    public delegate void UserStatus(object sender, UserStatusEventArgs e);

    public delegate void UserMessage(object sender, UserMessageEventArgs e);

    public delegate void UserJoinedGroup(string group, string user);

    public delegate void UserPartedGroup(string group, string user);

    public delegate void AdminKickedUser(string to, string from, string group);

    public delegate void AdminBannedUser(string to, string from, string group);

    // Group delegates
    public delegate void GroupSendMessage(object sender, GroupSendMessageEventArgs e);

    public delegate void GroupSendUserJoined(string to, string user, string group);

    public delegate void GroupSendUserParted(string to, string user, string group);

    public delegate void GroupSendUserKicked(string to, string target, string from, string group, string reason);

    public delegate void GroupSendUserBanned(string to, string target, string from, string group, string reason);

    public delegate void GroupUserEventError(string to, string errormessage);

    public delegate void GroupSendGroupProperty(string to, string whoset, GroupProperty prop);
}