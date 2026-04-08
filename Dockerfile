# ── Stage 1: Build ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies first (layer cache friendly)
COPY Docker-Training-2026.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Docker-Training-2026.dll"]

# FROM mcr.microsoft.com/dotnet/sdk:10.0

# WORKDIR /app

# COPY *.csproj ./
# RUN dotnet restore

# COPY . .
# RUN dotnet publish -c Release -o out

# CMD ["dotnet", "out/YourApp.dll"]