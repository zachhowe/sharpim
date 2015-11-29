using SharpIMServer.Core;

namespace SharpIMServer.Data
{
    public static class DataAccessor
    {
        private static Database m_database;

        public static void Initialize(Database database)
        {
            m_database = database;
        }

        public static void InsertUser(User u)
        {
            m_database.InsertUser(u);
        }

        public static void UpdateUser(User u)
        {
            m_database.UpdateUser(u);
        }
    }
}
