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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace RocketCalendar.Views.Pages
{
    /// <summary>
    /// Interaction logic for ImagesPage.xaml
    /// </summary>
    public partial class ImagesDisplayPage : INavigableView<ViewModels.Pages.ImagesDisplayViewModel>
    {
        public ViewModels.Pages.ImagesDisplayViewModel ViewModel { get; }

        public ImagesDisplayPage(ViewModels.Pages.ImagesDisplayViewModel viewModel)
        {
            ViewModel = viewModel;

            DataContext = this;

            InitializeComponent();
        }
    }
}
