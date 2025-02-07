namespace Supervisório_Banco_Renault.Models
{
    public class OP20_Traceability
    {
        public Guid Id { get; set; }
        public string TraceabilityCode { get; set; }
        public bool RadiatorVerified { get; set; }
        public bool CondenserVerified { get; set; }
        public bool RadiatorCodeVerified { get; set; }
        public bool CondenserCoversVerified { get; set; }
        public bool TraceabilityCodeVerified { get; set; }
        public DateTime DateTimeOP20 { get; set; }
        public string RadiatorCode { get; set; }
        public float FinalCondenserPressure { get; set; }
        public float FinalCondenserLeak { get; set; }
        public float FinalCondenserPSRead { get; set; }
        public float FinalRadiatorPressure { get; set; }
        public float FinalRadiatorLeak { get; set; }
        public float FinalRadiatorPSRead { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
