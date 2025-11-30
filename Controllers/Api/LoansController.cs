using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly LibraryContext _context;

        public LoansController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();
        }

        // POST: api/Loans
        [HttpPost]
        public async Task<ActionResult<Loan>> PostLoan(Loan loan)
        {
            // Check if book is available
            var book = await _context.Books.FindAsync(loan.BookId);
            if (book == null || book.AvailableCopies <= 0)
            {
                return BadRequest("Book is not available for loan");
            }

            // Set loan dates
            loan.LoanDate = DateTime.UtcNow;
            loan.DueDate = DateTime.UtcNow.AddDays(14); // 2 weeks loan period
            loan.Status = "Active";

            // Update book availability
            book.AvailableCopies--;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.Id }, loan);
        }

        // PUT: api/Loans/5/return
        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            // Update loan
            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = "Returned";

            // Calculate fine if overdue
            if (loan.ReturnDate > loan.DueDate)
            {
                var daysOverdue = (loan.ReturnDate.Value - loan.DueDate).Days;
                loan.FineAmount = daysOverdue * 5; // $5 per day
            }

            // Update book availability
            loan.Book.AvailableCopies++;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Loans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            return loan;
        }
    }
}