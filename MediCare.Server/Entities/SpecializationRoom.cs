using MediCare.Server.Entities;

public class SpecializationRoom
{
    public int SpecializationID { get; set; }
    public Specialization Specialization { get; set; }

    public int RoomID { get; set; }
    public Room Room { get; set; }
}
