using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly LibraryContext _context;

        public ReportsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Reports/most-borrowed
        [HttpGet("most-borrowed")]
        public async Task<ActionResult<IEnumerable<object>>> GetMostBorrowedBooks()
        {
            var result = await _context.Loans
                .Include(l => l.Book)
                .GroupBy(l => new { l.BookId, l.Book.Title })
                .Select(g => new
                {
                    g.Key.BookId,
                    g.Key.Title,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(10)
                .ToListAsync();

            return result;
        }

        // GET: api/Reports/usage-by-faculty
        [HttpGet("usage-by-faculty")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsageByFaculty()
        {
            var result = await _context.Loans
                .Include(l => l.User)
                .GroupBy(l => l.User.Faculty)
                .Select(g => new
                {
                    Faculty = g.Key,
                    LoanCount = g.Count()
                })
                .OrderByDescending(x => x.LoanCount)
                .ToListAsync();

            return result;
        }

        // GET: api/Reports/overdue-loans
        [HttpGet("overdue-loans")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetOverdueLoans()
        {
            var overdueLoans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == "Active" && l.DueDate < DateTime.UtcNow)
                .ToListAsync();

            return overdueLoans;
        }

        // GET: api/Reports/available-books
        [HttpGet("available-books")]
        public async Task<ActionResult<int>> GetAvailableBooksCount()
        {
            var count = await _context.Books.SumAsync(b => b.AvailableCopies);
            return count;
        }

        // GET: api/Reports/active-loans
        [HttpGet("active-loans")]
        public async Task<ActionResult<int>> GetActiveLoansCount()
        {
            var count = await _context.Loans.CountAsync(l => l.Status == "Active");
            return count;
        }
    }
}