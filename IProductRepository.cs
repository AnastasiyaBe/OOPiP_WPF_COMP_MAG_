//какие методы должен реализовать репозиторий для работы с товарами и производителями
using System.Collections.Generic;

namespace ComputerLibrary
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        List<Product> GetFilteredAndSorted(int? manufacturerId, string sortByPrice);
        List<Manufacturer> GetAllManufacturers();
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);
    }
}