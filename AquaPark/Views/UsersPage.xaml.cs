using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            DataContext = new UsersViewModel();
        }
    }
}
