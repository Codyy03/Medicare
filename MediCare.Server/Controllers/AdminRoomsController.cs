using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AdminRoomsController : ControllerBase
{
    private readonly MediCareDbContext context;

    public AdminRoomsController(MediCareDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Retrieves all rooms along with their associated specializations.
    /// Accessible only by Admin users.
    /// </summary>
    /// <returns>A list of <see cref="RoomDto"/> objects representing all rooms.</returns>
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

    /// <summary>
    /// Deletes a room by its unique identifier.
    /// Accessible only by Admin users.
    /// </summary>
    /// <param name="id">The ID of the room to delete.</param>
    /// <returns>204 No Content if successful, or 404 Not Found if the room does not exist.</returns>
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

    /// <summary>
    /// Creates a new room with the specified details and associated specializations.
    /// Accessible only by Admin users.
    /// </summary>
    /// <param name="model">The DTO containing room details and specializations.</param>
    /// <returns>204 No Content if successful, or 400 Bad Request if validation fails.</returns>
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

    /// <summary>
    /// Retrieves a specific room by its unique identifier, including its specializations.
    /// Accessible only by Admin users.
    /// </summary>
    /// <param name="id">The ID of the room to retrieve.</param>
    /// <returns>A <see cref="RoomDto"/> if found, or 404 Not Found if the room does not exist.</returns>
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

    /// <summary>
    /// Updates an existing room with new details and associated specializations.
    /// Accessible only by Admin users.
    /// </summary>
    /// <param name="model">The DTO containing updated room details and specializations.</param>
    /// <returns>204 No Content if successful, or 404 Not Found if the room does not exist.</returns>
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

    /// <summary>
    /// Data Transfer Object (DTO) representing a room with its details and associated specializations.
    /// </summary>
    public class RoomDto
    {
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public List<SpecializationDto> Specializations { get; set; } = new();
    }

    /// <summary>
    /// Data Transfer Object (DTO) representing a specialization associated with a room.
    /// </summary>
    public class SpecializationDto
    {
        public int ID { get; set; }
        public string SpecializationName { get; set; } = string.Empty;
    }
}