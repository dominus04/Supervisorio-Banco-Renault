namespace Supervisório_Banco_Renault.Models
{
    public class Traceability
    {
        public Guid Id { get; set; }
        public string TraceabilityCode { get; set; }
        public DateTime DateTimeOP20 { get; set; }
        public DateTime DateTimeOP10 { get; set; }
        public string ModuleCode { get; set; }
        public string CondenserCode { get; set; }
        public string FinalCondenserPressure { get; set; }
        public string FinalCondenserLeak { get; set; }
        public string FinalCondenserPSRead { get; set; }
        public string FinalRadiatorPressure { get; set; }
        public string FinalRadiatorLeak { get; set; }
        public string FinalRadiatorPSRead { get; set; }
        public string FinalModuleLabel { get; set; }
        public User UserOP20 { get; set; }
        public User UserOP10 { get; set; }
    }
}
