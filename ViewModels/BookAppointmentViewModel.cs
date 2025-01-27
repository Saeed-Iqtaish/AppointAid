using AppointAid.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointAid.ViewModels
{
    public class BookAppointmentViewModel
    {
        public int PatientId { get; set; }
        public int ReportId { get; set; }
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public List<SelectListItem> Doctors { get; set; } = new();
        public List<SelectListItem> AvailableDates { get; set; } = new();
        public List<SelectListItem> AvailableTimes { get; set; } = new();
        public int SelectedDoctorId { get; set; }
        public DateTime PreferredDate { get; set; }
        public TimeSpan PreferredTime { get; set; }
    }
}