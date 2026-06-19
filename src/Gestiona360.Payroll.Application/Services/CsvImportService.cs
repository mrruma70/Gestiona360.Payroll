using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Gestiona360.Payroll.Application.Features.Employees.Commands;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services
{
    public class CsvImportService : ICsvImportService
    {
        public async Task<List<DgiTaxBracketCsvRecord>> ReadDgiTaxBracketsAsync(Stream fileStream, CancellationToken ct)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            });

            var records = new List<DgiTaxBracketCsvRecord>();
            await foreach (var record in csv.GetRecordsAsync<DgiTaxBracketCsvRecord>(ct))
            {
                records.Add(record);
            }

            return records;
        }

        public async Task<List<MitrabSalaryCsvRecord>> ReadMitrabSalariesAsync(Stream fileStream, CancellationToken ct)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CreateDefaultConfig());

            var records = new List<MitrabSalaryCsvRecord>();
            await foreach (var record in csv.GetRecordsAsync<MitrabSalaryCsvRecord>(ct))
            {
                records.Add(record);
            }
            return records;
        }

        private static CsvConfiguration CreateDefaultConfig() => new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };

    }
}
