# Use .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (they are in the build context root since we're in QuantityMeasurementWebAPI folder)
COPY *.csproj ./
COPY ../QuantityMeasurementBusinessLayer/*.csproj ./QuantityMeasurementBusinessLayer/
COPY ../QuantityMeasurementRepositoryLayer/*.csproj ./QuantityMeasurementRepositoryLayer/
COPY ../QuantityMeasurementModelLayer/*.csproj ./QuantityMeasurementModelLayer/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .
COPY ../QuantityMeasurementBusinessLayer ./QuantityMeasurementBusinessLayer
COPY ../QuantityMeasurementRepositoryLayer ./QuantityMeasurementRepositoryLayer
COPY ../QuantityMeasurementModelLayer ./QuantityMeasurementModelLayer

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
