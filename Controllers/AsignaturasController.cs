using MatriculasUnisinu.Data;
using MatriculasUnisinu.Models.Asignaturas;
using MatriculasUnisinu.Models.Comun;
using Microsoft.AspNetCore.Mvc;

namespace MatriculasUnisinu.Controllers;

[ApiController]
[Route("api/asignaturas")]
public class AsignaturasController : ControllerBase
{
    // GET /api/asignaturas
    // Regla 3: solo devuelve asignaturas activas (Activa == true).
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Asignatura>), StatusCodes.Status200OK)]
    public IActionResult ObtenerActivas()
    {
        var ofertaActiva = AlmacenEnMemoria.Asignaturas.Where(a => a.Activa).ToList();
        return Ok(ofertaActiva);
    }

    // POST /api/asignaturas
    // Registrar una nueva asignatura académica.
    [HttpPost]
    [ProducesResponseType(typeof(Asignatura), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status409Conflict)]
    public IActionResult Crear([FromBody] CrearAsignaturaRequest request)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.Nombre) ||
            string.IsNullOrWhiteSpace(request.Codigo))
        {
            return BadRequest(new ErrorRespuesta
            {
                Codigo = 400,
                Estado = "Bad Request",
                Mensaje = "Los campos 'nombre' y 'codigo' son obligatorios y no pueden estar vacíos."
            });
        }

        if (request.Creditos <= 0)
        {
            return BadRequest(new ErrorRespuesta
            {
                Codigo = 400,
                Estado = "Bad Request",
                Mensaje = "El campo 'creditos' debe ser un número mayor que cero."
            });
        }

        var existeCodigo = AlmacenEnMemoria.Asignaturas
            .Any(a => string.Equals(a.Codigo, request.Codigo, StringComparison.OrdinalIgnoreCase));
        if (existeCodigo)
        {
            return Conflict(new ErrorRespuesta
            {
                Codigo = 409,
                Estado = "Conflict",
                Mensaje = $"Ya existe una asignatura con el código '{request.Codigo}'."
            });
        }

        var nuevoId = AlmacenEnMemoria.Asignaturas.Count == 0
            ? 101
            : AlmacenEnMemoria.Asignaturas.Max(a => a.Id) + 1;

        var nueva = new Asignatura(nuevoId, request.Nombre!, request.Codigo!, request.Creditos, request.Activa);
        AlmacenEnMemoria.Asignaturas.Add(nueva);

        return CreatedAtAction(nameof(ObtenerActivas), new { id = nueva.Id }, nueva);
    }

    // DELETE /api/asignaturas/{id}
    // Regla 1: borrado físico si no tiene matrículas; borrado lógico (Activa = false) si las tiene.
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorRespuesta), StatusCodes.Status404NotFound)]
    public IActionResult Eliminar(int id)
    {
        var asignatura = AlmacenEnMemoria.Asignaturas.FirstOrDefault(a => a.Id == id);
        if (asignatura is null)
        {
            return NotFound(new ErrorRespuesta
            {
                Codigo = 404,
                Estado = "Not Found",
                Mensaje = $"No existe una asignatura con id {id}."
            });
        }

        var tieneMatriculas = AlmacenEnMemoria.Matriculas.Any(m => m.AsignaturaId == id);

        if (!tieneMatriculas)
        {
            // Sin matrículas asociadas: eliminación física.
            AlmacenEnMemoria.Asignaturas.Remove(asignatura);
            return NoContent();
        }

        // Con estudiantes matriculados: borrado lógico para preservar el historial académico.
        var indice = AlmacenEnMemoria.Asignaturas.IndexOf(asignatura);
        AlmacenEnMemoria.Asignaturas[indice] = asignatura with { Activa = false };

        return Ok(new
        {
            mensaje = "La asignatura tiene matrículas asociadas. Se aplicó borrado lógico (Activa = false) " +
                      "para preservar el historial académico.",
            asignatura = AlmacenEnMemoria.Asignaturas[indice]
        });
    }
}
