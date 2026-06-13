using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gestiona360.Payroll.Application.Contracts.Common.Validators
{
    
        /// <summary>
        /// Validador oficial de cédula de identidad nicaragüense.
        /// Utiliza el algoritmo de módulo 23 con alfabeto específico (sin I, Ñ, O, U).
        /// Estructura: PPP-DDMMYY-SSSSC donde:
        /// - PPP: Código de municipio (3 dígitos)
        /// - DDMMYY: Fecha de nacimiento
        /// - SSSS: Correlativo (4 dígitos)
        /// - C: Letra verificadora (A-Y, excluyendo I, Ñ, O, U)
        /// </summary>
        public static class CedulaValidator
        {
            // Alfabeto oficial (23 letras, sin I, Ñ, O, U)
            internal const string LETRAS = "ABCDEFGHJKLMNPQRSTUVWXY";

            /// <summary>
            /// Valida una cédula nicaragüense de forma completa.
            /// Verifica: formato, código de municipio, fecha, edad, sufijo y letra.
            /// </summary>
            public static bool EsValida(string cedula)
            {
                if (string.IsNullOrWhiteSpace(cedula))
                    return false;

                try
                {
                    var validador = new ValidacionCedula();
                    validador.SetCedula(cedula.Trim());
                    return validador.IsCedulaValida();
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// Calcula la letra correcta para una secuencia numérica dada.
            /// Útil para sugerir correcciones cuando la cédula es inválida.
            /// </summary>
            public static char? CalcularLetra(string cedulaSinLetra)
            {
                if (string.IsNullOrWhiteSpace(cedulaSinLetra))
                    return null;

                string soloNumeros = cedulaSinLetra.Replace("-", "");
                if (soloNumeros.Length != 13 || !long.TryParse(soloNumeros, out long numero))
                    return null;

                int posicion = (int)(numero - Math.Floor((double)numero / 23.0) * 23);
                if (posicion >= LETRAS.Length)
                    return '?';

                return LETRAS[posicion];
            }

            /// <summary>
            /// Extrae la fecha de nacimiento embebida en la cédula.
            /// </summary>
            public static DateTime? ExtraerFechaNacimiento(string cedula)
            {
                if (string.IsNullOrWhiteSpace(cedula))
                    return null;

                string limpia = cedula.Replace("-", "").Trim().ToUpper();
                if (limpia.Length != 14)
                    return null;

                string fechaStr = limpia.Substring(3, 6); // DDMMYY
                if (!Regex.IsMatch(fechaStr, @"^(0[1-9]|[12][0-9]|3[01])(0[1-9]|1[012])(\d{2})$"))
                    return null;

                int dia = int.Parse(fechaStr.Substring(0, 2));
                int mes = int.Parse(fechaStr.Substring(2, 2));
                int anio = int.Parse(fechaStr.Substring(4, 2));

                // Asumir siglo 1900-1999 para años mayores al actual, 2000-2099 para menores
                int anioActual = DateTime.Now.Year % 100;
                anio += (anio > anioActual) ? 1900 : 2000;

                try
                {
                    return new DateTime(anio, mes, dia);
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Calcula la edad del empleado basada en la fecha embebida en la cédula.
            /// </summary>
            public static int? CalcularEdad(string cedula)
            {
                var fecha = ExtraerFechaNacimiento(cedula);
                if (!fecha.HasValue)
                    return null;

                var hoy = DateTime.Today;
                int edad = hoy.Year - fecha.Value.Year;
                if (fecha.Value > hoy.AddYears(-edad))
                    edad--;

                return edad;
            }

            /// <summary>
            /// Obtiene el nombre del municipio basado en el código de la cédula.
            /// </summary>
            public static string? ObtenerMunicipio(string cedula)
            {
                if (string.IsNullOrWhiteSpace(cedula))
                    return null;

                string limpia = cedula.Replace("-", "").Trim();
                if (limpia.Length < 3 || !int.TryParse(limpia.Substring(0, 3), out int codigo))
                    return null;

                return CodigoMunicipioMap.TryGetValue(codigo, out string? nombre) ? nombre : null;
            }
        }

        /// <summary>
        /// Clase interna que implementa la validación completa de la cédula.
        /// Basada en el algoritmo oficial nicaragüense.
        /// </summary>
        internal class ValidacionCedula
        {
            private string? _cedula;

            public void SetCedula(string cedula)
            {
                if (string.IsNullOrWhiteSpace(cedula))
                {
                    _cedula = null;
                    return;
                }

                string limpia = cedula.Trim().Replace("-", "").ToUpper();
                _cedula = limpia.Length == 14 ? limpia : null;
            }

            public string? GetCedula() => _cedula;
            public string? GetPrefijo() => _cedula?.Substring(0, 3);
            public string? GetFecha() => _cedula?.Substring(3, 6);
            public string? GetSufijo() => _cedula?.Substring(9, 5);
            public string? GetLetra() => _cedula?.Substring(13, 1);
            public string? GetNumeroSinLetra() => _cedula?.Substring(0, 13);

            /// <summary>
            /// Validación completa de la cédula.
            /// </summary>
            public bool IsCedulaValida()
            {
                return _cedula != null &&
                       IsPrefijoValido() &&
                       IsCodigoMunicipioValido() &&
                       IsFechaValida() &&
                       IsEdadValida() &&
                       IsSufijoValido() &&
                       IsLetraValida();
            }

            public bool IsPrefijoValido()
            {
                string? prefijo = GetPrefijo();
                return prefijo != null && Regex.IsMatch(prefijo, @"^\d{3}$");
            }

            public bool IsCodigoMunicipioValido()
            {
                string? prefijo = GetPrefijo();
                if (prefijo == null || !int.TryParse(prefijo, out int codigo))
                    return false;

                return CodigoMunicipioMap.ContainsKey(codigo);
            }

            public bool IsFechaValida()
            {
                string? fecha = GetFecha();
                if (fecha == null)
                    return false;

                if (!Regex.IsMatch(fecha, @"^(0[1-9]|[12][0-9]|3[01])(0[1-9]|1[012])(\d{2})$"))
                    return false;

                try
                {
                    DateTime fechaDate = DateTime.ParseExact(fecha, "ddMMyy", CultureInfo.InvariantCulture);
                    return fecha.Equals(fechaDate.ToString("ddMMyy"), StringComparison.Ordinal);
                }
                catch
                {
                    return false;
                }
            }

            public bool IsEdadValida()
            {
                string? fecha = GetFecha();
                if (fecha == null || !IsFechaValida())
                    return false;

                int dia = int.Parse(fecha.Substring(0, 2));
                int mes = int.Parse(fecha.Substring(2, 2));
                int anio = int.Parse(fecha.Substring(4, 2));

                DateTime ahora = DateTime.Now;
                int anioActual = ahora.Year % 100;

                // Si el año es menor al actual (en 2 dígitos), asumir siglo XX
                if (anio > anioActual)
                    anio += 1900;
                else
                    anio += 2000;

                try
                {
                    var fechaNac = new DateTime(anio, mes, dia);
                    var edad = ahora.Year - fechaNac.Year;
                    if (fechaNac > ahora.AddYears(-edad))
                        edad--;

                    return edad >= 15; // Edad mínima legal para trabajar en Nicaragua
                }
                catch
                {
                    return false;
                }
            }

            public bool IsSufijoValido()
            {
                string? sufijo = GetSufijo();
                return sufijo != null && Regex.IsMatch(sufijo, @"^\d{4}[A-Y]$");
            }

            public bool IsLetraValida()
            {
                string? letra = GetLetra();
                if (string.IsNullOrEmpty(letra))
                    return false;

                return letra.Equals(CalcularLetra().ToString(), StringComparison.Ordinal);
            }

            public char CalcularLetra()
            {
                string? numeroSinLetra = GetNumeroSinLetra();
                if (numeroSinLetra == null || !long.TryParse(numeroSinLetra, out long numero))
                    return '?';

                int posicion = (int)(numero - Math.Floor((double)numero / 23.0) * 23);
                if (posicion < 0 || posicion >= CedulaValidator.LETRAS.Length)
                    return '?';

                return CedulaValidator.LETRAS[posicion];
            }
        }

        /// <summary>
        /// Mapa de códigos de municipios de Nicaragua según el registro civil.
        /// Incluye los 153 municipios de los 17 departamentos + 2 regiones autónomas.
        /// </summary>
        internal static class CodigoMunicipioMap
        {
            private static readonly Dictionary<int, string> _map = new()
        {
            // ═══════════════════════════════════════════════════════════════
            // MANAGUA (1-9)
            // ═══════════════════════════════════════════════════════════════
            { 1, "Managua" },
            { 2, "San Rafael del Sur" },
            { 3, "Tipitapa" },
            { 4, "Villa Carlos Fonseca" },
            { 5, "San Francisco Libre" },
            { 6, "Mateare" },
            { 7, "Ticuantepe" },
            { 8, "Ciudad Sandino" },
            { 9, "El Crucero" },

            // ═══════════════════════════════════════════════════════════════
            // BOACO (361-366)
            // ═══════════════════════════════════════════════════════════════
            { 361, "Boaco" },
            { 362, "Camoapa" },
            { 363, "Santa Lucía" },
            { 364, "San José de los Remates" },
            { 365, "San Lorenzo" },
            { 366, "Teustepe" },

            // ═══════════════════════════════════════════════════════════════
            // CARAZO (41-48)
            // ═══════════════════════════════════════════════════════════════
            { 41, "Jinotepe" },
            { 42, "Diriamba" },
            { 43, "San Marcos" },
            { 44, "Santa Teresa" },
            { 45, "Dolores" },
            { 46, "La Paz de Carazo" },
            { 47, "El Rosario" },
            { 48, "La Conquista" },

            // ═══════════════════════════════════════════════════════════════
            // CHINANDEGA (81-93)
            // ═══════════════════════════════════════════════════════════════
            { 81, "Chinandega" },
            { 82, "Corinto" },
            { 83, "El Realejo" },
            { 84, "Chichigalpa" },
            { 85, "Posoltega" },
            { 86, "El Viejo" },
            { 87, "Puerto Morazán" },
            { 88, "Somotillo" },
            { 89, "Villa Nueva" },
            { 90, "Santo Tomás del Norte" },
            { 91, "Cinco Pinos" },
            { 92, "San Francisco del Norte" },
            { 93, "San Pedro del Norte" },

            // ═══════════════════════════════════════════════════════════════
            // CHONTALES (121-130)
            // ═══════════════════════════════════════════════════════════════
            { 121, "Juigalpa" },
            { 122, "Acoyapa" },
            { 123, "Santo Tomás" },
            { 124, "Villa Sandino" },
            { 125, "San Pedro de Lóvago" },
            { 126, "La Libertad" },
            { 127, "Santo Domingo" },
            { 128, "Comalapa" },
            { 129, "San Francisco de Cuapa" },
            { 130, "El Coral" },

            // ═══════════════════════════════════════════════════════════════
            // ESTELÍ (161-166)
            // ═══════════════════════════════════════════════════════════════
            { 161, "Estelí" },
            { 162, "Pueblo Nuevo" },
            { 163, "Condega" },
            { 164, "San Juan de Limay" },
            { 165, "La Trinidad" },
            { 166, "San Nicolás" },

            // ═══════════════════════════════════════════════════════════════
            // GRANADA (201-204)
            // ═══════════════════════════════════════════════════════════════
            { 201, "Granada" },
            { 202, "Nandaime" },
            { 203, "Diriomo" },
            { 204, "Diriá" },

            // ═══════════════════════════════════════════════════════════════
            // JINOTEGA (241-247)
            // ═══════════════════════════════════════════════════════════════
            { 241, "Jinotega" },
            { 242, "San Rafael del Norte" },
            { 243, "San Sebastián de Yalí" },
            { 244, "La Concordia" },
            { 245, "San José de Bocay" },
            { 246, "El Cuá-Bocay" },
            { 247, "Santa María de Pantasma" },

            // ═══════════════════════════════════════════════════════════════
            // LEÓN (281-291)
            // ═══════════════════════════════════════════════════════════════
            { 281, "León" },
            { 283, "El Jicaral" },
            { 284, "La Paz Centro" },
            { 285, "Santa Rosa del Peñón" },
            { 286, "Quezalguaque" },
            { 287, "Nagarote" },
            { 288, "El Sauce" },
            { 289, "Achuapa" },
            { 290, "Telica" },
            { 291, "Larreynaga-Malpaisillo" },

            // ═══════════════════════════════════════════════════════════════
            // MADRIZ (321-329)
            // ═══════════════════════════════════════════════════════════════
            { 321, "Somoto" },
            { 322, "Telpaneca" },
            { 323, "San Juan del Río Coco" },
            { 324, "Palacagüina" },
            { 325, "Yalagüina" },
            { 326, "Totogalpa" },
            { 327, "San Lucas" },
            { 328, "Las Sabanas" },
            { 329, "San José de Cusmapa" },

            // ═══════════════════════════════════════════════════════════════
            // MASAYA (401-409)
            // ═══════════════════════════════════════════════════════════════
            { 401, "Masaya" },
            { 402, "Nindirí" },
            { 403, "Tisma" },
            { 404, "Catarina" },
            { 405, "San Juan de Oriente" },
            { 406, "Niquinohomo" },
            { 407, "Nandasmo" },
            { 408, "Masatepe" },
            { 409, "La Concepción" },

            // ═══════════════════════════════════════════════════════════════
            // MATAGALPA (441-454)
            // ═══════════════════════════════════════════════════════════════
            { 441, "Matagalpa" },
            { 442, "San Ramón" },
            { 443, "Matiguás" },
            { 444, "Muy Muy" },
            { 445, "Esquipulas" },
            { 446, "San Dionisio" },
            { 447, "San Isidro" },
            { 448, "Sébaco" },
            { 449, "Ciudad Darío" },
            { 450, "Terrabona" },
            { 451, "Río Blanco" },
            { 452, "Tuma-La Dalia" },
            { 453, "Rancho Grande" },
            { 454, "Waslala" },

            // ═══════════════════════════════════════════════════════════════
            // NUEVA SEGOVIA (481-493)
            // ═══════════════════════════════════════════════════════════════
            { 481, "Ocotal" },
            { 482, "Santa María" },
            { 483, "Macuelizo" },
            { 484, "Dipilto" },
            { 485, "Ciudad Antigua" },
            { 486, "Mozonte" },
            { 487, "San Fernando" },
            { 488, "El Jícaro" },
            { 489, "Jalapa" },
            { 490, "Murra" },
            { 491, "Quilalí" },
            { 492, "Wiwilí Nueva Segovia" },
            { 493, "San José de Bocay" },

            // ═══════════════════════════════════════════════════════════════
            // RÍO SAN JUAN (521-526)
            // ═══════════════════════════════════════════════════════════════
            { 521, "San Carlos" },
            { 522, "El Castillo" },
            { 523, "San Miguelito" },
            { 524, "Morrito" },
            { 525, "San Juan del Norte" },
            { 526, "El Almendro" },

            // ═══════════════════════════════════════════════════════════════
            // RIVAS (561-570)
            // ═══════════════════════════════════════════════════════════════
            { 561, "Rivas" },
            { 562, "San Jorge" },
            { 563, "Buenos Aires" },
            { 564, "Potosí" },
            { 565, "Belén" },
            { 566, "Tola" },
            { 567, "San Juan del Sur" },
            { 568, "Cárdenas" },
            { 569, "Moyogalpa" },
            { 570, "Altagracia" },

            // ═══════════════════════════════════════════════════════════════
            // RACCS - Atlántico Sur (601-628)
            // ═══════════════════════════════════════════════════════════════
            { 601, "Bluefields" },
            { 602, "Corn Island" },
            { 603, "El Rama" },
            { 604, "Muelle de los Bueyes" },
            { 605, "La Cruz de Río Grande" },
            { 606, "Prinzapolka" },
            { 616, "Nueva Guinea" },
            { 619, "Tortuguero" },
            { 624, "Kukra Hill" },
            { 626, "Laguna de Perlas" },
            { 627, "Desembocadura de Río Grande" },
            { 628, "El Ayote" },

            // ═══════════════════════════════════════════════════════════════
            // RACCN - Atlántico Norte (607-615)
            // ═══════════════════════════════════════════════════════════════
            { 607, "Puerto Cabezas" },
            { 608, "Waspán" },
            { 610, "Siuna" },
            { 611, "Bonanza" },
            { 612, "Rosita" },
            { 615, "Bocana de Paiwas" }
        };

            public static bool ContainsKey(int codigo) => _map.ContainsKey(codigo);
            public static bool TryGetValue(int codigo, out string? nombre) => _map.TryGetValue(codigo, out nombre);
        }
    }

