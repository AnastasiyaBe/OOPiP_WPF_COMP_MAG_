using System.Collections.Generic;
using ComputerLibrary.Interfaces;
using ComputerLibrary.Models;
using MySql.Data.MySqlClient;

namespace ComputerLibrary.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (var conn = DbConnectionFactory.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, name, characteristics, price, stock_quantity, image_path, category_id, manufacturer_id FROM products";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Characteristics = reader.IsDBNull(reader.GetOrdinal("characteristics"))
                                ? null : reader.GetString("characteristics"),
                            Price = reader.GetDecimal("price"),
                            StockQuantity = reader.GetInt32("stock_quantity"),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path"))
                                ? null : reader.GetString("image_path"),
                            CategoryId = reader.GetInt32("category_id"),
                            ManufacturerId = reader.GetInt32("manufacturer_id")
                        });
                    }
                }
            }
            return products;
        }

        public List<Manufacturer> GetAllManufacturers()
        {
            var manufacturers = new List<Manufacturer>();
            using (var conn = DbConnectionFactory.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, name FROM manufacturers";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        manufacturers.Add(new Manufacturer
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name")
                        });
                    }
                }
            }
            return manufacturers;
        }
    }
}