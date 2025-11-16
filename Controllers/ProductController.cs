using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnectPrototype.Data;
using AgriEnergyConnectPrototype.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgriEnergyConnectPrototype.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Employees and Admins: browse products (produce + energy)
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Index(string location, string category)
        {
            var query = _context.Products
                .Include(p => p.FarmerProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(p => EF.Functions.Like(p.Location, $"%{location}%"));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => EF.Functions.Like(p.Category, $"%{category}%"));

            // sort: resources with date null fall back safely
            query = query
                .OrderByDescending(p => p.ProductionDate ?? DateTime.MinValue)
                .ThenBy(p => p.ProductName);

            return View(await query.ToListAsync());
        }

        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            // Pre-fill for convenience
            return View(new Product
            {
                Kind = ProductKind.Produce,
                ProductionDate = DateTime.UtcNow.Date
            });
        }

        [HttpPost]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View(product);

            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.FarmerProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null)
            {
                ModelState.AddModelError("", "Farmer profile not found.");
                return View(product);
            }

            product.FarmerId = profile.FarmerId;

            // Normalize fields based on Kind
            if (product.Kind == ProductKind.Produce)
            {
                if (!product.ProductionDate.HasValue) product.ProductionDate = DateTime.UtcNow.Date;
                product.VendorName = null; product.EnergyType = null; product.PowerkW = null;
                product.SuitableFor = null; product.DatasheetUrl = null; product.PriceZar = null;
            }
            else // EnergySolution
            {
                product.ProductionDate = null; product.IsOrganic = false;
                product.Unit = null; product.PricePerUnit = null;
                if (string.IsNullOrWhiteSpace(product.Location)) product.Location = "National";
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProducts));
        }

        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> MyProducts()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.FarmerProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null) return NotFound("Farmer profile not found.");

            var products = await _context.Products
                .Where(p => p.FarmerId == profile.FarmerId)
                .OrderByDescending(p => p.ProductionDate ?? DateTime.MinValue)
                .ThenBy(p => p.ProductName)
                .ToListAsync();

            return View(products);
        }
    }
}
