using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppointAid.Data;
using AppointAid.Models;
using AppointAid.ViewModels;
using AppointAid.Services;

namespace AppointAid.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (!doctorId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a doctor to view this page.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch requested tests for the doctor
            var requestedTests = await _context.MedicalTests
                .Include(mt => mt.Patient)
                .Where(mt => mt.DoctorId == doctorId && mt.IsApproved == null) // Pending approval
                .ToListAsync();

            // Fetch upcoming appointments for the doctor
            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId && a.TimeSlot.Date > DateTime.Now)
                .ToListAsync();

            ViewBag.RequestedTests = requestedTests;
            ViewBag.UpcomingAppointments = upcomingAppointments;

            return View();
        }

        // GET: Doctors1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Sector)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors1/Create
        public IActionResult Create()
        {
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId");
            return View();
        }

        // POST: Doctors1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,FirstName,LastName,NationalNumber,SectorId,Specialty,PasswordHash")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId", doctor.SectorId);
            return View(doctor);
        }

        // GET: Doctors1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId", doctor.SectorId);
            return View(doctor);
        }

        // POST: Doctors1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,FirstName,LastName,NationalNumber,SectorId,Specialty,PasswordHash")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId", doctor.SectorId);
            return View(doctor);
        }

        // GET: Doctors1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Sector)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }

        [HttpGet]
        public async Task<IActionResult> PatientHistory(int id)
        {
            var patient = await GetPatientAsync(id);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            var appointments = await GetAppointmentsAsync(id);
            var medicalTests = await GetMedicalTestsAsync(id);

            var model = new PatientHistoryViewModel
            {
                Patient = patient,
                Appointments = appointments,
                MedicalTests = medicalTests
            };

            return View(model);
        }

        private async Task<Patient> GetPatientAsync(int patientId)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        private async Task<List<Appointment>> GetAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        private async Task<List<MedicalTest>> GetMedicalTestsAsync(int patientId)
        {
            return await _context.MedicalTests
                .Where(mt => mt.PatientId == patientId)
                .ToListAsync();
        }


        // GET: RequestedTests
        [HttpGet]
        public async Task<IActionResult> RequestedTests()
        {
            var tests = await _context.MedicalTests
                .Include(mt => mt.Patient)
                .Where(mt => mt.IsApproved == null) // Fetch pending tests
                .ToListAsync();

            return View(tests);
        }


        [HttpGet]
        public async Task<IActionResult> ReviewTest(int id)
        {
            var test = await _context.MedicalTests
                .Include(mt => mt.Patient)
                .FirstOrDefaultAsync(mt => mt.MedicalTestId == id);

            if (test == null)
            {
                TempData["ErrorMessage"] = "Test request not found.";
                return RedirectToAction("RequestedTests");
            }

            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewTest(int id, bool approve)
        {
            var test = await _context.MedicalTests.FindAsync(id);
            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("Index");
            }

            test.IsApproved = approve;

            _context.MedicalTests.Update(test);
            await _context.SaveChangesAsync();

            TempData["Message"] = approve ? "Test approved successfully." : "Test denied successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SetSchedule()
        {
            var viewModel = new WeeklyScheduleViewModel
            {
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(7),
                Days = Enum.GetNames(typeof(DayOfWeek)).Select(day => new ScheduleDay
                {
                    Day = day,
                    IsAvailable = false,
                    StartTime = null,
                    EndTime = null
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetSchedule(WeeklyScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (!doctorId.HasValue)
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // Remove existing time slots within the specified range for the doctor
            var existingTimeSlots = await _context.DoctorTimeSlots
                .Include(dts => dts.TimeSlot)
                .Where(dts => dts.DoctorId == doctorId && dts.TimeSlot.Date >= model.StartDate && dts.TimeSlot.Date <= model.EndDate)
                .ToListAsync();

            _context.DoctorTimeSlots.RemoveRange(existingTimeSlots);
            await _context.SaveChangesAsync();

            // Add new schedule
            foreach (var day in model.Days.Where(d => d.IsAvailable))
            {
                for (var date = model.StartDate; date <= model.EndDate; date = date.AddDays(1))
                {
                    if (Enum.GetName(typeof(DayOfWeek), date.DayOfWeek) == day.Day)
                    {
                        var timeSlot = new TimeSlot
                        {
                            Date = date,
                            StartTime = day.StartTime.Value,
                            EndTime = day.EndTime.Value,
                            Availability = true
                        };

                        _context.TimeSlots.Add(timeSlot);
                        await _context.SaveChangesAsync();

                        _context.DoctorTimeSlots.Add(new DoctorTimeSlot
                        {
                            DoctorId = doctorId.Value,
                            TimeSlotId = timeSlot.TimeSlotId
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Schedule saved successfully!";
            return RedirectToAction("Index", "Doctors");
        }
    }
}