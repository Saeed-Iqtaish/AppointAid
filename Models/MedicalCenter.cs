namespace AppointAid.Models
{
    public class MedicalCenter
    {

        public int MedicalCenterId { get; set; }
        public string MedicalCenterName { get; set; }
        public string Location { get; set; }

        public ICollection<Sector>? Sectors { get; set; } = new List<Sector>();
        public ICollection<Nurse>? Nurses { get; set; } = new List<Nurse>();
    }
}
