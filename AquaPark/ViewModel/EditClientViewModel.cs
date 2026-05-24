using AquaPark.Data;
using AquaPark.Services;
using AquaPark.Views;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class EditClientViewModel : BaseViewModel
    {
        private readonly int _clientId;

        private string _fullName = string.Empty;
        private DateTime? _birthDate;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private string _errorMessage = string.Empty;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public EditClientViewModel(int clientId)
        {
            _clientId = clientId;

            SaveCommand = new RelayCommand(Save);
            BackCommand = new RelayCommand(Back);

            LoadClient();
        }

        private void LoadClient()
        {
            var client = AppData.db.Clients.FirstOrDefault(c => c.ClientId == _clientId);

            if (client == null)
            {
                ErrorMessage = "Клиент не найден";
                return;
            }

            FullName = client.FullName;
            BirthDate = client.BirthDate.HasValue
                ? client.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;
            Phone = client.Phone;
            Email = client.Email;
        }

        private void Save(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Введите ФИО клиента";
                return;
            }

            var client = AppData.db.Clients.FirstOrDefault(c => c.ClientId == _clientId);

            if (client == null)
            {
                ErrorMessage = "Клиент не найден";
                return;
            }

            client.FullName = FullName;
            client.BirthDate = BirthDate.HasValue
                ? DateOnly.FromDateTime(BirthDate.Value)
                : null;
            client.Phone = Phone;
            client.Email = Email;

            AppData.db.SaveChanges();

            MessageBox.Show("Данные клиента успешно изменены",
                            "Сохранение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            Back(null);
        }

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new ClientsPage());
            }
        }
    }
}