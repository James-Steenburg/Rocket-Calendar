using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Controls;
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
    public partial class CalendarViewModel : ObservableObject
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
            ActiveCalendar.CurrentMonth--;
            OnPropertyChanged(nameof(GeneratedWeeks));
            OnPropertyChanged(nameof(SelectedRocketMonthIndex));
        }

        [RelayCommand] 
        private void IncrementDisplayMonth(object cmdParameter)
        {
            ActiveCalendar.CurrentMonth++;
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
        private void SaveOverEvent(RocketEvent eventToRemove) //needs rework
        {
            try
            {
                RocketEvent newEvent = ActiveCalendar.WipEventPlaceholder;

                if (!eventToRemove.IsRepeatingEvent)
                {
                    ActiveCalendar.EventCollection.Remove(eventToRemove);

                    if (!newEvent.IsRepeatingEvent)
                    {
                        ActiveCalendar.EventCollection.Add(newEvent);
                    }
                    else if (newEvent.IsRepeatingEvent)
                    {
                        ActiveCalendar.RepeatingEventCollection.Add(newEvent);
                    }
                }
                else if (eventToRemove.IsRepeatingEvent)
                {
                    ActiveCalendar.RepeatingEventCollection.Remove(eventToRemove);

                    if (!newEvent.IsRepeatingEvent)
                    {
                        ActiveCalendar.EventCollection.Add(newEvent);
                    }
                    else if (newEvent.IsRepeatingEvent)
                    {
                        ActiveCalendar.RepeatingEventCollection.Add(newEvent);
                    }
                }

                ShowSuccessSnackbar("Event updated: " + newEvent.EventName);

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

                EventDayInput = ActiveCalendar.WipEventPlaceholder.EventDate.DateDay;
                EventMonthIndexInput = ActiveCalendar.WipEventPlaceholder.EventDate.DateMonth;
                EventYearInput = ActiveCalendar.WipEventPlaceholder.EventDate.DateYear;


                AddOrEditEventWindow addOrEditEventWin = new AddOrEditEventWindow(this);
                addOrEditEventWin.ShowDialog();

            }
            else if (obj is RocketDate)
            {
                RocketEvent rEvent = new RocketEvent((RocketDate)obj, "Event Name..", "Event Description..", false, 0, 0, 0, 0);

                WindowTitle = "Rocket | Add Event Window";

                ActiveCalendar.WipEventPlaceholder = rEvent;

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
        private int _weekRepeatInterval;

        [ObservableProperty]
        private int _monthRepeatInterval;

        [ObservableProperty]
        private int _YearRepeatInterval;

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

        

        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(GeneratedWeeks))]
        //private int _selectedRocketMonthIndex;

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

        //[ObservableProperty]
        //private ObservableCollection<RocketWeekModel> _weeksCollection;

        [ObservableProperty]
        private RocketEvent _editedEvent;

        //[ObservableProperty]
        //private ObservableCollection<RocketEvent> _currentEvents;

        [ObservableProperty]
        private DataTable _myDataTable;
        private IContentDialogService _contentDialogService;

        

        
        public ObservableCollection<RocketWeekModel> GeneratedWeeks
        {
            get
            {
                var currentEvents = ActiveCalendar.EventCollection.Where(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth);

                bool checkForEvents = (ActiveCalendar.EventCollection.Any(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth));

                if(currentEvents == null)
                {
                    checkForEvents = false;
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
                            if(checkForEvents)
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

        private ObservableCollection<RocketWeekModel> GenerateEventList(ObservableCollection<RocketWeekModel> weeksCollection)
        {
            
            if (ActiveCalendar.EventCollection.Any(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth))
            {
                var ce = ActiveCalendar.EventCollection.Where(e => e.EventDate.DateYear == ActiveCalendar.CurrentYear && e.EventDate.DateMonth == ActiveCalendar.CurrentMonth);

                /*
                foreach (var item in ce)
                {
                    CurrentEvents.Add(item);
                }
                */
                foreach (var re in ce)
                {
                    for (int i = 0; i < weeksCollection.Count; i++)
                    {
                        for (int j = 0; j < weeksCollection[i].RocketWeek.Count; j++)
                        {
                            if (weeksCollection[i].RocketWeek[j].DateDay == re.EventDate.DateDay)
                            {
                                weeksCollection[i].RocketWeek[j].Events.Add(re);
                            }
                        }
                    }
                }
                
            }

            return weeksCollection;
        }

        

        

        



        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            ActiveCalendar = _appData.ActiveRocketCalendar;
            //SelectedRocketMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth];
        }

        /*
        private void GenerateMyDataTable(ObservableCollection<RocketWeekModel> weeks, int numOfWeeksToPopulate)
        {
            DataTable dt = new DataTable();

            //AddColumn<string>(dt, "type");
            for (int i = 0; i < ActiveCalendar.DayNameCollection.Count; i++)
            {
                AddColumn<string>(dt, ActiveCalendar.DayNameCollection[i].ToString());
            }

            for(int j=0; j < numOfWeeksToPopulate; j++)
            {
                var columnValues = new List<object>();
                for(int k=0; k < ActiveCalendar.DayNameCollection.Count; k++)
                {
                    columnValues.Add(weeks[j].RocketWeek[k]);
                }

                AddRow(dt, columnValues);
            }
            MyDataTable = dt;
        }

        private void AddColumn<TData>(DataTable dataTable, string columnName, int columnIndex = -1)
        {
            DataColumn newColumn = new DataColumn(columnName, typeof(TData));

            dataTable.Columns.Add(newColumn);
            if (columnIndex > -1)
            {
                newColumn.SetOrdinal(columnIndex);
            }

            int newColumnIndex = dataTable.Columns.IndexOf(newColumn);

            // Initialize existing rows with default value for the new column
            foreach (DataRow row in dataTable.Rows)
            {
                row[newColumnIndex] = default(TData);
            }
        }

        private void AddRow(DataTable dataTable, IList<object> columnValues)
        {
            DataRow rowModelWithCurrentColumns = dataTable.NewRow();
            dataTable.Rows.Add(rowModelWithCurrentColumns);

            for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
            {
                rowModelWithCurrentColumns[columnIndex] = columnValues[columnIndex];
            }
        }

        */

        /*
        
        public void GenerateWeeksCollections()
        {
            Helpers.RocketDateHelper rdh = new Helpers.RocketDateHelper();

            int daysInMonth = ActiveCalendar.MonthCollection[ActiveCalendar.CurrentMonth].NumOfDays;
            int daysInWeek = ActiveCalendar.DayNameCollection.Count;
            int firstDayWeekIndex = rdh.GetFirstDayNameIndexOfCurrentYear(ActiveCalendar, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear);

            int numOfWeeksToPopulate = (int)Math.Ceiling((double)(daysInMonth - (daysInWeek - firstDayWeekIndex)) / (double)daysInWeek) + 1;

            WeeksCollection.Clear();
            ObservableCollection<RocketDate> generatedWeek = new ObservableCollection<RocketDate>();

            int dayCellsPaddedInFirstWeek = 0;
            int daysGeneratedCount = 0;
            int daysInLastWeekGenerated = 0;

            for (int i = 0; i < numOfWeeksToPopulate; i++)
            {
                generatedWeek.Clear();

                if(i == 0)
                {
                    //generate first week
                    for (int j = 0; j < firstDayWeekIndex; j++)
                    {
                        generatedWeek.Add(new RocketDate(true));
                        dayCellsPaddedInFirstWeek++;
                    }
                    for (int k = dayCellsPaddedInFirstWeek; k < daysInWeek; k++)
                    {
                        generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                        daysGeneratedCount++;
                    }
                }
                else if(i == numOfWeeksToPopulate - 1)
                {
                    //generating the last week..
                    for (int l = daysGeneratedCount;  l < daysInMonth; l++)
                    {
                        generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
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
                        generatedWeek.Add(new RocketDate(daysGeneratedCount + 1, ActiveCalendar.CurrentMonth, ActiveCalendar.CurrentYear));
                        daysGeneratedCount++;
                    }
                }

                WeeksCollection.Add(new RocketWeekModel(generatedWeek));
            }
        }

        */

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
