using System.Windows;

namespace ComputerStoreWPF
{
    public partial class RoleSelectionWindow : Window
    {
        public RoleSelectionWindow()
        {
            InitializeComponent();
        }

        private void OpenCatalog_Click(object sender, RoutedEventArgs e)
        {
            var catalogWindow = new CatalogWindow();
            catalogWindow.Show();
            this.Close();
        }
    }
}