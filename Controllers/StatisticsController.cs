using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BudgetBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using BudgetBook.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BudgetBook.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public StatisticsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, TransactionType? type, int? categoryId)
    {
        var userId = _userManager.GetUserId(User);

        var start = startDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var end = endDate ?? start.AddMonths(1).AddDays(-1);

        var transactionsInRange = _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.BookingDate >= start && t.BookingDate <= end);

        if (type.HasValue)
        {
            transactionsInRange = transactionsInRange.Where(t => t.Type == type.Value);
        }

        if (categoryId.HasValue) {
            transactionsInRange = transactionsInRange.Where(t => t.CategoryId == categoryId);
        }

        var totalIncome = await transactionsInRange
            .Where(t => t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount);

        var totalExpense = await transactionsInRange
            .Where(t => t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount);

        var expensesByCategory = await transactionsInRange
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategorySummary
            {
                CategoryName = g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.Total)
            .ToListAsync();

        var transactionsByMonth = await transactionsInRange
            .GroupBy(t => new { t.BookingDate.Year, t.BookingDate.Month })
            .Select(g => new MonthlySummary
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToListAsync();

        ViewBag.SelectedType = type;
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.AllCategories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

        var model = new StatisticsViewModel
        {
            StartDate = start,
            EndDate = end,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            ExpensesByCategory = expensesByCategory,
            TransactionsByMonth = transactionsByMonth
        };

        return View(model);
    }
}
