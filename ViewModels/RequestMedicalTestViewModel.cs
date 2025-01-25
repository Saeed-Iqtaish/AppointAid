using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class RequestMedicalTestViewModel
    {
        [Required]
        [Display(Name = "Medical Center")]
        public int MedicalCenterId { get; set; }

        public List<SelectListItem> MedicalCenters { get; set; } = new List<SelectListItem>();

        [Required]
        [Display(Name = "Test Name")]
        public string TestName { get; set; }

        [Required]
        [Display(Name = "Reason")]
        public string Reason { get; set; }
    }
}