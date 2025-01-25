using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointAid.Data;
using AppointAid.ViewModels;
using AppointAid.Models;
using AppointAid.Utilities;
using AppointAid.Services;

namespace AppointAid.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PatientService _patientService;
        public AccountController(ApplicationDbContext context, PatientService patientService)
        {
            _patientService = patientService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            object user = null;

            switch (model.Role)
            {
                case "Doctor":
                    user = await _context.Doctors.FirstOrDefaultAsync(d => d.NationalNumber == model.NationalNumber);
                    break;
                case "Nurse":
                    user = await _context.Nurses.FirstOrDefaultAsync(n => n.NationalNumber == model.NationalNumber);
                    break;
                case "Patient":
                    user = await _context.Patients.FirstOrDefaultAsync(p => p.NationalNumber == model.NationalNumber);
                    break;
            }

            if (user != null)
            {
                string storedPasswordHash = user switch
                {
                    Doctor d => d.PasswordHash,
                    Nurse n => n.PasswordHash,
                    Patient p => p.PasswordHash,
                    _ => null
                };

                if (PasswordHasher.VerifyPassword(storedPasswordHash, model.Password))
                {
                    TempData["Message"] = $"Welcome back, {model.Role}!";
                    HttpContext.Session.SetString("UserRole", model.Role);
                    HttpContext.Session.SetString("UserId", model.NationalNumber);

                    return model.Role switch
                    {
                        "Doctor" => RedirectToAction("Index", "Doctor"),
                        "Nurse" => RedirectToAction("Index", "Nurse"),
                        "Patient" => RedirectToAction("Index", "Patients"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                ModelState.AddModelError("", "Invalid password. Please try again.");
            }
            else
            {
                ModelState.AddModelError("", "Invalid national number or password.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the NationalNumber is already registered
            bool patientExists = await _patientService.IsPatientExistsAsync(model.NationalNumber);
            if (patientExists)
            {
                ModelState.AddModelError("NationalNumber", "A patient with this National Number already exists.");
                return View(model);
            }

            // Hash the password
            string hashedPassword = PasswordHasher.HashPassword(model.Password);

            // Map RegisterViewModel to Patient entity
            var patient = new Patient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                NationalNumber = model.NationalNumber,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = hashedPassword
            };

            // Add patient to the database
            await _patientService.AddPatientAsync(patient);

            TempData["Message"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }
    }
}