using System;
using System.Collections;
using System.Threading;

using SharpIM.Server.Collections;

namespace SharpIM.Server.Core
{
    public class UserPool : UserCollection
    {
        public static UserPool OpenPool()
        {
            return null;
        }

        public static UserPool MergePools(UserPool p1, UserPool p2)
        {
            return null;
        }

        public void SocketPoolingThread()
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