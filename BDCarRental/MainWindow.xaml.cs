using System.Windows;
using BDCarRental.ViewModels;
using MahApps.Metro.Controls;

namespace BDCarRental
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : MetroWindow
   {
      private CarsViewModel _carsViewModel;
      private BookingsViewModel _bookingsViewModel;
      public MainWindow()
      {
         InitializeComponent();
         _carsViewModel = (CarsViewModel)CarsViewUserControl.DataContext;
         _bookingsViewModel = (BookingsViewModel)BookingsViewUserControl.DataContext;
         
      }
      
      private void CarsTabSelected(object sender, RoutedEventArgs e)
      {
         _carsViewModel.GetAllCars();
      }
      
      private void BookingsTabSelected(object sender, RoutedEventArgs e)
      {
         _bookingsViewModel.GetAllBookings();
      }
   }
}
