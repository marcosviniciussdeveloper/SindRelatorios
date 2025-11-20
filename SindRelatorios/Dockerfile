# 1. ESTÁGIO DE BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj que está na mesma pasta do Dockerfile
COPY ["SindRelatorios.csproj", "./"]
RUN dotnet restore "SindRelatorios.csproj"

# Copia todo o restante dos arquivos da pasta atual
COPY . .

# Compila e publica
RUN dotnet publish "SindRelatorios.csproj" -c Release -o /app/publish

# 2. ESTÁGIO FINAL (RUNTIME)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Configura a porta para o Fly.io
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SindRelatorios.dll"]