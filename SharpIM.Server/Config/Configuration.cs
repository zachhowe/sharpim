using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace SharpIM.Server.Config
{
    public class Configuration
    {
        private readonly string m_appName;
        private readonly ConfigDictionary m_settings;
        private readonly string m_settingsFile;
        private bool m_autoSave;

        public Configuration(string appName, string settingsFileName)
        {
            m_appName = appName;
            m_settings = new ConfigDictionary();
            m_settingsFile = settingsFileName;

            if (!File.Exists(settingsFileName))
            {
                LoadDefaults();
                Save();
            }
            else
            {
                Load();
            }
        }

        public object this[string name]
        {
            get { return m_settings.GetValue(name); }
            set
            {
                m_settings.PutValue(name, value);
                if (m_autoSave) Save();
            }
        }

        public bool AutoSave
        {
            get { return m_autoSave; }
            set { m_autoSave = value; }
        }

        public bool Check()
        {
            return true;
        }

        public void LoadDefaults()
        {
            m_settings.PutValue("ConnectionString",
                                "Server=localhost;Port=3306;User Id=sharpim;Password=sharpim;Database=sharpim");
            m_settings.PutValue("Port", 4103);
        }

        public void Load()
        {
            try
            {
                var x = new XmlTextReader(m_settingsFile);
                var xdoc = new XmlDocument();
                xdoc.Load(x);

                // Now load the actual settings:
                foreach (XmlNode node in xdoc.GetElementsByTagName("Setting"))
                {
                    string node_name = node.Attributes["Name"].Value;
                    string node_type = node.Attributes["Type"].Value;
                    object node_value = node.Attributes["Value"].Value;

                    switch (node_type)
                    {
                        case "System.Int32":
                            node_value = int.Parse((string) node_value);
                            break;
                        case "System.String":
                            break;
                        default:
                            continue;
                    }

                    m_settings.PutValue(node_name, node_value);
                }

                x.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Save()
        {
            Save(m_settingsFile);
        }

        public void Save(string settingsFile)
        {
            try
            {
                var xsets = new XmlWriterSettings();
                xsets.Indent = true;
                xsets.IndentChars = "  ";
                xsets.Encoding = Encoding.UTF8;

                XmlWriter xw = XmlWriter.Create(settingsFile, xsets);

                xw.WriteStartDocument();
                xw.WriteStartElement(m_appName);

                xw.WriteStartElement("Settings");

                IDictionaryEnumerator sdenum = m_settings.GetEnumerator();

                while (sdenum.MoveNext())
                {
                    xw.WriteStartElement("Setting");
                    xw.WriteAttributeString("Name", (string) sdenum.Key);
                    xw.WriteAttributeString("Type", sdenum.Value.GetType().FullName);
                    xw.WriteAttributeString("Value", sdenum.Value.ToString());
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteEndDocument();

                xw.Flush();
                xw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void PrintOutputXml()
        {
            try
            {
                var xsets = new XmlWriterSettings();
                xsets.Indent = true;
                xsets.IndentChars = "  ";
                xsets.Encoding = Encoding.UTF8;

                XmlWriter xw = XmlWriter.Create(Console.Out, xsets);

                xw.WriteStartDocument();
                xw.WriteStartElement(m_appName);

                xw.WriteStartElement("Config");

                IDictionaryEnumerator sdenum = m_settings.GetEnumerator();

                while (sdenum.MoveNext())
                {
                    xw.WriteStartElement("Setting");
                    xw.WriteAttributeString("Name", (string) sdenum.Key);
                    xw.WriteAttributeString("Type", sdenum.Value.GetType().FullName);
                    xw.WriteAttributeString("Value", sdenum.Value.ToString());
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteEndDocument();

                xw.Flush();
                xw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}