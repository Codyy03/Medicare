using MediCare.Server.Data;
using MediCare.Server.Entities;
//using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
//using static MediCare.Server.Entities.Enums;
[ApiController]
[Route("api/[controller]")]
public class AdminRoomsController : ControllerBase
{
    private readonly MediCareDbContext context;

    public AdminRoomsController(MediCareDbContext context)
    {
        this.context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<Room>>> GetAll()
    {

        var rooms = await context.Rooms
        .Include(r => r.SpecializationRooms)
        .ThenInclude(sr => sr.Specialization)
        .ToListAsync();

        var dto = rooms.Select(r => new RoomDto
        {
            ID = r.ID,
            RoomNumber = r.RoomNumber,
            RoomType = r.RoomType,
            Specializations = r.SpecializationRooms
                .Select(sr => new SpecializationDto
                {
                    ID = sr.Specialization.ID,
                    SpecializationName = sr.Specialization.SpecializationName
                })
                .ToList()
        });

        return Ok(dto);

    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await context.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> Create(RoomDto model)
    {
        if (await context.Rooms.AnyAsync(r => r.RoomNumber == model.RoomNumber))
            return BadRequest("Room number must be unique");

        if (model.RoomNumber < 1)
        {
            return BadRequest("Room number must be positive");
        }
        var room = new Room
        {
            RoomNumber = model.RoomNumber,
            RoomType = model.RoomType,
        };

        context.Rooms.Add(room);
        await context.SaveChangesAsync();
        foreach (SpecializationDto specialization in model.Specializations)
        {
            var SpecializationRoom = new SpecializationRoom
            {
                RoomID = room.ID,
                SpecializationID = specialization.ID
            };

            context.SpecializationRooms.Add(SpecializationRoom);
        }

        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetRoomById(int id)
    {
        var room = await context.Rooms
        .Where(r => r.ID == id)
        .Include(r => r.SpecializationRooms)
        .ThenInclude(sr => sr.Specialization)
        .SingleOrDefaultAsync();

        if (room == null)
            return NotFound();
        var dto = new RoomDto
        {
            ID = room.ID,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            Specializations = room.SpecializationRooms
            .Select(sr => new SpecializationDto
            {
                ID = sr.Specialization.ID,
                SpecializationName = sr.Specialization.SpecializationName
            })
            .ToList()
        };
        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> AdminUpdateRoom(RoomDto model)
    {
        var room = await context.Rooms
            .Include(r => r.SpecializationRooms)
            .SingleOrDefaultAsync(r => r.ID == model.ID);

        if (room == null)
            return NotFound("Room not found");

        // Walidacja numeru pokoju
        if (model.RoomNumber < 1)
            return BadRequest("Room number must be positive");

        if (await context.Rooms.AnyAsync(r => r.RoomNumber == model.RoomNumber && r.ID != model.ID))
            return BadRequest("Room number must be unique");

        room.RoomNumber = model.RoomNumber;
        room.RoomType = model.RoomType;

        context.SpecializationRooms.RemoveRange(room.SpecializationRooms);

        foreach (var spec in model.Specializations)
        {
            var specRoom = new SpecializationRoom
            {
                RoomID = room.ID,
                SpecializationID = spec.ID
            };
            context.SpecializationRooms.Add(specRoom);
        }

        await context.SaveChangesAsync();
        return NoContent();
    }

    public class RoomDto
    {
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public List<SpecializationDto> Specializations { get; set; } = new();
    }

    public class SpecializationDto
    {
        public int ID { get; set; }
        public string SpecializationName { get; set; } = string.Empty;
    }
}