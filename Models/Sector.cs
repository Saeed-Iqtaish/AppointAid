namespace AppointAid.Models
{
    public class Sector
    {
        public int SectorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MedicalCenterId { get; set; }

        public MedicalCenter? MedicalCenter { get; set; }
        public ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();
    }
}
