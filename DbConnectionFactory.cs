//создание подключений к базе данных в зависимости от роли пользователя
using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace ComputerLibrary
{
    public sealed class DbConnectionFactory
    {
        private static DbConnectionFactory _instance;
        private static readonly object _lock = new object();
        private string _connectionString;
        private string _currentRole;

        private DbConnectionFactory() { }

        public static DbConnectionFactory Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DbConnectionFactory();//объект создаётся только при первом обращении
                    return _instance;
                }
            }
        }

        public void InitializeAsUser()
        {
            _currentRole = "user";
            _connectionString = ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString;//читает строку подключения из App.config по ключу "UserConnection"
        }

        public bool InitializeAsAdmin(string enteredPassword)
        {
            const string correctAdminPassword = "admin123";
            if (enteredPassword != correctAdminPassword)
                return false;

            _currentRole = "admin";
            _connectionString = ConfigurationManager.ConnectionStrings["AdminConnection"].ConnectionString;//читает строку подключения "AdminConnection"
            return true;
        }

        public MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Фабрика не инициализирована. Вызовите InitializeAsUser() или InitializeAsAdmin().");
            return new MySqlConnection(_connectionString);
        }

        public string GetCurrentRole() => _currentRole;
    }
}