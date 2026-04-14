using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PadelBookingAdmin.Models.Users;
using PadelBookingAdmin.Services;

namespace PadelBookingAdmin.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly PadelApiService _padelApiService;

        public UsersController(PadelApiService padelApiService)
        {
            _padelApiService = padelApiService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _padelApiService.GetUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            // Optionally, we could load Clubs here to populate a dropdown for ClubId
            try 
            {
                var clubs = await _padelApiService.GetClubsAsync();
                ViewBag.Clubs = clubs;
            }
            catch 
            {
                ViewBag.Clubs = new List<Models.Clubs.ClubResponse>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var clubs = await _padelApiService.GetClubsAsync();
                ViewBag.Clubs = clubs;
                return View(request);
            }

            try
            {
                await _padelApiService.CreateUserAsync(request);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating user: " + ex.Message);
                var clubs = await _padelApiService.GetClubsAsync();
                ViewBag.Clubs = clubs;
                return View(request);
            }
        }
    }
}
