using Enigma.Data;
using Enigma.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enigma.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public IndexModel(ApplicationDbContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public DateTime Date { get; set; } = DateTime.Now;

        [BindProperty]
        public bool Hard { get; set; }

        [BindProperty]
        public List<string> Templates { get; set; }

        public async Task OnGetAsync()
        {
            Templates = await context.CounselorTemplates
                .Select(t => t.FirstName)
                .ToListAsync();

            if (Templates.Count == 0)
                Templates = Enumerable.Range(0, 6).Select(r => string.Empty).ToList();
            else
                Templates.Add(string.Empty);
        }

        public async Task<IActionResult> OnPostSaveTemplatesAsync()
        {
            if (Templates == null)
                return Page();

            context.CounselorTemplates.RemoveRange(context.CounselorTemplates);
            context.CounselorTemplates.AddRange(
                Templates
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Select(t => new CounselorTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = t
                    }));

            await context.SaveChangesAsync();

            return Redirect("/");
        }

        public async Task<IActionResult> OnPostAddMonthDatesAsync()
        {
            await AddAndRemoveDatesAsync(Date, Date.AddDays(30));

            return Redirect("/");
        }

        public async Task<IActionResult> OnPostAddWeekDatesAsync()
        {
            await AddAndRemoveDatesAsync(Date, Date.AddDays(7));

            return Redirect("/");
        }

        public async Task<IActionResult> OnPostAddDayDatesAsync()
        {
            await AddAndRemoveDatesAsync(Date, Date);

            return Redirect("/");
        }

        #region Utility Methods
        public async Task AddAndRemoveDatesAsync(DateTime start, DateTime end)
        {
            if (Hard)
                await RemoveDatesAsync(start, end);

            await AddDatesAsync(start, end);
        }

        public async Task RemoveDatesAsync(DateTime start, DateTime end)
        {
            context.Counselors.RemoveRange(
                await context.Counselors
                    .Where(c => c.Date.Date >= start.Date)
                    .Where(c => c.Date.Date <= end.Date)
                    .ToListAsync());

            await context.SaveChangesAsync();
        }

        public async Task AddDatesAsync(DateTime start, DateTime end)
        {
            var dates = Enumerable.Range(0, 1 + end.Subtract(start).Days)
              .Select(offset => start.AddDays(offset))
              .ToArray();

            var names = await context.CounselorTemplates
                .Select(t => t.FirstName)
                .ToListAsync();

            await context.Counselors.AddRangeAsync(
                dates.SelectMany(d =>
                    names.Select(n => new Counselor
                    {
                        Id = Guid.NewGuid().ToString(),
                        Date = d,
                        FirstName = n
                    })));

            await context.SaveChangesAsync();
        }
        #endregion Utility Methods
    }
}
