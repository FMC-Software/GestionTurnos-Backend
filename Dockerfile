# 1. Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia todo el contenido de la carpeta backend (incluyendo .slnx y la carpeta src)
COPY . .

# Restaura las dependencias de la solución
RUN dotnet restore

# Publica el proyecto específico (GestionTurnos.Presentation es la API).
RUN dotnet publish src/GestionTurnos.Presentation/GestionTurnos.Presentation.csproj -c Release -o /app/publish

# 2. Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GestionTurnos.Presentation.dll"]
