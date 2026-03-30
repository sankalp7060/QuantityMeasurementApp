# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file from parent directory
COPY ../QuantityMeasurementApp.sln ./

# Copy all project files from their locations relative to parent
COPY ../QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj ./QuantityMeasurementWebAPI/
COPY ../QuantityMeasurementBusinessLayer/QuantityMeasurementBusinessLayer.csproj ./QuantityMeasurementBusinessLayer/
COPY ../QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj ./QuantityMeasurementRepositoryLayer/
COPY ../QuantityMeasurementModelLayer/QuantityMeasurementModelLayer.csproj ./QuantityMeasurementModelLayer/

# Restore dependencies
RUN dotnet restore "QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj"

# Copy all source code
COPY ../QuantityMeasurementWebAPI ./QuantityMeasurementWebAPI
COPY ../QuantityMeasurementBusinessLayer ./QuantityMeasurementBusinessLayer
COPY ../QuantityMeasurementRepositoryLayer ./QuantityMeasurementRepositoryLayer
COPY ../QuantityMeasurementModelLayer ./QuantityMeasurementModelLayer

# Build and publish
WORKDIR "/src/QuantityMeasurementWebAPI"
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
