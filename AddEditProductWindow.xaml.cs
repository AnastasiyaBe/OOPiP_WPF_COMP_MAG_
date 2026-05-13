using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ComputerLibrary;

namespace ComputerStoreWPF
{
    public partial class AddEditProductWindow : Window
    {
        public Product NewProduct { get; private set; }
        private List<Category> _categories;
        private List<Manufacturer> _manufacturers;

        public AddEditProductWindow(List<Category> categories, List<Manufacturer> manufacturers, Product existingProduct = null)
        {
            InitializeComponent();
            _categories = categories;
            _manufacturers = manufacturers;

            CategoryCombo.ItemsSource = _categories;
            ManufacturerCombo.ItemsSource = _manufacturers;
            CategoryCombo.DisplayMemberPath = "Name";
            ManufacturerCombo.DisplayMemberPath = "Name";

            if (existingProduct != null)
            {
                Title = "Редактирование товара";
                NameTextBox.Text = existingProduct.Name;
                CharacteristicsTextBox.Text = existingProduct.Characteristics;
                PriceTextBox.Text = existingProduct.Price.ToString("F2");
                StockTextBox.Text = existingProduct.StockQuantity.ToString();
                ImagePathTextBox.Text = existingProduct.ImagePath;
                CategoryCombo.SelectedItem = existingProduct.Category;
                ManufacturerCombo.SelectedItem = existingProduct.Manufacturer;
                NewProduct = existingProduct;
            }
            else
            {
                Title = "Добавление товара";
                NewProduct = new Product();
                CategoryCombo.SelectedIndex = 0;
                ManufacturerCombo.SelectedIndex = 0;
                NewProduct.Category = (Category)CategoryCombo.SelectedItem;
                NewProduct.Manufacturer = (Manufacturer)ManufacturerCombo.SelectedItem;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(StockTextBox.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Введите корректное количество (целое неотрицательное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CategoryCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ManufacturerCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewProduct.Name = NameTextBox.Text.Trim();
            NewProduct.Characteristics = CharacteristicsTextBox.Text.Trim();
            NewProduct.Price = price;
            NewProduct.StockQuantity = stock;
            NewProduct.ImagePath = ImagePathTextBox.Text.Trim();
            NewProduct.Category = (Category)CategoryCombo.SelectedItem;
            NewProduct.Manufacturer = (Manufacturer)ManufacturerCombo.SelectedItem;

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