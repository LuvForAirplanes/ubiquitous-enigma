using Enigma.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Enigma.Controllers
{
    [Produces("application/json")]
    [Route("/events")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext context;

        public EventsController(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Get(DateTime? end = null, DateTime? start = null)
        {
            end = end.HasValue ? end.Value.ToUniversalTime() : end;
            start = start.HasValue ? start.Value.ToUniversalTime() : start;
            return new OkObjectResult(
                await context.Counselors
                    .Where(c => c.Date.Date >= start.Value.Date)
                    .Where(c => c.Date.Date <= end.Value.Date)
                    .Select(e => new
                    {
                        Id = e.Id,
                        Title = $"{e.FirstName} {e.LastName}",
                        Start = e.Date.Date,
                        End = e.Date.Date,
                        AllDay = true
                    })
                    .ToListAsync()
                );
        }

        [HttpPost("/api/events/edit")]
        public async Task UpdateNameAsync(string id, string name)
        {
            var counselor = await context.Counselors.FirstOrDefaultAsync(c => c.Id == id);
            counselor.FirstName = name;
            await context.SaveChangesAsync();
        }

        [HttpDelete("/api/events/delete")]
        public async Task DeleteNameAsync(string id)
        {
            context.Remove(await context.Counselors.FirstOrDefaultAsync(c => c.Id == id));
            await context.SaveChangesAsync();
        }
    }
}
