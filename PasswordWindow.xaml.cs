using System.Windows;
using ComputerLibrary;

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
            string enteredPassword = PasswordBox.Password;
            if (DbConnectionFactory.Instance.InitializeAsAdmin(enteredPassword))
            {
                MessageBox.Show("Доступ разрешён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль администратора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}