namespace Supervisório_Banco_Renault.Models
{
    public class Recipe
    {

        public Guid Id { get; set; }
        public string? ModuleCode { get; set; }
        public short InitialCharacter { get; set; } = 1;
        public short FinalCharacter { get; set; } = 1;
        public bool VerifyModuleTag { get; set; } = true;
        public bool VerifyTraceabilityTag { get; set; } = true;
        public bool VerifyCondenserCovers { get; set; } = true;
        public bool VerifyRadiator { get; set; } = true;
        public short AteqRadiatorProgram { get; set; } = 1;
        public double RadiatorPSMinimum { get; set; }
        public double RadiatorPSMaximum { get; set; }
        public bool VerifyCondenser { get; set; } = true;
        public short AteqCondenserProgram { get; set; } = 1;
        public double CondenserPSMinimum { get; set; }
        public double CondenserPSMaximum { get; set; }


    }
}
