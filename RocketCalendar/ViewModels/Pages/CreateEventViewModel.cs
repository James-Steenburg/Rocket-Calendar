using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using Wpf.Ui;
using System.Drawing;
using System.Windows.Media;
using System.Media;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class CreateEventViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;

        private ISnackbarService _snackbarService;

        [ObservableProperty]
        private RocketCalendarModel _activeCalendar;

        [ObservableProperty]
        private bool _isPrivateEvent = false;

        [ObservableProperty]
        private string? _eventTitle;

        [ObservableProperty]
        private string? _eventDescription;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        [NotifyPropertyChangedFor(nameof(SelectedInputMonthMaxDays))]
        private int _eventMonthIndexInput;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        private int _eventDayInput;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        private int _eventYearInput;

        //[ObservableProperty]
        //private bool _isRepeatingEvent;

        [ObservableProperty]
        private int _weekRepeatInterval;

        [ObservableProperty]
        private int _monthRepeatInterval;

        [ObservableProperty]
        private int _yearRepeatInterval;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        private ObservableCollection<RocketMonth> _monthListViewItems = new ObservableCollection<RocketMonth>()
        {
            new RocketMonth("July", 31), new RocketMonth("August", 11), new RocketMonth("Month", 4)
        };

        [ObservableProperty]
        private ObservableCollection<RocketBrush> _rocketBrushList = new ObservableCollection<RocketBrush>()
        {
            new RocketBrush(System.Windows.Media.Brushes.Transparent, "Transparent"),
            new RocketBrush(System.Windows.Media.Brushes.DarkGray, "Dark Gray"),
            new RocketBrush(System.Windows.Media.Brushes.Firebrick, "Firebrick"),
            new RocketBrush(System.Windows.Media.Brushes.Gold, "Gold"),
            new RocketBrush(System.Windows.Media.Brushes.LightGoldenrodYellow, "Light Yellow Goldenrod"),
            new RocketBrush(System.Windows.Media.Brushes.LightGray, "Light Gray"),
            new RocketBrush(System.Windows.Media.Brushes.LightSkyBlue, "Light Sky Blue"),
            new RocketBrush(System.Windows.Media.Brushes.MediumAquamarine, "Medium Aquamarine"),
            new RocketBrush(System.Windows.Media.Brushes.MediumBlue, "Medium Blue"),
            new RocketBrush(System.Windows.Media.Brushes.MediumOrchid, "Medium Orchid"),
            new RocketBrush(System.Windows.Media.Brushes.MediumPurple, "Medium Purple"),
            new RocketBrush(System.Windows.Media.Brushes.MediumSeaGreen, "Medium Sea Green"),
            new RocketBrush(System.Windows.Media.Brushes.MediumSlateBlue, "Medium Slate Blue"),
            new RocketBrush(System.Windows.Media.Brushes.MediumSpringGreen, "Medium Spring Green"),
            new RocketBrush(System.Windows.Media.Brushes.MediumTurquoise, "Medium Turquoise"),
            new RocketBrush(System.Windows.Media.Brushes.MediumVioletRed, "Medium Violet Red"),
            new RocketBrush(System.Windows.Media.Brushes.Pink, "Pink"),
            new RocketBrush(System.Windows.Media.Brushes.Salmon, "Salmon"),
            new RocketBrush(System.Windows.Media.Brushes.SeaGreen, "Sea Green"),
            new RocketBrush(System.Windows.Media.Brushes.Tomato, "Tomato")
        };

        [ObservableProperty]
        private int _selectedBrushIndex;

        public int SelectedInputMonthMaxDays
        {
            get
            {
                if(ActiveCalendar.MonthCollection.Count > 0 && EventMonthIndexInput >= 0)
                {
                    if(EventDayInput > ActiveCalendar.MonthCollection[EventMonthIndexInput].NumOfDays)
                    {
                        EventDayInput = ActiveCalendar.MonthCollection[EventMonthIndexInput].NumOfDays;
                    }
                    return ActiveCalendar.MonthCollection[EventMonthIndexInput].NumOfDays;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool IsCreateEventButtonEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(EventTitle)
                    && ActiveCalendar.MonthCollection.Count > 0
                    && EventDayInput >= 0
                    && EventMonthIndexInput >= 0
                    && EventDayInput > 0;

                //null check for event desc or throw a message asking if no desc is intentional
            }
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
        private void CreateEvent(object content)
        {
            try
            {
                RocketDate eventDate = new RocketDate(1, EventDayInput, EventMonthIndexInput, EventYearInput);
                RocketEvent newEvent = new RocketEvent(
                    eventDate,
                    EventTitle,
                    EventDescription,
                    IsPrivateEvent,
                    SelectedBrushIndex, 
                    WeekRepeatInterval, 
                    MonthRepeatInterval, 
                    YearRepeatInterval
                    );

                if(!newEvent.IsRepeatingEvent)
                {
                    ActiveCalendar.EventCollection.Add(newEvent);
                }
                else if(newEvent.IsRepeatingEvent)
                {
                    ActiveCalendar.RepeatingEventCollection.Add(newEvent);
                }

                

                SnackbarAppearanceComboBoxSelectedIndex = 3;
                _snackbarService.Show(
                "Event Added!",
                EventTitle + " event added to your calendar: " + ActiveCalendar.CalendarName,
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
                "The application failed to add your event to your calendar.",
                _snackbarAppearance,
                new SymbolIcon(SymbolRegular.ErrorCircle24),
                TimeSpan.FromSeconds(3)
                );
            }

            
            //Clear/Dispose of CreateEvent V/VM?
        }

        public CreateEventViewModel(GlobalAppData appData, ISnackbarService snackbarService)
        {
            _appData = appData;
            _snackbarService = snackbarService;
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
            //May want to change to only save event list
            _appData.ActiveRocketCalendar = ActiveCalendar;
        }

        private void InitializeViewModel()
        {
            //initialization ...


            _isInitialized = true;
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
    }
}
