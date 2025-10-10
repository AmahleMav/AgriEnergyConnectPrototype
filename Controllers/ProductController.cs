using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnectPrototype.Data;
using AgriEnergyConnectPrototype.Models;

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

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index(string location, string category)
        {
            var query = _context.Products.Include(p => p.FarmerProfile).AsQueryable();

            // Support keyword-based filtering
            if (!string.IsNullOrEmpty(location))
                query = query.Where(p => EF.Functions.Like(p.Location, $"%{location}%"));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => EF.Functions.Like(p.Category, $"%{category}%"));

            return View(await query.ToListAsync());
        }

        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var profile = await _context.FarmerProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (profile == null)
                {
                    ModelState.AddModelError("", "Farmer profile not found.");
                    return View(product);
                }

                product.FarmerId = profile.FarmerId;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyProducts));
            }
            return View(product);
        }

        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> MyProducts()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.FarmerProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                return NotFound("Farmer profile not found.");
            }

            var products = await _context.Products
                .Where(p => p.FarmerId == profile.FarmerId)
                .ToListAsync();

            return View(products);
        }
    }
}
