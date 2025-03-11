namespace Supervisório_Banco_Renault.Models
{
    public class Recipe
    {

        public Guid Id { get; set; }
        public string? ModuleCode { get; set; }
        public string? ClientCode { get; set; }
        public string RadiatorModel { get; set; }
        public short InitialCharacter { get; set; } = 1;
        public short CodeLength { get; set; } = 1;
        public bool ReadRadiatorLabel { get; set; } = true;
        public bool ReadRadiatorLabelOP10 { get; set; } = true;
        public bool ReadCondenserLabelOP10 { get; set; } = true;
        public bool VerifyRadiatorLabel { get; set; } = true;
        public bool VerifyTraceabilityLabel { get; set; } = true;
        public bool VerifyCondenserCovers { get; set; } = true;
        public bool VerifyRadiator { get; set; } = true;
        public short AteqRadiatorProgram { get; set; } = 1;
        public double RadiatorPSMinimum { get; set; }
        public double RadiatorPSMaximum { get; set; }
        public bool VerifyCondenser { get; set; } = true;
        public short AteqCondenserProgram { get; set; } = 1;
        public double CondenserPSMinimum { get; set; }
        public double CondenserPSMaximum { get; set; }
        public Label? Label { get; set; }
        public Guid LabelId { get; set; }
    }
}
