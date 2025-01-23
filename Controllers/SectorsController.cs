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
    public class SectorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sectors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sectors.Include(s => s.MedicalCenter);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sectors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors
                .Include(s => s.MedicalCenter)
                .FirstOrDefaultAsync(m => m.SectorId == id);
            if (sector == null)
            {
                return NotFound();
            }

            return View(sector);
        }

        // GET: Sectors/Create
        public IActionResult Create()
        {
            ViewData["MedicalCenterId"] = new SelectList(_context.MedicalCenters, "MedicalCenterId", "MedicalCenterId");
            return View();
        }

        // POST: Sectors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SectorId,Name,MedicalCenterId")] Sector sector)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicalCenterId"] = new SelectList(_context.MedicalCenters, "MedicalCenterId", "MedicalCenterId", sector.MedicalCenterId);
            return View(sector);
        }

        // GET: Sectors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }
            ViewData["MedicalCenterId"] = new SelectList(_context.MedicalCenters, "MedicalCenterId", "MedicalCenterId", sector.MedicalCenterId);
            return View(sector);
        }

        // POST: Sectors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SectorId,Name,MedicalCenterId")] Sector sector)
        {
            if (id != sector.SectorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectorExists(sector.SectorId))
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
            ViewData["MedicalCenterId"] = new SelectList(_context.MedicalCenters, "MedicalCenterId", "MedicalCenterId", sector.MedicalCenterId);
            return View(sector);
        }

        // GET: Sectors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors
                .Include(s => s.MedicalCenter)
                .FirstOrDefaultAsync(m => m.SectorId == id);
            if (sector == null)
            {
                return NotFound();
            }

            return View(sector);
        }

        // POST: Sectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sector = await _context.Sectors.FindAsync(id);
            if (sector != null)
            {
                _context.Sectors.Remove(sector);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectorExists(int id)
        {
            return _context.Sectors.Any(e => e.SectorId == id);
        }
    }
}
