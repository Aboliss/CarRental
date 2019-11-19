using BDCarRental.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace BDCarRental.Views
{
   /// <summary>
   /// Interaction logic for CarsView.xaml
   /// </summary>
   public partial class CarsView : UserControl
   {
      CarsViewModel _viewModel = new CarsViewModel(DialogCoordinator.Instance);

      public CarsView()
      {
         InitializeComponent();
         this.DataContext = _viewModel;
         CarsDataGrid.ItemsSource = _viewModel.Cars;
      }
   }
}
