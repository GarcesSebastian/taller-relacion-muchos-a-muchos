# Sistema de Gestión de Matrículas Académicas — Unisinú

API en **C# .NET 8 (ASP.NET Core MVC)** que modela una relación **Muchos a Muchos (N:M)**
entre `Estudiante` y `Asignatura` mediante la entidad intermedia `Matrícula`. Implementa
borrado lógico (Soft Delete), validación de asignaturas activas y control de duplicados por
año y periodo académico.

- **Framework:** ASP.NET Core Web API (.NET 8), arquitectura MVC con controladores
- **URL base local:** `http://localhost:5000`
- **Swagger UI:** `http://localhost:5000/swagger`

> Electiva Disciplinar IV · Docente: Ing. Ricardo Vanegas · Universidad del Sinú

---

## 1. Estructura del proyecto

```
Taller Relacion Muchos a Muchos/
├─ Controllers/
│  ├─ AsignaturasController.cs   # GET activas, POST, DELETE (soft/físico)
│  ├─ MatriculasController.cs    # POST con reglas de negocio
│  └─ EstudiantesController.cs   # GET lista y GET {id}/asignaturas
├─ Models/
│  ├─ Asignaturas/
│  │  ├─ Asignatura.cs           # record (incluye Activa para Soft Delete)
│  │  └─ CrearAsignaturaRequest.cs
│  ├─ Estudiantes/
│  │  └─ Estudiante.cs           # record
│  ├─ Matriculas/
│  │  ├─ Matricula.cs            # record (Año y Periodo)
│  │  └─ CrearMatriculaRequest.cs
│  └─ Comun/
│     └─ ErrorRespuesta.cs       # DTO de error estándar
├─ Data/
│  └─ AlmacenEnMemoria.cs        # Colecciones en memoria compartidas (simula la BD)
├─ Properties/
│  └─ launchSettings.json        # Perfil HTTP en puerto 5000
├─ Program.cs                    # Configuración MVC (AddControllers / MapControllers) + Swagger
├─ MatriculasUnisinu.csproj
└─ README.md
```

---

## 2. Modelo de datos (records)

```csharp
record Estudiante(int Id, string Nombre, string Carrera);
record Asignatura(int Id, string Nombre, string Codigo, int Creditos, bool Activa = true);
record Matricula(int Id, int EstudianteId, int AsignaturaId, int Anio = 2026, string Periodo = "2026-1");
```

Datos precargados en memoria (`Data/AlmacenEnMemoria.cs`):

- **Estudiantes:** `1` Ana Pérez, `2` Carlos Gómez, `3` María Rodríguez
- **Asignaturas:** `101` Estructuras de Datos, `102` Programación Web, `103` Bases de Datos I
- **Matrículas:** vacío al iniciar

---

## 3. Reglas de negocio implementadas

| Regla | Descripción |
|-------|-------------|
| **1. Borrado lógico** | `DELETE /api/asignaturas/{id}`: si la asignatura **no** tiene matrículas, se elimina físicamente. Si **sí** tiene matrículas, se cambia `Activa = false` para preservar el historial. |
| **2. Validaciones de matrícula** | `POST /api/matriculas`: valida que el estudiante exista, que la asignatura exista y esté `Activa == true` (si está inactiva → **400**), y controla duplicados por año + periodo. |
| **3. Oferta académica** | `GET /api/asignaturas` solo devuelve las asignaturas con `Activa == true`. |

---

## 4. Endpoints

| Método | Ruta | Descripción | Códigos |
|--------|------|-------------|---------|
| GET | `/api/asignaturas` | Lista solo asignaturas activas | 200 |
| POST | `/api/asignaturas` | Registra una asignatura | 201 / 400 / 409 |
| DELETE | `/api/asignaturas/{id}` | Elimina física o lógicamente | 204 / 200 / 404 |
| POST | `/api/matriculas` | Matricula estudiante (año + periodo) | 201 / 400 / 404 / 409 |
| GET | `/api/estudiantes/{id}/asignaturas` | Historial de un estudiante | 200 / 404 |
| GET | `/api/estudiantes` | Lista de estudiantes (apoyo pruebas) | 200 |

---

## 5. Cómo ejecutar en local

Requiere el **SDK de .NET 8**. Desde la carpeta del proyecto:

```powershell
dotnet run
```

Luego abre `http://localhost:5000/swagger`.

---

## 6. Pruebas rápidas (flujo sugerido en Swagger)

1. **GET `/api/asignaturas`** → devuelve las 3 asignaturas activas (200).
2. **POST `/api/matriculas`** con:
   ```json
   { "estudianteId": 1, "asignaturaId": 101, "anio": 2026, "periodo": "2026-1" }
   ```
   → 201 Created.
3. **POST `/api/matriculas`** repitiendo el mismo cuerpo → 409 Conflict (duplicado por periodo).
4. **POST `/api/matriculas`** con `estudianteId: 99` → 404 Not Found.
5. **DELETE `/api/asignaturas/101`** (ya tiene matrícula) → 200 con borrado lógico (`activa: false`).
6. **GET `/api/asignaturas`** → la 101 ya no aparece (quedó inactiva).
7. **POST `/api/matriculas`** con `asignaturaId: 101` → 400 Bad Request (asignatura inactiva).
8. **DELETE `/api/asignaturas/102`** (sin matrículas) → 204 No Content (borrado físico).
9. **GET `/api/estudiantes/1/asignaturas`** → historial del estudiante 1 (200).

---

## 7. Despliegue en Replit

Comprime la carpeta del proyecto en un `.zip` y súbela a un Repl de Replit. Asegúrate de
que el entorno del Repl tenga el **SDK de .NET 8** y ejecútalo con `dotnet run`. Abre la URL
pública añadiendo `/swagger` para probar la API.
