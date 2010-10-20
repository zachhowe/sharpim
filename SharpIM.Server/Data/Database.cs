using MySql.Data.MySqlClient;
using SharpIM.Server.Core;

namespace SharpIM.Server.Data
{
    public class Database
    {
        private readonly MySqlConnection sqlConnection;

        public Database(string connString)
        {
            sqlConnection = new MySqlConnection(connString);
            sqlConnection.Open();
        }

        public User GetUserInfo(string name)
        {
            var usr = new User(null);

            return null;
        }
    }
}