using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class CalendarViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _calendarName = "ExampleCalendarName";

        [RelayCommand]
        private void DecrementDisplayYear(object obj)
        {
            //...
        }

        [RelayCommand]
        private void IncrementDisplayYear(object obj)
        {
            //...
        }

        [RelayCommand]
        private void DecrementDisplayMonth(object obj)
        {
            //...
        }

        [RelayCommand]
        private void IncrementDisplayMonth(object obj)
        {
            //...
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            //initialization ...

            _isInitialized = true;
        }
    }
}
