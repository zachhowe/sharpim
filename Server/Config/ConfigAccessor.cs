namespace SharpIMServer.Config
{
    public static class ConfigAccessor
    {
        private static Configuration m_config;

        public static void Initialize(Configuration config)
        {
            m_config = config;
        }
        
        public static int Port
        {
            get
            {
                return (int) m_config["Port"];
            }
        }
    }
}
