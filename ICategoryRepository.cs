using System.Collections.Generic;
using ComputerLibrary.Models;

namespace ComputerLibrary.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategories();
    }
}