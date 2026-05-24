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
    public class EditAttractionViewModel : BaseViewModel
    {
        private readonly int _attractionId;

        private ObservableCollection<Zone> _zones = null!;
        private Zone _selectedZone = null!;

        private string _attractionName = string.Empty;
        private string _description = string.Empty;
        private int _ageLimit;
        private int? _heightLimit;
        private bool _isActive;
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

        public EditAttractionViewModel(int attractionId)
        {
            _attractionId = attractionId;

            SaveCommand = new RelayCommand(Save);
            BackCommand = new RelayCommand(Back);

            LoadData();
            LoadAttraction();
        }

        private void LoadData()
        {
            Zones = new ObservableCollection<Zone>(
                AppData.db.Zones.ToList()
            );
        }

        private void LoadAttraction()
        {
            var attraction = AppData.db.Attractions
                .FirstOrDefault(a => a.AttractionId == _attractionId);

            if (attraction == null)
            {
                ErrorMessage = "Аттракцион не найден";
                return;
            }

            AttractionName = attraction.AttractionName;
            Description = attraction.Description;
            AgeLimit = attraction.AgeLimit;
            HeightLimit = attraction.HeightLimit;
            IsActive = attraction.IsActive;

            SelectedZone = Zones
                .FirstOrDefault(z => z.ZoneId == attraction.ZoneId)!;
        }

        private void Save(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(AttractionName))
            {
                ErrorMessage = "Введите название аттракциона";
                return;
            }

            if (SelectedZone == null)
            {
                ErrorMessage = "Выберите зону";
                return;
            }

            if (AgeLimit < 0)
            {
                ErrorMessage = "Возрастное ограничение не может быть меньше 0";
                return;
            }

            if (HeightLimit.HasValue && HeightLimit.Value < 0)
            {
                ErrorMessage = "Ограничение по росту не может быть меньше 0";
                return;
            }

            var attraction = AppData.db.Attractions
                .FirstOrDefault(a => a.AttractionId == _attractionId);

            if (attraction == null)
            {
                ErrorMessage = "Аттракцион не найден";
                return;
            }

            attraction.AttractionName = AttractionName;
            attraction.ZoneId = SelectedZone.ZoneId;
            attraction.Description = Description;
            attraction.AgeLimit = AgeLimit;
            attraction.HeightLimit = HeightLimit;
            attraction.IsActive = IsActive;

            AppData.db.SaveChanges();

            MessageBox.Show("Данные аттракциона успешно изменены",
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