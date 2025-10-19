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


        [HttpGet("visitsTime")]
        public async Task<ActionResult<VisitTimeDto>> GetVisitsTime(
            [FromQuery] int id,
            [FromQuery] DateOnly date)
        {
            var visitsTime = await context.Visits.Where(v => v.DoctorID == id && v.VisitDate == date)
                .Select(v => new VisitTimeDto
                {
                    VisitTime = v.VisitTime, 
                }).ToListAsync();

            return Ok(visitsTime);
        }
        [HttpPost]
        public async Task<IActionResult> CreateVisit([FromBody] VisitCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.VisitDate <= DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Visit date must be at least tomorrow.");

            // Pobierz lekarza z jego specjalizacjami
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == dto.DoctorID);

            if (doctor == null)
                return NotFound($"Doctor with ID {dto.DoctorID} not found.");

            // Sprawdź pacjenta
            var patientExists = await context.Patients.AnyAsync(p => p.ID == dto.PatientID);
            if (!patientExists)
                return NotFound($"Patient with ID {dto.PatientID} not found.");

            // Sprawdź kolizję terminów
            bool conflict = await context.Visits.AnyAsync(v =>
                v.DoctorID == dto.DoctorID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime);

            if (conflict)
                return Conflict("This doctor already has a visit scheduled at the given time.");

            // Automatyczny status
            int statusId = 1; // Scheduled

            // Automatyczne przypisanie pokoju na podstawie specjalizacji
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

            // Walidacja enuma Reason
            if (!Enum.IsDefined(typeof(VisitReason), dto.Reason))
                return BadRequest("Invalid visit reason.");

            // Utworzenie wizyty
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

        public class VisitTimeDto
        { 
            public required TimeOnly VisitTime {  get; set; }
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

    }
}
