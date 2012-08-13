using System;
using MySql.Data.MySqlClient;
using SharpIM.Server.Core;

namespace SharpIM.Server.Data
{
    public class Database
    {
        private MySqlConnection _sqlConnection;

        public Database(string connString)
        {
            try
            {
                _sqlConnection = new MySqlConnection(connString);
                _sqlConnection.Open();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("MySQL Exception: {0:s}", e.Message);
            }
        }

        public void InsertUser(User user)
        {
            MySqlCommand cmd = _sqlConnection.CreateCommand();
            cmd.CommandText = string.Format("INSERT INTO `users` (`id`, `username`, `password`, `email`) VALUES (0, '{0:s}', '{1:s}', '{2:s}')", user.Username, user.Password, user.Email);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void UpdateUser(User user)
        {
        }

        public User SelectUser(string name)
        {
            User usr = new User(null);

            return usr;
        }
    }
}
