using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class Recipe
    {

        public Guid Id { get; set; }
        public string? ModuleCode { get; set; }
        public int InitialCharacter { get; set; } = 1;
        public int FinalCharacter { get; set; } = 1;
        public bool VerifyModuleTag { get; set; } = true;
        public bool VerifyTraceabilityTag { get; set; } = true;
        public bool VerifyCondenserCovers { get; set; } = true;
        public bool VerifyRadiator { get; set; } = true;
        public int AteqRadiatorProgram { get; set; } = 1;
        public float RadiatorPSMinimum { get; set; }
        public float RadiatorPSMaximum { get; set; }
        public bool VerifyCondenser { get; set; } = true;
        public int AteqCondenserProgram { get; set; } = 1;
        public float CondenserPSMinimum { get; set; }
        public float CondenserPSMaximum { get; set; }

    }
}
