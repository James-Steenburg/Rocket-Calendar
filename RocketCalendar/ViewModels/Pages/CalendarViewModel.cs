using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
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

        private GlobalAppData _appData;

        [ObservableProperty]
        private string _calendarName = "ExampleCalendarName";

        [ObservableProperty]
        private Models.RocketCalendar _activeCalendar;

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

            ActiveCalendar = _appData.ActiveRocketCalendar;
        }

        public void OnNavigatedFrom() 
        {
            //Add validation and save work prompt if needed
            //May want to change to where the only thing saved is what can be changed
            _appData.ActiveRocketCalendar = ActiveCalendar;
        }

        private void InitializeViewModel()
        {
            //initialization ...

            _isInitialized = true;
        }

        public CalendarViewModel(GlobalAppData appData)
        {
            _appData = appData;
        }
    }
}
