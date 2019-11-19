using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDCarRental.Models
{
   public class BookingModel
   {
      public int Id { get; set; }
      public DateTime? StartDate { get; set; }
      public DateTime? EndDate { get; set; }
      public decimal? TotalCost { get; set; }
      public string BookedCustomer { get; set; }
      public string BookedCar { get; set; }
      public int CarId { get; set; }
      public int CustomerId { get; set; }
   }
}
