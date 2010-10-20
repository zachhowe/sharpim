using System.Collections;

namespace SharpIM.Server.Config
{
    internal class ConfigDictionary : DictionaryBase
    {
        public object GetValue(string name)
        {
            return base.Dictionary[name];
        }

        public void PutValue(string key, object val)
        {
            if (base.Dictionary.Contains(key))
            {
                base.Dictionary[key] = val;
            }
            else
            {
                base.Dictionary.Add(key, val);
            }
        }
    }
}