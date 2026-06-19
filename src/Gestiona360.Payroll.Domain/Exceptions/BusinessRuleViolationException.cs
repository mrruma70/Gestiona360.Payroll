using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Exceptions
{
    /// <summary>
    /// Excepción específica para violaciones de reglas de negocio.
    /// Permite diferenciar errores de dominio de errores técnicos o de validación de entrada.
    /// </summary>
    public class BusinessRuleViolationException : Exception
    {
        public BusinessRuleViolationException(string message)
            : base(message) { }

        public BusinessRuleViolationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
