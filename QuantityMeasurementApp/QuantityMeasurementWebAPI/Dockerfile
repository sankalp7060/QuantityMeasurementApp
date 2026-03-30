# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire repository into the build context
# Since the build context is QuantityMeasurementApp/QuantityMeasurementWebAPI, 
# we need to copy from the parent directory
COPY .. .

# Restore dependencies
RUN dotnet restore "QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj"

# Build and publish
WORKDIR "/src/QuantityMeasurementWebAPI"
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
