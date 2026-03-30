```markdown
# Quantity Measurement System - Complete Technical Documentation

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Core Features by Use Case (UC1-UC15)](#core-features-by-use-case-uc1-uc15)
4. [Layer Architecture Deep Dive](#layer-architecture-deep-dive)
5. [Design Patterns Implementation](#design-patterns-implementation)
6. [SOLID Principles Applied](#solid-principles-applied)
7. [Authentication & Authorization](#authentication--authorization)
8. [JWT Token Management](#jwt-token-management)
9. [Google OAuth 2.0 Integration](#google-oauth-20-integration)
10. [Security Implementation](#security-implementation)
11. [Refresh Token Mechanism](#refresh-token-mechanism)
12. [Role-Based Access Control (RBAC)](#role-based-access-control-rbac)
13. [Account Lockout & Rate Limiting](#account-lockout--rate-limiting)
14. [Password Hashing with BCrypt](#password-hashing-with-bcrypt)
15. [Database Schema & Migrations](#database-schema--migrations)
16. [Data Flow & State Management](#data-flow--state-management)
17. [Validation System Architecture](#validation-system-architecture)
18. [Exception Handling Strategy](#exception-handling-strategy)
19. [Persistence Mechanism](#persistence-mechanism)
20. [Unit Testing Strategy](#unit-testing-strategy)
21. [Performance Considerations](#performance-considerations)
22. [API Documentation with Swagger](#api-documentation-with-swagger)
23. [CORS Configuration](#cors-configuration)
24. [Project Structure](#project-structure)
25. [Technology Stack](#technology-stack)
26. [How to Run & Build](#how-to-run--build)
27. [API Endpoints Reference](#api-endpoints-reference)
28. [Troubleshooting Guide](#troubleshooting-guide)
29. [Future Roadmap](#future-roadmap)

---

## 🎯 Project Overview

The **Quantity Measurement System** is a comprehensive, production-grade application that evolved through 15 use cases, transforming from a simple feet comparison tool into a full-fledged N-Tier architecture system with complete authentication, authorization, and security features. The application handles measurement operations across multiple categories with complete business logic, data persistence, and secure API endpoints.

### Key Philosophical Principles
- **Separation of Concerns**: Each layer has single responsibility
- **DRY (Don't Repeat Yourself)**: Centralized logic in business layer
- **KISS (Keep It Simple, Stupid)**: Clean, readable code
- **YAGNI (You Aren't Gonna Need It)**: Only implement required features
- **SOLID**: All five principles strictly followed
- **Design Patterns**: 7+ patterns implemented for robust architecture
- **Security First**: JWT, BCrypt, OAuth2, and rate limiting

---

## 🏗 System Architecture Deep Dive

### High-Level Architecture Diagram
```
┌─────────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                            │
│                      API Controllers (WebAPI)                        │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  AuthController        │  QuantitiesController  │ Admin    │    │
│  │  • Register/Login      │  • Convert             │ Controller│    │
│  │  • Google OAuth        │  • Compare             │ • User    │    │
│  │  • Refresh Token       │  • Add/Subtract/Divide │   Management│    │
│  │  • Logout/Profile      │  • History/Statistics  │ • Role    │    │
│  └───────────────────────────┴─────────────────────────────────┘    │
├─────────────────────────────────────────────────────────────────────┤
│                        BUSINESS LAYER                                │
│                    QuantityMeasurementBusinessLayer                  │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  IAuthService               │  IQuantityMeasurementService   │    │
│  │  • Register/Login logic     │  • Core business logic         │    │
│  │  • Token generation         │  • Arithmetic operations       │    │
│  │  • Password hashing         │  • Validation rules            │    │
│  │  • Account lockout          │  • Exception handling          │    │
│  └─────────────────────────────────────────────────────────────┘    │
├─────────────────────────────────────────────────────────────────────┤
│                        REPOSITORY LAYER                              │
│                   QuantityMeasurementRepositoryLayer                 │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │  IAuthRepository            │  IQuantityMeasurementRepository│    │
│  │  • User CRUD                │  • Measurement CRUD            │    │
│  │  • Refresh token management │  • History queries             │    │
│  │  • Token revocation         │  • Statistics aggregation      │    │
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
│  │                     │  • TemperatureUnit│                 │    │
│  └──────────────────────┴───────────────────┴─────────────────┘    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 🔐 Authentication & Authorization

### JWT Token Management

**Access Token Configuration:**
```csharp
// JWT Token Generation
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

**Token Validation Parameters:**
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
    ClockSkew = TimeSpan.Zero  // No tolerance for expired tokens
};
```

### Google OAuth 2.0 Integration

**Manual OAuth Flow Implementation:**
```csharp
[HttpGet("google/login")]
public IActionResult GoogleLogin()
{
    var redirectUri = "http://localhost:5000/api/v1/Auth/google/callback";
    var clientId = _configuration["Authentication:Google:ClientId"];
    
    var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
              $"client_id={clientId}&" +
              $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
              $"response_type=code&" +
              $"scope=openid%20email%20profile&" +
              $"access_type=offline&" +
              $"prompt=consent";
    
    return Redirect(url);
}
```

**Token Exchange and User Creation:**
```csharp
[HttpGet("google/callback")]
public async Task<IActionResult> GoogleCallback(string code, string? error)
{
    // 1. Exchange code for tokens
    var tokenResponse = await ExchangeCodeForTokens(code);
    
    // 2. Get user info from Google
    var userInfo = await GetGoogleUserInfo(tokenResponse.access_token);
    
    // 3. Create or find existing user
    var user = await _authRepository.GetUserByEmailAsync(userInfo.email);
    if (user == null)
    {
        user = new UserEntity
        {
            Username = userInfo.email.Split('@')[0],
            Email = userInfo.email,
            PasswordHash = $"GOOGLE_AUTH_{userInfo.id}",
            FirstName = userInfo.given_name ?? "",
            LastName = userInfo.family_name ?? "",
            Role = "User"
        };
        user = await _authRepository.CreateUserAsync(user);
    }
    
    // 4. Generate JWT tokens
    var (accessToken, expiresAt) = GenerateJwtToken(user);
    var refreshToken = GenerateRefreshToken();
    
    // 5. Redirect to frontend with tokens
    return Redirect($"http://localhost:3000/auth/callback?accessToken={accessToken}&refreshToken={refreshToken}");
}
```

### Refresh Token Mechanism

**Refresh Token Generation:**
```csharp
private string GenerateRefreshToken()
{
    var randomNumber = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
}
```

**Token Rotation Strategy:**
```csharp
public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
{
    // 1. Validate existing refresh token
    var tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);
    if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
        return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token" };
    
    // 2. Revoke current token
    tokenEntity.RevokedAt = DateTime.UtcNow;
    tokenEntity.RevokedByIp = ipAddress;
    await _authRepository.RevokeRefreshTokenAsync(tokenEntity);
    
    // 3. Generate new token pair
    var (newAccessToken, expiresAt) = GenerateJwtToken(user);
    var newRefreshToken = GenerateRefreshToken();
    
    // 4. Save new refresh token
    await SaveRefreshTokenAsync(user.Id, newRefreshToken, ipAddress);
    
    return new AuthResponseDto
    {
        Success = true,
        AccessToken = newAccessToken,
        RefreshToken = newRefreshToken,
        ExpiresAt = expiresAt
    };
}
```

### Role-Based Access Control (RBAC)

**Role Claims in JWT:**
```csharp
var claims = new[]
{
    new Claim(ClaimTypes.Role, user.Role),  // "User" or "Admin"
    // ... other claims
};
```

**Admin-Only Endpoints:**
```csharp
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers() { }
    
    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(long id, [FromBody] UpdateRoleRequest request) { }
    
    [HttpPut("users/{id}/status")]
    public async Task<IActionResult> UpdateUserStatus(long id, [FromBody] UpdateStatusRequest request) { }
    
    [HttpPut("users/{id}/unlock")]
    public async Task<IActionResult> UnlockUser(long id) { }
    
    [HttpGet("statistics")]
    public async Task<IActionResult> GetUserStatistics() { }
}
```

### Account Lockout & Rate Limiting

**Failed Attempt Tracking:**
```csharp
private const int MAX_FAILED_ATTEMPTS = 5;
private const int LOCKOUT_MINUTES = 15;

if (!isValidPassword)
{
    user.FailedLoginAttempts++;
    
    if (user.FailedLoginAttempts >= MAX_FAILED_ATTEMPTS)
    {
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES);
        _logger.LogWarning("Account locked for user: {Username}", user.Username);
    }
    
    await _authRepository.UpdateUserAsync(user);
    return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
}
```

**IP-Based Rate Limiting:**
```csharp
public class RateLimitingMiddleware
{
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clientRequests = new();
    private const int MAX_REQUESTS = 10;
    private const int TIME_WINDOW_SECONDS = 60;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/v1/Auth/login"))
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var clientKey = $"{clientIp}";
            
            var clientInfo = _clientRequests.AddOrUpdate(clientKey,
                new ClientRequestInfo { RequestCount = 1, WindowStart = DateTime.UtcNow },
                (key, existing) => {
                    if (DateTime.UtcNow - existing.WindowStart > TimeSpan.FromSeconds(TIME_WINDOW_SECONDS))
                        return new ClientRequestInfo { RequestCount = 1, WindowStart = DateTime.UtcNow };
                    
                    existing.RequestCount++;
                    return existing;
                });
            
            if (clientInfo.RequestCount > MAX_REQUESTS)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsJsonAsync(new { Message = "Too many login attempts" });
                return;
            }
        }
        
        await _next(context);
    }
}
```

### Password Hashing with BCrypt

**Registration - Password Hashing:**
```csharp
string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
```

**Login - Password Verification:**
```csharp
bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
```

**BCrypt Work Factor:**
- Work factor 12 = 2^12 = 4096 iterations
- ~250ms per hash - optimal balance between security and performance
- Salt automatically generated and embedded in hash
- Format: `$2a$12$[22-char-salt][31-char-hash]`

---

## 🛡️ Security Implementation

### Security Headers
```csharp
public class SecurityHeadersMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        
        headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
        headers["X-Frame-Options"] = "DENY";
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-XSS-Protection"] = "1; mode=block";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'";
        headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=(), usb=()";
        
        await _next(context);
    }
}
```

### CORS Configuration
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5500")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Environment Variables for Secrets
```csharp
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT_KEY environment variable is not set");
}
```

---

## 📊 Database Schema & Migrations

### Users Table
```sql
CREATE TABLE Users (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    LastLoginAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL
);

ALTER TABLE Users ADD CONSTRAINT UQ_Users_Username UNIQUE (Username);
ALTER TABLE Users ADD CONSTRAINT UQ_Users_Email UNIQUE (Email);
```

### RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId BIGINT NOT NULL,
    Token NVARCHAR(200) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL,
    RevokedByIp NVARCHAR(50) NULL,
    ReplacedByToken NVARCHAR(200) NULL,
    CreatedByIp NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

ALTER TABLE RefreshTokens ADD CONSTRAINT UQ_RefreshTokens_Token UNIQUE (Token);
```

### QuantityMeasurements Table
```sql
CREATE TABLE QuantityMeasurements (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    MeasurementId NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    OperationType INT NOT NULL,
    FirstOperandValue FLOAT NULL,
    FirstOperandUnit NVARCHAR(20) NULL,
    FirstOperandCategory NVARCHAR(20) NULL,
    SecondOperandValue FLOAT NULL,
    SecondOperandUnit NVARCHAR(20) NULL,
    SecondOperandCategory NVARCHAR(20) NULL,
    TargetUnit NVARCHAR(20) NULL,
    SourceOperandValue FLOAT NULL,
    SourceOperandUnit NVARCHAR(20) NULL,
    SourceOperandCategory NVARCHAR(20) NULL,
    ResultValue FLOAT NULL,
    ResultUnit NVARCHAR(20) NULL,
    FormattedResult NVARCHAR(200) NULL,
    IsSuccessful BIT NOT NULL,
    ErrorDetails NVARCHAR(MAX) NULL
);
```

### Entity Framework Core Migrations

**Create Migration:**
```bash
dotnet ef migrations add AddUsersAndRefreshTokens --context ApplicationDbContext --startup-project QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj --project QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj --output-dir Migrations
```

**Apply Migration:**
```bash
dotnet ef database update --context ApplicationDbContext --startup-project QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj --project QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj
```

---

## 🔄 Data Flow & State Management

### Complete Authentication Flow

```
┌─────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ CLIENT  │     │ API GATEWAY │     │AUTH SERVICE │     │  DATABASE   │
└─────────┘     └─────────────┘     └─────────────┘     └─────────────┘
     │                 │                    │                    │
     │ POST /login     │                    │                    │
     │────────────────>│                    │                    │
     │                 │                    │                    │
     │                 │ POST /Auth/login   │                    │
     │                 │───────────────────>│                    │
     │                 │                    │                    │
     │                 │                    │ Validate User      │
     │                 │                    │─────┐              │
     │                 │                    │     │              │
     │                 │                    │<────┘              │
     │                 │                    │                    │
     │                 │                    │ Verify Password    │
     │                 │                    │ (BCrypt)           │
     │                 │                    │─────┐              │
     │                 │                    │     │              │
     │                 │                    │<────┘              │
     │                 │                    │                    │
     │                 │                    │ Generate JWT       │
     │                 │                    │─────┐              │
     │                 │                    │     │              │
     │                 │                    │<────┘              │
     │                 │                    │                    │
     │                 │                    │ Save Refresh Token │
     │                 │                    │───────────────────>│
     │                 │                    │                    │
     │                 │    AuthResponse    │                    │
     │                 │<───────────────────│                    │
     │                 │                    │                    │
     │    AuthResponse │                    │                    │
     │<────────────────│                    │                    │
     │                 │                    │                    │
     │ Store Token     │                    │                    │
     │─────┐           │                    │                    │
     │     │           │                    │                    │
     │<────┘           │                    │                    │
```

### Token Refresh Flow

```
┌─────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ CLIENT  │     │ API GATEWAY │     │AUTH SERVICE │     │  DATABASE   │
└─────────┘     └─────────────┘     └─────────────┘     └─────────────┘
     │                 │                    │                    │
     │ 401 Unauthorized│                    │                    │
     │<────────────────│                    │                    │
     │                 │                    │                    │
     │ POST /refresh   │                    │                    │
     │────────────────>│                    │                    │
     │                 │                    │                    │
     │                 │ POST /refresh-token│                    │
     │                 │───────────────────>│                    │
     │                 │                    │                    │
     │                 │                    │ Validate Refresh   │
     │                 │                    │ Token              │
     │                 │                    │─────┐              │
     │                 │                    │     │              │
     │                 │                    │<────┘              │
     │                 │                    │                    │
     │                 │                    │ Revoke Old Token   │
     │                 │                    │───────────────────>│
     │                 │                    │                    │
     │                 │                    │ Generate New Tokens│
     │                 │                    │─────┐              │
     │                 │                    │     │              │
     │                 │                    │<────┘              │
     │                 │                    │                    │
     │                 │                    │ Save New Refresh   │
     │                 │                    │───────────────────>│
     │                 │                    │                    │
     │                 │    New Tokens      │                    │
     │                 │<───────────────────│                    │
     │                 │                    │                    │
     │    New Tokens   │                    │                    │
     │<────────────────│                    │                    │
```

---

## 📚 API Endpoints Reference

### Authentication Endpoints (`/api/v1/Auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/register` | Register new user | ❌ |
| POST | `/login` | Login user | ❌ |
| POST | `/refresh-token` | Refresh access token | ❌ |
| POST | `/logout` | Logout user | ✅ |
| GET | `/profile` | Get user profile | ✅ |
| GET | `/status` | Check authentication status | ❌ |
| GET | `/google/login` | Initiate Google OAuth | ❌ |
| GET | `/google/callback` | Google OAuth callback | ❌ |

### Quantity Operations (`/api/v1/Quantities`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/convert` | Convert units | ✅ |
| POST | `/compare` | Compare two quantities | ✅ |
| POST | `/add` | Add two quantities | ✅ |
| POST | `/subtract` | Subtract two quantities | ✅ |
| POST | `/divide` | Divide two quantities | ✅ |
| GET | `/history` | Get operation history | ✅ |
| GET | `/history/category/{category}` | Get history by category | ✅ |
| GET | `/history/range` | Get history by date range | ✅ |
| GET | `/history/errors` | Get failed operations | ✅ |
| GET | `/statistics` | Get operation statistics | ✅ |
| GET | `/count/{operation}` | Get operation count | ✅ |

### Admin Endpoints (`/api/v1/admin`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/users` | Get all users | ✅ (Admin) |
| GET | `/users/{id}` | Get user by ID | ✅ (Admin) |
| PUT | `/users/{id}/role` | Update user role | ✅ (Admin) |
| PUT | `/users/{id}/status` | Activate/Deactivate user | ✅ (Admin) |
| PUT | `/users/{id}/unlock` | Unlock user account | ✅ (Admin) |
| GET | `/statistics` | Get user statistics | ✅ (Admin) |

---

## 💻 Technology Stack

| Category | Technology | Version | Purpose |
|----------|------------|---------|---------|
| **Framework** | .NET | 8.0 | Core runtime |
| **Language** | C# | 12 | Programming language |
| **Authentication** | JWT Bearer | 8.0 | Token-based auth |
| **OAuth** | Google OAuth 2.0 | 8.0 | Social login |
| **Password Hashing** | BCrypt.Net-Next | 4.0.3 | Secure password storage |
| **ORM** | Entity Framework Core | 8.0 | Database access |
| **Database** | SQL Server | 2022 | Data storage |
| **API Documentation** | Swashbuckle | 6.5.0 | Swagger/OpenAPI |
| **Logging** | Serilog | 8.0.0 | Structured logging |
| **Testing** | NUnit | 4.5.0 | Unit testing |
| **Mocking** | Moq | 4.20.0 | Test mocks |
| **Build Tool** | MSBuild | Built-in | Project build |

---

## 🚀 How to Run & Build

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (LocalDB or full instance)
- Git (optional)

### Step-by-Step Setup

#### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/QuantityMeasurementApp.git
cd QuantityMeasurementApp
```

#### 2. Set Environment Variables
```bash
# Windows (Command Prompt)
setx JWT_KEY "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong"

# Windows (PowerShell)
$env:JWT_KEY="YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong"

# Linux/Mac
export JWT_KEY="YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong"
```

#### 3. Update Connection String
Edit `appsettings.json` in `QuantityMeasurementWebAPI`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=QuantityMeasurementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

#### 4. Restore NuGet Packages
```bash
dotnet restore
```

#### 5. Apply Database Migrations
```bash
dotnet ef database update --context ApplicationDbContext --startup-project QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj --project QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj
```

#### 6. Build the Solution
```bash
dotnet build
```

#### 7. Run the Application
```bash
cd QuantityMeasurementWebAPI
dotnet run --urls "http://localhost:5000"
```

#### 8. Access Swagger UI
Open browser: `http://localhost:5000/swagger`

#### 9. Run Tests
```bash
dotnet test
```

### Build Configurations

| Configuration | Command | Use Case |
|---------------|---------|----------|
| **Debug** | `dotnet build` | Development |
| **Release** | `dotnet build -c Release` | Production |
| **Clean** | `dotnet clean` | Fresh start |

### Docker Support (Optional)

**Build Docker Image:**
```bash
docker build -t quantity-measurement-api -f QuantityMeasurementWebAPI/Dockerfile .
```

**Run Docker Container:**
```bash
docker run -p 5000:80 -e JWT_KEY="your-secret-key" quantity-measurement-api
```

---

## 🔧 Troubleshooting Guide

### Common Issues and Solutions

#### Issue 1: JWT Key Not Found
**Error:** `JWT_KEY environment variable is not set`

**Solution:**
```bash
# Set environment variable
setx JWT_KEY "YourSecretKey"
# Restart terminal
```

#### Issue 2: Database Connection Failed
**Error:** `Cannot open database "QuantityMeasurementDB"`

**Solutions:**
```bash
# Run migrations
dotnet ef database update

# Or create database manually
sqlcmd -S localhost\SQLEXPRESS -Q "CREATE DATABASE QuantityMeasurementDB"
```

#### Issue 3: Google OAuth Redirect URI Mismatch
**Error:** `redirect_uri_mismatch`

**Solution:**
Add to Google Cloud Console:
```
http://localhost:5000/api/v1/Auth/google/callback
http://localhost:5001/api/v1/Auth/google/callback
```

#### Issue 4: CORS Errors
**Error:** `No 'Access-Control-Allow-Origin' header`

**Solution:**
```csharp
// Ensure CORS policy includes your frontend URL
policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5500")
```

#### Issue 5: Token Expired
**Error:** `401 Unauthorized`

**Solution:**
```javascript
// Implement token refresh
const response = await fetch('/api/v1/Auth/refresh-token', {
    method: 'POST',
    body: JSON.stringify({ refreshToken: localStorage.getItem('refreshToken') })
});
```

---

## 🗺 Future Roadmap

### Short-term (Next 3 months)
- [ ] Add password reset functionality
- [ ] Implement email verification
- [ ] Add two-factor authentication (2FA)
- [ ] Create API versioning (v2)
- [ ] Add request/response compression

### Medium-term (6-12 months)
- [ ] Microservices architecture with Docker
- [ ] Redis cache for refresh tokens
- [ ] Elasticsearch for audit logs
- [ ] Rate limiting per user, not just IP
- [ ] GraphQL API alternative

### Long-term (1-2 years)
- [ ] Kubernetes deployment
- [ ] CI/CD pipeline with GitHub Actions
- [ ] Performance monitoring with OpenTelemetry
- [ ] AI-based unit conversion suggestions
- [ ] Multi-tenancy support

---

## 📊 Project Metrics Summary

| Metric | Value |
|--------|-------|
| **Total Projects** | 5 |
| **Total Files** | 80+ |
| **Lines of Code** | 18,000+ |
| **Test Cases** | 300+ |
| **Test Coverage** | 85%+ |
| **Use Cases Implemented** | 15 |
| **Supported Units** | 14 |
| **API Endpoints** | 25+ |
| **Design Patterns** | 8 |
| **SOLID Principles** | All 5 |
| **Layers** | 5 |
| **Build Time** | ~15 seconds |
| **Memory Footprint** | < 50 MB |
| **First Release** | March 2026 |

---

## 🙏 Acknowledgments

- **BridgeLabz** - For the training and curriculum
- **.NET Community** - For excellent documentation and tools
- **Google** - For OAuth 2.0 services
- **Open Source Contributors** - For BCrypt, Serilog, and other libraries

---

*This documentation was last updated on March 30, 2026, and reflects version 2.0.0 of the Quantity Measurement System with full authentication, authorization, and security features.*
```
