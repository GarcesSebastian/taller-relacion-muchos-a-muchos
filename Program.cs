var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sistema de Gestión de Matrículas Académicas - Unisinú",
        Version = "v1",
        Description = "Minimal API MVC en C# .NET que modela la relación Muchos a Muchos (N:M) " +
                      "entre Estudiantes y Asignaturas mediante la entidad intermedia Matrícula. " +
                      "Incluye borrado lógico (Soft Delete), validación de asignaturas activas y " +
                      "control de duplicados por año y periodo académico."
    });
});

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Matrículas Unisinú v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();
