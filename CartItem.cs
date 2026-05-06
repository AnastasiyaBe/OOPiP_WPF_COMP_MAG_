//товар в корзине(строка в корзине)
namespace ComputerLibrary
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        //общая стоимость всех единиц этого товара, добавленных в корзину
        public decimal Total => Product.Price * Quantity;
    }
}