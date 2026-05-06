//репозиторий для работы с товарами и производителями
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ComputerLibrary
{
    public class ProductRepository : IProductRepository
    {
        //получить все товары без фильтров
        public List<Product> GetAll()
        {
            var products = new List<Product>();
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Name, Characteristics, Price, StockQuantity, ImagePath, CategoryId, ManufacturerId FROM Products";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        products.Add(MapProduct(reader));
                }
            }
            return products;
        }

        //возвращает товары с возможной фильтрацией по производителю и сортировкой по цене
        public List<Product> GetFilteredAndSorted(int? manufacturerId, string sortByPrice)
        {
            var products = new List<Product>();
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Name, Characteristics, Price, StockQuantity, ImagePath, CategoryId, ManufacturerId FROM Products WHERE 1=1";
                if (manufacturerId.HasValue && manufacturerId.Value > 0)
                    sql += " AND ManufacturerId = @manId";
                if (!string.IsNullOrEmpty(sortByPrice) && (sortByPrice == "ASC" || sortByPrice == "DESC"))
                    sql += $" ORDER BY Price {sortByPrice}";

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

        //получить всех производителей
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

        //преобразует текущую строку в объект
        private Product MapProduct(MySqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Characteristics = reader.GetString("Characteristics"),
                Price = reader.GetDecimal("Price"),
                StockQuantity = reader.GetInt32("StockQuantity"),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString("ImagePath"),
                CategoryId = reader.GetInt32("CategoryId"),
                ManufacturerId = reader.GetInt32("ManufacturerId")
            };
        }

        //добавляет параметры для операций вставки или обновления товара
        private void SetProductParameters(MySqlCommand cmd, Product product, bool includeId = false)
        {
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@char", product.Characteristics ?? "");
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
            cmd.Parameters.AddWithValue("@image", product.ImagePath ?? "");
            cmd.Parameters.AddWithValue("@catId", product.CategoryId);
            cmd.Parameters.AddWithValue("@manId", product.ManufacturerId);
            if (includeId)
                cmd.Parameters.AddWithValue("@id", product.Id);
        }

        //добавляет новый товар
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

        //обновляет все поля по Id
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

        //удаляет товар по Id
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