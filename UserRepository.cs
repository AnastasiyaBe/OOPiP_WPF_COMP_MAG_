using MySql.Data.MySqlClient;
using ComputerLibrary.Models;

namespace ComputerLibrary.Repositories
{
    public class UserRepository
    {
        // Аутентификация администратора
        public User AuthenticateAdmin(string login, string password)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Login, Password, Role FROM Users WHERE Login = @login AND Password = @password AND Role = 'admin'";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Login = reader.GetString("Login"),
                                Password = reader.GetString("Password"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
            }
            return null;
        }
        public User GetDefaultCustomer()
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Login, Password, Role FROM Users WHERE Role = 'customer' LIMIT 1";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32("Id"),
                            Login = reader.GetString("Login"),
                            Password = reader.GetString("Password"),
                            Role = reader.GetString("Role")
                        };
                    }
                }
            }
            return null;
        }
    }
}