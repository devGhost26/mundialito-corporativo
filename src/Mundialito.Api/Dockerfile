# Etapa 1: compilar y publicar con el SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Mundialito.Domain/Mundialito.Domain.csproj src/Mundialito.Domain/
COPY src/Mundialito.Application/Mundialito.Application.csproj src/Mundialito.Application/
COPY src/Mundialito.Infrastructure/Mundialito.Infrastructure.csproj src/Mundialito.Infrastructure/
COPY src/Mundialito.Api/Mundialito.Api.csproj src/Mundialito.Api/
RUN dotnet restore src/Mundialito.Api/Mundialito.Api.csproj

COPY src/ src/
RUN dotnet publish src/Mundialito.Api/Mundialito.Api.csproj -c Release -o /app/publish --no-restore

# Etapa 2: imagen final, solo con el runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "Mundialito.Api.dll"]