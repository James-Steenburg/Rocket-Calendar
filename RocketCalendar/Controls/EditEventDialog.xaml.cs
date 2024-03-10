using CommunityToolkit.Mvvm.ComponentModel;
using RocketCalendar.Models;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace RocketCalendar.Controls
{
    /// <summary>
    /// Interaction logic for EditEventDialog.xaml
    /// </summary>
    public partial class EditEventDialog : ContentDialog
    {
        public static readonly DependencyProperty RocketEventProperty = DependencyProperty.Register("EdittedEvent", typeof(RocketEvent), typeof(EditEventDialog));
        public RocketEvent EdittedEvent
        {
            get { return (RocketEvent)GetValue(RocketEventProperty); }
            set { SetValue(RocketEventProperty, value); }
        }

        private RocketCalendarModel _rocketCalendar;

        public int SelectedInputMonthMaxDays
        {
            get
            {
                return 10;
            }
        }

        public EditEventDialog(ContentPresenter contentPresenter, RocketCalendarModel rocketCalendar) : base(contentPresenter)
        {
            //ViewModel = calendarViewModel;

            //DataContext = calendarViewModel;

            //EdittedEvent = ViewModel.ActiveCalendar.WipEventPlaceholder;

            //DataContext = this;
            //_rocketCalendar = rocketCalendar;
            //EdittedEvent = _rocketCalendar.WipEventPlaceholder;

            InitializeComponent();
            
        }

        public CalendarViewModel ViewModel { get; }
    }
}
