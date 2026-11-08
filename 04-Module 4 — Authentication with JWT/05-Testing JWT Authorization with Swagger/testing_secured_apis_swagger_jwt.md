# Testing Secured APIs with Swagger (JWT Authorization)

## 1. Overview

Swagger can test JWT-protected endpoints, but it does not automatically understand or send JWTs unless JWT Bearer support is configured. Adding a Bearer security scheme enables the Swagger UI **Authorize** button, which sends the token in the `Authorization` header for subsequent protected requests.

## 2. Detailed Core Concepts

### Why the Authorize Button May Be Missing

JWT authentication can work correctly even when Swagger does not show an **Authorize** button.

- Swagger is a documentation and API testing tool.
- It does not automatically know that the API uses JWT.
- JWT support must be explicitly configured in Swagger.

A missing button does not mean the API's JWT security is disabled.

### What the Authorize Button Does

The Swagger **Authorize (🔒)** button:
- Adds the JWT to request headers.
- Automatically sends it with protected requests.
- Avoids manually entering the header for every request.

It only helps with testing; it does **not** change the API's security configuration.

### Step 1: Add the Required Namespace

In `Program.cs`:

```csharp
using Microsoft.OpenApi.Models;
```

### Step 2: Configure Swagger for JWT

Replace the existing `AddSwaggerGen()` configuration with:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
```

This tells Swagger:
- JWT Bearer authentication is used.
- The token is sent in the `Authorization` header.
- The expected format is `Bearer <token>`.

### Step 3: Restart the API

Swagger configuration changes require a full restart:

1. Stop the API.
2. Run it again.
3. Open Swagger UI.

After restarting, the **Authorize** button should appear near the top-right of Swagger UI. If it does not, refresh the page and check the Swagger configuration.

### Step 4: Get a JWT from the Login Endpoint

Call:

```http
POST /api/Auth/login
```

Example request body:

```json
{
  "email": "ali.ahmed@student.com",
  "password": "password1"
}
```

Copy the token from the response.

### Step 5: Authorize Swagger

1. Click **Authorize** (🔒).
2. Enter the token in this exact format:

```text
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...
```

3. Click **Authorize**.
4. Click **Close**.

**Important:** The word `Bearer`, followed by a space, is mandatory.

### Step 6: Test a Protected Endpoint

Call:

```http
GET /api/Students
```

Expected results:

| Request | Expected result |
|---|---|
| Without JWT | `401 Unauthorized` |
| With a valid JWT | `200 OK` and student data |

### What Swagger Does Behind the Scenes

Swagger automatically adds this header to requests:

```http
Authorization: Bearer <your-token>
```

The token is reused for subsequent requests from Swagger UI until authorization is cleared or the UI state is reset.

### Common Beginner Mistakes

- Forgetting the `Bearer` keyword or the space after it.
- Using an expired token.
- Not restarting the API after changing Swagger configuration.
- Thinking the Swagger Authorize button changes or provides API security.

### Scope of This Configuration

- It configures Swagger UI for testing.
- It does not change production API security.
- It allows a token to be reused across requests in Swagger.
- The API's JWT authentication middleware and `[Authorize]` attributes remain responsible for enforcing security.

## 3. Quick-Reference Cheat Sheet

| Item | Remember |
|---|---|
| Swagger purpose | Documentation and API testing |
| Why button is missing | Swagger has not been configured for JWT |
| Required namespace | `Microsoft.OpenApi.Models` |
| Security scheme name | `Bearer` |
| Header name | `Authorization` |
| Header format | `Bearer <token>` |
| Token source | `POST /api/Auth/login` |
| Restart | Required after Swagger configuration changes |
| Protected endpoint test | `GET /api/Students` |
| No token | `401 Unauthorized` |
| Valid token | `200 OK` (assuming no other authorization restriction) |
| Security impact | Swagger configuration only; does not replace API authentication middleware |
