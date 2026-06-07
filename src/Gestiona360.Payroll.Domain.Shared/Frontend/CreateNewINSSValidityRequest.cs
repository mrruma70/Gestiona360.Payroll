using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class CreateNewINSSValidityRequest
    {
        [Required]
        public DateOnly EffectiveFrom { get; set; }

        [Required]
        [MaxLength(150)]
        public string LegalReference { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal RateWorker { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal RateEmployerSmall { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal RateEmployerLarge { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal MaxSalaryCap { get; set; }
    }
}
