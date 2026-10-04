using MatriculasUnisinu.Models.Asignaturas;
using MatriculasUnisinu.Models.Estudiantes;
using MatriculasUnisinu.Models.Matriculas;

namespace MatriculasUnisinu.Data;

// Almacén de datos en memoria compartido por los controladores.
// Simula la "base de datos" del sistema de matrículas.
public static class AlmacenEnMemoria
{
    // 3. Colecciones de Datos Iniciales en Memoria
    public static readonly List<Estudiante> Estudiantes = new()
    {
        new Estudiante(1, "Ana Pérez", "Ingeniería de Sistemas"),
        new Estudiante(2, "Carlos Gómez", "Ingeniería de Sistemas"),
        new Estudiante(3, "María Rodríguez", "Ingeniería Industrial")
    };

    public static readonly List<Asignatura> Asignaturas = new()
    {
        new Asignatura(101, "Estructuras de Datos", "SIS-101", 3),
        new Asignatura(102, "Programación Web", "SIS-102", 4),
        new Asignatura(103, "Bases de Datos I", "SIS-103", 3)
    };

    public static readonly List<Matricula> Matriculas = new();
}
