using BDCarRental.Models;
using BDCarRental.Views;
using CarRentalDataService.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BDCarRental.ViewModels
{
   public class CarsViewModel
   {
      private HttpClient _client;
      private IDialogCoordinator _dialogCoordinator;
      private RelayCommand _openBookingCommand;
      private CustomDialog _customDialog;
      private BookingControl _bookingControl;
      private ObservableCollection<CarModel> _cars;
      private int _bookingCarId;
      private decimal _bookingCarPrice;
      private decimal _totalPrice;

      public ObservableCollection<CarModel> Cars 
      { 
         get { return _cars; }
         set 
         {
            if (_cars != value)
            {
               _cars = value;
            }
         }
      }

      public ICommand OpenBookingCommand
      {
         get
         {
            if (_openBookingCommand == null)
            {
               _openBookingCommand = new RelayCommand(param => this.OpenBookingDialogAsync(param));
            }
            return _openBookingCommand;
         }
      }

      public CarsViewModel(IDialogCoordinator instance)
      {
         _dialogCoordinator = instance;
         Init();
      }
      
      // Get all cars
      public void GetAllCars()
      {
         List<Car> cars;

         HttpResponseMessage response = _client.GetAsync("api/Cars").Result;
         if (response.IsSuccessStatusCode)
         {
            cars = response.Content.ReadAsAsync<List<Car>>().Result;
            if (cars != null && cars.Count > 0)
            {
               MapToCarModel(cars);
            }            
         }
      }

      private CustomerModel CreateCustomer(CustomerModel customer)
      {
         CustomerModel result = new CustomerModel();
         
         HttpResponseMessage response = _client.PostAsJsonAsync("api/Customers", customer).Result;
         if (response.IsSuccessStatusCode)
         {
            result = response.Content.ReadAsAsync<CustomerModel>().Result;
         }
         return result;
      }

      private BookingModel CreateBooking(BookingModel booking)
      {
         BookingModel result = new BookingModel();

         HttpResponseMessage response = _client.PostAsJsonAsync("api/Bookings", booking).Result;
         if (response.IsSuccessStatusCode)
         {
            result = response.Content.ReadAsAsync<BookingModel>().Result;
         }
         return result;
      }

      private void Init()
      {
         _client = new HttpClient();
         _client.BaseAddress = new Uri("http://localhost:50487/");
         _cars = new ObservableCollection<CarModel>();
         GetAllCars();
      }

      private async void OpenBookingDialogAsync(object param)
      {
         _bookingCarId = (int)param;
         CarModel bookingCar = _cars.First(x => x.Id == _bookingCarId);
         _bookingCarPrice = bookingCar.Price;

         MetroDialogSettings metroDialogSettings = new MetroDialogSettings()
         {
            AnimateHide = true,
            AnimateShow = true,
            ColorScheme = MetroDialogColorScheme.Accented
         };

         _customDialog = new CustomDialog();
         _bookingControl = new BookingControl();
         _customDialog.Content = _bookingControl;

         _bookingControl.BookingCarLabel.Content = bookingCar.Make + " " + bookingCar.Model + " (" + bookingCar.Year + ")";
         _bookingControl.ConfirmButton.Click += OnConfirmButtonClickAsync;
         _bookingControl.CancelButton.Click += OnCancelButtonClickAsync;
         _bookingControl.BookingStartDate.SelectedDateChanged += CalculateTotalPrice;
         _bookingControl.BookingEndDate.SelectedDateChanged += CalculateTotalPrice;

         await _dialogCoordinator.ShowMetroDialogAsync(this, _customDialog, metroDialogSettings);
      }

      private bool ProcessBooking()
      {         
         string firstName = _bookingControl.BookingFirstName.Text;
         string lastName = _bookingControl.BookingLastName.Text;
         DateTime? startDate = _bookingControl.BookingStartDate.SelectedDate;
         DateTime? endDate = _bookingControl.BookingEndDate.SelectedDate;

         // Validate fields
         bool isValid = ValidateForm(firstName, lastName, startDate, endDate);
         
         if (!isValid)
         {
            return false;
         }
         else
         {
            CustomerModel cust = new CustomerModel
            {
               FirstName = firstName,
               LastName = lastName
            };

            // 1. Create new customer
            CustomerModel newCustomer = CreateCustomer(cust);

            // 2. Create new booking
            BookingModel book = new BookingModel
            {
               CarId = _bookingCarId,
               StartDate = startDate,
               EndDate = endDate,
               CustomerId = newCustomer.Id,
               TotalCost = _totalPrice
            };
            BookingModel newBooking = CreateBooking(book);
            
            // 3. Update car list
            GetAllCars();

            return true;
         }         
      }

      private void CalculateTotalPrice(object sender, SelectionChangedEventArgs e)
      {
         DateTime? start = _bookingControl.BookingStartDate.SelectedDate;
         DateTime? end = _bookingControl.BookingEndDate.SelectedDate;

         if (start != null & end != null)
         {
            if (start < DateTime.Today)
            {
               _totalPrice = 0;
               _bookingControl.BookingStartDate.Text = null;
            }
            else if(end < start)
            {
               _totalPrice = 0;
               _bookingControl.BookingEndDate.Text = null;
            }            
            else if (start < end)
            {
               TimeSpan duration = end.Value.Subtract(start.Value);
               _totalPrice = ((decimal)duration.TotalDays + 1) * _bookingCarPrice;
            }
            else if (start == end)
            {
               _totalPrice = _bookingCarPrice;

            }
            _bookingControl.TotalPrice.Text = _totalPrice.ToString() + " €";
         }
         else
         {
            _totalPrice = 0;
            _bookingControl.TotalPrice.Text = _totalPrice.ToString() + " €";
         }
      }

      private async void OnConfirmButtonClickAsync(object sender, RoutedEventArgs e)
      {
         bool result = ProcessBooking();
         if (result)
         {
            await _dialogCoordinator.HideMetroDialogAsync(this, _customDialog);
            CloseBookingControlDialog();
         }         
      }

      private async void OnCancelButtonClickAsync(object sender, RoutedEventArgs e)
      {
         await _dialogCoordinator.HideMetroDialogAsync(this, _customDialog);
         CloseBookingControlDialog();
      }

      private void CloseBookingControlDialog()
      {
         _bookingCarId = 0;
         _totalPrice = 0;
         _bookingCarPrice = 0;
         _bookingControl = null;
         _customDialog = null;
      }

      private bool ValidateForm(string firstName, string lastName, DateTime? startDate, DateTime? endDate)
      {
         int counter = 0;

         if (string.IsNullOrEmpty(firstName))
         {
            _bookingControl.BookingFirstName.BorderBrush = Brushes.Red;
            counter++;
         }
         else
         {
            _bookingControl.BookingFirstName.BorderBrush = Brushes.Black;
         }
         if (string.IsNullOrEmpty(lastName))
         {
            _bookingControl.BookingLastName.BorderBrush = Brushes.Red;
            counter++;
         }
         else
         {
            _bookingControl.BookingLastName.BorderBrush = Brushes.Black;
         }
         if (startDate == null)
         {
            _bookingControl.BookingStartDate.BorderBrush = Brushes.Red;
            counter++;
         }
         else
         {
            _bookingControl.BookingStartDate.BorderBrush = Brushes.Black;
         }
         if (endDate == null)
         {
            _bookingControl.BookingEndDate.BorderBrush = Brushes.Red;
            counter++;
         }
         else
         {
            _bookingControl.BookingEndDate.BorderBrush = Brushes.Black;
         }

         return counter > 0 ? false : true;
      }

      private void MapToCarModel(List<Car> cars)
      {
         _cars.Clear();

         foreach (Car car in cars)
         {
            _cars.Add(new CarModel
            {
               Id = car.Id,
               Make = car.Make,
               Model = car.Model,
               Year = car.ModelYear,
               Price = car.PricePerDay,
               Availability = car.IsAvailable
            });
         }
      }

   }
}
