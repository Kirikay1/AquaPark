using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class AddAttractionViewModel : BaseViewModel
    {
        private ObservableCollection<Zone> _zones = null!;
        private Zone _selectedZone = null!;

        private string _attractionName = string.Empty;
        private string _description = string.Empty;
        private int _ageLimit;
        private int? _heightLimit;
        private bool _isActive = true;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Zone> Zones
        {
            get => _zones;
            set
            {
                _zones = value;
                OnPropertyChanged();
            }
        }

        public Zone SelectedZone
        {
            get => _selectedZone;
            set
            {
                _selectedZone = value;
                OnPropertyChanged();
            }
        }

        public string AttractionName
        {
            get => _attractionName;
            set
            {
                _attractionName = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public int AgeLimit
        {
            get => _ageLimit;
            set
            {
                _ageLimit = value;
                OnPropertyChanged();
            }
        }

        public int? HeightLimit
        {
            get => _heightLimit;
            set
            {
                _heightLimit = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
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

        public AddAttractionViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Attractions"));
            BackCommand = new RelayCommand(Back);

            LoadZones();
        }

        private void LoadZones()
        {
            Zones = new ObservableCollection<Zone>(
                AppData.db.Zones.ToList()
            );
        }

        private void Save(object? parameter)
        {
            if (!ValidationService.ValidateAttraction(AttractionName, AgeLimit, HeightLimit, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            if (SelectedZone == null)
            {
                ErrorMessage = "Выберите зону";
                return;
            }

            Attraction attraction = new Attraction
            {
                AttractionName = AttractionName.Trim(),
                ZoneId = SelectedZone.ZoneId,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                AgeLimit = AgeLimit,
                HeightLimit = HeightLimit,
                IsActive = IsActive
            };

            AppData.db.Attractions.Add(attraction);
            AppData.db.SaveChanges();

            MessageBox.Show("Аттракцион успешно добавлен",
                            "Сохранение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            Back(null);
        }

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AttractionsPage());
            }
        }
    }
}
