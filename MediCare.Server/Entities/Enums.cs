namespace MediCare.Server.Entities
{
    public class Enums
    {
        public enum Role
        {
            Patient = 1,
            Doctor = 2,
            Admin = 3
        }
        public enum VisitReason
        {
            Consultation = 1,
            FollowUp = 2,
            Prescription = 3,
            Checkup = 4
        }

        public enum VisitStatus
        {
            Scheduled = 1,
            Completed = 2,
            Cancelled = 3
        }

        public enum Status
        {
            Active = 1,
            Inactive = 2
        }
    }
}
