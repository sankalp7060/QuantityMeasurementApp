# Create directory structure for Model project
New-Item -ItemType Directory -Force -Path QuantityMeasurementModel/DTOs

# Create directory structure for Repo project
New-Item -ItemType Directory -Force -Path QuantityMeasurementRepo/Interfaces
New-Item -ItemType Directory -Force -Path QuantityMeasurementRepo/Implementation
New-Item -ItemType Directory -Force -Path QuantityMeasurementRepo/Entities

# Create directory structure for BusinessLogic project
New-Item -ItemType Directory -Force -Path QuantityMeasurementBusinessLogic/Interfaces
New-Item -ItemType Directory -Force -Path QuantityMeasurementBusinessLogic/Implementation

# Create directory structure for ASP project
New-Item -ItemType Directory -Force -Path QuantityMeasurementAppASP/Controllers
New-Item -ItemType Directory -Force -Path QuantityMeasurementAppASP/Extensions

Write-Host "Directory structure created successfully!" -ForegroundColor Green