using AquaPark.Services;
using AquaPark.Views;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        public ICommand ClientsCommand { get; }
        public ICommand TicketsCommand { get; }

        public MenuViewModel()
        {
            ClientsCommand = new RelayCommand(OpenClientsPage);
            TicketsCommand = new RelayCommand(OpenTicketsPage);
        }

        private void OpenClientsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new ClientsPage());
            }
        }

        private void OpenTicketsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new TicketsPage());
            }
        }
    }
}