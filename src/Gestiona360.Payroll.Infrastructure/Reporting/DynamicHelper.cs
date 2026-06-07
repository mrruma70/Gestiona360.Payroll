using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Infrastructure.Reporting
{
    public static class DynamicHelper
    {
        public static IDictionary<string, object> ToDictionary(dynamic item)
        {
            // Dapper devuelve objetos que implementan IDictionary<string, object>
            return (IDictionary<string, object>)item;
        }
    }
}
