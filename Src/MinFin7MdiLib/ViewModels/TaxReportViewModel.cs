using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MinFin7MdiLib.Services;

namespace MinFin7MdiLib.ViewModels
{
    public class TaxReportViewModel : INotifyPropertyChanged
    {
        private readonly TaxReportService _reportService;
        private double _grandTotal;
        private string _statusText;
        private DateTime _printDate;

        public TaxReportViewModel(TaxReportService reportService)
        {
            _reportService = reportService;
            PrintDate = DateTime.Now;
            StatusText = "Ready";
        }

        public double GrandTotal
        {
            get => _grandTotal;
            set
            {
                if (_grandTotal != value)
                {
                    _grandTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime PrintDate
        {
            get => _printDate;
            set
            {
                if (_printDate != value)
                {
                    _printDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand PrintCommand { get; set; }
        public ICommand ExportToPdfCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple implementation of ICommand for the view model
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}