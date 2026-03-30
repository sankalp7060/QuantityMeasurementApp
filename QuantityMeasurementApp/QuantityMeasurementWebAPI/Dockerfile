# Use .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj", "QuantityMeasurementWebAPI/"]
COPY ["QuantityMeasurementBusinessLayer/QuantityMeasurementBusinessLayer.csproj", "QuantityMeasurementBusinessLayer/"]
COPY ["QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj", "QuantityMeasurementRepositoryLayer/"]
COPY ["QuantityMeasurementModelLayer/QuantityMeasurementModelLayer.csproj", "QuantityMeasurementModelLayer/"]

RUN dotnet restore "QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/QuantityMeasurementWebAPI"
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]