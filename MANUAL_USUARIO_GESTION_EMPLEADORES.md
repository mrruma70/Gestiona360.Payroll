# 📘 Manual de Usuario - Módulo Gestión de Empleadores

## 📄 Información del Documento

| Propiedad | Valor |
|---|---|
| **Módulo** | Gestión de Empleadores |
| **Versión** | 1.0 |
| **Fecha de Actualización** | 2025 |
| **Estado** | ✅ Implementado |
| **Ubicación en Menú** | 🏢 Gestión de Empleadores → Empresa, Sucursales y Parámetros |
| **Ruta de Acceso** | `/employer-management` |

---

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Acceso al Módulo](#acceso-al-módulo)
3. [Interfaz Principal](#interfaz-principal)
4. [Pestaña 1: Empresa Principal](#pestaña-1-empresa-principal)
5. [Pestaña 2: Sucursales](#pestaña-2-sucursales)
6. [Pestaña 3: Configuración Global](#pestaña-3-configuración-global)
7. [Procedimientos Comunes](#procedimientos-comunes)
8. [Guía de Troubleshooting](#guía-de-troubleshooting)
9. [Integración con Otros Módulos](#integración-con-otros-módulos)

---

## 🎯 Descripción General

El módulo **Gestión de Empleadores** es el centro administrativo del sistema de nómina. Permite registrar y mantener actualizada toda la información de la empresa licenciataria, sus sucursales y los parámetros fiscales y laborales que rigen la operación.

### Funcionalidades Principales

✅ Administración de datos legales y comerciales de la empresa  
✅ Gestión de sucursales y centros de costo  
✅ Configuración de parámetros fiscales (INSS, IR, INATEC)  
✅ Gestión de salarios mínimos (MITRAB)  
✅ Integración con tasa de cambio BCN  
✅ Exportación de documentos legales (PDF)  
✅ Exportación de reportes (Excel)

### Acceso Requerido

- **Rol Mínimo**: Administrador del Sistema o Recursos Humanos
- **Permisos**: Lectura y modificación de configuración global

---

## 🚀 Acceso al Módulo

### Opción 1: Desde el Menú Principal
1. Localiza el menú de navegación lateral izquierdo
2. Haz clic en **🏢 Gestión de Empleadores**
3. Selecciona **Empresa, Sucursales y Parámetros**

### Opción 2: Acceso Directo
- Escribe la URL: `https://tu-dominio.com/employer-management`

---

## 🖥️ Interfaz Principal

El módulo utiliza un sistema de **tres pestañas principales** para organizar la información:

```
┌─────────────────────────────────────────────────────────────┐
│  🏢 Gestión de Empleadores                                  │
├──────────────┬──────────────┬──────────────────────────────┤
│ Empresa      │ Sucursales   │ Configuración Global          │
│ Principal    │              │                                │
└──────────────┴──────────────┴──────────────────────────────┘
```

### Tips de Interfaz

💡 **Tip**: Utiliza las pestañas para navegar entre diferentes secciones. Los cambios se guardan independientemente en cada sección.

---

## 📑 PESTAÑA 1: EMPRESA PRINCIPAL

### Propósito
Registrar y mantener los datos legales, comerciales e identificadores fiscales de tu empresa licenciataria.

### Datos Configurables

#### 🏛️ Información Legal

| Campo | Requerido | Descripción | Ejemplo |
|---|:---:|---|---|
| **Nombre Legal** | ✅ | Razón social exacta registrada en el RUC | EMPRESA S.A. |
| **Nombre Comercial** | ❌ | Nombre de fantasía (si aplica) | Mi Empresa |
| **RUC/NIT** | ✅ | Número de identificación fiscal | J0610010123456 |
| **Código Patronal INSS** | ✅ | Identificador ante el INSS | P-123456 |
| **Actividad CIIU** | ✅ | Código de actividad económica | 4520 |
| **Representante Legal** | ✅ | Nombre completo del representante | Juan Pérez García |
| **Cédula Rep. Legal** | ✅ | Documento de identidad del representante | 00112003400123 |

#### 📍 Información de Contacto

| Campo | Requerido | Descripción |
|---|:---:|---|
| **Ciudad/Departamento** | ✅ | Ubicación principal de la empresa |
| **Dirección Fiscal** | ✅ | Domicilio registrado ante la DGI |
| **Correo Electrónico** | ✅ | Email corporativo para comunicaciones |
| **Teléfono** | ✅ | Número de contacto principal |

#### 💰 Configuración Financiera

| Campo | Requerido | Descripción | Opciones |
|---|:---:|---|---|
| **Moneda Base** | ✅ | Divisa de operación principal | NIO, USD |
| **Tasa BCN** | ❌ | Tasa de cambio actualizada automáticamente | Visualización solo lectura |
| **Frecuencia Default** | ✅ | Períodos de pago por defecto | Semanal, Quincenal, Mensual |
| **Zona Franca** | ❌ | ¿Opera en régimen de zona franca? | Sí/No |

#### 🖼️ Branding

| Campo | Requerido | Descripción |
|---|:---:|---|
| **Logo Empresa (URL)** | ❌ | Enlace a imagen del logo para reportes |

### Operaciones Disponibles

#### 💾 Guardar Cambios
1. Completa los campos requeridos
2. Haz clic en el botón **💾 Guardar Cambios**
3. Espera confirmación "✅ Datos de empresa actualizados"

#### 📄 Exportar Ficha Legal (PDF)
1. Haz clic en **📄 Exportar Ficha Legal (PDF)**
2. El sistema generará un documento con los datos legales de la empresa
3. Se descargará automáticamente con formato: `Ficha_Legal_[NombreEmpresa]_[fecha].pdf`

**Incluye:**
- Datos legales de la empresa
- Información del representante legal
- Dirección fiscal
- Identificadores fiscales

---

### 📸 Captura de Pantalla - Pestaña Empresa Principal

```
┌──────────────────────────────────────────────────────────────────────┐
│  📋 Datos de la Empresa Licenciataria                               │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de la pestaña Empresa Principal]                  │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Toma una captura de pantalla de la interfaz de Empresa          │
│  2. Guardala con nombre: "empresa-principal.png"                   │
│  3. Asegúrate que la imagen muestre:                               │
│     ✓ Todos los campos de entrada                                  │
│     ✓ Selector de moneda base                                      │
│     ✓ Botones de acción (Guardar, Exportar)                       │
│     ✓ Tasa BCN actualizada                                         │
│  4. Reemplaza este texto por: ![Empresa Principal](./images/empresa-principal.png) │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el texto anterior con:
> ```markdown
> ![Empresa Principal](./images/empresa-principal.png)
> ```

---

## 📑 PESTAÑA 2: SUCURSALES

### Propósito
Administrar todas las sucursales, filiales u oficinas de la empresa principal.

### Visualización de Sucursales

Tabla listado con las siguientes columnas:

| Columna | Descripción |
|---|---|
| **Código** | Identificador único de sucursal (SUC-001, SUC-002, etc.) |
| **Nombre** | Denominación de la sucursal |
| **Ciudad / Dirección** | Ubicación geográfica |
| **Centro Costo** | Centro de costo asignado por defecto |
| **Estado** | 🟢 Activa / 🟡 Inactiva |
| **Acciones** | Editar, Eliminar |

### Operaciones Disponibles

#### ➕ Crear Nueva Sucursal

1. Haz clic en **➕ Nueva Sucursal**
2. Se abre un panel deslizable (drawer) a la derecha
3. Completa los siguientes datos:

**Campos de la Nueva Sucursal:**

| Campo | Requerido | Descripción | Formato |
|---|:---:|---|---|
| **Código Interno** | ✅ | Identificador único de sucursal | SUC-XXX |
| **Nombre** | ✅ | Nombre de la sucursal | "Sucursal Managua" |
| **Dirección** | ✅ | Domicilio físico | Calle, No. |
| **Ciudad** | ✅ | Ciudad o departamento | "Managua" |
| **Teléfono Suc.** | ✅ | Línea de contacto | +505 XXXX-XXXX |
| **Gerente Sucursal** | ❌ | Empleado responsable | [Seleccionar de lista] |
| **Centro de Costo** | ❌ | Asignación contable | [Seleccionar de lista] |
| **¿Opera en Zona Franca?** | ❌ | Régimen fiscal especial | Sí/No |
| **Estado Activo** | ❌ | Disponibilidad operativa | Sí/No |

4. Haz clic en **💾 Guardar Sucursal**
5. Se mostrará confirmación "✅ Sucursal creada"

#### ✏️ Editar Sucursal Existente

1. En la tabla de sucursales, localiza la sucursal a editar
2. Haz clic en el icono **✏️ (Editar)**
3. Se abre el panel deslizable con los datos actuales
4. Realiza los cambios necesarios
5. Haz clic en **💾 Guardar Sucursal**
6. Se mostrará confirmación "✅ Sucursal actualizada"

#### 🗑️ Eliminar Sucursal

> ⚠️ **Nota Importante**: La eliminación desactiva la sucursal (soft delete), no la borra completamente de la base de datos.

1. En la tabla, localiza la sucursal a eliminar
2. Haz clic en el icono **🗑️ (Eliminar)**
3. Aparecerá un diálogo de confirmación
4. Haz clic en **Desactivar** para confirmar
5. La sucursal pasará a estado 🟡 Inactiva

#### 📤 Exportar Listado de Sucursales (Excel)

1. Haz clic en **📤 Exportar Listado (Excel)**
2. El sistema generará un archivo Excel con todas las sucursales
3. Se descargará como: `Sucursales_[timestamp].xlsx`

**Incluye:**
- Código y nombre de cada sucursal
- Dirección y ciudad
- Centro de costo asignado
- Estado (Activo/Inactivo)

---

### 📸 Captura de Pantalla - Pestaña Sucursales

```
┌──────────────────────────────────────────────────────────────────────┐
│  🏗️ Gestión de Sucursales                                           │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de la tabla de sucursales]                         │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Captura una pantalla mostrando:                                 │
│     ✓ Tabla con 2-3 sucursales de ejemplo                         │
│     ✓ Botones: Exportar Excel, Nueva Sucursal, Buscar             │
│     ✓ Acciones: Editar y Eliminar por fila                        │
│     ✓ Estados visuales (🟢 Activa, 🟡 Inactiva)                  │
│  2. Guarda como: "sucursales-tabla.png"                            │
│  3. Reemplaza este bloque con:                                      │
│     ![Tabla de Sucursales](./images/sucursales-tabla.png)          │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el texto anterior con:
> ```markdown
> ![Tabla de Sucursales](./images/sucursales-tabla.png)
> ```

---

### 📸 Captura de Pantalla - Formulario Nueva Sucursal

```
┌──────────────────────────────────────────────────────────────────────┐
│  ➕ Nueva Sucursal                                                   │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen del drawer/panel de formulario]                    │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Haz clic en "Nueva Sucursal"                                    │
│  2. Captura el panel deslizable con los campos:                     │
│     ✓ Código Interno                                                │
│     ✓ Nombre                                                        │
│     ✓ Dirección, Ciudad, Teléfono                                  │
│     ✓ Selectores: Gerente, Centro de Costo                         │
│     ✓ Switches: Zona Franca, Estado Activo                         │
│     ✓ Botones: Cancelar, Guardar Sucursal                          │
│  3. Guarda como: "nueva-sucursal-drawer.png"                        │
│  4. Reemplaza este bloque con:                                      │
│     ![Formulario Nueva Sucursal](./images/nueva-sucursal-drawer.png) │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el texto anterior con:
> ```markdown
> ![Formulario Nueva Sucursal](./images/nueva-sucursal-drawer.png)
> ```

---

## 📑 PESTAÑA 3: CONFIGURACIÓN GLOBAL

### Propósito
Mantener actualizada la configuración de parámetros fiscales y laborales que afectan el cálculo de nómina.

### Sub-pestañas de Configuración

La pestaña "Configuración Global" se divide en **4 sub-pestañas**:

#### 3.1️⃣ TAB: FISCALES (INSS, IR, INATEC)

Administra los parámetros de contribuciones fiscales.

##### 🏛️ INSS - Institución Nicaragüense de Seguridad Social

**Funcionalidad:**
- Historial de configuraciones INSS con sus períodos de vigencia
- Registro de nuevas vigencias cuando cambia la legislación

**Tabla de Historial INSS:**

| Columna | Descripción |
|---|---|
| **Vigencia Desde** | Fecha de inicio (dd/MM/yyyy) |
| **Vigencia Hasta** | Fecha de fin, o "Vigente" si actual |
| **Fundamento Legal** | Ley o decreto aplicable |
| **Trabajador %** | Porcentaje de aporte del trabajador |
| **Empleador %** | Porcentaje de aporte del empleador |
| **Tope Salarial** | Límite máximo para cálculos |
| **Estado** | ✅ Vigente / ⏳ Histórico |

**Acción: ➕ Registrar Nueva Vigencia**
1. Haz clic en **➕ Registrar Nueva Vigencia**
2. Se abre un diálogo para registrar nueva configuración INSS
3. Completa: fecha de vigencia, porcentajes, tope salarial
4. Haz clic en **Guardar**

---

##### 🧾 IR - Impuesto sobre la Renta

**Selector de Año:**
- Permite seleccionar qué año de tabla IR deseas visualizar
- Útil cuando existen múltiples reformas fiscales por año

**Tabla de Tramos Impositivos:**

| Columna | Descripción |
|---|---|
| **Tramo** | Rango salarial (ej: C$ 100,000 - C$ 200,000) |
| **Base** | Impuesto fijo que aplica al tramo |
| **% Marginal** | Porcentaje que se aplica al exceso |
| **Exceso de** | Punto de referencia para el cálculo marginal |

**Acción: 📥 Importar Tabla DGI CSV**
1. Haz clic en **📥 Importar Tabla DGI CSV**
2. Se abre un diálogo de carga de archivos
3. Selecciona un archivo CSV con el formato DGI
4. El sistema valida y carga la tabla
5. Confirmación: "✅ Tabla DGI importada correctamente"

**Formato Esperado del CSV:**
```
Tramo,Base,Marginal,Exceso
100000,0,15,100000
200000,15000,20,200000
```

---

##### 🎓 INATEC - Aporte Formación Técnica

**Selector de Año:**
- Permite cambiar el año de vigencia INATEC

**Información de Configuración:**

| Campo | Tipo | Descripción |
|---|---|---|
| **Tasa Patronal** | Decimal | Porcentaje sobre nómina bruta |
| **Excepciones** | Texto | Sectores o empresas eximidas |
| **Fundamento Legal** | Chip | Referencia normativa |

**Acción: ➕ Registrar Nueva Vigencia**
1. Haz clic en **➕ Registrar Nueva Vigencia**
2. Se abre diálogo para nueva vigencia INATEC
3. Completa: año, tasa, excepciones, referencia legal
4. Haz clic en **Guardar**

---

#### 3.2️⃣ TAB: LABORALES - SALARIOS MÍNIMOS MITRAB

**Funcionalidad:**
- Mantener actualizado el acuerdo de salarios mínimos entre sectores
- Gestión de múltiples años de acuerdos

**Selector de Año:**
- Selecciona qué año de acuerdo MITRAB deseas visualizar

**Tabla de Salarios Mínimos:**

| Columna | Descripción |
|---|---|
| **Sector** | Sector económico (agricultura, comercio, etc.) |
| **Salario C$** | Monto en Córdobas Nicaragüenses |
| **Salario USD** | Equivalente en Dólares |
| **Estado** | ✅ Vigente |

**Acción: 📥 Importar Acuerdo MITRAB (CSV)**
1. Haz clic en **📥 Importar Acuerdo MITRAB (CSV)**
2. Se abre diálogo de carga
3. Selecciona archivo CSV con estructura MITRAB
4. Sistema valida y carga datos
5. Confirmación: "✅ Acuerdo MITRAB importado"

**Formato Esperado del CSV:**
```
Sector,SalarioNIO,SalarioUSD
Agricultura,6000.00,164.20
Comercio,6500.00,177.80
```

---

#### 3.3️⃣ TAB: SEGURIDAD

**Funcionalidad:**
- Visualizar políticas de seguridad e inmutabilidad de datos

**Políticas Configuradas:**

| Política | Estado | Descripción |
|---|:---:|---|
| Bloquear UPDATE/DELETE en períodos cerrados | ✅ | Previene alteración de nóminas procesadas |
| Requerir Firma Digital Doble | ✅ | Aprobación dual para cambios críticos |

**Algoritmo de Hash:** SHA-256

---

#### 3.4️⃣ TAB: NOTIFICACIONES - INTEGRACIONES

**Funcionalidad:**
- Visualizar integraciones de terceros y última sincronización

**Integraciones Configuradas:**

| Integración | Descripción | Última Sync |
|---|---|---|
| **API BCN** | Obtiene tasa de cambio oficial | [Timestamp] |
| **Bancos ACH** | Conexión para pagos nómina | BAC, Banpro, Lafise, Ficohsa |

**Actualización de Tasa BCN:**
- Se sincroniza automáticamente a las 12:00 del mediodía
- Aparece en la Empresa Principal: **💱 Tasa BCN**

---

### 📸 Captura de Pantalla - Configuración Fiscal

```
┌──────────────────────────────────────────────────────────────────────┐
│  📊 Fiscales - Historial INSS                                        │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de la tabla de INSS]                               │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre pestaña "Configuración Global" → "Fiscales"                │
│  2. Captura la tabla de historial INSS mostrando:                   │
│     ✓ Al menos 1-2 vigencias (una vigente, una histórica)          │
│     ✓ Botón: ➕ Registrar Nueva Vigencia                          │
│     ✓ Columnas: Vigencia, Porcentajes, Tope, Estado               │
│  3. Guarda como: "config-fiscal-inss.png"                           │
│  4. Reemplaza este bloque con:                                      │
│     ![Configuración INSS](./images/config-fiscal-inss.png)          │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el texto anterior con:
> ```markdown
> ![Configuración INSS](./images/config-fiscal-inss.png)
> ```

---

### 📸 Captura de Pantalla - Tabla IR

```
┌──────────────────────────────────────────────────────────────────────┐
│  🧾 IR - Tramos Impositivos                                          │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de la tabla IR]                                    │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. En la sub-pestaña Fiscales, localiza sección "IR"               │
│  2. Captura mostrando:                                               │
│     ✓ Selector de Año (arriba a la izquierda)                      │
│     ✓ Tabla con tramos salariales (3-5 filas)                      │
│     ✓ Botón: 📥 Importar Tabla DGI CSV                            │
│  3. Guarda como: "config-ir-tramos.png"                             │
│  4. Reemplaza este bloque con:                                      │
│     ![Tabla IR](./images/config-ir-tramos.png)                      │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el texto anterior con:
> ```markdown
> ![Tabla IR](./images/config-ir-tramos.png)
> ```

---

### 📸 Captura de Pantalla - Salarios Mínimos MITRAB

```
┌──────────────────────────────────────────────────────────────────────┐
│  💰 Salarios Mínimos MITRAB                                          │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de la tabla MITRAB]                                │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre sub-pestaña "Laborales"                                     │
│  2. Captura mostrando:                                               │
│     ✓ Selector de Año MITRAB                                        │
│     ✓ Tabla con sectores y salarios (3-5 filas)                    │
│     ✓ Columnas: Sector, Salario C$, Salario USD, Estado            │
│     ✓ Botón: 📥 Importar Acuerdo MITRAB (CSV)                    │
│  3. Guarda como: "config-mitrab-salarios.png"                       │
│  4. Reemplaza este bloque con:                                      │
│     ![Salarios MITRAB](./images/config-mitrab-salarios.png)         │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el texto anterior con:
> ```markdown
> ![Salarios MITRAB](./images/config-mitrab-salarios.png)
> ```

---

## 🎯 PROCEDIMIENTOS COMUNES

### Procedimiento 1: Actualizar Datos de Empresa Principal

**Objetivo:** Cambiar información legal o comercial de la empresa

**Pasos:**
1. Accede al módulo → Pestaña "Empresa Principal"
2. Localiza el campo a actualizar
3. Realiza el cambio (ej: nuevo email, teléfono)
4. Haz clic en **💾 Guardar Cambios**
5. Espera confirmación "✅ Datos de empresa actualizados"
6. Si hay error, verifica:
   - Campos requeridos completos (marcados con *)
   - Formato correcto (RUC, INSS, CIIU)
   - Permiso de acceso

---

### Procedimiento 2: Crear Sucursal Nueva

**Objetivo:** Registrar una nueva ubicación operativa

**Pasos:**
1. Accede al módulo → Pestaña "Sucursales"
2. Haz clic en **➕ Nueva Sucursal**
3. Completa los datos mínimos:
   - 🏷️ Código: SUC-002 (formato propuesto)
   - 🏢 Nombre: "Sucursal León" (descriptivo)
   - 📍 Dirección: calle y número exactos
   - 🏙️ Ciudad: ciudad/departamento
   - 📞 Teléfono: número de contacto
4. Opcional: asigna gerente y centro de costo
5. Haz clic en **💾 Guardar Sucursal**
6. Verifica que aparezca en la tabla con estado 🟢 Activa

---

### Procedimiento 3: Registrar Nueva Vigencia INSS

**Objetivo:** Actualizar parámetros INSS cuando cambia la ley

**Pasos:**
1. Accede al módulo → Pestaña "Configuración Global" → Sub-pestaña "Fiscales"
2. Busca la sección "🏛️ INSS"
3. Haz clic en **➕ Registrar Nueva Vigencia**
4. En el diálogo, completa:
   - 📅 Vigencia Desde: [fecha de inicio]
   - 📅 Vigencia Hasta: [dejar vacío si es vigente]
   - 📋 Fundamento Legal: "Ley 822" o decreto aplicable
   - 💯 % Trabajador: porcentaje (ej: 8.25)
   - 💯 % Empleador: porcentaje (ej: 15.5)
   - 🔝 Tope Salarial: límite máximo (ej: 100,000)
5. Haz clic en **Guardar**
6. La nueva vigencia aparecerá en la tabla
7. La anterior pasará a estado ⏳ Histórico

---

### Procedimiento 4: Importar Tabla IR de la DGI

**Objetivo:** Cargar nueva tabla de impuestos cuando cambia la reforma fiscal

**Pasos:**
1. Obtén el archivo CSV de la DGI (Dirección General de Ingresos)
2. Accede al módulo → Configuración Global → Fiscales → Sección "IR"
3. Selecciona el año en el dropdown (arriba a la izquierda)
4. Haz clic en **📥 Importar Tabla DGI CSV**
5. En el diálogo:
   - Selecciona el archivo CSV
   - El sistema valida formato
   - Haz clic en "Subir" o "Cargar"
6. Espera confirmación "✅ Tabla IR cargada correctamente"
7. Verifica que la tabla se haya actualizado con los nuevos tramos

**Validación de CSV:**
- Columnas: Tramo, Base, Marginal, Exceso
- Separador: comas
- Decimales: punto (.)
- Sin encabezados si el sistema lo especifica

---

### Procedimiento 5: Desactivar Sucursal

**Objetivo:** Deshabilitar una sucursal que ya no opera

**Pasos:**
1. Accede al módulo → Pestaña "Sucursales"
2. Localiza la sucursal en la tabla
3. Haz clic en el icono 🗑️ (Eliminar/Desactivar)
4. Aparecerá confirmación: "¿Desactivar la sucursal '[nombre]'?"
5. Haz clic en **Desactivar**
6. Se mostrará "✅ Sucursal '[nombre]' desactivada"
7. La sucursal pasará a estado 🟡 Inactiva
8. Las nóminas futuras no usarán esta sucursal por defecto

---

## ⚠️ GUÍA DE TROUBLESHOOTING

### Problema 1: No puedo guardar los datos de empresa

**Síntoma:** Al hacer clic en "Guardar Cambios", no pasa nada o aparece error

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Campos requeridos vacíos | Verifica que todos los campos con * estén completos |
| Formato RUC incorrecto | Debe ser: J + 13 dígitos (ej: J0610010123456) |
| Formato INSS incorrecto | Debe ser: P + 6 dígitos (ej: P-123456) |
| Permiso insuficiente | Verifica que tengas rol de Administrador o RH |
| Conexión a internet | Recarga la página e intenta nuevamente |

---

### Problema 2: La tasa de cambio BCN no se actualiza

**Síntoma:** En "Empresa Principal" la tasa muestra fecha antigua

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Sincronización manual no ejecutada | Espera a las 12:00 (sincronización automática) |
| API BCN no disponible | Contacta al equipo de IT |
| Zona horaria del servidor | Verifica configuración de servidor |

---

### Problema 3: No puedo eliminar/desactivar una sucursal

**Síntoma:** Al hacer clic en eliminar, aparece error o nada ocurre

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Sucursal tiene empleados activos | Transfiere empleados a otra sucursal primero |
| Sucursal tiene nóminas abiertas | Cierra todos los períodos de nómina de esa sucursal |
| Permiso insuficiente | Contacta al administrador del sistema |

---

### Problema 4: El archivo CSV no carga para IR o MITRAB

**Síntoma:** Al intentar importar, aparece error de validación

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Formato de columnas incorrecto | Verifica estructura esperada del CSV |
| Separador incorrecto | Use comas (,) como separador |
| Decimales con coma en lugar de punto | Cambia formato: 15,5 → 15.5 |
| Archivo corrupto | Descarga nuevamente desde DGI |
| Archivo CSV con BOM | Guarda como UTF-8 sin BOM |

**Pasos para corregir CSV:**
1. Abre con editor de texto (Notepad++)
2. Codificación → UTF-8 sin BOM
3. Reemplazar: encuentra "," por "."
4. Verifica separador sea coma
5. Guarda e intenta nuevamente

---

### Problema 5: No veo Centro de Costo al crear sucursal

**Síntoma:** El dropdown de Centro de Costo está vacío

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| No existen centros de costo creados | Crea primero los centros en módulo correspondiente |
| Centros están inactivos | Activa los centros de costo necesarios |
| Permiso de lectura insuficiente | Contacta al administrador |

---

## 🔗 INTEGRACIÓN CON OTROS MÓDULOS

### Módulo de Empleados
- **Relación:** Las sucursales creadas aquí aparecen como opciones al registrar empleados
- **Flujo:** Empresa → Sucursal → Empleado con sucursal asignada
- **Datos sincronizados:** Nombre, código y estado de sucursal

### Módulo de Nómina
- **Relación:** Los parámetros fiscales (INSS, IR, INATEC) afectan cálculos de nómina
- **Flujo:** Config INSS → Cálculo de aportes → Acción de nómina
- **Datos sincronizados:** Tasa de cambio BCN, frecuencias de pago

### Módulo de Reportes
- **Relación:** Exportaciones (PDF, Excel) usan logo y datos de empresa
- **Flujo:** Logo empresa → Generación de reportes
- **Datos sincronizados:** Nombre legal, RUC, dirección

### Módulo de Acciones de Personal
- **Relación:** El representante legal es parte de cambios estructurales
- **Flujo:** Cambio representante → Auditoría
- **Datos sincronizados:** Representante legal y su documento

---

## 📞 SOPORTE Y CONTACTO

### Contacto Técnico
- **Email**: soporte@gestiona360.com
- **Teléfono**: +505 XXXX-XXXX
- **Horario**: Lunes a Viernes, 8:00 - 17:00

### Escalamiento
Para problemas no resueltos aquí:
1. Documenta el error (captura pantalla + mensaje exacto)
2. Anota: hora, usuario, acción realizada
3. Contacta a soporte con esta información

---

## 🎓 RECURSOS ADICIONALES

- 📖 Manual Sistema Completo: [enlace]
- 🎥 Video Tutorial: [enlace]
- 📊 Ejemplos de CSV para importación: [enlace]
- 🔐 Políticas de Seguridad: [enlace]

---

**Versión:** 1.0  
**Última actualización:** 2025  
**Próxima revisión:** Q2 2025

