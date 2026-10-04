namespace MatriculasUnisinu.Models.Matriculas;

// Modelo de Matrícula (entidad intermedia con Año y Periodo Académico)
public record Matricula(int Id, int EstudianteId, int AsignaturaId, int Anio = 2026, string Periodo = "2026-1");
