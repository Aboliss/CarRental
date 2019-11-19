using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Models
{
   public class CarModel
   {
      public int Id { get; set; }
      public string Make { get; set; }
      public string Model { get; set; }
      public short Year { get; set; }
      public decimal Price { get; set; }
      public bool Availability { get; set; }
   }
}
