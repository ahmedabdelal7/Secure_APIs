# Login Endpoint: Verifying Passwords & Issuing JWT

## 1. Overview

The login endpoint verifies a student's email and password, then issues a signed JWT when the credentials are valid. It keeps authentication separate from Student CRUD logic and remains stateless: no server-side login session is stored.

## 2. Detailed Core Concepts

### Separate Authentication from Student CRUD

Authentication must not be implemented inside `StudentsController`:
- `StudentsController` handles student data and CRUD operations.
- `AuthController` handles login, authentication, and token issuance.

A suitable project structure is:

```text
StudentApi/
├── Controllers/
│   ├── StudentsController.cs
│   └── AuthController.cs
├── Models/
│   ├── Student.cs
│   └── LoginRequest.cs
├── DataSimulation/
│   └── StudentDataSimulation.cs
└── Program.cs
```

`AuthController` can later be extended to handle refresh tokens, logout, and external providers.

### Step 1: Create the Login Request Model

File: `Models/LoginRequest.cs`

```csharp
namespace StudentApi.Models
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
```

The password is sent for login verification only; it is not stored as plaintext or returned to the client.

### Step 2: Implement `AuthController`

File: `Controllers/AuthController.cs`

The endpoint is:

```http
POST /api/Auth/login
```

Its main steps are:

1. **Find the student:** Search `StudentDataSimulation.StudentsList` by email. Email acts as the unique login identifier.
2. **Handle unknown email:** If no student is found, return `401 Unauthorized` with `Invalid credentials`.
3. **Verify the password:** Use `BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash)`.
4. **Handle invalid password:** Return the same `401 Unauthorized` response. This avoids revealing whether the email or password was incorrect.
5. **Build claims:** Create identity claims for the JWT:
   - `ClaimTypes.NameIdentifier` — student ID.
   - `ClaimTypes.Email` — student email.
   - `ClaimTypes.Role` — role (Student or Admin).
6. **Create the signing key:** Build a `SymmetricSecurityKey` from the secret key.
7. **Define signing credentials:** Use `SigningCredentials` with `SecurityAlgorithms.HmacSha256`.
8. **Create the JWT:** Include issuer, audience, claims, expiration, and signing credentials.
9. **Return the token:** Serialize it with `JwtSecurityTokenHandler().WriteToken(token)`.

### JWT Settings Shown in the Lesson

| Setting | Example value |
|---|---|
| Issuer | `StudentApi` |
| Audience | `StudentApiUsers` |
| Signing algorithm | `HmacSha256` |
| Expiration | 30 minutes |
| Claims | Student ID, email, role |

The signing key used to create the token must match the key used by the JWT validation middleware.

### Step 3: Install Required NuGet Packages

Install these packages through **Manage NuGet Packages → Browse**:

- `System.IdentityModel.Tokens.Jwt`
- `Microsoft.IdentityModel.Tokens`

### Step 4: Understand the Login Flow

1. Student sends email and password.
2. API finds the student by email.
3. API verifies the password against the stored BCrypt hash.
4. API builds identity claims.
5. API creates and signs a JWT.
6. API returns the token to the client.

No session is stored; identity information travels inside the token.

### Request and Response Examples

**Request**

```http
POST /api/Auth/login
Content-Type: application/json
```

```json
{
  "email": "ali.ahmed@student.com",
  "password": "password1"
}
```

**Successful response**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6..."
}
```

### What the Login Endpoint Does Not Do

- It does not authorize actions.
- It does not protect existing endpoints yet.
- It does not check resource ownership.

Those responsibilities are covered in later lessons.

## 3. Quick-Reference Cheat Sheet

| Concept | Remember |
|---|---|
| Login endpoint | `POST /api/Auth/login` |
| Request model | `LoginRequest` with `Email` and `Password` |
| Controller | `AuthController`, separate from `StudentsController` |
| User lookup | Find student by email |
| Password verification | `BCrypt.Verify(plainPassword, storedHash)` |
| Invalid credentials | `401 Unauthorized` with a generic message |
| Identity claims | Student ID, email, role |
| Signing key | `SymmetricSecurityKey` |
| Signing algorithm | `HmacSha256` |
| Token creation | `JwtSecurityToken` |
| Token serialization | `JwtSecurityTokenHandler().WriteToken(token)` |
| Example expiration | 30 minutes |
| Session storage | None; authentication is stateless |
| Not included yet | Authorization, endpoint protection, ownership checks |
