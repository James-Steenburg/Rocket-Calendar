using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Controls;
using Wpf.Ui;
using System.Windows.Forms.PropertyGridInternal;
using RocketCalendar.Models;
using RocketCalendar.Helpers;
using System.Windows;
using RocketCalendar.Properties;


namespace RocketCalendar.ViewModels
{
    public partial class RocketCalendarViewModel : ObservableObject
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;
        RocketCalendar.Helpers.FileIOHelper io = new RocketCalendar.Helpers.FileIOHelper();

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private bool _isStatusBarVisible;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        public RocketCalendarViewModel(INavigationService navigationService, GlobalAppData appData)
        {
            IsStatusBarVisible = false;

            _appData = appData;

            if (!_isInitialized)
                InitializeViewModel();

            if(_appData.IS_DEBUG_MODE)
            {
                //debug code here
            }
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "Rocket | Calendar";

            try
            {
                RocketCalendarModel newCalendar = io.LoadCalendar_XML(Properties.Settings.Default.DefaultCalendarFilePath);
                if (newCalendar == null)
                {
                    GeneratePlaceHolderCalendar();
                }
                else
                {
                    _appData.ActiveRocketCalendar = newCalendar;
                }    
            }
            catch
            {
                GeneratePlaceHolderCalendar();
            }


            Visibility ActiveCalendarVisibility = Visibility.Visible;
            if (_appData.ActiveRocketCalendar == null)
            {
                ActiveCalendarVisibility = Visibility.Collapsed;
            }

            NavigationItems = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                ToolTip = "Home",
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Calendar",
                Icon = new SymbolIcon { Symbol = SymbolRegular.CalendarLtr24 },
                TargetPageType = typeof(Views.Pages.CalendarPage),
                ToolTip = _appData.ActiveRocketCalendar.CalendarName,
                Visibility = ActiveCalendarVisibility
            },
            new NavigationViewItem()
            {
                Content = "Create Event",
                Icon = new SymbolIcon { Symbol = SymbolRegular.CalendarToday24 },
                TargetPageType = typeof(Views.Pages.CreateEventPage),
                ToolTip = "Create a New Event",
                Visibility = ActiveCalendarVisibility
            },
            new NavigationViewItem()
            {
                Content = "Create Calendar",
                Icon = new SymbolIcon { Symbol = SymbolRegular.CalendarAdd24 },
                ToolTip = "Create a New Calendar",
                TargetPageType = typeof(Views.Pages.CreateCalendarPage)
            },
            new NavigationViewItem()
            {
                Content = "File IO",
                Icon = new SymbolIcon { Symbol = SymbolRegular.StreamInputOutput20 },
                ToolTip = "File IO",
                TargetPageType = typeof(Views.Pages.FileIOPage)
            }
        };

            NavigationFooter = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                ToolTip = "Settings",
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

            TrayMenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };

            _isInitialized = true;
        }

        private void GeneratePlaceHolderCalendar()
        {
            _appData.ActiveRocketCalendar = new RocketCalendarModel(
                    "Example Gregorian Calendar",
                    new RocketDate(6, 2, 2, 2024),
                    new ObservableCollection<RocketMonth>()
                    {
                        new RocketMonth("January", 31),
                        new RocketMonth("February", 28, 4),
                        new RocketMonth("March", 31),
                        new RocketMonth("April", 30),
                        new RocketMonth("May", 31),
                        new RocketMonth("June", 30),
                        new RocketMonth("July", 31),
                        new RocketMonth("August", 31),
                        new RocketMonth("September", 30),
                        new RocketMonth("October", 31),
                        new RocketMonth("November", 30),
                        new RocketMonth("December", 31)
                    },
                    new ObservableCollection<string>()
                    {
                        "Sunday",
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                        "Friday",
                        "Saturday"
                    },
                    new ObservableCollection<RocketEvent>()
                    {
                        new RocketEvent(new RocketDate(25,11,100),"Christmas","Merry Christmas!",true,10,0,1)
                    },
                    2,
                    2024
                    );
        }
    }
}
