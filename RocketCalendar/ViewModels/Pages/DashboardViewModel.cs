using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketCalendar.Controls;
using RocketCalendar.Models;
using RocketCalendar.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class DashboardViewModel(IContentDialogService contentDialogService) : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }

        [RelayCommand]
        private async Task OnShowAddOrEditEventDialog()
        {
            //var editEventDialog = new EditEventDialog(contentDialogService.GetContentPresenter())
            //{
            //EdittedEvent = new RocketEvent(new RocketDate(7, 2, 2024), "Taverna Event", "Pizza Time", false, 1)
            //};

            //ContentDialogResult result = await contentDialogService.ShowSimpleDialogAsync(editEventDialog);
            //_ = await editEventDialog.ShowAsync();
            //AddOrEditEventWindow eventWindow = new AddOrEditEventWindow();
            //eventWindow.ShowDialog();


        }

        private bool _isInitialized = false;
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
