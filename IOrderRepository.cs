//какие методы должен реализовать репозиторий для работы с заказами
using System.Collections.Generic;

namespace ComputerLibrary
{
    public interface IOrderRepository
    {
        List<StoreOrder> GetAllOrders();
        void UpdateOrderStatus(int orderId, string newStatus);
        void DeleteOrder(int orderId);
        void CreateOrder(string customerName, string customerPhone, IEnumerable<CartItem> cartItems);
    }
}