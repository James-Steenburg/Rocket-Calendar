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
 */


namespace RocketCalendar.ViewModels
{
    public partial class RocketCalendarViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        public RocketCalendarViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
                InitializeViewModel();
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
