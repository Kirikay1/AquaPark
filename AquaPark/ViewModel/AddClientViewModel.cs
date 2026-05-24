using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class AddClientViewModel : BaseViewModel
    {
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

        public AddClientViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Clients"));
            BackCommand = new RelayCommand(Back);
        }

        private void Save(object? parameter)
        {
            if (!ValidationService.ValidateClient(FullName, BirthDate, Phone, Email, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            Client client = new Client
            {
                FullName = FullName.Trim(),
                BirthDate = BirthDate.HasValue
                    ? DateOnly.FromDateTime(BirthDate.Value)
                    : null,
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim()
            };

            AppData.db.Clients.Add(client);
            AppData.db.SaveChanges();

            MessageBox.Show("Клиент успешно добавлен",
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
