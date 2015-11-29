using System.Collections;

namespace SharpIMServer.Collections
{
    public class MembersCollection
    {
        private ArrayList admins = new ArrayList();
        private ArrayList users = new ArrayList();

        public ArrayList Admins
        {
            get { return admins; }
            set { admins = value; }
        }

        public ArrayList Normal
        {
            get { return users; }
            set { users = value; }
        }
    }
}
