using System;

namespace SharpIMClient
{
    public class Utility
    {
        public static string GetRestOfMessage(string[] msg, int start)
        {
            return string.Join(" ", msg, start, msg.Length - start);
        }
        
        public static long UnixTime(DateTime dt)
        {
            return UnixTime(dt, 2005);
        }
        
        public static long UnixTime(DateTime dt, int startYear)
        {
            var unixEpoch = new DateTime(startYear, 1, 1, 0, 0, 0);
            return (dt.Ticks - unixEpoch.Ticks) / 10000000;
        }
    }
}
