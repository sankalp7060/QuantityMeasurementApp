# Use .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file (located one level up from Dockerfile)
COPY ../*.sln ./

# Copy all project files from their correct locations
COPY ../QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj ./QuantityMeasurementWebAPI/
COPY ../QuantityMeasurementBusinessLayer/QuantityMeasurementBusinessLayer.csproj ./QuantityMeasurementBusinessLayer/
COPY ../QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj ./QuantityMeasurementRepositoryLayer/
COPY ../QuantityMeasurementModelLayer/QuantityMeasurementModelLayer.csproj ./QuantityMeasurementModelLayer/

# Restore dependencies
RUN dotnet restore "QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj"

# Copy everything else
COPY .. .

# Build and publish
WORKDIR "/src/QuantityMeasurementWebAPI"
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
