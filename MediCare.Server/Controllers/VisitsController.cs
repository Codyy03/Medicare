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
                Status = VisitStatus.Scheduled,
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes
            });
        }
        [HttpGet("roomsBySpecialization/{specId}")]
        public async Task<ActionResult<List<RoomDto>>> GetFreeRoomsBySpecialization(
     int specId,
     [FromQuery] DateOnly date,
     [FromQuery] TimeOnly time)
        {
            var rooms = await context.SpecializationRooms
                .Where(sr => sr.SpecializationID == specId)
                .Select(sr => sr.Room)
                .Distinct()
                .ToListAsync();

            if (!rooms.Any())
                return NotFound("No rooms assigned to this specialization.");

            // sprawdzaj tylko wizyty zaczynające się dokładnie o tej godzinie
            var occupiedRoomIds = await context.Visits
                .Where(v =>
                    v.SpecializationID == specId &&
                    v.VisitDate == date &&
                    v.VisitTime == time
                )
                .Select(v => v.RoomID)
                .Distinct()
                .ToListAsync();

            var freeRooms = rooms
                .Where(r => !occupiedRoomIds.Contains(r.ID))
                .Select(r => new RoomDto
                {
                    ID = r.ID,
                    RoomType = r.RoomType,
                    RoomNumber = r.RoomNumber
                })
                .OrderBy(r => r.RoomNumber)
                .ToList();

            if (!freeRooms.Any())
                return NotFound("No free rooms available for this specialization at this time.");

            return Ok(freeRooms);
        }


        [HttpGet("visitsTime")]
        public async Task<ActionResult<List<VisitTimeDto>>> GetVisitsTime(
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

            if (dto.VisitDate <= DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Visit date must be at least tomorrow.");

            // 1️⃣ Pobierz lekarza z jego specjalizacjami
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == dto.DoctorID);

            if (doctor == null)
                return NotFound($"Doctor with ID {dto.DoctorID} not found.");

            // 2️⃣ Sprawdź pacjenta
            var patientExists = await context.Patients.AnyAsync(p => p.ID == dto.PatientID);
            if (!patientExists)
                return NotFound($"Patient with ID {dto.PatientID} not found.");

            // 3️⃣ Sprawdź kolizję terminów
            bool conflict = await context.Visits.AnyAsync(v =>
                v.DoctorID == dto.DoctorID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime);

            if (conflict)
                return Conflict("This doctor already has a visit scheduled at the given time.");

            // 4️⃣ Znajdź dostępny pokój i specjalizację
            var specializationIds = doctor.Specializations.Select(s => s.ID).ToList();

            var availableRoom = await context.SpecializationRooms
                .Where(sr => specializationIds.Contains(sr.SpecializationID))
                .FirstOrDefaultAsync(sr =>
                    !context.Visits.Any(v =>
                        v.RoomID == sr.RoomID &&
                        v.VisitDate == dto.VisitDate &&
                        v.VisitTime == dto.VisitTime));

            if (availableRoom == null)
                return BadRequest("No room available for this doctor's specializations.");

            int roomId = availableRoom.RoomID;
            int specializationId = availableRoom.SpecializationID;

            // 5️⃣ Walidacja enuma Reason
            if (!Enum.IsDefined(typeof(VisitReason), dto.Reason))
                return BadRequest("Invalid visit reason.");

            // 6️⃣ Utworzenie wizyty
            var visit = new Visit
            {
                VisitDate = dto.VisitDate,
                VisitTime = dto.VisitTime,
                DoctorID = dto.DoctorID,
                PatientID = dto.PatientID,
                Status = VisitStatus.Scheduled,
                RoomID = roomId,
                SpecializationID = specializationId,
                Reason = (VisitReason)dto.Reason,
                AdditionalNotes = dto.AdditionalNotes
            };

            context.Visits.Add(visit);
            await context.SaveChangesAsync();

            // 7️⃣ Odpowiedź
            var doctorName = $"{doctor.Name} {doctor.Surname}";
            var patientName = (await context.Patients.FindAsync(dto.PatientID))?.Name ?? "";
            var room = (await context.Rooms.FindAsync(roomId))?.RoomType ?? "";

            return CreatedAtAction(nameof(GetVisitById), new { id = visit.ID }, new VisitResponseDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = doctorName,
                Specialization = string.Join(", ", doctor.Specializations.Select(s => s.SpecializationName)),
                PatientName = patientName,
                Room = room,
                Status = visit.Status,
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes
            });
        }

        [HttpGet("freeRoomsForDay/{specId}")]
        public async Task<ActionResult<Dictionary<string, List<RoomDto>>>> GetFreeRoomsForDay(
     int specId,
     [FromQuery] DateOnly date)
        {
            var rooms = await context.SpecializationRooms
                .Where(sr => sr.SpecializationID == specId)
                .Select(sr => sr.Room)
                .Distinct()
                .ToListAsync();

            if (!rooms.Any())
                return NotFound("No rooms assigned to this specialization.");

            var roomIds = rooms.Select(r => r.ID).ToList();

            var result = new Dictionary<string, List<RoomDto>>();
            var start = new TimeOnly(8, 0);
            var end = new TimeOnly(16, 0);

            for (var current = start; current < end; current = current.AddMinutes(30))
            {
                var slotStartMinutes = current.Hour * 60 + current.Minute;
                var slotEndMinutes = slotStartMinutes + 30;

                var occupiedRoomIds = await context.Visits
                    .Where(v =>
                        roomIds.Contains(v.RoomID) &&
                        v.VisitDate == date &&
                        (v.VisitTime.Hour * 60 + v.VisitTime.Minute) < slotEndMinutes &&
                        ((v.VisitTime.Hour * 60 + v.VisitTime.Minute) + 30) > slotStartMinutes
                    )
                    .Select(v => v.RoomID)
                    .Distinct()
                    .ToListAsync();

                var freeRooms = rooms
                    .Where(r => !occupiedRoomIds.Contains(r.ID))
                    .Select(r => new RoomDto
                    {
                        ID = r.ID,
                        RoomType = r.RoomType,
                        RoomNumber = r.RoomNumber
                    })
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

                result[current.ToString("HH:mm")] = freeRooms;
            }

            return Ok(result);
        }


        [HttpPost("checkFreeRooms")]
        public async Task<ActionResult<List<RoomDto>>> CheckFreeRooms([FromBody] VisitCreateDto dto)
        {
            var doctor = await context.Doctors
    .Include(d => d.Specializations)
    .FirstOrDefaultAsync(d => d.ID == dto.DoctorID);

            if (doctor == null)
                return NotFound("Doctor not found.");

            var specializationIds = doctor.Specializations.Select(s => s.ID).ToList();
            if (!specializationIds.Any())
                return BadRequest("Doctor has no specializations.");

            // np. wybierasz pierwszą specjalizację
            int specializationId = specializationIds.First();

            // pobierz wszystkie pokoje przypisane do specjalizacji
            var rooms = await context.SpecializationRooms
                .Where(sr => sr.SpecializationID == specializationId)
                .Select(sr => sr.Room)
                .Distinct()
                .ToListAsync();

            if (!rooms.Any())
                return NotFound("No rooms assigned to this specialization.");

            // sprawdź czy lekarz ma już wizytę w tym czasie
            bool conflict = await context.Visits.AnyAsync(v =>
                v.DoctorID == dto.DoctorID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime);

            if (conflict)
                return Conflict("This doctor already has a visit scheduled at the given time.");

            // sprawdź zajęte pokoje (niezależnie od lekarza)
            var occupiedRoomIds = await context.Visits
                .Where(v =>
                    v.VisitDate == dto.VisitDate &&
                    v.VisitTime == dto.VisitTime)
                .Select(v => v.RoomID)
                .Distinct()
                .ToListAsync();

            // zwróć wolne pokoje
            var freeRooms = rooms
                .Where(r => !occupiedRoomIds.Contains(r.ID))
                .Select(r => new RoomDto
                {
                    ID = r.ID,
                    RoomType = r.RoomType,
                    RoomNumber = r.RoomNumber
                })
                .OrderBy(r => r.RoomNumber)
                .ToList();

            return Ok(freeRooms);
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
            public VisitStatus Status { get; set; }
            public string Reason { get; set; }
            public string? AdditionalNotes { get; set; }
        }
        public class RoomDto
        {
            public int ID { get; set; }
            public string RoomType { get; set; } = string.Empty;
            public int RoomNumber { get; set; }
        }

        public class VisitCheckDto
        {
            public DateOnly VisitDate { get; set; }
            public TimeOnly VisitTime { get; set; }
            public int DoctorID { get; set; }
            public int SpecializationID { get; set; }
        }


    }
}
