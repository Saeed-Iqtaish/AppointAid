using System.Collections.Generic;
using AppointAid.Models;

namespace AppointAid.ViewModels
{
    public class PatientHistoryViewModel
    {
        public Patient Patient { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<MedicalTest> MedicalTests { get; set; }
    }
}