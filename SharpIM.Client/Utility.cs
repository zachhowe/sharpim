using System;
using System.Security.Cryptography;
using System.Text;

namespace SharpIM.Client
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