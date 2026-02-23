# 📏 Quantity Measurement Application

A comprehensive .NET console application for performing measurements, conversions, and arithmetic operations across multiple physical quantities including Length, Weight, Volume, and Temperature. Built with clean architecture, SOLID principles, and extensive test coverage.

---

## 📋 Table of Contents
- [Overview](#overview)
- [Features by Use Case](#features-by-use-case)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage Guide](#usage-guide)
- [API Reference](#api-reference)
- [Testing](#testing)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

The Quantity Measurement Application is a feature-rich console application that demonstrates progressive software development through 14 use cases. It starts with basic measurement equality and evolves into a sophisticated, generic measurement system supporting multiple physical quantities with full arithmetic operations.

**Key Highlights:**
- ✅ 4 measurement categories (Length, Weight, Volume, Temperature)
- ✅ 13 different units
- ✅ Full arithmetic operations (addition, subtraction, division)
- ✅ Selective operation support (temperature constraints)
- ✅ Clean, generic architecture
- ✅ 275+ passing unit tests
- ✅ SOLID principles adherence
- ✅ DRY (Don't Repeat Yourself) compliance

---

## 🚀 Features by Use Case

### **UC1: Feet Measurement Equality**
*Basic building block for measurement comparison*

```csharp
var feet1 = new Feet(1.0);
var feet2 = new Feet(1.0);
bool areEqual = feet1.Equals(feet2); // true
```

**Features:**
- Equality comparison for feet measurements
- Proper null checking
- Reflexive, symmetric, transitive properties
- Value-based equality

---

### **UC2: Inch Measurement Equality**
*Extending to multiple units*

```csharp
var inch1 = new Inch(1.0);
var inch2 = new Inch(1.0);
bool areEqual = inch1.Equals(inch2); // true
```

**Features:**
- Inch class with same equality contract as Feet
- Demonstrates pattern replication
- **Note:** This introduces code duplication (addressed in UC3)

---

### **UC3: Generic Quantity Class**
*Eliminating code duplication with generics*

```csharp
public class Quantity
{
    private readonly double _value;
    private readonly LengthUnit _unit;
    
    public Quantity(double value, LengthUnit unit) { ... }
    public override bool Equals(object? obj) { ... }
}
```

**Features:**
- Single class replaces both Feet and Inch
- Uses enum for unit type
- DRY principle implementation
- Value-based equality across units

**Supported Units:** FEET, INCH

---

### **UC4: Extended Unit Support**
*Adding Yards and Centimeters*

```csharp
public enum LengthUnit
{
    FEET,       // Base unit
    INCH,       // 1 ft = 12 in
    YARD,       // 1 yd = 3 ft
    CENTIMETER  // 1 cm = 0.393701 in
}
```

**Features:**
- YARD unit (conversion factor: 3.0)
- CENTIMETER unit (conversion factor: 0.0328084)
- Cross-unit equality (1 yd = 3 ft = 36 in = 91.44 cm)
- Scalable enum design

---

### **UC5: Unit Conversion**
*Converting between different units*

```csharp
var feet = new Quantity(1.0, LengthUnit.FEET);
var inches = feet.ConvertTo(LengthUnit.INCH); // 12.0 in
var yards = feet.ConvertTo(LengthUnit.YARD);   // 0.333 yd
```

**Features:**
- `ConvertTo()` instance method
- `Convert()` static method
- Base unit normalization
- Bidirectional conversion
- Round-trip preservation

**Conversion Factors:**
| From | To | Formula |
|------|-----|---------|
| FEET | INCH | × 12 |
| INCH | FEET | ÷ 12 |
| YARD | FEET | × 3 |
| FEET | YARD | ÷ 3 |
| CM | INCH | × 0.393701 |

---

### **UC6: Addition Operations**
*Adding measurements with default unit*

```csharp
var feet = new Quantity(1.0, LengthUnit.FEET);
var inches = new Quantity(12.0, LengthUnit.INCH);
var sum = feet.Add(inches); // 2.0 FEET (result in first unit)
```

**Features:**
- `Add()` method (implicit target unit)
- Result in first operand's unit
- Cross-unit addition
- Immutability (returns new object)

**Examples:**
| Operation | Result |
|-----------|--------|
| 1 ft + 2 ft | 3 ft |
| 1 ft + 12 in | 2 ft |
| 12 in + 1 ft | 24 in |

---

### **UC7: Addition with Target Unit**
*Flexible result unit specification*

```csharp
var feet = new Quantity(1.0, LengthUnit.FEET);
var inches = new Quantity(12.0, LengthUnit.INCH);
var sum = feet.Add(inches, LengthUnit.YARD); // 0.667 yd
```

**Features:**
- Overloaded `Add()` with target unit parameter
- Result in any supported unit
- Method overloading for API flexibility

**Examples:**
| Operation | Target | Result |
|-----------|--------|--------|
| 1 ft + 12 in | FEET | 2 ft |
| 1 ft + 12 in | INCH | 24 in |
| 1 ft + 12 in | YARD | 0.667 yd |

---

### **UC8: Standalone Unit Enum**
*Separating unit responsibility*

```csharp
public enum LengthUnit
{
    FEET, INCH, YARD, CENTIMETER
}

public static class LengthUnitExtensions
{
    public static double ToBaseUnit(this LengthUnit unit, double value)
    {
        return value * GetConversionFactor(unit);
    }
    
    public static double FromBaseUnit(this LengthUnit unit, double baseValue)
    {
        return baseValue / GetConversionFactor(unit);
    }
}
```

**Features:**
- Unit enum extracted to standalone
- Conversion logic moved to unit
- Single Responsibility Principle
- No circular dependencies

---

### **UC9: Weight Measurements**
*Adding a second measurement category*

```csharp
public enum WeightUnit
{
    KILOGRAM,   // Base unit
    GRAM,       // 1 kg = 1000 g
    POUND       // 1 lb ≈ 0.453592 kg
}

var kg = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
var g = new WeightQuantity(1000.0, WeightUnit.GRAM);
var equal = kg.Equals(g); // true
```

**Features:**
- WeightUnit enum
- WeightQuantity class
- All operations (equality, conversion, addition)
- Parallel structure to Length

**Weight Units:**
| Unit | Symbol | Conversion to kg |
|------|--------|------------------|
| KILOGRAM | kg | 1.0 |
| GRAM | g | 0.001 |
| POUND | lb | 0.45359237 |

---

### **UC10: Generic Quantity with IMeasurable**
*Unified architecture for all measurements*

```csharp
public interface IMeasurable
{
    double GetConversionFactor();
    double ToBaseUnit(double value);
    double FromBaseUnit(double baseValue);
    string GetSymbol();
    string GetName();
}

public class GenericQuantity<T> where T : class, IMeasurable
{
    private readonly double _value;
    private readonly T _unit;
    
    public GenericQuantity<T> Add(GenericQuantity<T> other) { ... }
    public GenericQuantity<T> ConvertTo(T targetUnit) { ... }
    public override bool Equals(object? obj) { ... }
}
```

**Features:**
- Generic class replaces all category-specific classes
- Type-safe operations
- Compile-time category checking
- Single implementation for all measurements

**Benefits:**
- ✅ No code duplication
- ✅ Type safety across categories
- ✅ Easy to add new categories
- ✅ Consistent behavior

---

### **UC11: Volume Measurements**
*Third category demonstrating scalability*

```csharp
public class VolumeUnit : IMeasurable
{
    public static readonly VolumeUnit LITRE = new("litres", "L", 1.0);
    public static readonly VolumeUnit MILLILITRE = new("millilitres", "mL", 0.001);
    public static readonly VolumeUnit GALLON = new("gallons", "gal", 3.78541);
}

var litre = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
var ml = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
var equal = litre.Equals(ml); // true
```

**Volume Units:**
| Unit | Symbol | Conversion to L |
|------|--------|-----------------|
| LITRE | L | 1.0 |
| MILLILITRE | mL | 0.001 |
| GALLON | gal | 3.78541 |

**Key Achievement:** Added without modifying GenericQuantity class!

---

### **UC12: Subtraction and Division**
*Complete arithmetic operations*

```csharp
var a = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
var b = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);

// Subtraction
var diff = a.Subtract(b);              // 5.0 FEET
var diffInInches = a.Subtract(b, LengthUnit.INCH); // 60.0 INCH

// Division
var ratio = a.Divide(b);                // 2.0 (dimensionless)
```

**Features:**
- `Subtract()` with/without target unit
- `Divide()` returning dimensionless ratio
- Non-commutative operations
- Division by zero protection

**Examples:**
| Operation | Result |
|-----------|--------|
| 10 ft - 5 ft | 5 ft |
| 10 ft - 6 in | 9.5 ft |
| 10 ft - 6 in (in inches) | 114 in |
| 10 ft ÷ 2 ft | 5.0 |
| 24 in ÷ 2 ft | 1.0 |

---

### **UC13: Centralized Arithmetic Logic**
*DRY principle for arithmetic operations*

```csharp
private enum ArithmeticOperation { ADD, SUBTRACT, DIVIDE }

private double PerformBaseArithmetic(GenericQuantity<T> other, ArithmeticOperation operation)
{
    double thisInBase = _unit.ToBaseUnit(_value);
    double otherInBase = other._unit.ToBaseUnit(other._value);
    
    return operation switch
    {
        ArithmeticOperation.ADD => thisInBase + otherInBase,
        ArithmeticOperation.SUBTRACT => thisInBase - otherInBase,
        ArithmeticOperation.DIVIDE when Math.Abs(otherInBase) < 0.000000001 
            => throw new DivideByZeroException(),
        ArithmeticOperation.DIVIDE => thisInBase / otherInBase,
        _ => throw new InvalidOperationException()
    };
}
```

**Benefits:**
- Single source of truth for arithmetic
- Centralized validation
- Easy to add new operations
- Consistent error handling
- Reduced code duplication

---

### **UC14: Temperature with Selective Arithmetic**
*Handling special measurement constraints*

```csharp
public class TemperatureUnit : IMeasurable
{
    public static readonly TemperatureUnit CELSIUS = new(
        "Celsius", "°C",
        toBaseUnit: c => c,
        fromBaseUnit: c => c);
        
    public static readonly TemperatureUnit FAHRENHEIT = new(
        "Fahrenheit", "°F",
        toBaseUnit: f => (f - 32) * 5 / 9,
        fromBaseUnit: c => (c * 9 / 5) + 32);
        
    public static readonly TemperatureUnit KELVIN = new(
        "Kelvin", "K",
        toBaseUnit: k => k - 273.15,
        fromBaseUnit: c => c + 273.15);
        
    public void ValidateOperationSupport(string operation)
    {
        throw new NotSupportedException(
            $"Temperature does not support {operation} operations");
    }
}
```

**Features:**
- Non-linear conversions using formulas
- **No arithmetic support** (throws NotSupportedException)
- ISupportsArithmetic interface
- Default methods for backward compatibility

**Temperature Equivalents:**
| Celsius | Fahrenheit | Kelvin |
|---------|------------|--------|
| 0°C | 32°F | 273.15K |
| 100°C | 212°F | 373.15K |
| -40°C | -40°F | 233.15K |
| -273.15°C | -459.67°F | 0K |

**Why No Arithmetic?**
- Adding absolute temperatures is meaningless (10°C + 20°C ≠ 30°C)
- Subtraction gives temperature difference, not temperature
- Division yields meaningless ratios

---

## 🏗 Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────┐
│            UI (Console)              │
│  └ Menus, Helpers, User Interaction  │
├─────────────────────────────────────┤
│           Services                    │
│  └ GenericMeasurementService          │
├─────────────────────────────────────┤
│            Domain                     │
│  ├── Quantities (GenericQuantity)    │
│  └── Units (Length, Weight, Volume,  │
│             Temperature)              │
├─────────────────────────────────────┤
│            Core                       │
│  ├── Abstractions (IMeasurable)      │
│  └── Exceptions                       │
└─────────────────────────────────────┘
```

### SOLID Principles Implementation

| Principle | Implementation |
|-----------|----------------|
| **S**ingle Responsibility | Each class has one reason to change |
| **O**pen/Closed | New units added without modifying Quantity class |
| **L**iskov Substitution | Any IMeasurable works with GenericQuantity |
| **I**nterface Segregation | IMeasurable has minimal, focused methods |
| **D**ependency Inversion | High-level modules depend on abstractions |

---

## 💻 Installation

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/) (optional)

### Steps

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/QuantityMeasurementApp.git
cd QuantityMeasurementApp
```

2. **Build the solution**
```bash
dotnet build
```

3. **Run the application**
```bash
dotnet run --project QuantityMeasurementApp
```

4. **Run tests**
```bash
dotnet test
```

---

## 🎮 Usage Guide

### Main Menu
```
╔════════════════════════════════════════════════════════╗
║                    MAIN MENU                          ║
╠════════════════════════════════════════════════════════╣
║                                                        ║
║    1.  Length Measurements (ft, in, yd, cm)           ║
║    2.  Weight Measurements (kg, g, lb)                ║
║    3.  Volume Measurements (L, mL, gal)               ║
║    4.  Temperature Measurements (°C, °F, K)           ║
║    5.  Exit                                           ║
╚════════════════════════════════════════════════════════╝
```

### Length Operations Example
```
┌───── LENGTH MEASUREMENTS ─────┐

1.  Convert Length Units
    (e.g., 1 ft = 12 in)

2.  Compare Lengths
    (e.g., 1 ft = 12 in = 0.333 yd)

3.  Arithmetic Operations
    (Add, Subtract, Divide)

Enter your choice: 1

Select SOURCE unit:
  1. Feet (ft)
  2. Inches (in)
  3. Yards (yd)
  4. Centimeters (cm)
Enter choice (1-4): 1

Select TARGET unit:
  1. Feet (ft)
  2. Inches (in)
  3. Yards (yd)
  4. Centimeters (cm)
Enter choice (1-4): 2

Enter value in feet: 5

╔════════════════════════════════════════╗
║         CONVERSION RESULT             ║
╠════════════════════════════════════════╣
║  5.000 ft =   60.000000 in            ║
╚════════════════════════════════════════╝
```

### Temperature Special Handling
```
┌───── TEMPERATURE ARITHMETIC ─────┐

  Temperature 1: 100 °C
  Temperature 2: 50 °C

  ┌──────────────────────┬────────────────────────────────┐
  │ Operation             │ Result                         │
  ├──────────────────────┼────────────────────────────────┤
  │ Add (temp1 + temp2)   │ ❌ NOT SUPPORTED               │
  │ Subtract (temp1 - temp2)│ ❌ NOT SUPPORTED             │
  │ Divide (temp1 ÷ temp2) │ ❌ NOT SUPPORTED              │
  └──────────────────────┴────────────────────────────────┘

  ✅ Temperature supports:
     • Equality comparison
     • Unit conversion
```

---

## 📚 API Reference

### Core Classes

#### `GenericQuantity<T>` where T : IMeasurable
```csharp
// Constructor
public GenericQuantity(double value, T unit)

// Properties
public double Value { get; }
public T Unit { get; }

// Conversion
public GenericQuantity<T> ConvertTo(T targetUnit)
public double ConvertToDouble(T targetUnit)

// Equality
public override bool Equals(object? obj)
public override int GetHashCode()

// Addition
public GenericQuantity<T> Add(GenericQuantity<T> other)
public GenericQuantity<T> Add(GenericQuantity<T> other, T targetUnit)

// Subtraction
public GenericQuantity<T> Subtract(GenericQuantity<T> other)
public GenericQuantity<T> Subtract(GenericQuantity<T> other, T targetUnit)

// Division
public double Divide(GenericQuantity<T> other)

// Static helpers
public static GenericQuantity<T> Add(double v1, T u1, double v2, T u2, T target)
public static GenericQuantity<T> Subtract(double v1, T u1, double v2, T u2, T target)
public static double Divide(double v1, T u1, double v2, T u2)
```

#### `IMeasurable` Interface
```csharp
public interface IMeasurable
{
    double GetConversionFactor();
    double ToBaseUnit(double value);
    double FromBaseUnit(double baseValue);
    string GetSymbol();
    string GetName();
    
    // Default methods (UC14)
    bool SupportsArithmeticOperation() => true;
    void ValidateOperationSupport(string operation) { }
}
```

### Unit Classes

#### `LengthUnit`
```csharp
public static readonly LengthUnit FEET;
public static readonly LengthUnit INCH;
public static readonly LengthUnit YARD;
public static readonly LengthUnit CENTIMETER;

// Base unit: FEET
// All arithmetic operations supported
```

#### `WeightUnit`
```csharp
public static readonly WeightUnit KILOGRAM;
public static readonly WeightUnit GRAM;
public static readonly WeightUnit POUND;

// Base unit: KILOGRAM
// All arithmetic operations supported
```

#### `VolumeUnit`
```csharp
public static readonly VolumeUnit LITRE;
public static readonly VolumeUnit MILLILITRE;
public static readonly VolumeUnit GALLON;

// Base unit: LITRE
// All arithmetic operations supported
```

#### `TemperatureUnit`
```csharp
public static readonly TemperatureUnit CELSIUS;
public static readonly TemperatureUnit FAHRENHEIT;
public static readonly TemperatureUnit KELVIN;

// Base unit: CELSIUS
// Arithmetic operations NOT supported
// Conversions use formulas, not simple factors
```

---

## 🧪 Testing

### Test Statistics
- **Total Tests:** 275+
- **Test Categories:** 7
- **Coverage:** Near 100% of core functionality

### Test Categories

| Category | Files | Tests |
|----------|-------|-------|
| Unit Tests | `LengthUnitTests.cs`, `WeightUnitTests.cs`, etc. | 50+ |
| Equality Tests | `GenericQuantityEqualityTests.cs` | 40+ |
| Conversion Tests | `GenericQuantityConversionTests.cs` | 35+ |
| Arithmetic Tests | `GenericQuantityArithmeticTests.cs` | 45+ |
| Subtraction Tests | `GenericQuantitySubtractionTests.cs` | 30+ |
| Division Tests | `GenericQuantityDivisionTests.cs` | 25+ |
| Edge Cases | `GenericQuantityEdgeCasesTests.cs` | 20+ |
| Integration | `EndToEndTests.cs`, `TemperatureEndToEndTests.cs` | 30+ |

### Sample Test
```csharp
[TestMethod]
public void Add_Length_CrossUnit_ResultInFirstUnit_ReturnsCorrectSum()
{
    // Arrange
    var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
    var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

    // Act
    var sumLength = feetLength.Add(inchesLength);

    // Assert
    Assert.AreEqual(2.0, sumLength.Value, 0.000001);
    Assert.AreEqual(LengthUnit.FEET, sumLength.Unit);
}
```

---

## 📁 Project Structure

```
QuantityMeasurementApp/
│
├── QuantityMeasurementApp.sln
│
├── QuantityMeasurementApp/                # Main application
│   ├── Core/                              # Core abstractions
│   │   ├── Abstractions/
│   │   │   └── IMeasurable.cs
│   │   └── Exceptions/
│   │       ├── InvalidUnitException.cs
│   │       └── InvalidValueException.cs
│   │
│   ├── Domain/                             # Business logic
│   │   ├── Quantities/
│   │   │   └── GenericQuantity.cs
│   │   └── Units/
│   │       ├── LengthUnit.cs
│   │       ├── WeightUnit.cs
│   │       ├── VolumeUnit.cs
│   │       └── TemperatureUnit.cs
│   │
│   ├── Services/                           # Service layer
│   │   └── GenericMeasurementService.cs
│   │
│   ├── UI/                                  # User interface
│   │   ├── Helpers/
│   │   │   ├── ConsoleHelper.cs
│   │   │   └── GenericUnitSelector.cs
│   │   └── Menus/
│   │       ├── MainMenu.cs
│   │       ├── GenericLengthMenu.cs
│   │       ├── GenericWeightMenu.cs
│   │       ├── GenericVolumeMenu.cs
│   │       └── GenericTemperatureMenu.cs
│   │
│   └── Utils/                               # Utilities
│       └── Validators/
│           └── InputValidator.cs
│
└── QuantityMeasurementApp.Tests/            # Test project
    ├── ArchitectureTests/
    ├── DomainTests/
    │   ├── GenericQuantityTests/
    │   └── UnitTests/
    ├── IntegrationTests/
    ├── ServiceTests/
    └── TestHelpers/
```

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow existing code style
- Add tests for new features
- Ensure all tests pass
- Update documentation as needed

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 🙏 Acknowledgments

- Built with .NET 8.0
- Testing with MSTest
- Inspired by clean architecture principles
- Thanks to all contributors

---

## 📊 Summary

| Metric | Value |
|--------|-------|
| Use Cases | 14 |
| Measurement Categories | 4 |
| Total Units | 13 |
| Lines of Code | ~1100 |
| Test Files | 20+ |
| Total Tests | 275+ |
| Build Status | ✅ Passing |

---

*Last Updated: February 2025*
