using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoSQL.Models;
using NoSQL.UI.ViewModels;

namespace NoSQL.UI.Controllers
{
    /// <summary>
    /// Handles all requests fired by the web application with regards to Tickets.
    /// </summary>
    public class TicketController : ControllerBase
    {
        public TicketController(ILogger<HomeController> logger) : base(logger)
        {
        }

        /// <summary>
        /// Shows all tickets in the database and allows the addition of tickets.
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            List<Ticket> tickets = new List<Ticket>();

            using (var client = GetHttpClient())
            {
                var response = client.GetAsync("Ticket");
                response.Wait();

                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<Ticket>>();
                    tickets = readTask.Result;
                }
            }

            var ticketvm = new List<TicketViewModel>();
            foreach (var ticket in tickets)
            {
                ticketvm.Add(new TicketViewModel(ticket));
            }

            if (TempData["apiError"] != null)
            {
                ModelState.AddModelError("apiError", TempData["apiError"].ToString());
            }


            return View(ticketvm);
        }

        /// <summary>
        /// Inserts a new ticket into the database with values set according to the values in the Html form.
        /// </summary>
        [HttpPost]
        public IActionResult CreateTicket(TicketViewModel ticketvm)
        {
            Ticket ticket;
            if (ticketvm.Id != null)
            {
                ticket = new Ticket(ticketvm.Id, ticketvm.Subject, ticketvm.FirstName, ticketvm.LastName, ticketvm.Date,
                    ticketvm.Status);    
            }
            else
            {
                ticket = new Ticket(ticketvm.Subject, ticketvm.FirstName, ticketvm.LastName, ticketvm.Date,
                    ticketvm.Status);
            }
            

            using (var client = GetHttpClient())
            {
                var response = client.PostAsJsonAsync("Ticket", ticket);
                response.Wait();

                var result = response.Result;
                if (!result.IsSuccessStatusCode)
                {
                    TempData["apiError"] = result.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    TempData["apiError"] = null;
                }
            }

            
            return RedirectToAction("Index");
        }
    }
}