using BDCarRental.Models;
using BDCarRental.Views;
using CarRentalDataService.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace BDCarRental.ViewModels
{
   public class BookingsViewModel
   {
      private HttpClient _client;
      private IDialogCoordinator _dialogCoordinator;
      private RelayCommand _openReturnCommand;
      private CustomDialog _customDialog;
      private ReturnControl _returnControl;
      private ObservableCollection<BookingModel> _bookings;
      int _bookingId;

      public ObservableCollection<BookingModel> Bookings 
      {
         get { return _bookings; }
         set
         {
            if (_bookings != value)
            {
               _bookings = value;
            }
         }
      }

      public ICommand OpenReturnCommand
      {
         get
         {
            if (_openReturnCommand == null)
            {
               _openReturnCommand = new RelayCommand(param => this.OpenReturnDialogAsync(param));
            }
            return _openReturnCommand;
         }
      }

      public BookingsViewModel(IDialogCoordinator instance)
      {
         _dialogCoordinator = instance;
         Init();
      }

      private void Init()
      {
         _client = new HttpClient();
         _client.BaseAddress = new Uri("http://localhost:50487/");
         _bookings = new ObservableCollection<BookingModel>();
         GetAllBookings();
      }

      public void GetAllBookings()
      {
         List<Booking> bookings;

         HttpResponseMessage response = _client.GetAsync("api/Bookings").Result;
         if (response.IsSuccessStatusCode)
         {
            bookings = response.Content.ReadAsAsync<List<Booking>>().Result;
            if (bookings != null)
            {
               MapToBookingModel(bookings);
            }
         }
         GetBookingCustomerData();
         GetBookingCarData();
      }

      private void GetBookingCustomerData()
      {
         foreach (BookingModel booking in _bookings)
         {
            Customer customer;
            HttpResponseMessage response = _client.GetAsync("api/Customers/" + booking.CustomerId).Result;
            if (response.IsSuccessStatusCode)
            {
               customer = response.Content.ReadAsAsync<Customer>().Result;
               booking.BookedCustomer = customer.FirstName + " " + customer.LastName;
            }
         }
      }

      private void GetBookingCarData()
      {
         foreach (BookingModel booking in _bookings)
         {
            Car car;

            HttpResponseMessage response = _client.GetAsync("api/Cars/" + booking.CarId).Result;
            if (response.IsSuccessStatusCode)
            {
               car = response.Content.ReadAsAsync<Car>().Result;
               booking.BookedCar = car.Make + " " + car.Model + " (" + car.ModelYear + ")";
            }
         }
      }

      private void ReturnCar()
      {
         HttpResponseMessage response = _client.DeleteAsync("api/Bookings/" + _bookingId).Result;
         if (response.IsSuccessStatusCode)
         {
            GetAllBookings();
         }
      }

      private async void OpenReturnDialogAsync(object param)
      {
         _bookingId = (int)param;

         MetroDialogSettings metroDialogSettings = new MetroDialogSettings()
         {
            AnimateHide = true,
            AnimateShow = true,
            ColorScheme = MetroDialogColorScheme.Accented
         };

         _customDialog = new CustomDialog();
         _returnControl = new ReturnControl();
         _customDialog.Content = _returnControl;

         string carToReturn = _bookings.FirstOrDefault(x => x.Id == _bookingId).BookedCar;

         _returnControl.CarToReturn.Content = carToReturn;
         _returnControl.ConfirmButton.Click += OnConfirmButtonClickAsync;
         _returnControl.CancelButton.Click += OnCancelButtonClickAsync;

         await _dialogCoordinator.ShowMetroDialogAsync(this, _customDialog, metroDialogSettings);
      }

      private async void OnConfirmButtonClickAsync(object sender, RoutedEventArgs e)
      {         
         ReturnCar();
         GetAllBookings();
         await _dialogCoordinator.HideMetroDialogAsync(this, _customDialog);
         CloseBookingControlDialog();
      }

      private async void OnCancelButtonClickAsync(object sender, RoutedEventArgs e)
      {
         await _dialogCoordinator.HideMetroDialogAsync(this, _customDialog);
         CloseBookingControlDialog();
      }

      private void CloseBookingControlDialog()
      {
         _bookingId = 0;
         _returnControl = null;
         _customDialog = null;
      }

      private void MapToBookingModel(List<Booking> bookings)
      {
         _bookings.Clear();

         foreach (Booking booking in bookings)
         {
            _bookings.Add(new BookingModel
            {
               Id = booking.Id,
               CarId = booking.CarId,
               CustomerId = booking.CustomerId,
               StartDate = booking.StartDate,
               EndDate = booking.EndDate,
               TotalCost = booking.TotalCost
            });
         }
      }
   }
}
