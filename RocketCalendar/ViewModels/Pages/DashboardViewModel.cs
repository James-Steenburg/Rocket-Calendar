using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
using RocketCalendar.Services;
using RocketCalendar.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        RocketCalendar.Helpers.FileIOHelper io = new RocketCalendar.Helpers.FileIOHelper();

        private bool _isInitialized = false;

        private GlobalAppData _appData;
        private IContentDialogService _contentDialogService;
        private WindowsProviderService _windowsProviderService;
        private ISnackbarService _snackbarService;
        private INavigationService _navigationService;

        [ObservableProperty]
        private RocketCalendarModel _activeCalendar;

        [RelayCommand]
        private void LoadACalendarButton()
        {
            try
            {
                //Import Calendar from Xml here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.OpenFileDialog openFileDialog = new()
                {
                    InitialDirectory = appDataFolder,
                    Filter = "Rocket xml files (*.xml)|*.xml"
                };

                if (openFileDialog.ShowDialog() != true)
                {
                    //ShowErrorSnackbar("The application failed to import your calendar from a Xml file. Verify you are selecting the correct file.");
                    return;
                }

                var newCalendar = io.LoadCalendar_XML(openFileDialog.FileName);

                if (newCalendar != null)
                {
                    var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Calendar Data Change",
                        Content = "Are you sure you want to load a new calendar? Unsaved data will be lost.",
                        PrimaryButtonText = "Load New Calendar",
                        PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Caution
                    };

                    var result = uiMessageBox.ShowDialogAsync();

                    if (result.Result.ToString() == "Primary")
                    {
                        _appData.ActiveRocketCalendar = newCalendar;
                        Properties.Settings.Default.DefaultCalendarFilePath = openFileDialog.FileName;
                        Properties.Settings.Default.Save();
                        ShowSuccessSnackbar("Your event list was imported from a Xml file");
                    }
                }
                else
                {
                    ShowErrorSnackbar("The application failed to import your calendar from a Xml file. Verify you are selecting the correct file.");
                }
            }
            catch
            {
                ShowErrorSnackbar("The application failed to import your calendar from a Xml file");
            }
        }

        [RelayCommand]
        private void CreateACalendarButton()
        {
            _navigationService.Navigate(typeof(Views.Pages.CreateCalendarPage));
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

        private void ShowErrorSnackbar(string message)
        {
            SnackbarAppearanceComboBoxSelectedIndex = 5;
            _snackbarService.Show(
            "Error:",
            message,
            _snackbarAppearance,
            new SymbolIcon(SymbolRegular.ErrorCircle24),
            TimeSpan.FromSeconds(3)
            );
        }

        private void ShowSuccessSnackbar(string message)
        {
            SnackbarAppearanceComboBoxSelectedIndex = 3;
            _snackbarService.Show(
            "Success!",
            message,
            _snackbarAppearance,
            new SymbolIcon(SymbolRegular.CheckmarkCircle24),
            TimeSpan.FromSeconds(3)
            );
        }


        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() 
        { 
        }

        private void InitializeViewModel()
        {
            //initialization ...

            _isInitialized = true;
        }

        public DashboardViewModel(GlobalAppData appData, ISnackbarService snackbarService, IContentDialogService contentDialogService, WindowsProviderService windowsProviderService, INavigationService navigationService)
        {
            _appData = appData;
            ActiveCalendar = _appData.ActiveRocketCalendar;
            _contentDialogService = contentDialogService;
            _windowsProviderService = windowsProviderService;
            _snackbarService = snackbarService;
            _navigationService = navigationService;
            //SelectedRocketMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth];
        }

    }
}
