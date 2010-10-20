/* TakIM - The Better .NET Instant Messenger
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

/*
 * Created by SharpDevelop.
 * User: Zachary Howe
 * Date: 5/7/2005
 * Time: 9:03 AM
 */

namespace SharpIM.Client
{
    public delegate void UserJoinedGroupEventHandler(object sender, string group, string user);

    public delegate void UserPartedGroupEventHandler(object sender, string group, string user);

    public delegate void UserKickedUserEventHandler(object sender, string to, string from, string group, string reason);

    public delegate void UserBannedUserEventHandler(object sender, string to, string from, string group, string reason);

    public delegate void GroupMessageEventHandler(object sender, string group, string user, string message);

    public delegate void UserStatusEventHandler(object sender, string user, BuddyStatus status);

    public delegate void UserMessageEventHandler(object sender, string user, string message);

    public delegate void ServerMessageEventHandler(object sender, string message);

    public delegate void BuddyListReceivedEventHandler(object sender);
}