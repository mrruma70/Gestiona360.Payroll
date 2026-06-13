# 📝 EJEMPLOS DE CURL: Testing JWT API
## Gestiona360.Payroll | Pruebas de Endpoints

---

## 🔧 Setup Inicial

### Variables
```bash
# Configurar variables de entorno
API_URL="https://localhost:7119"
EMAIL="admin@example.com"
PASSWORD="Admin123!@"
TENANT_CODE="EMPRESA001"

# Para ignorar certificados SSL auto-firmados en desarrollo
INSECURE="--insecure"
```

---

## 🔑 1. AUTENTICACIÓN

### 1.1 Login - Obtener Tokens

```bash
curl -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"email": "'$EMAIL'",
	"password": "'$PASSWORD'",
	"tenantCode": "'$TENANT_CODE'"
  }' | jq

# RESPUESTA (200 OK):
# {
#   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
#   "refreshToken": "sxR2w5Q8pK9mL0nJ8hU7yG6tF5rE4qW3zX2cV1bM0aL...",
#   "expiresIn": 900,
#   "tokenType": "Bearer",
#   "user": {
#     "id": "550e8400-e29b-41d4-a716-446655440000",
#     "email": "admin@example.com",
#     "firstName": "Admin",
#     "lastName": "User",
#     "fullName": "Admin User",
#     "roles": ["Admin"],
#     "isActive": true,
#     "createdAt": "2025-01-01T00:00:00"
#   }
# }

# GUARDAR TOKENS
ACCESS_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
REFRESH_TOKEN="sxR2w5Q8pK9mL0nJ8hU7yG6tF5rE4qW3zX2cV1bM0aL..."
```

### 1.2 Login Fallido - Credenciales Inválidas

```bash
# ❌ Credenciales incorrectas
curl -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"email": "admin@example.com",
	"password": "PasswordIncorrecto123",
	"tenantCode": null
  }' | jq

# RESPUESTA (401 Unauthorized):
# {
#   "message": "Credenciales inválidas"
# }
```

### 1.3 Login - Email Inválido

```bash
# ❌ Email vacío
curl -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"email": "",
	"password": "Admin123!@",
	"tenantCode": null
  }' | jq

# RESPUESTA (400 Bad Request):
# {
#   "email": ["El correo es obligatorio"]
# }
```

---

## 🔄 2. REFRESH TOKEN

### 2.1 Renovar Access Token

```bash
# Usar refresh token para obtener nuevo access token
curl -X POST "$API_URL/api/auth/refresh" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"refreshToken": "'$REFRESH_TOKEN'"
  }' | jq

# RESPUESTA (200 OK):
# {
#   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
#   "refreshToken": "nU8yH7gF6tE5rD4qC3pB2aM1zX0wV9uT8sR7qP6oN...",
#   "expiresIn": 900,
#   "tokenType": "Bearer",
#   "user": {...}
# }

# Actualizar variable
ACCESS_TOKEN="nuevo_token_aqui"
```

### 2.2 Refresh con Token Inválido

```bash
# ❌ Token expirado o inválido
curl -X POST "$API_URL/api/auth/refresh" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"refreshToken": "refresh_token_invalido_o_expirado"
  }' | jq

# RESPUESTA (401 Unauthorized):
# {
#   "message": "Refresh token inválido"
# }
```

---

## 👤 3. INFORMACIÓN DEL USUARIO

### 3.1 Obtener Usuario Actual (Autenticado)

```bash
curl -X GET "$API_URL/api/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# {
#   "id": "550e8400-e29b-41d4-a716-446655440000",
#   "email": "admin@example.com",
#   "firstName": "Admin",
#   "lastName": "User",
#   "fullName": "Admin User",
#   "roles": ["Admin"],
#   "isActive": true,
#   "createdAt": "2025-01-01T00:00:00"
# }
```

### 3.2 Sin Token - Error 401

```bash
# ❌ Sin header Authorization
curl -X GET "$API_URL/api/auth/me" \
  $INSECURE | jq

# RESPUESTA (401 Unauthorized):
# {
#   "message": "Unauthorized"
# }
```

### 3.3 Con Token Expirado

```bash
# ❌ Token expirado
curl -X GET "$API_URL/api/auth/me" \
  -H "Authorization: Bearer eyJ...[token_expirado]" \
  $INSECURE | jq

# RESPUESTA (401 Unauthorized):
# {
#   "message": "Token expired"
# }

# Solución: Usar endpoint /refresh para renovar
```

---

## 🚪 4. LOGOUT

### 4.1 Cerrar Sesión

```bash
curl -X POST "$API_URL/api/auth/logout" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE

# RESPUESTA (204 No Content):
# (sin body)

# Después de logout:
# - El refresh token es revocado en BD
# - El token anterior ya no es válido
# - Usuario debe hacer login nuevamente
```

### 4.2 Logout Sin Autenticación (debería permitirse)

```bash
curl -X POST "$API_URL/api/auth/logout" \
  $INSECURE

# RESPUESTA (204 No Content):
# (sin body, es idempotente)
```

---

## 📊 5. ENDPOINTS PROTEGIDOS (CRUD)

### 5.1 Obtener Todos los Empleados

```bash
curl -X GET "$API_URL/api/employees" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# [
#   {
#     "id": "...",
#     "code": "EMP001",
#     "firstName": "John",
#     "lastName": "Doe",
#     "email": "john@example.com",
#     "isActive": true
#   },
#   ...
# ]

# ❌ SIN TOKEN:
# (401 Unauthorized)

# ❌ SIN ROL REQUERIDO (si es admin-only):
# (403 Forbidden)
```

### 5.2 Obtener Empleado Específico

```bash
EMPLOYEE_ID="550e8400-e29b-41d4-a716-446655440000"

curl -X GET "$API_URL/api/employees/$EMPLOYEE_ID" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# {
#   "id": "550e8400-e29b-41d4-a716-446655440000",
#   "code": "EMP001",
#   "firstName": "John",
#   "lastName": "Doe",
#   "email": "john@example.com",
#   ...
# }

# ❌ NO ENCONTRADO (404 Not Found):
# Si el ID no existe
```

### 5.3 Crear Empleado (Requiere Rol Admin/HR)

```bash
curl -X POST "$API_URL/api/employees" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"code": "EMP999",
	"firstName": "Jane",
	"lastName": "Smith",
	"email": "jane@example.com",
	"identificationNumber": "123456789",
	"birthDate": "1990-01-15",
	"salary": 50000
  }' | jq

# RESPUESTA (201 Created):
# {
#   "id": "550e8400-e29b-41d4-a716-446655440001",
#   "code": "EMP999",
#   "firstName": "Jane",
#   "lastName": "Smith",
#   ...
# }

# ❌ SIN ROL ADMIN/HR (403 Forbidden):
# Si el usuario autenticado no tiene rol Admin/HR
```

### 5.4 Actualizar Empleado

```bash
EMPLOYEE_ID="550e8400-e29b-41d4-a716-446655440001"

curl -X PUT "$API_URL/api/employees/$EMPLOYEE_ID" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"firstName": "Jane",
	"lastName": "Doe",
	"email": "jane.doe@example.com",
	"salary": 55000
  }' | jq

# RESPUESTA (200 OK):
# {
#   "id": "550e8400-e29b-41d4-a716-446655440001",
#   "firstName": "Jane",
#   "lastName": "Doe",
#   "email": "jane.doe@example.com",
#   "salary": 55000,
#   ...
# }
```

### 5.5 Eliminar Empleado (Solo Admin)

```bash
EMPLOYEE_ID="550e8400-e29b-41d4-a716-446655440001"

curl -X DELETE "$API_URL/api/employees/$EMPLOYEE_ID" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE

# RESPUESTA (204 No Content):
# (sin body)

# ❌ SIN ROL ADMIN (403 Forbidden):
# Si el usuario no tiene rol Admin
```

---

## 🏢 6. OTROS ENDPOINTS

### 6.1 Obtener Empresa Principal

```bash
curl -X GET "$API_URL/api/companies/main" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# {
#   "id": "...",
#   "name": "Mi Empresa",
#   "identificationNumber": "1234567-8",
#   "email": "info@empresa.com",
#   ...
# }
```

### 6.2 Obtener Bancos

```bash
curl -X GET "$API_URL/api/banks" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# [
#   {
#     "id": 1,
#     "name": "Banco Nacional",
#     "code": "BN",
#     "isActive": true
#   },
#   ...
# ]
```

### 6.3 Obtener Turnos

```bash
curl -X GET "$API_URL/api/shifts" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# [
#   {
#     "id": "...",
#     "name": "Turno Matutino",
#     "startTime": "06:00:00",
#     "endTime": "14:00:00",
#     "isActive": true
#   },
#   ...
# ]
```

---

## 📋 7. REPORTES

### 7.1 Generar Reporte de IR

```bash
curl -X POST "$API_URL/api/reports/DGI_IR_RETENTION_MONTHLY?format=Excel" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"Year": 2025,
	"Month": 1
  }' \
  -o "IR_2025_01.xlsx"

# Descarga archivo Excel

# ❌ SIN FORMATO VÁLIDO:
# curl -X POST "$API_URL/api/reports/DGI_IR_RETENTION_MONTHLY?format=InvalidFormat" \
# (400 Bad Request)
```

### 7.2 Listar Reportes Disponibles

```bash
curl -X GET "$API_URL/api/reports/available" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# RESPUESTA (200 OK):
# [
#   {
#     "code": "DGI_IR_RETENTION_MONTHLY",
#     "name": "Retenciones IR Mensual",
#     "description": "Reporte mensual de retenciones de IR",
#     "defaultFormat": "Excel",
#     "parameters": [
#       {
#         "name": "Year",
#         "type": "int",
#         "label": "Año",
#         "isRequired": true,
#         "defaultValue": "2025"
#       },
#       ...
#     ]
#   },
#   ...
# ]
```

---

## 🔍 8. ERRORES Y SOLUCIONES

### 8.1 401 Unauthorized

```bash
# Problema: Sin token o token inválido
curl -X GET "$API_URL/api/employees" \
  $INSECURE | jq

# {"message":"Unauthorized"}

# ✅ Solución: Agregar token válido
curl -X GET "$API_URL/api/employees" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq
```

### 8.2 403 Forbidden

```bash
# Problema: Usuario autenticado pero sin rol requerido
curl -X POST "$API_URL/api/employees" \
  -H "Authorization: Bearer $ACCESS_TOKEN_SIN_ADMIN" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{"firstName":"John","lastName":"Doe","email":"john@example.com"}'

# {"message":"Forbidden"}

# ✅ Solución: Usar usuario con rol Admin/HR
```

### 8.3 400 Bad Request

```bash
# Problema: Validación fallida
curl -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"email": "invalid-email",
	"password": "short"
  }' | jq

# {
#   "email": ["El formato del correo es inválido"],
#   "password": ["La contraseña debe tener entre 8 y 100 caracteres"]
# }

# ✅ Solución: Validar datos antes de enviar
```

### 8.4 500 Internal Server Error

```bash
# Problema: Error en el servidor (revisar logs)
curl -X GET "$API_URL/api/employees" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

# {"message":"Error interno del servidor"}

# ✅ Solución: 
# 1. Revisar logs del API en consola
# 2. Verificar BD está conectada
# 3. Contactar administrador
```

---

## 🧪 9. FLUJO COMPLETO DE TESTING

### 9.1 Test de Autenticación Exitosa

```bash
#!/bin/bash

API_URL="https://localhost:7119"
INSECURE="--insecure"

echo "🔐 TEST 1: Login"
LOGIN=$(curl -s -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"email": "admin@example.com",
	"password": "Admin123!@"
  }')

echo "$LOGIN" | jq

ACCESS_TOKEN=$(echo "$LOGIN" | jq -r '.accessToken')
REFRESH_TOKEN=$(echo "$LOGIN" | jq -r '.refreshToken')

echo "✅ Access Token: $ACCESS_TOKEN"
echo "✅ Refresh Token: $REFRESH_TOKEN"

echo -e "\n👤 TEST 2: Get Current User"
curl -s -X GET "$API_URL/api/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq

echo -e "\n📊 TEST 3: Get Employees"
curl -s -X GET "$API_URL/api/employees" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  $INSECURE | jq '.[] | {id, firstName, lastName}' | head -20

echo -e "\n🔄 TEST 4: Refresh Token"
REFRESH=$(curl -s -X POST "$API_URL/api/auth/refresh" \
  -H "Content-Type: application/json" \
  $INSECURE \
  -d '{
	"refreshToken": "'$REFRESH_TOKEN'"
  }')

echo "$REFRESH" | jq

NEW_ACCESS_TOKEN=$(echo "$REFRESH" | jq -r '.accessToken')
echo "✅ New Access Token: $NEW_ACCESS_TOKEN"

echo -e "\n🚪 TEST 5: Logout"
curl -s -X POST "$API_URL/api/auth/logout" \
  -H "Authorization: Bearer $NEW_ACCESS_TOKEN" \
  $INSECURE -w "\nStatus: %{http_code}\n"

echo -e "\n❌ TEST 6: Intenta acceder después de logout (debería fallar si token se revoca)"
curl -s -X GET "$API_URL/api/auth/me" \
  -H "Authorization: Bearer $NEW_ACCESS_TOKEN" \
  $INSECURE | jq

echo -e "\n✅ TODOS LOS TESTS COMPLETADOS"
```

Ejecutar:
```bash
chmod +x test_api.sh
./test_api.sh
```

---

## 🛠️ 10. POSTMAN / THUNDER CLIENT

### 10.1 Colección Postman (JSON)

```json
{
  "info": {
	"name": "Gestiona360 API",
	"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
	{
	  "name": "Auth",
	  "item": [
		{
		  "name": "Login",
		  "request": {
			"method": "POST",
			"header": [{"key": "Content-Type", "value": "application/json"}],
			"body": {
			  "mode": "raw",
			  "raw": "{\"email\":\"admin@example.com\",\"password\":\"Admin123!@\"}"
			},
			"url": {"raw": "{{base_url}}/api/auth/login", "protocol": "https"}
		  }
		},
		{
		  "name": "Refresh Token",
		  "request": {
			"method": "POST",
			"header": [{"key": "Content-Type", "value": "application/json"}],
			"body": {
			  "mode": "raw",
			  "raw": "{\"refreshToken\":\"{{refresh_token}}\"}"
			},
			"url": {"raw": "{{base_url}}/api/auth/refresh"}
		  }
		}
	  ]
	},
	{
	  "name": "Employees",
	  "item": [
		{
		  "name": "Get All",
		  "request": {
			"method": "GET",
			"header": [{"key": "Authorization", "value": "Bearer {{access_token}}"}],
			"url": {"raw": "{{base_url}}/api/employees"}
		  }
		}
	  ]
	}
  ],
  "variable": [
	{"key": "base_url", "value": "https://localhost:7119"},
	{"key": "access_token", "value": ""},
	{"key": "refresh_token", "value": ""}
  ]
}
```

---

## 📞 TROUBLESHOOTING

### P: ¿Cómo obtengo el access token?
**R:** Usa `POST /api/auth/login` con email y contraseña. La respuesta incluirá el `accessToken`.

### P: ¿Cuánto tiempo dura el token?
**R:** Por defecto 15 minutos. Después, usa `POST /api/auth/refresh` con el `refreshToken`.

### P: ¿Qué es el refreshToken?
**R:** Un token de larga duración (7 días) que se usa solo para renovar el accessToken.

### P: ¿Puedo usar un token varias veces?
**R:** Sí, hasta que expire (15 min). Después debe renovarse con el refreshToken.

### P: ¿Qué pasa si expira el refreshToken?
**R:** El usuario debe hacer login nuevamente.

### P: ¿Los tokens son seguros?
**R:** Sí, si usas HTTPS y almacenas en sessionStorage (no localStorage).

---

**Más información en DIAGNOSTICO_SEGURIDAD_COMPLETO.md y GUIA_IMPLEMENTACION_JWT_SWAGGER.md**

