namespace MatriculasUnisinu.Models.Asignaturas;

// Datos de entrada para registrar una asignatura
public record CrearAsignaturaRequest(string? Nombre, string? Codigo, int Creditos, bool Activa = true);
