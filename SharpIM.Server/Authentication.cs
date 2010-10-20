namespace SharpIM.Server
{
    public class Authentication
    {
        public static bool RegisterUser(string user, string hash)
        {
/*
			string configpath = "Data/Users/" + user.ToLower() + ".xml";
			
			if (!File.Exists(configpath))
			{
				File.Create(configpath).Close();
				
				XmlConfigSource xml = new XmlConfigSource();
				
				xml.AddConfig("Buddies");
				
				IConfig configSettings = xml.AddConfig("Settings");
				configSettings.Set("Screen Name", user);
				configSettings.Set("Email", "");
				configSettings.Set("Password", hash);
                configSettings.Set("IsAdmin", false.ToString());
                configSettings.Set("IsOperator", false.ToString());
				
				xml.Save(configpath);
			}
*/

            return true;
        }

        public static bool CheckUser(string user, string pass)
        {
/*
			string configpath = "Data/Users/" + user.ToLower() + ".xml";
			
			if (File.Exists(configpath))
			{
				XmlConfigSource xml = new XmlConfigSource(configpath);
				
				IConfig configSettings = xml.Configs["Settings"];
				string md5 = configSettings.GetString("Password", "");
				
				if (pass == md5) return true;
			}*/

            return true;

//			return false;
        }
    }
}