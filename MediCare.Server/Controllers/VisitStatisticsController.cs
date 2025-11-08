using MediCare.Server.Data;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitStatisticsController : ControllerBase
    {
        readonly MediCareDbContext context;
        public VisitStatisticsController(MediCareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves aggregated statistics of all visits in the system.
        /// Returns total count and breakdown by status (Scheduled, Completed, Cancelled).
        /// Useful for dashboards and overall KPI.
        /// </summary>
        [HttpGet("stats/summary")]
        public async Task<ActionResult<VisitStatsDto>> GetVisitSummary()
        {
            var total = await context.Visits.CountAsync();
            var completed = await context.Visits.CountAsync(v => v.Status == VisitStatus.Completed);
            var cancelled = await context.Visits.CountAsync(v => v.Status == VisitStatus.Cancelled);
            var scheduled = await context.Visits.CountAsync(v => v.Status == VisitStatus.Scheduled);

            return Ok(new VisitStatsDto
            {
                Total = total,
                Completed = completed,
                Cancelled = cancelled,
                Scheduled = scheduled
            });
        }

        /// <summary>
        /// Retrieves daily statistics of visits for the last N days (default 30).
        /// Groups visits by date and returns counts per status.
        /// Useful for trend analysis and line charts.
        /// </summary>
        [HttpGet("stats/daily")]
        public async Task<ActionResult<List<DailyVisitStatsDto>>> GetDailyStats([FromQuery] int days = 30)
        {
            var since = DateOnly.FromDateTime(DateTime.Today.AddDays(-days));

            var stats = await context.Visits
                .Where(v => v.VisitDate >= since)
                .GroupBy(v => v.VisitDate)
                .Select(g => new DailyVisitStatsDto
                {
                    Date = g.Key,
                    Total = g.Count(),
                    Completed = g.Count(v => v.Status == VisitStatus.Completed),
                    Cancelled = g.Count(v => v.Status == VisitStatus.Cancelled),
                    Scheduled = g.Count(v => v.Status == VisitStatus.Scheduled)
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            return Ok(stats);
        }

        /// <summary>
        /// Retrieves top doctors ranked by number of visits (excluding cancelled).
        /// Returns doctor name, total visits, and breakdown by status.
        /// Useful for ranking charts and identifying busiest doctors.
        /// </summary>
        [HttpGet("stats/top-doctors")]
        public async Task<ActionResult<List<TopDoctorStatsDto>>> GetTopDoctors([FromQuery] int top = 5)
        {
            var stats = await context.Visits
                .Where(v => v.Status != VisitStatus.Cancelled)
                .GroupBy(v => v.DoctorID)
                .Select(g => new TopDoctorStatsDto
                {
                    DoctorID = g.Key,
                    DoctorName = g.Select(v => v.Doctor.Name + " " + v.Doctor.Surname).FirstOrDefault(),
                    TotalVisits = g.Count(),
                    Completed = g.Count(v => v.Status == VisitStatus.Completed),
                    Scheduled = g.Count(v => v.Status == VisitStatus.Scheduled)
                })
                .OrderByDescending(s => s.TotalVisits)
                .Take(top)
                .ToListAsync();

            return Ok(stats);
        }

        /// <summary>
        /// Retrieves statistics grouped by specialization (excluding cancelled).
        /// Returns specialization name, total visits, and breakdown by status.
        /// Useful for pie charts and workload analysis per specialization.
        /// </summary>
        [HttpGet("stats/specializations")]
        public async Task<ActionResult<List<SpecializationStatsDto>>> GetSpecializationStats()
        {
            var stats = await context.Visits
                .Where(v => v.Status != VisitStatus.Cancelled)
                .GroupBy(v => v.SpecializationID)
                .Select(g => new SpecializationStatsDto
                {
                    SpecializationID = g.Key,
                    SpecializationName = g.Select(v => v.Specialization.SpecializationName).FirstOrDefault(),
                    TotalVisits = g.Count(),
                    Completed = g.Count(v => v.Status == VisitStatus.Completed),
                    Scheduled = g.Count(v => v.Status == VisitStatus.Scheduled)
                })
                .OrderByDescending(s => s.TotalVisits)
                .ToListAsync();

            return Ok(stats);
        }

        /// <summary>
        /// Data transfer object representing aggregated visit statistics.
        /// Includes absolute counts and percentage shares.
        /// </summary>
        public class VisitStatsDto
        {
            public int Total { get; set; }
            public int Completed { get; set; }
            public int Cancelled { get; set; }
            public int Scheduled { get; set; }
        }

        /// <summary>
        /// Daily statistics for visits, including counts and percentages.
        /// </summary>
        public class DailyVisitStatsDto
        {
            public DateOnly Date { get; set; }
            public int Total { get; set; }
            public int Completed { get; set; }
            public int Cancelled { get; set; }
            public int Scheduled { get; set; }
        }

        /// <summary>
        /// Statistics grouped by doctor.
        /// </summary>
        public class TopDoctorStatsDto
        {
            public int DoctorID { get; set; }
            public string DoctorName { get; set; } = "";
            public int TotalVisits { get; set; }
            public int Completed { get; set; }
            public int Scheduled { get; set; }
        }

        /// <summary>
        /// Statistics grouped by specialization.
        /// </summary>
        public class SpecializationStatsDto
        {
            public int SpecializationID { get; set; }
            public string SpecializationName { get; set; } = "";
            public int TotalVisits { get; set; }
            public int Completed { get; set; }
            public int Scheduled { get; set; }
        }
    }
}
