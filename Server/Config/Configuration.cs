using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpIMServer.Config
{
    public class Configuration
    {
        private readonly string m_appName;
        private readonly Dictionary<string, object> m_settings;
        private readonly string m_settingsFile;
        private bool m_autoSave;

        public Configuration(string appName, string settingsFileName)
        {
            m_appName = appName;
            m_autoSave = false;
            m_settings = new Dictionary<string, object>();
            m_settingsFile = settingsFileName;

            Load();
        }

        public object this[string name]
        {
            get
            { 
                return m_settings[name];
            }
            set
            {
                m_settings[name] = value;
                if (m_autoSave) Save();
            }
        }

        public bool IsValid()
        {
            return m_settings.ContainsKey("Port");
        }

        public void LoadDefaults()
        {
            m_settings["Port"] = 4103;
        }
        
        private void Load()
        {
        }
        
        private void Save()
        {
        }
    }
}
