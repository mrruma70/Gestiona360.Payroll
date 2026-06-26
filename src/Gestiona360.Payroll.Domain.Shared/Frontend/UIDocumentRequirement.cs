using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class UIDocumentRequirement
    {
        public string DocumentName { get; set; } = "";
        public bool IsMandatory { get; set; }
        public bool IsUploaded { get; set; }
        public string FileName { get; set; } = "";
        public string FileUrl { get; set; } = "";
    }
}
