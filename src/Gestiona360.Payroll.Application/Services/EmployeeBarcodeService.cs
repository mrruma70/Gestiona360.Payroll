using QRCoder;
using System;

namespace Gestiona360.Payroll.Application.Services
{
    public class EmployeeBarcodeService
    {
        /// <summary>
        /// Genera el payload para el código QR.
        /// Formato: G360:EMP-001|{CompanyId}
        /// </summary>
        public string GeneratePayload(string employeeCode, Guid companyId)
        {
            return $"G360:{employeeCode}|{companyId}";
        }

        /// <summary>
        /// Genera un código QR en formato PNG usando QRCoder (C# puro, sin System.Drawing).
        /// </summary>
        public byte[] GenerateQrCode(string payload, int pixelsPerModule = 10)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }
    }
}