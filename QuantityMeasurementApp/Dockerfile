# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (layer caching)
COPY QuantityMeasurementApp.sln .
COPY QuantityMeasurementModelLayer/QuantityMeasurementModelLayer.csproj QuantityMeasurementModelLayer/
COPY QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj QuantityMeasurementRepositoryLayer/
COPY QuantityMeasurementBusinessLayer/QuantityMeasurementBusinessLayer.csproj QuantityMeasurementBusinessLayer/
COPY QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj QuantityMeasurementWebAPI/
COPY QuantityMeasurementApp/QuantityMeasurementApp.csproj QuantityMeasurementApp/

# Restore dependencies
RUN dotnet restore QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj

# Copy all source files
COPY . .

# Publish the Web API project
RUN dotnet publish QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj \
    -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render sets PORT env var; ASP.NET Core reads ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
