using AppointAid.Models;
using System.Collections.Generic;

namespace AppointAid.ViewModels
{
    public class PatientHomeViewModel
    {
        public string PatientName { get; set; }
        public IEnumerable<Appointment> UpcomingAppointments { get; set; }
        public IEnumerable<MedicalTest> UpcomingTests { get; set; }

        public string RenderUpcomingAppointments()
        {
            string html = string.Empty;
            foreach (var appointment in UpcomingAppointments)
            {
                html += $@"
                <div class='card mb-3'>
                    <div class='card-body'>
                        <p>Doctor: {appointment.DoctorId}</p>
                        <p>Sector: {appointment.PatientReport.SectorId}</p>
                        <p>Date: {appointment.TimeSlot}</p>
                        <div>
                            <a href='#' class='btn btn-primary'>Get Location</a>
                            <a href='#' class='btn btn-warning'>Reschedule</a>
                            <a href='#' class='btn btn-danger'>Cancel</a>
                        </div>
                    </div>
                </div>";
            }
            return html;
        }

        public string RenderUpcomingTests()
        {
            string html = string.Empty;
            foreach (var test in UpcomingTests)
            {
                html += $@"
                <div class='card mb-3'>
                    <div class='card-body'>
                        <p>Test: {test.TestName}</p>
                        <p>Sector: {test.Doctor.SectorId}</p>
                        <p>Location: {test.Doctor.Sector.MedicalCenter.Location}</p>
                        <p>Date: {test.ScheduledDate}</p>
                        <p>Time: {test.ScheduledTime}</p>
                        <div>
                            <a href='#' class='btn btn-primary'>Get Location</a>
                            <a href='#' class='btn btn-warning'>Reschedule</a>
                            <a href='#' class='btn btn-danger'>Cancel</a>
                        </div>
                    </div>
                </div>";
            }
            return html;
        }
    }
}