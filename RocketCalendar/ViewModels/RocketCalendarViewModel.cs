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


/*
 Notes:
    Views: 
        Home
        Calendar 1
            Setting options to delete, duplicate
            Right Click Calendar Date to create event
            Select Event to open a ui.simpleContentDialog
                btn 1 > edit event
                btn 2 > delete event? Remove and add delete option to edit portion?
                btn 3 > close
        Calendar 2
        Calendar n
        Create Calendar
        Create Event
        
        File IO >>> or should this be under calendar options and not a view? Both? Need an import and base file path folder setting?
            XML 
            Excel
            Txt
        Settings

    Saving Calendar Paths
        Concatenated List saved in variable: 
            Properties.Settings.Default.CalendarPathConcatenatedList = "";
                Delimiter with {Path.PathSeparator} aka ";" 
                    Ex: Console.WriteLine($"Path.PathSeparator: '{Path.PathSeparator}'");




TO DO:
    add leap year control when creating calendar months
    add Calendar Name to RocketCalendarModel
    Move Day and Calendar controls to custom controls: https://www.youtube.com/watch?v=t8zB_SYGOF0
    Update models to have oberservable properties to reduce variables needed

    Maybe?
    Change Navigation items to different calendars, settings and create calendar
        Create Event will be moved to an external window.
 */


namespace RocketCalendar.ViewModels
{
    public partial class RocketCalendarViewModel : ObservableObject
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        public RocketCalendarViewModel(INavigationService navigationService, GlobalAppData appData)
        {
            if (!_isInitialized)
                InitializeViewModel();

            _appData = appData;

            if(_appData.IS_DEBUG_MODE)
            {
                _appData.ActiveRocketCalendar = new RocketCalendarModel(
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
                        new RocketEvent(new RocketDate(1,1,1),"bday","it was my bday",false,2),
                        new RocketEvent(new RocketDate(3,2,2024),"Today","Just playing God of War",false,2),
                        new RocketEvent(new RocketDate(3,2,2024),"Jessie","Just playing God of War",false,8),
                        new RocketEvent(new RocketDate(3,2,2024),"Played","Just playing God of War",false,9),
                        new RocketEvent(new RocketDate(3,2,2024),"Kratos","Just playing God of War",true,10),
                        new RocketEvent(new RocketDate(5,6,2024),"Bday","wow 30",true,10)
                    },
                    2,
                    2024
                    );

                Helpers.RocketDateHelper rdh = new RocketDateHelper();


                //Validation:
                //Base Date: Monday December 25th, 2023 --> RocketDate(1,25,11,2023) --> correct
                //Base Date 2: Wednesday Feb 21st, 2024 --> RocketDate(3,21,1,2024) --> correct
                //Base Date 3: Saturday March 2nd, 2024 --> RocketDate(6,2,2,2024) --> correct

                //July 1st 1975 - Tuesday 
                string shouldBeTuesday = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 6, 1975));

                //July 1st 2010 - Thursday 
                string shouldBeThursday2 = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 6, 2010));

                //Jan 1 2024 - Monday
                string shouldBeMonday2 = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 0, 2024));

                //Sept 1st 2023 - Friday
                string shouldBeFriday = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 8, 2023));

                //Dec 1st 2023 - Friday
                string shouldBeFriday2 = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 11, 2023));

                //Feb 1st 2024 - Thursday
                string shouldBeThursday = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 1, 2024));

                //July 1st 2024 - Monday
                string shouldBeMonday = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 6, 2024));

                //July 1st 2091 - Sunday 
                string shouldBeSunday = rdh.ConvertDayIndexToString(rdh.GetFirstDayNameIndexOfCurrentYear(_appData.ActiveRocketCalendar, 6, 2091));

                string check = shouldBeTuesday + shouldBeFriday + shouldBeFriday2 + shouldBeThursday + shouldBeMonday + shouldBeThursday2 + shouldBeMonday2 + shouldBeSunday; 
            }
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "Rocket Calendar";

            NavigationItems = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            /*
            new NavigationViewItem()
            {
                Content = "Image Simulation",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ImageMultiple24 },
                TargetPageType = typeof(Views.Pages.ImagesDisplayPage)
            },
            */
            new NavigationViewItem()
            {
                Content = "Calendar",
                Icon = new SymbolIcon { Symbol = SymbolRegular.CalendarLtr24 },
                TargetPageType = typeof(Views.Pages.CalendarPage),
                ToolTip = "Calendar Name"
            },
            new NavigationViewItem()
            {
                Content = "Create Event",
                Icon = new SymbolIcon { Symbol = SymbolRegular.CalendarToday24 },
                TargetPageType = typeof(Views.Pages.CreateEventPage)
            },
            new NavigationViewItem()
            {
                Content = "Create Calendar",
                Icon = new SymbolIcon { Symbol = SymbolRegular.CalendarAdd24 },
                TargetPageType = typeof(Views.Pages.CreateCalendarPage)
            }
        };

            NavigationFooter = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

            TrayMenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };

            _isInitialized = true;
        }
    }
}
