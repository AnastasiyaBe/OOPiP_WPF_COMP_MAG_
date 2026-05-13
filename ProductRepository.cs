using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ComputerLibrary
{
    public class ProductRepository : IProductRepository
    {
        // полный маппинг с Category и Manufacturer
        private Product MapProduct(MySqlDataReader reader)
        {
            var category = new Category
            {
                Id = reader.GetInt32("CategoryId"),
                Name = reader.GetString("CategoryName")
            };
            var manufacturer = new Manufacturer
            {
                Id = reader.GetInt32("ManufacturerId"),
                Name = reader.GetString("ManufacturerName")
            };
            return new Product
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Characteristics = reader.GetString("Characteristics"),
                Price = reader.GetDecimal("Price"),
                StockQuantity = reader.GetInt32("StockQuantity"),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString("ImagePath"),
                Category = category,
                Manufacturer = manufacturer
            };
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT p.Id, p.Name, p.Characteristics, p.Price, p.StockQuantity, p.ImagePath,
                           c.Id AS CategoryId, c.Name AS CategoryName,
                           m.Id AS ManufacturerId, m.Name AS ManufacturerName
                    FROM Products p
                    INNER JOIN Categories c ON p.CategoryId = c.Id
                    INNER JOIN Manufacturers m ON p.ManufacturerId = m.Id";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        products.Add(MapProduct(reader));
                }
            }
            return products;
        }

        public List<Product> GetFilteredAndSorted(int? manufacturerId, string sortByPrice)
        {
            var products = new List<Product>();
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT p.Id, p.Name, p.Characteristics, p.Price, p.StockQuantity, p.ImagePath,
                           c.Id AS CategoryId, c.Name AS CategoryName,
                           m.Id AS ManufacturerId, m.Name AS ManufacturerName
                    FROM Products p
                    INNER JOIN Categories c ON p.CategoryId = c.Id
                    INNER JOIN Manufacturers m ON p.ManufacturerId = m.Id
                    WHERE 1=1";
                if (manufacturerId.HasValue && manufacturerId.Value > 0)
                    sql += " AND m.Id = @manId";
                if (!string.IsNullOrEmpty(sortByPrice) && (sortByPrice == "ASC" || sortByPrice == "DESC"))
                    sql += $" ORDER BY p.Price {sortByPrice}";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (manufacturerId.HasValue && manufacturerId.Value > 0)
                        cmd.Parameters.AddWithValue("@manId", manufacturerId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            products.Add(MapProduct(reader));
                    }
                }
            }
            return products;
        }

        public List<Manufacturer> GetAllManufacturers()
        {
            var manufacturers = new List<Manufacturer>();
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT DISTINCT m.Id, m.Name 
                               FROM Manufacturers m
                               INNER JOIN Products p ON m.Id = p.ManufacturerId
                               ORDER BY m.Name";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        manufacturers.Add(new Manufacturer
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name")
                        });
                    }
                }
            }
            return manufacturers;
        }

        private void SetProductParameters(MySqlCommand cmd, Product product, bool includeId = false)
        {
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@char", product.Characteristics ?? "");
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
            cmd.Parameters.AddWithValue("@image", product.ImagePath ?? "");
            cmd.Parameters.AddWithValue("@catId", product.Category.Id);
            cmd.Parameters.AddWithValue("@manId", product.Manufacturer.Id);
            if (includeId)
                cmd.Parameters.AddWithValue("@id", product.Id);
        }

        public void AddProduct(Product product)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO Products (Name, Characteristics, Price, StockQuantity, ImagePath, CategoryId, ManufacturerId) 
                               VALUES (@name, @char, @price, @stock, @image, @catId, @manId)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    SetProductParameters(cmd, product);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE Products SET Name=@name, Characteristics=@char, Price=@price, 
                               StockQuantity=@stock, ImagePath=@image, CategoryId=@catId, ManufacturerId=@manId 
                               WHERE Id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    SetProductParameters(cmd, product, includeId: true);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Products WHERE Id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}