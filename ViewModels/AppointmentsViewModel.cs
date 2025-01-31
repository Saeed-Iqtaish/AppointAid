using System.Collections.Generic;
using AppointAid.Models;

namespace AppointAid.ViewModels
{
    public class AppointmentsViewModel
    {
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> PreviousAppointments { get; set; } = new List<Appointment>();
    }
}