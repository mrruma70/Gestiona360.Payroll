using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{ 
    /// <summary>
    /// DTO para representar un documento adjunto a una acción de personal.
    /// </summary>
    public class DocumentAttachmentDto
    {
        /// <summary>Nombre del archivo</summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>URL o ruta de acceso al archivo</summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>Tipo MIME del archivo (ej: "application/pdf")</summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>Tamaño del archivo en bytes</summary>
        public long FileSizeBytes { get; set; }

        /// <summary>Fecha de carga del documento</summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>Indica si el documento es obligatorio</summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Tipo/categoría del documento (ej: "Carta de Aumento Salarial Firmada").
        /// Se usa para hacer match con los documentos requeridos en la UI.
        /// </summary>
        public string? DocumentType { get; set; }
    }
}
