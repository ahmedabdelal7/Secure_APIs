# Protecting Student Endpoints with JWT Authentication Middleware

## 1. Overview

JWT authentication middleware validates incoming bearer tokens and establishes the caller's identity before controller code runs. Registering the middleware and applying `[Authorize]` to `StudentsController` makes its endpoints require a valid JWT, while authorization decisions such as roles and ownership remain separate.

## 2. Detailed Core Concepts

### Login vs. Middleware: Two Different Responsibilities

- **Login endpoint:** Verifies credentials and issues a JWT.
- **Authentication middleware:** Enforces token validation on incoming requests.

For each request, the middleware:
1. Looks for a JWT.
2. Validates the token.
3. Extracts identity claims.
4. Rejects requests with missing or invalid tokens when authentication is required.

This happens before the controller code runs.

### Step 1: Install the Required Packages

Install:
- `Microsoft.AspNetCore.Authentication.JwtBearer` — select version `8.0.0` or the version matching your project.
- `System.IdentityModel.Tokens.Jwt`

These packages support reading JWTs, validating signatures, and extracting claims.

### Step 2: Configure JWT Authentication in `Program.cs`

Register JWT Bearer authentication and define token validation parameters:

```csharp
builder.Services.AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "StudentApi",
            ValidAudience = "StudentApiUsers",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "THIS_IS_A_VERY_SECRET_KEY_123456"))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
```

The validation settings mean:

| Setting | What it validates |
|---|---|
| `ValidateIssuer` | Token was issued by the expected issuer |
| `ValidateAudience` | Token is intended for this API |
| `ValidateLifetime` | Token has not expired |
| `ValidateIssuerSigningKey` | Token signature is valid |
| `ValidIssuer` | Expected issuer: `StudentApi` |
| `ValidAudience` | Expected audience: `StudentApiUsers` |
| `IssuerSigningKey` | Key must match the key used to sign the JWT |

### Step 3: Configure the Request Pipeline in the Correct Order

After building the app, the lesson's relevant pipeline is:

```csharp
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
```

- `UseAuthentication()` identifies the caller from the JWT.
- `UseAuthorization()` evaluates authorization requirements.
- Authentication must run **before** authorization.

The lesson also shows Swagger middleware inside the Development environment check:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### Step 4: Protect Student Endpoints with `[Authorize]`

In `Controllers/StudentsController.cs`:

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    // Protected endpoints
}
```

Applying `[Authorize]` at the controller level means every endpoint in `StudentsController` requires a valid JWT.

### What Happens When a Request Arrives?

**Without a JWT:**

```http
GET /api/Students
```

Expected result:

```text
401 Unauthorized
```

**With a valid JWT:**

```http
GET /api/Students
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

Expected result:

```text
200 OK
```

The middleware validates the token, extracts its claims, and allows the request to proceed.

### Accessing Identity Inside the Controller

After successful authentication, the controller can access the authenticated identity without manually parsing the JWT:

```csharp
User.Identity.IsAuthenticated
User.FindFirst(ClaimTypes.NameIdentifier)
User.FindFirst(ClaimTypes.Role)
```

These provide:
- Whether the current user is authenticated.
- The user identifier claim.
- The role claim.

### What JWT Authentication Does Not Do

JWT authentication middleware does **not**:
- Check roles by itself.
- Check resource ownership.
- Enforce business rules.

It answers: **“Is this request from a valid, authenticated user?”**

### Common Beginner Mistakes

- Forgetting `app.UseAuthentication()`.
- Putting `UseAuthorization()` before `UseAuthentication()`.
- Configuring token validation values incorrectly.
- Assuming `[Authorize]` automatically checks a specific role.

## 3. Quick-Reference Cheat Sheet

| Item | Remember |
|---|---|
| JWT login | Issues the token |
| Authentication middleware | Validates the token on incoming requests |
| Package | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| Package version | `8.0.0` or matching project version |
| Default scheme | `JwtBearerDefaults.AuthenticationScheme` |
| Validation | Issuer + audience + lifetime + signing key |
| Pipeline order | HTTPS redirection → authentication → authorization → controller mapping |
| Protect controller | `[Authorize]` |
| Missing/invalid token on protected endpoint | `401 Unauthorized` |
| Valid token (and no further authorization restriction) | Request proceeds; lesson example shows `200 OK` |
| Read identity | `User.Identity` and `User.FindFirst(...)` |
| Not handled here | Role checks, ownership, business rules |
