using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class AddUserPage : Page
    {
        public AddUserPage()
        {
            InitializeComponent();
            DataContext = new AddUserViewModel();
        }
    }
}
