using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RocketCalendar.ViewModels.Pages
{
    public partial class FileIOViewModel : ObservableObject
    {
        private bool _isInitialized = false;
        private GlobalAppData _appData;
        private ISnackbarService _snackbarService;
        RocketCalendar.Helpers.FileIOHelper io = new RocketCalendar.Helpers.FileIOHelper();

        [ObservableProperty]
        private bool _includePrivateEvents;

        //unused?
        [ObservableProperty]
        private string _rocketEventListFileName;

        //unused?
        [ObservableProperty]
        private string _rocketCalendarFileName;

        #region SnackbarService Support

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

        #endregion SnackbarService Support


        public FileIOViewModel(GlobalAppData appData, ISnackbarService snackbarService)
        {
            _appData = appData;
            _snackbarService = snackbarService;
            
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

        
        [RelayCommand]
        private void ExportEventListToExcel()
        {
            try
            {
                //Save Event List To Excel here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = _appData.ActiveRocketCalendar.CalendarName,
                    InitialDirectory = appDataFolder,
                    Filter = "Excel files (*.xlsx;*.xlsm;*.xlsb;*.xltx)|*.xlsx;*.xlsm;*.xlsb;*.xltx|All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }


                if (IncludePrivateEvents)
                {
                    io.SaveEventList_Excel(_appData.ActiveRocketCalendar.EventCollection, saveFileDialog.FileName);
                }
                else
                {
                    ObservableCollection<RocketEvent> publicEventsCollection = new ObservableCollection<RocketEvent>();
                    var publicEvents = _appData.ActiveRocketCalendar.EventCollection.Where(e => e.IsPrivate == false);
                    foreach (var item in publicEvents)
                    {
                        publicEventsCollection.Add(item);
                    }
                    io.SaveEventList_Excel(publicEventsCollection, saveFileDialog.FileName);
                }

                ShowSuccessSnackbar("Your event list was saved to an Excel file");
            }
            catch
            {
                ShowErrorSnackbar("The application failed to save your event list to an Excel file");
            }
        }

        [RelayCommand]
        private void ImportEventListFromExcel()
        {
            try
            {
                //Import Event List from Excel here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.OpenFileDialog openFileDialog = new()
                {
                    InitialDirectory = appDataFolder,
                    Filter = "Excel files (*.xlsx;*.xlsm;*.xlsb;*.xltx)|*.xlsx;*.xlsm;*.xlsb;*.xltx|All files (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() != true)
                {
                    return;
                }

                var newEventList = io.LoadEventList_Excel(openFileDialog.FileName);

                if (newEventList != null)
                {
                    var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Event List Changes",
                        Content = "Do you want to the loaded event list to be added to or overwrite your current event list?",
                        PrimaryButtonText = "Add",
                        PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Primary,
                        SecondaryButtonText = "Overwrite",
                        SecondaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Danger
                    };

                    var result = uiMessageBox.ShowDialogAsync();

                    
                     if (result.Result.ToString() == "Primary")
                    {
                        foreach (RocketEvent e in newEventList)
                        {
                            _appData.ActiveRocketCalendar.EventCollection.Add(e);
                        }
                        ShowSuccessSnackbar("Your event list was imported from a Xml file");
                    }
                    else if (result.Result.ToString() == "Secondary")
                    {
                        _appData.ActiveRocketCalendar.EventCollection.Clear();
                        foreach (RocketEvent e in newEventList)
                        {
                            _appData.ActiveRocketCalendar.EventCollection.Add(e);
                        }
                        ShowSuccessSnackbar("Your event list was imported from a Xml file");
                    }
                }
                else
                {
                    ShowErrorSnackbar("The application failed to import your event list from an Excel file. Verify you are selecting the correct file.");
                }
                ShowSuccessSnackbar("Your event list was imported from an Excel file");
            }
            catch
            {
                ShowErrorSnackbar("The application failed to import your event list from an Excel file");
            }
        }

        [RelayCommand]
        private void ExportEventListToXml()
        {
            try
            {
                //Save Event List To Xaml here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = _appData.ActiveRocketCalendar.CalendarName + "_Events",
                    InitialDirectory = appDataFolder,
                    Filter = "Rocket xml files (*.xml)|*.xml"
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }

                if (IncludePrivateEvents)
                {
                    io.SaveEventList_XML(_appData.ActiveRocketCalendar.EventCollection, saveFileDialog.FileName);
                }
                else
                {
                    ObservableCollection<RocketEvent> publicEventsCollection = new ObservableCollection<RocketEvent>();
                    var publicEvents = _appData.ActiveRocketCalendar.EventCollection.Where(e => e.IsPrivate == false);
                    foreach (var item in publicEvents)
                    {
                        publicEventsCollection.Add(item);
                    }
                    io.SaveEventList_XML(publicEventsCollection, saveFileDialog.FileName);
                }
                ShowSuccessSnackbar("Your event list was saved to an Xaml file");
            }
            catch
            {
                ShowErrorSnackbar("The application failed to save your event list to a Xaml file");
            }
        }

        [RelayCommand]
        private void ImportEventListFromXml()
        {
            try
            {
                //Import Event List from Xml here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.OpenFileDialog openFileDialog = new()
                {
                    InitialDirectory = appDataFolder,
                    Filter = "Rocket xml files (*.xml)|*.xml"
                };

                if (openFileDialog.ShowDialog() != true)
                {
                    //ShowErrorSnackbar("The application failed to import your event list from a Xml file. Verify you are selecting the correct file.");
                    return;
                }

                var newEventList =  io.LoadEventList_XML(openFileDialog.FileName);

                if(newEventList != null)
                {
                    var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Event List Changes",
                        Content = "Do you want to the loaded event list to be added to or overwrite your current event list?",
                        PrimaryButtonText = "Add",
                        PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Primary,
                        SecondaryButtonText = "Overwrite",
                        SecondaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Danger
                    };

                    var result = uiMessageBox.ShowDialogAsync();

                    if (result.Result.ToString() == "Primary")
                    {
                        foreach (RocketEvent e in newEventList)
                        {
                            _appData.ActiveRocketCalendar.EventCollection.Add(e);
                        }
                        ShowSuccessSnackbar("Your event list was imported from a Xml file");
                    }
                    else if (result.Result.ToString() == "Secondary")
                    {
                        _appData.ActiveRocketCalendar.EventCollection.Clear();
                        foreach (RocketEvent e in newEventList)
                        {
                            _appData.ActiveRocketCalendar.EventCollection.Add(e);
                        }
                        ShowSuccessSnackbar("Your event list was imported from a Xml file");
                    }
                }
                else
                {
                    ShowErrorSnackbar("The application failed to import your event list from a Xml file. Verify you are selecting the correct file.");
                }
            }
            catch
            {
                ShowErrorSnackbar("The application failed to import your event list from a Xml file");
            }
        }

        [RelayCommand]
        private void ExportCalendarToXml()
        {
            try
            {
                //Export Calendar to Xaml here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = _appData.ActiveRocketCalendar.CalendarName,
                    InitialDirectory = appDataFolder,
                    Filter = "Rocket xml files (*.xml)|*.xml"
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }

                //save to SaveFileDialog.FileName here
                io.SaveCalendar_XML(_appData.ActiveRocketCalendar, saveFileDialog.FileName);

                ShowSuccessSnackbar("Your calendar was saved to a Xaml file");
            }
            catch
            {
                ShowErrorSnackbar("The application failed to save your calendar to a Xaml file");
            }
        }

        [RelayCommand]
        private void ImportCalendarFromXml()
        {
            try
            {
                //Import Calendar from Xml here..
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "RocketCalendar");

                if(!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                Microsoft.Win32.OpenFileDialog openFileDialog = new()
                {
                    InitialDirectory = appDataFolder,
                    Filter = "Rocket xml files (*.xml)|*.xml"
                };

                if (openFileDialog.ShowDialog() != true)
                {
                    //ShowErrorSnackbar("The application failed to import your calendar from a Xml file. Verify you are selecting the correct file.");
                    return;
                }

                var newCalendar = io.LoadCalendar_XML(openFileDialog.FileName);

                if (newCalendar != null)
                {
                    var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Calendar Data Change",
                        Content = "Are you sure you want to load a new calendar? Unsaved data will be lost.",
                        PrimaryButtonText = "Load New Calendar",
                        PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Caution
                    };

                    var result = uiMessageBox.ShowDialogAsync();

                    if (result.Result.ToString() == "Primary")
                    {
                        _appData.ActiveRocketCalendar = newCalendar;
                        Properties.Settings.Default.DefaultCalendarFilePath = openFileDialog.FileName;
                        Properties.Settings.Default.Save();
                        ShowSuccessSnackbar("Your event list was imported from a Xml file");
                    }
                }
                else
                {
                    ShowErrorSnackbar("The application failed to import your calendar from a Xml file. Verify you are selecting the correct file.");
                }
            }
            catch
            {
                ShowErrorSnackbar("The application failed to import your calendar from a Xml file");
            }
        }
    }
}
