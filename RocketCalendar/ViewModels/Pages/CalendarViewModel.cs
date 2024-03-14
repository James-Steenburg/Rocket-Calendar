using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
using RocketCalendar.Services;
using RocketCalendar.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class CalendarViewModel : ObservableObject, INavigationAware
    {
        #region Properties


        #endregion Properties

        #region Relay Commands

        [RelayCommand]
        private void DecrementDisplayYear(object obj)
        {
            ActiveCalendar.CurrentYear--;
            OnPropertyChanged(nameof(GeneratedWeeks));
        }

        [RelayCommand]
        private void IncrementDisplayYear(object obj)
        {
            ActiveCalendar.CurrentYear++;
            OnPropertyChanged(nameof(GeneratedWeeks));
        }

        [RelayCommand]
        private void DecrementDisplayMonth(object cmdParameter)
        {
            if (ActiveCalendar.CurrentMonth > 0)
            {
                ActiveCalendar.CurrentMonth--;
            }
            else
            {
                ActiveCalendar.CurrentMonth = ActiveCalendar.MonthCollection.Count - 1;
                ActiveCalendar.CurrentYear--;
            }

            OnPropertyChanged(nameof(GeneratedWeeks));
            OnPropertyChanged(nameof(SelectedRocketMonthIndex));
        }

        [RelayCommand] 
        private void IncrementDisplayMonth(object cmdParameter)
        {
            if (ActiveCalendar.CurrentMonth < ActiveCalendar.MonthCollection.Count - 1)
            {
                ActiveCalendar.CurrentMonth++;
            }
            else
            {
                ActiveCalendar.CurrentMonth = 0;
                ActiveCalendar.CurrentYear++;
            }
            
            OnPropertyChanged(nameof(GeneratedWeeks));
            OnPropertyChanged(nameof(SelectedRocketMonthIndex));
        }

        [RelayCommand]
        private async void OpenEventDetails(object obj)
        {
            RocketEvent rEvent = (RocketEvent)obj;
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = rEvent.EventName,
                Content = rEvent.EventDescription,
                PrimaryButtonText = "Edit",
                SecondaryButtonText = "Delete",
                SecondaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Danger
            };

            var result = await uiMessageBox.ShowDialogAsync();

            switch (result.ToString())
            {
                case "Primary":
                    OpenAddOrEditEventWindow(rEvent);
                    break;
                case "Secondary":
                    DeleteEvent(rEvent);
                    break;
                default:
                    break;
            }

        }

        [RelayCommand]
        private void DayClicked(object obj)
        {

            OpenAddOrEditEventWindow((RocketDate)obj);

        }

        [RelayCommand]
        private async void DeleteEvent(object eventToDelete)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Delete Event",
                Content = "Are you sure you want to delete this event?",
                PrimaryButtonText = "Delete",
                PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Danger
            };

            var result = await uiMessageBox.ShowDialogAsync();

            if (result.ToString() == "Primary")
            {
                RocketEvent rEvent = (RocketEvent)eventToDelete;

                if (ActiveCalendar.EventCollection.Any(e => e == rEvent))
                {
                    ActiveCalendar.EventCollection.RemoveAt(ActiveCalendar.EventCollection.IndexOf(rEvent));
                    OnPropertyChanged(nameof(GeneratedWeeks));
                }
            }
        }

        [RelayCommand]
        private void SaveOverEvent(RocketEvent eventToRemove)
        {
            try
            {
                if (EventIndexBeingEditted != null && EventIndexBeingEditted >= 0 && EventIndexBeingEditted < ActiveCalendar.EventCollection.Count)
                {
                    //Remove EdittedEvent
                    ActiveCalendar.EventCollection.RemoveAt((int)EventIndexBeingEditted);

                }

                //Add new event
                ActiveCalendar.EventCollection.Add(ActiveCalendar.WipEventPlaceholder);
                OnPropertyChanged(nameof(GeneratedWeeks));

                ShowSuccessSnackbar("Event updated: " + ActiveCalendar.WipEventPlaceholder.EventName);

            }
            catch
            {
                ShowErrorSnackbar("The application failed to update your event.");
            }
        }

        [RelayCommand]
        private async Task OpenAddOrEditEventWindow(object obj)
        {
            if (obj is RocketEvent)
            {
                EditedEvent = (RocketEvent)obj;

                WindowTitle = "Rocket | Edit Event Window";
                ActiveCalendar.WipEventPlaceholder = EditedEvent;

                EventIndexBeingEditted = ActiveCalendar.EventCollection.IndexOf(EditedEvent);

                AddOrEditEventWindow addOrEditEventWin = new AddOrEditEventWindow(this);
                addOrEditEventWin.ShowDialog();

            }
            else if (obj is RocketDate)
            {
                RocketEvent rEvent = new RocketEvent((RocketDate)obj, "Event Name..", "Event Description..", false, 0, 0, 0);

                WindowTitle = "Rocket | Add Event Window";

                ActiveCalendar.WipEventPlaceholder = rEvent;

                EventIndexBeingEditted = null;

                AddOrEditEventWindow addOrEditEventWin = new AddOrEditEventWindow(this);
                addOrEditEventWin.ShowDialog();
            }
        }

        #endregion Relay Commands

        #region Support Functions

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

        #endregion Support Functions


        private bool _isInitialized = false;

        private GlobalAppData _appData;

        private WindowsProviderService _windowsProviderService;
        private ISnackbarService _snackbarService;
        private IContentDialogService _contentDialogService;


        [ObservableProperty]
        private RocketCalendarModel _activeCalendar;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GeneratedWeeks))]
        private RocketMonth _selectedRocketMonth;

        [ObservableProperty]
        private string _windowTitle;

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

        [ObservableProperty]
        private int _monthRepeatInterval;

        [ObservableProperty]
        private int _YearRepeatInterval;

        [ObservableProperty]
        private int? _eventIndexBeingEditted;


        [ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        private int _eventDayInput;

        [ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        [NotifyPropertyChangedFor(nameof(SelectedInputMonthMaxDays))]
        private int _eventMonthIndexInput;

        [ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(IsCreateEventButtonEnabled))]
        private int _eventYearInput;

        public int SelectedInputMonthMaxDays
        {
            get
            {
                if (ActiveCalendar.MonthCollection.Count > 0 && EventMonthIndexInput >= 0)
                {
                    if (EventDayInput > ActiveCalendar.MonthCollection[EventMonthIndexInput].NumOfDays)
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

        public int SelectedRocketMonthIndex
        {
            get 
            { 
                return ActiveCalendar.CurrentMonth; 
            }
            set 
            { 
                if(value < ActiveCalendar.MonthCollection.Count && value >= 0)
                {
                    //_selectedRocketMonthIndex = value;
                    ActiveCalendar.CurrentMonth = value;
                    OnPropertyChanged(nameof(SelectedRocketMonthIndex));
                    OnPropertyChanged(nameof(GeneratedWeeks));
                }
            }
        }

        [ObservableProperty]
        private RocketEvent _editedEvent;

        //[ObservableProperty]
        //private ObservableCollection<RocketEvent> _currentEvents;

        //[ObservableProperty]
        //private DataTable _myDataTable;

        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(GeneratedWeeks))]
        //private int _selectedRocketMonthIndex;

        //[ObservableProperty]
        //private ObservableCollection<RocketWeekModel> _weeksCollection;

        public ObservableCollection<RocketWeekModel> GeneratedWeeks
        {
            get
            {
                var currentEvents = ActiveCalendar.EventCollection.Where(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth);

                bool checkForEvents = ActiveCalendar.EventCollection.Any(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth);

                if(currentEvents == null)
                {
                    checkForEvents = false;
                }

                var currentRepeatingEvents = ActiveCalendar.EventCollection.Where(e => e.IsRepeatingEvent == true);

                bool checkForRepeatingEvents = ActiveCalendar.EventCollection.Any(e => e.IsRepeatingEvent == true);

                if (currentRepeatingEvents == null)
                {
                    checkForRepeatingEvents = false;
                }

                ObservableCollection<RocketWeekModel> rocketWeeksCollection = new ObservableCollection<RocketWeekModel>();

                Helpers.RocketDateHelper rdh = new Helpers.RocketDateHelper();

                int daysInMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth].NumOfDays;

                if (ActiveCalendar.CurrentYear % ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth].LeapYearInterval == 0)
                {
                    daysInMonth++;
                }

                int daysInWeek = ActiveCalendar.DayNameCollection.Count;
                int firstDayWeekIndex = rdh.GetFirstDayNameIndexOfCurrentYear(ActiveCalendar, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear);

                int numOfWeeksToPopulate = (int)Math.Ceiling((double)(daysInMonth - (daysInWeek - firstDayWeekIndex)) / (double)daysInWeek) + 1;

                if(rocketWeeksCollection != null) { rocketWeeksCollection.Clear(); }
                
                int dayCellsPaddedInFirstWeek = 0;
                int daysGeneratedCount = 0;
                int daysInLastWeekGenerated = 0;



                for (int i = 0; i < numOfWeeksToPopulate; i++)
                {
                    ObservableCollection<RocketDate> generatedWeek = new ObservableCollection<RocketDate>();

                    generatedWeek.Clear();

                    if (i == 0)
                    {
                        //generate first week
                        for (int j = 0; j < firstDayWeekIndex; j++)
                        {
                            generatedWeek.Add(new RocketDate(true));
                            dayCellsPaddedInFirstWeek++;
                        }
                        for (int k = dayCellsPaddedInFirstWeek; k < daysInWeek; k++)
                        {
                            if (checkForEvents || checkForRepeatingEvents)
                            {
                                ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                                if (checkForEvents)
                                {
                                    foreach (var rEvent in currentEvents)
                                    {
                                        if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && !rEvent.IsRepeatingEvent)
                                        {
                                            eList.Add(rEvent);
                                        }
                                    }
                                }
                                if (checkForRepeatingEvents)
                                {
                                    //Add repeating events to list
                                    foreach (var repeatingEvent in currentRepeatingEvents)
                                    {
                                        if (daysGeneratedCount + 1 == repeatingEvent.EventDate.DateDay)
                                        {
                                            //Check if it applies
                                            if (rdh.DoesRepeatingEventApplyToDate(ActiveCalendar, repeatingEvent, new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear)))
                                            {
                                                eList.Add(repeatingEvent);
                                            }
                                        }
                                    }
                                }

                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                            }

                            /*
                             if (checkForEvents)
                            {
                                ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                                foreach (var rEvent in currentEvents)
                                {
                                    if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && ActiveCalendar.CurrentMonth == rEvent.EventDate.DateMonth)
                                    {
                                        eList.Add(rEvent);
                                    }
                                }
                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                            }
                             */

                            else
                            {
                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                            }
                            
                            daysGeneratedCount++;
                        }
                    }
                    else if (i == numOfWeeksToPopulate - 1)
                    {
                        //generating the last week..
                        for (int l = daysGeneratedCount; l < daysInMonth; l++)
                        {
                            if (checkForEvents || checkForRepeatingEvents)
                            {
                                ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                                if (checkForEvents)
                                {
                                    foreach (var rEvent in currentEvents)
                                    {
                                        if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && !rEvent.IsRepeatingEvent)
                                        {
                                            eList.Add(rEvent);
                                        }
                                    }
                                }
                                if (checkForRepeatingEvents)
                                {
                                    //Add repeating events to list
                                    foreach (var repeatingEvent in currentRepeatingEvents)
                                    {
                                        if(daysGeneratedCount + 1 == repeatingEvent.EventDate.DateDay)
                                        {
                                            //Check if it applies
                                            if (rdh.DoesRepeatingEventApplyToDate(ActiveCalendar, repeatingEvent, new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear)))
                                            {
                                                eList.Add(repeatingEvent);
                                            }
                                        }
                                    }
                                }
                                
                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                            }
                            else
                            {
                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                            }
                            daysGeneratedCount++;
                            daysInLastWeekGenerated++;
                        }
                        for (int m = daysInLastWeekGenerated; m < daysInWeek; m++)
                        {
                            generatedWeek.Add(new RocketDate(true));
                        }
                    }
                    else
                    {
                        //generating full weeks
                        for (int n = 0; n < daysInWeek; n++)
                        {
                            if (checkForEvents || checkForRepeatingEvents)
                            {
                                ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                                if (checkForEvents)
                                {
                                    foreach (var rEvent in currentEvents)
                                    {
                                        if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && !rEvent.IsRepeatingEvent)
                                        {
                                            eList.Add(rEvent);
                                        }
                                    }
                                }
                                if (checkForRepeatingEvents)
                                {
                                    //Add repeating events to list
                                    foreach (var repeatingEvent in currentRepeatingEvents)
                                    {
                                        if (daysGeneratedCount + 1 == repeatingEvent.EventDate.DateDay)
                                        {
                                            //Check if it applies
                                            if (rdh.DoesRepeatingEventApplyToDate(ActiveCalendar, repeatingEvent, new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear)))
                                            {
                                                eList.Add(repeatingEvent);
                                            }
                                        }
                                    }
                                }

                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                            }

                            /*
                             if (checkForEvents)
                            {
                                ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                                foreach (var rEvent in currentEvents)
                                {
                                    if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay)
                                    {
                                        eList.Add(rEvent);
                                    }
                                }
                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                            }
                             */

                            else
                            {
                                generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                            }
                            daysGeneratedCount++;
                        }
                    }


                    rocketWeeksCollection.Add(new RocketWeekModel(generatedWeek));
                }

                // GenerateMyDataTable(rocketWeeksCollection, numOfWeeksToPopulate);
                //rocketWeeksCollection = GenerateEventList(rocketWeeksCollection);
                return rocketWeeksCollection; 
            }
        }

        private async Task<ObservableCollection<RocketWeekModel>> GeneratedWeeksTask()
        {
            //progress bar true
            //ObservableCollection<RocketWeekModel> weeks = new ObservableCollection<RocketWeekModel>();
            //weeks = GetGenerateWeeks();
            var weeks = await Task.Run(() => GetGenerateWeeks());

            return weeks;
            //progress bar false



            //or whatever private async void calls this will need to control the status bar and await getting the obs collection from here.
        }

        private ObservableCollection<RocketWeekModel> GetGenerateWeeks()
        {
            var currentEvents = ActiveCalendar.EventCollection.Where(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth);

            bool checkForEvents = ActiveCalendar.EventCollection.Any(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth);

            if (currentEvents == null)
            {
                checkForEvents = false;
            }

            var currentRepeatingEvents = ActiveCalendar.EventCollection.Where(e => e.IsRepeatingEvent == true);

            bool checkForRepeatingEvents = ActiveCalendar.EventCollection.Any(e => e.IsRepeatingEvent == true);

            if (currentRepeatingEvents == null)
            {
                checkForRepeatingEvents = false;
            }

            ObservableCollection<RocketWeekModel> rocketWeeksCollection = new ObservableCollection<RocketWeekModel>();

            Helpers.RocketDateHelper rdh = new Helpers.RocketDateHelper();

            int daysInMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth].NumOfDays;

            if (ActiveCalendar.CurrentYear % ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth].LeapYearInterval == 0)
            {
                daysInMonth++;
            }

            int daysInWeek = ActiveCalendar.DayNameCollection.Count;
            int firstDayWeekIndex = rdh.GetFirstDayNameIndexOfCurrentYear(ActiveCalendar, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear);

            int numOfWeeksToPopulate = (int)Math.Ceiling((double)(daysInMonth - (daysInWeek - firstDayWeekIndex)) / (double)daysInWeek) + 1;

            if (rocketWeeksCollection != null) { rocketWeeksCollection.Clear(); }

            int dayCellsPaddedInFirstWeek = 0;
            int daysGeneratedCount = 0;
            int daysInLastWeekGenerated = 0;



            for (int i = 0; i < numOfWeeksToPopulate; i++)
            {
                ObservableCollection<RocketDate> generatedWeek = new ObservableCollection<RocketDate>();

                generatedWeek.Clear();

                if (i == 0)
                {
                    //generate first week
                    for (int j = 0; j < firstDayWeekIndex; j++)
                    {
                        generatedWeek.Add(new RocketDate(true));
                        dayCellsPaddedInFirstWeek++;
                    }
                    for (int k = dayCellsPaddedInFirstWeek; k < daysInWeek; k++)
                    {
                        if (checkForEvents || checkForRepeatingEvents)
                        {
                            ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                            if (checkForEvents)
                            {
                                foreach (var rEvent in currentEvents)
                                {
                                    if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && !rEvent.IsRepeatingEvent)
                                    {
                                        eList.Add(rEvent);
                                    }
                                }
                            }
                            if (checkForRepeatingEvents)
                            {
                                //Add repeating events to list
                                foreach (var repeatingEvent in currentRepeatingEvents)
                                {
                                    if (daysGeneratedCount + 1 == repeatingEvent.EventDate.DateDay)
                                    {
                                        //Check if it applies
                                        if (rdh.DoesRepeatingEventApplyToDate(ActiveCalendar, repeatingEvent, new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear)))
                                        {
                                            eList.Add(repeatingEvent);
                                        }
                                    }
                                }
                            }

                            generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                        }

                        else
                        {
                            generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                        }

                        daysGeneratedCount++;
                    }
                }
                else if (i == numOfWeeksToPopulate - 1)
                {
                    //generating the last week..
                    for (int l = daysGeneratedCount; l < daysInMonth; l++)
                    {
                        if (checkForEvents || checkForRepeatingEvents)
                        {
                            ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                            if (checkForEvents)
                            {
                                foreach (var rEvent in currentEvents)
                                {
                                    if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && !rEvent.IsRepeatingEvent)
                                    {
                                        eList.Add(rEvent);
                                    }
                                }
                            }
                            if (checkForRepeatingEvents)
                            {
                                //Add repeating events to list
                                foreach (var repeatingEvent in currentRepeatingEvents)
                                {
                                    if (daysGeneratedCount + 1 == repeatingEvent.EventDate.DateDay)
                                    {
                                        //Check if it applies
                                        if (rdh.DoesRepeatingEventApplyToDate(ActiveCalendar, repeatingEvent, new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear)))
                                        {
                                            eList.Add(repeatingEvent);
                                        }
                                    }
                                }
                            }

                            generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                        }
                        else
                        {
                            generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                        }
                        daysGeneratedCount++;
                        daysInLastWeekGenerated++;
                    }
                    for (int m = daysInLastWeekGenerated; m < daysInWeek; m++)
                    {
                        generatedWeek.Add(new RocketDate(true));
                    }
                }
                else
                {
                    //generating full weeks
                    for (int n = 0; n < daysInWeek; n++)
                    {
                        if (checkForEvents || checkForRepeatingEvents)
                        {
                            ObservableCollection<RocketEvent> eList = new ObservableCollection<RocketEvent>();
                            if (checkForEvents)
                            {
                                foreach (var rEvent in currentEvents)
                                {
                                    if (daysGeneratedCount + 1 == rEvent.EventDate.DateDay && !rEvent.IsRepeatingEvent)
                                    {
                                        eList.Add(rEvent);
                                    }
                                }
                            }
                            if (checkForRepeatingEvents)
                            {
                                //Add repeating events to list
                                foreach (var repeatingEvent in currentRepeatingEvents)
                                {
                                    if (daysGeneratedCount + 1 == repeatingEvent.EventDate.DateDay)
                                    {
                                        //Check if it applies
                                        if (rdh.DoesRepeatingEventApplyToDate(ActiveCalendar, repeatingEvent, new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear)))
                                        {
                                            eList.Add(repeatingEvent);
                                        }
                                    }
                                }
                            }

                            generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear, eList));
                        }

                        else
                        {
                            generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                        }
                        daysGeneratedCount++;
                    }
                }


                rocketWeeksCollection.Add(new RocketWeekModel(generatedWeek));
            }

            return rocketWeeksCollection;
        }
        
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            ActiveCalendar = _appData.ActiveRocketCalendar;
            OnPropertyChanged(nameof(GeneratedWeeks));
            //SelectedRocketMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth];
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
            ActiveCalendar = _appData.ActiveRocketCalendar;
            //SelectedRocketMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth];
            _isInitialized = true;
        }

        public CalendarViewModel(GlobalAppData appData, ISnackbarService snackbarService, IContentDialogService contentDialogService, WindowsProviderService windowsProviderService)
        {
            _appData = appData;
            ActiveCalendar = _appData.ActiveRocketCalendar;
            _contentDialogService = contentDialogService;
            _windowsProviderService = windowsProviderService;
            _snackbarService = snackbarService;
            //SelectedRocketMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth];
        }

        
    }
}
