using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class AddAttractionSchedulePage : Page
    {
        public AddAttractionSchedulePage()
        {
            InitializeComponent();
            DataContext = new AddAttractionScheduleViewModel();
        }
    }
}
