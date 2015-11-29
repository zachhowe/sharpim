using System.Collections;
using SharpIMServer.Core;

namespace SharpIMServer.Collections
{
    public class UserCollection : CollectionBase
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
                List.Remove(val);
            }
        }

        public bool Contains(string name)
        {
            foreach (User user in List)
            {
                if (user.Username == name)
                {
                    return true;
                }
                else continue;
            }

            return false;
        }

        public BuddyStatus GetBuddyStatus(string name)
        {
            if (!Contains(name))
            {
                return BuddyStatus.Offline;
            }
            else
            {
                User usr = GetUser(name);

                return usr.Status;
            }
        }

        public User GetUser(string username)
        {
            foreach (User user in List)
            {
                if (user.Username == username)
                {
                    return user;
                }
            }

            return null;
        }
    }
}
