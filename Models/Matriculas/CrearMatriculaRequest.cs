namespace MatriculasUnisinu.Models.Matriculas;

// Datos de entrada para registrar una matrícula
public record CrearMatriculaRequest(int EstudianteId, int AsignaturaId, int Anio = 2026, string Periodo = "2026-1");
