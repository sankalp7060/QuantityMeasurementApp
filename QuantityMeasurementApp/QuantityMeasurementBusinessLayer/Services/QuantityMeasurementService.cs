#nullable disable

using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementBusinessLayer.Exceptions;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Services
{
    /// <summary>
    /// Implementation of quantity measurement business logic.
    /// Follows Single Responsibility and Open/Closed Principles.
    /// </summary>
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;
        private readonly ILogger<QuantityMeasurementService> _logger;

        public QuantityMeasurementService(
            ILogger<QuantityMeasurementService> logger,
            IQuantityMeasurementRepository repository
        )
        {
            _logger = logger;
            _repository = repository;
        }

        #region Private Helper Methods

        private object CreateQuantity(QuantityDTO dto)
        {
            try
            {
                var unit = GetUnit(dto.Category, dto.Unit);
                if (unit == null)
                    return null;

                var quantityType = typeof(GenericQuantity<>).MakeGenericType(unit.GetType());
                return Activator.CreateInstance(quantityType, dto.Value, unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating quantity for {Category}/{Unit}",
                    dto.Category,
                    dto.Unit
                );
                return null;
            }
        }

        private object GetUnit(string category, string unitName)
        {
            return category.ToUpper() switch
            {
                "LENGTH" => unitName.ToUpper() switch
                {
                    "FEET" => LengthUnit.FEET,
                    "INCH" => LengthUnit.INCH,
                    "YARD" => LengthUnit.YARD,
                    "CENTIMETER" => LengthUnit.CENTIMETER,
                    _ => null,
                },
                "WEIGHT" => unitName.ToUpper() switch
                {
                    "KILOGRAM" => WeightUnit.KILOGRAM,
                    "GRAM" => WeightUnit.GRAM,
                    "POUND" => WeightUnit.POUND,
                    _ => null,
                },
                "VOLUME" => unitName.ToUpper() switch
                {
                    "LITRE" => VolumeUnit.LITRE,
                    "MILLILITRE" => VolumeUnit.MILLILITRE,
                    "GALLON" => VolumeUnit.GALLON,
                    _ => null,
                },
                "TEMPERATURE" => unitName.ToUpper() switch
                {
                    "CELSIUS" => TemperatureUnit.CELSIUS,
                    "FAHRENHEIT" => TemperatureUnit.FAHRENHEIT,
                    "KELVIN" => TemperatureUnit.KELVIN,
                    _ => null,
                },
                _ => null,
            };
        }

        private bool ValidateSameCategory(QuantityDTO q1, QuantityDTO q2, out string errorMessage)
        {
            if (!string.Equals(q1.Category, q2.Category, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage =
                    $"Category mismatch: {q1.Category} and {q2.Category} cannot be compared";
                return false;
            }
            errorMessage = null;
            return true;
        }

        private string GetUnitSymbol(object unit)
        {
            return unit.GetType().GetMethod("GetSymbol")?.Invoke(unit, null)?.ToString() ?? "";
        }

        private QuantityMeasurementEntity CreateSuccessEntity(
            OperationType operation,
            BinaryQuantityRequest request,
            double resultValue,
            string resultUnit,
            string formattedResult
        )
        {
            return QuantityMeasurementEntity.CreateBinaryOperation(
                operation,
                request.Quantity1.Value,
                request.Quantity1.Unit,
                request.Quantity1.Category,
                request.Quantity2.Value,
                request.Quantity2.Unit,
                request.Quantity2.Category,
                request.TargetUnit,
                resultValue,
                resultUnit,
                formattedResult,
                true
            );
        }

        private QuantityMeasurementEntity CreateConversionSuccessEntity(
            ConversionRequest request,
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
                true
            );
        }

        private QuantityMeasurementEntity CreateErrorEntity(
            OperationType operation,
            BinaryQuantityRequest request,
            string errorMessage
        )
        {
            return QuantityMeasurementEntity.CreateBinaryOperation(
                operation,
                request.Quantity1.Value,
                request.Quantity1.Unit,
                request.Quantity1.Category,
                request.Quantity2.Value,
                request.Quantity2.Unit,
                request.Quantity2.Category,
                request.TargetUnit,
                null,
                null,
                null,
                false,
                errorMessage
            );
        }

        private QuantityMeasurementEntity CreateConversionErrorEntity(
            ConversionRequest request,
            string errorMessage
        )
        {
            return QuantityMeasurementEntity.CreateConversion(
                request.Source.Value,
                request.Source.Unit,
                request.Source.Category,
                request.TargetUnit,
                null,
                null,
                null,
                false,
                errorMessage
            );
        }

        #endregion

        #region Public Methods

        public async Task<QuantityResponse> CompareQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation(
                        "Comparing quantities: {Q1} vs {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    // Validate category match
                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Compare,
                            request,
                            categoryError
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(categoryError, OperationType.Compare);
                    }

                    // Create quantity objects
                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Compare,
                            request,
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            "Invalid quantity format",
                            OperationType.Compare
                        );
                    }

                    // Perform comparison
                    var equalsMethod = q1.GetType().GetMethod("Equals", new[] { typeof(object) });
                    var result = (bool)equalsMethod!.Invoke(q1, new[] { q2 })!;

                    var message = result
                        ? $"✅ {request.Quantity1.Value} {request.Quantity1.Unit} equals {request.Quantity2.Value} {request.Quantity2.Unit}"
                        : $"❌ {request.Quantity1.Value} {request.Quantity1.Unit} does not equal {request.Quantity2.Value} {request.Quantity2.Unit}";

                    var formattedResult =
                        $"{request.Quantity1.Value} {request.Quantity1.Unit} {(result ? "=" : "≠")} {request.Quantity2.Value} {request.Quantity2.Unit}";

                    // Save to repository
                    var entity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Compare,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        null,
                        result ? 1.0 : 0.0,
                        null,
                        formattedResult,
                        true
                    );
                    _repository.Save(entity);

                    return new QuantityResponse
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
                    _logger.LogError(ex, "Error comparing quantities");
                    var quantityEx = new QuantityMeasurementException(
                        $"Comparison error: {ex.Message}",
                        ex
                    )
                    {
                        OperationType = "Compare",
                    };
                    throw quantityEx;
                }
            });
        }

        public async Task<QuantityResponse> ConvertQuantityAsync(ConversionRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation(
                        "Converting {Value} {FromUnit} to {ToUnit}",
                        request.Source.Value,
                        request.Source.Unit,
                        request.TargetUnit
                    );

                    var source = CreateQuantity(request.Source);
                    if (source == null)
                    {
                        var errorEntity = CreateConversionErrorEntity(
                            request,
                            "Invalid source quantity"
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            "Invalid source quantity",
                            OperationType.Convert
                        );
                    }

                    var targetUnit = GetUnit(request.Source.Category, request.TargetUnit);
                    if (targetUnit == null)
                    {
                        var errorEntity = CreateConversionErrorEntity(
                            request,
                            $"Invalid target unit: {request.TargetUnit}"
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            $"Invalid target unit: {request.TargetUnit}",
                            OperationType.Convert
                        );
                    }

                    var converted = source
                        .GetType()
                        .GetMethod("ConvertTo")
                        ?.Invoke(source, new[] { targetUnit });
                    if (converted == null)
                    {
                        var errorEntity = CreateConversionErrorEntity(request, "Conversion failed");
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            "Conversion failed",
                            OperationType.Convert
                        );
                    }

                    var resultValue = (double)
                        converted.GetType().GetProperty("Value")!.GetValue(converted)!;
                    var resultUnitSymbol = GetUnitSymbol(targetUnit);
                    var formattedResult =
                        $"{request.Source.Value} {request.Source.Unit} = {resultValue:F6} {resultUnitSymbol}";

                    // Save to repository
                    var entity = CreateConversionSuccessEntity(
                        request,
                        resultValue,
                        resultUnitSymbol,
                        formattedResult
                    );
                    _repository.Save(entity);

                    return QuantityResponse.SuccessResponse(
                        resultValue,
                        resultUnitSymbol,
                        OperationType.Convert,
                        formattedResult
                    );
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported conversion operation");
                    var errorEntity = CreateConversionErrorEntity(request, ex.Message);
                    _repository.Save(errorEntity);
                    return QuantityResponse.ErrorResponse(ex.Message, OperationType.Convert);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during conversion");
                    var quantityEx = new QuantityMeasurementException(
                        $"Conversion error: {ex.Message}",
                        ex
                    )
                    {
                        OperationType = "Convert",
                    };
                    throw quantityEx;
                }
            });
        }

        public async Task<QuantityResponse> AddQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation(
                        "Adding quantities: {Q1} + {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    // Validate category match
                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Add,
                            request,
                            categoryError
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(categoryError, OperationType.Add);
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Add,
                            request,
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            "Invalid quantity format",
                            OperationType.Add
                        );
                    }

                    // Determine target unit
                    object targetUnit;
                    if (!string.IsNullOrEmpty(request.TargetUnit))
                    {
                        targetUnit = GetUnit(request.Quantity1.Category, request.TargetUnit);
                        if (targetUnit == null)
                        {
                            var errorEntity = CreateErrorEntity(
                                OperationType.Add,
                                request,
                                $"Invalid target unit: {request.TargetUnit}"
                            );
                            _repository.Save(errorEntity);
                            return QuantityResponse.ErrorResponse(
                                $"Invalid target unit: {request.TargetUnit}",
                                OperationType.Add
                            );
                        }
                    }
                    else
                    {
                        targetUnit = q1.GetType().GetProperty("Unit")!.GetValue(q1)!;
                    }

                    // Perform addition
                    var result = q1.GetType()
                        .GetMethod("Add", new[] { q2.GetType(), targetUnit.GetType() })!
                        .Invoke(q1, new[] { q2, targetUnit });

                    var resultValue = (double)
                        result.GetType().GetProperty("Value")!.GetValue(result)!;
                    var resultUnitSymbol = GetUnitSymbol(targetUnit);
                    var formattedResult =
                        $"{request.Quantity1.Value} {request.Quantity1.Unit} + {request.Quantity2.Value} {request.Quantity2.Unit} = {resultValue:F6} {resultUnitSymbol}";

                    // Save to repository
                    var entity = CreateSuccessEntity(
                        OperationType.Add,
                        request,
                        resultValue,
                        resultUnitSymbol,
                        formattedResult
                    );
                    _repository.Save(entity);

                    return QuantityResponse.SuccessResponse(
                        resultValue,
                        resultUnitSymbol,
                        OperationType.Add,
                        formattedResult
                    );
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported addition operation");
                    var errorEntity = CreateErrorEntity(OperationType.Add, request, ex.Message);
                    _repository.Save(errorEntity);
                    return QuantityResponse.ErrorResponse(ex.Message, OperationType.Add);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during addition");
                    var quantityEx = new QuantityMeasurementException(
                        $"Addition error: {ex.Message}",
                        ex
                    )
                    {
                        OperationType = "Add",
                    };
                    throw quantityEx;
                }
            });
        }

        public async Task<QuantityResponse> SubtractQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation(
                        "Subtracting quantities: {Q1} - {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    // Validate category match
                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Subtract,
                            request,
                            categoryError
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            categoryError,
                            OperationType.Subtract
                        );
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Subtract,
                            request,
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);
                        return QuantityResponse.ErrorResponse(
                            "Invalid quantity format",
                            OperationType.Subtract
                        );
                    }

                    // Determine target unit
                    object targetUnit;
                    if (!string.IsNullOrEmpty(request.TargetUnit))
                    {
                        targetUnit = GetUnit(request.Quantity1.Category, request.TargetUnit);
                        if (targetUnit == null)
                        {
                            var errorEntity = CreateErrorEntity(
                                OperationType.Subtract,
                                request,
                                $"Invalid target unit: {request.TargetUnit}"
                            );
                            _repository.Save(errorEntity);
                            return QuantityResponse.ErrorResponse(
                                $"Invalid target unit: {request.TargetUnit}",
                                OperationType.Subtract
                            );
                        }
                    }
                    else
                    {
                        targetUnit = q1.GetType().GetProperty("Unit")!.GetValue(q1)!;
                    }

                    // Perform subtraction
                    var result = q1.GetType()
                        .GetMethod("Subtract", new[] { q2.GetType(), targetUnit.GetType() })!
                        .Invoke(q1, new[] { q2, targetUnit });

                    var resultValue = (double)
                        result.GetType().GetProperty("Value")!.GetValue(result)!;
                    var resultUnitSymbol = GetUnitSymbol(targetUnit);
                    var formattedResult =
                        $"{request.Quantity1.Value} {request.Quantity1.Unit} - {request.Quantity2.Value} {request.Quantity2.Unit} = {resultValue:F6} {resultUnitSymbol}";

                    // Save to repository
                    var entity = CreateSuccessEntity(
                        OperationType.Subtract,
                        request,
                        resultValue,
                        resultUnitSymbol,
                        formattedResult
                    );
                    _repository.Save(entity);

                    return QuantityResponse.SuccessResponse(
                        resultValue,
                        resultUnitSymbol,
                        OperationType.Subtract,
                        formattedResult
                    );
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported subtraction operation");
                    var errorEntity = CreateErrorEntity(
                        OperationType.Subtract,
                        request,
                        ex.Message
                    );
                    _repository.Save(errorEntity);
                    return QuantityResponse.ErrorResponse(ex.Message, OperationType.Subtract);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during subtraction");
                    var quantityEx = new QuantityMeasurementException(
                        $"Subtraction error: {ex.Message}",
                        ex
                    )
                    {
                        OperationType = "Subtract",
                    };
                    throw quantityEx;
                }
            });
        }

        public async Task<DivisionResponse> DivideQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation(
                        "Dividing quantities: {Q1} ÷ {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    // Validate category match
                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Divide,
                            request,
                            categoryError
                        );
                        _repository.Save(errorEntity);
                        return DivisionResponse.ErrorResponse(categoryError);
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = CreateErrorEntity(
                            OperationType.Divide,
                            request,
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);
                        return DivisionResponse.ErrorResponse("Invalid quantity format");
                    }

                    // Perform division
                    var divideMethod = q1.GetType().GetMethod("Divide", new[] { q2.GetType() });
                    var ratio = (double)divideMethod!.Invoke(q1, new[] { q2 })!;

                    string interpretation;
                    if (Math.Abs(ratio - 1.0) < 0.000001)
                    {
                        interpretation = "The quantities are equal";
                    }
                    else if (ratio > 1.0)
                    {
                        interpretation =
                            $"{request.Quantity1.Value} {request.Quantity1.Unit} is {ratio:F2} times larger than {request.Quantity2.Value} {request.Quantity2.Unit}";
                    }
                    else
                    {
                        var inverse = 1.0 / ratio;
                        interpretation =
                            $"{request.Quantity1.Value} {request.Quantity1.Unit} is {inverse:F2} times smaller than {request.Quantity2.Value} {request.Quantity2.Unit}";
                    }

                    // Save to repository
                    var entity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Divide,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        null,
                        ratio,
                        null,
                        interpretation,
                        true
                    );
                    _repository.Save(entity);

                    return DivisionResponse.SuccessResponse(ratio, interpretation);
                }
                catch (DivideByZeroException ex)
                {
                    _logger.LogWarning(ex, "Division by zero attempted");
                    var errorEntity = CreateErrorEntity(
                        OperationType.Divide,
                        request,
                        "Division by zero is not allowed"
                    );
                    _repository.Save(errorEntity);
                    return DivisionResponse.ErrorResponse("Division by zero is not allowed");
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported division operation");
                    var errorEntity = CreateErrorEntity(OperationType.Divide, request, ex.Message);
                    _repository.Save(errorEntity);
                    return DivisionResponse.ErrorResponse(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during division");
                    var quantityEx = new QuantityMeasurementException(
                        $"Division error: {ex.Message}",
                        ex
                    )
                    {
                        OperationType = "Divide",
                    };
                    throw quantityEx;
                }
            });
        }

        #endregion
    }
}
