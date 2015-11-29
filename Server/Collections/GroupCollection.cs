using System.Collections;
using SharpIMServer.Core;

namespace SharpIMServer.Collections
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
