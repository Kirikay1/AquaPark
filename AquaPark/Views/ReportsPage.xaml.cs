using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            DataContext = new ReportsViewModel();
        }
    }
}
