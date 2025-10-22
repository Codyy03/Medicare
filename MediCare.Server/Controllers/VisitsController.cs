using MediCare.Server.Data;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitsController : ControllerBase
    {
        readonly MediCareDbContext context;
        readonly JwtTokenHelper jwtHelper;
        public VisitsController(MediCareDbContext context, JwtTokenHelper jwtHelper)
        {
            this.context = context;
            this.jwtHelper = jwtHelper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VisitResponseDto>> GetVisitById(int id)
        {
            var visit = await context.Visits
                .Include(v => v.Doctor).ThenInclude(d => d.Specializations)
                .Include(v => v.Patient)
                .Include(v => v.Room)
                .Include(v => v.Status)
                .FirstOrDefaultAsync(v => v.ID == id);

            if (visit == null)
                return NotFound();

            return Ok(new VisitResponseDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = $"{visit.Doctor.Name} {visit.Doctor.Surname}",
                Specialization = string.Join(", ", visit.Doctor.Specializations.Select(s => s.SpecializationName)),
                PatientName = visit.Patient.Name,
                Room = visit.Room.RoomType,
                Status = visit.Status.Name,
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes
            });
        }

        [HttpGet("roomBySpecialization/{specId}")]
        public async Task<ActionResult<RoomDto>> GetRoomBySpecialization(int specId)
        {
            var room = await context.SpecializationRooms
                .Where(sr => sr.SpecializationID == specId)
                .Select(sr => new RoomDto
                {
                    RoomType = sr.Room.RoomType,
                    RoomNumber = sr.Room.RoomNumber
                })
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound("No room assigned to this specialization.");

            return Ok(room);
        }

        [HttpGet("visitsTime")]
        public async Task<ActionResult<VisitTimeDto>> GetVisitsTime(
            [FromQuery] int id,
            [FromQuery] DateOnly date)
        {
            var visitsTime = await context.Visits
                .Where(v => v.DoctorID == id && v.VisitDate == date)
                .Select(v => new VisitTimeDto
                {
                    VisitTime = v.VisitTime,
                    Room = v.Room.RoomType
                })
                .ToListAsync();

            return Ok(visitsTime);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVisit([FromBody] VisitCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get doctor with specializations
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == dto.DoctorID);

            if (doctor == null)
                return NotFound($"Doctor with ID {dto.DoctorID} not found.");

            // Check if patient exists
            var patientExists = await context.Patients.AnyAsync(p => p.ID == dto.PatientID);
            if (!patientExists)
                return NotFound($"Patient with ID {dto.PatientID} not found.");

            // Check for scheduling conflicts
            bool conflict = await context.Visits.AnyAsync(v =>
                v.DoctorID == dto.DoctorID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime);

            if (conflict)
                return Conflict("This doctor already has a visit scheduled at the given time.");

            // Assign initial status
            int statusId = 1; // Scheduled

            // Assign room based on doctor's specializations
            var specializationIds = doctor.Specializations.Select(s => s.ID).ToList();

            int roomId = await context.SpecializationRooms
                .Where(sr => specializationIds.Contains(sr.SpecializationID))
                .Select(sr => sr.RoomID)
                .FirstOrDefaultAsync(rid => !context.Visits.Any(v =>
                    v.RoomID == rid &&
                    v.VisitDate == dto.VisitDate &&
                    v.VisitTime == dto.VisitTime));

            if (roomId == 0)
                return BadRequest("No room available for this doctor's specializations.");

            // Validation of visit reason
            if (!Enum.IsDefined(typeof(VisitReason), dto.Reason))
                return BadRequest("Invalid visit reason.");

            // validation of visit date and time
            var errors = ValidateVisitDate(dto.VisitDate.ToDateTime(dto.VisitTime), dto.VisitTime);
            if (errors.Any())
                return BadRequest(errors);




            // Create and save the visit
            var visit = new Visit
            {
                VisitDate = dto.VisitDate,
                VisitTime = dto.VisitTime,
                DoctorID = dto.DoctorID,
                PatientID = dto.PatientID,
                StatusID = statusId,
                RoomID = roomId,
                Reason = (VisitReason)dto.Reason,
                AdditionalNotes = dto.AdditionalNotes
            };

            context.Visits.Add(visit);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVisitById), new { id = visit.ID }, new VisitResponseDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = $"{doctor.Name} {doctor.Surname}",
                Specialization = string.Join(", ", doctor.Specializations.Select(s => s.SpecializationName)),
                PatientName = (await context.Patients.FindAsync(dto.PatientID))?.Name ?? "",
                Room = (await context.Rooms.FindAsync(roomId))?.RoomType ?? "",
                Status = "Scheduled",
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes
            });

        }

        /// <summary>
        /// Validates the visit date and time.
        /// </summary>
        /// <param name="dto.VisitDate">Date of visit</param>
        /// <param name="dto.VisitTime">Time of visit</param>
        /// <returns>
        /// A list of error messages. If the list is empty, the date and time are valid.
        /// </returns>
        List<string> ValidateVisitDateTime(DateTime dto.VisitDate, TimeOnly dto.VisitTime)
        {
            var errors = new List<string>();

            if (dto.VisitDate <= DateTime.Today)
            {
                errors.Add("Visit date must be at least tomorrow.");
            }

            if (dto.VisitDate.DayOfWeek == DayOfWeek.Saturday || dto.VisitDate.DayOfWeek == DayOfWeek.Sunday)
            {
                errors.Add("Visits cannot be scheduled on weekends.");
            }

            if (dto.VisitTime.Hour < 8 || dto.VisitTime.Hour > 16)
            {
                errors.Add("Visits can only be scheduled between 8 AM and 5 PM.");
            }

            if (dto.VisitTime.Minute != 0 && dto.VisitTime.Minute != 30)
            {
                errors.Add("Visits can only be scheduled on the hour or half-hour.");
            }
            
            return errors;
        }


        public class VisitTimeDto
        { 
            public required TimeOnly VisitTime {  get; set; }
            public string Room { get; set; } = string.Empty;
        }

        public class VisitCreateDto
        {
            public required DateOnly VisitDate { get; set; }
            public required int DoctorID { get; set; }
            public required int PatientID { get; set; }
            public required TimeOnly VisitTime { get; set; }
            public string? AdditionalNotes { get; set; }
            public required int Reason { get; set; }

        }
        public class VisitResponseDto
        {
            public int ID { get; set; }
            public DateOnly VisitDate { get; set; }
            public TimeOnly VisitTime { get; set; }
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public string PatientName { get; set; }
            public string Room { get; set; }
            public string Status { get; set; }
            public string Reason { get; set; }
            public string? AdditionalNotes { get; set; }
        }
        public class RoomDto
        {
            public string RoomType { get; set; } = string.Empty;
            public int RoomNumber { get; set; }
        }

    }
}
