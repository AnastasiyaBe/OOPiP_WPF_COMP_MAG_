using System.Windows;
using ComputerLibrary.Repositories;

namespace ComputerStoreWPF
{
    public partial class CatalogWindow : Window
    {
        public CatalogWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var repo = new ProductRepository();
            var products = repo.GetAllProducts();
            ProductsItemsControl.ItemsSource = products;
        }
    }
}