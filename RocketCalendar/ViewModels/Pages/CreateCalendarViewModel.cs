using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Wpf.Ui.Controls;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using System.IO;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class CreateCalendarViewModel : ObservableObject
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;
        private IContentDialogService _contentDialogService;
        private ISnackbarService _snackbarService;
        RocketCalendar.Helpers.FileIOHelper io = new RocketCalendar.Helpers.FileIOHelper();

        [ObservableProperty]
        private bool _isFlyoutOpen = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private int _baseDateIndexInput;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        [NotifyPropertyChangedFor(nameof(IsSpecificDayNumberSelectEnabled))]
        [NotifyPropertyChangedFor(nameof(SelectedBaseMonthMaxDays))]
        private int _baseMonthIndexInput;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private int _baseDayInput;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private int _baseYearInput;


        public bool IsSpecificDayNumberSelectEnabled
        {
            get
            {
                return (MonthListViewItems.Count > 0);
            }
        }

        public int SelectedBaseMonthMaxDays
        {
            get
            {
                if(MonthListViewItems.Count > 0 && BaseMonthIndexInput >= 0)
                {
                    if(BaseDayInput > MonthListViewItems[BaseMonthIndexInput].NumOfDays)
                    {
                        BaseDayInput = MonthListViewItems[BaseMonthIndexInput].NumOfDays;
                    }
                    return MonthListViewItems[BaseMonthIndexInput].NumOfDays;
                }
                else
                {
                    return 1;
                }
            }
        }

        public bool IsCreateCalendarButtonEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(NewCalendarName)
                    && MonthListViewItems.Count > 0
                    && DayNameListViewItems.Count > 0
                    && BaseDateIndexInput >= 0
                    && BaseMonthIndexInput >= 0
                    && BaseDayInput > 0
                    && IsSpecificDayNumberSelectEnabled;
            }
        }

        [ObservableProperty]
        private bool? _isAddMonthButtonEnabled = false;
        
        [ObservableProperty]
        private string _newMonthName;

        [ObservableProperty]
        private int _newMonthDayCount;

        [ObservableProperty]
        private string _newDayName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private RocketDate? _baseDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private string _newCalendarName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private ObservableCollection<RocketMonth> _monthListViewItems = new ObservableCollection<RocketMonth>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateCalendarButtonEnabled))]
        private ObservableCollection<string> _dayNameListViewItems = new ObservableCollection<string>();

        [ObservableProperty]
        private ObservableCollection<RocketMonth> _monthListBoxItems = new ObservableCollection<RocketMonth>();

        [ObservableProperty]
        private ObservableCollection<string> _dayNameListBoxItems = new ObservableCollection<string>();

        [RelayCommand]
        private async Task CreateCalendar(object content)
        {
            try
            {
                ContentDialogResult result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Creating Calendar",
                    Content = "Are you sure you want to save this calendar over the current calendar?",
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel",

                }
                );

                string resultText = result switch
                {
                    ContentDialogResult.Primary => "Calendar Saved",
                    ContentDialogResult.Secondary => "User did not save their work",
                    _ => "User cancelled the dialog"
                };

                if (resultText == "Calendar Saved")
                {
                    ObservableCollection<RocketEvent> events = new ObservableCollection<RocketEvent>();

                    _appData.ActiveRocketCalendar = new RocketCalendarModel(
                        NewCalendarName,
                        new RocketDate(BaseDateIndexInput, BaseDayInput, BaseMonthIndexInput, BaseYearInput),
                        MonthListViewItems,
                        DayNameListViewItems,
                        events,
                        BaseMonthIndexInput,
                        BaseYearInput
                        );

                    SaveCalendarToFile();
                    ShowSuccessSnackbar("Your new calendar has been loaded.");
                }
            }
            catch
            {
                ShowSuccessSnackbar("A new calendar was not created.");
            }

            

            /*
             //not needed?
            if(MonthListViewItems.Count > 0 && DayNameListViewItems.Count > 0 && NewCalendarName.Length > 0)
            {
                MonthListBoxItems = MonthListViewItems;
                DayNameListBoxItems = DayNameListViewItems;

                //Set Base Date
                ContentDialogResult result = await contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Last Step!",
                    Content = content,
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel",
                }
                );

                string resultText = result switch
                {
                    ContentDialogResult.Primary => "User saved their work",
                    ContentDialogResult.Secondary => "User did not save their work",
                    _ => "User cancelled the dialog"
                };

                //...
            }
            else
            {
                if (!IsFlyoutOpen)
                    IsFlyoutOpen = true;
            }

             */

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
        private void AddMonthToCollection(object obj)
        {
            //...
            MonthListViewItems.Add(new RocketMonth(NewMonthName, NewMonthDayCount));
            OnPropertyChanged(nameof(IsCreateCalendarButtonEnabled));
            OnPropertyChanged(nameof(IsSpecificDayNumberSelectEnabled));
            OnPropertyChanged(nameof(SelectedBaseMonthMaxDays));
        }

        [RelayCommand]
        private void RemoveMonthFromCollection(object obj)
        {
            if ((int)obj >= 0)
            {
                MonthListViewItems.RemoveAt((int)obj);
                OnPropertyChanged(nameof(IsCreateCalendarButtonEnabled));
                OnPropertyChanged(nameof(IsSpecificDayNumberSelectEnabled));
                OnPropertyChanged(nameof(SelectedBaseMonthMaxDays));
            }
        }

        [RelayCommand]
        private void AddDayToCollection(object obj)
        {
            //...
            DayNameListViewItems.Add(NewDayName);
            OnPropertyChanged(nameof(IsCreateCalendarButtonEnabled));
        }

        [RelayCommand]
        private void RemoveDayFromCollection(object obj)
        {
            if ((int)obj >= 0)
            {
                DayNameListViewItems.RemoveAt((int)obj);
                OnPropertyChanged(nameof(IsCreateCalendarButtonEnabled));
            }
        }

        [RelayCommand]
        private void CloseFlyout(object obj)
        {
            if (IsFlyoutOpen)
                IsFlyoutOpen = false;
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


        public CreateCalendarViewModel(GlobalAppData appData, IContentDialogService contentDialogService, ISnackbarService snackbarService)
        {
            _appData = appData;
            _contentDialogService = contentDialogService;
            _snackbarService = snackbarService;
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

        private void SaveCalendarToFile()
        {
            try
            {
                //Export Calendar to Xaml here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = _appData.ActiveRocketCalendar.CalendarName,
                    InitialDirectory = appDataFolder,
                    Filter = "Rocket xml files (*.xml)|*.xml"
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }

                //save to SaveFileDialog.FileName here
                io.SaveCalendar_XML(_appData.ActiveRocketCalendar, saveFileDialog.FileName);

                ShowSuccessSnackbar("Your calendar was saved to a Xml file");
            }
            catch
            {
                ShowErrorSnackbar("The application failed to save your calendar to a Xml file");
            }
        }
    }
}
