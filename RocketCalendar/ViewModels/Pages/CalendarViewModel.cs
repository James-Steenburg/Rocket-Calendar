using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class CalendarViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        private GlobalAppData _appData;

        [ObservableProperty]
        private string _calendarName = "ExampleCalendarName";

        [ObservableProperty]
        private RocketCalendarModel _activeCalendar;

        [ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(GeneratedWeeks))]
        private RocketMonth _selectedRocketMonth;

        //[ObservableProperty]
        //private ObservableCollection<RocketWeekModel> _weeksCollection;


        //[ObservableProperty]
        //private ObservableCollection<RocketEvent> _currentEvents;

        [ObservableProperty]
        private DataTable _myDataTable;

        [RelayCommand]
        private void DecrementDisplayYear(object obj)
        {
            //...
        }

        [RelayCommand]
        private void IncrementDisplayYear(object obj)
        {
            //...
        }

        [RelayCommand]
        private void DecrementDisplayMonth(object cmdParameter)
        {
            int selectedIndex = (int)cmdParameter;

            if(selectedIndex > 0)
            {
                selectedIndex--;
            } 
            else if (selectedIndex == 0)
            {
                selectedIndex = ActiveCalendar.MonthCollection.Count - 1;
            }
            SelectedRocketMonth = ActiveCalendar.MonthCollection[selectedIndex];
            ActiveCalendar.CurrentMonth = selectedIndex;
            OnPropertyChanged(nameof(SelectedRocketMonth));
            OnPropertyChanged(nameof(GeneratedWeeks));
        }

        [RelayCommand]
        private void IncrementDisplayMonth(object cmdParameter)
        {
            int selectedIndex = (int)cmdParameter;

            if (selectedIndex < ActiveCalendar.MonthCollection.Count - 1)
            {
                selectedIndex++;
            }
            else if (selectedIndex == ActiveCalendar.MonthCollection.Count - 1)
            {
                selectedIndex = 0;
            }
            SelectedRocketMonth = ActiveCalendar.MonthCollection[selectedIndex];
            ActiveCalendar.CurrentMonth = selectedIndex;
            OnPropertyChanged(nameof(SelectedRocketMonth));
            OnPropertyChanged(nameof(GeneratedWeeks));
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            ActiveCalendar = _appData.ActiveRocketCalendar;
        }

        
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

        [RelayCommand]
        private void OpenEventDetails(object obj)
        {
            RocketEvent rEvent = (RocketEvent)obj;


        }

        [RelayCommand]
        private void DayClicked(object obj)
        {
            //RocketEvent rEvent = (RocketEvent)obj;


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

            _isInitialized = true;
        }

        public CalendarViewModel(GlobalAppData appData)
        {
            _appData = appData;
            ActiveCalendar = _appData.ActiveRocketCalendar;
        }
    }
}
