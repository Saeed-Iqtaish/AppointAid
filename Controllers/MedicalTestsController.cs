using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppointAid.Data;
using AppointAid.Models;

namespace AppointAid.Controllers
{
    public class MedicalTestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicalTests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MedicalTests.Include(m => m.Doctor).Include(m => m.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MedicalTests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalTest = await _context.MedicalTests
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalTestId == id);
            if (medicalTest == null)
            {
                return NotFound();
            }

            return View(medicalTest);
        }

        // GET: MedicalTests/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName");
            return View();
        }

        // POST: MedicalTests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalTestId,TestName,PatientId,DoctorId,IsApproved,Time,Results")] MedicalTest medicalTest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalTest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName", medicalTest.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", medicalTest.PatientId);
            return View(medicalTest);
        }

        // GET: MedicalTests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalTest = await _context.MedicalTests.FindAsync(id);
            if (medicalTest == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName", medicalTest.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", medicalTest.PatientId);
            return View(medicalTest);
        }

        // POST: MedicalTests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalTestId,TestName,PatientId,DoctorId,IsApproved,Time,Results")] MedicalTest medicalTest)
        {
            if (id != medicalTest.MedicalTestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalTestExists(medicalTest.MedicalTestId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName", medicalTest.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", medicalTest.PatientId);
            return View(medicalTest);
        }

        // GET: MedicalTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalTest = await _context.MedicalTests
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalTestId == id);
            if (medicalTest == null)
            {
                return NotFound();
            }

            return View(medicalTest);
        }

        // POST: MedicalTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalTest = await _context.MedicalTests.FindAsync(id);
            if (medicalTest != null)
            {
                _context.MedicalTests.Remove(medicalTest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalTestExists(int id)
        {
            return _context.MedicalTests.Any(e => e.MedicalTestId == id);
        }
    }
}
