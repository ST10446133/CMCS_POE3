namespace POE_CMCS_Mvc.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }
        public string LecturerUsername { get; set; } = string.Empty;
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime DateSubmitted { get; set; }
        public byte[]? SupportingDoc { get; set; }

        public string? VerificationNotes { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? FinalApprovedBy { get; set; }
        public DateTime? FinalApprovedAt { get; set; }
        public decimal Amount => HoursWorked * HourlyRate;
    }
}
