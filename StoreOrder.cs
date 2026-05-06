//модель заказа
using System;

namespace ComputerLibrary
{
    public class StoreOrder
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Status { get; set; }
    }
}