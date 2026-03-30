# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files (they are in the build context since we set build context to QuantityMeasurementWebAPI)
COPY . .

# Restore dependencies
RUN dotnet restore "QuantityMeasurementWebAPI.csproj"

# Build and publish
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
