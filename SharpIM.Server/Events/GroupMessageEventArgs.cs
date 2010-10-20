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
 * Date: 5/21/2005
 * Time: 9:15 AM
 */

using System;

namespace SharpIM.Server.Events
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