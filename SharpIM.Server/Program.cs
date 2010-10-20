/* SharpIM - The Better .NET Instant Messenger
 * Copyright (C) 2005-2010 Zachary Howe
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
using System.IO;
using System.Threading;
using SharpIM.Server.Config;
using SharpIM.Server.Data;

namespace SharpIM.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var conf = new Configuration("SharpIM", "SharpIM.Server.Config.xml");
            if (!conf.Check())
            {
                conf.LoadDefaults();
            }
            else
            {
                ConfigAccessor.Initialize(conf);

                var connString = (string) ConfigAccessor.GetValue("ConnectionString");
                var port = (int) ConfigAccessor.GetValue("Port");

                var db = new Database(connString);
                DataAccessor.Initialize(db);

                var server = new Core.Server(port);
                server.StartServer();

                Console.WriteLine("SharpIM Server initialized on TCP Port {0:d}", port);
                Console.WriteLine("Waiting for connections...");
                Console.WriteLine();

                while (true)
                {
                    Thread.Sleep(1024);
                }
            }
        }
    }
}