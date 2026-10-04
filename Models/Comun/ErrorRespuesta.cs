namespace MatriculasUnisinu.Models.Comun;

// DTO de error estándar
public class ErrorRespuesta
{
    public int Codigo { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}
