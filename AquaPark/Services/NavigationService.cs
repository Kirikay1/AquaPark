using AquaPark.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace AquaPark.Services
{
    public static class NavigationService
    {
        public static void Navigate(Page page)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow)
            {
                return;
            }

            if (mainWindow.MainFrame.Content is Page currentPage
                && currentPage.DataContext is BaseViewModel viewModel
                && !viewModel.ConfirmDiscardChanges())
            {
                return;
            }

            mainWindow.OpenPage(page);
        }
    }
}
