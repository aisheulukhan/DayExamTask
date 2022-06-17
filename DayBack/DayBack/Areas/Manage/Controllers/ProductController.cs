using DayBack.DAL;
using DayBack.Models;
using DayBack.Utilities.Extentiton;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DayBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {

        public AppDbContext _context { get; }
        private readonly IWebHostEnvironment _env;


        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {

            if (product.Photo.CheckSize(800))
            {
                ModelState.AddModelError("Photo", "File size must be less than 800kb");
                return View();
            }
            if (!product.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "File must be image");
                return View();
            }
            product.Image = await product.Photo.SaveFileAsync(Path.Combine(_env.WebRootPath, "image"));
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);
            if (product == null) return NotFound();
            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "image", product.Image)))
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "image", product.Image));
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Product product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product  == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                var s = await _context.Products.FindAsync(product.Id);
                s.Title = product.Title;
                s.Desc = product.Desc;
                s.CategoryId = product.CategoryId;
                if (product.Photo != null)
                {
                    if (product.Image != null)
                    {
                        string filePath = Path.Combine(_env.WebRootPath, "image", product.Image);
                        System.IO.File.Delete(filePath);
                    }
                    s.Image = ProcessUploadedFile(product);
                }
                _context.Update(s);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string ProcessUploadedFile(Product product)
        {
            string uniqueFileName = null;

            if (product.Photo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + product.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    product.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
