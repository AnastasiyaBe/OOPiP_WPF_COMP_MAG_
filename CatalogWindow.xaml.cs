using ComputerLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ComputerStoreWPF
{
    public partial class CatalogWindow : Window
    {
        private ObservableCollection<CartItem> _cartItems = new ObservableCollection<CartItem>();

        public CatalogWindow()
        {
            InitializeComponent();
            LoadManufacturers();
            LoadProducts();
        }

        private void LoadManufacturers()
        {
            var repo = new ProductRepository();
            var manufacturers = repo.GetAllManufacturers();
            manufacturers.Insert(0, new Manufacturer { Id = 0, Name = "Все производители" });
            ManufacturerCombo.ItemsSource = manufacturers;
            ManufacturerCombo.DisplayMemberPath = "Name";
            ManufacturerCombo.SelectedValuePath = "Id";
            ManufacturerCombo.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            int? manufacturerId = null;
            var selected = ManufacturerCombo.SelectedItem as Manufacturer;
            if (selected != null && selected.Id != 0)
                manufacturerId = selected.Id;   

            string sortOrder = null;
            if (PriceAscRadio.IsChecked == true) sortOrder = "ASC";
            else if (PriceDescRadio.IsChecked == true) sortOrder = "DESC";

            var repo = new ProductRepository();
            List<Product> products = repo.GetFilteredAndSorted(manufacturerId, sortOrder);
            ProductsItemsControl.ItemsSource = products;
        }

        private void ManufacturerCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadProducts();
        }

        private void Sorting_Changed(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.Tag as Product;
            if (product == null) return;

            // Проверка: если товара нет на складе, нельзя добавить
            if (product.StockQuantity <= 0)
            {
                MessageBox.Show($"Товар \"{product.Name}\" закончился на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = _cartItems.FirstOrDefault(ci => ci.Product.Id == product.Id);
            int currentQty = existing?.Quantity ?? 0;
            int newQty = currentQty + 1;

            // Проверяем, не превышает ли новое количество остаток
            if (newQty > product.StockQuantity)
            {
                MessageBox.Show($"Нельзя добавить больше, чем есть на складе (доступно: {product.StockQuantity} шт.)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (existing != null)
                existing.Quantity++;
            else
                _cartItems.Add(new CartItem { Product = product, Quantity = 1 });

            int totalUnits = _cartItems.Sum(ci => ci.Quantity);
            MessageBox.Show($"Товар \"{product.Name}\" добавлен. Всего в корзине: {totalUnits} шт.", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GoToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var cartWindow = new CartWindow(_cartItems);
            if (cartWindow.ShowDialog() == true)
            {
                LoadProducts(); 
            }
        }

        private void ChangeRoleButton_Click(object sender, RoutedEventArgs e)
        {
            var roleWindow = new RoleSelectionWindow();
            roleWindow.Show();
            this.Close();
        }
    }
}