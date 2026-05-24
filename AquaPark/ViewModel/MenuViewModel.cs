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
        public ICommand AttractionsCommand { get; }
        public ICommand SalesCommand { get; }
        public ICommand PaymentsCommand { get; }

        public MenuViewModel()
        {
            ClientsCommand = new RelayCommand(OpenClientsPage);
            TicketsCommand = new RelayCommand(OpenTicketsPage);
            AttractionsCommand = new RelayCommand(OpenAttractionsPage);
            SalesCommand = new RelayCommand(OpenSalesPage);
            PaymentsCommand = new RelayCommand(OpenPaymentsPage);
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

        private void OpenAttractionsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AttractionsPage());
            }
        }

        private void OpenSalesPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new SalesPage());
            }
        }

        private void OpenPaymentsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new PaymentsPage());
            }
        }
    }
}