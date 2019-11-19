using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarRentalDataService.Models;

namespace CarRentalDataService.Controllers
{
   public class BookingsController : ApiController
   {
      private CarRentalApplicationEntities db = new CarRentalApplicationEntities();

      // GET: api/Bookings
      public IQueryable<Booking> GetBookings()
      {
         return db.Bookings;
      }

      // GET: api/Bookings/5
      [ResponseType(typeof(Booking))]
      public async Task<IHttpActionResult> GetBooking(int id)
      {
         Booking booking = await db.Bookings.FindAsync(id);
         if (booking == null)
         {
            return NotFound();
         }

         return Ok(booking);
      }

      // PUT: api/Bookings/5
      [ResponseType(typeof(void))]
      public async Task<IHttpActionResult> PutBooking(int id, Booking booking)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         if (id != booking.Id)
         {
            return BadRequest();
         }

         db.Entry(booking).State = EntityState.Modified;

         try
         {
            await db.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!BookingExists(id))
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return StatusCode(HttpStatusCode.NoContent);
      }

      // POST: api/Bookings
      [ResponseType(typeof(Booking))]
      public async Task<IHttpActionResult> PostBooking(Booking booking)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         db.Bookings.Add(booking);
         Car car = await db.Cars.FindAsync(booking.CarId);
         car.IsAvailable = false;

         try
         {
            await db.SaveChangesAsync();
         }
         catch (DbUpdateException)
         {
            if (BookingExists(booking.Id))
            {
               return Conflict();
            }
            else
            {
               throw;
            }
         }

         return CreatedAtRoute("DefaultApi", new { id = booking.Id }, booking);
      }

      // DELETE: api/Bookings/5
      [ResponseType(typeof(Booking))]
      public async Task<IHttpActionResult> DeleteBooking(int id)
      {
         Booking booking = await db.Bookings.FindAsync(id);
         if (booking == null)
         {
            return NotFound();
         }

         Car car = await db.Cars.FindAsync(booking.CarId);
         car.IsAvailable = true;
         db.Bookings.Remove(booking);

         await db.SaveChangesAsync();

         return Ok(booking);
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            db.Dispose();
         }
         base.Dispose(disposing);
      }

      private bool BookingExists(int id)
      {
         return db.Bookings.Count(e => e.Id == id) > 0;
      }
   }
}