using RocketCalendar.Controls;
using RocketCalendar.Models;
using RocketCalendar.ViewModels;
using RocketCalendar.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui;

namespace RocketCalendar.Views
{
    /// <summary>
    /// Interaction logic for AddOrEditEventWindow.xaml
    /// </summary>
    public partial class AddOrEditEventWindow
    {

        public ViewModels.Pages.CalendarViewModel ViewModel { get; }

        public AddOrEditEventWindow(CalendarViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
