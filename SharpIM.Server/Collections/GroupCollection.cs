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

/*
 * Created by SharpDevelop.
 * User: Zachary Howe
 * Date: 5/16/2005
 * Time: 9:49 PM
 */

using System.Collections;
using SharpIM.Server.Core;

namespace SharpIM.Server.Collections
{
    public class GroupCollection : CollectionBase
    {
        public void Add(object val)
        {
            lock (List)
            {
                List.Add(val);
            }
        }

        public void Remove(object val)
        {
            lock (List)
            {
                List.Add(val);
            }
        }

        public bool Contains(string name)
        {
            foreach (Group group in List)
            {
                if (group.GroupName == name)
                {
                    return true;
                }
                else continue;
            }

            return false;
        }
    }
}