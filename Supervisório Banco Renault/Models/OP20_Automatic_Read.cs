using S7.Net.Types;

namespace Supervisório_Banco_Renault.Models
{
    public class OP20_Automatic_Read
    {
        public ushort Step { get; set; }
        public float RadiatorAteqPressure { get; set; }
        public float RadiatorAteqLeak { get; set; }
        public float RadiatorPS { get; set; }
        public bool RadiatorTestOK { get; set; }
        public bool RadiatorTestNOK { get; set; }
        public float CondenserAteqPressure { get; set; }
        public float CondenserAteqLeak { get; set; }
        public float CondenserPS { get; set; }
        public bool CondenserTestOK { get; set; }
        public bool CondenserTestNOK { get; set; }

        [S7String(S7StringType.S7String, 50)]
        public string ModuleTag;

        [S7String(S7StringType.S7String, 50)]
        public string TraceabilityTag;
    }
}
