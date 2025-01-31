using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointAid.ViewModels
{
    public class RescheduleAppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string SectorName { get; set; }
        public DateTime SelectedDate { get; set; }
        public int SelectedTimeSlotId { get; set; }

        public IEnumerable<SelectListItem>? AvailableDates { get; set; }
        public IEnumerable<SelectListItem>? AvailableTimeSlots { get; set; }
    }
}