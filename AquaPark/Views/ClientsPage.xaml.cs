using AquaPark.Data;
using AquaPark.ViewModel;
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
            DataContext = new ClientsViewModel();
        }
    }
}
