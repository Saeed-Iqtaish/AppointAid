using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class SymptomsViewModel
    {
        [Required(ErrorMessage = "Please select a medical center.")]
        public int MedicalCenterId { get; set; }

        [Required(ErrorMessage = "Please describe your symptoms.")]
        [StringLength(1000, ErrorMessage = "Symptoms description is too long.")]
        public string Symptoms { get; set; }

        public IEnumerable<SelectListItem> MedicalCenters { get; set; }
    }
}