using MediCare.Server.Data;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing medical visits in the MediCare system.
    /// Provides endpoints to schedule, retrieve, update, and cancel patient visits.
    /// </summary>
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

        /// <summary>
        /// Retrieves all visits from the database with related doctor, patient, room, and specialization information.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of <see cref="VisitResponseDto"/> objects. 
        /// Each visit includes details such as date, time, doctor name, patient name, specialization, room, status, reason, 
        /// and any additional notes.
        /// </remarks>
        /// <returns>
        /// A list of visits wrapped in an <see cref="ActionResult"/>. 
        /// Returns HTTP 200 (OK) with the list of visits.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<List<VisitResponseDto>>> GetVisits()
        {
            var visits = await context.Visits
                .Include(v => v.Doctor)
                    .ThenInclude(d => d.Specializations)
                .Include(v => v.Patient)
                .Include(v => v.Room)
                .Select(visit => new VisitResponseDto
                {
                    ID = visit.ID,
                    VisitDate = visit.VisitDate,
                    VisitTime = visit.VisitTime,
                    DoctorName = $"{visit.Doctor.Name} {visit.Doctor.Surname}",
                    Specialization = visit.Specialization.SpecializationName,
                    PatientName = $"{visit.Patient.Name} {visit.Patient.Surname}",
                    Room = visit.Room.RoomType,
                    Status = visit.Status.ToString(),
                    Reason = visit.Reason.ToString(),
                    AdditionalNotes = visit.AdditionalNotes
                })
                .ToListAsync();

            return Ok(visits);
        }

        /// <summary>
        /// Retrieves a single visit by its ID, including doctor, patient, room, and specialization details.
        /// </summary>
        /// <param name="id">The unique identifier of the visit.</param>
        /// <returns>A <see cref="VisitResponseDto"/> with visit details, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VisitResponseDto>> GetVisitById(int id)
        {
            var visit = await context.Visits
                .Include(v => v.Doctor).ThenInclude(d => d.Specializations)
                .Include(v => v.Patient)
                .Include(v => v.Room)
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
                Status = visit.Status.ToString(),
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes,
                VisitNotes = visit.VisitNotes,
                PrescriptionText = visit.PrescriptionText
            });
        }

        /// <summary>
        /// Retrieves all visit times for a given doctor on a specific date.
        /// </summary>
        /// <param name="id">The doctor's ID.</param>
        /// <param name="date">The date for which to fetch visit times.</param>
        /// <returns>A list of <see cref="VisitTimeDto"/> objects with time and room info.</returns>
        [HttpGet("visitsTime")]
        public async Task<ActionResult<List<VisitTimeDto>>> GetVisitsTime(
            [FromQuery] int id,
            [FromQuery] DateOnly date)
        {
            var visitsTime = await context.Visits
                .Where(v => v.DoctorID == id
                         && v.VisitDate == date
                         && v.Status != VisitStatus.Cancelled)
                .Select(v => new VisitTimeDto
                {
                    VisitTime = v.VisitTime,
                    Room = v.Room.RoomType
                })
                .ToListAsync();

            return Ok(visitsTime);
        }

        /// <summary>
        /// Retrieves all visits for the currently authenticated doctor.
        /// Includes patient, room, and specialization details.
        /// </summary>
        /// <returns>A list of <see cref="DoctorVisitsDto"/> objects.</returns>
        [HttpGet("doctor")]
        public async Task<ActionResult<List<DoctorVisitsDto>>> GetDoctorVisits()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var doctorId = int.Parse(userId);

            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == doctorId);

            if (doctor == null) return NotFound();

            var visits = await context.Visits
                .Where(v => v.DoctorID == doctor.ID)
                .Include(v => v.Patient)
                .Include(v => v.Room)
                .OrderBy(v => v.VisitDate)
                .ToListAsync();

            var result = visits.Select(visit => new DoctorVisitsDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = $"{doctor.Name} {doctor.Surname}",
                Specialization = string.Join(", ", doctor.Specializations.Select(s => s.SpecializationName)),
                PatientName = $"{visit.Patient.Name} {visit.Patient.Surname}",
                Room = visit.Room.RoomType,
                RoomNumber = visit.Room.RoomNumber,
                Status = visit.Status.ToString(),
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all visits for the currently authenticated patient.
        /// Includes doctor, room, and specialization details.
        /// </summary>
        /// <returns>A list of <see cref="VisitResponseDto"/> objects.</returns>
        [HttpGet("patient")]
        public async Task<ActionResult<List<VisitResponseDto>>> GetPatientVisits()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            int patientId = int.Parse(userId);

            List<Visit> visits = await context.Visits
                .Where(v => v.PatientID == patientId)
                .Include(v => v.Doctor)
                .Include(v => v.Room)
                .Include(v => v.Specialization)
                .ToListAsync();

            List<VisitResponseDto> result = visits.Select(visit => new VisitResponseDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = $"{visit.Doctor.Name} {visit.Doctor.Surname}",
                Specialization = visit.Specialization.SpecializationName,
                Room = $"{visit.Room.RoomType} {visit.Room.RoomNumber}",
                Status = visit.Status.ToString(),
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes,
                PrescriptionText = visit.PrescriptionText,
                VisitNotes = visit.VisitNotes

            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all scheduled visits for the authenticated doctor on the current day.
        /// Includes patient, room, and specialization details.
        /// </summary>
        /// <returns>A <see cref="TodayVisitsResponse"/> containing visits and doctor specializations.</returns>
        [HttpGet("visitsToday")]
        public async Task<ActionResult<TodayVisitsResponse>> GetTodayVisits()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var doctorId = int.Parse(userId);

            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == doctorId);

            if (doctor == null) return NotFound();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var visits = await context.Visits
                .Where(v => v.DoctorID == doctor.ID && v.VisitDate == today && v.Status == VisitStatus.Scheduled)
                .Include(v => v.Patient)
                .Include(v => v.Room)
                .Include(v => v.Specialization)
                .OrderBy(v => v.VisitTime)
                .ToListAsync();

            var result = new TodayVisitsResponse
            {
                Visits = visits.Select(visit => new TodayVisitsDto
                {
                    ID = visit.ID,
                    PatientName = $"{visit.Patient.Name} {visit.Patient.Surname}",
                    Specialization = visit.Specialization.SpecializationName,
                    Reason = visit.Reason.ToString(),
                    Room = $"{visit.Room.RoomType} {visit.Room.RoomNumber}",
                    VisitTime = visit.VisitTime,
                }).ToList(),

                Specializations = doctor.Specializations
                    .Select(s => new SpecializationDto
                    {
                        ID = s.ID,
                        Name = s.SpecializationName
                    })
                    .ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing visit record in the database.
        /// </summary>
        /// <remarks>
        /// This endpoint allows administrators to modify selected fields of a visit, including:
        /// - Date and time of the visit
        /// - Status (Scheduled, Completed, Cancelled)
        /// - Reason (Consultation, Follow-up, Prescription, Checkup)
        /// - Additional notes, prescription text, and visit notes
        ///
        /// The doctor, patient, room, and specialization associations remain unchanged.
        /// </remarks>
        /// <param name="id">The unique identifier of the visit to update. Must match the ID in the request body.</param>
        /// <param name="dto">The <see cref="VisitEditDto"/> containing updated visit details.</param>
        /// <returns>
        /// Returns HTTP 204 (No Content) if the update succeeds.  
        /// Returns HTTP 400 (Bad Request) if the ID does not match or if invalid status/reason values are provided.  
        /// Returns HTTP 404 (Not Found) if the visit with the given ID does not exist.
        /// </returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] VisitEditDto dto)
        {
            if (id != dto.ID)
                return BadRequest("ID mismatch");

            var visit = await context.Visits.FindAsync(id);
            if (visit == null)
                return NotFound();

            visit.VisitDate = DateOnly.FromDateTime(dto.VisitDate);
            visit.VisitTime = TimeOnly.Parse(dto.VisitTime);
            visit.AdditionalNotes = dto.AdditionalNotes;

            if (Enum.TryParse<VisitStatus>(dto.Status, out var status))
                visit.Status = status;
            else
                return BadRequest("Invalid status value");

            if (Enum.TryParse<VisitReason>(dto.Reason, out var reason))
                visit.Reason = reason;
            else
                return BadRequest("Invalid reason value");

            visit.PrescriptionText = dto.PrescriptionText;
            visit.VisitNotes = dto.VisitNotes;

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Marks a visit as started and completed by updating notes and prescription.
        /// </summary>
        /// <param name="id">The visit ID.</param>
        /// <param name="dto">The notes and prescription details.</param>
        /// <returns>200 OK if updated, or 404 if not found.</returns>
        [HttpPut("startVisit/{id}")]
        public async Task<IActionResult> StartVisit(int id, [FromBody] StartVisitDto dto)
        {
            var visit = await context.Visits.Include(v => v.Patient).FirstOrDefaultAsync(v => v.ID == id);
            if (visit == null) return NotFound();

            visit.VisitNotes = dto.Notes;
            visit.PrescriptionText = dto.Prescription;
            visit.Status = VisitStatus.Completed;

            await context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Cancels a scheduled patient visit by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the visit to cancel.</param>
        /// <returns>
        /// 200 OK with the updated <see cref="VisitResponseDto"/> if the visit was successfully cancelled.  
        /// 404 NotFound if the visit does not exist.  
        /// 400 BadRequest if
        [HttpPost("canceledVisit/{id}")]
        public async Task<IActionResult> CancelPatientVisit(int id)
        {
            var visit = await context.Visits
             .Include(v => v.Doctor)
             .Include(v => v.Specialization)
             .Include(v => v.Room)
             .FirstOrDefaultAsync(v => v.ID == id);


            if (visit == null) return NotFound();

            if (visit.Status == VisitStatus.Completed || visit.Status == VisitStatus.Cancelled)
                return BadRequest();

            visit.Status = VisitStatus.Cancelled;
            await context.SaveChangesAsync();

            VisitResponseDto dto = new VisitResponseDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = $"{visit.Doctor.Name} {visit.Doctor.Surname}",
                Specialization = visit.Specialization.SpecializationName,
                Room = $"{visit.Room.RoomType} {visit.Room.RoomNumber}",
                Status = visit.Status.ToString(),
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes,
                PrescriptionText = visit.PrescriptionText,
                VisitNotes = visit.VisitNotes
            };

            return Ok(dto);
        }

        /// <summary>
        /// Creates a new visit for a patient with a doctor, specialization, and room.
        /// Validates conflicts, specialization-room mapping, and visit reason.
        /// </summary>
        /// <param name="dto">The visit creation data transfer object.</param>
        /// <returns>A <see cref="VisitResponseDto"/> with created visit details, or an error if validation fails.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateVisit([FromBody] VisitCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.VisitDate <= DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Visit date must be at least tomorrow.");

            // Download a doctor with his specializations
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == dto.DoctorID);

            if (doctor == null)
                return NotFound($"Doctor with ID {dto.DoctorID} not found.");

            // check if the doctor has this specialization
            if (!doctor.Specializations.Any(s => s.ID == dto.SpecializationID))
                return BadRequest("This doctor does not have the selected specialization.");

            // check patient
            var patientExists = await context.Patients.AnyAsync(p => p.ID == dto.PatientID);
            if (!patientExists)
                return NotFound($"Patient with ID {dto.PatientID} not found.");

            // Check doctor appointment conflicts (ignore cancelled visits)
            bool doctorConflict = await context.Visits.AnyAsync(v =>
                v.DoctorID == dto.DoctorID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime &&
                v.Status != VisitStatus.Cancelled);

            if (doctorConflict)
                return Conflict("This doctor already has a visit scheduled at the given time.");

            // Check if the room belongs to this specialization
            bool roomValid = await context.SpecializationRooms
                .AnyAsync(sr => sr.SpecializationID == dto.SpecializationID && sr.RoomID == dto.RoomID);

            if (!roomValid)
                return BadRequest("Selected room is not valid for this specialization.");

            // Check room collision (ignore cancelled visits)
            bool roomConflict = await context.Visits.AnyAsync(v =>
                v.RoomID == dto.RoomID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime &&
                v.Status != VisitStatus.Cancelled);

            if (roomConflict)
                return Conflict("This room is already occupied at the given time.");

            // Reason enum validation
            if (!Enum.IsDefined(typeof(VisitReason), dto.Reason))
                return BadRequest("Invalid visit reason.");

            // create visit
            var visit = new Visit
            {
                VisitDate = dto.VisitDate,
                VisitTime = dto.VisitTime,
                DoctorID = dto.DoctorID,
                PatientID = dto.PatientID,
                Status = VisitStatus.Scheduled,
                RoomID = dto.RoomID,
                SpecializationID = dto.SpecializationID,
                Reason = (VisitReason)dto.Reason,
                AdditionalNotes = dto.AdditionalNotes
            };

            context.Visits.Add(visit);
            await context.SaveChangesAsync();

            var doctorName = $"{doctor.Name} {doctor.Surname}";
            var patientName = (await context.Patients.FindAsync(dto.PatientID))?.Name ?? "";
            var room = (await context.Rooms.FindAsync(dto.RoomID))?.RoomType ?? "";

            return CreatedAtAction(nameof(GetVisitById), new { id = visit.ID }, new VisitResponseDto
            {
                ID = visit.ID,
                VisitDate = visit.VisitDate,
                VisitTime = visit.VisitTime,
                DoctorName = doctorName,
                Specialization = doctor.Specializations.First(s => s.ID == dto.SpecializationID).SpecializationName,
                PatientName = patientName,
                Room = room,
                Status = visit.Status.ToString(),
                Reason = visit.Reason.ToString(),
                AdditionalNotes = visit.AdditionalNotes
            });
        }

        /// <summary>
        /// Retrieves all free rooms for a given specialization and doctor on a specific day.
        /// Returns a dictionary mapping time slots to available rooms.
        /// </summary>
        /// <param name="specId">The specialization ID.</param>
        /// <param name="doctorId">The doctor ID.</param>
        /// <param name="date">The date to check availability.</param>
        /// <returns>A dictionary of time slots with lists of free rooms.</returns>
        [HttpGet("freeRoomsForDay/{specId}")]
        public async Task<ActionResult<Dictionary<string, List<RoomDto>>>> GetFreeRoomsForDay(
            int specId,
            [FromQuery] int doctorId,
            [FromQuery] DateOnly date)
        {
            // check if the doctor has this specialization
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == doctorId);

            if (doctor == null)
                return NotFound("Doctor not found.");

            if (!doctor.Specializations.Any(s => s.ID == specId))
                return BadRequest("This doctor does not have the selected specialization.");

            // get rooms assigned to this specialization
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

                // only uncancelled visits block the slot
                var occupiedRoomIds = await context.Visits
                    .Where(v =>
                        roomIds.Contains(v.RoomID) &&
                        v.VisitDate == date &&
                        v.Status != VisitStatus.Cancelled &&
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

        /// <summary>
        /// Checks free rooms for a specific doctor, specialization, date, and time slot.
        /// Ensures the doctor has the specialization and validates conflicts.
        /// </summary>
        /// <param name="dto">The visit creation DTO containing doctor, specialization, date, and time.</param>
        /// <returns>A list of available <see cref="RoomDto"/> objects for the given slot.</returns>
        [HttpPost("checkFreeRooms")]
        public async Task<ActionResult<List<RoomDto>>> CheckFreeRooms([FromBody] VisitCreateDto dto)
        {
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == dto.DoctorID);

            if (doctor == null)
                return NotFound("Doctor not found.");

            if (!doctor.Specializations.Any(s => s.ID == dto.SpecializationID))
                return BadRequest("This doctor does not have the selected specialization.");

            var rooms = await context.SpecializationRooms
                .Where(sr => sr.SpecializationID == dto.SpecializationID)
                .Select(sr => sr.Room)
                .Distinct()
                .ToListAsync();

            if (!rooms.Any())
                return NotFound("No rooms assigned to this specialization.");

            bool conflict = await context.Visits.AnyAsync(v =>
                v.DoctorID == dto.DoctorID &&
                v.VisitDate == dto.VisitDate &&
                v.VisitTime == dto.VisitTime);

            if (conflict)
                return Conflict("This doctor already has a visit scheduled at the given time.");

            var occupiedRoomIds = await context.Visits
                .Where(v =>
                    v.VisitDate == dto.VisitDate &&
                    v.VisitTime == dto.VisitTime)
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

            return Ok(freeRooms);
        }

        /// <summary>
        /// DTO representing a doctor's visit time and assigned room.
        /// Used when fetching occupied slots for a given doctor and date.
        public class VisitTimeDto
        {
            public required TimeOnly VisitTime { get; set; }
            public string Room { get; set; } = string.Empty;
        }

        /// <summary>
        /// DTO used when creating a new visit.
        /// Contains all required information for scheduling and validation.
        /// </summary>
        public class VisitCreateDto
        {
            public required DateOnly VisitDate { get; set; }
            public required int DoctorID { get; set; }
            public required int PatientID { get; set; }
            public required TimeOnly VisitTime { get; set; }
            public string? AdditionalNotes { get; set; }
            public required int Reason { get; set; }
            public required int SpecializationID { get; set; }
            public required int RoomID { get; set; }

        }

        /// <summary>
        /// DTO returned after creating or fetching a visit.
        /// Contains detailed information about the scheduled appointment.
        /// </summary>
        public class VisitResponseDto
        {
            public required int ID { get; set; }
            public required DateOnly VisitDate { get; set; }
            public required TimeOnly VisitTime { get; set; }
            public required string DoctorName { get; set; }
            public required string Specialization { get; set; }
            public string PatientName { get; set; }
            public required string Room { get; set; }
            public required string Status { get; set; }
            public required string Reason { get; set; }
            public string? AdditionalNotes { get; set; }
            public string? VisitNotes { get; set; }
            public string? PrescriptionText { get; set; }
        }

        /// <summary>
        /// DTO representing edit data for a visit.
        /// </summary>
        public class VisitEditDto
        {
            public int ID { get; set; }
            public DateTime VisitDate { get; set; }
            public string VisitTime { get; set; } = string.Empty;
            public string? AdditionalNotes { get; set; }
            public required string Status { get; set; }
            public required string Reason { get; set; }
            public string? VisitNotes { get; set; }
            public string? PrescriptionText { get; set; }
        }

        /// <summary>
        /// DTO representing today's visits for a doctor.
        /// </summary>
        public class TodayVisitsDto
        {
            public required int ID { get; set; }
            public required TimeOnly VisitTime { get; set; }
            public required string PatientName { get; set; }
            public required string Reason { get; set; }
            public required string Room { get; set; }
            public required string Specialization { get; set; }
        }

        /// <summary>
        /// DTO returned by the visitsToday endpoint.
        /// Contains today's visits and the doctor's specializations.
        /// </summary>
        public class TodayVisitsResponse
        {
            public List<TodayVisitsDto> Visits { get; set; } = new();
            public List<SpecializationDto> Specializations { get; set; } = new();
        }

        /// <summary>
        /// DTO used when starting a visit.
        /// Contains optional notes and prescription text.
        /// </summary>
        public class StartVisitDto
        {
            public string? Notes { get; set; }
            public string? Prescription { get; set; }
        }

        /// <summary>
        /// DTO representing a medical specialization.
        /// Used to return specialization information such as ID and name
        /// when fetching visits or doctor details.
        /// </summary>
        public class SpecializationDto
        {
            public int ID { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// DTO representing a doctor's visit with patient and room details.
        /// </summary>
        public class DoctorVisitsDto
        {
            public int ID { get; set; }
            public DateOnly VisitDate { get; set; }
            public TimeOnly VisitTime { get; set; }
            public required string DoctorName { get; set; }
            public required string Specialization { get; set; }
            public required string PatientName { get; set; }
            public required string Room { get; set; }
            public int RoomNumber { get; set; }
            public required string Status { get; set; }
            public required string Reason { get; set; }
            public string? AdditionalNotes { get; set; }
        }
        /// <summary>
        /// DTO representing a consultation room.
        /// </summary>
        public class RoomDto
        {
            public int ID { get; set; }
            public string RoomType { get; set; } = string.Empty;
            public int RoomNumber { get; set; }
        }

        /// <summary>
        /// DTO used to check free rooms for a specific doctor, specialization, date, and time.
        /// </summary>
        public class VisitCheckDto
        {
            public DateOnly VisitDate { get; set; }
            public TimeOnly VisitTime { get; set; }
            public int DoctorID { get; set; }
            public int SpecializationID { get; set; }
        }
    }
}
