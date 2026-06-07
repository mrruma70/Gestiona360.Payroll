using System.Collections.Concurrent;
using System.Reflection;

namespace Gestiona360.Payroll.Infrastructure.Reporting
{
    /// <summary>
    /// Lee archivos .sql marcados como "Embedded Resource" y los cachea en memoria.
    /// </summary>
    public static class EmbeddedSqlReader
    {
        private static readonly ConcurrentDictionary<string, string> _cache = new();

        public static string GetSql(string resourceName, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            return _cache.GetOrAdd(resourceName, name =>
            {
                // Busca el recurso por nombre parcial (flexible)
                var fullName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new FileNotFoundException(
                        $"No se encontró el recurso SQL embebido: {name}. " +
                        $"Asegúrate de que el archivo .sql tenga Build Action = 'Embedded Resource'.");

                using var stream = assembly.GetManifestResourceStream(fullName)
                    ?? throw new InvalidOperationException($"No se pudo abrir el stream del recurso: {fullName}");

                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            });
        }

        /// <summary>
        /// Reemplaza placeholders del tipo @ParamName con valores reales.
        /// IMPORTANTE: Esto es solo para valores dinámicos en WHERE/ORDER BY.
        /// Para valores de columnas, usa parámetros de Dapper.
        /// </summary>
        public static string ReplacePlaceholders(string sql, Dictionary<string, object> parameters)
        {
            foreach (var param in parameters)
            {
                // Reemplaza {{Key}} por el valor (útil para nombres de tabla dinámicos, etc.)
                sql = sql.Replace($"{{{{{param.Key}}}}}", param.Value?.ToString() ?? "");
            }
            return sql;
        }
    }
}
