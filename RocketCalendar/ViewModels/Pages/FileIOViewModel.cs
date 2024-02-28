using CommunityToolkit.Mvvm.ComponentModel;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class FileIOViewModel : ObservableObject
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;

        public FileIOViewModel(GlobalAppData appData)
        {
            _appData = appData;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
            
            //pull whatever is needed from AppData here
        }

        public void OnNavigatedFrom()
        {
            //save anything needed in AppData - shouldn't need anything saved in this vm
        }

        private void InitializeViewModel()
        {
            //initialization ...

            _isInitialized = true;
        }
    }
}
