using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class FileIOViewModel : ObservableObject
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;
        private ISnackbarService _snackbarService;

        [ObservableProperty]
        private bool _includePrivateEvents;

        public FileIOViewModel(GlobalAppData appData, ISnackbarService snackbarService)
        {
            _appData = appData;
            _snackbarService = snackbarService;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
            
            //pull whatever is needed from AppData here
        }

        public void OnNavigatedFrom()
        {
            //save anything needed in AppData - shouldn't need anything saved in this vm
        }

        private void InitializeViewModel()
        {
            //initialization ...

            _isInitialized = true;
        }

        private ControlAppearance _snackbarAppearance = ControlAppearance.Secondary;

        private int _snackbarAppearanceComboBoxSelectedIndex = 1;

        public int SnackbarAppearanceComboBoxSelectedIndex
        {
            get => _snackbarAppearanceComboBoxSelectedIndex;
            set
            {
                SetProperty<int>(ref _snackbarAppearanceComboBoxSelectedIndex, value);
                UpdateSnackbarAppearance(value);
            }
        }

        private void UpdateSnackbarAppearance(int appearanceIndex)
        {
            _snackbarAppearance = appearanceIndex switch
            {
                1 => ControlAppearance.Secondary,
                2 => ControlAppearance.Info,
                3 => ControlAppearance.Success,
                4 => ControlAppearance.Caution,
                5 => ControlAppearance.Danger,
                6 => ControlAppearance.Light,
                7 => ControlAppearance.Dark,
                8 => ControlAppearance.Transparent,
                _ => ControlAppearance.Primary
            };
        }

        [RelayCommand]
        private void SaveToExcel()
        {
            try
            {
                //Save To Excel here..

                SnackbarAppearanceComboBoxSelectedIndex = 3;
                _snackbarService.Show(
                "Success!",
                "Your event list was saved to an Excel file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.CheckmarkCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
            catch
            {
                SnackbarAppearanceComboBoxSelectedIndex = 5;
                _snackbarService.Show(
                "Error:",
                "The application failed to save your event list to an Excel file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.ErrorCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
        }

        [RelayCommand]
        private void SaveToXaml()
        {
            try
            {
                //Save To Xaml here..

                SnackbarAppearanceComboBoxSelectedIndex = 3;
                _snackbarService.Show(
                "Success!",
                "Your event list was saved to an Xaml file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.CheckmarkCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
            catch
            {
                SnackbarAppearanceComboBoxSelectedIndex = 5;
                _snackbarService.Show(
                "Error:",
                "The application failed to save your event list to a Xaml file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.ErrorCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
        }

        [RelayCommand]
        private void ExportCalendarToXaml()
        {
            try
            {
                //Export Calendar to Xaml here..

                SnackbarAppearanceComboBoxSelectedIndex = 3;
                _snackbarService.Show(
                "Success!",
                "Your calendar was saved to a Xaml file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.CheckmarkCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
            catch
            {
                SnackbarAppearanceComboBoxSelectedIndex = 5;
                _snackbarService.Show(
                "Error:",
                "The application failed to save your calendar to a Xaml file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.ErrorCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
        }

        [RelayCommand]
        private void ImportCalendarFromXaml()
        {
            try
            {
                //Import Calendar from Xaml here..

                SnackbarAppearanceComboBoxSelectedIndex = 3;
                _snackbarService.Show(
                "Success!",
                "Your calendar was imported from a Xaml file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.CheckmarkCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
            catch
            {
                SnackbarAppearanceComboBoxSelectedIndex = 5;
                _snackbarService.Show(
                "Error:",
                "The application failed to import your calendar from a Xaml file",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.ErrorCircle24),
                TimeSpan.FromSeconds(3)
                );
            }
        }
    }
}
