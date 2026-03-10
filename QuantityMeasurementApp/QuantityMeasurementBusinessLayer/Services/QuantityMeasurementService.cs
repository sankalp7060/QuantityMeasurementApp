#nullable disable

using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Services;

namespace QuantityMeasurementBusinessLayer.Services
{
    /// <summary>
    /// Implementation of quantity measurement business logic.
    /// </summary>
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;
        private readonly IRedisCacheRepository _cacheRepository;
        private readonly ILogger<QuantityMeasurementService> _logger;

        public QuantityMeasurementService(
            ILogger<QuantityMeasurementService> logger,
            IRedisCacheRepository cacheRepository
        )
        {
            _repository = QuantityMeasurementCacheRepository.Instance;
            _logger = logger;
            _cacheRepository = cacheRepository;
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

        private object GetBaseUnit(string category)
        {
            return category.ToUpper() switch
            {
                "LENGTH" => LengthUnit.FEET,
                "WEIGHT" => WeightUnit.KILOGRAM,
                "VOLUME" => VolumeUnit.LITRE,
                "TEMPERATURE" => TemperatureUnit.CELSIUS,
                _ => null,
            };
        }

        private string GetUnitSymbol(object unit)
        {
            return unit.GetType().GetMethod("GetSymbol")?.Invoke(unit, null)?.ToString() ?? "";
        }

        private async Task CacheOperationResult(
            string operationType,
            object request,
            object response
        )
        {
            try
            {
                if (_cacheRepository == null)
                    return;

                var cacheKey = $"{operationType}:{DateTime.UtcNow.Ticks}:{Guid.NewGuid()}";

                var entity = new QuantityMeasurementEntity
                {
                    Id = cacheKey,
                    Timestamp = DateTime.UtcNow,
                    OperationType = operationType switch
                    {
                        "Compare" => OperationType.Compare,
                        "Convert" => OperationType.Convert,
                        "Add" => OperationType.Add,
                        "Subtract" => OperationType.Subtract,
                        "Divide" => OperationType.Divide,
                        _ => OperationType.Compare,
                    },
                    IsSuccess = true,
                    FormattedResult = response?.ToString(),
                };

                await _cacheRepository.SaveAsync(entity);
                _logger.LogDebug("Cached operation result with key: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache operation result");
            }
        }

        #endregion

        #region Public Single Operation Methods

        public async Task<QuantityResponse> CompareQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation(
                        "Comparing quantities: {Q1} vs {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Compare,
                            request.Quantity1.Value,
                            request.Quantity1.Unit,
                            request.Quantity1.Category,
                            request.Quantity2.Value,
                            request.Quantity2.Unit,
                            request.Quantity2.Category,
                            null,
                            null,
                            null,
                            null,
                            false,
                            categoryError
                        );
                        _repository.Save(errorEntity);

                        return QuantityResponse.ErrorResponse(
                            categoryError!,
                            OperationType.Compare
                        );
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Compare,
                            request.Quantity1.Value,
                            request.Quantity1.Unit,
                            request.Quantity1.Category,
                            request.Quantity2.Value,
                            request.Quantity2.Unit,
                            request.Quantity2.Category,
                            null,
                            null,
                            null,
                            null,
                            false,
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);

                        return QuantityResponse.ErrorResponse(
                            "Invalid quantity format",
                            OperationType.Compare
                        );
                    }

                    var equalsMethod = q1.GetType().GetMethod("Equals", new[] { typeof(object) });
                    var result = (bool)equalsMethod!.Invoke(q1, new[] { q2 })!;

                    var message = result
                        ? $"✅ {request.Quantity1.Value} {request.Quantity1.Unit} equals {request.Quantity2.Value} {request.Quantity2.Unit}"
                        : $"❌ {request.Quantity1.Value} {request.Quantity1.Unit} does not equal {request.Quantity2.Value} {request.Quantity2.Unit}";

                    var formattedResult =
                        $"{request.Quantity1.Value} {request.Quantity1.Unit} {(result ? "=" : "≠")} {request.Quantity2.Value} {request.Quantity2.Unit}";

                    var response = new QuantityResponse
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = message,
                        Result = result ? 1.0 : 0.0,
                        FormattedResult = formattedResult,
                        Operation = OperationType.Compare,
                        Timestamp = DateTime.UtcNow,
                    };

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

                    await CacheOperationResult("Compare", request, response);

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error comparing quantities");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Compare,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        null,
                        null,
                        null,
                        null,
                        false,
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(
                        $"Comparison error: {ex.Message}",
                        OperationType.Compare,
                        500
                    );
                }
            });
        }

        public async Task<QuantityResponse> ConvertQuantityAsync(ConversionRequest request)
        {
            return await Task.Run(async () =>
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
                        var errorEntity = QuantityMeasurementEntity.CreateConversion(
                            request.Source.Value,
                            request.Source.Unit,
                            request.Source.Category,
                            request.TargetUnit,
                            null,
                            null,
                            null,
                            false,
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
                        var errorEntity = QuantityMeasurementEntity.CreateConversion(
                            request.Source.Value,
                            request.Source.Unit,
                            request.Source.Category,
                            request.TargetUnit,
                            null,
                            null,
                            null,
                            false,
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
                        var errorEntity = QuantityMeasurementEntity.CreateConversion(
                            request.Source.Value,
                            request.Source.Unit,
                            request.Source.Category,
                            request.TargetUnit,
                            null,
                            null,
                            null,
                            false,
                            "Conversion failed"
                        );
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

                    var response = QuantityResponse.SuccessResponse(
                        resultValue,
                        resultUnitSymbol,
                        OperationType.Convert,
                        formattedResult
                    );

                    var entity = QuantityMeasurementEntity.CreateConversion(
                        request.Source.Value,
                        request.Source.Unit,
                        request.Source.Category,
                        request.TargetUnit,
                        resultValue,
                        resultUnitSymbol,
                        formattedResult,
                        true
                    );
                    _repository.Save(entity);

                    await CacheOperationResult("Convert", request, response);

                    return response;
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported conversion operation");
                    var errorEntity = QuantityMeasurementEntity.CreateConversion(
                        request.Source.Value,
                        request.Source.Unit,
                        request.Source.Category,
                        request.TargetUnit,
                        null,
                        null,
                        null,
                        false,
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(ex.Message, OperationType.Convert);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during conversion");
                    var errorEntity = QuantityMeasurementEntity.CreateConversion(
                        request.Source.Value,
                        request.Source.Unit,
                        request.Source.Category,
                        request.TargetUnit,
                        null,
                        null,
                        null,
                        false,
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(
                        $"Conversion error: {ex.Message}",
                        OperationType.Convert,
                        500
                    );
                }
            });
        }

        public async Task<QuantityResponse> AddQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation(
                        "Adding quantities: {Q1} + {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Add,
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
                            categoryError
                        );
                        _repository.Save(errorEntity);

                        return QuantityResponse.ErrorResponse(categoryError!, OperationType.Add);
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Add,
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
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);

                        return QuantityResponse.ErrorResponse(
                            "Invalid quantity format",
                            OperationType.Add
                        );
                    }

                    object targetUnit;
                    if (!string.IsNullOrEmpty(request.TargetUnit))
                    {
                        targetUnit = GetUnit(request.Quantity1.Category, request.TargetUnit);
                        if (targetUnit == null)
                        {
                            var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                                OperationType.Add,
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

                    var result = q1.GetType()
                        .GetMethod("Add", new[] { q2.GetType(), targetUnit.GetType() })!
                        .Invoke(q1, new[] { q2, targetUnit });

                    var resultValue = (double)
                        result.GetType().GetProperty("Value")!.GetValue(result)!;
                    var resultUnitSymbol = GetUnitSymbol(targetUnit);
                    var formattedResult =
                        $"{request.Quantity1.Value} {request.Quantity1.Unit} + {request.Quantity2.Value} {request.Quantity2.Unit} = {resultValue:F6} {resultUnitSymbol}";

                    var response = QuantityResponse.SuccessResponse(
                        resultValue,
                        resultUnitSymbol,
                        OperationType.Add,
                        formattedResult
                    );

                    var entity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Add,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        request.TargetUnit,
                        resultValue,
                        resultUnitSymbol,
                        formattedResult,
                        true
                    );
                    _repository.Save(entity);

                    await CacheOperationResult("Add", request, response);

                    return response;
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported addition operation");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Add,
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
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(ex.Message, OperationType.Add);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during addition");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Add,
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
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(
                        $"Addition error: {ex.Message}",
                        OperationType.Add,
                        500
                    );
                }
            });
        }

        public async Task<QuantityResponse> SubtractQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation(
                        "Subtracting quantities: {Q1} - {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Subtract,
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
                            categoryError
                        );
                        _repository.Save(errorEntity);

                        return QuantityResponse.ErrorResponse(
                            categoryError!,
                            OperationType.Subtract
                        );
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Subtract,
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
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);

                        return QuantityResponse.ErrorResponse(
                            "Invalid quantity format",
                            OperationType.Subtract
                        );
                    }

                    object targetUnit;
                    if (!string.IsNullOrEmpty(request.TargetUnit))
                    {
                        targetUnit = GetUnit(request.Quantity1.Category, request.TargetUnit);
                        if (targetUnit == null)
                        {
                            var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                                OperationType.Subtract,
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

                    var result = q1.GetType()
                        .GetMethod("Subtract", new[] { q2.GetType(), targetUnit.GetType() })!
                        .Invoke(q1, new[] { q2, targetUnit });

                    var resultValue = (double)
                        result.GetType().GetProperty("Value")!.GetValue(result)!;
                    var resultUnitSymbol = GetUnitSymbol(targetUnit);
                    var formattedResult =
                        $"{request.Quantity1.Value} {request.Quantity1.Unit} - {request.Quantity2.Value} {request.Quantity2.Unit} = {resultValue:F6} {resultUnitSymbol}";

                    var response = QuantityResponse.SuccessResponse(
                        resultValue,
                        resultUnitSymbol,
                        OperationType.Subtract,
                        formattedResult
                    );

                    var entity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Subtract,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        request.TargetUnit,
                        resultValue,
                        resultUnitSymbol,
                        formattedResult,
                        true
                    );
                    _repository.Save(entity);

                    await CacheOperationResult("Subtract", request, response);

                    return response;
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported subtraction operation");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Subtract,
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
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(ex.Message, OperationType.Subtract);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during subtraction");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Subtract,
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
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return QuantityResponse.ErrorResponse(
                        $"Subtraction error: {ex.Message}",
                        OperationType.Subtract,
                        500
                    );
                }
            });
        }

        public async Task<DivisionResponse> DivideQuantitiesAsync(BinaryQuantityRequest request)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation(
                        "Dividing quantities: {Q1} ÷ {Q2}",
                        $"{request.Quantity1.Value} {request.Quantity1.Unit}",
                        $"{request.Quantity2.Value} {request.Quantity2.Unit}"
                    );

                    if (
                        !ValidateSameCategory(
                            request.Quantity1,
                            request.Quantity2,
                            out var categoryError
                        )
                    )
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Divide,
                            request.Quantity1.Value,
                            request.Quantity1.Unit,
                            request.Quantity1.Category,
                            request.Quantity2.Value,
                            request.Quantity2.Unit,
                            request.Quantity2.Category,
                            null,
                            null,
                            null,
                            null,
                            false,
                            categoryError
                        );
                        _repository.Save(errorEntity);

                        return DivisionResponse.ErrorResponse(categoryError!);
                    }

                    var q1 = CreateQuantity(request.Quantity1);
                    var q2 = CreateQuantity(request.Quantity2);

                    if (q1 == null || q2 == null)
                    {
                        var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                            OperationType.Divide,
                            request.Quantity1.Value,
                            request.Quantity1.Unit,
                            request.Quantity1.Category,
                            request.Quantity2.Value,
                            request.Quantity2.Unit,
                            request.Quantity2.Category,
                            null,
                            null,
                            null,
                            null,
                            false,
                            "Invalid quantity format"
                        );
                        _repository.Save(errorEntity);

                        return DivisionResponse.ErrorResponse("Invalid quantity format");
                    }

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

                    var response = DivisionResponse.SuccessResponse(ratio, interpretation);

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

                    await CacheOperationResult("Divide", request, response);

                    return response;
                }
                catch (DivideByZeroException)
                {
                    _logger.LogWarning("Division by zero attempted");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Divide,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        null,
                        null,
                        null,
                        null,
                        false,
                        "Division by zero is not allowed"
                    );
                    _repository.Save(errorEntity);

                    return DivisionResponse.ErrorResponse("Division by zero is not allowed");
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Unsupported division operation");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Divide,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        null,
                        null,
                        null,
                        null,
                        false,
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return DivisionResponse.ErrorResponse(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during division");
                    var errorEntity = QuantityMeasurementEntity.CreateBinaryOperation(
                        OperationType.Divide,
                        request.Quantity1.Value,
                        request.Quantity1.Unit,
                        request.Quantity1.Category,
                        request.Quantity2.Value,
                        request.Quantity2.Unit,
                        request.Quantity2.Category,
                        null,
                        null,
                        null,
                        null,
                        false,
                        ex.Message
                    );
                    _repository.Save(errorEntity);

                    return DivisionResponse.ErrorResponse($"Division error: {ex.Message}");
                }
            });
        }

        #endregion

        #region Batch Operations

        public async Task<BatchResponse<QuantityResponse>> CompareQuantitiesBatchAsync(
            BatchBinaryRequest request
        )
        {
            return await Task.Run(async () =>
            {
                var results = new List<QuantityResponse>();
                int successCount = 0;
                int failureCount = 0;

                foreach (var item in request.Requests)
                {
                    try
                    {
                        var result = await CompareQuantitiesAsync(item);
                        results.Add(result);
                        if (result.Success)
                            successCount++;
                        else
                            failureCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in batch compare operation");
                        results.Add(
                            QuantityResponse.ErrorResponse(
                                $"Batch item error: {ex.Message}",
                                OperationType.Compare,
                                500
                            )
                        );
                        failureCount++;
                    }
                }

                var batchResponse = new BatchResponse<QuantityResponse>
                {
                    Success = failureCount == 0,
                    Message =
                        failureCount == 0
                            ? "All operations completed successfully"
                            : $"{failureCount} operation(s) failed",
                    Results = results,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Timestamp = DateTime.UtcNow,
                };

                await CacheOperationResult("CompareBatch", request, batchResponse);

                return batchResponse;
            });
        }

        public async Task<BatchResponse<QuantityResponse>> ConvertQuantitiesBatchAsync(
            BatchConversionRequest request
        )
        {
            return await Task.Run(async () =>
            {
                var results = new List<QuantityResponse>();
                int successCount = 0;
                int failureCount = 0;

                foreach (var item in request.Requests)
                {
                    try
                    {
                        var result = await ConvertQuantityAsync(item);
                        results.Add(result);
                        if (result.Success)
                            successCount++;
                        else
                            failureCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in batch convert operation");
                        results.Add(
                            QuantityResponse.ErrorResponse(
                                $"Batch item error: {ex.Message}",
                                OperationType.Convert,
                                500
                            )
                        );
                        failureCount++;
                    }
                }

                var batchResponse = new BatchResponse<QuantityResponse>
                {
                    Success = failureCount == 0,
                    Message =
                        failureCount == 0
                            ? "All operations completed successfully"
                            : $"{failureCount} operation(s) failed",
                    Results = results,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Timestamp = DateTime.UtcNow,
                };

                await CacheOperationResult("ConvertBatch", request, batchResponse);

                return batchResponse;
            });
        }

        public async Task<BatchResponse<QuantityResponse>> AddQuantitiesBatchAsync(
            BatchBinaryRequest request
        )
        {
            return await Task.Run(async () =>
            {
                var results = new List<QuantityResponse>();
                int successCount = 0;
                int failureCount = 0;

                foreach (var item in request.Requests)
                {
                    try
                    {
                        var result = await AddQuantitiesAsync(item);
                        results.Add(result);
                        if (result.Success)
                            successCount++;
                        else
                            failureCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in batch add operation");
                        results.Add(
                            QuantityResponse.ErrorResponse(
                                $"Batch item error: {ex.Message}",
                                OperationType.Add,
                                500
                            )
                        );
                        failureCount++;
                    }
                }

                var batchResponse = new BatchResponse<QuantityResponse>
                {
                    Success = failureCount == 0,
                    Message =
                        failureCount == 0
                            ? "All operations completed successfully"
                            : $"{failureCount} operation(s) failed",
                    Results = results,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Timestamp = DateTime.UtcNow,
                };

                await CacheOperationResult("AddBatch", request, batchResponse);

                return batchResponse;
            });
        }

        public async Task<BatchResponse<QuantityResponse>> SubtractQuantitiesBatchAsync(
            BatchBinaryRequest request
        )
        {
            return await Task.Run(async () =>
            {
                var results = new List<QuantityResponse>();
                int successCount = 0;
                int failureCount = 0;

                foreach (var item in request.Requests)
                {
                    try
                    {
                        var result = await SubtractQuantitiesAsync(item);
                        results.Add(result);
                        if (result.Success)
                            successCount++;
                        else
                            failureCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in batch subtract operation");
                        results.Add(
                            QuantityResponse.ErrorResponse(
                                $"Batch item error: {ex.Message}",
                                OperationType.Subtract,
                                500
                            )
                        );
                        failureCount++;
                    }
                }

                var batchResponse = new BatchResponse<QuantityResponse>
                {
                    Success = failureCount == 0,
                    Message =
                        failureCount == 0
                            ? "All operations completed successfully"
                            : $"{failureCount} operation(s) failed",
                    Results = results,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Timestamp = DateTime.UtcNow,
                };

                await CacheOperationResult("SubtractBatch", request, batchResponse);

                return batchResponse;
            });
        }

        public async Task<BatchResponse<DivisionResponse>> DivideQuantitiesBatchAsync(
            BatchBinaryRequest request
        )
        {
            return await Task.Run(async () =>
            {
                var results = new List<DivisionResponse>();
                int successCount = 0;
                int failureCount = 0;

                foreach (var item in request.Requests)
                {
                    try
                    {
                        var result = await DivideQuantitiesAsync(item);
                        results.Add(result);
                        if (result.Success)
                            successCount++;
                        else
                            failureCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in batch divide operation");
                        results.Add(
                            DivisionResponse.ErrorResponse($"Batch item error: {ex.Message}")
                        );
                        failureCount++;
                    }
                }

                var batchResponse = new BatchResponse<DivisionResponse>
                {
                    Success = failureCount == 0,
                    Message =
                        failureCount == 0
                            ? "All operations completed successfully"
                            : $"{failureCount} operation(s) failed",
                    Results = results,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Timestamp = DateTime.UtcNow,
                };

                await CacheOperationResult("DivideBatch", request, batchResponse);

                return batchResponse;
            });
        }

        #endregion

        #region Cache Management Methods

        public async Task<bool> ClearCacheAsync()
        {
            try
            {
                await _cacheRepository.ClearAllAsync();
                _logger.LogInformation("Cache cleared successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache");
                return false;
            }
        }

        public async Task<CacheStatsResponse> GetCacheStatisticsAsync()
        {
            try
            {
                var isConnected = await _cacheRepository.IsConnectedAsync();
                var allItems = await _cacheRepository.GetAllAsync();

                return new CacheStatsResponse
                {
                    IsRedisConnected = isConnected,
                    CacheItemCount = allItems?.Count ?? 0,
                    Timestamp = DateTime.UtcNow,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache stats");
                return new CacheStatsResponse
                {
                    IsRedisConnected = false,
                    CacheItemCount = 0,
                    Timestamp = DateTime.UtcNow,
                };
            }
        }

        #endregion
    }
}
