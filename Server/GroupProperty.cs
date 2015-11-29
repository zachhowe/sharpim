namespace SharpIMServer.Core
{
    public class GroupProperty
    {
        private readonly PropertyType propType;
        private object propValue;

        public GroupProperty(PropertyType type, object val)
        {
            propType = type;
            propValue = val;
        }

        public PropertyType PropertyType
        {
            get { return propType; }
        }

        public object PropertyValue
        {
            get { return propValue; }
        }
    }
}
