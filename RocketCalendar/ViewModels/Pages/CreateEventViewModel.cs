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

        [ObservableProperty]
        private Models.RocketCalendar _activeCalendar;

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
                if (MonthListViewItems.Count > 0 && EventMonthIndexInput >= 0)
                {
                    if (EventDayInput > MonthListViewItems[EventMonthIndexInput].NumOfDays)
                    {
                        EventDayInput = MonthListViewItems[EventMonthIndexInput].NumOfDays;
                    }
                    return MonthListViewItems[EventMonthIndexInput].NumOfDays;
                }
                else
                {
                    return 1;
                }
            }
        }

        public bool IsCreateEventButtonEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(EventTitle)
                    && MonthListViewItems.Count > 0
                    && EventDayInput >= 0
                    && EventMonthIndexInput >= 0
                    && EventDayInput > 0;

                //null check for event desc or throw a message asking if no desc is intentional
            }
        }

        [RelayCommand]
        private void CreateEvent(object content)
        {
            //Todo: Add functionality to get eventDayNameIndex for eventDate

            RocketDate eventDate = new RocketDate(1, EventDayInput, EventMonthIndexInput, EventYearInput);
            RocketEvent newEvent = new RocketEvent(
                eventDate,
                EventTitle,
                EventDescription,
                IsPrivateEvent,
                SelectedBrushIndex
                );


            //Add Event to Calendar
            //Change Nav
            //Clear/Dispose of CreateEvent V/VM?
            //Snackbar acknowledging new event created
        }

        public CreateEventViewModel(GlobalAppData appData)
        {
            _appData = appData;
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
    }
}
