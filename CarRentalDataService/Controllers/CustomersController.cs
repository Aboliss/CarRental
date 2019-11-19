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
   public class CustomersController : ApiController
   {
      private CarRentalApplicationEntities db = new CarRentalApplicationEntities();

      // GET: api/Customers
      public IQueryable<Customer> GetCustomers()
      {
         return db.Customers;
      }

      // GET: api/Customers/5
      [ResponseType(typeof(Customer))]
      public async Task<IHttpActionResult> GetCustomer(int id)
      {
         Customer customer = await db.Customers.FindAsync(id);
         if (customer == null)
         {
            return NotFound();
         }

         return Ok(customer);
      }

      ////GET: api/Customers/Customer
      //[ResponseType(typeof(Customer))]
      //public async Task<IHttpActionResult> GetOrCreateCustomerByName([FromUri]Customer customer)
      //{
      //   Customer existingCustomer = db.Customers.Where(c => c.FirstName == customer.FirstName && c.LastName == customer.LastName).FirstOrDefault();

      //   if (existingCustomer != null)
      //   {
      //      return Ok(existingCustomer);            
      //   }
      //   else
      //   {
      //      Customer newCustomer = db.Customers.Add(new Customer { FirstName = customer.FirstName, LastName = customer.LastName });

      //      try
      //      {
      //         await db.SaveChangesAsync();
      //         return Ok(newCustomer);
      //      }
      //      catch (Exception)
      //      {
      //         return BadRequest();
      //      }
      //   }
      //}


      // PUT: api/Customers/5
      [ResponseType(typeof(void))]
      public async Task<IHttpActionResult> PutCustomer(int id, Customer customer)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         if (id != customer.Id)
         {
            return BadRequest();
         }

         db.Entry(customer).State = EntityState.Modified;

         try
         {
            await db.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!CustomerExists(id))
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

      // POST: api/Customers
      [ResponseType(typeof(Customer))]
      public async Task<IHttpActionResult> PostCustomer(Customer customer)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         db.Customers.Add(customer);

         try
         {
            await db.SaveChangesAsync();
         }
         catch (DbUpdateException)
         {
            if (CustomerExists(customer.Id))
            {
               return Conflict();
            }
            else
            {
               throw;
            }
         }

         return CreatedAtRoute("DefaultApi", new { id = customer.Id }, customer);
      }

      // DELETE: api/Customers/5
      [ResponseType(typeof(Customer))]
      public async Task<IHttpActionResult> DeleteCustomer(int id)
      {
         Customer customer = await db.Customers.FindAsync(id);
         if (customer == null)
         {
            return NotFound();
         }

         db.Customers.Remove(customer);
         await db.SaveChangesAsync();

         return Ok(customer);
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            db.Dispose();
         }
         base.Dispose(disposing);
      }

      private bool CustomerExists(int id)
      {
         return db.Customers.Count(e => e.Id == id) > 0;
      }

      private bool CustomerExists(string firstName, string lastName)
      {
         return db.Customers.Count(c => c.FirstName == firstName && c.LastName == lastName) > 0;
      }
   }
}