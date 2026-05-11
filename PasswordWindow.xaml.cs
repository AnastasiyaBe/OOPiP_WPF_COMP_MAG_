using ComputerLibrary;
using ComputerLibrary.Repositories;
using System.Windows;

namespace ComputerStoreWPF
{
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userRepo = new UserRepository();
            var admin = userRepo.AuthenticateAdmin("admin", password);
            if (admin != null)
            {
                DbConnectionFactory.Instance.InitializeAsAdmin(); // переключаем на администратора
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}