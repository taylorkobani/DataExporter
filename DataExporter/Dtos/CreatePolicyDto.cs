namespace DataExporter.Dtos
{
    public class CreatePolicyDto
    {
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Premium { get; set; }
        public DateTime StartDate { get; set; }
    }
}
