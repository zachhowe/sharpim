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
 * Date: 5/17/2005
 * Time: 3:58 PM
 */

using System;

namespace SharpIM.Server.Events
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