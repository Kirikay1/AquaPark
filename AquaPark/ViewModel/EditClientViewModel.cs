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

            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Clients"));
            BackCommand = new RelayCommand(Back);

            LoadClient();
            EnableUnsavedChangesTracking();
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
            Phone = client.Phone ?? string.Empty;
            Email = client.Email ?? string.Empty;
        }

        private void Save(object? parameter)
        {
            if (!ValidationService.ValidateClient(FullName, BirthDate, Phone, Email, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            if (DuplicateCheckService.ClientPhoneExists(Phone, _clientId))
            {
                ErrorMessage = "Клиент с таким телефоном уже существует";
                return;
            }

            if (DuplicateCheckService.ClientEmailExists(Email, _clientId))
            {
                ErrorMessage = "Клиент с таким email уже существует";
                return;
            }

            var client = AppData.db.Clients.FirstOrDefault(c => c.ClientId == _clientId);

            if (client == null)
            {
                ErrorMessage = "Клиент не найден";
                return;
            }

            client.FullName = FullName.Trim();
            client.BirthDate = BirthDate.HasValue
                ? DateOnly.FromDateTime(BirthDate.Value)
                : null;
            client.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
            client.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();

            if (!DatabaseErrorService.TrySaveChanges("Данные клиента успешно изменены"))
            {
                return;
            }

            AuditService.Log("Изменение", "Клиенты", client.ClientId, client.FullName);
            MarkAsSaved();

            Back(null);
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new ClientsPage());
        }
    }
}
