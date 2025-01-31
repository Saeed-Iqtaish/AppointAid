using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class BookAppointmentViewModel
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ReportId { get; set; }

        [Required]
        public int SelectedDoctorId { get; set; }

        [Required]
        public DateTime PreferredDate { get; set; }

        [Required]
        public int SelectedTimeSlotId { get; set; }

        public string SectorName { get; set; }

        [BindNever]
        public IEnumerable<SelectListItem> Doctors { get; set; }

        [BindNever]
        public IEnumerable<SelectListItem> TimeSlots { get; set; }
    }
}