using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class EditAttractionScheduleViewModel : BaseViewModel
    {
        private readonly int _scheduleId;
        private ObservableCollection<Attraction> _attractions = null!;
        private ObservableCollection<string> _statuses = null!;
        private Attraction _selectedAttraction = null!;
        private DateTime? _workDate;
        private string _startTimeText = string.Empty;
        private string _endTimeText = string.Empty;
        private string _selectedStatus = string.Empty;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Attraction> Attractions
        {
            get => _attractions;
            set
            {
                _attractions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Statuses
        {
            get => _statuses;
            set
            {
                _statuses = value;
                OnPropertyChanged();
            }
        }

        public Attraction SelectedAttraction
        {
            get => _selectedAttraction;
            set
            {
                _selectedAttraction = value;
                OnPropertyChanged();
            }
        }

        public DateTime? WorkDate
        {
            get => _workDate;
            set
            {
                _workDate = value;
                OnPropertyChanged();
            }
        }

        public string StartTimeText
        {
            get => _startTimeText;
            set
            {
                _startTimeText = value;
                OnPropertyChanged();
            }
        }

        public string EndTimeText
        {
            get => _endTimeText;
            set
            {
                _endTimeText = value;
                OnPropertyChanged();
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
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

        public EditAttractionScheduleViewModel(int scheduleId)
        {
            _scheduleId = scheduleId;

            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Schedules"));
            BackCommand = new RelayCommand(Back);

            LoadData();
            LoadSchedule();
            EnableUnsavedChangesTracking();
        }

        private void LoadData()
        {
            Attractions = new ObservableCollection<Attraction>(
                AppData.db.Attractions
                    .Include(a => a.Zone)
                    .ToList()
            );

            Statuses = new ObservableCollection<string>
            {
                "Работает",
                "Ремонт",
                "Закрыт"
            };
        }

        private void LoadSchedule()
        {
            var schedule = AppData.db.AttractionSchedules
                .FirstOrDefault(s => s.ScheduleId == _scheduleId);

            if (schedule == null)
            {
                ErrorMessage = "Запись расписания не найдена";
                return;
            }

            SelectedAttraction = Attractions.FirstOrDefault(a => a.AttractionId == schedule.AttractionId)!;
            WorkDate = schedule.WorkDate.ToDateTime(TimeOnly.MinValue);
            StartTimeText = schedule.StartTime.ToString("HH:mm");
            EndTimeText = schedule.EndTime.ToString("HH:mm");
            SelectedStatus = schedule.Status;
        }

        private void Save(object? parameter)
        {
            if (SelectedAttraction == null)
            {
                ErrorMessage = "Выберите аттракцион";
                return;
            }

            if (!ValidationService.ValidateSchedule(WorkDate, StartTimeText, EndTimeText, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            var schedule = AppData.db.AttractionSchedules
                .FirstOrDefault(s => s.ScheduleId == _scheduleId);

            if (schedule == null)
            {
                ErrorMessage = "Запись расписания не найдена";
                return;
            }

            schedule.AttractionId = SelectedAttraction.AttractionId;
            schedule.WorkDate = DateOnly.FromDateTime(WorkDate.GetValueOrDefault());
            schedule.StartTime = TimeOnly.Parse(StartTimeText);
            schedule.EndTime = TimeOnly.Parse(EndTimeText);
            schedule.Status = SelectedStatus;

            if (!DatabaseErrorService.TrySaveChanges("Запись расписания успешно изменена"))
            {
                return;
            }

            AuditService.Log("Изменение", "Расписание", schedule.ScheduleId, schedule.Status);
            MarkAsSaved();

            Back(null);
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new AttractionSchedulesPage());
        }
    }
}
