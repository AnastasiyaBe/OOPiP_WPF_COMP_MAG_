using System.Collections.Generic;
using ComputerLibrary.Models;

namespace ComputerLibrary.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        List<Manufacturer> GetAllManufacturers();
    }
}