using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class ActionLogsViewModel : PagedTableViewModel
    {
        private ObservableCollection<ActionLogEntry> _logs = null!;
        private string _searchText = string.Empty;
        private DateTime? _dateFrom;
        private DateTime? _dateTo;

        public ObservableCollection<ActionLogEntry> Logs { get => _logs; set { _logs = value; OnPropertyChanged(); } }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ResetPage(); LoadLogs(); }
        }

        public DateTime? DateFrom
        {
            get => _dateFrom;
            set { _dateFrom = value; OnPropertyChanged(); ResetPage(); LoadLogs(); }
        }

        public DateTime? DateTo
        {
            get => _dateTo;
            set { _dateTo = value; OnPropertyChanged(); ResetPage(); LoadLogs(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand BackCommand { get; }

        public ActionLogsViewModel()
        {
            RefreshCommand = new RelayCommand(_ => LoadLogs());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            BackCommand = new RelayCommand(_ => NavigationService.Navigate(new MenuPage()));

            LoadLogs();
        }

        protected override void LoadPage()
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            var query = AuditService.GetLogs(SearchText, DateFrom, DateTo).AsQueryable();
            query = ApplyPaging(query);
            Logs = new ObservableCollection<ActionLogEntry>(query.ToList());
        }

        private void ClearSearch(object? parameter)
        {
            SearchText = string.Empty;
            DateFrom = null;
            DateTo = null;
        }
    }
}
