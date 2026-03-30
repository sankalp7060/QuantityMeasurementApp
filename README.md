The user wants a merged README.md. This is a pure text/markdown task — no need for computer tools. I'll write the merged README directly.

Here's the merged, deduplicated README.md:

```markdown
# Quantity Measurement System - Complete Technical Documentation

...
```

Let me write it out cleanly:

---

````markdown
# Quantity Measurement System — Complete Technical Documentation

## 📋 Table of Contents
1. [Project Overview](#-project-overview)
2. [System Architecture](#-system-architecture)
3. [Core Features by Use Case (UC1–UC15)](#-core-features-by-use-case-uc1uc15)
4. [Layer Architecture Deep Dive](#-layer-architecture-deep-dive)
5. [Design Patterns Implementation](#-design-patterns-implementation)
6. [SOLID Principles Applied](#-solid-principles-applied)
7. [Authentication & Authorization](#-authentication--authorization)
8. [JWT Token Management](#jwt-token-management)
9. [Google OAuth 2.0 Integration](#google-oauth-20-integration)
10. [Refresh Token Mechanism](#refresh-token-mechanism)
11. [Role-Based Access Control (RBAC)](#role-based-access-control-rbac)
12. [Account Lockout & Rate Limiting](#account-lockout--rate-limiting)
13. [Password Hashing with BCrypt](#password-hashing-with-bcrypt)
14. [Security Implementation](#-security-implementation)
15. [Database Schema & Migrations](#-database-schema--migrations)
16. [Data Flow & State Management](#-data-flow--state-management)
17. [Validation System Architecture](#-validation-system-architecture)
18. [Exception Handling Strategy](#-exception-handling-strategy)
19. [Persistence Mechanism](#-persistence-mechanism)
20. [Unit Testing Strategy](#-unit-testing-strategy)
21. [Performance Considerations](#-performance-considerations)
22. [User Interface Design](#-user-interface-design)
23. [API Endpoints Reference](#-api-endpoints-reference)
24. [Project Structure](#-project-structure)
25. [Technology Stack](#-technology-stack)
26. [How to Run & Build](#-how-to-run--build)
27. [Troubleshooting Guide](#-troubleshooting-guide)
28. [Future Roadmap](#-future-roadmap)
29. [Project Metrics Summary](#-project-metrics-summary)

---

## 🎯 Project Overview

The **Quantity Measurement System** is a comprehensive, production-grade application that evolved through 15 use cases, transforming from a simple feet comparison tool into a full-fledged N-Tier architecture system. It handles measurement operations across multiple unit categories with complete business logic, data persistence, secure REST API endpoints, and a console UI.

### Key Philosophical Principles

- **Separation of Concerns** — Each layer has a single responsibility
- **DRY (Don't Repeat Yourself)** — Centralized logic in the business layer
- **KISS (Keep It Simple, Stupid)** — Clean, readable code
- **YAGNI (You Aren't Gonna Need It)** — Only implement required features
- **SOLID** — All five principles strictly followed
- **Design Patterns** — 7+ patterns implemented for robust architecture
- **Security First** — JWT, BCrypt, OAuth2, and rate limiting

---

## 🏗 System Architecture

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                            │
│          Console UI + API Controllers (WebAPI)                       │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  AuthController        │  QuantitiesController  │ Admin    │    │
│  │  • Register/Login      │  • Convert             │ Controller│    │
│  │  • Google OAuth        │  • Compare             │ • User    │    │
│  │  • Refresh Token       │  • Add/Subtract/Divide │   Mgmt   │    │
│  │  MainConsoleMenu       │  MeasurementMenu       │ • Roles  │    │
│  └─────────────────────────────────────────────────────────────┘    │
├─────────────────────────────────────────────────────────────────────┤
│                        BUSINESS LAYER                                │
│                    QuantityMeasurementBusinessLayer                  │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  IAuthService               │  IQuantityMeasurementService  │    │
│  │  • Register/Login logic     │  • Core business logic        │    │
│  │  • Token generation         │  • Arithmetic operations      │    │
│  │  • Password hashing         │  • Validation rules           │    │
│  │  • Account lockout          │  • Exception handling         │    │
│  └─────────────────────────────────────────────────────────────┘    │
├─────────────────────────────────────────────────────────────────────┤
│                        REPOSITORY LAYER                              │
│                   QuantityMeasurementRepositoryLayer                 │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  IAuthRepository            │  IQuantityMeasurementRepo     │    │
│  │  • User CRUD                │  • Measurement CRUD           │    │
│  │  • Refresh token management │  • History queries            │    │
│  │  • Token revocation         │  • Statistics aggregation     │    │
│  └─────────────────────────────────────────────────────────────┘    │
├─────────────────────────────────────────────────────────────────────┤
│                          MODEL LAYER                                  │
│                     QuantityMeasurementModelLayer                     │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  DTOs              │  Entities        │  Enums              │    │
│  │  • AuthDTOs        │  • UserEntity    │  • OperationType    │    │
│  │  • MeasurementDTOs │  • RefreshToken  │                     │    │
│  │  • ResponseDTOs    │    Entity        │                     │    │
│  └─────────────────────┴──────────────────┴─────────────────────┘    │
├─────────────────────────────────────────────────────────────────────┤
│                         CORE DOMAIN LAYER                              │
│                        QuantityMeasurementApp                         │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  GenericQuantity<T>  │  Unit Classes     │  IMeasurable    │    │
│  │  • Type safety       │  • LengthUnit     │  • Interface    │    │
│  │  • Immutability      │  • WeightUnit     │  • Contract     │    │
│  │  • Value equality    │  • VolumeUnit     │                 │    │
│  │                      │  • TemperatureUnit│                 │    │
│  └──────────────────────┴───────────────────┴─────────────────┘    │
└─────────────────────────────────────────────────────────────────────┘
```

### Layer Communication Flow

```
User Input → Presentation Layer → Factory → Service Layer → Repository Layer → Model Layer
      ↑              ↓               ↓            ↓               ↓                ↓
      └──────────────┴───────────────┴────────────┴───────────────┴────────────────┘
                                   Response Flow
```

---

## 📚 Core Features by Use Case (UC1–UC22)

| Use Case | Feature |
|----------|---------|
| UC1 | Feet measurement equality (custom `Equals`, `GetHashCode`) |
| UC2 | Inch measurement equality + cross-unit equality (1 ft = 12 in) |
| UC3 | Generic `Quantity<T>` class — eliminated 90% code duplication (DRY) |
| UC4 | Yards & Centimeters support |
| UC5 | Bidirectional unit conversion with round-trip precision |
| UC6 | Addition of quantities in base unit, result in first operand's unit |
| UC7 | Addition with explicit target unit (3 method overloads) |
| UC8 | Standalone `IMeasurable` interface; units hold conversion responsibility |
| UC9 | Weight measurements: Kilogram, Gram, Pound |
| UC10 | `GenericQuantity<T>` with `IMeasurable`; compile-time cross-category prevention |
| UC11 | Volume measurements: Litre, Millilitre, Gallon |
| UC12 | Subtraction and dimensionless division |
| UC13 | Centralized arithmetic via `ArithmeticOperation` enum + pattern matching |
| UC14 | Temperature (°C, °F, K) — conversion only; arithmetic throws `NotSupportedException` |
| UC15 | Full N-Tier architecture across 6 projects |
| UC16 | Database integration using ADO.NET + in-memory caching for performance optimization |
| UC17 | ASP.NET Web API layer for exposing application services |
| UC18 | Authentication & Authorization using JWT and OAuth |
| UC19 | Frontend implementation using HTML, CSS, and JavaScript |
| UC20 | Modern frontend using React (component-based architecture) |
| UC21 | Microservices architecture — independent, scalable services |
| UC22 | Deployment pipeline — application hosting, CI/CD, and environment configuration |

### Unit Conversion Reference

| Unit | To Base |
|------|---------|
| Yard | 3.0 ft |
| Centimeter | 1/(2.54×12) ft |
| Gram | 0.001 kg |
| Pound | 0.45359237 kg |
| Millilitre | 0.001 L |
| Gallon | 3.78541 L |
| Fahrenheit | (°C × 9/5) + 32 |
| Kelvin | °C + 273.15 |

---

## 🧩 Layer Architecture Deep Dive

### Model Layer — DTOs & Entities

```csharp
// Input contract
public class QuantityDTO
{
    [Required] public double Value { get; set; }
    [Required] public string Unit { get; set; }
    [Required] public string Category { get; set; }
}

// Binary operation contract
public class BinaryQuantityRequest
{
    public QuantityDTO Quantity1 { get; set; }
    public QuantityDTO Quantity2 { get; set; }
    public string? TargetUnit { get; set; }
}

// Standard output
public class QuantityResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public double? Result { get; set; }
    public string? ResultUnit { get; set; }
    public string? FormattedResult { get; set; }
    public OperationType Operation { get; set; }
    public DateTime Timestamp { get; set; }

    public static QuantityResponse SuccessResponse(...) { }
    public static QuantityResponse ErrorResponse(...) { }
}
```

### Repository Layer — Singleton Cache

```csharp
public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
{
    private static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
        new Lazy<QuantityMeasurementCacheRepository>(() => new QuantityMeasurementCacheRepository());

    public static QuantityMeasurementCacheRepository Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, QuantityMeasurementEntity> _storage;
    private readonly object _fileLock = new object();

    private QuantityMeasurementCacheRepository()
    {
        _storage = new ConcurrentDictionary<string, QuantityMeasurementEntity>();
        LoadFromDisk();
    }
}
```

### Business Layer — Service Contract

```csharp
public interface IQuantityMeasurementService
{
    Task<QuantityResponse> CompareQuantitiesAsync(BinaryQuantityRequest request);
    Task<QuantityResponse> ConvertQuantityAsync(ConversionRequest request);
    Task<QuantityResponse> AddQuantitiesAsync(BinaryQuantityRequest request);
    Task<QuantityResponse> SubtractQuantitiesAsync(BinaryQuantityRequest request);
    Task<DivisionResponse> DivideQuantitiesAsync(BinaryQuantityRequest request);
}
```

**Operation Flow Pattern (each method):**
1. Log start → 2. Validate inputs → 3. Create domain objects → 4. Perform logic → 5. Create entity → 6. Persist to repository → 7. Return response

### Presentation Layer — Factory

```csharp
public class ServiceFactory
{
    private readonly IQuantityMeasurementRepository _repository;
    private readonly IQuantityMeasurementService _service;

    public ServiceFactory(ILoggerFactory loggerFactory)
    {
        _repository = QuantityMeasurementCacheRepository.Instance;
        _service = new QuantityMeasurementService(
            loggerFactory.CreateLogger<QuantityMeasurementService>(), _repository);
    }

    public IQuantityMeasurementService GetService() => _service;
    public IQuantityMeasurementRepository GetRepository() => _repository;
}
```

---

## 🏭 Design Patterns Implementation

| Pattern | Location | Purpose |
|---------|----------|---------|
| **Singleton** | Repository | Single source of truth, thread-safe via `Lazy<T>` |
| **Factory** | Presentation | Centralized dependency creation & injection |
| **Facade** | Controller/Menu | Simplifies subsystem complexity for UI |
| **Adapter** | Legacy Integration | Reuses UC1–14 console code in N-Tier shell |
| **Strategy** | Domain | Encapsulates `ADD`/`SUBTRACT`/`DIVIDE` arithmetic |
| **Repository** | Data Access | Abstracts persistence; mockable for testing |
| **Dependency Injection** | Throughout | Loose coupling; testable with mocks |

### Strategy Pattern — Centralized Arithmetic

```csharp
private double PerformBaseArithmetic(Quantity<T> other, ArithmeticOperation operation)
{
    double thisInBase  = _unit.ToBaseUnit(_value);
    double otherInBase = other._unit.ToBaseUnit(other._value);

    return operation switch
    {
        ArithmeticOperation.ADD      => thisInBase + otherInBase,
        ArithmeticOperation.SUBTRACT => thisInBase - otherInBase,
        ArithmeticOperation.DIVIDE   => thisInBase / otherInBase,
        _ => throw new InvalidOperationException()
    };
}
```

---

## 📐 SOLID Principles Applied

### Single Responsibility (SRP)
Each project/class has exactly one reason to change:

| Component | Responsibility |
|-----------|----------------|
| `MeasurementMenu` | UI display & input only |
| `QuantityMeasurementService` | Business rules only |
| `QuantityMeasurementCacheRepository` | Data storage only |
| `QuantityDTO` | Input shape only |

### Open/Closed (OCP)
New units are added by creating a new `IMeasurable` class — zero changes to existing code.

### Liskov Substitution (LSP)
All units implement `IMeasurable` and are fully substitutable. Temperature throws `NotSupportedException` on arithmetic (not a silent failure — explicit and meaningful).

### Interface Segregation (ISP)
- `IQuantityMeasurementService` — business methods only
- `IQuantityMeasurementRepository` — data access methods only
- No class is forced to implement unused members

### Dependency Inversion (DIP)
High-level modules depend on abstractions:
```csharp
// Controller depends on interface, not concrete class
private readonly IQuantityMeasurementService _service;

// Service depends on interface, not concrete repository
private readonly IQuantityMeasurementRepository _repository;
```

---

## 🔐 Authentication & Authorization

### JWT Token Management

**Token Generation:**
```csharp
var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    },
    expires: DateTime.UtcNow.AddMinutes(15),
    signingCredentials: credentials
);
```

**Validation Parameters:**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
    ClockSkew = TimeSpan.Zero
};
```

### Google OAuth 2.0 Integration

```csharp
[HttpGet("google/login")]
public IActionResult GoogleLogin()
{
    var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
              $"client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
              $"response_type=code&scope=openid%20email%20profile&" +
              $"access_type=offline&prompt=consent";
    return Redirect(url);
}

[HttpGet("google/callback")]
public async Task<IActionResult> GoogleCallback(string code, string? error)
{
    var tokenResponse = await ExchangeCodeForTokens(code);
    var userInfo     = await GetGoogleUserInfo(tokenResponse.access_token);
    // Create or retrieve user, generate JWT, redirect with tokens
}
```

### Refresh Token Mechanism

```csharp
private string GenerateRefreshToken()
{
    var randomNumber = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
}
```

**Token Rotation:** Each refresh call revokes the current token and issues a new pair.

### Role-Based Access Control (RBAC)

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpGet("users")]         public async Task<IActionResult> GetAllUsers() { }
    [HttpPut("users/{id}/role")]  public async Task<IActionResult> UpdateUserRole(...) { }
    [HttpPut("users/{id}/unlock")] public async Task<IActionResult> UnlockUser(...) { }
    [HttpGet("statistics")]    public async Task<IActionResult> GetUserStatistics() { }
}
```

### Account Lockout & Rate Limiting

```csharp
// Lockout after 5 failed attempts for 15 minutes
private const int MAX_FAILED_ATTEMPTS = 5;
private const int LOCKOUT_MINUTES = 15;

if (user.FailedLoginAttempts >= MAX_FAILED_ATTEMPTS)
    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES);
```

```csharp
// IP-based rate limiting on /Auth/login (10 requests / 60 seconds)
public class RateLimitingMiddleware
{
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clientRequests = new();
    private const int MAX_REQUESTS = 10;
    private const int TIME_WINDOW_SECONDS = 60;
    // ...
}
```

### Password Hashing with BCrypt

```csharp
// Registration
string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

// Login
bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
```

> **Work factor 12** = 2¹² = 4096 iterations (~250 ms per hash). Salt is auto-generated and embedded.

---

## 🛡️ Security Implementation

### Security Headers Middleware

```csharp
headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
headers["X-Frame-Options"]           = "DENY";
headers["X-Content-Type-Options"]    = "nosniff";
headers["X-XSS-Protection"]          = "1; mode=block";
headers["Referrer-Policy"]           = "strict-origin-when-cross-origin";
```

### CORS Configuration

```csharp
policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5500")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
```

### Secrets Management

```csharp
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT_KEY environment variable is not set");
```

---

## 📊 Database Schema & Migrations

### Users Table

```sql
CREATE TABLE Users (
    Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    Username            NVARCHAR(50)  NOT NULL UNIQUE,
    Email               NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash        NVARCHAR(MAX) NOT NULL,
    FirstName           NVARCHAR(50)  NOT NULL,
    LastName            NVARCHAR(50)  NOT NULL,
    CreatedAt           DATETIME2     NOT NULL,
    LastLoginAt         DATETIME2     NULL,
    IsActive            BIT           NOT NULL DEFAULT 1,
    Role                NVARCHAR(50)  NOT NULL DEFAULT 'User',
    FailedLoginAttempts INT           NOT NULL DEFAULT 0,
    LockoutEnd          DATETIME2     NULL
);
```

### RefreshTokens Table

```sql
CREATE TABLE RefreshTokens (
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId          BIGINT        NOT NULL,
    Token           NVARCHAR(200) NOT NULL UNIQUE,
    ExpiresAt       DATETIME2     NOT NULL,
    RevokedAt       DATETIME2     NULL,
    RevokedByIp     NVARCHAR(50)  NULL,
    ReplacedByToken NVARCHAR(200) NULL,
    CreatedByIp     NVARCHAR(50)  NULL,
    CreatedAt       DATETIME2     NOT NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

### QuantityMeasurements Table

```sql
CREATE TABLE QuantityMeasurements (
    Id                    BIGINT IDENTITY(1,1) PRIMARY KEY,
    MeasurementId         NVARCHAR(50)  NOT NULL,
    CreatedAt             DATETIME2     NOT NULL,
    OperationType         INT           NOT NULL,
    FirstOperandValue     FLOAT NULL, FirstOperandUnit NVARCHAR(20) NULL, FirstOperandCategory NVARCHAR(20) NULL,
    SecondOperandValue    FLOAT NULL, SecondOperandUnit NVARCHAR(20) NULL, SecondOperandCategory NVARCHAR(20) NULL,
    TargetUnit            NVARCHAR(20)  NULL,
    SourceOperandValue    FLOAT NULL, SourceOperandUnit NVARCHAR(20) NULL, SourceOperandCategory NVARCHAR(20) NULL,
    ResultValue           FLOAT NULL, ResultUnit NVARCHAR(20) NULL,
    FormattedResult       NVARCHAR(200) NULL,
    IsSuccessful          BIT           NOT NULL,
    ErrorDetails          NVARCHAR(MAX) NULL
);
```

### EF Core Migration Commands

```bash
# Create migration
dotnet ef migrations add InitialCreate \
  --context ApplicationDbContext \
  --startup-project QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj \
  --project QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj

# Apply migration
dotnet ef database update \
  --context ApplicationDbContext \
  --startup-project QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj \
  --project QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj
```

---

## 🔄 Data Flow & State Management

### Authentication Flow

```
CLIENT → POST /login → AuthController → AuthService → BCrypt.Verify → SaveRefreshToken → Return JWT
CLIENT ← 401         ← API            ← POST /refresh-token → Revoke old → Generate new → Return new pair
```

### Measurement Operation Flow

```
User Input
  → Presentation (validate format)
  → Service (validate rules, create domain objects)
  → Domain (GenericQuantity<T> arithmetic)
  → Repository (persist entity)
  → Response DTO
  → Display
```

### State Transitions

| State | Next States |
|-------|-------------|
| INIT | MAIN_MENU |
| MAIN_MENU | OPERATION_SELECT, EXIT |
| OPERATION_SELECT | INPUT_COLLECTION |
| INPUT_COLLECTION | VALIDATION, CANCELLED |
| VALIDATION | PROCESSING, ERROR |
| PROCESSING | RESULT, ERROR |
| RESULT | OPERATION_SELECT |
| ERROR | OPERATION_SELECT, RETRY |

---

## ✅ Validation System Architecture

### Three-Layer Validation

**Layer 1 — Presentation**
- Empty / whitespace check
- Numeric format check (`double.TryParse`)
- ESC / Backspace key handling

**Layer 2 — Business**
- Same category enforcement
- Unit existence check
- Division-by-zero guard (`|value| < 1e-9`)
- Temperature arithmetic rejection

**Layer 3 — Domain**
```csharp
private static void ValidateValue(double value)
{
    if (double.IsNaN(value) || double.IsInfinity(value))
        throw new InvalidValueException(value);
}
```

### Validation Rules Matrix

| Rule | Layer | Error Message |
|------|-------|---------------|
| Value not empty | Presentation | "Value cannot be empty." |
| Value is numeric | Presentation | "Invalid number." |
| Value is finite | Domain | "Value must be a finite number." |
| Category valid | Presentation | "Invalid category." |
| Unit valid | Presentation | "Invalid choice." |
| Unit exists | Business | "Invalid target unit: {unit}" |
| Same category | Business | "Category mismatch: {cat1} and {cat2}" |
| Not dividing by zero | Business | "Division by zero is not allowed." |
| Operation supported | Business | "Temperature units do not support {op}." |

---

## ⚠️ Exception Handling Strategy

### Exception Hierarchy

```
Exception
├── QuantityMeasurementException  (custom — business layer)
│   ├── OperationType property
│   └── Timestamp property
├── InvalidValueException         (custom — domain layer)
├── NotSupportedException         (temperature arithmetic)
└── DivideByZeroException
```

### Handling by Layer

| Layer | Strategy |
|-------|----------|
| Domain | Throw typed custom exceptions |
| Business | Catch domain exceptions → wrap in `QuantityMeasurementException` |
| Presentation | Catch all → display friendly message + retry prompt |

---

## 💾 Persistence Mechanism

### Architecture

```
In-Memory ConcurrentDictionary  ←→  quantity_data.json (auto-save / auto-load)
```

### Persistence Features

| Feature | Implementation |
|---------|----------------|
| Thread Safety | `ConcurrentDictionary` + `lock` on file writes |
| Atomic Writes | `File.WriteAllText` inside `lock (_fileLock)` |
| Auto-save | After every operation |
| Auto-load | On application start |
| Format | Indented JSON (`WriteIndented = true`) |

### Sample Stored Record

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2026-03-11T13:31:05.123Z",
  "operationType": 1,
  "quantity1Value": 12.0,
  "quantity1Unit": "FEET",
  "quantity1Category": "LENGTH",
  "formattedResult": "12 FEET ≠ 12 INCH",
  "isSuccess": true
}
```

---

## 🧪 Unit Testing Strategy

### Test Categories

| Suite | Coverage |
|-------|----------|
| `DomainTests` | GenericQuantity arithmetic, conversion, equality, edge cases |
| `UnitTests` | Each unit class (Length, Weight, Volume, Temperature) |
| `IntegrationTests` | End-to-end workflows including temperature |
| `ServiceTests` | Business service with mocked repository |
| `ArchitectureTests` | Layer independence / scalability |

### Sample Test Patterns

```csharp
// Arrange-Act-Assert
[TestMethod]
public void Add_Length_CrossUnit_ResultInFirstUnit()
{
    var feet   = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
    var inches = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);
    var result = feet.Add(inches);
    Assert.AreEqual(2.0, result.Value, 0.001);
    Assert.AreEqual(LengthUnit.FEET, result.Unit);
}

// Expected Exception
[TestMethod, ExpectedException(typeof(NotSupportedException))]
public void Add_Temperature_ThrowsNotSupportedException()
{
    var t1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
    var t2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
    t1.Add(t2);
}
```

---

## ⚡ Performance Considerations

| Technique | Details |
|-----------|---------|
| Lazy initialization | Repository created only on first access |
| ConcurrentDictionary | Thread-safe reads without explicit locks |
| Async service methods | `Task.Run` wrapping for future I/O scalability |
| In-memory cache | Sub-millisecond reads after initial load |

### Performance Metrics

| Operation | Avg Time |
|-----------|----------|
| Compare / Add / Subtract / Divide | < 1 ms |
| Repository Save (to disk) | ~10 ms |
| Repository Load (startup) | ~50 ms |
| Application total memory | < 50 MB |

---

## 🖥 User Interface Design (Console)

### Features

| Feature | Implementation |
|---------|----------------|
| ESC Cancel | Key detection at every prompt |
| Backspace | `StringBuilder`-based input loop |
| Color Coding | `Console.ForegroundColor` (Cyan=headers, Green=success, Red=errors) |
| Box Drawing | Unicode characters (`╔ ═ ║ ╚`) |
| Retry Prompt | After every error — `y/n` confirmation |

### Input Flow

```
Category (1–4) → Unit (1–N) → Value (numeric + backspace) → Confirm → Display Result
```

---

## 📚 API Endpoints Reference

### Authentication (`/api/v1/Auth`)

| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/register` | ❌ |
| POST | `/login` | ❌ |
| POST | `/refresh-token` | ❌ |
| POST | `/logout` | ✅ |
| GET  | `/profile` | ✅ |
| GET  | `/status` | ❌ |
| GET  | `/google/login` | ❌ |
| GET  | `/google/callback` | ❌ |

### Quantity Operations (`/api/v1/Quantities`)

| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/convert` | ✅ |
| POST | `/compare` | ✅ |
| POST | `/add` | ✅ |
| POST | `/subtract` | ✅ |
| POST | `/divide` | ✅ |
| GET  | `/history` | ✅ |
| GET  | `/history/category/{category}` | ✅ |
| GET  | `/history/range` | ✅ |
| GET  | `/history/errors` | ✅ |
| GET  | `/statistics` | ✅ |
| GET  | `/count/{operation}` | ✅ |

### Admin (`/api/v1/admin`) — Requires `Admin` role

| Method | Endpoint |
|--------|----------|
| GET | `/users` |
| GET | `/users/{id}` |
| PUT | `/users/{id}/role` |
| PUT | `/users/{id}/status` |
| PUT | `/users/{id}/unlock` |
| GET | `/statistics` |

---

## 📁 Project Structure

```
QuantityMeasurementApp/
│
├── QuantityMeasurementApp.sln
│
├── QuantityMeasurementApp/              # UC1–14 Core Domain
│   ├── Core/
│   │   ├── Abstractions/IMeasurable.cs
│   │   └── Exceptions/
│   ├── Domain/
│   │   ├── Quantities/GenericQuantity.cs
│   │   ├── Units/  (LengthUnit, WeightUnit, VolumeUnit, TemperatureUnit)
│   │   └── ValueObjects/ (Feet, Inch)
│   └── UI/
│       └── Menus/ (MainMenu, GenericLengthMenu, …)
│
├── QuantityMeasurementApp.Tests/
│   ├── DomainTests/
│   ├── UnitTests/
│   ├── IntegrationTests/
│   ├── ServiceTests/
│   └── ArchitectureTests/
│
├── QuantityMeasurementModelLayer/
│   ├── DTOs/  (QuantityDTO, QuantityResponse, AuthDTOs)
│   └── Entities/ (QuantityMeasurementEntity, UserEntity, RefreshTokenEntity)
│
├── QuantityMeasurementRepositoryLayer/
│   ├── Interface/ (IQuantityMeasurementRepository, IAuthRepository)
│   ├── Services/  (QuantityMeasurementCacheRepository)
│   └── Migrations/
│
├── QuantityMeasurementBusinessLayer/
│   ├── Interface/ (IQuantityMeasurementService, IAuthService)
│   ├── Services/  (QuantityMeasurementService, AuthService)
│   └── Exceptions/QuantityMeasurementException.cs
│
├── QuantityMeasurementConsole/          # Console UI (UC15)
│   ├── Factory/ServiceFactory.cs
│   └── Menus/ (MainConsoleMenu, MeasurementMenu)
│
└── QuantityMeasurementWebAPI/           # REST API
    ├── Controllers/ (AuthController, QuantitiesController, AdminController)
    ├── Middleware/ (RateLimitingMiddleware, SecurityHeadersMiddleware)
    └── Program.cs
```

---

## 💻 Technology Stack

| Category | Technology | Version | Purpose |
|----------|------------|---------|---------|
| Framework | .NET | 8.0 | Core runtime |
| Language | C# | 12 | Programming language |
| Authentication | JWT Bearer | 8.0 | Token-based auth |
| OAuth | Google OAuth 2.0 | — | Social login |
| Password Hashing | BCrypt.Net-Next | 4.0.3 | Secure password storage |
| ORM | Entity Framework Core | 8.0 | Database access |
| Database | SQL Server | 2022 | Data storage |
| API Docs | Swashbuckle | 6.5.0 | Swagger / OpenAPI |
| Logging | Serilog | 8.0.0 | Structured logging |
| Testing | NUnit + MSTest | 4.5.0 / 17.1.0 | Unit testing |
| Mocking | Moq | 4.20.0 | Test doubles |
| Serialization | System.Text.Json | 8.0.5 | JSON handling |
| IDE | Visual Studio 2022 | 17.0+ | Development |

---

## 🚀 How to Run & Build

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Git (optional)

### Console Application

```bash
git clone https://github.com/yourusername/QuantityMeasurementApp.git
cd QuantityMeasurementApp

dotnet restore
dotnet build

cd QuantityMeasurementConsole
dotnet run
```

### Web API

```bash
# 1. Set the JWT secret
# Windows CMD
setx JWT_KEY "YourSuperSecretKeyAtLeast32CharactersLong"
# PowerShell
$env:JWT_KEY="YourSuperSecretKeyAtLeast32CharactersLong"
# Linux / macOS
export JWT_KEY="YourSuperSecretKeyAtLeast32CharactersLong"

# 2. Update appsettings.json connection string
# "Server=localhost\\SQLEXPRESS;Database=QuantityMeasurementDB;Trusted_Connection=True;TrustServerCertificate=true"

# 3. Apply migrations
dotnet ef database update \
  --context ApplicationDbContext \
  --startup-project QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj \
  --project QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj

# 4. Run
cd QuantityMeasurementWebAPI
dotnet run --urls "http://localhost:5000"

# 5. Swagger UI → http://localhost:5000/swagger
```

### Run Tests

```bash
dotnet test
```

### Build Configurations

| Configuration | Command |
|---------------|---------|
| Debug | `dotnet build` |
| Release | `dotnet build -c Release` |
| Clean | `dotnet clean` |

### Publish Self-Contained

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true
# Linux
dotnet publish -c Release -r linux-x64 --self-contained true
# macOS
dotnet publish -c Release -r osx-x64 --self-contained true
```

### Docker (Optional)

```bash
docker build -t quantity-measurement-api -f QuantityMeasurementWebAPI/Dockerfile .
docker run -p 5000:80 -e JWT_KEY="your-secret-key" quantity-measurement-api
```

---

## 🔧 Troubleshooting Guide

| Issue | Error | Solution |
|-------|-------|----------|
| JWT Key missing | `JWT_KEY environment variable is not set` | Set the env var and restart terminal |
| DB connection failed | `Cannot open database` | Run `dotnet ef database update` or create DB manually |
| Google OAuth mismatch | `redirect_uri_mismatch` | Add `http://localhost:5000/api/v1/Auth/google/callback` in Google Cloud Console |
| CORS error | `No 'Access-Control-Allow-Origin'` | Add your frontend origin to the CORS policy |
| Token expired | `401 Unauthorized` | Call `POST /api/v1/Auth/refresh-token` with the refresh token |
| JSON file corrupted | Repository load failure | Delete `quantity_data.json` to reset the cache |
| ESC not working | Cannot cancel in console | Run in a proper terminal; use Ctrl+C as fallback |
| Test tolerance failures | Floating-point assertion errors | Increase `const double tolerance` value |

---

## 🗺 Future Roadmap

### Short-term (0–3 months)
- [ ] Password reset & email verification
- [ ] Two-factor authentication (2FA)
- [ ] API versioning (v2)
- [ ] SQLite persistence option for console app
- [ ] Export history to CSV / Excel

### Medium-term (3–12 months)
- [ ] Redis cache for refresh tokens
- [ ] Microservices split with Docker Compose
- [ ] GraphQL API alternative
- [ ] Rate limiting per user (not just IP)
- [ ] Blazor / MAUI front-end

### Long-term (1–2 years)
- [ ] Kubernetes deployment
- [ ] CI/CD pipeline with GitHub Actions
- [ ] OpenTelemetry performance monitoring
- [ ] Multi-tenancy support
- [ ] AI-based unit conversion suggestions

---

## 📊 Project Metrics Summary

| Metric | Value |
|--------|-------|
| Total Projects | 7 (core + tests + model + repo + business + console + WebAPI) |
| Total Files | 80+ |
| Lines of Code | 18,000+ |
| Test Cases | 300+ |
| Test Coverage | 90%+ |
| Use Cases Implemented | 15 |
| Supported Units | 14 |
| API Endpoints | 25+ |
| Design Patterns | 7 |
| SOLID Principles | All 5 |
| Architecture Layers | 5 |
| Build Time | ~15 seconds |
| Memory Footprint | < 50 MB |
| First Release | March 2026 |

---

## 👥 Contributors

- **Lead Developer / Architect / Testing / Documentation** — [Your Name]

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License — Copyright (c) 2026 Quantity Measurement System
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files, to deal in the Software
without restriction...
```

---

## 🙏 Acknowledgments

- **BridgeLabz** — Training curriculum and guidance
- **.NET Community** — Excellent documentation and tooling
- **Google** — OAuth 2.0 services
- **Open Source Contributors** — BCrypt.Net, Serilog, Moq, and other libraries

---

*Last updated: March 30, 2026 — Version 2.0.0*
````

---
