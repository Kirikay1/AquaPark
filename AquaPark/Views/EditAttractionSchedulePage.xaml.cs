using AquaPark.ViewModel;
using System.Windows.Controls;

namespace AquaPark.Views
{
    public partial class EditAttractionSchedulePage : Page
    {
        public EditAttractionSchedulePage(int scheduleId)
        {
            InitializeComponent();
            DataContext = new EditAttractionScheduleViewModel(scheduleId);
        }
    }
}
