using MySql.Data.MySqlClient;

namespace ComputerLibrary
{
    public static class DbConnectionFactory
    {
        private const string ConnectionString =
            "Server=localhost;Database=computer_store;Uid=shop_user;Pwd=user123;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}