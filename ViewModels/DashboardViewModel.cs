using AppointAid.Models;

public class DashboardViewModel
{
    public List<Appointment> ApprovedAppointments { get; set; } = new List<Appointment>();
    public List<MedicalTest> ApprovedTests { get; set; } = new List<MedicalTest>();
    public List<PatientReport> ApprovedReports { get; set; } = new List<PatientReport>();
}