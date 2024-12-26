using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models.Enums
{
    public enum AccessLevel
    {
        None = 0,
        Operador = 1,
        Lideranca = 2,
        Manutencao = 3,
        Administrador = 4
    }
}
