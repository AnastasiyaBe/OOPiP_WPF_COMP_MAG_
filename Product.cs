namespace ComputerLibrary.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Characteristics { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
    }
}