using ComputerLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ComputerStoreWPF
{
    public partial class CartWindow : Window
    {
        public class DisplayItem : INotifyPropertyChanged
        {
            public CartItem CartItem { get; set; }
            private bool _isSelected = true;
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            public Product Product => CartItem.Product;
            public int Quantity => CartItem.Quantity;
            public decimal Total => CartItem.Total;
        }

        private ObservableCollection<DisplayItem> _displayItems;
        private ObservableCollection<CartItem> _originalCart;

        public CartWindow(ObservableCollection<CartItem> cartItems)
        {
            InitializeComponent();
            _originalCart = cartItems;
            _displayItems = new ObservableCollection<DisplayItem>();
            foreach (var item in _originalCart)
                _displayItems.Add(new DisplayItem { CartItem = item });
            CartDataGrid.ItemsSource = _displayItems;
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = _displayItems.Where(d => d.IsSelected).Sum(d => d.Total);
            TotalTextBlock.Text = total.ToString("F2") + " BYN";
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateTotal();
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var display = btn.Tag as DisplayItem;
            if (display != null)
            {
                display.CartItem.Quantity++;
                CartDataGrid.Items.Refresh();
                UpdateTotal();
            }
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var display = btn.Tag as DisplayItem;
            if (display != null)
            {
                if (display.CartItem.Quantity > 1)
                {
                    display.CartItem.Quantity--;
                    CartDataGrid.Items.Refresh();
                    UpdateTotal();
                }
                else
                {
                    _originalCart.Remove(display.CartItem);
                    _displayItems.Remove(display);
                    UpdateTotal();
                }
            }
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            var selected = _displayItems.Where(d => d.IsSelected).Select(d => d.CartItem).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Не выбрано ни одного товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal totalAmount = selected.Sum(item => item.Total);
            var dialog = new CustomerInfoDialog(selected, totalAmount);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var repo = new OrderRepository();
                    repo.CreateOrder(dialog.CustomerName, dialog.CustomerPhone, selected);

                    foreach (var item in selected)
                    {
                        _originalCart.Remove(item);
                        var disp = _displayItems.FirstOrDefault(d => d.CartItem == item);
                        if (disp != null) _displayItems.Remove(disp);
                    }

                    MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (_originalCart.Count == 0)
                        DialogResult = true;
                    else
                        UpdateTotal();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}