# Stage base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar apenas o csproj e restaurar
COPY ["src/SecureAuth.API/SecureAuth.API.csproj", "SecureAuth.API/"]
RUN dotnet restore "SecureAuth.API/SecureAuth.API.csproj"

# Copiar todo código
COPY . .

WORKDIR "/src/src/SecureAuth.API"

# Publicar
RUN dotnet publish -c Release -o /app/publish

# Stage final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Entrypoint rodando migrations antes da API
ENTRYPOINT ["dotnet", "SecureAuth.API.dll"]