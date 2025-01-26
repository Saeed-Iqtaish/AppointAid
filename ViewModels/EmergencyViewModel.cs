using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class EmergencyRequestViewModel
    {
        [Required]
        public string EmergencyType { get; set; }

        public List<SelectListItem> EmergencyTypes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Accident", Text = "Accident" },
            new SelectListItem { Value = "Heart Attack", Text = "Heart Attack" },
            new SelectListItem { Value = "Severe Injury", Text = "Severe Injury" },
            new SelectListItem { Value = "Other", Text = "Other" }
        };

        [Required]
        public string Description { get; set; }

        public string Location { get; set; }

        public bool EnableGPS { get; set; }
    }
}
