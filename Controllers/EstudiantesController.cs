using MatriculasUnisinu.Data;
using MatriculasUnisinu.Models.Comun;
using MatriculasUnisinu.Models.Estudiantes;
using Microsoft.AspNetCore.Mvc;

namespace MatriculasUnisinu.Controllers;

[ApiController]
[Route("api/estudiantes")]
public class EstudiantesController : ControllerBase
{
    // GET /api/estudiantes  (listado de apoyo para pruebas)
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Estudiante>), StatusCodes.Status200OK)]
    public IActionResult ObtenerTodos()
    {
        return Ok(AlmacenEnMemoria.Estudiantes);
    }

    // GET /api/estudiantes/{id}/asignaturas
    // Consultar el historial de asignaturas matriculadas por un estudiante específico.
    [HttpGet("{id:int}/asignaturas")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status404NotFound)]
    public IActionResult ObtenerHistorial(int id)
    {
        var estudiante = AlmacenEnMemoria.Estudiantes.FirstOrDefault(e => e.Id == id);
        if (estudiante is null)
        {
            return NotFound(new ErrorRespuesta
            {
                Codigo = 404,
                Estado = "Not Found",
                Mensaje = $"No existe un estudiante con id {id}."
            });
        }

        var historial = AlmacenEnMemoria.Matriculas
            .Where(m => m.EstudianteId == id)
            .Select(m =>
            {
                var asignatura = AlmacenEnMemoria.Asignaturas.FirstOrDefault(a => a.Id == m.AsignaturaId);
                return new
                {
                    matriculaId = m.Id,
                    asignaturaId = m.AsignaturaId,
                    nombre = asignatura?.Nombre,
                    codigo = asignatura?.Codigo,
                    creditos = asignatura?.Creditos,
                    activa = asignatura?.Activa,
                    anio = m.Anio,
                    periodo = m.Periodo
                };
            })
            .ToList();

        return Ok(new
        {
            estudiante = new { estudiante.Id, estudiante.Nombre, estudiante.Carrera },
            totalAsignaturas = historial.Count,
            asignaturas = historial
        });
    }
}
