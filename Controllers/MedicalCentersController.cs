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
    public class MedicalCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicalCenters
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicalCenters.ToListAsync());
        }

        // GET: MedicalCenters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalCenter = await _context.MedicalCenters
                .FirstOrDefaultAsync(m => m.MedicalCenterId == id);
            if (medicalCenter == null)
            {
                return NotFound();
            }

            return View(medicalCenter);
        }

        // GET: MedicalCenters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicalCenters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalCenterId,MedicalCenterName,Location")] MedicalCenter medicalCenter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicalCenter);
        }

        // GET: MedicalCenters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalCenter = await _context.MedicalCenters.FindAsync(id);
            if (medicalCenter == null)
            {
                return NotFound();
            }
            return View(medicalCenter);
        }

        // POST: MedicalCenters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalCenterId,MedicalCenterName,Location")] MedicalCenter medicalCenter)
        {
            if (id != medicalCenter.MedicalCenterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalCenter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalCenterExists(medicalCenter.MedicalCenterId))
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
            return View(medicalCenter);
        }

        // GET: MedicalCenters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalCenter = await _context.MedicalCenters
                .FirstOrDefaultAsync(m => m.MedicalCenterId == id);
            if (medicalCenter == null)
            {
                return NotFound();
            }

            return View(medicalCenter);
        }

        // POST: MedicalCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalCenter = await _context.MedicalCenters.FindAsync(id);
            if (medicalCenter != null)
            {
                _context.MedicalCenters.Remove(medicalCenter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalCenterExists(int id)
        {
            return _context.MedicalCenters.Any(e => e.MedicalCenterId == id);
        }
    }
}
