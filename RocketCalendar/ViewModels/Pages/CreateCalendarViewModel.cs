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

namespace RocketCalendar.ViewModels.Pages
{
    public partial class CreateCalendarViewModel(IContentDialogService contentDialogService) : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private bool _isFlyoutOpen = false;

        [ObservableProperty]
        private bool _isCreateCalendarButtonEnabled = true;

        [ObservableProperty]
        private bool? _isAddMonthButtonEnabled = false;
        
        [ObservableProperty]
        private string _newMonthName;

        [ObservableProperty]
        private int _newMonthDayCount;

        [ObservableProperty]
        private string _newDayName;

        [ObservableProperty]
        private RocketDate _baseDate;

        [ObservableProperty]
        private string _newCalendarName;

        [ObservableProperty]
        private ObservableCollection<RocketMonth> _monthListViewItems = new ObservableCollection<RocketMonth>();

        [ObservableProperty]
        private ObservableCollection<string> _dayNameListViewItems = new ObservableCollection<string>();

        [ObservableProperty]
        private ObservableCollection<RocketMonth> _monthListBoxItems = new ObservableCollection<RocketMonth>();

        [ObservableProperty]
        private ObservableCollection<string> _dayNameListBoxItems = new ObservableCollection<string>();

        [RelayCommand]
        private async Task CreateCalendar(object content)
        {
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


        }

        [RelayCommand]
        private void AddMonthToCollection(object obj)
        {
            //...
            MonthListViewItems.Add(new RocketMonth(NewMonthName, NewMonthDayCount));
        }

        [RelayCommand]
        private void RemoveMonthFromCollection(object obj)
        {
            if ((int)obj >= 0)
            {
                MonthListViewItems.RemoveAt((int)obj);
            }
        }

        [RelayCommand]
        private void AddDayToCollection(object obj)
        {
            //...
            DayNameListViewItems.Add(NewDayName);
        }

        [RelayCommand]
        private void RemoveDayFromCollection(object obj)
        {
            if ((int)obj >= 0)
            {
                DayNameListViewItems.RemoveAt((int)obj);
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

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            //initialization ...


            _isInitialized = true;
        }

    }
}
