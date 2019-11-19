using BDCarRental.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace BDCarRental.Views
{
   /// <summary>
   /// Interaction logic for BookingsView.xaml
   /// </summary>
   public partial class BookingsView : UserControl
   {
      BookingsViewModel _viewModel = new BookingsViewModel(DialogCoordinator.Instance);

      public BookingsView()
      {
         InitializeComponent();
         this.DataContext = _viewModel;
         BookingsDataGrid.ItemsSource = _viewModel.Bookings;
      }
   }
}
