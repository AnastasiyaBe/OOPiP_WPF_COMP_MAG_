//доступ к данным таблицы категорий в бд
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ComputerLibrary
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAll()
        {
            var list = new List<Category>();
            using (var conn = DbConnectionFactory.Instance.GetConnection()) //получает соединение с бд от фабрики
            {
                conn.Open();
                string sql = "SELECT Id, Name FROM Categories ORDER BY Name"; //получает ID и Name, сортирует по имени
                using (var cmd = new MySqlCommand(sql, conn))//команда для выполнения запроса
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Category { Id = reader.GetInt32("Id"), Name = reader.GetString("Name") });//заполняются свойства объекта Category Id и Name, он добавляется в список
                    }
                }
            }
            return list;
        }
    }
}