using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerLibrary
{
    public class OrderRepository : IOrderRepository
    {
        public List<StoreOrder> GetAllOrders()
        //возвращает список всех заказов из таблицы Orders, отсортированных по убыванию Id
        {
            var orders = new List<StoreOrder>();
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, OrderDate, CustomerName, CustomerPhone, Status FROM Orders ORDER BY Id DESC";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //создаёт объект StoreOrder и добавляет в список
                        orders.Add(new StoreOrder
                        {
                            Id = reader.GetInt32("Id"),
                            OrderDate = reader.GetDateTime("OrderDate"),
                            CustomerName = reader.GetString("CustomerName"),
                            CustomerPhone = reader.GetString("CustomerPhone"),
                            Status = reader.GetString("Status")
                        });
                    }
                }
            }
            return orders;
        }

        //изменяет статус конкретного заказа
        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Orders SET Status = @status WHERE Id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //удаляет заказ и все связанные с ним позиции из OrderItems
        public void DeleteOrder(int orderId)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                string deleteItemsSql = "DELETE FROM OrderItems WHERE OrderId = @id";
                using (var cmd = new MySqlCommand(deleteItemsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
                string deleteOrderSql = "DELETE FROM Orders WHERE Id = @id";
                using (var cmd = new MySqlCommand(deleteOrderSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //
        public void CreateOrder(string customerName, string customerPhone, IEnumerable<CartItem> cartItems)
        {
            using (var conn = DbConnectionFactory.Instance.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        //вставка нового заказа
                        string orderSql = @"INSERT INTO Orders (OrderDate, CustomerName, CustomerPhone, Status) 
                                            VALUES (@date, @name, @phone, 'новый'); SELECT LAST_INSERT_ID();";
                        int orderId;
                        using (var cmd = new MySqlCommand(orderSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd.Parameters.AddWithValue("@name", customerName);
                            cmd.Parameters.AddWithValue("@phone", customerPhone);
                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        //вставка в OrderItems с фиксацией цены на момент заказа
                        foreach (var item in cartItems)
                        {
                            decimal price = item.Product.Price;
                            string itemSql = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtMoment) VALUES (@oid, @pid, @qty, @price)";
                            using (var cmd = new MySqlCommand(itemSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@oid", orderId);
                                cmd.Parameters.AddWithValue("@pid", item.Product.Id);
                                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@price", price);
                                cmd.ExecuteNonQuery();
                            }

                            //уменьшение остатка на складе
                            string updateStockSql = "UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE Id = @pid";
                            using (var cmd = new MySqlCommand(updateStockSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@pid", item.Product.Id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}