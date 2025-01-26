using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppointAid.Data;
using AppointAid.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointAid.Services
{
    public class PatientHistoryService
    {
        private readonly ApplicationDbContext _context;

        public PatientHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> GetPatientAsync(int patientId)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<MedicalTest>> GetMedicalTestsAsync(int patientId)
        {
            return await _context.MedicalTests
                .Where(mt => mt.PatientId == patientId)
                .ToListAsync();
        }
    }
}