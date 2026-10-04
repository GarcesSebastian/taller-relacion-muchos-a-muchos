# ===== Etapa 1: compilación =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar dependencias (se cachea si el .csproj no cambia)
COPY ["MatriculasUnisinu.csproj", "./"]
RUN dotnet restore "MatriculasUnisinu.csproj"

# Copiar el resto del código y publicar en modo Release
COPY . .
RUN dotnet publish "MatriculasUnisinu.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== Etapa 2: ejecución =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# La API escucha en el puerto 5000 (binding a 0.0.0.0 en Program.cs)
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MatriculasUnisinu.dll"]
