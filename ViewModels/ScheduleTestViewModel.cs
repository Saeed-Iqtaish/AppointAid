using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class ScheduleTestViewModel
    {
        public int TestId { get; set; }
        [Required(ErrorMessage = "Scheduled date is required.")]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "Scheduled time is required.")]
        public TimeSpan ScheduledTime { get; set; }
    }
}