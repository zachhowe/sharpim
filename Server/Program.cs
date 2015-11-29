using System;
using System.Threading;
using SharpIMServer.Config;
using SharpIMServer.Core;

namespace SharpIMServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var conf = new Configuration("SharpIM", "SharpIM.Server.Config.xml");
            if (!conf.IsValid())
            {
                conf.LoadDefaults();
            }
            ConfigAccessor.Initialize(conf);
            
            var server = new Server(ConfigAccessor.Port);
            server.StartServer();

            Console.WriteLine("SharpIM Server initialized on TCP Port {0:d}", ConfigAccessor.Port);
            Console.WriteLine("Waiting for connections...");
            Console.WriteLine();
            
            while (true)
            {
                Thread.Sleep(1024);
            }
        }
    }
}
