# 📘 Manual de Usuario - Módulo Gestión de Empleados

## 📄 Información del Documento

| Propiedad | Valor |
|---|---|
| **Módulo** | Gestión de Empleados |
| **Versión** | 1.0 |
| **Fecha de Actualización** | 2025 |
| **Estado** | ✅ Parcialmente Implementado (67% - 2 de 3 opciones principales) |
| **Ubicación en Menú** | 👥 Empleados → Listado de Empleados / Acciones de Personal |
| **Rutas de Acceso** | `/employees`, `/personal-actions` |

---

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Acceso al Módulo](#acceso-al-módulo)
3. [Sección 1: Listado de Empleados](#sección-1-listado-de-empleados)
4. [Sección 2: Crear Nuevo Empleado](#sección-2-crear-nuevo-empleado)
5. [Sección 3: Editar Empleado](#sección-3-editar-empleado)
6. [Sección 4: Detalle de Empleado](#sección-4-detalle-de-empleado)
7. [Sección 5: Acciones de Personal](#sección-5-acciones-de-personal)
8. [Sección 6: Asignación de Conceptos](#sección-6-asignación-de-conceptos)
9. [Sección 7: Turnos y Horarios](#sección-7-turnos-y-horarios)
10. [Procedimientos Comunes](#procedimientos-comunes)
11. [Guía de Troubleshooting](#guía-de-troubleshooting)
12. [Integración con Otros Módulos](#integración-con-otros-módulos)

---

## 🎯 Descripción General

El módulo **Gestión de Empleados** es el corazón del sistema de nómina. Permite registrar, actualizar y administrar la información de todos los empleados de la empresa, sus contratos, estados laborales y asignaciones.

### Funcionalidades Principales

✅ Listado completo de empleados con filtros avanzados  
✅ Crear nuevos empleados con asistente (wizard)  
✅ Editar información personal, laboral y fiscal  
✅ Ver detalle completo de cada empleado  
✅ Registrar acciones de personal (cambios de estado, promociones)  
✅ Asignar conceptos de nómina por empleado  
✅ Gestionar turnos y horarios  
✅ Historial de nómina individual  
✅ Exportación de reportes  

### Acceso Requerido

- **Rol Mínimo**: Recursos Humanos o Administrador
- **Permisos**: Lectura y modificación de empleados

### Funcionalidades NO Implementadas ⏳

- ❌ Control de Asistencia (Pendiente)
- ❌ Finiquitos (Pendiente)

---

## 🚀 Acceso al Módulo

### Opción 1: Desde el Menú Principal
1. Localiza el menú lateral izquierdo
2. Haz clic en **👥 Empleados**
3. Selecciona una opción:
   - **Listado de Empleados** → Ver todos los empleados
   - **Acciones de Personal** → Registrar cambios

### Opción 2: Acceso Directo
- Listado: `https://tu-dominio.com/employees`
- Acciones: `https://tu-dominio.com/personal-actions`

---

## 🖥️ SECCIÓN 1: LISTADO DE EMPLEADOS

### Propósito
Visualizar todos los empleados con información resumida, aplicar filtros y acceder a operaciones de gestión.

### Interfaz Principal

```
┌─────────────────────────────────────────────────────────────┐
│  👥 Gestión de Empleados                                    │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  📊 ESTADÍSTICAS (4 tarjetas)                              │
│  • Total Empleados: 150                                    │
│  • ✅ Activos: 145                                          │
│  • ⏸️ Suspendidos: 3                                         │
│  • ⏳ En Prueba: 2                                           │
│                                                              │
│  🔍 BÚSQUEDA Y FILTROS                                     │
│  [Buscar...]    [Buscar] [Limpiar]                         │
│  [Sucursal ▼] [Tipo Contrato ▼] [Estado ▼] [Puesto ▼]   │
│  [Confianza ▼] [Extranjero ▼] [En Prueba ▼] [Reingreso ▼] │
│                                                              │
│  📋 TABLA DE EMPLEADOS                                      │
│  [Cód.] [Nombre] [ID] [Sucursal] [Estado] [Puesto] [Acciones] │
│  EMP001 Juan Pérez 00112003... Managua ✅ Contable [👁️🖊️🗑️]  │
│  EMP002 María López 00201004... León ✅ Gerente [👁️🖊️🗑️]    │
│                                                              │
│  📤 EXPORTAR EXCEL                                          │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### 📊 Tarjetas de Estadísticas

| Estadística | Icono | Color | Descripción |
|---|---|---|---|
| **Total Empleados** | 👥 | Azul (Primary) | Suma de todos los empleados registrados |
| **Activos** | ✅ | Verde (Success) | Empleados en estado laboral activo |
| **Suspendidos** | ⏸️ | Naranja (Warning) | Empleados temporalmente suspensos |
| **En Prueba** | ⏳ | Gris (Info) | Empleados en período de prueba |

### 🔍 Búsqueda y Filtros

#### Búsqueda Rápida
- **Campo**: "Buscar por nombre, cédula, código, correo, RUC o INSS..."
- **Funcionamiento**: Presiona Enter o haz clic en "Buscar"
- **Limpieza**: Haz clic en "Limpiar" para resetear búsqueda

#### Filtros Avanzados (Fila 1)

| Filtro | Tipo | Opciones | Efecto |
|---|---|---|---|
| **🏙️ Sucursal** | Dropdown | Todas / [Sucursal 1] / [Sucursal 2] | Filtra empleados por sucursal |
| **📄 Tipo Contrato** | Dropdown | Todos / [Contrato A] / [Contrato B] | Filtra por tipo de contrato |
| **⚙️ Estado Laboral** | Dropdown | Todos / ✅ Activo / ⏸️ Suspendido / ⏹️ Terminado | Filtra por estado actual |
| **👔 Puesto** | Dropdown | Todos / [Puesto 1] / [Puesto 2] | Filtra por cargo/posición |

#### Filtros Avanzados (Fila 2)

| Filtro | Tipo | Opciones | Efecto |
|---|---|---|---|
| **👔 Confianza** | Dropdown | Todos / ✅ Sí / ❌ No | Personal de confianza |
| **🌍 Extranjero** | Dropdown | Todos / ✅ Sí / ❌ No | Personalidad jurídica extranjera |
| **⏳ En Período Prueba** | Dropdown | Todos / ✅ Sí / ❌ No | Período de prueba activo |
| **🔄 Reingreso** | Dropdown | Todos / ✅ Sí / ❌ No | Empleados que se reincorporan |

### 📋 Tabla de Empleados

**Columnas:**

| Columna | Contenido | Ancho | Descripción |
|---|---|---|---|
| **Código** | EMP001 | Fijo | Identificador único del empleado |
| **Nombre Completo** | Juan Pérez García | Flexible | Nombre y apellido |
| **Identificación** | 00112003400123 | Fijo | Número de cédula o pasaporte |
| **Sucursal** | Managua | Fijo | Ubicación del empleado |
| **Estado Laboral** | ✅ Activo | Fijo | Chip de color según estado |
| **Puesto** | Contador | Flexible | Posición o cargo |
| **Acciones** | 👁️ 🖊️ 🗑️ | Fijo | Iconos de acción |

**Estados Laborales (Chips):**
- 🟢 ✅ Activo - Verde
- 🟡 ⏸️ Suspendido - Naranja
- 🔴 ⏹️ Terminado - Rojo
- 🔵 ⏳ En Prueba - Azul

### ⚙️ Operaciones Disponibles

#### 👁️ Ver Detalle
1. En la tabla, haz clic en el icono **👁️**
2. Se abrirá la página de detalle del empleado
3. Verás toda la información del empleado de forma resumida

#### 🖊️ Editar Empleado
1. En la tabla, haz clic en el icono **🖊️**
2. Se abrirá el formulario de edición
3. Realiza los cambios necesarios
4. Haz clic en "Guardar"

#### 🗑️ Desactivar Empleado
1. En la tabla, haz clic en el icono **🗑️**
2. Se mostrará una confirmación
3. Haz clic en "Desactivar" para confirmar
4. El empleado pasará a estado ⏹️ Terminado

#### ➕ Crear Nuevo Empleado
1. Localiza el botón **➕ Nuevo Empleado** (arriba de la tabla)
2. Se abrirá un asistente (Wizard) paso a paso
3. Completa la información en cada pantalla
4. Finaliza con "Crear Empleado"

#### 📤 Exportar Lista (Excel)
1. Haz clic en **📤 Exportar Listado (Excel)**
2. Se generará un archivo con todos los empleados filtrados
3. Se descargará automáticamente

---

### 📸 Captura de Pantalla - Listado de Empleados

```
┌──────────────────────────────────────────────────────────────────────┐
│  👥 Gestión de Empleados - Listado                                   │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen del listado completo]                              │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Captura la pantalla mostrando:                                  │
│     ✓ Las 4 tarjetas de estadísticas (Total, Activos, etc.)       │
│     ✓ Barras de búsqueda y filtros                                 │
│     ✓ Tabla con 3-5 empleados de ejemplo                          │
│     ✓ Estados visuales (chips de color)                            │
│     ✓ Botones de acción (Nueva Empleado, Exportar, etc.)         │
│  2. Usa datos de ejemplo (no sensibles)                            │
│  3. Guarda como: "listado-empleados.png"                           │
│  4. Reemplaza este bloque con:                                      │
│     ![Listado de Empleados](./images/listado-empleados.png)        │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Listado de Empleados](./images/listado-empleados.png)
> ```

---

## 🖥️ SECCIÓN 2: CREAR NUEVO EMPLEADO

### Propósito
Registrar un nuevo empleado en el sistema mediante un formulario asistente paso a paso.

### Acceso

**Opción 1: Desde Listado de Empleados**
1. Ve a 👥 Empleados → Listado de Empleados
2. Haz clic en **➕ Nuevo Empleado**
3. Se abre el Asistente (Wizard) de 5 pasos

**Opción 2: Acceso Directo**
- URL: `https://tu-dominio.com/employees/wizard` (si aplica)

### Asistente de Creación (5 Pasos)

#### Paso 1️⃣: INFORMACIÓN PERSONAL

**Campos:**

| Campo | Requerido | Formato | Descripción |
|---|:---:|---|---|
| **Nombres** | ✅ | Texto | Nombre(s) del empleado |
| **Apellidos** | ✅ | Texto | Apellido(s) del empleado |
| **Cédula/Pasaporte** | ✅ | 14 dígitos o formato pasaporte | Identificación personal |
| **Correo Electrónico** | ✅ | email@ejemplo.com | Para comunicaciones |
| **Teléfono** | ✅ | +505 XXXX-XXXX | Número de contacto |
| **Fecha Nacimiento** | ✅ | dd/MM/yyyy | Para cálculos de edad |
| **Género** | ✅ | Masculino / Femenino / Otro | Datos demográficos |

#### Paso 2️⃣: INFORMACIÓN FISCAL

**Campos:**

| Campo | Requerido | Formato | Descripción |
|---|:---:|---|---|
| **Número RUC** | ✅ | JXXXXXXXXXX | Registro único de contribuyente |
| **Número INSS** | ✅ | XXXXXXXXXXX | Número de seguridad social |
| **Extranjero** | ❌ | Sí / No | ¿Es extranjero? |

#### Paso 3️⃣: INFORMACIÓN LABORAL

**Campos:**

| Campo | Requerido | Tipo | Descripción |
|---|:---:|---|---|
| **Sucursal** | ✅ | Dropdown | Ubicación de trabajo |
| **Centro de Costo** | ❌ | Dropdown | Asignación contable |
| **Puesto** | ✅ | Dropdown | Cargo o posición |
| **Tipo Contrato** | ✅ | Dropdown | Permanente, Temporal, etc. |
| **Salario Base Mensual** | ✅ | Decimal | En moneda de la empresa |
| **Frecuencia Pago** | ✅ | Dropdown | Semanal, Quincenal, Mensual |
| **Fecha Ingreso** | ✅ | dd/MM/yyyy | Día de incorporación |

#### Paso 4️⃣: DATOS ADICIONALES

**Campos:**

| Campo | Requerido | Tipo | Descripción |
|---|:---:|---|---|
| **¿Personal de Confianza?** | ❌ | Sí / No | Puestos directivos |
| **¿En Período de Prueba?** | ❌ | Sí / No | Primeros 90 días |
| **Duración Período Prueba** | ❌ | Número de días | Si aplica período prueba |
| **¿Es Reingreso?** | ❌ | Sí / No | Empleado anterior |
| **Fecha Reingreso Anterior** | ❌ | dd/MM/yyyy | Si es reingreso |

#### Paso 5️⃣: RESUMEN Y CONFIRMACIÓN

**Contenido:**
- ✅ Resumen de todos los datos ingresados
- ⚠️ Validaciones pendientes
- 🎯 Botón "Crear Empleado"
- 🔙 Botón "Atrás" para corregir

### 📸 Captura de Pantalla - Asistente Crear Empleado

```
┌──────────────────────────────────────────────────────────────────────┐
│  ➕ Nuevo Empleado - Asistente Paso a Paso                           │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen del wizard]                                        │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Inicia el proceso de crear nuevo empleado                       │
│  2. Captura mostrando:                                               │
│     ✓ Indicador de progreso (1/5, 2/5, etc.)                       │
│     ✓ Campos del Paso 1 (Información Personal)                     │
│     ✓ Botones: Anterior, Siguiente, Cancelar                       │
│     ✓ Validaciones en tiempo real                                   │
│  3. Opcionalmente captura otros pasos                               │
│  4. Guarda como: "wizard-nuevo-empleado.png"                        │
│  5. Reemplaza este bloque con:                                      │
│     ![Asistente Crear Empleado](./images/wizard-nuevo-empleado.png) │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Asistente Crear Empleado](./images/wizard-nuevo-empleado.png)
> ```

---

## 🖥️ SECCIÓN 3: EDITAR EMPLEADO

### Propósito
Modificar información de un empleado existente.

### Acceso

1. Ve a 👥 Empleados → Listado de Empleados
2. Localiza el empleado a editar
3. Haz clic en el icono **🖊️** (Editar)
4. O accede directamente: `https://tu-dominio.com/employees/{ID}/edit`

### Formulario de Edición

El formulario se divide en **7 secciones** principales:

#### 📋 SECCIÓN 1: DATOS PERSONALES

| Campo | Editable | Descripción |
|---|:---:|---|
| Cédula | ❌ | Solo lectura (no puede cambiar) |
| Código Empleado | ❌ | Solo lectura (sistema) |
| Nombres | ✅ | Actualizar nombres |
| Apellidos | ✅ | Actualizar apellidos |
| Correo | ✅ | Cambiar email |
| Teléfono | ✅ | Actualizar número |

#### 📋 SECCIÓN 2: DATOS FISCALES

| Campo | Editable | Descripción |
|---|:---:|---|
| N° RUC | ✅ | Cambiar registro RUC |
| N° INSS | ✅ | Cambiar número INSS |

#### 📋 SECCIÓN 3: ASIGNACIÓN LABORAL

| Campo | Editable | Descripción |
|---|:---:|---|
| Sucursal | ✅ | Cambiar sucursal |
| Centro de Costo | ✅ | Reasignar centro |
| Puesto | ✅ | Cambiar cargo |
| Tipo Contrato | ✅ | Actualizar contrato |

#### 📋 SECCIÓN 4: SALARIO Y BENEFICIOS

| Campo | Editable | Descripción |
|---|:---:|---|
| Salario Base Mensual | ✅ | Ajustar remuneración |
| Frecuencia Pago | ✅ | Cambiar período de pago |
| ¿Zona Franca? | ✅ | Cambiar régimen fiscal |

#### 📋 SECCIÓN 5: DETALLES ADICIONALES

| Campo | Editable | Descripción |
|---|:---:|---|
| Personal de Confianza | ✅ | Actualizar estatus |
| ¿Extranjero? | ✅ | Cambiar nacionalidad |
| En Período Prueba | ✅ | Activar/desactivar prueba |
| Es Reingreso | ✅ | Marcar como reingreso |

#### 📋 SECCIÓN 6: FECHAS IMPORTANTES

| Campo | Editable | Descripción |
|---|:---:|---|
| Fecha Ingreso | ❌ | Solo lectura (histórico) |
| Fecha Inicio Período Prueba | ✅ | Cambiar si aplica |
| Duración Período Prueba | ✅ | Días de prueba |

#### 📋 SECCIÓN 7: ESTADO LABORAL

| Campo | Editable | Descripción |
|---|:---:|---|
| Estado Actual | ✅ | Activo / Suspendido / Terminado |
| Motivo Cambio Estado | ✅ | Descripción del cambio |
| Fecha Cambio | ✅ | Cuándo se realiza cambio |

### Operaciones

#### 💾 Guardar Cambios
1. Completa los campos necesarios
2. Haz clic en **💾 Guardar Cambios** (botón verde)
3. Se mostrará confirmación "✅ Empleado actualizado"
4. Serás redirigido al listado

#### ❌ Cancelar Edición
1. Haz clic en **Cancelar** (botón gris)
2. Se descartarán todos los cambios
3. Volverás al listado

---

### 📸 Captura de Pantalla - Editar Empleado

```
┌──────────────────────────────────────────────────────────────────────┐
│  ✏️ Editar Empleado: Juan Pérez García                               │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen del formulario de edición]                         │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre un empleado en modo edición                                │
│  2. Captura mostrando:                                               │
│     ✓ Encabezado: "Editar Empleado: [Nombre]"                     │
│     ✓ Secciones: Datos Personales, Fiscales, Laborales            │
│     ✓ Campos editable y solo lectura                                │
│     ✓ Botones: Guardar Cambios, Cancelar                           │
│     ✓ Indicadores de campos requeridos (*)                          │
│  3. Muestra al menos 2 secciones completas                          │
│  4. Guarda como: "editar-empleado.png"                              │
│  5. Reemplaza este bloque con:                                      │
│     ![Formulario Editar](./images/editar-empleado.png)              │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Formulario Editar](./images/editar-empleado.png)
> ```

---

## 🖥️ SECCIÓN 4: DETALLE DE EMPLEADO

### Propósito
Visualizar toda la información de un empleado de forma consolidada y realizar acciones asociadas.

### Acceso

1. Ve a 👥 Empleados → Listado de Empleados
2. Haz clic en el icono **👁️** (Ver detalle)
3. O accede: `https://tu-dominio.com/employees/{ID}`

### Vista de Detalle

La página de detalle incluye **5 pestañas principales**:

#### Pestaña 1️⃣: INFORMACIÓN GENERAL

**Contenido:**
- Foto de perfil (si disponible)
- Nombre completo y código
- Cédula e identificación fiscal
- Estado laboral actual (Chip de color)
- Información de contacto

**Acciones:**
- 🖊️ Editar (va a formulario de edición)
- 📞 Contactar (abre email/teléfono)
- 📋 Ver más detalles

#### Pestaña 2️⃣: INFORMACIÓN LABORAL

**Contenido:**
- Sucursal y centro de costo
- Puesto y tipo de contrato
- Salario base mensual
- Frecuencia de pago
- Fecha de ingreso
- Período de prueba (si aplica)

#### Pestaña 3️⃣: CONCEPTOS ASIGNADOS

**Contenido:**
- Tabla de conceptos de nómina
- Valores y frecuencias

**Acciones:**
- ➕ Agregar concepto
- 🖊️ Editar concepto
- 🗑️ Remover concepto

#### Pestaña 4️⃣: HISTORIAL DE NÓMINA

**Contenido:**
- Tabla con nóminas procesadas
- Montos, períodos, estados
- Links a colillas individuales

**Acciones:**
- 📄 Ver colilla (PDF)
- 👁️ Ver detalle

#### Pestaña 5️⃣: ACCIONES DE PERSONAL

**Contenido:**
- Historial de cambios de estado
- Cambios de puesto
- Cambios de salario

---

### 📸 Captura de Pantalla - Detalle de Empleado

```
┌──────────────────────────────────────────────────────────────────────┐
│  👤 Detalle de Empleado: Juan Pérez García                           │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen del detalle]                                       │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre un empleado en modo detalle                                │
│  2. Captura mostrando:                                               │
│     ✓ Encabezado con nombre del empleado                           │
│     ✓ Foto (o icono por defecto)                                   │
│     ✓ Estado laboral (chip de color)                                │
│     ✓ Tabs: Información General, Laboral, Conceptos, Nómina       │
│     ✓ Información del tab activo                                    │
│     ✓ Botones de acción                                             │
│  3. Guarda como: "detalle-empleado.png"                             │
│  4. Reemplaza este bloque con:                                      │
│     ![Detalle de Empleado](./images/detalle-empleado.png)           │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Detalle de Empleado](./images/detalle-empleado.png)
> ```

---

## 🖥️ SECCIÓN 5: ACCIONES DE PERSONAL

### Propósito
Registrar cambios en la situación laboral de empleados (cambios de estado, promociones, cambios salariales, etc.).

### Acceso

1. Ve a 👥 Empleados → **Acciones de Personal**
2. O accede: `https://tu-dominio.com/personal-actions`

### Interfaz Principal

```
┌─────────────────────────────────────────────────────────────┐
│  📋 Acciones de Personal                                    │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  📊 ESTADÍSTICAS                                           │
│  • Acciones Este Mes: 12                                   │
│  • Cambios de Estado: 5                                    │
│  • Promociones: 3                                          │
│  • Cambios Salariales: 4                                   │
│                                                              │
│  📋 HISTORIAL DE ACCIONES                                  │
│  [Empleado] [Tipo Acción] [Fecha] [Usuario] [Detalles]   │
│  Juan Pérez Cambio Estado 15/01 Admin Activo → Suspendido │
│                                                              │
│  ➕ REGISTRAR NUEVA ACCIÓN                                 │
│  [Empleado ▼] [Tipo Acción ▼] [Detalles...]              │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### 📊 Tipos de Acciones de Personal

| Tipo | Descripción | Campos Asociados |
|---|---|---|
| **Cambio de Estado** | Modificar estado laboral | Estado anterior, Estado nuevo, Motivo, Fecha efectiva |
| **Promoción** | Ascenso de puesto | Puesto anterior, Puesto nuevo, Aumento salarial, Fecha |
| **Cambio de Salario** | Ajuste remunerativo | Salario anterior, Salario nuevo, Motivo, Porcentaje cambio |
| **Cambio de Sucursal** | Traslado a otra sucursal | Sucursal anterior, Sucursal nueva, Motivo, Fecha |
| **Cambio de Centro Costo** | Reasignación contable | Centro anterior, Centro nuevo, Fecha |
| **Cambio de Contrato** | Modificación de tipo contrato | Contrato anterior, Contrato nuevo, Motivo, Fecha |
| **Otros** | Cambios misceláneos | Descripción libre, Documentos adjuntos |

### ➕ Registrar Nueva Acción

#### Paso 1: Seleccionar Empleado
1. Haz clic en **➕ Nueva Acción**
2. Abre dropdown: "Seleccionar Empleado"
3. Busca el empleado por:
   - Nombre completo
   - Código
   - Cédula

#### Paso 2: Seleccionar Tipo de Acción
1. En el dropdown "Tipo de Acción", selecciona:
   - Cambio de Estado
   - Promoción
   - Cambio de Salario
   - Cambio de Sucursal
   - Otro

#### Paso 3: Completar Detalles
Los campos cambian según el tipo de acción:

**Para Cambio de Estado:**
- Estado anterior (solo lectura)
- Estado nuevo (Activo / Suspendido / Terminado)
- Motivo del cambio
- Fecha efectiva

**Para Promoción:**
- Puesto anterior (solo lectura)
- Puesto nuevo (seleccionar)
- Salario anterior (solo lectura)
- Nuevo salario
- Fecha de promoción

**Para Cambio de Salario:**
- Salario anterior (solo lectura)
- Nuevo salario
- Motivo (Aumento, Reajuste, Reducción)
- % de cambio (calculado automáticamente)
- Fecha efectiva

#### Paso 4: Guardar Acción
1. Haz clic en **💾 Registrar Acción**
2. Se mostrará confirmación
3. La acción aparecerá en el historial

### 📋 Historial de Acciones

**Columnas de la tabla:**

| Columna | Descripción |
|---|---|
| **Empleado** | Nombre completo |
| **Tipo Acción** | Categoría de acción |
| **Fecha** | Cuándo se registró |
| **Usuario** | Quién registró la acción |
| **Detalles** | Cambios específicos |
| **Acciones** | 👁️ Ver / 🖊️ Editar / 🗑️ Eliminar |

---

### 📸 Captura de Pantalla - Acciones de Personal

```
┌──────────────────────────────────────────────────────────────────────┐
│  📋 Acciones de Personal - Historial                                 │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen del módulo de acciones]                            │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre el módulo de Acciones de Personal                          │
│  2. Captura mostrando:                                               │
│     ✓ Tarjetas de estadísticas                                       │
│     ✓ Tabla con 3-5 acciones registradas                           │
│     ✓ Tipos de acciones variados                                    │
│     ✓ Botón: ➕ Nueva Acción                                       │
│     ✓ Estados visuales (cambio de color, etc.)                      │
│  3. Guarda como: "acciones-personal.png"                            │
│  4. Reemplaza este bloque con:                                      │
│     ![Acciones de Personal](./images/acciones-personal.png)         │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Acciones de Personal](./images/acciones-personal.png)
> ```

---

## 🖥️ SECCIÓN 6: ASIGNACIÓN DE CONCEPTOS

### Propósito
Asignar conceptos de nómina específicos a cada empleado (bonificaciones, deducciones, beneficios especiales).

### Acceso

1. Ve a 👥 Empleados → Listado de Empleados
2. Abre un empleado (👁️ Ver detalle)
3. En la pestaña **"Conceptos Asignados"**, haz clic en **➕ Agregar Concepto**
4. O accede directamente: `https://tu-dominio.com/employees/{ID}/concepts`

### Conceptos Disponibles

| Tipo | Código | Descripción | % Típico |
|---|---|---|---|
| **Bonificación** | BON001 | Bonificación por desempeño | Variable |
| **Bonificación de Riesgo** | BOR001 | Riesgo ocupacional (según MITRAB) | 5-15% |
| **Comisión** | COM001 | Comisión por ventas | Variable |
| **Deducción Legal** | DED001 | Retención INSS, IR, etc. | Regulada |
| **Deducción Voluntaria** | DEV001 | Cuota sindical, cooperativa | Variable |
| **Beneficio Especial** | BEN001 | Descuentos, becas, etc. | Variable |

### Agregar Concepto

#### Paso 1: Seleccionar Concepto
1. Haz clic en **➕ Agregar Concepto**
2. Se abre un diálogo o formulario
3. Selecciona el concepto de la lista

#### Paso 2: Configurar Parámetros
- **Concepto**: [Nombre del concepto - solo lectura]
- **Tipo**: [Bonificación / Deducción / Beneficio]
- **Monto o %**: Valor fijo o porcentaje
- **Frecuencia**: Cada nómina / Mensual / Trimestral
- **Fecha Inicio**: Cuándo aplica
- **Fecha Fin**: Cuándo termina (opcional si es indefinido)
- **Motivo**: Descripción del concepto
- **Notas**: Información adicional

#### Paso 3: Guardar
1. Haz clic en **💾 Guardar Concepto**
2. El concepto aparecerá en la tabla
3. Afectará las próximas nóminas

### 📋 Tabla de Conceptos Asignados

| Concepto | Tipo | Monto | Frecuencia | Fecha Inicio | Fecha Fin | Acciones |
|---|---|---|---|---|---|---|
| Bonificación Desempeño | Bonificación | C$ 2,000 | Mensual | 01/01/2025 | --- | 🖊️ 🗑️ |
| Deducción CAJA | Deducción | 5% | Nómina | 01/01/2025 | --- | 🖊️ 🗑️ |

---

### 📸 Captura de Pantalla - Asignación de Conceptos

```
┌──────────────────────────────────────────────────────────────────────┐
│  🎁 Asignación de Conceptos - Empleado: Juan Pérez                   │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de conceptos asignados]                            │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre un empleado → pestaña "Conceptos Asignados"                │
│  2. Captura mostrando:                                               │
│     ✓ Tabla de conceptos actuales                                    │
│     ✓ Botón: ➕ Agregar Concepto                                    │
│     ✓ 3-5 conceptos de ejemplo                                       │
│     ✓ Columnas: Concepto, Tipo, Monto, Frecuencia                  │
│     ✓ Acciones: Editar, Eliminar                                    │
│  3. Guarda como: "conceptos-asignados.png"                          │
│  4. Reemplaza este bloque con:                                      │
│     ![Conceptos Asignados](./images/conceptos-asignados.png)        │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Conceptos Asignados](./images/conceptos-asignados.png)
> ```

---

## 🖥️ SECCIÓN 7: TURNOS Y HORARIOS

### Propósito
Asignar y gestionar turnos de trabajo para cada empleado.

### Acceso

1. Ve a 👥 Empleados → Listado de Empleados
2. Abre un empleado (👁️ Ver detalle)
3. En la pestaña **"Turnos"**, visualiza/edita turnos
4. O accede: `https://tu-dominio.com/employees/{ID}/shifts`

### Tipos de Turnos

| Turno | Horario | Descripción |
|---|---|---|
| **Diurno** | 8:00 - 17:00 | Jornada de día estándar |
| **Nocturno** | 18:00 - 3:00 | Jornada de noche |
| **Madrugada** | 4:00 - 12:00 | Turno de madrugada |
| **Flexible** | Variable | Horario flexible |
| **Fin de Semana** | 8:00 - 17:00 | Jornada en fin de semana |

### Asignar Turno

1. En la tabla de turnos, haz clic en **➕ Asignar Turno**
2. Completa:
   - **Turno**: Seleccionar de lista
   - **Fecha Inicio**: Cuándo aplica
   - **Fecha Fin**: Hasta cuándo (indefinido si no aplica)
   - **Lunes a Viernes**: Horario de lunes a viernes
   - **Sábados**: Horario sabatino (si aplica)
   - **Domingos**: Horario dominical (si aplica)
3. Haz clic en **💾 Guardar Turno**

### 📋 Tabla de Turnos Asignados

| Turno | Horario | Fecha Inicio | Fecha Fin | Estado | Acciones |
|---|---|---|---|---|---|
| Diurno | 8:00 - 17:00 | 01/01/2025 | --- | ✅ Vigente | 🖊️ 🗑️ |
| Nocturno | 18:00 - 3:00 | 15/02/2025 | 28/02/2025 | ⏳ Programado | 🖊️ 🗑️ |

---

### 📸 Captura de Pantalla - Turnos y Horarios

```
┌──────────────────────────────────────────────────────────────────────┐
│  ⏰ Turnos y Horarios - Empleado: Juan Pérez                         │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  [Aquí va imagen de turnos]                                         │
│                                                                       │
│  INSTRUCCIONES PARA INSERTAR IMAGEN:                               │
│  1. Abre un empleado → pestaña "Turnos"                             │
│  2. Captura mostrando:                                               │
│     ✓ Tabla de turnos asignados                                      │
│     ✓ Botón: ➕ Asignar Turno                                      │
│     ✓ Información de turnos vigentes                                 │
│     ✓ Estados: Vigente, Programado                                  │
│     ✓ Acciones: Editar, Eliminar                                    │
│  3. Guarda como: "turnos-horarios.png"                              │
│  4. Reemplaza este bloque con:                                      │
│     ![Turnos y Horarios](./images/turnos-horarios.png)              │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

> **Insertar Imagen**: Reemplaza el bloque anterior con:
> ```markdown
> ![Turnos y Horarios](./images/turnos-horarios.png)
> ```

---

## 🎯 PROCEDIMIENTOS COMUNES

### Procedimiento 1: Registrar Nuevo Empleado Completo

**Objetivo:** Incorporar un empleado nuevo al sistema

**Pasos:**
1. Ve a 👥 Empleados → Listado de Empleados
2. Haz clic en **➕ Nuevo Empleado**
3. **Paso 1**: Completa información personal
   - Nombres y apellidos
   - Cédula/Pasaporte
   - Email y teléfono
   - Fecha nacimiento y género
4. **Paso 2**: Datos fiscales
   - Número RUC
   - Número INSS
5. **Paso 3**: Información laboral
   - Sucursal de trabajo
   - Puesto/cargo
   - Tipo de contrato
   - Salario base
   - Frecuencia de pago
   - Fecha de ingreso
6. **Paso 4**: Datos adicionales
   - Marcar "En período de prueba" si aplica
   - Duración de prueba (típicamente 90 días)
7. **Paso 5**: Revisar resumen
   - Verifica todos los datos
   - Haz clic en **Crear Empleado**
8. **Post-creación:**
   - Asigna conceptos de nómina (bonificaciones, deducciones)
   - Asigna turnos de trabajo
   - Asigna al equipo/departamento

---

### Procedimiento 2: Cambiar Estado de Empleado (Suspensión/Reactivación)

**Objetivo:** Suspender temporalmente o reactivar un empleado

**Pasos:**
1. Ve a 👥 Empleados → Acciones de Personal
2. Haz clic en **➕ Nueva Acción**
3. Selecciona el **Empleado**
4. En "Tipo de Acción", selecciona **"Cambio de Estado"**
5. Completa:
   - **Estado anterior**: [Se llena automáticamente]
   - **Estado nuevo**: Selecciona "Suspendido" o "Activo"
   - **Motivo**: "Licencia sin pago", "Suspensión disciplinaria", etc.
   - **Fecha efectiva**: Cuándo aplica el cambio
6. Haz clic en **💾 Registrar Acción**
7. El cambio de estado aparecerá en:
   - Historial de Acciones de Personal
   - Perfil del empleado (chip de estado)
   - Cálculos de nómina futuros

---

### Procedimiento 3: Registrar Promoción (Cambio de Puesto y Salario)

**Objetivo:** Ascender a un empleado a un puesto superior

**Pasos:**
1. Ve a 👥 Empleados → Acciones de Personal
2. Haz clic en **➕ Nueva Acción**
3. Selecciona el **Empleado**
4. En "Tipo de Acción", selecciona **"Promoción"**
5. Completa:
   - **Puesto anterior**: [Se llena automáticamente]
   - **Puesto nuevo**: Selecciona nuevo cargo
   - **Salario anterior**: [Se llena automáticamente]
   - **Nuevo salario**: Ingresa monto nuevo
   - **Fecha de promoción**: Cuándo es efectiva
6. Haz clic en **💾 Registrar Acción**
7. **Verificación:**
   - El nuevo salario aparecerá en próximas nóminas
   - El puesto se actualizará en el perfil
   - Se registrará en historial de cambios

---

### Procedimiento 4: Asignar Bonificación Especial

**Objetivo:** Agregar bonificación puntual a un empleado

**Pasos:**
1. Ve a 👥 Empleados → Listado de Empleados
2. Busca el empleado
3. Haz clic en **👁️ Ver detalle**
4. Ve a la pestaña **"Conceptos Asignados"**
5. Haz clic en **➕ Agregar Concepto**
6. Completa:
   - **Concepto**: Selecciona "Bonificación Desempeño" o similar
   - **Tipo**: "Bonificación"
   - **Monto**: Cantidad a bonificar (ej: C$ 5,000)
   - **Frecuencia**: "Única" o "Mensual"
   - **Fecha Inicio**: Cuándo se agrega
   - **Fecha Fin**: Cuándo termina (si es puntual, poner fin próximo al inicio)
   - **Motivo**: "Desempeño excepcional", "Productividad", etc.
7. Haz clic en **💾 Guardar Concepto**
8. La bonificación se incluirá en la próxima nómina

---

### Procedimiento 5: Generar Reporte de Empleados

**Objetivo:** Exportar listado de empleados a Excel

**Pasos:**
1. Ve a 👥 Empleados → Listado de Empleados
2. (Opcional) Aplica filtros:
   - Selecciona sucursal, estado, puesto, etc.
3. Haz clic en **📤 Exportar Listado (Excel)**
4. El navegador descargará archivo `Empleados_[fecha].xlsx`
5. **Contenido:**
   - Código, nombre, cédula
   - Email, teléfono
   - Sucursal, puesto, estado
   - Salario, fecha ingreso
   - Período prueba, estado laboral

---

## ⚠️ GUÍA DE TROUBLESHOOTING

### Problema 1: No puedo crear un empleado - Error de validación

**Síntoma:** Al intentar crear empleado, aparece error rojo en campos

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Campos requeridos vacíos | Completa todos los campos marcados con * |
| Formato cédula incorrecto | Debe ser 14 dígitos o formato pasaporte válido |
| Email ya existe | Usa otro email o verifica que no esté registrado |
| RUC o INSS duplicado | No puede haber dos empleados con mismo número |
| Fecha futura | Fecha ingreso no puede ser posterior a hoy |

---

### Problema 2: El salario no aparece en la nómina

**Síntoma:** Al calcular nómina, el empleado no tiene monto de salario

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Empleado en estado "Terminado" | Cambia estado a "Activo" |
| Sin puesto asignado | Asigna un puesto válido |
| Período fuera de rango laboral | Verifica fecha ingreso sea anterior a período nómina |
| Frecuencia pago no coincide | Asigna frecuencia que coincida con período nómina |

---

### Problema 3: No veo el empleado en los filtros

**Síntoma:** Aplico filtros pero no aparece el empleado buscado

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Empleado desactivado | Busca incluyendo "Terminados" |
| Sucursal filtrada diferente | Cambia filtro de sucursal a "Todas" |
| Nombre escrito diferente | Busca por cédula o código en lugar de nombre |
| Término búsqueda incompleto | Prueba con término más corto o general |

---

### Problema 4: Error al editar: "Cambio de estado inválido"

**Síntoma:** No puedo cambiar el estado laboral del empleado

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Estado ya es el solicitado | Selecciona estado diferente al actual |
| Período nómina abierto | Cierra/finaliza períodos antes de cambio |
| Nómina sin procesar | Procesa nómina antes de cambiar estado |
| Permiso insuficiente | Contacta a administrador |

---

### Problema 5: Concepto no aplica a la nómina

**Síntoma:** Asigno bonificación pero no aparece en cálculo de nómina

**Causas y Soluciones:**

| Causa | Solución |
|---|---|
| Fecha inicio posterior a nómina | Asigna fecha anterior o igual a período nómina |
| Frecuencia incorrecta | Si es mensual, aplica a todas; si "Única", solo una |
| Concepto desactivado en sistema | Verifica que concepto esté activo en configuración |
| Estado empleado "Suspendido" | Reactiva empleado; suspensión evita cálculos |

---

## 🔗 INTEGRACIÓN CON OTROS MÓDULOS

### Módulo de Nómina
- **Relación:** Los empleados son base para cálculo de nómina
- **Flujo:** Crear Empleado → Asignar Conceptos → Calcular Nómina
- **Sincronización:** Cambios de salario afectan nómina inmediatamente

### Módulo de Gestión de Empleadores
- **Relación:** Empleados se asignan a sucursales registradas
- **Flujo:** Crear Sucursal → Crear Empleado en Sucursal
- **Sincronización:** Estado de sucursal (activa/inactiva) afecta empleados

### Módulo de Reportes
- **Relación:** Datos de empleados alimentan reportes de nómina
- **Flujo:** Empleado → Nómina → Colilla (PDF)
- **Sincronización:** Logo empresa, datos empleado aparecen en colilla

### Módulo de Control de Asistencia (Futuro)
- **Relación:** Turnos y asistencia de empleados
- **Flujo:** Asignar Turno → Registrar Asistencia
- **Sincronización:** Faltas/retardos afectan cálculo de nómina

---

## 📞 SOPORTE Y CONTACTO

### Contacto Técnico
- **Email**: soporte@gestiona360.com
- **Teléfono**: +505 XXXX-XXXX
- **Horario**: Lunes a Viernes, 8:00 - 17:00

### Escalamiento
Para problemas no resueltos:
1. Documenta pantalla (captura + mensaje exacto)
2. Anota: hora, usuario, acción realizada
3. Contacta soporte con esta información

---

## 🎓 RECURSOS ADICIONALES

- 📖 Manual Sistema Completo: [enlace]
- 🎥 Video Tutorial Crear Empleado: [enlace]
- 📊 Plantilla de importación masiva: [enlace]
- 🔐 Políticas de privacidad de datos: [enlace]

---

**Versión:** 1.0  
**Última actualización:** 2025  
**Próxima revisión:** Q2 2025

