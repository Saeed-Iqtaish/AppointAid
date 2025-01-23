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
    public class DoctorTimeSlotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorTimeSlotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DoctorTimeSlots
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DoctorTimeSlots.Include(d => d.Doctor).Include(d => d.TimeSlot);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DoctorTimeSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorTimeSlot = await _context.DoctorTimeSlots
                .Include(d => d.Doctor)
                .Include(d => d.TimeSlot)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctorTimeSlot == null)
            {
                return NotFound();
            }

            return View(doctorTimeSlot);
        }

        // GET: DoctorTimeSlots/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName");
            ViewData["TimeSlotId"] = new SelectList(_context.TimeSlots, "TimeSlotId", "TimeSlotId");
            return View();
        }

        // POST: DoctorTimeSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,TimeSlotId")] DoctorTimeSlot doctorTimeSlot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorTimeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName", doctorTimeSlot.DoctorId);
            ViewData["TimeSlotId"] = new SelectList(_context.TimeSlots, "TimeSlotId", "TimeSlotId", doctorTimeSlot.TimeSlotId);
            return View(doctorTimeSlot);
        }

        // GET: DoctorTimeSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorTimeSlot = await _context.DoctorTimeSlots.FindAsync(id);
            if (doctorTimeSlot == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName", doctorTimeSlot.DoctorId);
            ViewData["TimeSlotId"] = new SelectList(_context.TimeSlots, "TimeSlotId", "TimeSlotId", doctorTimeSlot.TimeSlotId);
            return View(doctorTimeSlot);
        }

        // POST: DoctorTimeSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("DoctorId,TimeSlotId")] DoctorTimeSlot doctorTimeSlot)
        {
            if (id != doctorTimeSlot.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorTimeSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorTimeSlotExists(doctorTimeSlot.DoctorId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FirstName", doctorTimeSlot.DoctorId);
            ViewData["TimeSlotId"] = new SelectList(_context.TimeSlots, "TimeSlotId", "TimeSlotId", doctorTimeSlot.TimeSlotId);
            return View(doctorTimeSlot);
        }

        // GET: DoctorTimeSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorTimeSlot = await _context.DoctorTimeSlots
                .Include(d => d.Doctor)
                .Include(d => d.TimeSlot)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctorTimeSlot == null)
            {
                return NotFound();
            }

            return View(doctorTimeSlot);
        }

        // POST: DoctorTimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var doctorTimeSlot = await _context.DoctorTimeSlots.FindAsync(id);
            if (doctorTimeSlot != null)
            {
                _context.DoctorTimeSlots.Remove(doctorTimeSlot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorTimeSlotExists(int? id)
        {
            return _context.DoctorTimeSlots.Any(e => e.DoctorId == id);
        }
    }
}
