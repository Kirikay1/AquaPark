using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class TicketsViewModel : BaseViewModel
    {
        private ObservableCollection<Ticket> _tickets = null!;
        private Ticket _selectedTicket = null!;

        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set
            {
                _tickets = value;
                OnPropertyChanged();
            }
        }

        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }

        public TicketsViewModel()
        {
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);

            LoadTickets();
        }

        private void LoadTickets()
        {
            Tickets = new ObservableCollection<Ticket>(
                AppData.db.Tickets
                    .Include(t => t.TicketType)
                    .Include(t => t.Client)
                    .AsNoTracking()
                    .ToList()
            );
        }

        private void Add(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AddTicketPage());
            }
        }

        private void Edit(object? parameter)
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Выберите билет для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Изменение билета сделаем следующим шагом",
                            "Билеты",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Delete(object? parameter)
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Выберите билет для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранный билет?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var ticket = AppData.db.Tickets
                .FirstOrDefault(t => t.TicketId == SelectedTicket.TicketId);

            if (ticket == null)
            {
                MessageBox.Show("Билет не найден",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            AppData.db.Tickets.Remove(ticket);
            AppData.db.SaveChanges();

            LoadTickets();

            MessageBox.Show("Билет успешно удален",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Refresh(object? parameter)
        {
            LoadTickets();
        }

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new MenuPage());
            }
        }
    }
}