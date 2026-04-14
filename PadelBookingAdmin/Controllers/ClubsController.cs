using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PadelBookingAdmin.Models.Clubs;
using PadelBookingAdmin.Services;

namespace PadelBookingAdmin.Controllers
{
    [Authorize]
    public class ClubsController : Controller
    {
        private readonly PadelApiService _padelApiService;

        public ClubsController(PadelApiService padelApiService)
        {
            _padelApiService = padelApiService;
        }

        public async Task<IActionResult> Index()
        {
            var clubs = await _padelApiService.GetClubsAsync();
            return View(clubs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            try
            {
                await _padelApiService.CreateClubAsync(request);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating club: " + ex.Message);
                return View(request);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var club = await _padelApiService.GetClubAsync(id);
            if (club == null) return NotFound();

            var updateRequest = new UpdateClubRequest
            {
                Name = club.Name ?? "",
                Address = club.Address ?? "",
                OpenTime = club.OpenTime ?? "",
                CloseTime = club.CloseTime ?? "",
                IsActive = club.IsActive
            };

            ViewBag.Id = id;
            return View(updateRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateClubRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(request);
            }

            try
            {
                await _padelApiService.UpdateClubAsync(id, request);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating club: " + ex.Message);
                ViewBag.Id = id;
                return View(request);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _padelApiService.DeleteClubAsync(id);
            }
            catch (Exception)
            {
                // Optionally handle errors, maybe return to error page. 
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
