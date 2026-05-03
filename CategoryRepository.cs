using System.Collections.Generic;
using ComputerLibrary.Interfaces;
using ComputerLibrary.Models;
using MySql.Data.MySqlClient;

namespace ComputerLibrary.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            using (var conn = DbConnectionFactory.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, name FROM categories";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name")
                        });
                    }
                }
            }
            return categories;
        }
    }
}