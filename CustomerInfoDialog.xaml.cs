using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ComputerLibrary;

namespace ComputerStoreWPF
{
    public partial class CustomerInfoDialog : Window
    {
        public string CustomerName { get; private set; }
        public string CustomerPhone { get; private set; }

        public CustomerInfoDialog(List<CartItem> selectedItems, decimal totalAmount)
        {
            InitializeComponent();
            OrderItemsListView.ItemsSource = selectedItems;
            TotalTextBlock.Text = totalAmount.ToString("F2") + " BYN";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();

            if (!Regex.IsMatch(name, @"^[А-ЯA-Z][а-яa-z]*$"))
            {
                MessageBox.Show("Имя должно начинаться с большой буквы и содержать только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string phoneDigits = Regex.Replace(phone, @"\D", "");
            if (!phone.StartsWith("+375") || phoneDigits.Length != 12)
            {
                MessageBox.Show("Телефон должен быть в формате +375 XX XXX XX XX (например, +375 29 123 45 67)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CustomerName = char.ToUpper(name[0]) + name.Substring(1).ToLower();
            CustomerPhone = phone;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}