using System;
using System.Collections;
using System.Threading;
using SharpIMServer.Collections;

namespace SharpIMServer.Core
{
    public class UserPool : UserCollection
    {
        public void SocketPollingThread()
        {
            while (true)
            {
                lock (List)
                {
                   foreach (User u in List)
                   {
                        u.Talk();
                    }
                }

                Thread.Sleep(128);
            }
        }
    }
}
