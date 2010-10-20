namespace SharpIM.Server.Config
{
    public static class ConfigAccessor
    {
        private static Configuration m_config;

        public static void Initialize(Configuration config)
        {
            m_config = config;
        }

        public static object GetValue(string name)
        {
            return m_config[name];
        }

        public static void PutValue(string name, object val)
        {
            m_config[name] = val;
        }

        public static bool IsDefined(string name)
        {
            return (m_config[name] == null);
        }
    }
}