using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnectPrototype.Data;
using AgriEnergyConnectPrototype.Models;

namespace AgriEnergyConnectPrototype.Controllers
{
    // Allow Employees and Admins
    [Authorize(Roles = "Employee,Admin")]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FarmersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var farmers = await _context.FarmerProfiles
                .Include(f => f.User)
                .Include(f => f.Products)
                .ToListAsync();
            return View(farmers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FarmerProfile model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, "Farmer123!");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Farmer");

                    model.UserId = user.Id;
                    _context.FarmerProfiles.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var farmer = await _context.FarmerProfiles
                .Include(f => f.User)
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.FarmerId == id);

            if (farmer == null)
                return NotFound();

            return View(farmer);
        }
    }
}
