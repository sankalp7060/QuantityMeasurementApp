// Import required namespaces for logging, domain models, DTOs, exceptions, and repository
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementModelLayer.Exceptions;
using QuantityMeasurementRepositoryLayer.Interface;

// Define the namespace for business layer service implementations
namespace QuantityMeasurementBusinessLayer.Services
{
    /// <summary>
    /// Implementation of quantity measurement service
    /// This class contains all business logic for quantity operations
    /// It coordinates between the domain models and the repository layer
    /// </summary>
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        // Private fields for dependencies
        private readonly IQuantityMeasurementRepository _repository; // Repository for data access
        private readonly ILogger<QuantityMeasurementService> _logger; // Logger for tracking operations

        /// <summary>
        /// Constructor for QuantityMeasurementService
        /// Dependencies are injected via constructor injection
        /// </summary>
        /// <param name="repository">Repository for data persistence</param>
        /// <param name="logger">Logger for this service</param>
        public QuantityMeasurementService(
            IQuantityMeasurementRepository repository,
            ILogger<QuantityMeasurementService> logger
        )
        {
            // Assign dependencies with null checking (defensive programming)
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Private Helper Methods
        // Helper methods are private - they're implementation details not exposed through interface

        /// <summary>
        /// Creates a domain quantity object from a request DTO
        /// Uses reflection to create the appropriate GenericQuantity<T> type based on category
        /// </summary>
        /// <param name="dto">The quantity request DTO</param>
        /// <returns>A domain quantity object or null if creation fails</returns>
        private object? CreateQuantity(QuantityRequestDto dto)
        {
            try
            {
                // Get the unit object for the given category and unit name
                var unit = GetUnit(dto.Category, dto.Unit);
                if (unit == null)
                {
                    // Log warning and return null if unit not found
                    _logger.LogWarning(
                        "Invalid unit {Unit} for category {Category}",
                        dto.Unit,
                        dto.Category
                    );
                    return null;
                }

                // Create generic type: GenericQuantity<T> where T is the unit type
                // For example: GenericQuantity<LengthUnit> for length measurements
                var quantityType = typeof(GenericQuantity<>).MakeGenericType(unit.GetType());

                // Create instance using Activator (dynamic object creation)
                // Equivalent to: new GenericQuantity<T>(dto.Value, unit)
                return Activator.CreateInstance(quantityType, dto.Value, unit);
            }
            catch (Exception ex)
            {
                // Log error and return null if creation fails
                _logger.LogError(
                    ex,
                    "Error creating quantity for {Category}/{Unit}",
                    dto.Category,
                    dto.Unit
                );
                return null;
            }
        }

        /// <summary>
        /// Gets the unit object based on category and unit name
        /// Uses pattern matching with switch expression to map strings to unit objects
        /// </summary>
        /// <param name="category">Measurement category (LENGTH, WEIGHT, VOLUME, TEMPERATURE)</param>
        /// <param name="unitName">Unit name (FEET, KILOGRAM, LITRE, CELSIUS, etc.)</param>
        /// <returns>The corresponding unit object or null if not found</returns>
        private object? GetUnit(string category, string unitName)
        {
            // Convert to uppercase for case-insensitive comparison
            return category.ToUpper() switch
            {
                // LENGTH category units
                "LENGTH" => unitName.ToUpper() switch
                {
                    "FEET" => LengthUnit.FEET,
                    "INCHES" => LengthUnit.INCH, // Changed from "INCH" to "INCHES"
                    "YARDS" => LengthUnit.YARD, // Changed from "YARD" to "YARDS"
                    "CENTIMETERS" => LengthUnit.CENTIMETER, // Changed from "CENTIMETER" to "CENTIMETERS"
                    "FT" => LengthUnit.FEET,
                    "IN" => LengthUnit.INCH,
                    "YD" => LengthUnit.YARD,
                    "CM" => LengthUnit.CENTIMETER,
                    _ => null,
                },
                // WEIGHT category units
                "WEIGHT" => unitName.ToUpper() switch
                {
                    "KILOGRAMS" => WeightUnit.KILOGRAM, // Now looking for "KILOGRAMS" (plural)
                    "GRAMS" => WeightUnit.GRAM, // Now looking for "GRAMS" (plural)
                    "POUNDS" => WeightUnit.POUND, // Now looking for "POUNDS" (plural)
                    "KG" => WeightUnit.KILOGRAM, // Also accept symbol
                    "G" => WeightUnit.GRAM, // Also accept symbol
                    "LB" => WeightUnit.POUND, // Also accept symbol
                    _ => null,
                },
                // VOLUME category units
                "VOLUME" => unitName.ToUpper() switch
                {
                    "LITRES" => VolumeUnit.LITRE, // Changed from "LITRE" to "LITRES"
                    "MILLILITRES" => VolumeUnit.MILLILITRE, // Changed from "MILLILITRE" to "MILLILITRES"
                    "GALLONS" => VolumeUnit.GALLON, // Changed from "GALLON" to "GALLONS"
                    "L" => VolumeUnit.LITRE,
                    "ML" => VolumeUnit.MILLILITRE,
                    "GAL" => VolumeUnit.GALLON,
                    _ => null,
                },
                // TEMPERATURE category units
                "TEMPERATURE" => unitName.ToUpper() switch
                {
                    "CELSIUS" => TemperatureUnit.CELSIUS,
                    "FAHRENHEIT" => TemperatureUnit.FAHRENHEIT,
                    "KELVIN" => TemperatureUnit.KELVIN,
                    "C" => TemperatureUnit.CELSIUS,
                    "F" => TemperatureUnit.FAHRENHEIT,
                    "K" => TemperatureUnit.KELVIN,
                    _ => null,
                },
                // Unknown category
                _ => null,
            };
        }

        /// <summary>
        /// Validates that two quantities belong to the same category
        /// Different categories cannot be compared or operated together
        /// </summary>
        /// <param name="q1">First quantity</param>
        /// <param name="q2">Second quantity</param>
        /// <param name="errorMessage">Output parameter for error message if validation fails</param>
        /// <returns>True if same category, False otherwise</returns>
        private bool ValidateSameCategory(
            QuantityRequestDto q1,
            QuantityRequestDto q2,
            out string errorMessage
        )
        {
            // Compare categories case-insensitively
            if (!string.Equals(q1.Category, q2.Category, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage =
                    $"Category mismatch: {q1.Category} and {q2.Category} cannot be compared";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Gets the symbol representation of a unit
        /// Uses reflection to call GetSymbol() method on the unit object
        /// </summary>
        /// <param name="unit">The unit object</param>
        /// <returns>Unit symbol (e.g., "ft", "kg", "L", "°C")</returns>
        private string GetUnitSymbol(object unit)
        {
            // Call GetSymbol method via reflection and return result
            return unit.GetType().GetMethod("GetSymbol")?.Invoke(unit, null)?.ToString() ?? "";
        }

        /// <summary>
        /// Creates a success entity for binary operations
        /// Factory method to create and populate a QuantityMeasurementEntity for successful operations
        /// </summary>
        private QuantityMeasurementEntity CreateSuccessEntity(
            OperationType operation,
            BinaryOperationRequestDto request,
            double resultValue,
            string resultUnit,
            string formattedResult
        )
        {
            return QuantityMeasurementEntity.CreateBinaryOperation(
                operation,
                request.FirstQuantity.Value,
                request.FirstQuantity.Unit,
                request.FirstQuantity.Category,
                request.SecondQuantity.Value,
                request.SecondQuantity.Unit,
                request.SecondQuantity.Category,
                request.TargetUnit,
                resultValue,
                resultUnit,
                formattedResult,
                true // IsSuccessful = true
            );
        }

        /// <summary>
        /// Creates a success entity for conversion operations
        /// Factory method for conversion-specific entity creation
        /// </summary>
        private QuantityMeasurementEntity CreateConversionSuccessEntity(
            ConversionRequestDto request,
            double resultValue,
            string resultUnit,
            string formattedResult
        )
        {
            return QuantityMeasurementEntity.CreateConversion(
                request.Source.Value,
                request.Source.Unit,
                request.Source.Category,
                request.TargetUnit,
                resultValue,
                resultUnit,
                formattedResult,
                true // IsSuccessful = true
            );
        }

        /// <summary>
        /// Creates an error entity for binary operations
        /// Factory method to create and populate an entity for failed operations
        /// </summary>
        private QuantityMeasurementEntity CreateErrorEntity(
            OperationType operation,
            BinaryOperationRequestDto request,
            string errorMessage
        )
        {
            return QuantityMeasurementEntity.CreateBinaryOperation(
                operation,
                request.FirstQuantity.Value,
                request.FirstQuantity.Unit,
                request.FirstQuantity.Category,
                request.SecondQuantity.Value,
                request.SecondQuantity.Unit,
                request.SecondQuantity.Category,
                request.TargetUnit,
                null, // No result value
                null, // No result unit
                null, // No formatted result
                false, // IsSuccessful = false
                errorMessage
            );
        }

        /// <summary>
        /// Creates an error entity for conversion operations
        /// Factory method for conversion-specific error entity
        /// </summary>
        private QuantityMeasurementEntity CreateConversionErrorEntity(
            ConversionRequestDto request,
            string errorMessage
        )
        {
            return QuantityMeasurementEntity.CreateConversion(
                request.Source.Value,
                request.Source.Unit,
                request.Source.Category,
                request.TargetUnit,
                null, // No result value
                null, // No result unit
                null, // No formatted result
                false, // IsSuccessful = false
                errorMessage
            );
        }

        /// <summary>
        /// Maps a database entity to a history DTO
        /// Converts internal entity to public-facing DTO for API responses
        /// </summary>
        /// <param name="entity">Database entity</param>
        /// <returns>History DTO</returns>
        private MeasurementHistoryDto MapToHistoryDto(QuantityMeasurementEntity entity)
        {
            return new MeasurementHistoryDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Operation = entity.OperationType.ToString(), // Convert enum to string
                Result = entity.FormattedResult,
                IsSuccessful = entity.IsSuccessful,
                Error = entity.ErrorDetails,
            };
        }

        #endregion

        #region Core Operations
        // These methods implement the main business logic for measurements

        /// <inheritdoc/>
        public async Task<QuantityResponseDto> CompareQuantitiesAsync(
            BinaryOperationRequestDto request
        )
        {
            try
            {
                // Log the comparison operation
                _logger.LogInformation(
                    "Comparing quantities: {First} vs {Second}",
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit}",
                    $"{request.SecondQuantity.Value} {request.SecondQuantity.Unit}"
                );

                // Step 1: Validate that quantities are in the same category
                if (
                    !ValidateSameCategory(
                        request.FirstQuantity,
                        request.SecondQuantity,
                        out var categoryError
                    )
                )
                {
                    // Save error to repository
                    var errorEntity = CreateErrorEntity(
                        OperationType.Compare,
                        request,
                        categoryError
                    );
                    await _repository.AddAsync(errorEntity);

                    // Return error response
                    return QuantityResponseDto.ErrorResponse(categoryError, OperationType.Compare);
                }

                // Step 2: Create domain quantity objects
                var q1 = CreateQuantity(request.FirstQuantity);
                var q2 = CreateQuantity(request.SecondQuantity);

                // Check if quantity creation failed
                if (q1 == null || q2 == null)
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Compare,
                        request,
                        "Invalid quantity format"
                    );
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(
                        "Invalid quantity format",
                        OperationType.Compare
                    );
                }

                // Step 3: Perform comparison using reflection to call Equals method
                var equalsMethod = q1.GetType().GetMethod("Equals", new[] { typeof(object) });
                var result = (bool)equalsMethod!.Invoke(q1, new[] { q2 })!;

                // Step 4: Create user-friendly messages
                var message = result
                    ? $"✅ {request.FirstQuantity.Value} {request.FirstQuantity.Unit} equals {request.SecondQuantity.Value} {request.SecondQuantity.Unit}"
                    : $"❌ {request.FirstQuantity.Value} {request.FirstQuantity.Unit} does not equal {request.SecondQuantity.Value} {request.SecondQuantity.Unit}";

                var formattedResult =
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} {(result ? "=" : "≠")} {request.SecondQuantity.Value} {request.SecondQuantity.Unit}";

                // Step 5: Save successful operation to repository
                var successEntity = CreateSuccessEntity(
                    OperationType.Compare,
                    request,
                    result ? 1.0 : 0.0, // Store 1 for equal, 0 for not equal
                    string.Empty, // No unit for comparison result
                    formattedResult
                );
                await _repository.AddAsync(successEntity);

                // Step 6: Return success response
                return new QuantityResponseDto
                {
                    Success = true,
                    Message = message,
                    Result = result ? 1.0 : 0.0,
                    FormattedResult = formattedResult,
                    Operation = OperationType.Compare,
                    Timestamp = DateTime.UtcNow,
                };
            }
            catch (Exception ex)
            {
                // Log and wrap any unexpected exceptions
                _logger.LogError(ex, "Error comparing quantities");
                throw new QuantityMeasurementException($"Comparison error: {ex.Message}", ex)
                {
                    OperationType = "Compare",
                };
            }
        }

        /// <inheritdoc/>
        public async Task<QuantityResponseDto> ConvertQuantityAsync(ConversionRequestDto request)
        {
            try
            {
                // Log the conversion operation
                _logger.LogInformation(
                    "Converting {Value} {FromUnit} to {ToUnit}",
                    request.Source.Value,
                    request.Source.Unit,
                    request.TargetUnit
                );

                // Step 1: Create source quantity object
                var source = CreateQuantity(request.Source);
                if (source == null)
                {
                    // Save error to repository
                    var errorEntity = CreateConversionErrorEntity(
                        request,
                        "Invalid source quantity"
                    );
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(
                        "Invalid source quantity",
                        OperationType.Convert
                    );
                }

                // Step 2: Validate target unit
                var targetUnit = GetUnit(request.Source.Category, request.TargetUnit);
                if (targetUnit == null)
                {
                    var errorEntity = CreateConversionErrorEntity(
                        request,
                        $"Invalid target unit: {request.TargetUnit}"
                    );
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(
                        $"Invalid target unit: {request.TargetUnit}",
                        OperationType.Convert
                    );
                }

                // Step 3: Perform conversion using ConvertTo method
                var converted = source
                    .GetType()
                    .GetMethod("ConvertTo")
                    ?.Invoke(source, new[] { targetUnit });

                if (converted == null)
                {
                    var errorEntity = CreateConversionErrorEntity(request, "Conversion failed");
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(
                        "Conversion failed",
                        OperationType.Convert
                    );
                }

                // Step 4: Extract result value and format
                var resultValue = (double)
                    converted.GetType().GetProperty("Value")!.GetValue(converted)!;
                var resultUnitSymbol = GetUnitSymbol(targetUnit);
                var formattedResult =
                    $"{request.Source.Value} {request.Source.Unit} = {resultValue:F6} {resultUnitSymbol}";

                // Step 5: Save to repository
                var successEntity = CreateConversionSuccessEntity(
                    request,
                    resultValue,
                    resultUnitSymbol,
                    formattedResult
                );
                await _repository.AddAsync(successEntity);

                // Step 6: Return success response
                return QuantityResponseDto.SuccessResponse(
                    resultValue,
                    resultUnitSymbol,
                    OperationType.Convert,
                    formattedResult
                );
            }
            catch (NotSupportedException ex)
            {
                // Handle domain-specific NotSupportedException (e.g., incompatible units)
                _logger.LogWarning(ex, "Unsupported conversion operation");
                var errorEntity = CreateConversionErrorEntity(request, ex.Message);
                await _repository.AddAsync(errorEntity);

                return QuantityResponseDto.ErrorResponse(ex.Message, OperationType.Convert);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                _logger.LogError(ex, "Error during conversion");
                throw new QuantityMeasurementException($"Conversion error: {ex.Message}", ex)
                {
                    OperationType = "Convert",
                };
            }
        }

        /// <inheritdoc/>
        public async Task<QuantityResponseDto> AddQuantitiesAsync(BinaryOperationRequestDto request)
        {
            try
            {
                // Log addition operation
                _logger.LogInformation(
                    "Adding quantities: {First} + {Second}",
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit}",
                    $"{request.SecondQuantity.Value} {request.SecondQuantity.Unit}"
                );

                // Step 1: Validate category match
                if (
                    !ValidateSameCategory(
                        request.FirstQuantity,
                        request.SecondQuantity,
                        out var categoryError
                    )
                )
                {
                    var errorEntity = CreateErrorEntity(OperationType.Add, request, categoryError);
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(categoryError, OperationType.Add);
                }

                // Step 2: Create quantity objects
                var q1 = CreateQuantity(request.FirstQuantity);
                var q2 = CreateQuantity(request.SecondQuantity);

                if (q1 == null || q2 == null)
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Add,
                        request,
                        "Invalid quantity format"
                    );
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(
                        "Invalid quantity format",
                        OperationType.Add
                    );
                }

                // Step 3: Determine target unit for result
                object? targetUnit;
                if (!string.IsNullOrEmpty(request.TargetUnit))
                {
                    // Use requested target unit if provided
                    targetUnit = GetUnit(request.FirstQuantity.Category, request.TargetUnit);
                    if (targetUnit == null)
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Add,
                            request,
                            $"Invalid target unit: {request.TargetUnit}"
                        );
                        await _repository.AddAsync(errorEntity);

                        return QuantityResponseDto.ErrorResponse(
                            $"Invalid target unit: {request.TargetUnit}",
                            OperationType.Add
                        );
                    }
                }
                else
                {
                    // Default to first quantity's unit
                    targetUnit = q1.GetType().GetProperty("Unit")!.GetValue(q1)!;
                }

                // Step 4: Perform addition using reflection
                var result = q1.GetType()
                    .GetMethod("Add", new[] { q2.GetType(), targetUnit.GetType() })!
                    .Invoke(q1, new[] { q2, targetUnit });

                // Step 5: Extract result value
                var valueProperty = result!.GetType().GetProperty("Value");
                if (valueProperty == null)
                {
                    throw new QuantityMeasurementException(
                        "Failed to get Value property from result"
                    );
                }
                var resultValueObj = valueProperty.GetValue(result);
                if (resultValueObj == null)
                {
                    throw new QuantityMeasurementException("Result value is null");
                }
                var resultValue = (double)resultValueObj;

                // Step 6: Format result
                var resultUnitSymbol = GetUnitSymbol(targetUnit);
                var formattedResult =
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} + {request.SecondQuantity.Value} {request.SecondQuantity.Unit} = {resultValue:F6} {resultUnitSymbol}";

                // Step 7: Save to repository
                var successEntity = CreateSuccessEntity(
                    OperationType.Add,
                    request,
                    resultValue,
                    resultUnitSymbol,
                    formattedResult
                );
                await _repository.AddAsync(successEntity);

                // Step 8: Return success response
                return QuantityResponseDto.SuccessResponse(
                    resultValue,
                    resultUnitSymbol,
                    OperationType.Add,
                    formattedResult
                );
            }
            catch (NotSupportedException ex)
            {
                // Handle unsupported operations (e.g., adding temperatures)
                _logger.LogWarning(ex, "Unsupported addition operation");
                var errorEntity = CreateErrorEntity(OperationType.Add, request, ex.Message);
                await _repository.AddAsync(errorEntity);

                return QuantityResponseDto.ErrorResponse(ex.Message, OperationType.Add);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during addition");
                throw new QuantityMeasurementException($"Addition error: {ex.Message}", ex)
                {
                    OperationType = "Add",
                };
            }
        }

        /// <inheritdoc/>
        public async Task<QuantityResponseDto> SubtractQuantitiesAsync(
            BinaryOperationRequestDto request
        )
        {
            try
            {
                // Log subtraction operation
                _logger.LogInformation(
                    "Subtracting quantities: {First} - {Second}",
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit}",
                    $"{request.SecondQuantity.Value} {request.SecondQuantity.Unit}"
                );

                // Step 1: Validate category match
                if (
                    !ValidateSameCategory(
                        request.FirstQuantity,
                        request.SecondQuantity,
                        out var categoryError
                    )
                )
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Subtract,
                        request,
                        categoryError
                    );
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(categoryError, OperationType.Subtract);
                }

                // Step 2: Create quantity objects
                var q1 = CreateQuantity(request.FirstQuantity);
                var q2 = CreateQuantity(request.SecondQuantity);

                if (q1 == null || q2 == null)
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Subtract,
                        request,
                        "Invalid quantity format"
                    );
                    await _repository.AddAsync(errorEntity);

                    return QuantityResponseDto.ErrorResponse(
                        "Invalid quantity format",
                        OperationType.Subtract
                    );
                }

                // Step 3: Determine target unit
                object? targetUnit;
                if (!string.IsNullOrEmpty(request.TargetUnit))
                {
                    targetUnit = GetUnit(request.FirstQuantity.Category, request.TargetUnit);
                    if (targetUnit == null)
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Subtract,
                            request,
                            $"Invalid target unit: {request.TargetUnit}"
                        );
                        await _repository.AddAsync(errorEntity);

                        return QuantityResponseDto.ErrorResponse(
                            $"Invalid target unit: {request.TargetUnit}",
                            OperationType.Subtract
                        );
                    }
                }
                else
                {
                    targetUnit = q1.GetType().GetProperty("Unit")!.GetValue(q1)!;
                }

                // Step 4: Perform subtraction
                var result = q1.GetType()
                    .GetMethod("Subtract", new[] { q2.GetType(), targetUnit.GetType() })!
                    .Invoke(q1, new[] { q2, targetUnit });

                // Step 5: Extract result
                var resultValue = (double)result!.GetType().GetProperty("Value")!.GetValue(result)!;
                var resultUnitSymbol = GetUnitSymbol(targetUnit);
                var formattedResult =
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} - {request.SecondQuantity.Value} {request.SecondQuantity.Unit} = {resultValue:F6} {resultUnitSymbol}";

                // Step 6: Save to repository
                var successEntity = CreateSuccessEntity(
                    OperationType.Subtract,
                    request,
                    resultValue,
                    resultUnitSymbol,
                    formattedResult
                );
                await _repository.AddAsync(successEntity);

                // Step 7: Return success
                return QuantityResponseDto.SuccessResponse(
                    resultValue,
                    resultUnitSymbol,
                    OperationType.Subtract,
                    formattedResult
                );
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported subtraction operation");
                var errorEntity = CreateErrorEntity(OperationType.Subtract, request, ex.Message);
                await _repository.AddAsync(errorEntity);

                return QuantityResponseDto.ErrorResponse(ex.Message, OperationType.Subtract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during subtraction");
                throw new QuantityMeasurementException($"Subtraction error: {ex.Message}", ex)
                {
                    OperationType = "Subtract",
                };
            }
        }

        /// <inheritdoc/>
        public async Task<DivisionResponseDto> DivideQuantitiesAsync(
            BinaryOperationRequestDto request
        )
        {
            try
            {
                // Log division operation
                _logger.LogInformation(
                    "Dividing quantities: {First} ÷ {Second}",
                    $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit}",
                    $"{request.SecondQuantity.Value} {request.SecondQuantity.Unit}"
                );

                // Step 1: Validate category match
                if (
                    !ValidateSameCategory(
                        request.FirstQuantity,
                        request.SecondQuantity,
                        out var categoryError
                    )
                )
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Divide,
                        request,
                        categoryError
                    );
                    await _repository.AddAsync(errorEntity);

                    return DivisionResponseDto.ErrorResponse(categoryError);
                }

                // Step 2: Create quantity objects
                var q1 = CreateQuantity(request.FirstQuantity);
                var q2 = CreateQuantity(request.SecondQuantity);

                if (q1 == null || q2 == null)
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Divide,
                        request,
                        "Invalid quantity format"
                    );
                    await _repository.AddAsync(errorEntity);

                    return DivisionResponseDto.ErrorResponse("Invalid quantity format");
                }

                // Step 3: Check for division by zero
                // Convert second quantity to base unit to get absolute value
                var q2InBase = q2.GetType()
                    .GetMethod("ConvertTo")
                    ?.Invoke(
                        q2,
                        new[]
                        {
                            GetUnit(request.SecondQuantity.Category, request.SecondQuantity.Unit),
                        }
                    );
                var q2Value = (double)q2InBase?.GetType().GetProperty("Value")!.GetValue(q2InBase)!;

                if (Math.Abs(q2Value) < 0.000001) // Using small epsilon for floating point comparison
                {
                    var errorEntity = CreateErrorEntity(
                        OperationType.Divide,
                        request,
                        "Division by zero is not allowed"
                    );
                    await _repository.AddAsync(errorEntity);

                    return DivisionResponseDto.ErrorResponse("Division by zero is not allowed");
                }

                // Step 4: Perform division
                var divideMethod = q1.GetType().GetMethod("Divide", new[] { q2.GetType() });
                var ratio = (double)divideMethod!.Invoke(q1, new[] { q2 })!;

                // Step 5: Create human-readable interpretation
                string interpretation;
                if (Math.Abs(ratio - 1.0) < 0.000001)
                {
                    interpretation = "The quantities are equal";
                }
                else if (ratio > 1.0)
                {
                    interpretation =
                        $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} is {ratio:F2} times larger than {request.SecondQuantity.Value} {request.SecondQuantity.Unit}";
                }
                else
                {
                    var inverse = 1.0 / ratio;
                    interpretation =
                        $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} is {inverse:F2} times smaller than {request.SecondQuantity.Value} {request.SecondQuantity.Unit}";
                }

                // Step 6: Save to repository
                var successEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                    OperationType.Divide,
                    request.FirstQuantity.Value,
                    request.FirstQuantity.Unit,
                    request.FirstQuantity.Category,
                    request.SecondQuantity.Value,
                    request.SecondQuantity.Unit,
                    request.SecondQuantity.Category,
                    null, // No target unit for division (result is dimensionless)
                    ratio,
                    null, // No result unit (dimensionless)
                    interpretation,
                    true
                );
                await _repository.AddAsync(successEntity);

                // Step 7: Return success response
                return DivisionResponseDto.SuccessResponse(ratio, interpretation);
            }
            catch (DivideByZeroException)
            {
                // Handle explicit divide by zero
                _logger.LogWarning("Division by zero attempted");
                var errorEntity = CreateErrorEntity(
                    OperationType.Divide,
                    request,
                    "Division by zero is not allowed"
                );
                await _repository.AddAsync(errorEntity);

                return DivisionResponseDto.ErrorResponse("Division by zero is not allowed");
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported division operation");
                var errorEntity = CreateErrorEntity(OperationType.Divide, request, ex.Message);
                await _repository.AddAsync(errorEntity);

                return DivisionResponseDto.ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during division");
                throw new QuantityMeasurementException($"Division error: {ex.Message}", ex)
                {
                    OperationType = "Divide",
                };
            }
        }

        #endregion

        #region History and Query Operations
        // These methods retrieve historical data

        /// <inheritdoc/>
        public async Task<IEnumerable<MeasurementHistoryDto>> GetOperationHistoryAsync(
            OperationType? operation = null
        )
        {
            try
            {
                IEnumerable<QuantityMeasurementEntity> entities;

                // Get entities based on whether operation filter is provided
                if (operation.HasValue)
                {
                    entities = await _repository.GetByOperationAsync(operation.Value);
                }
                else
                {
                    entities = await _repository.GetAllAsync();
                }

                // Map each entity to DTO and return
                return entities.Select(MapToHistoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operation history");
                throw; // Rethrow to let global handler process
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MeasurementHistoryDto>> GetCategoryHistoryAsync(
            string category
        )
        {
            try
            {
                var entities = await _repository.GetByCategoryAsync(category);
                return entities.Select(MapToHistoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category history for {Category}", category);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MeasurementHistoryDto>> GetDateRangeHistoryAsync(
            DateTime start,
            DateTime end
        )
        {
            try
            {
                var entities = await _repository.GetByDateRangeAsync(start, end);
                return entities.Select(MapToHistoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting date range history");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MeasurementHistoryDto>> GetErrorHistoryAsync()
        {
            try
            {
                var entities = await _repository.GetFailedOperationsAsync();
                return entities.Select(MapToHistoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting error history");
                throw;
            }
        }

        #endregion

        #region Statistics
        // These methods provide statistical analysis

        /// <inheritdoc/>
        public async Task<Dictionary<string, object>> GetStatisticsAsync()
        {
            try
            {
                // Gather various statistics
                var totalCount = await _repository.GetTotalCountAsync();
                var operationCounts = await _repository.GetOperationCountsAsync();
                var successCount = await _repository
                    .GetSuccessfulOperationsAsync()
                    .ContinueWith(t => t.Result.Count()); // Count successful operations
                var errorCount = await _repository
                    .GetFailedOperationsAsync()
                    .ContinueWith(t => t.Result.Count()); // Count failed operations

                // Return as dictionary for flexibility
                return new Dictionary<string, object>
                {
                    ["TotalOperations"] = totalCount,
                    ["SuccessfulOperations"] = successCount,
                    ["FailedOperations"] = errorCount,
                    ["SuccessRate"] = totalCount > 0 ? (double)successCount / totalCount : 0,
                    ["OperationCounts"] = operationCounts,
                    ["LastUpdated"] = DateTime.UtcNow,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetOperationCountAsync(OperationType operation)
        {
            try
            {
                return await _repository.GetCountByOperationAsync(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operation count for {Operation}", operation);
                throw;
            }
        }

        #endregion
    }
}
