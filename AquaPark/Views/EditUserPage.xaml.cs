using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class EditUserPage : Page
    {
        public EditUserPage(int userId)
        {
            InitializeComponent();
            DataContext = new EditUserViewModel(userId);
        }
    }
}
