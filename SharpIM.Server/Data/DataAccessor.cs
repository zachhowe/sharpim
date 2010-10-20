namespace SharpIM.Server.Data
{
    public static class DataAccessor
    {
        private static Database m_database;

        public static void Initialize(Database database)
        {
            m_database = database;
        }
    }
}