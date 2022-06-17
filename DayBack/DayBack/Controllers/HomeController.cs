using DayBack.DAL;
using DayBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DayBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task< IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Products = await _context.Products.ToListAsync()
            };
            return View(homeVM);
        }

        
    }
}
