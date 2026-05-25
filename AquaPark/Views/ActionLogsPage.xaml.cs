using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class ActionLogsPage : Page
    {
        public ActionLogsPage()
        {
            InitializeComponent();
            DataContext = new ActionLogsViewModel();
        }
    }
}
