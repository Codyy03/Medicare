using MediCare.Server.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SpecializationRoom
{
    [Key, Column(Order = 0)]
    public int SpecializationID { get; set; }
    public Specialization Specialization { get; set; }

    [Key, Column(Order = 1)]
    public int RoomID { get; set; }
    public Room Room { get; set; }
}
