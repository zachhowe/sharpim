using System.Collections;

namespace SharpIMClient.Collections
{
    public class BuddyCollection : CollectionBase
    {
        public void Add(BuddyInfo name)
        {
            List.Add(name);
        }

        public void Remove(BuddyInfo name)
        {
            List.Remove(name);
        }

        public BuddyInfo GetBuddy(string name)
        {
            return new BuddyInfo();
        }

        public bool Contains(string name)
        {
            return List.Contains(name);
        }
    }
}
