using AquaPark.Services;
using System;
using System.Linq;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public abstract class PagedTableViewModel : BaseViewModel
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private int _totalItems;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                int nextPage = Math.Max(1, value);

                if (_pageNumber == nextPage)
                {
                    return;
                }

                _pageNumber = nextPage;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageInfo));
                LoadPage();
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0 || _pageSize == value)
                {
                    return;
                }

                _pageSize = value;
                _pageNumber = 1;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageNumber));
                OnPropertyChanged(nameof(PageInfo));
                LoadPage();
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            private set
            {
                _totalItems = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));

        public string PageInfo => $"Страница {PageNumber} из {TotalPages} ({TotalItems} записей)";

        public ICommand FirstPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }

        protected PagedTableViewModel()
        {
            FirstPageCommand = new RelayCommand(_ => PageNumber = 1, _ => PageNumber > 1);
            PreviousPageCommand = new RelayCommand(_ => PageNumber--, _ => PageNumber > 1);
            NextPageCommand = new RelayCommand(_ => PageNumber++, _ => PageNumber < TotalPages);
            LastPageCommand = new RelayCommand(_ => PageNumber = TotalPages, _ => PageNumber < TotalPages);
        }

        protected IQueryable<T> ApplyPaging<T>(IQueryable<T> query)
        {
            TotalItems = query.Count();

            if (PageNumber > TotalPages)
            {
                _pageNumber = TotalPages;
                OnPropertyChanged(nameof(PageNumber));
            }

            return query.Skip((PageNumber - 1) * PageSize).Take(PageSize);
        }

        protected void ResetPage()
        {
            if (PageNumber == 1)
            {
                return;
            }

            _pageNumber = 1;
            OnPropertyChanged(nameof(PageNumber));
            OnPropertyChanged(nameof(PageInfo));
        }

        protected abstract void LoadPage();
    }
}
