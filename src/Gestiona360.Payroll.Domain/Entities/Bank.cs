using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Bank : BaseEntityInt
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;          // "Banco de América Central (BAC)"

        [Required, MaxLength(10)]
        public string Code { get; set; } = null!;          // "BAC", "BANPRO", "LAFISE", "FICOHSA"

        [MaxLength(10)]
        public string? AchCode { get; set; }               // Código para archivos ACH (opcional, según cada banco)

        [Required, MaxLength(20)]
        public string? AccountNumberPrefix { get; set; }   // Prefijo de cuenta (opcional)

        public bool IsActive { get; set; } = true;
    }
}
