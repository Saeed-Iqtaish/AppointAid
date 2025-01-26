using AppointAid.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointAid.ViewModels
{
    public class BookAppointmentViewModel
    {
        public int ReportId { get; set; }
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime PreferredDate { get; set; }
        public TimeSpan PreferredTime { get; set; }
        public Sector? Sector { get; set; }
        public List<SelectListItem>? Doctors { get; set; }
    }
}