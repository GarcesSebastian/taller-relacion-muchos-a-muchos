namespace MatriculasUnisinu.Models.Asignaturas;

// Modelo de Asignatura (incluye estado para Borrado Lógico)
public record Asignatura(int Id, string Nombre, string Codigo, int Creditos, bool Activa = true);
