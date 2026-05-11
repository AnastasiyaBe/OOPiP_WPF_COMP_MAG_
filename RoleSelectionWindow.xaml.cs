using System.Windows;
using ComputerLibrary;

namespace ComputerStoreWPF
{
    public partial class RoleSelectionWindow : Window
    {
        public RoleSelectionWindow()
        {
            InitializeComponent();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            DbConnectionFactory.Instance.InitializeAsUser();
            var catalogWindow = new CatalogWindow();
            catalogWindow.Show();
            this.Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            DbConnectionFactory.Instance.InitializeAsUser();

            var passwordWindow = new PasswordWindow();
            passwordWindow.Owner = this;
            if (passwordWindow.ShowDialog() == true)
            {
                var adminWindow = new AdminWindow(new ProductRepository(), new OrderRepository(), new CategoryRepository());
                adminWindow.Show();
                this.Close();
            }
        }
    }
}