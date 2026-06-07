using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class GlobalConfigDto
    {
        public INSSConfigDto INSSConfig { get; set; } = new();
        public INATECConfigDto INATECConfig { get; set; } = new();
        public List<IR_TaxBracketDto> IRTaxBrackets { get; set; } = new();
        public List<MinimumWageDto> MinimumWages { get; set; } = new();
        public CurrencyExchangeDto LatestExchangeRate { get; set; } = new();

        // ✅ ESTAS 4 LISTAS DEBEN EXISTIR
        public List<int> AvailableINSSYears { get; set; } = new();
        public List<int> AvailableIRYears { get; set; } = new();
        public List<int> AvailableMitrabYears { get; set; } = new();
        public List<int> AvailableINATECYears { get; set; } = new();

        public List<INSSConfigDto> INSSHistory { get; set; } = new();
    }

}
