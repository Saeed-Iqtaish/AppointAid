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
    public class EmergencyResponsesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmergencyResponsesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmergencyResponses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmergencyResponses.Include(e => e.Nurse).Include(e => e.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EmergencyResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencyResponse = await _context.EmergencyResponses
                .Include(e => e.Nurse)
                .Include(e => e.Patient)
                .FirstOrDefaultAsync(m => m.EmergencyResponseId == id);
            if (emergencyResponse == null)
            {
                return NotFound();
            }

            return View(emergencyResponse);
        }

        // GET: EmergencyResponses/Create
        public IActionResult Create()
        {
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName");
            return View();
        }

        // POST: EmergencyResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmergencyResponseId,PatientId,NurseId,Location,Time,Status")] EmergencyResponse emergencyResponse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emergencyResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName", emergencyResponse.NurseId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", emergencyResponse.PatientId);
            return View(emergencyResponse);
        }

        // GET: EmergencyResponses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencyResponse = await _context.EmergencyResponses.FindAsync(id);
            if (emergencyResponse == null)
            {
                return NotFound();
            }
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName", emergencyResponse.NurseId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", emergencyResponse.PatientId);
            return View(emergencyResponse);
        }

        // POST: EmergencyResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmergencyResponseId,PatientId,NurseId,Location,Time,Status")] EmergencyResponse emergencyResponse)
        {
            if (id != emergencyResponse.EmergencyResponseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emergencyResponse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmergencyResponseExists(emergencyResponse.EmergencyResponseId))
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
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName", emergencyResponse.NurseId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", emergencyResponse.PatientId);
            return View(emergencyResponse);
        }

        // GET: EmergencyResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencyResponse = await _context.EmergencyResponses
                .Include(e => e.Nurse)
                .Include(e => e.Patient)
                .FirstOrDefaultAsync(m => m.EmergencyResponseId == id);
            if (emergencyResponse == null)
            {
                return NotFound();
            }

            return View(emergencyResponse);
        }

        // POST: EmergencyResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emergencyResponse = await _context.EmergencyResponses.FindAsync(id);
            if (emergencyResponse != null)
            {
                _context.EmergencyResponses.Remove(emergencyResponse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmergencyResponseExists(int id)
        {
            return _context.EmergencyResponses.Any(e => e.EmergencyResponseId == id);
        }
    }
}
