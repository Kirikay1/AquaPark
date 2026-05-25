using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AquaPark.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();
        private bool _trackUnsavedChanges;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasUnsavedChanges { get; private set; }

        public bool HasErrors => _errors.Count > 0;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            if (_trackUnsavedChanges && propertyName != nameof(HasUnsavedChanges))
            {
                HasUnsavedChanges = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasUnsavedChanges)));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public System.Collections.IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return Array.Empty<string>();
            }

            return _errors[propertyName];
        }

        protected void SetError(string propertyName, string error)
        {
            _errors[propertyName] = new List<string> { error };
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        protected void ClearError(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        protected void ClearErrors()
        {
            var propertyNames = _errors.Keys.ToList();
            _errors.Clear();

            foreach (string propertyName in propertyNames)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            OnPropertyChanged(nameof(HasErrors));
        }

        protected void EnableUnsavedChangesTracking()
        {
            _trackUnsavedChanges = true;
            MarkAsSaved();
        }

        protected void MarkAsSaved()
        {
            HasUnsavedChanges = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasUnsavedChanges)));
        }

        public bool ConfirmDiscardChanges()
        {
            if (!HasUnsavedChanges)
            {
                return true;
            }

            MessageBoxResult result = MessageBox.Show("Есть несохраненные изменения. Перейти без сохранения?",
                                                      "Несохраненные изменения",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }
    }
}
