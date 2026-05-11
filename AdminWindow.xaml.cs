using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ComputerLibrary;

namespace ComputerStoreWPF
{
    public partial class AdminWindow : Window
    {
        public class ProductRow
        {
            public Product Product { get; set; }
            public int RowNumber { get; set; }
            public int Id => Product.Id;
            public string Name { get => Product.Name; set => Product.Name = value; }
            public string Characteristics { get => Product.Characteristics; set => Product.Characteristics = value; }
            public decimal Price { get => Product.Price; set => Product.Price = value; }
            public int StockQuantity { get => Product.StockQuantity; set => Product.StockQuantity = value; }
            public string ImagePath { get => Product.ImagePath; set => Product.ImagePath = value; }
            public int CategoryId { get => Product.CategoryId; set => Product.CategoryId = value; }
            public int ManufacturerId { get => Product.ManufacturerId; set => Product.ManufacturerId = value; }
        }

        public class OrderRow
        {
            public StoreOrder Order { get; set; }
            public int RowNumber { get; set; }
            public int Id => Order.Id;
            public string CustomerName => Order.CustomerName;
            public string CustomerPhone => Order.CustomerPhone;
            public System.DateTime OrderDate => Order.OrderDate;
            public string Status { get => Order.Status; set => Order.Status = value; }
        }

        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ICategoryRepository _categoryRepo;

        public ObservableCollection<ProductRow> ProductRows { get; set; }
        public ObservableCollection<OrderRow> OrderRows { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Manufacturer> Manufacturers { get; set; }
        public List<string> OrderStatuses { get; set; }

        public AdminWindow(IProductRepository productRepo, IOrderRepository orderRepo, ICategoryRepository categoryRepo)
        {
            InitializeComponent();
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _categoryRepo = categoryRepo;
            DataContext = this;

            OrderStatuses = new List<string> { "новый", "обработан", "доставлен" };

            LoadCategoriesAndManufacturers();
            LoadProducts();
            LoadOrders();
        }

        private void LoadCategoriesAndManufacturers()
        {
            Categories = new ObservableCollection<Category>(_categoryRepo.GetAll());
            Manufacturers = new ObservableCollection<Manufacturer>(_productRepo.GetAllManufacturers());
        }

        private void LoadProducts()
        {
            var products = _productRepo.GetAll();
            int number = 1;
            var rows = new ObservableCollection<ProductRow>();
            foreach (var p in products)
                rows.Add(new ProductRow { Product = p, RowNumber = number++ });
            ProductRows = rows;
            ProductsGrid.ItemsSource = ProductRows;
        }

        private void LoadOrders()
        {
            var orders = _orderRepo.GetAllOrders();
            var sorted = orders.OrderBy(o => o.Id).ToList(); 
            int number = 1;
            var rows = new ObservableCollection<OrderRow>();
            foreach (var o in sorted)
                rows.Add(new OrderRow { Order = o, RowNumber = number++ });
            OrderRows = rows;
            OrdersGrid.ItemsSource = OrderRows;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditProductWindow(Categories.ToList(), Manufacturers.ToList());
            if (dialog.ShowDialog() == true)
            {
                _productRepo.AddProduct(dialog.NewProduct);
                LoadProducts();
                MessageBox.Show("Товар добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as ProductRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить товар \"{selected.Name}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _productRepo.DeleteProduct(selected.Id);
                LoadProducts();
            }
        }

        private void SaveProducts_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in ProductRows)
                _productRepo.UpdateProduct(row.Product);
            MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveOrders_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in OrderRows)
                _orderRepo.UpdateOrderStatus(row.Id, row.Status);
            MessageBox.Show("Статусы заказов обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersGrid.SelectedItem as OrderRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (MessageBox.Show($"Удалить заказ №{selected.RowNumber} (ID {selected.Id}) от {selected.OrderDate:dd.MM.yyyy}?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _orderRepo.DeleteOrder(selected.Id);
                LoadOrders(); 
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