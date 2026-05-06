//какие методы должен предоставлять репозиторий, реализация в CategoryRepository
using System.Collections.Generic;

namespace ComputerLibrary
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
    }
}