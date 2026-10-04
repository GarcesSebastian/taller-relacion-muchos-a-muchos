using MatriculasUnisinu.Data;
using MatriculasUnisinu.Models.Comun;
using MatriculasUnisinu.Models.Matriculas;
using Microsoft.AspNetCore.Mvc;

namespace MatriculasUnisinu.Controllers;

[ApiController]
[Route("api/matriculas")]
public class MatriculasController : ControllerBase
{
    // POST /api/matriculas
    // Regla 2: valida estudiante existente, asignatura existente y activa, y control de duplicados por periodo.
    [HttpPost]
    [ProducesResponseType(typeof(Matricula), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status409Conflict)]
    public IActionResult Crear([FromBody] CrearMatriculaRequest request)
    {
        if (request is null)
        {
            return BadRequest(new ErrorRespuesta
            {
                Codigo = 400,
                Estado = "Bad Request",
                Mensaje = "Cuerpo de la solicitud requerido."
            });
        }

        // Verificar la existencia del estudiante.
        var estudiante = AlmacenEnMemoria.Estudiantes.FirstOrDefault(e => e.Id == request.EstudianteId);
        if (estudiante is null)
        {
            return NotFound(new ErrorRespuesta
            {
                Codigo = 404,
                Estado = "Not Found",
                Mensaje = $"No existe un estudiante con id {request.EstudianteId}."
            });
        }

        // Validar que la asignatura exista.
        var asignatura = AlmacenEnMemoria.Asignaturas.FirstOrDefault(a => a.Id == request.AsignaturaId);
        if (asignatura is null)
        {
            return NotFound(new ErrorRespuesta
            {
                Codigo = 404,
                Estado = "Not Found",
                Mensaje = $"No existe una asignatura con id {request.AsignaturaId}."
            });
        }

        // Validar que la asignatura esté activa.
        if (!asignatura.Activa)
        {
            return BadRequest(new ErrorRespuesta
            {
                Codigo = 400,
                Estado = "Bad Request",
                Mensaje = $"La asignatura '{asignatura.Nombre}' está inactiva y no admite nuevas matrículas."
            });
        }

        // Validar periodo.
        if (string.IsNullOrWhiteSpace(request.Periodo))
        {
            return BadRequest(new ErrorRespuesta
            {
                Codigo = 400,
                Estado = "Bad Request",
                Mensaje = "El campo 'periodo' es obligatorio (por ejemplo: 2026-1)."
            });
        }

        // Control de duplicados por año y periodo lectivo.
        var duplicada = AlmacenEnMemoria.Matriculas.Any(m =>
            m.EstudianteId == request.EstudianteId &&
            m.AsignaturaId == request.AsignaturaId &&
            m.Anio == request.Anio &&
            string.Equals(m.Periodo, request.Periodo, StringComparison.OrdinalIgnoreCase));

        if (duplicada)
        {
            return Conflict(new ErrorRespuesta
            {
                Codigo = 409,
                Estado = "Conflict",
                Mensaje = $"El estudiante {estudiante.Nombre} ya está matriculado en '{asignatura.Nombre}' " +
                          $"para el periodo {request.Anio}-{request.Periodo}."
            });
        }

        var nuevoId = AlmacenEnMemoria.Matriculas.Count == 0
            ? 1
            : AlmacenEnMemoria.Matriculas.Max(m => m.Id) + 1;

        var nueva = new Matricula(nuevoId, request.EstudianteId, request.AsignaturaId, request.Anio, request.Periodo);
        AlmacenEnMemoria.Matriculas.Add(nueva);

        return StatusCode(StatusCodes.Status201Created, nueva);
    }
}
