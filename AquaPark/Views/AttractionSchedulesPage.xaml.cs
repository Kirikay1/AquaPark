using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class AttractionSchedulesPage : Page
    {
        public AttractionSchedulesPage()
        {
            InitializeComponent();
            DataContext = new AttractionSchedulesViewModel();
        }
    }
}
