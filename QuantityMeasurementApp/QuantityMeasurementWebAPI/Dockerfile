# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY QuantityMeasurementApp.sln ./

# Copy all project files
COPY QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj QuantityMeasurementWebAPI/
COPY QuantityMeasurementBusinessLayer/QuantityMeasurementBusinessLayer.csproj QuantityMeasurementBusinessLayer/
COPY QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj QuantityMeasurementRepositoryLayer/
COPY QuantityMeasurementModelLayer/QuantityMeasurementModelLayer.csproj QuantityMeasurementModelLayer/

# Restore dependencies
RUN dotnet restore QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj

# Copy all source code
COPY QuantityMeasurementWebAPI/. QuantityMeasurementWebAPI/
COPY QuantityMeasurementBusinessLayer/. QuantityMeasurementBusinessLayer/
COPY QuantityMeasurementRepositoryLayer/. QuantityMeasurementRepositoryLayer/
COPY QuantityMeasurementModelLayer/. QuantityMeasurementModelLayer/

# Remove conflicting migration files before building
RUN rm -rf QuantityMeasurementRepositoryLayer/Migrations/*.Designer.cs 2>/dev/null || true
RUN rm -rf QuantityMeasurementRepositoryLayer/Migrations/*.cs 2>/dev/null || true

# Build and publish (disable warnings as errors)
WORKDIR /src/QuantityMeasurementWebAPI
RUN dotnet publish QuantityMeasurementWebAPI.csproj -c Release -o /app/publish -p:NoWarn=CS0579,CS0111

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]