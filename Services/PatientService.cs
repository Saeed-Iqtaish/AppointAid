using System.Threading.Tasks;
using AppointAid.Data;
using AppointAid.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointAid.Services
{
    public class PatientService
    {
        private readonly ApplicationDbContext _context;

        public PatientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsPatientExistsAsync(string nationalNumber)
        {
            return await _context.Patients.AnyAsync(p => p.NationalNumber == nationalNumber);
        }

        public async Task AddPatientAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }
    }
}