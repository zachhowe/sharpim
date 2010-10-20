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

using System;
using System.Security.Cryptography;
using System.Text;

namespace SharpIM.Server
{
    public class Utility
    {
        public static string HashString(string str)
        {
            string strResult = String.Empty;
            string strHashData;

            var sha1 = new SHA1CryptoServiceProvider();

            try
            {
                byte[] arrbytHashValue = sha1.ComputeHash(Encoding.ASCII.GetBytes(str));

                strHashData = BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return (strResult.ToLower());
        }

        public static string GetRestOfMessage(string[] msg, int start)
        {
            return string.Join(" ", msg, start, msg.Length - start);
        }

        public static long UnixTime(DateTime dt)
        {
            var unixEpoch = new DateTime(2005, 1, 1, 0, 0, 0);
            return (dt.Ticks - unixEpoch.Ticks)/10000000;
        }
    }
}