using AquaPark.Data;
using System.Windows.Controls;

namespace AquaPark.Views
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsDataGrid.ItemsSource = AppData.db.Clients.ToList();
        }
    }
}
