using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public static class HttpExtensions
    {
        public static async Task<string?> GetResponseContentAsync(this HttpRequestException ex)
        {
            if (ex.Data.Contains("ResponseContent"))
                return ex.Data["ResponseContent"] as string;
            return null;
        }
    }
}
